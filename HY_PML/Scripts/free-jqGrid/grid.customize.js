/**
 * @license jqGrid Chinese (Taiwan) Translation for v4.2
 * linquize
 * https://github.com/linquize/jqGrid
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 *
**/

/*jslint white: true */
/*global jQuery */
(function (factory) {
    "use strict";
    if (typeof define === "function" && define.amd) {
        // AMD. Register as an anonymous module.
        define(["jquery"], factory);
    } else if (typeof exports === "object") {
        // Node/CommonJS
        factory(require("jquery"));
    } else {
        // Browser globals
        factory(jQuery);
    }
}
    (function ($) {
        "use strict";
        var locInfo = {
            isRTL: false,
            defaults: {
                recordtext: "顯示{0}到{1}筆資料，共{2}筆資料", //"{2} records",
                emptyrecords: "沒有資料",
                loadtext: "Loading...",
                pgtext: "第{0}頁，共{1}頁",
                pgfirst: "",
                pglast: "",
                pgnext: "",
                pgprev: "",
                pgrecs: "",
                showhide: "",
                savetext: ""
            },
            search: {
                caption: "",
                Find: "",
                Reset: "",
                odata: [
                    { oper: "eq", text: "等於 " },
                    { oper: "ne", text: "不等於 " },
                    { oper: "lt", text: "小於 " },
                    { oper: "le", text: "小於等於 " },
                    { oper: "gt", text: "大於 " },
                    { oper: "ge", text: "大於等於 " },
                    { oper: "bw", text: "開始於 " },
                    { oper: "bn", text: "不開始於 " },
                    { oper: "in", text: "在其中 " },
                    { oper: "ni", text: "不在其中 " },
                    { oper: "ew", text: "結束於 " },
                    { oper: "en", text: "不結束於 " },
                    { oper: "cn", text: "包含 " },
                    { oper: "nc", text: "不包含 " },
                    { oper: "nu", text: "為空" },
                    { oper: "nn", text: "不為空" }
                ],
                groupOps: [
                    { op: "AND", text: "所有" },
                    { op: "OR", text: "任一" }
                ],
                addGroupTitle: "新增子群組",
                deleteGroupTitle: "刪除群組",
                addRuleTitle: "新增規則",
                deleteRuleTitle: "刪除規則",
                operandTitle: "請選取搜尋條件",
                resetTitle: "重設搜尋值"
            },
            edit: {
                addCaption: "新增",
                editCaption: "編輯",
                bSubmit: "確定",
                bCancel: "取消",
                bClose: "",
                drag: false,
                resize: false,
                clearAfterAdd: true,
                closeAfterEdit: true,
                closeOnEscape: true,
                saveData: "",
                bYes: "",
                bNo: "",
                bExit: "",
                msg: {
                    required: "必填",
                    number: "請輸入數字",
                    minValue: "值必須大於等於",
                    maxValue: "值必須小於等於",
                    email: "不合法的電子郵件",
                    integer: "請輸入整數",
                    date: "請輸入合法的日期格式",
                    url: "不合法的網址. 前綴應為 'http: //' 或 'https: //'",
                    nodefined: "沒有定義!",
                    novalue: "需要回傳值!",
                    customarray: "The custom function should return the array!",
                    customfcheck: "Custom checks should have custom functions"
                },
                width: 600,
                afterSubmit: function (response, postdata) {
                    var data = JSON.parse(response.responseText);
                    return [data.ok > 0, data.message, data.id];
                },
                beforeShowForm: function (formid, method) {

                },
                afterShowForm: function (formid, method) {
                    var model = $(this).jqGrid("getGridParam", "colModel");
                    for (var i in model) {
                        var col = model[i];
                        if (col.readonly === "all" || col.readonly === method) {
                            var disableElemet = function (formid, elementName) {
                                var target = $(formid).find("[name='" + elementName + "']");
                                if (target.length > 0)
                                    target.prop({ "readonly": "readonly", "disabled": "disabled" }).addClass("ui-state-disabled");
                                else
                                    setTimeout(function () { disableElemet(formid, elementName); }, 250);
                            };
                            disableElemet(formid, col.name);
                        }

                    }
                    var targets = ["grid", "subgrid", "subgrid2"];
                    for (var i in targets) {
                        var t = $("#editmod" + targets[i]);
                        if (t.length > 0)
                            t.position({ my: "center center", at: "center center", of: "#gbox_" + targets[i] });
                    }
                }
            },
            view: {
                caption: "",
                bClose: ""
            },
            del: {
                caption: "刪除",
                msg: "確定刪除此筆資料?",
                bSubmit: "刪除",
                bCancel: "取消",
                alertcap: "警告",
                alerttext: "尚未選擇",
                width: 300,
                afterSubmit: function (response, postdata) {
                    var data = JSON.parse(response.responseText);
                    return [data.ok > 0, data.message, data.id];
                },
                afterShowForm: function (formid) {
                    var targets = ["grid", "subgrid", "subgrid2"];
                    for (var i in targets) {
                        var t = $("#delmod" + targets[i]);
                        if (t.length > 0)
                            t.position({ my: "center center", at: "center center", of: "#gbox_" + targets[i] });
                    }
                }
            },
            nav: {
                edittext: "",
                edittitle: "編輯",
                addtext: "",
                addtitle: "新增",
                deltext: "",
                deltitle: "刪除",
                searchtext: "",
                searchtitle: "搜尋",
                refreshtext: "",
                refreshtitle: "重整",
                alertcap: "警告",
                alerttext: "尚未選擇",
                viewtext: "",
                viewtitle: "",
                savetext: "",
                savetitle: "儲存資料列",
                canceltext: "",
                canceltitle: "取消資料列編輯"
            },
            col: {
                caption: "",
                bSubmit: "",
                bCancel: ""
            },
            errors: {
                errcap: "",
                nourl: "",
                norecords: "",
                model: ""
            },
            formatter: {
                integer: { thousandsSeparator: " ", defaultValue: "0" },
                number: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: "0.00" },
                currency: { decimalSeparator: ".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix: "", defaultValue: "0.00" },
                date: {
                    dayNames: [
                        "日", "一", "二", "三", "四", "五", "六",
                        "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"
                    ],
                    monthNames: [
                        "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二",
                        "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"
                    ],
                    AmPm: ["上午", "下午", "上午", "下午"],
                    S: function (j) {
                        return j < 11 || j > 13 ? ["st", "nd", "rd", "th"][Math.min((j - 1) % 10, 3)] : "th";
                    },
                    srcformat: "Y-m-d",
                    newformat: "Y-m-d",
                    masks: {
                        ISO8601Long: "Y-m-d H:i:s",
                        ISO8601Short: "Y-m-d",
                        ShortDate: "Y-m-d",
                        LongDate: "l, F d, Y",
                        FullDateTime: "l, F d, Y g:i:s A",
                        MonthDay: "F d",
                        ShortTime: "g:i A",
                        LongTime: "g:i:s A",
                        YearMonth: "F, Y"
                    }
                }
            }
        };
        $.jgrid = $.jgrid || {};
        $.extend(true, $.jgrid, {
            defaults: {
                locale: "tw",
            },
            locales: {
                // In general the property name is free, but it's recommended to use the names based on
                // http://www.iana.org/assignments/language-subtag-registry/language-subtag-registry
                // http://rishida.net/utils/subtags/ and RFC 5646. See Appendix A of RFC 5646 for examples.
                // One can use the lang attribute to specify language tags in HTML, and the xml:lang attribute for XML
                // if it exists. See http://www.w3.org/International/articles/language-tags/#extlang
                tw: $.extend({}, locInfo, { name: "中文(台灣)", nameEnglish: "Chinese (Traditional, Taiwan)" }),
                "zh-TW": $.extend({}, locInfo, { name: "中文(台灣)", nameEnglish: "Chinese (Traditional, Taiwan)" })
            },
        });

        $.extend($.jgrid.defaults, {
            sortable: true,
        });
    }));
