using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
    public class UserGroupController : Controller
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
			//頁面的抬頭
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "群組維護";
            ViewBag.ControllerName = "UserGroup";
            ViewBag.FormPartialName = "_ElementInForm";
            ViewBag.FormCustomJsEdit = "$('#UserGroupNo').textbox('readonly')";

            //權限控管
            if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            return View();
        }

        [Authorize]
        public ActionResult getSelectionjqGrid(string formId, string inputFields, string inputValues)
        {
            ViewBag.formId = formId;
            ViewBag.inputValues = inputValues.Split(',');
            ViewBag.inputFields = inputFields.Split(',');
            return View();
        }

        [Authorize]
        public ActionResult Add(SYS_UserGroup data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index",
                this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();

            var duplicated = db.SYS_UserGroup.Any(x => x.UserGroupNo == data.UserGroupNo);
            if (duplicated)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "已存在重複的代號";
            }
            else
            {
                SYS_UserGroup userRecord = new SYS_UserGroup();
                userRecord.UserGroupNo = data.UserGroupNo;
                userRecord.UserGroupName = data.UserGroupName;
                userRecord.Remark = data.Remark;
                userRecord.IsActive = data.IsActive;

                //以下系統自填
                userRecord.CreatedDate = DateTime.Now;
                userRecord.CreatedBy = User.Identity.Name;
                userRecord.IsDelete = false;

                try
                {
                    db.SYS_UserGroup.Add(userRecord);
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

            return Content(JsonConvert.SerializeObject(result),
                WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult Edit(SYS_UserGroup data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index",
                 this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            SYS_UserGroup userRecord = db.SYS_UserGroup.FirstOrDefault(x => x.UserGroupNo == data.UserGroupNo);

            if (userRecord != null)
            {
                userRecord.UserGroupNo = data.UserGroupNo;
                userRecord.UserGroupName = data.UserGroupName;
                userRecord.Remark = data.Remark;
                userRecord.IsActive = data.IsActive;

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
            return Content(JsonConvert.SerializeObject(result),
                WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult Delete(SYS_UserGroup data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index",
                this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            SYS_UserGroup userRecord = db.SYS_UserGroup.FirstOrDefault(x => x.UserGroupNo == data.UserGroupNo);

            if (userRecord != null)
            {
                //以下系統自填
                userRecord.DeletedDate = DateTime.Now;
                userRecord.DeletedBy = User.Identity.Name;
                userRecord.IsDelete = true;
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

            return Content(JsonConvert.SerializeObject(result),
                WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult GetGridJSON(SYS_UserGroup data, int page = 1, int rows = 40)
        {
            var userGroup = db.SYS_UserGroup.Where(x => x.IsDelete == false);

            if (data.UserGroupNo.IsNotEmpty())
                userGroup = userGroup.Where(x => x.UserGroupNo.Contains(data.UserGroupNo));
            if (data.UserGroupName.IsNotEmpty())
                userGroup = userGroup.Where(x => x.UserGroupName.Contains(data.UserGroupName));
            if ((data.IsActive == false && Request["IsActive"]?.ToLower() == "false") || data.IsActive == true)
                userGroup = userGroup.Where(x => x.IsActive == data.IsActive);

            int records = userGroup.Count();
            userGroup = userGroup.OrderBy(x => x.UserGroupNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Message = "OK",
                Records = records,
                Pages = page,
                Data = userGroup,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}