using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProcessDowntimeWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProcessDowntimeWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string ReadJson(string attribute)
        {
            string Link = null;
            using (StreamReader r = new StreamReader("piconfig.json"))
            {
                string json = r.ReadToEnd();
                var data = (JArray)JObject.Parse(json)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                foreach (var item in dynJson)
                {
                    if (item.Name == attribute)
                    {
                        if (attribute == "PiWebApiServerMachine")
                        {
                            Link = item.HostName;
                        }
                        else
                        {
                            Link = item.WebId;
                        }
                    }
                }
            }
            return Link;
        }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;           
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Submit(IFormCollection fc)
        {
            ViewBag.Id = fc["Id"];
            ViewBag.Name = fc["Name"];
            return View("Downtime");
        }
        
        [HttpPost]
        public async Task<ActionResult> eventframe(IFormCollection input)
        {
            string WebId = ReadJson("AssetDatabase");
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            var tempname = input["TemplateName"];
             try
            {
                
                string st = '"' + input["FromDate"].ToString().Replace("T", " ") + '"';
                string et = '"' + input["ToDate"].ToString().Replace("T", " ") + '"';
              
                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/eventframes/search?databaseWebId="+ WebId + "&query=Template:=" + tempname + "%20Start:>=" + st + "%20Start:<" + et + "&maxCount=1000";

                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                //var data = JObject.Parse(content);
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                List<Event> _evt = new List<Event>();
                string st_string;
                string st_string_1;
                string et_string;
                string et_string_1;
                string TimeDiff;
                DateTime st_calc;
                DateTime et_calc;
                DateTime frame_start;
                DateTime frame_end;
                var downtimepercent = 0;
                var downtimepercentstart = 0;

                foreach (var item in dynJson)
                {
                    ////////
                    string asset = "";
                    string url2 = item.Links.ReferencedElements.ToString();

                    HttpResponseMessage response2 = await client.GetAsync(url2);
                    string content2 = await response2.Content.ReadAsStringAsync();
                    if (!response2.IsSuccessStatusCode)
                    {
                        var responseMessage2 = "Response status code does not indicate success: ";
                        throw new HttpRequestException(responseMessage2 + Environment.NewLine + content2);
                    }
                    var data2 = (JArray)JObject.Parse(content2)["Items"];
                    dynamic dynJson2 = JsonConvert.DeserializeObject(data2.ToString());
                    foreach (var item2 in dynJson2)
                    {
                        asset = item2.Name.ToString();

                    }
                    string AcknowledgedDate = "";
                    string AcknowledgedBy = "";
                    string Description_One = "";
                    string Description_Two = "";
                    string Description_Three = "";
                    string url3 = item.Links.Value.ToString();

                    HttpResponseMessage response3 = await client.GetAsync(url3);
                    string content3 = await response3.Content.ReadAsStringAsync();
                    if (!response3.IsSuccessStatusCode)
                    {
                        var responseMessage3 = "Response status code does not indicate success: ";
                        throw new HttpRequestException(responseMessage3 + Environment.NewLine + content2);
                    }
                    var data3 = (JArray)JObject.Parse(content3)["Items"];
                    dynamic dynJson3 = JsonConvert.DeserializeObject(data3.ToString());
                    foreach (var item3 in dynJson3)
                    {
                        if(item3.Name == "AcknowledgedBy")
                        {
                            string Ack = item3.Value.Value.ToString();
                            string Tmp = item3.Value.Timestamp.ToString();
                            if (Ack.Length<=2)
                            {
                                AcknowledgedBy = null;
                                AcknowledgedDate = null;
                            }
                            else
                            {
                                AcknowledgedBy = Ack;
                                AcknowledgedDate = Tmp;
                            }
                        }
                        if (item3.Name == "1_Description")
                        {
                            Description_One = item3.Value.Value.ToString();
                        }
                        if (item3.Name == "2_Description")
                        {
                            Description_Two = item3.Value.Value.ToString();
                        }
                        if (item3.Name == "3_Description")
                        {
                            Description_Three = item3.Value.Value.ToString();
                        }
                    }
                    ///////////
                    st_string = item.StartTime.ToString();
                    et_string = item.EndTime.ToString();
                    
                    st_string_1 = st_string.Replace("T", " ").ToString();
                    et_string_1 = et_string.Replace("T", " ").ToString();

                    if(et_string_1.Contains("9999"))
                    {
                        st_calc = DateTime.Parse(st_string_1);
                        et_calc = DateTime.Parse(DateTime.Now.ToString());//DateTime.Now.ToString() when 9999;
                        System.TimeSpan diff = et_calc.Subtract(st_calc);
                        TimeDiff = diff.ToString() + " (in progress...)";

                        //gantt calculation
                        string frame_start_string = input["FromDate"].ToString().Replace("T", " ");
                        string frame_end_string = input["ToDate"].ToString().Replace("T", " ");
                        frame_start = DateTime.Parse(frame_start_string);
                        frame_end = DateTime.Parse(frame_end_string);

                        System.TimeSpan Diff_frame = frame_end.Subtract(frame_start);
                        System.TimeSpan Diff_frame_start = frame_start.Subtract(st_calc.AddHours(3));

                        downtimepercentstart = 0;
                        downtimepercent = Math.Abs(Convert.ToInt32(diff.TotalSeconds) * 100 / Convert.ToInt32(Diff_frame.TotalSeconds));
                        downtimepercentstart = Math.Abs(Convert.ToInt32(Diff_frame_start.TotalSeconds) * 100 / Convert.ToInt32(Diff_frame.TotalSeconds));
                        //
                        _evt.Add(new Event()
                        {
                            WebId = item.WebId,
                            StartTime = st_calc.AddHours(3),
                            EndTime = et_calc,
                            Duration = TimeDiff,
                            AcknowledgedBy = AcknowledgedBy,
                            AcknowledgedDate = AcknowledgedDate,
                            PrimaryReferencedElement = asset,
                            Description_One = Description_One,
                            Description_Two = Description_Two,
                            Description_Three = Description_Three,
                            downtimepercent = downtimepercent,
                            downtimepercentstart = downtimepercentstart
                        }); ;
                    }
                    else
                    {
                        st_calc = DateTime.Parse(st_string_1);
                        et_calc = DateTime.Parse(et_string_1);
                        System.TimeSpan diff = et_calc.Subtract(st_calc);
                        TimeDiff = diff.ToString();
                        //gantt calculation
                        string frame_start_string = input["FromDate"].ToString().Replace("T", " ");
                        string frame_end_string = input["ToDate"].ToString().Replace("T", " ");
                        frame_start = DateTime.Parse(frame_start_string);
                        frame_end = DateTime.Parse(frame_end_string);

                        System.TimeSpan Diff_frame = frame_end.Subtract(frame_start);
                        System.TimeSpan Diff_frame_start = frame_start.Subtract(st_calc.AddHours(3));

                        downtimepercentstart = 0;
                        downtimepercent = Math.Abs(Convert.ToInt32(diff.TotalSeconds) * 100 / Convert.ToInt32(Diff_frame.TotalSeconds));
                        downtimepercentstart = Math.Abs(Convert.ToInt32(Diff_frame_start.TotalSeconds) * 100 / Convert.ToInt32(Diff_frame.TotalSeconds));
                        //
                        _evt.Add(new Event()
                        {
                            WebId = item.WebId,
                            StartTime = st_calc.AddHours(3),
                            EndTime = et_calc.AddHours(3),
                            Duration = TimeDiff,
                            AcknowledgedBy = AcknowledgedBy,
                            AcknowledgedDate = AcknowledgedDate,
                            PrimaryReferencedElement = asset,
                            Description_One = Description_One,
                            Description_Two = Description_Two,
                            Description_Three = Description_Three,
                            downtimepercent = downtimepercent,
                            downtimepercentstart = downtimepercentstart
                        });;
                    }                    
                }
                ViewData["json"] = _evt;
                ViewBag.StartTime = input["FromDate"];
                ViewBag.EndTime = input["ToDate"];              
                return View("Downtime");
            }
            catch (Exception)
            {
                return View("Downtime");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EventDetails(IFormCollection input)
        {  
            string Input = null;
            string WebId = null;
            string StartTime = null;
            string EndTime = null;
            string Duration = null;
            string ReferencedElement = null;
            string[] InputArray;
            Input = input["WebId"].ToString();
            InputArray = Input.Split('|');
            StartTime = InputArray[0];
            EndTime = InputArray[1];
            ReferencedElement = InputArray[2];
            WebId = InputArray[3];
            Duration = InputArray[4];
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");

            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            try
            {
                string url = "https://" + PiWebApiServerMachine + "/piwebapi/streamsets/" + WebId + "/value";
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                List<EventDescription> _evtdesc = new List<EventDescription>();
                string d1 = null;
                string d2 = null;
                string d3 = null;
                string d1WebId = null;
                string d2WebId = null;
                string d3WebId = null;
                string AcknowledgedBy = null;
                string AcknowledgedDate = null;
                string AcknowledgedWebId = null;
                ///
                string One_Additional_Info = null;
                string One_Crew = null;
                string One_Description = null;
                string One_EndTime = null;
                string One_Equipment_Path = null;
                string One_Equipment_Tag = null;
                string One_Field = null;
                string One_Schedule = null;
                string One_Scope = null;
                string One_StartTime = null;
                string Two_Additional_Info = null;
                string Two_Crew = null;
                string Two_Description = null;
                string Two_EndTime = null;
                string Two_Equipment_Path = null;
                string Two_Equipment_Tag = null;
                string Two_Field = null;
                string Two_Schedule = null;
                string Two_Scope = null;
                string Two_StartTime = null;
                string Three_Additional_Info = null;
                string Three_Crew = null;
                string Three_Description = null;
                string Three_EndTime = null;
                string Three_Equipment_Path = null;
                string Three_Equipment_Tag = null;
                string Three_Field = null;
                string Three_Schedule = null;
                string Three_Scope = null;
                string Three_StartTime = null;
                string One_Additional_InfowebId = null;
                string One_CrewwebId = null;
                string One_DescriptionwebId = null;
                string One_EndTimewebId = null;
                string One_Equipment_PathwebId = null;
                string One_Equipment_TagwebId = null;
                string One_FieldwebId = null;
                string One_SchedulewebId = null;
                string One_ScopewebId = null;
                string One_StartTimewebId = null;
                string Two_Additional_InfowebId = null;
                string Two_CrewwebId = null;
                string Two_DescriptionwebId = null;
                string Two_EndTimewebId = null;
                string Two_Equipment_PathwebId = null;
                string Two_Equipment_TagwebId = null;
                string Two_FieldwebId = null;
                string Two_SchedulewebId = null;
                string Two_ScopewebId = null;
                string Two_StartTimewebId = null;
                string Three_Additional_InfowebId = null;
                string Three_CrewwebId = null;
                string Three_DescriptionwebId = null;
                string Three_EndTimewebId = null;
                string Three_Equipment_PathwebId = null;
                string Three_Equipment_TagwebId = null;
                string Three_FieldwebId = null;
                string Three_SchedulewebId = null;
                string Three_ScopewebId = null;
                string Three_StartTimewebId = null;
                string Percentage = null;
                string PercentagewebId = null;
                ///
                foreach (var item in dynJson)
                {
                    if (item.Name == "Description 1"){d1 = item.Value.Value;d1WebId = item.WebId;}
                    if (item.Name == "Description 2"){d2 = item.Value.Value;d2WebId = item.WebId;}
                    if (item.Name == "Description 3"){d3 = item.Value.Value;d3WebId = item.WebId;}
                    if (item.Name == "AcknowledgedBy"){AcknowledgedBy = item.Value.Value;AcknowledgedDate = item.Value.Timestamp;AcknowledgedWebId = item.WebId;}
                    if (item.Name == "Percentage"){ Percentage = item.Value.Value; PercentagewebId = item.WebId; }
                    //
                    if (item.Name == "1_Additional_Info") { One_Additional_Info = item.Value.Value; One_Additional_InfowebId = item.WebId; }
                    if (item.Name == "1_Crew") { One_Crew = item.Value.Value; One_CrewwebId = item.WebId; }
                    if (item.Name == "1_Description") { One_Description = item.Value.Value; One_DescriptionwebId = item.WebId; }
                    if (item.Name == "1_EndTime") { One_EndTime = item.Value.Value; One_EndTimewebId = item.WebId; }
                    if (item.Name == "1_Equipment_Path") { One_Equipment_Path = item.Value.Value; One_Equipment_PathwebId = item.WebId; }
                    if (item.Name == "1_Equipment_Tag") { One_Equipment_Tag = item.Value.Value; One_Equipment_TagwebId = item.WebId; }
                    if (item.Name == "1_Field") { One_Field = item.Value.Value; One_FieldwebId = item.WebId; }
                    if (item.Name == "1_Schedule") { One_Schedule = item.Value.Value; One_SchedulewebId = item.WebId; }
                    if (item.Name == "1_Scope") { One_Scope = item.Value.Value; One_ScopewebId = item.WebId; }
                    if (item.Name == "1_StartTime") { One_StartTime = item.Value.Value; One_StartTimewebId = item.WebId; }
                    //
                    if (item.Name == "2_Additional_Info") { Two_Additional_Info = item.Value.Value; Two_Additional_InfowebId = item.WebId; }
                    if (item.Name == "2_Crew") { Two_Crew = item.Value.Value; Two_CrewwebId = item.WebId; }
                    if (item.Name == "2_Description") { Two_Description = item.Value.Value; Two_DescriptionwebId = item.WebId; }
                    if (item.Name == "2_EndTime") { Two_EndTime = item.Value.Value; Two_EndTimewebId = item.WebId; }
                    if (item.Name == "2_Equipment_Path") { Two_Equipment_Path = item.Value.Value; Two_Equipment_PathwebId = item.WebId; }
                    if (item.Name == "2_Equipment_Tag") { Two_Equipment_Tag = item.Value.Value; Two_Equipment_TagwebId = item.WebId; }
                    if (item.Name == "2_Field") { Two_Field = item.Value.Value; Two_FieldwebId = item.WebId; }
                    if (item.Name == "2_Schedule") { Two_Schedule = item.Value.Value; Two_SchedulewebId = item.WebId; }
                    if (item.Name == "2_Scope") { Two_Scope = item.Value.Value; Two_ScopewebId = item.WebId; }
                    if (item.Name == "2_StartTime") { Two_StartTime = item.Value.Value; Two_StartTimewebId = item.WebId; }
                    //
                    if (item.Name == "3_Additional_Info") { Three_Additional_Info = item.Value.Value; Three_Additional_InfowebId = item.WebId; }
                    if (item.Name == "3_Crew") { Three_Crew = item.Value.Value; Three_CrewwebId = item.WebId; }
                    if (item.Name == "3_Description") { Three_Description = item.Value.Value; Three_DescriptionwebId = item.WebId; }
                    if (item.Name == "3_EndTime") { Three_EndTime = item.Value.Value; Three_EndTimewebId = item.WebId; }
                    if (item.Name == "3_Equipment_Path") { Three_Equipment_Path = item.Value.Value; Three_Equipment_PathwebId = item.WebId; }
                    if (item.Name == "3_Equipment_Tag") { Three_Equipment_Tag = item.Value.Value; Three_Equipment_TagwebId = item.WebId; }
                    if (item.Name == "3_Field") { Three_Field = item.Value.Value; Three_FieldwebId = item.WebId; }
                    if (item.Name == "3_Schedule") { Three_Schedule = item.Value.Value; Three_SchedulewebId = item.WebId; }
                    if (item.Name == "3_Scope") { Three_Scope = item.Value.Value; Three_ScopewebId = item.WebId; }
                    if (item.Name == "3_StartTime") { Three_StartTime = item.Value.Value; Three_StartTimewebId = item.WebId; }
                }
                _evtdesc.Add(new EventDescription()
                {
                    StartTime = StartTime,
                    EndTime = EndTime,
                    ReferencedElement = ReferencedElement,
                    Description1 = d1,
                    Description2 = d2,
                    Description3 = d3,
                    Description1webId = d1WebId,
                    Description2webId = d2WebId,
                    Description3webId = d3WebId,
                    AcknowledgedBy = AcknowledgedBy,
                    AcknowledgedDate = AcknowledgedDate,
                    AcknowledgedWebId = AcknowledgedWebId,
                    Duration = Duration,
                    One_Additional_Info = One_Additional_Info,
                    One_Crew = One_Crew,
                    One_Description = One_Description,
                    One_EndTime = One_EndTime,
                    One_Equipment_Path = One_Equipment_Path,
                    One_Equipment_Tag = One_Equipment_Tag,
                    One_Field = One_Field,
                    One_Schedule = One_Schedule,
                    One_Scope = One_Scope,
                    One_StartTime = One_StartTime,
                    Two_Additional_Info = Two_Additional_Info,
                    Two_Crew = Two_Crew,
                    Two_Description = Two_Description,
                    Two_EndTime = Two_EndTime,
                    Two_Equipment_Path = Two_Equipment_Path,
                    Two_Equipment_Tag = Two_Equipment_Tag,
                    Two_Field = Two_Field,
                    Two_Schedule = Two_Schedule,
                    Two_Scope = Two_Scope,
                    Two_StartTime = Two_StartTime,
                    Three_Additional_Info = Three_Additional_Info,
                    Three_Crew = Three_Crew,
                    Three_Description = Three_Description,
                    Three_EndTime = Three_EndTime,
                    Three_Equipment_Path = Three_Equipment_Path,
                    Three_Equipment_Tag = Three_Equipment_Tag,
                    Three_Field = Three_Field,
                    Three_Schedule = Three_Schedule,
                    Three_Scope = Three_Scope,
                    Three_StartTime = Three_StartTime,
                    One_Additional_Info_webId = One_Additional_InfowebId,
                    One_Crew_webId = One_CrewwebId,
                    One_Description_webId = One_DescriptionwebId,
                    One_EndTime_webId = One_EndTimewebId,
                    One_Equipment_Path_webId = One_Equipment_PathwebId,
                    One_Equipment_Tag_webId = One_Equipment_TagwebId,
                    One_Field_webId = One_FieldwebId,
                    One_Schedule_webId = One_SchedulewebId,
                    One_Scope_webId = One_ScopewebId,
                    One_StartTime_webId = One_StartTimewebId,
                    Two_Additional_Info_webId = Two_Additional_InfowebId,
                    Two_Crew_webId = Two_CrewwebId,
                    Two_Description_webId = Two_DescriptionwebId,
                    Two_EndTime_webId = Two_EndTimewebId,
                    Two_Equipment_Path_webId = Two_Equipment_PathwebId,
                    Two_Equipment_Tag_webId = Two_Equipment_TagwebId,
                    Two_Field_webId = Two_FieldwebId,
                    Two_Schedule_webId = Two_SchedulewebId,
                    Two_Scope_webId = Two_ScopewebId,
                    Two_StartTime_webId = Two_StartTimewebId,
                    Three_Additional_Info_webId = Three_Additional_InfowebId,
                    Three_Crew_webId = Three_CrewwebId,
                    Three_Description_webId = Three_DescriptionwebId,
                    Three_EndTime_webId = Three_EndTimewebId,
                    Three_Equipment_Path_webId = Three_Equipment_PathwebId,
                    Three_Equipment_Tag_webId = Three_Equipment_TagwebId,
                    Three_Field_webId = Three_FieldwebId,
                    Three_Schedule_webId = Three_SchedulewebId,
                    Three_Scope_webId = Three_ScopewebId,
                    Three_StartTime_webId = Three_StartTimewebId,
                    Percentage = Percentage,
                    Percentage_webId = PercentagewebId
                });
                ViewData["json"] = _evtdesc;               
   
            return View("EventDetails");
            }   
                catch (Exception)
            {
            return View("EventDetails");
            }      
        }        
        [HttpPut]
        public async Task UpdatePIEvent([FromBody] EventValue eval)
        {
            var modelObj = eval;
            var WebId = modelObj.WebId;
            var Value = modelObj.Value;
                        
            var WebIdArray = WebId.Split('$');
            var ValueArray = Value.Split('$');
            var maxvalue = ValueArray.Length;
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            Object ValueObject;
            for (int i = 0; i <= maxvalue; i++)
            {
                if (i == 0)
                {
                    ValueObject = new
                    {
                        Value = User.FindFirst("DisplayName").Value.ToString()
                    };
                }
                else
                {
                    ValueObject = new
                    {
                        Value = ValueArray[i]
                    };
                }

                string data = JsonConvert.SerializeObject(ValueObject);

                var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/attributes/" + WebIdArray[i] + "/value";

                HttpResponseMessage response = await client.PutAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
            }
        }
        [HttpGet]
        public async Task<string> PIEnumerationSets(string EnumerationTarget)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string WebId = ReadJson("AssetDatabase");
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            try
            {
                //this will cause the server call to ignore invallid SSL Cert - should remove for 
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string url = @"https://"+ PiWebApiServerMachine + "/PIwebapi/assetdatabases/"+WebId+"/enumerationsets";
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var webid = "";
                foreach (var item in dynJson)
                {
                    if (item.Name == EnumerationTarget)
                    {
                        webid = item.WebId;
                    }
                }
                return webid.ToString();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<string> PIEnumerationValues(string WebId)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            try
            {
                string url = @"https://"+ PiWebApiServerMachine + "/PIwebapi/enumerationsets/"+WebId+"/enumerationvalues";
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var _enumvalue = "";
                foreach (var item in dynJson)
                {
                    _enumvalue += item.Name + ",";
                }
                return _enumvalue.Remove(_enumvalue.Length - 1, 1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public IActionResult PI_AF_Analyses()
        {
            return View();
        }
        [HttpGet]
        public IActionResult PI_DA_Archive()
        {
            return View();
        }
        [HttpGet]
        public async Task<string> SearchPIPoint(string PointName)
        {   
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            string WebId = ReadJson("dataservers");
            try
            {
                //this will cause the server call to ignore invallid SSL Cert - should remove for 
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/dataservers/"+WebId+"/points?namefilter=" + PointName + "";
                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var _enumvalue = "";
                foreach (var item in dynJson)
                {
                    _enumvalue += item.WebId + "|" + item.Descriptor + "|"+ item.Name + ",";
                }
                return _enumvalue.Remove(_enumvalue.Length - 1, 1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<string> ElementSearch(string ElementName)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string WebId = ReadJson("WareHouse");
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            try
            {
                //this will cause the server call to ignore invallid SSL Cert - should remove for 
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/elements/search?databaseWebId="+ WebId + "&query=name:=" + ElementName + "&searchFullHierarchy=true";
                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var _enumvalue = "";
                foreach (var item in dynJson)
                {
                    _enumvalue += item.Name + "|" + item.Description + "|" + item.Path + ",";
                }
                return _enumvalue.Remove(_enumvalue.Length - 1, 1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpGet]
        public async Task<string> Recorded(string WebId, string StartTime, string EndTime)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            try
            {
                //this will cause the server call to ignore invallid SSL Cert - should remove for 
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/streams/" + WebId + "/recorded?starttime=" + StartTime + "&endtime=" + EndTime + "";
                HttpResponseMessage response = await client.GetAsync(url);

                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var _enumvalue = "";
                foreach (var item in dynJson)
                {
                    _enumvalue += item.Timestamp + "|" + item.Value + ",";
                }
                return _enumvalue.Remove(_enumvalue.Length - 1, 1);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<string> Analyses(string WebId)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            try
            {
                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/elements/" + WebId + "/elements";
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var _areaname = "";
                foreach (var item in dynJson)
                {
                    _areaname += item.Name + "|" + item.WebId + ",";
                }
                return _areaname.Remove(_areaname.Length - 1, 1);
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> Attributes(string WebId)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            try
            {
                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/elements/" + WebId + "/attributes";
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var _areaname = "";
                foreach (var item in dynJson)
                {
                    _areaname += item.Name + "|" + item.WebId + ",";
                }
                return _areaname.Remove(_areaname.Length - 1, 1);
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> ConfigAttributes(string WebId)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", User.FindFirst("Credential").Value.ToString());

            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;
            string PiWebApiServerMachine = ReadJson("PiWebApiServerMachine");
            try
            {
                string url = @"https://"+ PiWebApiServerMachine + "/piwebapi/elements/" + WebId + "/analyses";
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                var data = (JArray)JObject.Parse(content)["Items"];
                dynamic dynJson = JsonConvert.DeserializeObject(data.ToString());
                var _rule = "";
                foreach (var item in dynJson)
                {
                    var analysisruleLink = item.Links.AnalysisRule;
                    string url2 = @""+analysisruleLink+"";
                    HttpResponseMessage response2 = await client.GetAsync(url2);
                    string content2 = await response2.Content.ReadAsStringAsync();
                    dynamic analysisrule = JsonConvert.DeserializeObject(content2);
                    _rule += analysisrule.DisplayString + ",";
                }
                return _rule.Remove(_rule.Length - 1, 1);
            }
            catch
            {
                return null;
            }
        }
        [Authorize]
        public IActionResult Downtime()
        {
            try 
            { 
            return View();
            }
            catch
            {
                return View("Error");
            }
        }
        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        public bool IsValid(string username, string password)
        {
            bool valid = false;
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                valid = context.ValidateCredentials(username, password);
                return valid;
            }  
        }
        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (IsValid(username,password))
                {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    var usr = UserPrincipal.FindByIdentity(context, username);
                    if (usr != null)
                    {
                        var claims = new List<Claim>();
                        claims.Clear();
                        string authInfo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", username, password)));
                        claims.Add(new Claim("Credential", authInfo));
                        if (usr.Description == null)
                        {
                            claims.Add(new Claim("DisplayName", usr.DisplayName.ToString()));
                        }
                        else
                        {
                            claims.Add(new Claim("DisplayName", usr.DisplayName.ToString()));
                            claims.Add(new Claim("Description", usr.Description.ToString()));
                        }
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(principal);                        
                    }
                    return Redirect(returnUrl);
                    }
                }
            TempData["Error"] = "Error: Invalid Username or Password.";
            return View("login");
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

