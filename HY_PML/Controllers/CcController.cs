using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class CcController : Controller
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
			ViewBag.Title = "付款方式";
			ViewBag.ControllerName = "Cc";
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
		public ActionResult Add(ORG_Cc data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var duplicated = db.ORG_Cc.Any(x => x.CcNo == data.CcNo && x.IsDelete == false);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複的付款代碼";
			}
			else
			{
				var userRecord = new ORG_Cc();
				userRecord.CcNo = data.CcNo;
				userRecord.CcName = data.CcName;
				userRecord.IsServer = data.IsServer;

				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;

				try
				{
					db.ORG_Cc.Add(userRecord);
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
		public ActionResult Edit(ORG_Cc data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Cc.FirstOrDefault(x => x.ID == data.ID);
			if (userRecord != null)
			{
				userRecord.CcNo = data.CcNo;
				userRecord.CcName = data.CcName;
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
		public ActionResult Delete(ORG_Cc data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Cc.FirstOrDefault(x => x.ID == data.ID);
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
		public ActionResult GetGridJSON(ORG_Cc data, int page = 1, int rows = 40)
		{
			var cc =
				from c in db.ORG_Cc.Where(x => x.IsDelete == false)
				join u in db.SYS_User on c.CreatedBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				select new
				{
					ID = c.ID,
					CcNo = c.CcNo,
					CcName = c.CcName,
					IsServer = c.IsServer,
					CreatedBy = u.UserName,
					CreatedDate = c.CreatedDate,
					UpdatedBy = c.UpdatedBy,
					UpdatedDate = c.UpdatedDate,
					DeletedBy = c.DeletedBy,
					DeletedDate = c.DeletedDate,
					IsDelete = c.IsDelete,
				};

			if (data.CcNo.IsNotEmpty())
				cc = cc.Where(x => x.CcNo.Contains(data.CcNo));
			if (data.CcName.IsNotEmpty())
				cc = cc.Where(x => x.CcName.Contains(data.CcName));
			if (Request["IsServer"] == "false")
				cc = cc.Where(x => x.IsServer == data.IsServer);

			var records = cc.Count();
			cc = cc.OrderBy(o => o.CcNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = cc,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}