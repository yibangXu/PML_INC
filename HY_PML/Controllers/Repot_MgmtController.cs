using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class Report_MgmtController : Controller
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
			ViewBag.Title = "報表資料";
			ViewBag.ControllerName = "Report_Mgmt";


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
		public ActionResult Add(ORG_Report_Mgmt data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();

			var duplicated = db.ORG_Report_Mgmt.Any(x => x.ReportCName == data.ReportCName && x.HubNo == data.HubNo && x.IsDelete == false);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = $"已存在重複的報表名稱+路線";
			}
			else
			{
				ORG_Report_Mgmt userRecord = new ORG_Report_Mgmt();
				userRecord.ReportCName = data.ReportCName;
				userRecord.ReportEName = data.ReportEName;
				userRecord.ReportCode = data.ReportCode;
				userRecord.HubNo = data.HubNo;
				userRecord.IsBackfill = data.IsBackfill;
				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;

				try
				{
					db.ORG_Report_Mgmt.Add(userRecord);
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
		public ActionResult Edit(ORG_Report_Mgmt data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_Report_Mgmt userRecord = db.ORG_Report_Mgmt.FirstOrDefault(x => x.ID == data.ID);
			var duplicated = db.ORG_Report_Mgmt.Where(x => x.ReportCName == data.ReportCName && x.HubNo == data.HubNo && x.IsDelete == false).ToList().Count();
			if (userRecord != null)
			{
				if (duplicated > 1)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = $"已存在重複的報表名稱+路線";
				}
				else
				{
					userRecord.ReportCName = data.ReportCName;
					userRecord.ReportEName = data.ReportEName;
					userRecord.ReportCode = data.ReportCode;
					userRecord.HubNo = data.HubNo;
					userRecord.IsBackfill = data.IsBackfill;

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
			}
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "找不到資料!";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Delete(ORG_Report_Mgmt data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_Report_Mgmt userRecord = db.ORG_Report_Mgmt.FirstOrDefault(x => x.ID == data.ID);

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
		public ActionResult GetGridJSON(ORG_Report_Mgmt data, int page = 1, int rows = 40)
		{
			var report_Mgmt = from r in db.ORG_Report_Mgmt.Where(x => x.IsDelete == false)
							  join h in db.ORG_Hub.Where(x => x.IsDelete == false) on r.HubNo equals h.HubNo into ps
							  from h in ps.DefaultIfEmpty()
							  join u in db.SYS_User on r.CreatedBy equals u.Account into ps2
							  from u in ps2.DefaultIfEmpty()
							  select new
							  {
								  r.ID,
								  r.ReportCName,
								  r.ReportEName,
								  r.ReportCode,
								  r.IsBackfill,
								  r.HubNo,
								  HubCode = h == null ? null : h.HubCode,
								  HubName = h == null ? null : h.HubName,
								  CreatedBy = u.UserName,
								  r.CreatedDate,
								  r.UpdatedBy,
								  r.UpdatedDate,
								  r.DeletedBy,
								  r.DeletedDate,
								  r.IsDelete
							  };

			if (data.ReportCName.IsNotEmpty())
				report_Mgmt = report_Mgmt.Where(x => x.ReportCName.Contains(data.ReportCName));
			if (data.ReportEName.IsNotEmpty())
				report_Mgmt = report_Mgmt.Where(x => x.ReportCName.Contains(data.ReportEName));
			if (data.HubNo.IsNotEmpty())
				report_Mgmt = report_Mgmt.Where(x => x.HubNo == data.HubNo);
			if (data.HubName.IsNotEmpty())
				report_Mgmt = report_Mgmt.Where(x => x.HubName == data.HubName);

			var records = report_Mgmt.Count();
			report_Mgmt = report_Mgmt.OrderBy(o => o.ReportCName).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = report_Mgmt,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}