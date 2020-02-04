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
	public class Bill_Lading_IsCheckController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;
		public ActionResult _ElementInForm1()
		{
			return PartialView();
		}
		public ActionResult _ElementInForm2()
		{
			return PartialView();
		}
		public ActionResult _ElementInForm3()
		{
			return PartialView();
		}

		[Authorize]
		public ActionResult Index()
		{
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			//取得登入者 站點資訊
			string cId = WebSiteHelper.CurrentUserID;
			int dbId = int.Parse(WebSiteHelper.CurrentUserDBID);
			var userInfo = from s in db.SYS_User
						   where s.IsDelete == false && s.ID == dbId
						   select s;
			string statNo = userInfo.First().StatNo;
			bool isTw = true;
			var statData = db.ORG_Stat.Where(x => x.StatNo == statNo).FirstOrDefault();
			if (statData.AreaID != 2)
				isTw = false;
			string statName = statData.StatName;

			ViewBag.Title = "提單檢核";
			ViewBag.dlgWidth = "960px";
			ViewBag.ControllerName = "Bill_Lading_IsCheck";
			ViewBag.AddFunc = "";
			ViewBag.EditFunc = "";
			ViewBag.DelFunc = "";
			ViewBag.SaveRecFunc = "";
			ViewBag.FormCustomJsNew = "";
			ViewBag.FormCustomJsEdit = "";
			ViewBag.PageParam = isTw ? "2" : "1";

			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		#region 主表GRID
		[Authorize]
		public ActionResult GetGridJSON(Bill_Lading data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var statNoList = new List<string>();
			var statNoSession = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
			if (User.Identity.Name != "hyAdmin" && (statNoSession != "hyAdmin") && (statNoSession != "TNNCON"))
			{
				if (statNoSession != "" && statNoSession != null)
				{
					statNoList.Add(statNoSession);
				}
			}
			else
			{
				statNoList = db.ORG_Stat.Where(x => x.IsDelete == false).Select(x => x.StatNo).ToList();
				statNoList.Add("");
				statNoList.Add(null);
			}

			var billLading =
				from b in db.Bill_Lading
				join h in db.ORG_Hub
				on b.HubNo equals h.HubNo into ps1
				from h in ps1.DefaultIfEmpty()
				join u in db.SYS_User on b.CreateBy equals u.Account into ps2
				from u in ps2.DefaultIfEmpty()
				where statNoList.Contains(b.SStatNo) && b.IsConfirm == true && b.IsDelete == false
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					WarehouseRNo = b.WarehouseRNo,
					HubNo = b.HubNo,
					HubName = h == null ? null : h.HubName,
					WarehouseRDate = b.WarehouseRDate,
					TransferNo = b.TransferNo,
					LadingDate = b.LadingDate,
					OrderNo = b.OrderNo,
					SendCustNo = b.SendCustNo,
					SendCHName = b.SendCHName,
					SendPhone = b.SendPhone,
					SendBy = b.SendBy,
					SendCustAddr = b.SendCustAddr,
					RecPhone = b.RecPhone,
					RecCustEName1 = b.RecCustEName1,
					RecCustEName2 = b.RecCustEName2,
					RecBy = b.RecBy,
					RecCompany = b.RecCompany,
					RecCustCHName = b.RecCustCHName,
					RecCustENAddr1 = b.RecCustENAddr1,
					RecCustENAddr2 = b.RecCustENAddr2,
					RecChAddr = b.RecChAddr,
					RecInvNo = b.RecInvNo,
					RecCity = b.RecCity,
					RecState = b.RecState,
					RecCountry = b.RecCountry,
					RecPostDist = b.RecPostDist,
					SectorNo = b.SectorNo,
					SectorName = b.SectorName,
					SStatNo = b.SStatNo,
					SStatName = b.SStatName,
					AStatName = b.AStatName,
					AStatNo = b.AStatNo,
					DestNo = b.DestNo,
					CName = b.CName,
					Type = b.Type,
					ProductNo = b.ProductNo,
					ProductName = b.ProductName,
					PiecesNo = b.PiecesNo,
					Qty = b.Qty,
					Weight = b.Weight,
					Volume = b.Volume,
					Cost = b.Cost,
					CostCurrency = b.CostCurrency,
					CcNo = b.CcNo,
					PayCustNo = b.PayCustNo,
					PayCustCHName = b.PayCustCHName,
					Freight = b.Freight,
					FuelCosts = b.FuelCosts,
					ToPayment = b.ToPayment,
					ToPaymentCurrency = b.ToPaymentCurrency,
					AgentPay = b.AgentPay,
					AgentPayCurrency = b.AgentPayCurrency,
					ProdIdPay = b.ProdIdPay,
					CustomsPay = b.CustomsPay,
					InsurancePay = b.InsurancePay,
					OtherPayTax = b.OtherPayTax,
					OtherPayNoTax = b.OtherPayNoTax,
					Length = b.Length,
					Width = b.Width,
					Height = b.Height,
					Total = b.Total,
					Remark = b.Remark,
					ImOrEx = b.ImOrEx == "Ex" ? "出口" : "進口",
					PhoneCheckTime = b.PhoneCheckTime,
					Status = b.Status,
					StatusTime = b.StatusTime,
					Source = b.Source,
					IsConfirm = b.IsConfirm,
					ConfirmBy = b.ConfirmBy,
					IsCheck = b.IsCheck,
					CheckBy = b.CheckBy,
					CheckTime = b.CheckTime,
					CreateBy = u.UserName,
					CreateTime = b.CreateTime,
					UpdateBy = b.UpdateBy,
					UpdateTime = b.UpdateTime,
					DeletedBy = b.DeletedBy,
					DeletedTime = b.DeletedTime,
					IsDelete = b.IsDelete,
					ShdetNo = b.ShdetNo,
				};

			if (data.LadingNo.IsNotEmpty())
				billLading = billLading.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type.IsNotEmpty())
				billLading = billLading.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.WarehouseRNo.IsNotEmpty())
				billLading = billLading.Where(x => x.WarehouseRNo.Contains(data.WarehouseRNo));
			if (data.HubName.IsNotEmpty())
				billLading = billLading.Where(x => x.HubName == data.HubName);
			if (data.IsCheck.HasValue)
				billLading = billLading.Where(x => x.IsCheck == data.IsCheck);
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				billLading = billLading.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			int records = billLading.Count();
			billLading = billLading.OrderBy(o => o.LadingNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = billLading,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		[Authorize]
		public ActionResult BillLadingCheck(string[] LadingNo)
		{
			var result = new ResultHelper();
			var repeat = false;
			using (var trans = db.Database.BeginTransaction())
			{
				var newdata = new List<Export_Bill>();
				foreach (var l in LadingNo)
				{
					var data = db.Bill_Lading.Where(x => x.LadingNo == l && x.IsDelete == false && x.IsConfirm == true).FirstOrDefault();
					if (data.IsCheck == true)
						repeat = true;

					data.IsCheck = true;
					data.CheckTime = DateTime.Now;
					data.CheckBy = User.Identity.Name;
					data.UpdateTime = DateTime.Now;
					data.UpdateBy = User.Identity.Name;
					db.Entry(data).State = EntityState.Modified;
				}
				if (repeat == true)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "選擇資料中，已存在檢核過之資料";
					trans.Rollback();
				}
				else
				{
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
				}
				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			}
		}

		[Authorize]
		public ActionResult GetLadingData(string param)
		{
			var data = db.Bill_Lading.Where(x => x.LadingNo == param && x.IsDelete == false).FirstOrDefault();
			if (data == null)
				data = db.Bill_Lading.Where(x => x.TransferNo == param && x.IsDelete == false).FirstOrDefault();
			var result = new ResultHelper();

			result.Data = data;
			result.Ok = DataModifyResultType.Success;
			result.Message = "OK";

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
	}
}
