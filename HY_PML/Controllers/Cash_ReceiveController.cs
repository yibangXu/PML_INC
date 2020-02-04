using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class Cash_ReceiveController : Controller
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
			ViewBag.Title = "收到現金";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "Cash_Receive";
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
		public ActionResult Add(Cash_Receive data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var saveData = new Cash_Receive();

			//編流水號
			{
				var prefix = "C" + DateTime.Now.ToString("yyyyMM");
				var lastCRNo = db.Cash_Receive.Where(x => x.CRNo.Contains(prefix)).OrderByDescending(x => x.CRNo).Select(x => x.CRNo).FirstOrDefault();
				if (lastCRNo != null)
					saveData.CRNo = prefix + (Convert.ToInt32(lastCRNo.Substring(7, 4)) + 1).ToString().PadLeft(4, '0');
				else
					saveData.CRNo = prefix + "1".PadLeft(4, '0');
			}
			saveData.CustNo = data.CustNo;
			saveData.CustName = data.CustName;
			saveData.LadingNo = data.LadingNo;
			saveData.Total = data.Total;
			saveData.CashRecv = data.CashRecv;
			saveData.RecvTime = data.RecvTime;
			saveData.Receiver = User.Identity.Name;
			saveData.Remark = data.Remark;

			//以下系統自填
			saveData.CreateTime = DateTime.Now;
			saveData.CreateBy = User.Identity.Name;
			saveData.IsDelete = false;
			saveData.IsCheck = false;


			var result = new ResultHelper();
			try
			{
				db.Cash_Receive.Add(saveData);
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
		public ActionResult Edit(Cash_Receive data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var editData = db.Cash_Receive.FirstOrDefault(x => x.CRNo == data.CRNo);

			if (editData != null)
			{

				editData.CustNo = data.CustNo;
				editData.CustName = data.CustName;
				editData.LadingNo = data.LadingNo;
				editData.Total = data.Total;
				editData.CashRecv = data.CashRecv;
				editData.RecvTime = data.RecvTime;
				editData.Remark = data.Remark;

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
		public ActionResult Delete(Cash_Receive data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.Cash_Receive.FirstOrDefault(x => x.CRNo == data.CRNo);
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
		public ActionResult GetGridJSON(Cash_Receive data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var cash_Receive =
				from c in db.Cash_Receive.Where(x => x.IsDelete == false)
				join u in db.SYS_User on c.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				select new
				{
					CRNo = c.CRNo,
					CustNo = c.CustNo,
					CustName = c.CustName,
					LadingNo = c.LadingNo,
					Total = c.Total,
					CashRecv = c.CashRecv,
					RecvTime = c.RecvTime,
					Receiver = c.Receiver,
					Remark = c.Remark,
					CreateBy = u.UserName,
					CreateTime = c.CreateTime,
					UpdateBy = c.UpdateBy,
					UpdateTime = c.UpdateTime,
					DeletedBy = c.DeletedBy,
					DeletedTime = c.DeletedTime,
					IsDelete = c.IsDelete,
					IsCheck = c.IsCheck,
					CheckBy = c.CheckBy,
					CheckTime = c.CheckTime,
				};

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				cash_Receive = cash_Receive.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}
			if (data.CRNo != null)
				cash_Receive = cash_Receive.Where(x => x.CRNo.Contains(data.CRNo));
			if (data.CustName != null)
				cash_Receive = cash_Receive.Where(x => x.CustName.Contains(data.CustName));
			if (data.LadingNo != null)
				cash_Receive = cash_Receive.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.RecvTime != null)
			{
				var sDate = data.RecvTime.Value.Date;
				var eDate = sDate.AddDays(1);
				cash_Receive = cash_Receive.Where(x => DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(eDate) < 0 && DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(sDate) >= 0);
			}
			if (data.Receiver != null)
				cash_Receive = cash_Receive.Where(x => x.Receiver.Contains(data.Receiver));

			int records = cash_Receive.Count();
			cash_Receive = cash_Receive.OrderBy(o => o.CRNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = cash_Receive,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}