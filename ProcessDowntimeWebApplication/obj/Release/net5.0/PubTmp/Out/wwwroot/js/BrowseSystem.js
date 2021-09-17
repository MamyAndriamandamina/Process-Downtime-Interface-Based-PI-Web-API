function SystemTree()
{
    $("#mydb").empty();
    return $.ajax({
        url: 'http://mgppiaf01:44310/Home/Analyses?WebId=F1EmcegbpJtv7kKlF5s1SRObWgPwOMDRR-6RGp89Qli5iTwwTUdQUElBRjAxXEFNQkFUT1ZZIFJFUE9SVElOR1xET1dOVElNRQ',
        type: 'GET',
        contentType: 'application/json',
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
                var Head = $("#mydb");
                for (i = 0; i < str.length; i++) {
                    try {
                        var x = str[i].split('|');
                        var newelement = "<ul><li><span class=\"glyphicon glyphicon-plus-sign\" style=\"color:rgb(63,72,20); cursor: pointer\"></span><a id=\"" + x[1] + "\" style=\"font-size:14px; color:rgb(63,72,204); font-family:'Verdana'\" onclick=\"BrowseAf(event)\" >" + x[0] + "</a></li></ul>";
                        Head.append(newelement);
                    }
                    catch (ex) {
                        alert("error");
                        return;
                    }
                }
            }
        },
        error: function (xhr) {
            alert("Error on loading the system tree!...probably a license issue, please contact the PIHelp@ambatovy.mg")
            return;
        },
    });
}
function BrowseAf(event)
{
    var WebId = event.target.id;
    BrowseAfAttributes(WebId);
  

    if ($("#" + WebId + "").children().length > 0 && $("#" + WebId + "").children().is(":visible") == true) {
        $("#" + WebId + "").children().hide();
        return;
    }
    else if ($("#" + WebId + "").children().length > 0 && $("#" + WebId + "").children().is(":hidden") == true) {
        $("#" + WebId + "").children().show();
        return;
    }
    else {
        $("#" + WebId + "").children().empty();
        return $.ajax({
            url: 'http://mgppiaf01:44310/Home/Analyses?WebId=' + WebId + '',
            type: 'GET',
            contentType: 'application/json',
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
                    var Head = $("#" + WebId + "");
                    for (i = 0; i < str.length; i++) {
                        try {
                            var x = str[i].split('|');
                            var newelement = "<ul><li><span class=\"glyphicon glyphicon-plus-sign\" style=\"color:rgb(63,72,20); cursor: pointer\"></span><a id=\"" + x[1] + "\" style=\"font-size:14px; color:rgb(63,72,204); font-family:'Verdana'\">" + x[0] + "</a></li></ul>";
                            Head.append(newelement);
                        }
                        catch (ex) {
                            alert("error");
                            return;
                        }
                    }
                }
            },
            error: function (xhr) {
                alert("Error on loading the system tree!...probably a license issue, please contact the PIHelp@ambatovy.mg")
                return;
            },
        });
    }
}
function BrowseAfAttributes(WebId)
{
    BrowseAfConfigAnalyses(WebId);
    $("#attributes").empty();
            return $.ajax({
            url: 'http://mgppiaf01:44310/Home/Attributes?WebId=' + WebId + '',
            type: 'GET',
            contentType: 'application/json',
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
                    var Head = $("#attributes");
                    for (i = 0; i < str.length; i++) {
                        try {
                            var x = str[i].split('|');
                            var newelement = "<ul><li><span class=\"glyphicon glyphicon-plus-sign\" style=\"color:rgb(63,72,20); cursor: pointer\"></span><a id=\"" + x[1] + "\" style=\"font-size:14px; color:rgb(63,72,204); font-family:'Verdana'\">" + x[0] + "</a></li></ul>";
                            Head.append(newelement);
                        }
                        catch (ex) {
                            alert("error");
                            return;
                        }
                    }
                }
            },
            error: function (xhr) {
                alert("Error on loading the system tree!...probably a license issue, please contact the PIHelp@ambatovy.mg")
                return;
            },
        });
    }
function BrowseAfConfigAnalyses(WebId) {
    $("#config").empty();
    return $.ajax({
        url: 'http://mgppiaf01:44310/Home/ConfigAttributes?WebId=' + WebId + '',
        type: 'GET',
        contentType: 'application/json',
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
                var Head = $("#config");
                for (i = 0; i < str.length; i++) {
                    try {
                        var x = str[i].split('|');
                        var newelement = "<ul><li><span class=\"glyphicon glyphicon-plus-sign\" style=\"color:rgb(63,72,20); cursor: pointer\"></span><a id=\"" + x[1] + "\" style=\"font-size:14px; color:rgb(63,72,204); font-family:'Verdana'\">" + x[0] + "</a></li></ul>";
                        Head.append(newelement);
                    }
                    catch (ex) {
                        alert("error");
                        return;
                    }
                }
            }
        },
        error: function (xhr) {
            alert("Error on loading the system tree!...probably a license issue, please contact the PIHelp@ambatovy.mg")
            return;
        },
    });
}
