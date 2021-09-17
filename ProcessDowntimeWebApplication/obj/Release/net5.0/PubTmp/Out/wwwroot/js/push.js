function PostEvent() {
    PostEventData();
    alert("processing...")
}
function PostEventData() {

    var v1 = $("#Eventpercent_1").find(":selected").val();
    var v2 = $("#Eventpercent_2").find(":selected").val();
    var v3 = $("#Eventpercent_3").find(":selected").val();

    var data = [];
    var webId = [];

    data[0] = "";//Acknowledger init
    data[1] = document.getElementById("starttime_1").textContent;
    data[2] = document.getElementById("endtime_1").textContent;
    if (parseInt(v1) == 0) {
        data[3] = "";
        data[4] = "";
        data[5] = "";
        data[6] = "";
        data[7] = "";
        data[8] = "";
        data[9] = "";
        data[10] = "";
    }
    else
    {
        data[3] = $("#Crew_1").find(":selected").text();
        data[4] = $("#Description_1").find(":selected").text();
        data[5] = $("#Field_1").find(":selected").text();
        data[6] = $("#Schedule_1").find(":selected").text();
        data[7] = $("#Scope_1").find(":selected").text();
        data[8] = document.getElementById("equipment_name_1").textContent;
        data[9] = document.getElementById("equipment_location_1").textContent;
        var info1 = $("#InfoText1").val();
        data[10] = replaceAll(info1, "$", "_");
    }
    data[11] = document.getElementById("starttime_2").textContent;
    data[12] = document.getElementById("endtime_2").textContent;
    if (parseInt(v2) == 0) {
        data[13] = "";
        data[14] = "";
        data[15] = "";
        data[16] = "";
        data[17] = "";
        data[18] = "";
        data[19] = "";
        data[20] = "";
    }
    else
    {
        data[13] = $("#Crew_2").find(":selected").text();
        data[14] = $("#Description_2").find(":selected").text();
        data[15] = $("#Field_2").find(":selected").text();
        data[16] = $("#Schedule_2").find(":selected").text();
        data[17] = $("#Scope_2").find(":selected").text();
        data[18] = document.getElementById("equipment_name_2").textContent;
        data[19] = document.getElementById("equipment_location_2").textContent;
        var info2 = $("#InfoText2").val();
        data[20] = replaceAll(info2, "$", "_");
    }
    data[21] = document.getElementById("starttime_3").textContent;
    data[22] = document.getElementById("endtime_3").textContent;
    if (parseInt(v2) == 0) {
        data[23] = "";
        data[24] = "";
        data[25] = "";
        data[26] = "";
        data[27] = "";
        data[28] = "";
        data[29] = "";
        data[30] = "";
    }
    else
    {
        data[23] = $("#Crew_3").find(":selected").text();
        data[24] = $("#Description_3").find(":selected").text();
        data[25] = $("#Field_3").find(":selected").text();
        data[26] = $("#Schedule_3").find(":selected").text();
        data[27] = $("#Scope_3").find(":selected").text();
        data[28] = document.getElementById("equipment_name_3").textContent;
        data[29] = document.getElementById("equipment_location_3").textContent;
        var info3 = $("#InfoText3").val();
        data[30] = replaceAll(info3, "$", "_");
    }
    data[31] = v1 + "|" + v2 + "|" + v3;
    
    webId[0] = document.getElementById("Acknowledged_webId").textContent;
    webId[1] = document.getElementById("data_1_start_time_webId").textContent;
    webId[2] = document.getElementById("data_1_end_time_webId").textContent;
    webId[3] = document.getElementById("data_1_crew_webId").textContent;
    webId[4] = document.getElementById("data_1_description_webId").textContent;
    webId[5] = document.getElementById("data_1_field_webId").textContent;
    webId[6] = document.getElementById("data_1_schedule_webId").textContent;
    webId[7] = document.getElementById("data_1_scope_webId").textContent;
    webId[8] = document.getElementById("data_1_equipment_tag_webId").textContent;
    webId[9] = document.getElementById("data_1_equipment_path_webId").textContent;
    webId[10] = document.getElementById("data_1_additional_info_webId").textContent;

    webId[11] = document.getElementById("data_2_start_time_webId").textContent;
    webId[12] = document.getElementById("data_2_end_time_webId").textContent;
    webId[13] = document.getElementById("data_2_crew_webId").textContent;
    webId[14] = document.getElementById("data_2_description_webId").textContent;
    webId[15] = document.getElementById("data_2_field_webId").textContent;
    webId[16] = document.getElementById("data_2_schedule_webId").textContent;
    webId[17] = document.getElementById("data_2_scope_webId").textContent;
    webId[18] = document.getElementById("data_2_equipment_tag_webId").textContent;
    webId[19] = document.getElementById("data_2_equipment_path_webId").textContent;
    webId[20] = document.getElementById("data_2_additional_info_webId").textContent;

    webId[21] = document.getElementById("data_3_start_time_webId").textContent;
    webId[22] = document.getElementById("data_3_end_time_webId").textContent;
    webId[23] = document.getElementById("data_3_crew_webId").textContent;
    webId[24] = document.getElementById("data_3_description_webId").textContent;
    webId[25] = document.getElementById("data_3_field_webId").textContent;
    webId[26] = document.getElementById("data_3_schedule_webId").textContent;
    webId[27] = document.getElementById("data_3_scope_webId").textContent;
    webId[28] = document.getElementById("data_3_equipment_tag_webId").textContent;
    webId[29] = document.getElementById("data_3_equipment_path_webId").textContent;
    webId[30] = document.getElementById("data_3_additional_info_webId").textContent;
    webId[31] = document.getElementById("Percentage_webId").textContent;
    
    var alldata = "";
    var alldatawebid = "";

    for (i = 0; i <= data.length; i++) {
        alldata = alldata + data[i] + "$";
        alldatawebid = alldatawebid + webId[i] + "$";
    }
    const obj1 = { Value: "" + alldata + "", WebId: "" + alldatawebid + "" };
    const myJSON1 = JSON.stringify(obj1);
    return $.ajax({
        type: 'PUT',
        url: 'http://mgppiaf01:44310/Home/UpdatePIEvent',
        contentType: 'application/json',
        async: true,
        datatype: 'json',
        data: myJSON1,
        success: function (result) {
            //console.log(result);
        },
        error: function (error) { console.log(error); }
    });
}
function replaceAll(string, search, replace) {
    return string.split(search).join(replace);
}
