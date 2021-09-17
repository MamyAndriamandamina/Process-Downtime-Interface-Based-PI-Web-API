function SetDataToPI() {
    $("#confirmation_table >tbody").empty();
    var st1 = "";
    var et1 = "";
    var cr1 = "";
    var dc1 = "";
    var fld1 = "";
    var sch1 = "";
    var scp1 = "";
    var equip1 = "";
    var equip_loc1 = "";
    var info1 = "";
    //
    var st2 = "";
    var et2 = "";
    var cr2 = "";
    var dc2 = "";
    var fld2 = "";
    var sch2 = "";
    var scp2 = "";
    var equip2 = "";
    var equip_loc2 = "";
    var info2 = "";
    //
    var st3 = "";
    var et3 = "";
    var cr3 = "";
    var dc3 = "";
    var fld3 = "";
    var sch3 = "";
    var scp3 = "";
    var equip3 = "";
    var equip_loc3 = "";
    var info3 = "";

    if ($("#Eventpercent_1").find(":selected").text() != 0)
    {
        var st1 = document.getElementById("starttime_1").textContent;
        var et1 = document.getElementById("endtime_1").textContent;
        var cr1 = $("#Crew_1").find(":selected").text();
        var dc1 = $("#Description_1").find(":selected").text();
        var fld1 = $("#Field_1").find(":selected").text();
        var sch1 = $("#Schedule_1").find(":selected").text();
        var scp1 = $("#Scope_1").find(":selected").text();
        var equip1 = document.getElementById("equipment_name_1").textContent;
        var equip_loc1 = document.getElementById("equipment_location_1").textContent;
        var info1 = $("#InfoText1").val();
    }
    if ($("#Eventpercent_2").find(":selected").text() != 0) {
        var st2 = document.getElementById("starttime_2").textContent;
        var et2 = document.getElementById("endtime_2").textContent;
        var cr2 = $("#Crew_2").find(":selected").text();
        var dc2 = $("#Description_2").find(":selected").text();
        var fld2 = $("#Field_2").find(":selected").text();
        var sch2 = $("#Schedule_2").find(":selected").text();
        var scp2 = $("#Scope_2").find(":selected").text();
        var equip2 = document.getElementById("equipment_name_2").textContent;
        var equip_loc2 = document.getElementById("equipment_location_2").textContent;
        var info2 = $("#InfoText2").val();
    }
    if ($("#Eventpercent_3").find(":selected").text() != 0)
    {
        var st3 = document.getElementById("starttime_3").textContent;
        var et3 = document.getElementById("endtime_3").textContent;
        var cr3 = $("#Crew_3").find(":selected").text();
        var dc3 = $("#Description_3").find(":selected").text();
        var fld3 = $("#Field_3").find(":selected").text();
        var sch3 = $("#Schedule_3").find(":selected").text();
        var scp3 = $("#Scope_3").find(":selected").text();
        var equip3 = document.getElementById("equipment_name_3").textContent;
        var equip_loc3 = document.getElementById("equipment_location_3").textContent;
        var info3 = $("#InfoText3").val();       
    }
    $("#confirmation_table >tbody").append('<tr><th><label id="item1">1</label></th>'
        + '<th><label id="tbl_starttime1">' + st1 + '</label></th>'
        + '<th><label id="tbl_endtime1">' + et1 + '</label></th>'
        + '<th><label id="tbl_crew1">' + cr1 + '</label></th>'
        + '<th><label id="tbl_description1">' + dc1 + '</label></th>'
        + '<th><label id="tbl_field1">' + fld1 + '</label></th>'
        + '<th><label id="tbl_schedule1">' + sch1 + '</label></th>'
        + '<th><label id="tbl_scope1">' + scp1 + '</label></th>'
        + '<th><label id="tbl_equipment1">' + equip1 + '</label></th>'
        + '<th><label id="tbl_equipment_loc1">' + equip_loc1 + '</label></th>'
        + '<th><label id="tbl_info1">' + info1 + '</label></th></tr>'
        +' <tr><th><label id="item2">2</label></th>'//next row
        + '<th><label id="tbl_starttime2">' + st2 + '</label></th>'
        + '<th><label id="tbl_endtime2">' + et2 + '</label></th>'
        + '<th><label id="tbl_crew2">' + cr2 + '</label></th>'
        + '<th><label id="tbl_description2">' + dc2 + '</label></th>'
        + '<th><label id="tbl_field2">' + fld2 + '</label></th>'
        + '<th><label id="tbl_schedule2">' + sch2 + '</label></th>'
        + '<th><label id="tbl_scope2">' + scp2 + '</label></th>'
        + '<th><label id="tbl_equipment2">' + equip2 + '</label></th>'
        + '<th><label id="tbl_equipment_loc2">' + equip_loc2 + '</label></th>'
        + '<th><label id="tbl_info2">' + info2 + '</label></th></tr>'
        + '<tr><th><label id="item3">3</label></th>'//next row
        + '<th><label id="tbl_starttime3">' + st3 + '</label></th>'
        + '<th><label id="tbl_endtime3">' + et3 + '</label></th>'
        + '<th><label id="tbl_crew3">' + cr3 + '</label></th>'
        + '<th><label id="tbl_description3">' + dc3 + '</label></th>'
        + '<th><label id="tbl_field3">' + fld3 + '</label></th>'
        + '<th><label id="tbl_schedule3">' + sch3 + '</label></th>'
        + '<th><label id="tbl_scope3">' + scp3 + '</label></th>'
        + '<th><label id="tbl_equipment3">' + equip3 + '</label></th>'
        + '<th><label id="tbl_equipment_loc3">' + equip_loc3 + '</label></th>'
        + '<th><label id="tbl_info3">' + info3 + '</label></th></tr>');
    }