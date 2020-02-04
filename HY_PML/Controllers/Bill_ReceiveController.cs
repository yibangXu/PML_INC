using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class Bill_ReceiveController : Controller
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
			ViewBag.Title = "收到票據";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "Bill_Receive";
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
		public ActionResult Add(Bill_Receive data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var saveData = new Bill_Receive();

			//編流水號
			{
				var prefix = "B" + DateTime.Now.ToString("yyyyMM");
				var lastCRNo = db.Bill_Receive.Where(x => x.CRNo.Contains(prefix)).OrderByDescending(x => x.CRNo).Select(x => x.CRNo).FirstOrDefault();
				if (lastCRNo != null)
					saveData.CRNo = prefix + (Convert.ToInt32(lastCRNo.Substring(7, 4)) + 1).ToString().PadLeft(4, '0');
				else
					saveData.CRNo = prefix + "1".PadLeft(4, '0');
			}
			saveData.CustNo = data.CustNo;
			saveData.CustName = data.CustName;
			saveData.LadingNo = data.LadingNo;
			saveData.Total = data.Total;
			saveData.RecvTime = data.RecvTime;
			saveData.Receiver = User.Identity.Name;
			saveData.BillBank = data.BillBank;
			saveData.BillNo = data.BillNo;
			saveData.BillDueDate = data.BillDueDate;
			saveData.BillAmount = data.BillAmount;
			saveData.Remark = data.Remark;

			//以下系統自填
			saveData.CreateTime = DateTime.Now;
			saveData.CreateBy = User.Identity.Name;
			saveData.IsDelete = false;
			saveData.IsCheck = false;

			var result = new ResultHelper();
			try
			{
				db.Bill_Receive.Add(saveData);
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
		public ActionResult Edit(Bill_Receive data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var editData = db.Bill_Receive.FirstOrDefault(x => x.CRNo == data.CRNo);

			if (editData != null)
			{
				editData.CustNo = data.CustNo;
				editData.CustName = data.CustName;
				editData.LadingNo = data.LadingNo;
				editData.Total = data.Total;
				editData.RecvTime = data.RecvTime;
				editData.BillBank = data.BillBank;
				editData.BillNo = data.BillNo;
				editData.BillDueDate = data.BillDueDate;
				editData.BillAmount = data.BillAmount;
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
		public ActionResult Delete(Bill_Receive data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.Bill_Receive.FirstOrDefault(x => x.CRNo == data.CRNo);
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
		public ActionResult GetGridJSON(Bill_Receive data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var bill_Receive =
				from b in db.Bill_Receive.Where(x => x.IsDelete == false)
				join u in db.SYS_User on b.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				select new
				{
					CRNo = b.CRNo,
					CustNo = b.CustNo,
					CustName = b.CustName,
					LadingNo = b.LadingNo,
					Total = b.Total,
					RecvTime = b.RecvTime,
					Receiver = b.Receiver,
					BillBank = b.BillBank,
					BillNo = b.BillNo,
					BillDueDate = b.BillDueDate,
					BillAmount = b.BillAmount,
					Remark = b.Remark,
					CreateBy = u.UserName,
					CreateTime = b.CreateTime,
					UpdateBy = b.UpdateBy,
					UpdateTime = b.UpdateTime,
					DeletedBy = b.DeletedBy,
					DeletedTime = b.DeletedTime,
					IsDelete = b.IsDelete,
					IsCheck = b.IsCheck,
					CheckBy = b.CheckBy,
					CheckTime = b.CheckTime,
				};

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				bill_Receive = bill_Receive.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}
			if (data.CRNo != null)
				bill_Receive = bill_Receive.Where(x => x.CRNo.Contains(data.CRNo));
			if (data.CustName != null)
				bill_Receive = bill_Receive.Where(x => x.CustName.Contains(data.CustName));
			if (data.LadingNo != null)
				bill_Receive = bill_Receive.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.RecvTime != null)
			{
				var sDate = data.RecvTime.Value.Date;
				var eDate = sDate.AddDays(1);
				bill_Receive = bill_Receive.Where(x => DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(eDate) < 0 && DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(sDate) >= 0);
			}
			if (data.Receiver != null)
				bill_Receive = bill_Receive.Where(x => x.Receiver.Contains(data.Receiver));
			if (data.BillBank != null)
				bill_Receive = bill_Receive.Where(x => x.BillBank.Contains(data.BillBank));
			if (data.BillNo != null)
				bill_Receive = bill_Receive.Where(x => x.BillNo.Contains(data.BillNo));
			if (data.BillDueDate != null)
				bill_Receive = bill_Receive.Where(x => x.BillDueDate == data.BillDueDate);

			int records = bill_Receive.Count();
			bill_Receive = bill_Receive.OrderBy(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = bill_Receive,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}