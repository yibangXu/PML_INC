using System.Web;
using System.Web.Optimization;

namespace HY_PML
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js",
                     "~/Scripts/jquery-{version}.min.js",
                    "~/Scripts/jquery-ui-{version}.js",
                    "~/Scripts/free-jqGrid/jquery.jqgrid.src.js",
                    "~/Scripts/free-jqGrid/grid.customize.js",
                    "~/Scripts/free-jqGrid/jQuery.jqGrid.autoWidthColumns.js",
                    "~/Scripts/jstree.min.js",
                    "~/Scripts/moment.min.js",
                    "~/Scripts/daterangepicker/*.js",
                    "~/Scripts/datetimepicker/*.js",
                    "~/Content/easyui/jquery.easyui.min.js",
                    "~/Scripts/Lib/*.js"));

            /* "~/Content/easyui/jquery.easyui.min.js",*/

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Scripts/bootstrap.js",
                     "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/report/a4").Include(
                      "~/Content/report/base/*.css",
                      "~/Content/report/a4.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/jquery-ui.theme.css",
                      "~/Content/ui.jqgrid.css",
                      "~/Content/style.min.css",
                      "~/Content/datetimepicker/*.css",
                      "~/Content/daterangepicker.css",
                      "~/Content/easyui/themes/default/easyui.css",
                      "~/Content/easyui/themes/icon.css",
                      "~/Content/jquery-ui-timepicker-addon.css",
                      "~/Content/adj/*.css"));

            /*
              "~/Content/easyui/themes/default/easyui.css",
                      "~/Content/easyui/themes/icon.css",
             */
        }
    }
}
