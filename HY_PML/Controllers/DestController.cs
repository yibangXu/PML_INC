using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class DestController : Controller
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
			ViewBag.Title = "目的地資料";
            ViewBag.ControllerName = "Dest";
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
        public ActionResult Add(ORG_Dest data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            var duplicated = db.ORG_Dest.Any(x => x.DestNo == data.DestNo && x.IsDelete == false);
            if (duplicated)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "已存在重複的目的地代碼";
            }
            else
            {
                var userRecord = new ORG_Dest();
                userRecord.DestNo = data.DestNo;
                userRecord.CName = data.CName;
                userRecord.ChName = data.ChName;
                userRecord.Zone = data.Zone;
                userRecord.State = data.State;
                userRecord.Country = data.Country;
                userRecord.Areas = data.Areas;
                userRecord.Zip = data.Zip;
                userRecord.Tel = data.Tel;
                userRecord.StatID = data.StatID;
                userRecord.AreaID = data.AreaID;
                userRecord.CurrencyID = data.CurrencyID;
                userRecord.IsServer = data.IsServer;

                //以下系統自填
                userRecord.CreatedDate = DateTime.Now;
                userRecord.CreatedBy = User.Identity.Name;
                userRecord.IsDelete = false;

                try
                {
                    db.ORG_Dest.Add(userRecord);
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
        public ActionResult Edit(ORG_Dest data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            var userRecord = db.ORG_Dest.FirstOrDefault(x => x.ID == data.ID);

            if (userRecord != null)
            {
                userRecord.CName = data.CName;
                userRecord.ChName = data.ChName;
                userRecord.Zone = data.Zone;
                userRecord.State = data.State;
                userRecord.Country = data.Country;
                userRecord.Areas = data.Areas;
                userRecord.Zip = data.Zip;
                userRecord.Tel = data.Tel;
                userRecord.StatID = data.StatID;
                userRecord.AreaID = data.AreaID;
                userRecord.CurrencyID = data.CurrencyID;
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
            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult Delete(ORG_Dest data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            ORG_Dest userRecord = db.ORG_Dest.FirstOrDefault(x => x.ID == data.ID);

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
        public ActionResult GetGridJSON(ORG_Dest data, int page = 1, int rows = 40)
        {
            var dest = from d in db.ORG_Dest
                       join c in db.ORG_Currency on d.CurrencyID equals c.ID into ps
                       from c in ps.DefaultIfEmpty()
                       join a in db.ORG_Area on d.AreaID equals a.ID into ps2
                       from a in ps2.DefaultIfEmpty()
                       join s in db.ORG_Stat on d.StatID equals s.ID into ps3
                       from s in ps3.DefaultIfEmpty()
					   join u in db.SYS_User  on d.CreatedBy equals u.Account into ps4
					   from u in ps4.DefaultIfEmpty()
                       where d.IsDelete == false
                       select new {
                           d.ID,
                           d.DestNo,
                           Dest = d.CName,
                           d.CName,
                           d.ChName,
                           d.Zone,
                           d.State,
                           d.Country,
                           d.Areas,
                           d.Zip,
                           d.Tel,
                           d.StatID,
                           StatNo = s == null ?null :s.StatNo,
                           d.AreaID,
                           AreaNo = a == null ? null : a.AreaNo,
                           d.CurrencyID,
                           CurrencyNo = c == null ?null:c.CurrencyNo,
                           d.IsServer,
                           CreatedBy = u.UserName,
                           d.CreatedDate,
                           d.UpdatedBy,
                           d.UpdatedDate,
                           d.DeletedBy,
                           d.DeletedDate,
                           d.IsDelete
                       };

            if (data.DestNo.IsNotEmpty())
                dest = dest.Where(x => x.DestNo.Contains(data.DestNo));
            if (data.ChName.IsNotEmpty())
                dest = dest.Where(x => x.ChName.Contains(data.ChName));
            if (data.Dest.IsNotEmpty())
                dest = dest.Where(x => x.Dest.Contains(data.Dest));
            if (data.CName.IsNotEmpty())
                dest = dest.Where(x => x.CName.Contains(data.CName));
            if (data.Zone.IsNotEmpty())
                dest = dest.Where(x => x.Zone.Contains(data.Zone));
            if (data.State.IsNotEmpty())
                dest = dest.Where(x => x.State.Contains(data.State));
            if (data.Country.IsNotEmpty())
                dest = dest.Where(x => x.Country.Contains(data.Country));
            if (data.Areas.IsNotEmpty())
                dest = dest.Where(x => x.Areas.Contains(data.Areas));
            if (data.Zip.IsNotEmpty())
                dest = dest.Where(x => x.Zip.Contains(data.Zip));
            if (data.Tel.IsNotEmpty())
                dest = dest.Where(x => x.Tel.Contains(data.Tel));
            if (data.StatNo.IsNotEmpty())
                dest = dest.Where(x => x.StatNo.Contains(data.StatNo));
            if (data.AreaNo.IsNotEmpty())
                dest = dest.Where(x => x.AreaNo.Contains(data.AreaNo));
            if (data.CurrencyNo.IsNotEmpty())
                dest = dest.Where(x => x.CurrencyNo.Contains(data.CurrencyNo));
            if ((data.IsServer == false && Request["IsServer"] == "false") || data.IsServer == true)
                dest = dest.Where(x => x.IsServer == data.IsServer);

            var records = dest.Count();
            dest = dest.OrderBy(o => o.DestNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Data = dest,
                Records = records,
                Pages = page,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}