// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function FindAllEnumerationSets()
{  
    FillSplitter();
    Browse("Crew");
    Browse("Description");
    Browse("Field");
    Browse("Schedule");
    Browse("Scope");
    $("#EventDetails").show();
    $("#Confirmation").show();
    $("#UpdateEvent").hide();
}
function FillSplitter()
{
    $("#Eventpercent_1").empty();
    $("#Eventpercent_2").empty();
    $("#Eventpercent_3").empty();
    for (var i = 0; i <= 100; i++)
    {
        $("#Eventpercent_1").append("<option value=\"" + i + "\">" + i + "</option>");
        $("#Eventpercent_2").append("<option value=\"" + i + "\">" + i + "</option>");
        $("#Eventpercent_3").append("<option value=\"" + i + "\">" + i + "</option>");
    }
}
function Browse(EnumerationTarget)
{   
    return $.ajax({
        url: 'https://localhost:44310/Home/PIEnumerationSets?EnumerationTarget='+EnumerationTarget+'',
        type: 'GET',
        contentType: 'application/json',
        async: true,
        datatype: 'json',
        success: function (Data)
        {
            var x = JSON.stringify(Data);
            var clean = x.replace(/['"]/g, '').replace('[', '').replace(']', '');
            var str = clean.split(',');
            try
            {
                BrowseEnumerationValue(EnumerationTarget, str);
            }
                catch (ex)
                {
                    return;
                }                
        },
        error: function (xhr)
        {
            alert("Error on loading the enumerations sets!..., please contact youremail@company.com")
            return;
        },
    });
}
function BrowseEnumerationValue(EnumerationTarget, WebId) {
    $("#" + EnumerationTarget + "_1").empty();
    $("#" + EnumerationTarget + "_2").empty();
    $("#" + EnumerationTarget + "_3").empty();
    return $.ajax({
        url: 'https://localhost:44310/Home/PIEnumerationValues?WebId=' + WebId + '',
        type: 'GET',
        contentType: 'application/json',
        async: true,
        datatype: 'json',
        success: function (Data) {
            var x = JSON.stringify(Data);
            var clean = x.replace(/['"]/g, '').replace('[', '').replace(']', '');
            var str = clean.split(',');
            for (i = 0; i < str.length; i++)
            {
                try
                {
                    $("#" + EnumerationTarget + "_1").append("<option>" + str[i] + "</option>");
                    $("#" + EnumerationTarget + "_2").append("<option>" + str[i] + "</option>");
                    $("#" + EnumerationTarget + "_3").append("<option>" + str[i] + "</option>");
                }
                catch (ex)
                {
                    return;
                }
            }
        },
        error: function (xhr) {
            alert("Error on loading the enumerations values!..., please contact youremail@company.com")
            return;
        },
    });
}


