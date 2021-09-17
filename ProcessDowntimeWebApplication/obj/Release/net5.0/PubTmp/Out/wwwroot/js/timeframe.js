function ValidateEvent()
{
    
    var v1 = parseInt($("#Eventpercent_1").find(":selected").val());
    var v2 = parseInt($("#Eventpercent_2").find(":selected").val());
    var v3 = parseInt($("#Eventpercent_3").find(":selected").val());
    if (v1 < 100) {
        if (v2 < (100 - v1) && v2 != 0) {
            if (v3 == (100 - (v1 + v2))) {
                return;
            }
            else {
                var v5 = $("#Eventpercent_3").val(parseInt(100 - (v1 + v2)));
                CalculateTimeFrame();
                return;
            }
        }
        else {
            var v4 = $("#Eventpercent_2").val(parseInt(100 - v1));
            $("#Eventpercent_3").val(0);
            CalculateTimeFrame();
            return;
        }
    }
    else {
        $("#Eventpercent_2").val(0);
        $("#Eventpercent_3").val(0);
        CalculateTimeFrame();
        ShowGantt();
    }
}
function MonthToString(month) {
    if (month.indexOf("/01/") > -1) { var z = month.replace("/01/", "-Jan-") };
    if (month.indexOf("/02/") > -1) { var z = month.replace("/02/", "-Feb-") };
    if (month.indexOf("/03/") > -1) { var z = month.replace("/03/", "-Mar-") };
    if (month.indexOf("/04/") > -1) { var z = month.replace("/04/", "-Apr-") };
    if (month.indexOf("/05/") > -1) { var z = month.replace("/05/", "-May-") };
    if (month.indexOf("/06/") > -1) { var z = month.replace("/06/", "-Jun-") };
    if (month.indexOf("/07/") > -1) { var z = month.replace("/07/", "-Jul-") };
    if (month.indexOf("/08/") > -1) { var z = month.replace("/08/", "-Aug-") };
    if (month.indexOf("/09/") > -1) { var z = month.replace("/09/", "-Sep-") };
    if (month.indexOf("/10/") > -1) { var z = month.replace("/10/", "-Oct-") };
    if (month.indexOf("/11/") > -1) { var z = month.replace("/11/", "-Nov-") };
    if (month.indexOf("/12/") > -1) { var z = month.replace("/12/", "-Dec-") };
    return z;
}
function CalculateTimeFrame() {

    var tottime = document.getElementById('duration_raw').innerHTML;
    var st = document.getElementById('starttime_raw').innerHTML;
    var et = document.getElementById('endtime_raw').innerHTML;
    if (tottime.indexOf('in progress') > -1)
    {
        var t = tottime.split('(');
        tottime = t[0];
    }
    var x = tottime.split(':');
    if (x[0].indexOf('.') > -1)
    {
        var y = x[0].split('.');
        day_sec = parseFloat(y[0]) * 86400;
        hour_sec_1 = parseFloat(y[1]) * 3600;
        hour_sec_2 = 0;
    }
    else
    {
        day_sec = 0;
        hour_sec_1 = 0;
        hour_sec_2 = parseFloat(x[0]) * 3600;
    }
        minutes_sec = parseFloat(x[1]) * 60;
        seconds_sec = parseFloat(x[2]);
    
    var tottime_sec = day_sec + hour_sec_1 + hour_sec_2 + minutes_sec + seconds_sec;
        
    var perc1 = parseFloat($("#Eventpercent_1").find(":selected").text());
    var perc2 = parseFloat($("#Eventpercent_2").find(":selected").text());
    var perc3 = parseFloat($("#Eventpercent_3").find(":selected").text());

    var split1 = Math.round(tottime_sec * perc1 / 100)*1000;
    var split2 = Math.round(tottime_sec * perc2 / 100)*1000;
    var split3 = Math.round(tottime_sec * perc3 / 100)*1000;
    
    if (st.indexOf("/01/") > -1) { var x = st.replace("/01/", "-Jan-")};
    if (st.indexOf("/02/") > -1) { var x = st.replace("/02/", "-Feb-")};
    if (st.indexOf("/03/") > -1) { var x = st.replace("/03/", "-Mar-")};
    if (st.indexOf("/04/") > -1) { var x = st.replace("/04/", "-Apr-")};
    if (st.indexOf("/05/") > -1) { var x = st.replace("/05/", "-May-")};
    if (st.indexOf("/06/") > -1) { var x = st.replace("/06/", "-Jun-")};
    if (st.indexOf("/07/") > -1) { var x = st.replace("/07/", "-Jul-")};
    if (st.indexOf("/08/") > -1) { var x = st.replace("/08/", "-Aug-")};
    if (st.indexOf("/09/") > -1) { var x = st.replace("/09/", "-Sep-")};
    if (st.indexOf("/10/") > -1) { var x = st.replace("/10/", "-Oct-")};
    if (st.indexOf("/11/") > -1) { var x = st.replace("/11/", "-Nov-")};
    if (st.indexOf("/12/") > -1) { var x = st.replace("/12/", "-Dec-") };

    if (et.indexOf("/01/") > -1) { var z = et.replace("/01/", "-Jan-") };
    if (et.indexOf("/02/") > -1) { var z = et.replace("/02/", "-Feb-") };
    if (et.indexOf("/03/") > -1) { var z = et.replace("/03/", "-Mar-") };
    if (et.indexOf("/04/") > -1) { var z = et.replace("/04/", "-Apr-") };
    if (et.indexOf("/05/") > -1) { var z = et.replace("/05/", "-May-") };
    if (et.indexOf("/06/") > -1) { var z = et.replace("/06/", "-Jun-") };
    if (et.indexOf("/07/") > -1) { var z = et.replace("/07/", "-Jul-") };
    if (et.indexOf("/08/") > -1) { var z = et.replace("/08/", "-Aug-") };
    if (et.indexOf("/09/") > -1) { var z = et.replace("/09/", "-Sep-") };
    if (et.indexOf("/10/") > -1) { var z = et.replace("/10/", "-Oct-") };
    if (et.indexOf("/11/") > -1) { var z = et.replace("/11/", "-Nov-") };
    if (et.indexOf("/12/") > -1) { var z = et.replace("/12/", "-Dec-") };


    var d1 = Date.parse(st);
    var d2 = d1 + split1;
    var d3 = d2 + split2;
    var d4 = Date.parse(et);

    document.getElementById('starttime_1').innerHTML = MonthToString(new Date(d1).toLocaleString("fr-FR"));
    document.getElementById('endtime_1').innerHTML = MonthToString(new Date(d2).toLocaleString("fr-FR"));
    document.getElementById('starttime_2').innerHTML = MonthToString(new Date(d2).toLocaleString("fr-FR"));
    document.getElementById('endtime_2').innerHTML = MonthToString(new Date(d3).toLocaleString("fr-FR"));
    document.getElementById('starttime_3').innerHTML = MonthToString(new Date(d3).toLocaleString("fr-FR"));
    document.getElementById('endtime_3').innerHTML = MonthToString(new Date(d4).toLocaleString("fr-FR"));

}

