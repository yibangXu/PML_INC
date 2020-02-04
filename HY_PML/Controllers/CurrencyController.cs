using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class CurrencyController : Controller
    {
        private PML db = new PML();
        string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

        public ActionResult _ElementInForm()
        {
            return PartialView();
        }

        // GET: Currency
        [Authorize]
        public ActionResult Index()
        {
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "幣別資料";
            ViewBag.ControllerName = "Currency";
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
        public ActionResult Add(ORG_Currency data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            var duplicated = db.ORG_Currency.Any(x => x.CurrencyNo == data.CurrencyNo && x.IsDelete == false);
            if (duplicated)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "已存在重複的幣別代碼";
            }
            else
            {
                var userRecord = new ORG_Currency();
                userRecord.CurrencyNo = data.CurrencyNo;
                userRecord.CurrencyName = data.CurrencyName;
                userRecord.Exch = data.Exch;
                userRecord.IsServer = data.IsServer;

                //以下系統自填
                userRecord.CreatedDate = DateTime.Now;
                userRecord.CreatedBy = User.Identity.Name;
                userRecord.IsDelete = false;

                try
                {
                    db.ORG_Currency.Add(userRecord);
                    db.SaveChanges();
                    result.Ok = DataModifyResultType.Success;
                    result.Message = "OK";
                }
                catch (Exception e)
                {
                    result.Ok = DataModifyResultType.Success;
                    result.Message = e.Message;
                }
            }
            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult Edit(ORG_Currency data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            var duplicated = db.ORG_Currency.Any(x => x.ID != data.ID && x.CurrencyNo == data.CurrencyNo && x.IsDelete == false);
            if (duplicated)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "已存在重複的幣別代碼";
            }
            else
            {
                var userRecord = db.ORG_Currency.FirstOrDefault(x => x.ID == data.ID);
                if (userRecord != null)
                {
                    userRecord.CurrencyName = data.CurrencyName;
                    userRecord.Exch = data.Exch;
                    userRecord.IsServer = data.IsServer;
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
            }
            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult Delete(ORG_Currency data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            var userRecord = db.ORG_Currency.FirstOrDefault(x => x.ID == data.ID);
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
        public ActionResult GetGridJSON(ORG_Currency data, int page = 1, int rows = 40)
        {
            var currency = db.ORG_Currency.Where(x => x.IsDelete == false);
            if (data.CurrencyNo.IsNotEmpty())
                currency = currency.Where(x => x.CurrencyNo.Contains(data.CurrencyNo));
            if (data.CurrencyName.IsNotEmpty())
                currency = currency.Where(x => x.CurrencyName.Contains(data.CurrencyName));
            if ((data.IsServer == false && Request["IsServer"] == "false") || data.IsServer == true)
                currency = currency.Where(x => x.IsServer == data.IsServer);

            var records = currency.Count();
            currency = currency.OrderBy(o => o.CurrencyNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Data = currency,
                Records = records,
                Pages = page,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}