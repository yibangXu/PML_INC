using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class HubController : Controller
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
			ViewBag.Title = "路線資料";
			ViewBag.ControllerName = "Hub";

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
		public ActionResult Add(ORG_Hub data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();

			var duplicated = db.ORG_Hub.Any(x => x.HubNo == data.HubNo && x.IsDelete == false);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複的路線代碼";
			}
			else
			{
				ORG_Hub userRecord = new ORG_Hub();
				userRecord.HubNo = "__" + String.Format("{0:yyyyMMdd}", DateTime.Now);
				userRecord.HubCode = data.HubCode;
				userRecord.HubName = data.HubName;
				userRecord.CustID = data.CustID;
				userRecord.IsServer = data.IsServer;
				userRecord.HubPName = data.HubPName;
				userRecord.PrintLang = data.PrintLang;

				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;

				try
				{
					db.ORG_Hub.Add(userRecord);
					db.SaveChanges();

					var target = db.ORG_Hub.Where(x => x.HubNo == userRecord.HubNo).OrderByDescending(x => x.ID).FirstOrDefault();
					target.HubNo = target.ID.ToString().PadLeft(10, '0');
					db.Entry(target).State = EntityState.Modified;
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
		public ActionResult Edit(ORG_Hub data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_Hub userRecord = db.ORG_Hub.FirstOrDefault(x => x.ID == data.ID);

			if (userRecord != null)
			{
				userRecord.HubCode = data.HubCode;
				userRecord.HubName = data.HubName;
				userRecord.CustID = data.CustID;
				userRecord.IsServer = data.IsServer;
				userRecord.HubPName = data.HubPName;
				userRecord.PrintLang = data.PrintLang;

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
		public ActionResult Delete(ORG_Hub data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_Hub userRecord = db.ORG_Hub.FirstOrDefault(x => x.ID == data.ID);

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
		public ActionResult GetGridJSON(ORG_Hub data, int page = 1, int rows = 40)
		{
			var hub = from h in db.ORG_Hub
					  join c in db.ORG_Cust on h.CustID equals c.ID into ps
					  from c in ps.DefaultIfEmpty()
					  join u in db.SYS_User on h.CreatedBy equals u.Account into ps2
					  from u in ps2.DefaultIfEmpty()
					  where h.IsDelete == false
					  select new
					  {
						  h.ID,
						  h.HubNo,
						  h.HubCode,
						  h.HubName,
						  h.HubPName,
						  h.CustID,
						  CustNo = c == null ? null : c.CustNo,
						  h.IsServer,
						  CreatedBy = u.UserName,
						  h.CreatedDate,
						  h.UpdatedBy,
						  h.UpdatedDate,
						  h.DeletedBy,
						  h.DeletedDate,
						  h.IsDelete,
						  h.PrintLang
					  };

			if (data.HubNo.IsNotEmpty())
				hub = hub.Where(x => x.HubNo.Contains(data.HubNo));
			if (data.HubCode.IsNotEmpty())
				hub = hub.Where(x => x.HubCode.Contains(data.HubCode));
			if (data.HubName.IsNotEmpty())
				hub = hub.Where(x => x.HubName.Contains(data.HubName));
			if ((data.IsServer == false && Request["IsServer"] == "false") || data.IsServer == true)
				hub = hub.Where(x => x.IsServer == data.IsServer);
			if (data.CustNo.IsNotEmpty())
				hub = hub.Where(x => x.CustNo.Contains(data.CustNo));

			var records = hub.Count();
			hub = hub.OrderBy(o => o.HubNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = hub,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}