window.Lib = window.Lib || {};
Lib.newWindow = function (url) {
    var h = window.screen.availHeight - 50;
    var w = window.screen.availWidth;
    var d = new Date();
    return window.open(url, "CfmisDialog" + d.getDay() + d.getHours() + d.getMinutes() + d.getSeconds(), "menubar=no,titlebar=no,toolbar=no,status=no,top=0,left=0,width=" + w + ",height=" + h);
};

Lib.Loading= function (isLoading, place) {
    if (isLoading === null) {
        return $("#ajax-loading").length !== 0;
    }
    else if (isLoading) {
        if ($("#ajax-loading").length === 0) {
            $("<div id='ajax-loading' style='z-index:99999;width:100%;height:100%;left:0px;top:0px;" + ((place != null && place != undefined && place != "") ? "" : "position:fixed;") + "text-align:center;background-color:#555;opacity:0.5;'><img src='../Content/images/rolling.gif' style='width:100px;height:100px;top:50%;position:absolute;margin-top:-30px;' /></div>").appendTo(place || "body");
        }
    }
    else {
        $("#ajax-loading").remove();
    }
}



