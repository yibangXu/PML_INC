using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class DiscountController : Controller
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
			ViewBag.Title = "折讓";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "Discount";
			ViewBag.FormCustomJsNew = "";
			ViewBag.FormCustomJsEdit = "";
			//權限控管
			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(),
				this.ControllerContext.RouteData.Values["controller"].ToString()))
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
		public ActionResult Add(Discount data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var saveData = new Discount();

			//編流水號
			{
				var prefix = "D" + DateTime.Now.ToString("yyyyMM");
				var lastCRNo = db.Discount.Where(x => x.CRNo.Contains(prefix)).OrderByDescending(x => x.CRNo).Select(x => x.CRNo).FirstOrDefault();
				if (lastCRNo != null)
					saveData.CRNo = prefix + (Convert.ToInt32(lastCRNo.Substring(7, 4)) + 1).ToString().PadLeft(4, '0');
				else
					saveData.CRNo = prefix + "1".PadLeft(4, '0');
			}
			saveData.CustNo = data.CustNo;
			saveData.CustName = data.CustName;
			saveData.LadingNo = data.LadingNo;
			saveData.Total = data.Total;
			saveData.discount = data.discount;
			saveData.Reason = data.Reason;

			//以下系統自填
			saveData.CreateTime = DateTime.Now;
			saveData.CreateBy = User.Identity.Name;
			saveData.IsDelete = false;
			saveData.IsCheck = false;

			var result = new ResultHelper();
			try
			{
				db.Discount.Add(saveData);
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
		public ActionResult Edit(Discount data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var editData = db.Discount.FirstOrDefault(x => x.CRNo == data.CRNo);

			if (editData != null)
			{
				editData.CustNo = data.CustNo;
				editData.CustName = data.CustName;
				editData.LadingNo = data.LadingNo;
				editData.Total = data.Total;
				editData.discount = data.discount;
				editData.Reason = data.Reason;

				//以下系統自填
				editData.UpdateTime = DateTime.Now;
				editData.UpdateBy = User.Identity.Name;
				try
				{
					db.Entry(editData).State = EntityState.Modified;
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
		public ActionResult Delete(Discount data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.Discount.FirstOrDefault(x => x.CRNo == data.CRNo);
			if (deletedData != null)
			{
				//以下系統自填
				deletedData.DeletedTime = DateTime.Now;
				deletedData.DeletedBy = User.Identity.Name;
				deletedData.IsDelete = true;
				try
				{
					db.Entry(deletedData).State = EntityState.Modified;
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
		public ActionResult GetGridJSON(Discount data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var discount =
				from d in db.Discount.Where(x => x.IsDelete == false)
				join u in db.SYS_User on d.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				select new
				{
					CRNo = d.CRNo,
					CustNo = d.CustNo,
					CustName = d.CustName,
					LadingNo = d.LadingNo,
					Total = d.Total,
					Discount = d.discount,
					Reason = d.Reason,
					CreateBy = u.UserName,
					CreateTime = d.CreateTime,
					UpdateBy = d.UpdateBy,
					UpdateTime = d.UpdateTime,
					DeletedBy = d.DeletedBy,
					DeletedTime = d.DeletedTime,
					IsDelete = d.IsDelete,
					IsCheck = d.IsCheck,
					CheckBy = d.CheckBy,
					CheckTime = d.CheckTime,

				};
			if (data.CRNo != null)
				discount = discount.Where(x => x.CRNo.Contains(data.CRNo));
			if (data.CustName != null)
				discount = discount.Where(x => x.CRNo.Contains(data.CustName));
			if (data.LadingNo != null)
				discount = discount.Where(x => x.LadingNo.Contains(data.LadingNo));

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				discount = discount.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}
			int records = discount.Count();
			discount = discount.OrderBy(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = discount,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}