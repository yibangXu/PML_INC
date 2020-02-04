using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class AreaController : Controller
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
			ViewBag.Title = "區域資料";
			ViewBag.ControllerName = "Area";
			ViewBag.FormPartialName = "_ElementInForm";
			ViewBag.FormCustomJsEdit = "$('#AreaNo').textbox('readonly')";

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
		public ActionResult Add(ORG_Area data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var userRecord = new ORG_Area();
			userRecord.AreaNo = data.AreaNo;
			userRecord.AreaName = data.AreaName;
			userRecord.AreaCode = data.AreaCode;

			//以下系統自填
			userRecord.CreatedDate = DateTime.Now;
			userRecord.CreatedBy = User.Identity.Name;
			userRecord.IsDelete = false;

			var result = new ResultHelper();
			try
			{
				db.ORG_Area.Add(userRecord);
				db.SaveChanges();
				result.Ok = DataModifyResultType.Success;
				result.Message = "OK";
			}
			catch (Exception e)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = e.Message;
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Edit(ORG_Area data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Area.FirstOrDefault(x => x.ID == data.ID);
			if (userRecord != null)
			{
				userRecord.AreaName = data.AreaName;
				userRecord.AreaCode = data.AreaCode;

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
		public ActionResult Delete(ORG_Area data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Area.FirstOrDefault(x => x.ID == data.ID);

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
		public ActionResult GetGridJSON(ORG_Area data, int page = 1, int rows = 40)
		{
			var area =
				from a in db.ORG_Area.Where(x => x.IsDelete == false)
				join u in db.SYS_User
				on a.CreatedBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				select new
				{
					ID = a.ID,
					AreaNo = a.AreaNo,
					AreaName = a.AreaName,
					AreaCode = a.AreaCode,
					CreatedDate = a.CreatedDate,
					CreatedBy = u.UserName,
					UpdatedDate = a.UpdatedDate,
					UpdatedBy = a.UpdatedBy,
					DeletedDate = a.DeletedDate,
					DeletedBy = a.DeletedBy,
					IsDelete = a.IsDelete,
				};

			if (data.AreaNo.IsNotEmpty())
				area = area.Where(x => x.AreaNo.Contains(data.AreaNo));
			if (data.AreaName.IsNotEmpty())
				area = area.Where(x => x.AreaName.Contains(data.AreaName));

			int records = area.Count();
			area = area.OrderBy(o => o.AreaNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = area,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}