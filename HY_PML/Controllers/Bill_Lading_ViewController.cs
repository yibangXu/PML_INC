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
	public class Bill_Lading_ViewController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;
		public ActionResult _ElementInForm()
		{
			return PartialView();
		}
		public ActionResult _ElementInForm2()
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

			ViewBag.Title = "提單貨件狀態";
			ViewBag.dlgWidth = "960px";
			ViewBag.ControllerName = "Bill_Lading_View";
			ViewBag.AddFunc = "";
			ViewBag.EditFunc = "";
			ViewBag.DelFunc = "";
			ViewBag.SaveRecFunc = "";
			ViewBag.FormCustomJsNew = "";
			ViewBag.FormCustomJsEdit = "";
			ViewBag.PageParam = isTw ? "2" : "1";

			ViewBag.Title2 = "貨件狀態";
			ViewBag.ControllerName2 = "Bill_Lading_View";
			ViewBag.AddFunc2 = "";
			ViewBag.EditFunc2 = "";
			ViewBag.DelFunc2 = "";
			ViewBag.FormPartialName2 = "_ElementInForm2";
			ViewBag.FormCustomJsNew2 = "";
			ViewBag.FormCustomJsEdit2 = "";

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

			var billLadingView =
							from b in db.Bill_Lading.Where(x => x.IsDelete == false)
							join h in db.ORG_Hub
							on b.HubNo equals h.HubNo into ps1
							from h in ps1.DefaultIfEmpty()
							join u in db.SYS_User on b.CreateBy equals u.Account into ps2
							from u in ps2.DefaultIfEmpty()
							where statNoList.Contains(b.SStatNo)
							select new
							{
								LadingNo = b.LadingNo,
								WarehouseRNo = b.WarehouseRNo,
								HubNo = b.HubNo,
								HubName = h == null ? null : h.HubName,
								WarehouseRDate = b.WarehouseRDate,
								TransferNo = b.TransferNo,
								LadingDate = b.LadingDate,
								SendCustNo = b.SendCustNo,
								SendCHName = b.SendCHName,
								SendPhone = b.SendPhone,
								SendBy = b.SendBy,
								SendCustAddr = b.SendCustAddr,
								RecPhone = b.RecPhone,
								RecBy = b.RecBy,
								RecCompany = b.RecCompany,
								RecCustCHName = b.RecCustCHName,
								RecChAddr = b.RecChAddr,
								RecInvNo = b.RecInvNo,
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
								CcNo = b.CcNo,
								PayCustNo = b.PayCustNo,
								PayCustCHName = b.PayCustCHName,
								ToPayment = b.ToPayment,
								ToPaymentCurrency = b.ToPaymentCurrency,
								AgentPay = b.AgentPay,
								AgentPayCurrency = b.AgentPayCurrency,
								Remark = b.Remark,
								PhoneCheckTime = b.PhoneCheckTime,
								Status = b.Status,
								StatusTime = b.StatusTime,
								IsConfirm = b.IsConfirm,
								ConfirmBy = b.ConfirmBy,
								IsCheck = b.IsCheck,
								CheckBy = b.CheckBy,
								CreateBy = u.UserName,
								CreateTime = b.CreateTime,
							};

			if (data.LadingNo.IsNotEmpty())
				billLadingView = billLadingView.Where(x => x.LadingNo.Contains(data.LadingNo));
			//if (start_date != null && end_date != null)
			//{
			//	var sDate = start_date.Value.Date;
			//	var eDate = end_date.Value.Date;
			//	billLadingView = billLadingView.Where(x => DbFunctions.TruncateTime(x.LadingDate).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.LadingDate).Value.CompareTo(sDate) >= 0);
			//}

			int records = billLadingView.Count();
			billLadingView = billLadingView.OrderBy(o => o.LadingDate).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = billLadingView,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		#region 子表GRID
		[Authorize]
		public ActionResult GetGridJSON2(string ladingNo)
		{

			var data = new List<Transportation_View>();

			var RData = db.TransportationR.Where(x => x.LadingNo == ladingNo).FirstOrDefault();
			if (RData != null)
			{
				data.Add(new Transportation_View()
				{

					TransportationNo = RData.TransportationNo,
					LadingNo = RData.LadingNo,
					Time = RData.ReceiveTime,
					Status = "收件",
					Stat = RData.RStatName,
					FromGo = "",
					Pcs = RData.ReceivePcs,
					TransportNo = "",
					By = RData.CreateBy,
				});
			}
			var DData = db.TransportationD.Where(x => x.LadingNo == ladingNo).FirstOrDefault();
			if (DData != null)
			{
				data.Add(new Transportation_View()
				{
					TransportationNo = DData.TransportationNo,
					LadingNo = DData.LadingNo,
					Time = DData.DeliveryTime,
					Status = "出貨",
					Stat = DData.DStatName,
					FromGo = DData.NextStatName,
					Pcs = DData.DeliveryPcs,
					TransportNo = DData.TransportNo,
					By = DData.CreateBy,
				});
			}
			var AData = db.TransportationA.Where(x => x.LadingNo == ladingNo).FirstOrDefault();
			if (AData != null)
			{
				data.Add(new Transportation_View()
				{
					TransportationNo = AData.TransportationNo,
					LadingNo = AData.LadingNo,
					Time = AData.ArrivalTime,
					Status = "到件",
					Stat = AData.AStatName,
					FromGo = AData.LastStatName,
					Pcs = AData.ArrivalPcs,
					TransportNo = "",
					By = AData.CreateBy,
				});
			}
			var SData = db.TransportationS.Where(x => x.LadingNo == ladingNo).FirstOrDefault();
			if (SData != null)
			{
				data.Add(new Transportation_View()
				{
					TransportationNo = SData.TransportationNo,
					LadingNo = SData.LadingNo,
					Time = SData.SendTime,
					Status = "派件",
					Stat = SData.SStatName,
					FromGo = "",
					Pcs = SData.SendPcs,
					TransportNo = "",
					By = SData.CreateBy,
				});
			}
			var viewData = data.Select((x, Index) => new Transportation_View()
			{
				Index = Index,
				TransportationNo = x.TransportationNo,
				LadingNo = x.LadingNo,
				Time = x.Time,
				Status = x.Status,
				Stat = x.Stat,
				FromGo = x.FromGo,
				Pcs = x.Pcs,
				TransportNo = x.TransportNo,
				By = x.By,
			}) as IEnumerable<Transportation_View>;
			viewData = viewData.OrderBy(x => x.Time);
			int records = viewData.Count();
			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = viewData,
				Records = records,
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion
	}
}
