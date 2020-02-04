using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class CBDController : Controller
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
			ViewBag.Title = "收現、收票，折讓檢核";
			//ViewBag.AddFunc = "Add";
			//ViewBag.EditFunc = "Edit";
			//ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "CBD";
			ViewBag.FormCustomJsNew = "";
			ViewBag.FormCustomJsEdit = "";
			//權限控管
			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(),
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		[Authorize]
		public ActionResult GetGridJSON(CBD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var dataList = new List<CBD>();
			var cash_Receive = from c in db.Cash_Receive.Where(x => x.IsDelete == false && x.IsCheck == false)
							   join u in db.SYS_User on c.CreateBy equals u.Account into ps
							   from u in ps.DefaultIfEmpty()
							   select new CBD()
							   {
                                   Type = "收現",
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
			var bill_Receive = from b in db.Bill_Receive.Where(x => x.IsDelete == false && x.IsCheck == false)
							   join u in db.SYS_User on b.CreateBy equals u.Account into ps
							   from u in ps.DefaultIfEmpty()
							   select new CBD()
                               {
                                   Type = "收票",
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
			var discount = from d in db.Discount.Where(x => x.IsDelete == false && x.IsCheck == false)
						   join u in db.SYS_User on d.CreateBy equals u.Account into ps
						   from u in ps.DefaultIfEmpty()
						   select new CBD()
                           {
                               Type = "折讓",
                               CRNo = d.CRNo,
							   CustNo = d.CustNo,
							   CustName = d.CustName,
							   LadingNo = d.LadingNo,
							   Total = d.Total,
                               discount = d.discount,
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

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				cash_Receive = cash_Receive.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
				bill_Receive = bill_Receive.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
				discount = discount.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}
			if (data.RecvTime != null)
			{
				var sDate = data.RecvTime.Value.Date;
				var eDate = sDate.AddDays(1);
				cash_Receive = cash_Receive.Where(x => DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(eDate) < 0 && DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(sDate) >= 0);
				bill_Receive = bill_Receive.Where(x => DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(eDate) < 0 && DbFunctions.TruncateTime(x.RecvTime).Value.CompareTo(sDate) >= 0);
			}

            cash_Receive.Concat(bill_Receive).Concat(discount);

            if (data.RecvTime == null)
                cash_Receive.Concat(discount);

			var cbdData = cash_Receive;
			if (data.Type != null)
				cbdData = cbdData.Where(x => x.Type == data.Type);
			if (data.CRNo != null)
				cbdData = cbdData.Where(x => x.CRNo.Contains(data.CRNo));
			if (data.CustName != null)
				cbdData = cbdData.Where(x => x.CustName.Contains(data.CustName));
			if (data.LadingNo != null)
				cbdData = cbdData.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.Receiver != null)
				cbdData = cbdData.Where(x => x.Receiver.Contains(data.Receiver));
			if (data.BillBank != null)
				cbdData = cbdData.Where(x => x.BillBank.Contains(data.BillBank));
			if (data.BillNo != null)
				cbdData = cbdData.Where(x => x.BillNo.Contains(data.BillNo));
			if (data.BillDueDate != null)
				cbdData = cbdData.Where(x => x.BillDueDate == data.BillDueDate);

			var records = cbdData.Count();

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = cbdData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}


		//收現、收票，折讓檢核
		[Authorize]
		public ActionResult CBDCheck(string[] CRNo)
		{
			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				foreach (var i in CRNo)
				{
					switch (i.Substring(0, 1))
					{
						case "C":
							var cData = db.Cash_Receive.Where(x => x.CRNo == i && x.IsDelete == false && x.IsCheck == false).FirstOrDefault();
							cData.IsCheck = true;
							cData.CheckTime = DateTime.Now;
							cData.CheckBy = User.Identity.Name;
							cData.UpdateTime = DateTime.Now;
							cData.UpdateBy = User.Identity.Name;
							db.Entry(cData).State = EntityState.Modified;
							break;
						case "B":
							var bData = db.Bill_Receive.Where(x => x.CRNo == i && x.IsDelete == false && x.IsCheck == false).FirstOrDefault();
							bData.IsCheck = true;
							bData.CheckTime = DateTime.Now;
							bData.CheckBy = User.Identity.Name;
							bData.UpdateTime = DateTime.Now;
							bData.UpdateBy = User.Identity.Name;
							db.Entry(bData).State = EntityState.Modified;
							break;
						case "D":
							var dData = db.Discount.Where(x => x.CRNo == i && x.IsDelete == false && x.IsCheck == false).FirstOrDefault();
							dData.IsCheck = true;
							dData.CheckTime = DateTime.Now;
							dData.CheckBy = User.Identity.Name;
							dData.UpdateTime = DateTime.Now;
							dData.UpdateBy = User.Identity.Name;
							db.Entry(dData).State = EntityState.Modified;
							break;
					}
				}
				try
				{
					db.SaveChanges();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
					trans.Commit();
				}
				catch (Exception e)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = e.Message;
					trans.Rollback();
				}
				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			}
		}
	}
}