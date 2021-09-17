function ReportId(event)
{
    var myid = event.target.id;
    if (myid == "Asset_1") { document.getElementById('Target').innerHTML = "equipment_name_1" };
    if (myid == "Asset_1") { document.getElementById('Target_location').innerHTML = "equipment_location_1" };
    if (myid == "Asset_2") { document.getElementById('Target').innerHTML = "equipment_name_2" };
    if (myid == "Asset_2") { document.getElementById('Target_location').innerHTML = "equipment_location_2" };
    if (myid == "Asset_3") { document.getElementById('Target').innerHTML = "equipment_name_3" };
    if (myid == "Asset_3") { document.getElementById('Target_location').innerHTML = "equipment_location_3" };
}
function SearchEquipment() {
    var element = $("#elementid").val();
    return $.ajax({
        url: 'https://localhost:44310/Home/ElementSearch?ElementName=' + element + '',
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
                $("#Equipment_search_table").empty();
                try {
                    $("#equip_num").empty();
                    var tablehead = '<thead class="thead-dark table-bordered"><tr>'
                        + '<th style="width: 5%">#</th>'
                        + '<th style="width: 20%">Equipment Name</th>'
                        + '<th style="width: 20%">Description</th>'
                        + '<th style="width: 40%">Location</th>'
                        + '<th style="width: 15%">Attach</th>'
                        + '</tr >'
                        + '</thead >';
                    $("#Equipment_search_table").append(tablehead);
                    for (i = 0; i < str.length; i++) {
                        var m = parseInt(i + 1);
                        var x = str[i].split('|');
                        var name = x[0];
                        var description = x[1];
                        var path = replaceAll(x[2].toString(), "\\\\", "\\");
                       
                        var body = '<tbody class="table-bordered"><tr>'
                            + '<th>' + m + '</th>'
                            + '<th>' + name + '</th>'
                            + '<th>' + description + '</th>'
                            + '<th>' + path + '</th>'
                            + '<th>' + '<a onclick=\"AttachEquipment(event)\" id="' + name + "|" + path + '" class=\"btn btn-success\" data-dismiss=\"modal\"> <span class=\"glyphicon glyphicon-tags\"></span>Attach</a>' + '</th>'
                            + '<tr></tbody>';
                        $("#Equipment_search_table").append(body);
                    }
                    $("#equip_num").append(str.length);
                }
                catch (ex) {
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
function replaceAll(string, search, replace) {
    return string.split(search).join(replace);
}
function AttachEquipment(event)
{
    var id = event.target.id;
    var x = id.split('|');
    var equipment_name = x[0];
    var equipment_path = x[1];
    var target_equip_name = document.getElementById("Target").textContent;
    var target_equip_location = document.getElementById("Target_location").textContent;
    document.getElementById(""+target_equip_name+"").innerHTML = equipment_name;
    document.getElementById(""+target_equip_location+"").innerHTML = equipment_path;
}