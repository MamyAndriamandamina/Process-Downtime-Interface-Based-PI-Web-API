function SearchForPIPoint() {
    var pointname = $("#pipointtext").val();
  
    return $.ajax({
        url: 'http://mgppiaf01:44310/Home/SearchPIPoint?PointName=' + pointname + '',
        type: 'GET',
        contentType: 'application/json; charset=UTF-8',
        async: true,
        datatype: 'json',
        success: function (Data)
        {
            if (!$.trim(Data)) {
                return;
            }
            else {
                // data is not blank;
                var x = JSON.stringify(Data);
                var clean = x.replace(/['"]/g, '').replace('[', '').replace(']', '');
                var str = clean.split(',');
                $("#foundpipoint").empty();
                try
                {
                    var tablehead = '<thead class="thead-dark table-bordered"><tr>'
                        + '<th>#</th>'
                        + '<th>PI Point</th>'
                        + '<th>Descriptor</th>'
                        + '<th>Details</th>'
                        + '</tr></thead>';
                    $("#foundpipoint").append(tablehead);
                    for (i = 0; i < str.length; i++)
                    {
                        var x = str[i].split('|');
                        var body = '<tbody class="table-bordered">'
                            + '<th>' + i + '</th>'
                            + '<th>' + x[2] + '</th>'
                            + '<th>' + x[1] + '</th>'
                            + '<th>' + '<a onclick=\"ShowPiPointDetails(event)\" id="' + x[0] + '" class=\"btn btn-success\"> <span class=\"glyphicon glyphicon-tags\"></span>Show</a>' + '</th>'
                            + '</tbody>';
                        $("#foundpipoint").append(body);
                    }
                }
                catch (ex)
                {
                    return;
                }
            }
        },
        error: function (xhr) {
            $("#foundpipoint").empty();
            alert("Error on PI Point Search Process...")
            return;
        },
    });
}
function ShowPiPointDetails(event)
{
    var starttime = $("#Fromdate").val();
    var endtime = $("#Todate").val();
    var webId = event.target.id;
    var options = {
        xaxis: {
            mode: "time",
            timeformat: "%H:%M",
            timezone: "browser"
        }
    };
   
    return $.ajax({
        url: 'http://mgppiaf01:44310/Home/Recorded?WebId=' + webId + '&StartTime=' + starttime + '&EndTime=' + endtime + '',
        type: 'GET',
        contentType: 'application/json; charset=UTF-8',
        async: true,
        datatype: 'json',
        success: function (Data) {
            if (!$.trim(Data)) {
                return;
            }
            else {
                // data is not blank;
                var x = JSON.stringify(Data);
                var clean = x.replace(/['"]/g, '').replace('[', '').replace(']', '');
                var str = clean.split(',');
                $("#mytable").empty();
                var plotData = [[]];
                try {
                    var tablehead = '<thead class="thead-dark table-bordered"><tr>'
                        + '<th>#</th>'
                        + '<th>Timestamp</th>'
                        + '<th>Value</th>'
                        + '</tr></thead>';
                    $("#mytable").append(tablehead);
                    for (i = 0; i < str.length; i++) {
                        var x = str[i].split('|');
                        var body = '<tbody class="table-bordered">'
                            + '<th>' + i + '</th>'
                            + '<th>' + x[0] + '</th>'
                            + '<th>' + x[1] + '</th>'
                            + '</tbody>';
                        $("#mytable").append(body);
                    }
                    //THIS IS FOR PUBLISHING A CHART BUT NOT COMPATIBLE WITH .NET CORE 5.0
                    //for (i = 0; i < str.length; i++)
                    //{
                    //    var x = str[i].split('|');
                    //    console.log(new Date(x[0]).getTime());
                    //    console.log(x[1]);
                    //    plotData[0].push([new Date(x[0]).getTime(), x[1]]);
                    //}
                    //$.plot($("#plotarea"), plotData[0], options);
                }
                catch (ex) {
                    return;
                }
            }
        },
        error: function (xhr) {
            $("#foundpipoint").empty();
            alert("Error on Searching data archive...")
            return;
        },
    });
}
