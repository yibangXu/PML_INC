/*
ReportOptions:{
	name:string,
	title:string,
	okTitle:string,
	url: string,
	params:array(ParamOptions),
	init:function(dialog),
	
	*formBody:string({0:Reprot Name},{1:Param Div Html}),
	*paramBody:string({0:Param Title},{1:Param Input Html}),
}

ParamOptions:{
	name:string,
	title:string,
	type:string,
	defaultValue,
	init:function(Input_Element,Other_ElementId),
	*inputBody:string({0:Reprot Name},{1:Param Name},{2:Param Type},{3:Param DefaultValue}),
}
*/
String.prototype.format = function () {
	var txt = this.toString();
	for (var i = 0; i < arguments.length; i++) {
		var exp = new RegExp('({)?\\{' + i + '\\}(?!})', 'gm');
		txt = txt.replace(exp, (arguments[i] == null ? "" : arguments[i]));
	}

	if (txt == null) return "";
	return txt.replace(new RegExp('({)?\\{\\d+\\}(?!})', 'gm'), "")
}
window.Lib = window.Lib || {};
window.Lib.paramDialog = function (options) {
    options = $.fn.extend({
        okTitle: "Yes",
    }, options);
    //{0}ReportName
    //{1}報表條件Input，以strParamBody輸出後填充
    var strDialogBody = options.formBody || '<div id="rpf{0}" style="display:none;"><fieldset style="margin-bottom:0.8em;"><input type="text" style="height:0px; padding:0; margin:0; border:none;" />{1}</fieldset><div class="btn-group pull-right"><a id="rpf{0}_Ok" class="btn btn-primary" style="color: white;">{2}</a><a id="rpf{0}_Cancel" class="btn btn-default">Cancel</a></div></div>';
    //{0}ParamTitle
    //{1}Input(Input/Select)
    var strParamBody = options.paramBody || '<div class="input-group" style="margin-bottom:0.4em;"><span class="input-group-addon">{0}</span>{1}</div>';
    //OutputParam
    var strParamHtml = "";
    var strDialogHtml = "";
    //Random a reportName if not contain in the options
    options.name = options.name || ("Report" + Math.floor(Math.random() * 9999999));
    options.title = options.title || options.name;

    for (var i in options.params) {
        var param = options.params[i];
        param.type = param.type || "text";
        if (param.type == "select") {
            var strInputBody = param.inputBody || '<select id="rpf{0}_{1}" class="form-control" type="text">';
            param.elementId = "rpf{0}_{1}".format(options.name, param.name);
            param.title = param.title || param.name;
            for (var v in param.values) {
                strInputBody += "<option value='{0}' {2}>{1}</option>".format(v, param.values[v], (param.defaultValue == v ? "selected" : ""));
            }
            strInputBody += "</select>";
            strParamHtml += strParamBody.format(param.title, strInputBody.format(options.name, param.name, param.type, param.defaultValue));
        }
        else if (param.type == "textarea") {
            //input
            //{0}Report_name
            //{1}Param_name
            //{2}Param_type
            //{3}Param_defaultValue
            var strInputBody = param.inputBody || '<textarea id="rpf{0}_{1}" class="form-control" style="height:300px;width:500px" >{3}';
            param.elementId = "rpf{0}_{1}".format(options.name, param.name);
            //param.title = param.title || param.name;
            strInputBody += "</textarea>";
            strParamHtml += strParamBody.format(param.title, strInputBody.format(options.name, param.name, param.type, param.defaultValue));
            //strParamHtml += strParamBody.format(param.title, param.name, param.type, param.defaultValue);
        }
        else if (param.type == "date") {
            //input
            //{0}Report_name
            //{1}Param_name
            //{2}Param_type
            //{3}Param_defaultValue
            var strInputBody = param.inputBody || '<input id="rpf{0}_{1}" class="form-control" type="date" value="{3}"/>';
            param.elementId = "rpf{0}_{1}".format(options.name, param.name);
            param.title = param.title || param.name;
            strParamHtml += strParamBody.format(param.title, strInputBody.format(options.name, param.name, param.type, param.defaultValue));
            //strParamHtml += strParamBody.format(param.title, param.name, param.type, param.defaultValue);
        }
        else if (param.type == "info") {
            //input
            //{0}Report_name
            //{1}Param_name
            //{2}Param_type
            //{3}Param_defaultValue
            var strInputBody = param.inputBody || '<input id="rpf{0}_{1}" class="form-control" type="text" value="{3}" readonly="readonly" />';
            param.elementId = "rpf{0}_{1}".format(options.name, param.name);
            param.title = param.title || param.name;
            strParamHtml += strParamBody.format(param.title, strInputBody.format(options.name, param.name, param.type, param.defaultValue));
            //strParamHtml += strParamBody.format(param.title, param.name, param.type, param.defaultValue);
        }
        else if (param.type == "hidden") {
            //input
            //{0}Report_name
            //{1}Param_name
            //{2}Param_type
            //{3}Param_defaultValue
            var strInputBody = param.inputBody || '<input id="rpf{0}_{1}" type="hidden" value="{3}" />';
            param.elementId = "rpf{0}_{1}".format(options.name, param.name);
            param.title = param.title || param.name;
            strParamHtml += strInputBody.format(options.name, param.name, param.type, param.defaultValue);
        }
        else if (param.type == "hr") {
            strParamHtml += "<hr>";
        }
        else {
            //input
            //{0}Report_name
            //{1}Param_name
            //{2}Param_type
            //{3}Param_defaultValue
            //{4}Param_Placeholder
            var strInputBody = param.inputBody || '<input id="rpf{0}_{1}" class="form-control" type="{2}" value="{3}" placeholder="{4}" />';
            param.elementId = "rpf{0}_{1}".format(options.name, param.name);
            param.title = param.title || param.name;
            strParamHtml += strParamBody.format(param.title, strInputBody.format(options.name, param.name, param.type, param.defaultValue, param.placeholder));
        }
    }
    strDialogHtml = strDialogBody.format(options.name, strParamHtml, options.okTitle);

    var dialogTarget = $(strDialogHtml).dialog(
        $.fn.extend({
            modal: true,
            show: { effect: "fold", duration: 150 },
            hide: { effect: "clip", duration: 150 },
            title: options.title,
            width: options.width || 450,
            close: function () { $("#rpf" + options.name).dialog("destroy");},
        }, options.dialog || null)
    );

    //Execute Dialog And Param Initialize Function
    if (typeof (options.init) == "function")
        options.init(dialogTarget);

    var paramIds = {};
    for (var i in options.params) {
        var param = options.params[i];
        param.elementTarget = $("#{0}".format(param.elementId));
        paramIds[param.name] = param.elementId;
    }
    for (var i in options.params) {
        var param = options.params[i];
        if (typeof (param.init) == "function")
            param.init(param.elementTarget, paramIds);
    }

    $("#rpf{0}_Ok".format(options.name)).click(function () {
        if (typeof (options.okFunc) != "undefined") {
            var p = {};
            for (var i in options.params) {
                var param = options.params[i];
                p[param.name] = param.elementTarget.val();
            }
            options.okFunc(p,dialogTarget)&&dialogTarget.dialog("close");
        }
        else {
            var url = options.url;
            if (!url.endsWith("?"))
                url += "?";
            for (var i in options.params) {
                var param = options.params[i];
                if (param.type != "info")
                    url += param.name + "=" + param.elementTarget.val() + "&";
            }
            if (options.openWindow == false)
                window.location = url;
            else
                OpenWindow(url);
            dialogTarget.dialog("destroy");
        }

    });
    $("#rpf{0}_Cancel".format(options.name)).click(function () {
        if (typeof (options.cancelFunc) != "undefined") {
            options.cancelFunc();
        }
        dialogTarget.dialog("destroy");

    });
    return dialogTarget;
};
