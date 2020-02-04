using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class DepartController : Controller
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
			ViewBag.Title = "部門資料";
            ViewBag.ControllerName = "Depart";
            ViewBag.FormPartialName = "_ElementInForm";

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
        public ActionResult Add(ORG_Depart data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index",
                this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            ResultHelper result = new ResultHelper();
            bool duplicated = db.ORG_Depart.Any(x => x.DepartNo == data.DepartNo);
            if (duplicated)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "已存在重複的代號";
            }
            else
            {
                ORG_Depart userRecord = new ORG_Depart();
                userRecord.DepartNo = data.DepartNo;
                userRecord.DepartName = data.DepartName;
                userRecord.IsActive = data.IsActive;
                userRecord.Remark = data.Remark;

                //以下系統自填
                userRecord.CreatedDate = DateTime.Now;
                userRecord.CreatedBy = User.Identity.Name;
                userRecord.IsDelete = false;

                try
                {
                    db.ORG_Depart.Add(userRecord);
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
            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult Edit(ORG_Depart data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index",
                 this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            ResultHelper result = new ResultHelper();
            ORG_Depart userRecord = db.ORG_Depart.FirstOrDefault(x => x.DepartNo == data.DepartNo);
            if (userRecord != null)
            {
                userRecord.DepartNo = data.DepartNo;
                userRecord.DepartName = data.DepartName;
                userRecord.IsActive = data.IsActive;
                userRecord.Remark = data.Remark;

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
        public ActionResult Delete(ORG_Depart data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index",
               this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            ResultHelper result = new ResultHelper();
            ORG_Depart userRecord = db.ORG_Depart.FirstOrDefault(x => x.DepartNo == data.DepartNo);

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

            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult GetGridJSON(ORG_Depart data, int page = 1, int rows = 40, string target = "", string IsActive = "false")
        {
            var depart = db.ORG_Depart.Where(x => x.IsDelete == false);

            if (data.DepartNo.IsNotEmpty())
                depart = depart.Where(x => x.DepartNo.Contains(data.DepartNo));
            if (data.DepartName.IsNotEmpty())
                depart = depart.Where(x => x.DepartName.Contains(data.DepartNo));
            if ((data.IsActive == false && Request["IsActive"]?.ToLower() == "false") || data.IsActive == true)
                depart = depart.Where(x => x.IsActive == data.IsActive);

            int records = depart.Count();
            depart = depart.OrderBy(x => x.DepartNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Message = "OK",
                Records = records,
                Pages = page,
                Data = depart,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}