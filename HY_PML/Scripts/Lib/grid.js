window.Lib = window.Lib || {};
Lib.jqGrid = $.extend(Lib.jqGrid || {}, {
    buildSelect: function (jsonStr, valueProp, TitleProp, nullable) {
        var select = "<select>";
        var rows = JSON.parse(jsonStr).rows;
        if (nullable)
            select += "<option value=''></option>";
        for (var i in rows) {
            select += "<option value='";
            if (valueProp.includes(",")) {
                valueProps = valueProp.split(',');
                for (var t in valueProps)
                    select += rows[i][valueProps[t]];
            } else {
                select += rows[i][valueProp];
            }
            select += "'>";
            if (TitleProp.includes(",")) {
                TitleProps = TitleProp.split(',');
                for (var t in TitleProps)
                    select += rows[i][TitleProps[t]] + " ";
            }
            else
                select += rows[i][TitleProp];
            select += "</option>";
        }
        select += "</select>";
        return select;
    },
    buildValue: function (jsonStr, valueProp, TitleProp) {
        var result = {};
        var rows = JSON.parse(jsonStr).rows;
        console.log(rows);
        for (var i in rows) {
            console.log(rows[i][valueProp]);
            console.logert(rows[i][TitleProp]);
            result[rows[i][valueProp]] = rows[i][TitleProp];
        }
        return result;
    }
});

$.jgrid.extend({
    stdPager: function (pager) {
        return $(this).jqGrid("navGrid", pager,
            { edit: true, add: true, del: true, search: false, view: false, },
            {/*edit*/mtype: "put", }, {/*add*/mtype: "post", }, {/*del*/mtype: "delete", }, {/*search*/ }, {/*view*/ }
        );
    },
    viewOnlyPager: function (pager) {
        return $(this).jqGrid("navGrid", pager,
            { edit: false, add: false, del: false, search: false, view: false, }
        );
    },
});