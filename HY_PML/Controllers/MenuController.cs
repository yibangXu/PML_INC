using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class MenuController : Controller
    {
        private PML db = new PML();
        string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

        public ActionResult _ElementInForm()
        {
            return PartialView();
        }

        [Authorize]
        public ActionResult Index()
        {
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			//頁面的抬頭
			ViewBag.Title = "選單維護";
            ViewBag.ControllerName = "Menu";
            ViewBag.FormPartialName = "_ElementInForm";

            //權限控管
            if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            return View();
        }

        [Authorize]
        public ActionResult Edit(SYS_MenuItem data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index",
                this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            ResultHelper result = new ResultHelper();
            SYS_MenuItem userRecord = db.SYS_MenuItem.FirstOrDefault(x => x.ID == data.ID);

            if (userRecord != null)
            {
                userRecord.Caption = data.Caption;
                userRecord.Sequence = data.Sequence;
                userRecord.IsDelete = data.IsDelete;

                //以下系統自填
                userRecord.UpdatedDate = DateTime.Now;
                userRecord.UpdatedBy = User.Identity.Name;
                try
                {
                    db.Entry(userRecord).State = EntityState.Modified;
                    db.SaveChanges();
                    result.Ok = DataModifyResultType.Success;
                    result.Message = "OK";
                }
                catch (Exception e)
                {
                    result.Ok = DataModifyResultType.Faild;
                    result.Message = e.Message;
                }
            }
            else
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "找不到資料!";
            }
            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult GetGridJSON(SYS_MenuItem data, int page = 1, int rows = 40)
        {
            var menu = from x in db.SYS_MenuItem
                       where x.ParentID == null
                       select new
                       {
                           x.ID,
                           x.Sequence,
                           x.Caption,
                           x.IsDelete
                       };
            if (data.Caption.IsNotEmpty())
                menu = menu.Where(x => x.Caption.Contains(data.Caption));
            if ((data.IsDelete == false && Request["IsDelete"]?.ToLower() == "false") || data.IsDelete == true)
                menu = menu.Where(x => x.IsDelete == data.IsDelete);

            int records = menu.Count();
            menu = menu.OrderBy(x => x.Sequence).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Records = records,
                Pages = page,
                Data = menu,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}