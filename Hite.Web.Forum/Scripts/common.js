function toggleCollapse(objId) {
    var imgObjId = objId + "_img";
    $('#' + objId).toggle();
    $('#' + imgObjId).attr("src", $('#' + objId).is(":hidden") ? "/images/collapsed_yes.gif" : "/images/collapsed_no.gif");
}