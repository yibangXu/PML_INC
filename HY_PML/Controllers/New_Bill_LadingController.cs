using BarcodeLib;
using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HY_PML.Controllers
{
	public class New_Bill_LadingController : Controller
	{
		#region PartialView
		//static private string path = "ftp://waws-prod-dm1-147.ftp.azurewebsites.windows.net//";    //目標路徑
		//static private string username = "yb\\$yb";  //ftp使用者名稱
		//static private string password = "P6EPa7PF4sftkPuayacDwj0C4BjY430Afn2ZlA2rWQCrCbJMEMfA2CtB4Cdk";   //ftp密碼
		private string path = WebConfigurationManager.AppSettings["path"]; //目標路徑
		private string username = WebConfigurationManager.AppSettings["username"]; //ftp使用者名稱
		private string password = WebConfigurationManager.AppSettings["password"]; //ftp密碼
		string FileTablePath = WebConfigurationManager.AppSettings["FileTablePath"];
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;
		public ActionResult _ElementInTab1()
		{
			return PartialView();
		}
		public ActionResult _ElementInTab2()
		{
			return PartialView();
		}
		public ActionResult _ElementInTab3()
		{
			return PartialView();
		}
		public ActionResult _ElementInTab4()
		{
			return PartialView();
		}
		public ActionResult _ElementInTab5()
		{
			return PartialView();
		}
		public ActionResult _ElementInTab6()
		{
			return PartialView();
		}
		public ActionResult _ElementInTab7()
		{
			return PartialView();
		}
		public ActionResult _ElementInTotalbar()
		{
			return PartialView();
		}
		#endregion

		#region Index
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
			var statData = db.ORG_Stat.Where(x => x.StatNo == statNo).FirstOrDefault();

			string statName = statData.StatName;
			ViewBag.ControllerName = "New_Bill_Lading";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.StatNo = statNo;
			ViewBag.StatName = statName;
			ViewBag.CreateBy = User.Identity.Name;
			ViewBag.StatNo = statNo;

			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}
		#endregion

		#region 提單 && 來往紀錄
		#region Bill_lading SearchGRID
		[Authorize]
		public ActionResult GetSearchGridJSON(Bill_Lading data, int page = 1, int rows = 20, DateTime? start_date = null, DateTime? end_date = null)
		{
			//System.Diagnostics.Process.Start("C:\\Users\\yibang.HONGYUAN\\Desktop\\FileZilla_Server-0_9_60_2.exe");
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
				from b in db.Bill_Lading.Where(x => x.IsDelete == false)
				join s in db.ShdetHeader
				on b.ShdetNo equals s.ShdetNo into ps
				from s in ps.DefaultIfEmpty()
				join h in db.ORG_Hub
				on b.HubNo equals h.HubNo into ps1
				from h in ps1.DefaultIfEmpty()
				join u in db.SYS_User on b.CreateBy equals u.Account into ps2
				from u in ps2.DefaultIfEmpty()
				join sd in db.ShdetDetail
				on b.ShdetNo equals sd.ShdetNo into ps3
				from sd in ps3.DefaultIfEmpty()
				join blr in db.Bill_Lading_Record
				on b.LadingNo equals blr.LadingNo into ps4
				from blr in ps4.DefaultIfEmpty()
				where statNoList.Contains(b.SStatNo) || statNoList.Contains(b.AStatNo) || statNoList.Contains(sd.StatNo) || statNoList.Contains(sd.CallStatNo)
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCustNo = b.SendCustNo,
					SendCHName = b.SendCHName,
					SendCompany = b.SendCompany,
					SendPhone = b.SendPhone,
					SendInvNo = b.SendInvNo,
					RecCompany = b.RecCompany,
					RecPhone = b.RecPhone,
					CName = b.CName,
					Type = b.Type,
					CocustomTyp = b.CocustomTyp,
					RecordType = blr == null ? 999 : blr.RecordType,
					ShdetNo = b.ShdetNo,
					HubNo = b.HubNo,
					HubName = h.HubName,
					CcNo = b.CcNo,
					S_SectorNo = sd.SectorNo,
					SStatNo = b.SStatNo,
					AStatNo = b.AStatNo,
					PickUpAreaNo = sd.PickUpAreaNo,
					ImOrEx = b.ImOrEx,
					IsFinish = sd == null ? false : sd.IsFinish,
					IsDesp = s == null ? false : s.IsDesp,
					IsInLading = b.IsInLading,
					CreateTime = b.CreateTime,
					CreateBy = b.CreateBy,
					Printed = b.Printed,
				};

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				billLading = billLading.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			if (data.LadingNo_Type.IsNotEmpty())
				billLading = billLading.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));

			if (data.IsDesp != null)
				billLading = billLading.Where(x => x.IsDesp == data.IsDesp);

			if (data.HubNo.IsNotEmpty())
				billLading = billLading.Where(x => x.HubNo == data.HubNo);

			if (data.S_SectorNo.IsNotEmpty())
				billLading = billLading.Where(x => x.S_SectorNo == data.S_SectorNo);

			if (data.SendCustNo.IsNotEmpty())
				billLading = billLading.Where(x => x.SendCustNo.Contains(data.SendCustNo));

			if (data.SendPhone.IsNotEmpty())
				billLading = billLading.Where(x => x.SendPhone.Contains(data.SendPhone));

			if (data.SendInvNo.IsNotEmpty())
				billLading = billLading.Where(x => x.SendInvNo.Contains(data.SendInvNo));

			if (data.SendCompany.IsNotEmpty())
				billLading = billLading.Where(x => x.SendCompany.Contains(data.SendCompany));

			if (data.RecCompany.IsNotEmpty())
				billLading = billLading.Where(x => x.RecCompany.Contains(data.RecCompany));

			if (data.RecPhone.IsNotEmpty())
				billLading = billLading.Where(x => x.RecPhone.Contains(data.RecPhone));

			if (data.AStatNo.IsNotEmpty())
				billLading = billLading.Where(x => x.AStatNo == data.AStatNo);

			if (data.SStatNo.IsNotEmpty())
				billLading = billLading.Where(x => x.SStatNo == data.SStatNo);

			if (data.PickUpAreaNo != null)
				billLading = billLading.Where(x => x.PickUpAreaNo == data.PickUpAreaNo);

			if (data.CocustomTyp != null)
				billLading = billLading.Where(x => x.CocustomTyp == data.CocustomTyp);

			if (data.IsFinish != null)
				billLading = billLading.Where(x => x.IsFinish == data.IsFinish);

			if (data.RecordType != null)
				billLading = billLading.Where(x => x.RecordType == data.RecordType);

			if (data.CreateBy.IsNotEmpty())
				billLading = billLading.Where(x => x.CreateBy.Contains(data.CreateBy));

			var billLadingGData = billLading.GroupBy(x => x.LadingNo);

			var billLadingData = billLadingGData.ToList().Select((x, index) => new Bill_Lading()
			{
				LadingNo = x.First().LadingNo,
				LadingNo_Type = x.First().LadingNo_Type,
				LadingDate = x.First().LadingDate,
				SendCustNo = x.First().SendCustNo,
				SendCHName = x.First().SendCHName,
				CName = x.First().CName,
				Type = x.First().Type,
				CocustomTyp = x.First().CocustomTyp,
				ShdetNo = x.First().ShdetNo,
				CreateTime = x.First().CreateTime,
				CreateBy = x.First().CreateBy,
				IsDesp = x.First().IsDesp,
				HubNo = x.First().HubNo,
				HubName = x.First().HubName,
				CcNo = x.First().CcNo,
				S_SectorNo = x.First().S_SectorNo,
				SendInvNo = x.First().SendInvNo,
				SStatNo = x.First().SStatNo,
				AStatNo = x.First().AStatNo,
				PickUpAreaNo = x.First().PickUpAreaNo,
				IsFinish = x.First().IsFinish,
				RecordType = x.First().RecordType,
				ImOrEx = x.First().ImOrEx,
				IsInLading = x.First().IsInLading,
				Printed = x.First().Printed,
			}) as IEnumerable<Bill_Lading>;

			//var distData = billLadingData.Distinct(new LadingCompare());
			int records = billLadingData.Count();
			billLadingData = billLadingData.OrderByDescending(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);
			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = billLadingData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		#region Bill_lading SearchSum
		[Authorize]
		public ActionResult GetSearchSumJSON(Bill_Lading data, int page = 1, int rows = 20, DateTime? start_date = null, DateTime? end_date = null)
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
				from b in db.Bill_Lading.Where(x => x.IsDelete == false)
				join s in db.ShdetHeader
				on b.ShdetNo equals s.ShdetNo into ps
				from s in ps.DefaultIfEmpty()
				join h in db.ORG_Hub
				on b.HubNo equals h.HubNo into ps1
				from h in ps1.DefaultIfEmpty()
				join u in db.SYS_User on b.CreateBy equals u.Account into ps2
				from u in ps2.DefaultIfEmpty()
				join sd in db.ShdetDetail
				on b.ShdetNo equals sd.ShdetNo into ps3
				from sd in ps3.DefaultIfEmpty()
				join blr in db.Bill_Lading_Record
				on b.LadingNo equals blr.LadingNo into ps4
				from blr in ps4.DefaultIfEmpty()
				where statNoList.Contains(b.SStatNo) || statNoList.Contains(b.AStatNo) || statNoList.Contains(sd.StatNo) || statNoList.Contains(sd.CallStatNo)
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCustNo = b.SendCustNo,
					SendCHName = b.SendCHName,
					SendCompany = b.SendCompany,
					SendPhone = b.SendPhone,
					SendInvNo = b.SendInvNo,
					RecCompany = b.RecCompany,
					RecPhone = b.RecPhone,
					CName = b.CName,
					Type = b.Type,
					CocustomTyp = b.CocustomTyp,
					RecordType = blr == null ? 999 : blr.RecordType,
					ShdetNo = b.ShdetNo,
					HubNo = b.HubNo,
					S_SectorNo = sd.SectorNo,
					SStatNo = b.SStatNo,
					AStatNo = b.AStatNo,
					PickUpAreaNo = sd.PickUpAreaNo,
					IsDesp = s == null ? false : s.IsDesp,
					IsFinish = sd == null ? false : sd.IsFinish,
					CreateTime = b.CreateTime,
					CreateBy = b.CreateBy,
					Qty = b.Qty ?? 0,
					PiecesNo = b.PiecesNo ?? 0,
					Volume = b.Volume ?? 0,
					Weight = b.Weight ?? 0,
					Freight = b.Freight ?? 0,
					ToPayment = b.ToPayment ?? 0,
					AgentPay = b.AgentPay ?? 0,
					FuelCosts = b.FuelCosts ?? 0,
					CustomsPay = b.CustomsPay ?? 0,
					InsurancePay = b.InsurancePay ?? 0,
					ProdIdPay = b.ProdIdPay ?? 0,
					OtherPayTax = b.OtherPayTax ?? 0,
					OtherPayNoTax = b.OtherPayNoTax ?? 0,
				};

			if (data.LadingNo_Type.IsNotEmpty())
				billLading = billLading.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				billLading = billLading.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			if (data.IsDesp != null)
				billLading = billLading.Where(x => x.IsDesp == data.IsDesp);

			if (data.HubNo.IsNotEmpty())
				billLading = billLading.Where(x => x.HubNo == data.HubNo);

			if (data.S_SectorNo.IsNotEmpty())
				billLading = billLading.Where(x => x.S_SectorNo == data.S_SectorNo);

			if (data.SendCustNo.IsNotEmpty())
				billLading = billLading.Where(x => x.SendCustNo.Contains(data.SendCustNo));

			if (data.SendPhone.IsNotEmpty())
				billLading = billLading.Where(x => x.SendPhone.Contains(data.SendPhone));

			if (data.SendInvNo.IsNotEmpty())
				billLading = billLading.Where(x => x.SendInvNo.Contains(data.SendInvNo));

			if (data.SendCompany.IsNotEmpty())
				billLading = billLading.Where(x => x.SendCompany.Contains(data.SendCompany));

			if (data.RecCompany.IsNotEmpty())
				billLading = billLading.Where(x => x.RecCompany.Contains(data.RecCompany));

			if (data.RecPhone.IsNotEmpty())
				billLading = billLading.Where(x => x.RecPhone.Contains(data.RecPhone));

			if (data.AStatNo.IsNotEmpty())
				billLading = billLading.Where(x => x.AStatNo == data.AStatNo);

			if (data.SStatNo.IsNotEmpty())
				billLading = billLading.Where(x => x.SStatNo == data.SStatNo);

			if (data.PickUpAreaNo != null)
				billLading = billLading.Where(x => x.PickUpAreaNo == data.PickUpAreaNo);

			if (data.CocustomTyp != null)
				billLading = billLading.Where(x => x.CocustomTyp == data.CocustomTyp);

			if (data.IsFinish != null)
				billLading = billLading.Where(x => x.IsFinish == data.IsFinish);

			if (data.RecordType != null)
				billLading = billLading.Where(x => x.RecordType == data.RecordType);

			if (data.CreateBy.IsNotEmpty())
				billLading = billLading.Where(x => x.CreateBy.Contains(data.CreateBy));

			int records = billLading.GroupBy(x => x.LadingNo).Count();
			var sumData = new Bill_Lading();
			if (records != 0)
			{
				sumData = new Bill_Lading()
				{
					SumQty = records,
					SumPiecesNo = billLading.Sum(s => s.PiecesNo),
					SumVolume = billLading.Sum(s => s.Volume),
					SumWeight = billLading.Sum(s => s.Weight),
					SumFreight = billLading.Sum(s => s.Freight),
					SumToPayment = billLading.Sum(s => s.ToPayment),
					SumAgentPay = billLading.Sum(s => s.AgentPay),
					SumFuelCosts = billLading.Sum(s => s.FuelCosts),
					SumCustomsPay = billLading.Sum(s => s.CustomsPay),
					SumInsurancePay = billLading.Sum(s => s.InsurancePay),
					SumProdIdPay = billLading.Sum(s => s.ProdIdPay),
					SumOtherPayTax = billLading.Sum(s => s.OtherPayTax),
					SumOtherPayNoTax = billLading.Sum(s => s.OtherPayNoTax),
				};
			}

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = sumData,
				Records = 1,
				Pages = 1,
				TotalPage = rows <= 0 ? 1 : (1 - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		#region Bill_lading GRID
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
				from b in db.Bill_Lading.Where(x => x.IsDelete == false)
				join s in db.ShdetHeader
				on b.ShdetNo equals s.ShdetNo into ps
				from s in ps.DefaultIfEmpty()
				join h in db.ORG_Hub
				on b.HubNo equals h.HubNo into ps1
				from h in ps1.DefaultIfEmpty()
				join u in db.SYS_User on b.CreateBy equals u.Account into ps2
				from u in ps2.DefaultIfEmpty()
				join sd in db.ShdetDetail
				on b.ShdetNo equals sd.ShdetNo into ps3
				from sd in ps3.DefaultIfEmpty()
				where statNoList.Contains(b.SStatNo) || statNoList.Contains(b.AStatNo) || statNoList.Contains(sd.StatNo) || statNoList.Contains(sd.CallStatNo)
				select new
				{

					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					WarehouseRNo = b.WarehouseRNo,
					WarehouseRDate = b.WarehouseRDate,
					HubNo = b.HubNo,
					HubName = h == null ? null : h.HubName,
					TransferNo = b.TransferNo,
					OrderNo = b.OrderNo,
					Sale = b.Sale,
					SalePhone = b.SalePhone,
					CreatePhone = b.CreatePhone,
					SendCustNo = b.SendCustNo,
					SendCHName = b.SendCHName,
					SendCustLevel = b.SendCustLevel,
					SendPhone = b.SendPhone,
					SendFaxNo = b.SendFaxNo,
					SendBy = b.SendBy,
					SendCompany = b.SendCompany,
					SendCustAddr = b.SendCustAddr,
					SendInvNo = b.SendInvNo,
					SendCountry = b.SendCountry,
					SendCity = b.SendCity,
					SendState = b.SendState,
					SendPostDist = b.SendPostDist,
					SendEBy = b.SendEBy,
					SendECompany = b.SendECompany,
					SendECustAddr = b.SendECustAddr,
					SendEInvNo = b.SendEInvNo,
					SendECountry = b.SendECountry,
					SendECity = b.SendECity,
					SendEState = b.SendEState,
					SendEPostDist = b.SendEPostDist,
					SendRemark = b.SendRemark,
					RecPhone = b.RecPhone,
					RecMPhone = b.RecMPhone,
					DestNo = b.DestNo,
					CName = b.CName,
					Type = b.Type,
					CocustomTyp = b.CocustomTyp,
					RecBy = b.RecBy,
					RecCompany = b.RecCompany,
					RecChAddr = b.RecChAddr,
					RecInvNo = b.RecInvNo,
					RecCountry = b.RecCountry,
					RecCity = b.RecCity,
					RecState = b.RecState,
					RecPostDist = b.RecPostDist,
					RecRemark = b.RecRemark,
					SectorNo = b.SectorNo,
					SectorName = b.SectorName,
					SStatNo = b.SStatNo,
					SStatName = b.SStatName,
					SDate = b.SDate,
					AStatNo = b.AStatNo,
					AStatName = b.AStatName,
					Hsno = b.Hsno,
					ProductNo = b.ProductNo,
					ProductName = b.ProductName,
					ForIndonesian = b.ForIndonesian,
					PiecesNo = b.PiecesNo,
					Qty = b.Qty,
					Weight = b.Weight,
					Volume = b.Volume,
					Currency = b.Currency,
					Cost = b.Cost,
					CostCurrency = b.CostCurrency,
					CcNo = b.CcNo,
					PayCustNo = b.PayCustNo,
					PayCustCHName = b.PayCustCHName,
					Freight = b.Freight,
					FreightCurrency = b.FreightCurrency,
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
					TotalCurrency = b.TotalCurrency,
					Remark = b.Remark,
					Remark2 = b.Remark2,
					ImOrEx = b.ImOrEx == "Ex" ? "出口" : "進口",
					PhoneCheckTime = b.PhoneCheckTime,
					Status = b.Status,
					StatusTime = b.StatusTime,
					Source = b.Source,
					IsConfirm = b.IsConfirm,
					IsCheck = b.IsCheck,
					CheckBy = u.UserName,
					CheckTime = b.CheckTime,
					CreateBy = b.CreateBy,
					CreateTime = b.CreateTime,
					UpdateBy = b.UpdateBy,
					UpdateTime = b.UpdateTime,
					DeletedBy = b.DeletedBy,
					DeletedTime = b.DeletedTime,
					IsDelete = b.IsDelete,
					ShdetNo = b.ShdetNo,
					RecCustCHName = b.RecCustCHName,
					RecCustEName1 = b.RecCustEName1,
					RecCustEName2 = b.RecCustEName2,
					RecCustENAddr1 = b.RecCustENAddr1,
					RecCustENAddr2 = b.RecCustENAddr2,
					IsDesp = s == null ? false : s.IsDesp,
					PrintLang = b.PrintLang,
					IsInLading = b.IsInLading,
				};
			if (data.LadingNo.IsNotEmpty())
				billLading = billLading.Where(x => x.LadingNo == data.LadingNo);
			billLading = billLading.OrderBy(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);
			int records = billLading.Count();

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

		#region Add Lading
		[Authorize]
		public ActionResult AddLading(Bill_Lading data, string[] dtl)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = data.LadingNo_Type == null ? null : db.Bill_Lading.FirstOrDefault(x => x.LadingNo_Type == data.LadingNo_Type && x.IsDelete == false);
				var shdetDtlData = data.LadingNo_Type == null ? null : db.ShdetDetail.FirstOrDefault(x => x.LadingNo_Type == data.LadingNo_Type);
				var ladingNo = "";
				if (userRecord == null && shdetDtlData == null)
				{
					var saveData = new Bill_Lading();
					////電子提單規則：
					////第１碼為遠達公司（Ｐ）
					////第２碼為出口國
					////	第３－８碼為西元年月日（２０１９取１９）
					////第９－１２為流水號
					////	第１３碼為驗證碼　（年＋月＋日＋流水號）／２９　取小數位第二位
					//var areaID = db.ORG_Stat.Where(x => x.StatNo == data.SStatNo).Select(x => x.AreaID).FirstOrDefault();
					//var areaNo = db.ORG_Area.Where(x => x.ID == areaID).Select(x => x.AreaNo).FirstOrDefault().Substring(0, 1); ;
					//var date = DateTime.Now.ToDateString().Replace("/", "").Substring(2);
					//var lastLadingNo = db.Bill_Lading.Where(x => x.LadingNo.Contains("P" + areaNo + date)).OrderByDescending(x => x.CreateTime).Select(x => x.LadingNo).FirstOrDefault();
					//var sN = lastLadingNo != null ? String.Format("{0:0000}", int.Parse(lastLadingNo.Substring(8, 4)) + 1) : "0001";
					//var vCode = String.Format("{0:0.00}", (Math.Floor((decimal.Parse(date.Substring(0, 2)) + decimal.Parse(date.Substring(2, 2)) + decimal.Parse(date.Substring(4, 2)) + decimal.Parse(sN)) / 29 * 100)) / 100).ToString().Split('.')[1].Substring(1, 1);
					//saveData.LadingNo = "P" + areaNo + date + sN + vCode;

					//2019-12-11 如台灣 TW18601567前面TW碼 為區域提單碼， 1860156 為流水號， X為條碼驗證碼 X = (186 + 01 + 56) / 29 = 8.379310344827586 取小數點第二碼7為驗證碼
					var areaID = db.ORG_Stat.Where(x => x.StatNo == data.SStatNo).Select(x => x.AreaID).FirstOrDefault();
					var areaNo = db.ORG_Area.Where(x => x.ID == areaID).Select(x => x.AreaCode).FirstOrDefault();
					var lastLadingNo = db.Bill_Lading.Where(x => x.LadingNo.Substring(0, 2) == areaNo).OrderByDescending(x => x.CreateTime).Select(x => x.LadingNo).FirstOrDefault();
					var sN = lastLadingNo != null ? String.Format("{0:0000000}", int.Parse(lastLadingNo.Substring(2, 7)) + 1) : "0000001";
					var vCode = String.Format("{0:0.00}", (Math.Floor((decimal.Parse(sN.Substring(0, 3)) + decimal.Parse(sN.Substring(3, 2)) + decimal.Parse(sN.Substring(5, 2))) / 29 * 100)) / 100).ToString().Split('.')[1].Substring(1, 1);
					saveData.LadingNo = areaNo + sN + vCode;

					ladingNo = saveData.LadingNo;
					if (data.LadingNo_Type != null)
					{
						saveData.LadingNo_Type = data.LadingNo_Type.ToUpper();
					}
					else
					{
						saveData.LadingNo_Type = saveData.LadingNo;
					}
					saveData.LadingDate = data.LadingDate;
					saveData.WarehouseRNo = data.WarehouseRNo;
					saveData.WarehouseRDate = data.WarehouseRDate;
					saveData.HubNo = data.HubNo;
					saveData.TransferNo = data.TransferNo;
					saveData.OrderNo = data.OrderNo;
					saveData.Sale = data.Sale;
					saveData.SalePhone = data.SalePhone;
					saveData.CreatePhone = data.CreatePhone;
					saveData.SendCustNo = data.SendCustNo;
					saveData.SendCHName = data.SendCHName;
					saveData.SendCustLevel = data.SendCustLevel;
					saveData.SendFaxNo = data.SendFaxNo;
					saveData.SendPhone = data.SendPhone;
					saveData.SendBy = data.SendBy;
					saveData.SendCompany = data.SendCompany;
					saveData.SendCustAddr = data.SendCustAddr;
					saveData.SendInvNo = data.SendInvNo;
					saveData.SendCountry = data.SendCountry;
					saveData.SendCity = data.SendCity;
					saveData.SendState = data.SendState;
					saveData.SendPostDist = data.SendPostDist;
					saveData.SendEBy = data.SendEBy;
					saveData.SendECompany = data.SendCompany;
					saveData.SendECustAddr = data.SendECustAddr;
					saveData.SendEInvNo = data.SendEInvNo;
					saveData.SendECountry = data.SendECountry;
					saveData.SendECity = data.SendECity;
					saveData.SendEState = data.SendEState;
					saveData.SendEPostDist = data.SendEPostDist;
					saveData.SendRemark = data.SendRemark;
					saveData.DestNo = data.DestNo;
					saveData.CName = data.CName;
					saveData.Type = data.Type;
					saveData.CocustomTyp = data.CocustomTyp;
					saveData.RecPhone = data.RecPhone;
					saveData.RecMPhone = data.RecMPhone;
					saveData.RecBy = data.RecBy;
					saveData.RecCompany = data.RecCompany;
					saveData.RecChAddr = data.RecChAddr;
					saveData.RecInvNo = data.RecInvNo;
					saveData.RecCountry = data.RecCountry;
					saveData.RecCity = data.RecCity;
					saveData.RecState = data.RecState;
					saveData.RecPostDist = data.RecPostDist;
					saveData.RecRemark = data.RecRemark;
					saveData.SectorNo = data.SectorNo;
					saveData.SectorName = data.SectorName;
					saveData.SStatNo = data.SStatNo;
					saveData.SStatName = data.SStatName;
					saveData.SDate = data.SDate;
					saveData.AStatNo = data.AStatNo;
					saveData.AStatName = data.AStatName;
					saveData.Hsno = data.Hsno;
					saveData.ProductNo = data.ProductNo;
					saveData.ProductName = data.ProductName;
					saveData.ForIndonesian = data.ForIndonesian;
					saveData.PiecesNo = data.PiecesNo;
					saveData.Currency = data.Currency;
					saveData.Cost = data.Cost;
					saveData.CostCurrency = data.Currency;
					saveData.CcNo = data.CcNo;
					saveData.PayCustNo = data.PayCustNo;
					saveData.PayCustCHName = data.PayCustCHName;
					saveData.Freight = data.Freight;
					saveData.FreightCurrency = data.Currency;
					saveData.FuelCosts = data.FuelCosts;
					saveData.ToPayment = data.ToPayment;
					saveData.ToPaymentCurrency = data.Currency;
					saveData.AgentPay = data.AgentPay;
					saveData.AgentPayCurrency = data.Currency;
					saveData.ProdIdPay = data.ProdIdPay;
					saveData.CustomsPay = data.CustomsPay;
					saveData.InsurancePay = data.InsurancePay;
					saveData.OtherPayTax = data.OtherPayTax;
					saveData.OtherPayNoTax = data.OtherPayNoTax;
					saveData.Total = data.Total;
					saveData.ToPaymentCurrency = data.Currency;
					saveData.Remark = data.Remark;
					saveData.Remark2 = data.Remark2;
					saveData.ShdetNo = saveData.LadingNo;
					saveData.ImOrEx = data.ImOrEx;
					saveData.PrintLang = data.PrintLang;
					saveData.IsInLading = data.IsInLading;

					//以下系統自填
					saveData.Source = "Web";
					saveData.IsConfirm = true;
					saveData.IsCheck = false;
					saveData.IsReview = false;
					saveData.CreateTime = DateTime.Now;
					saveData.CreateBy = User.Identity.Name;
					saveData.IsDelete = false;

					db.Bill_Lading.Add(saveData);

					var shdetData = new ShdetHeader();
					shdetData.LadingNo = saveData.LadingNo;
					shdetData.ShdetNo = saveData.LadingNo;
					shdetData.CustNo = saveData.SendCustNo;
					shdetData.CustCHName = saveData.SendCHName;
					shdetData.HubNo = saveData.HubNo;
					shdetData.Dest = saveData.CName;
					shdetData.IsDesp = false;
					shdetData.IsCancel = false;
					shdetData.IsReply = false;
					shdetData.IsFinish = false;
					shdetData.CreatedDate = DateTime.Now;
					shdetData.CreatedBy = User.Identity.Name;
					shdetData.IsDelete = false;
					shdetData.ReserveDate = saveData.SDate;
					shdetData.SDate = saveData.SDate;

					db.ShdetHeader.Add(shdetData);

					try
					{
						db.SaveChanges();
						trans.Commit();
						result.Ok = DataModifyResultType.Success;
						result.Message = "LadingNo:" + ladingNo;
						//2020-01-02 測試轉哲盟LOG過多問題 暫不開啟
						//SavePML_TWN(ladingNo);
					}
					catch (Exception e)
					{
						trans.Rollback();
						result.Ok = DataModifyResultType.Faild;
						result.Message = e.Message;
					}
				}
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "已存在此筆單號【" + data.LadingNo_Type + "】，</br>請確認！";
					trans.Rollback();
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region Edit Lading
		[Authorize]
		public ActionResult EditLading(Bill_Lading data, string[] dtl, string tab)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var ladingNo = "";
			bool valid = true;
			using (var trans = db.Database.BeginTransaction())
			{
				if (dtl[0].ToString() == "[]")
				{
					var deleteData = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo && x.IsDelete == false).ToArray();
					foreach (var d in deleteData)
					{
						d.IsDelete = true;
						d.DeletedBy = User.Identity.Name;
						d.DeletedTime = DateTime.Now;

						db.Entry(d).State = EntityState.Modified;
					}
				}
				if (dtl[0].ToString() != "[]" && dtl.Count() > 0)
				{
					var datalist = dtl[0].Replace("[", "").Replace("]", "").Replace("},{", "}㊣{").Split('㊣');
					var newData = new List<DeclCust_Main>();
					JavaScriptSerializer js = new JavaScriptSerializer();
					foreach (var d in datalist)
					{
						newData.Add(js.Deserialize<DeclCust_Main>(d));
					}
					var oldData = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo && x.IsDelete == false).ToArray();
					var cmp = new MainCompare();
					var deleteData = oldData.Except(newData, cmp);
					var addData = newData.Except(oldData, cmp);
					var editData = newData.Intersect(oldData, cmp);

					foreach (var d in deleteData)
					{
						d.IsDelete = true;
						d.DeletedBy = User.Identity.Name;
						d.DeletedTime = DateTime.Now;

						db.Entry(d).State = EntityState.Modified;
					}
					foreach (var e in editData)
					{
						var originData = db.DeclCust_Main.FirstOrDefault(x => x.LadingNo == e.LadingNo && x.sNo == e.sNo);
						originData.CustType = e.CustType;
						originData.Flight = e.Flight;
						originData.LadNo = e.LadNo;
						originData.BagNo = e.BagNo;
						originData.CleCusCode = e.CleCusCode;
						originData.CusCoode = e.CusCoode;
						originData.ProductNo = e.ProductNo;
						originData.ProductName = e.ProductName;
						originData.ProductEName = e.ProductEName;
						originData.Country = e.Country;
						originData.Type = e.Type;
						originData.HSNo = e.HSNo;
						originData.Qty = e.Qty;
						originData.Unit = e.Unit;
						originData.GrossWeight = e.GrossWeight;
						originData.Weight = e.Weight;
						originData.Price = e.Price;
						originData.Length = e.Length;
						originData.Width = e.Width;
						originData.Height = e.Height;
						originData.Total = e.Total;
						originData.Currency = e.Currency;
						originData.Pcs = e.Pcs;
						originData.PcsNo = e.PcsNo;
						originData.Remark = e.Remark;

						originData.UpdateBy = User.Identity.Name;
						originData.UpdateTime = DateTime.Now;
						db.Entry(originData).State = EntityState.Modified;
					}
					var originLastData = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo).ToArray().OrderByDescending(x => x.sNo).FirstOrDefault();
					var Seq = originLastData != null ? originLastData.sNo : 0;
					addData = addData.Select((x, Index) => new DeclCust_Main()
					{
						sNo = Seq + Index + 1,
						LadingNo = x.LadingNo,
						CustType = x.CustType,
						Flight = x.Flight,
						LadNo = x.LadNo,
						BagNo = x.BagNo,
						CleCusCode = x.CleCusCode,
						CusCoode = x.CusCoode,
						ProductNo = x.ProductNo,
						ProductName = x.ProductName,
						ProductEName = x.ProductEName,
						Country = x.Country,
						Type = x.Type,
						HSNo = x.HSNo,
						Qty = x.Qty,
						Unit = x.Unit,
						GrossWeight = x.GrossWeight,
						Weight = x.Weight,
						Price = x.Price,
						Length = x.Length,
						Width = x.Width,
						Height = x.Height,
						Total = x.Total,
						Currency = x.Currency,
						Pcs = x.Pcs,
						PcsNo = x.PcsNo,
						Remark = x.Remark,

						CreateTime = DateTime.Now,
						CreateBy = User.Identity.Name,
						IsDelete = false,
					});
					db.DeclCust_Main.AddRange(addData);
					data.Qty = addData.Sum(x => x.Qty) + editData.Sum(x => x.Qty);
					data.Weight = addData.Sum(x => x.Weight) + editData.Sum(x => x.Weight);
					data.Volume = addData.Sum(x => x.GrossWeight) + editData.Sum(x => x.GrossWeight);
				}

				var userRecord = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == data.LadingNo && x.IsDelete == false);
				ladingNo = userRecord.LadingNo;
				if (userRecord == null)
				{
					valid = false;
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
					trans.Rollback();
				}
				if (userRecord.LadingNo_Type != data.LadingNo_Type)
				{
					if (db.Bill_Lading.FirstOrDefault(x => x.LadingNo_Type == data.LadingNo_Type && x.IsDelete == false) != null || db.ShdetHeader.FirstOrDefault(x => x.ShdetNo == data.LadingNo_Type) != null)
					{
						valid = false;
						result.Ok = DataModifyResultType.Faild;
						result.Message = "已存在此筆單號【" + data.LadingNo_Type + "】，</br>請確認！";
						trans.Rollback();
					}
				}
				if (valid == true)
				{
					if (data.LadingNo_Type == null)
						userRecord.LadingNo_Type = data.LadingNo;
					else
					{
						if (userRecord.LadingNo_Type != data.LadingNo_Type)
						{
							userRecord.LadingNo_Type = data.LadingNo_Type;
							userRecord.LadingDate = data.LadingDate;
							UpdateLadingNo_Type(userRecord.LadingNo_Type, data.LadingNo_Type);
						}
					}
					var shdetdtlData = db.ShdetDetail.Where(x => x.ShdetNo == ladingNo);
					if (shdetdtlData.Count() != 0)
					{
						foreach (var s in shdetdtlData)
						{
							s.LadingNo_Type = userRecord.LadingNo_Type;
							db.Entry(s).State = EntityState.Modified;
						}
					}
					userRecord.WarehouseRNo = data.WarehouseRNo;
					userRecord.WarehouseRDate = data.WarehouseRDate;
					if (userRecord.HubNo != data.HubNo)
					{
						var sdData = db.ShdetDetail.Where(x => x.ShdetNo == data.LadingNo);
						foreach (var sd in sdData)
						{
							sd.HubNo = data.HubNo;
							db.Entry(sd).State = EntityState.Modified;
						}
						var shData = db.ShdetHeader.Where(x => x.ShdetNo == data.LadingNo);
						foreach (var sh in shData)
						{
							sh.HubNo = data.HubNo;
							db.Entry(sh).State = EntityState.Modified;
						}
					}
					userRecord.HubNo = data.HubNo;
					userRecord.TransferNo = data.TransferNo;
					userRecord.OrderNo = data.OrderNo;
					userRecord.Sale = data.Sale;
					userRecord.SalePhone = data.SalePhone;
					userRecord.CreatePhone = data.CreatePhone;
					userRecord.SendCustNo = data.SendCustNo;
					userRecord.SendCHName = data.SendCHName;
					userRecord.SendCustLevel = data.SendCustLevel;
					userRecord.SendPhone = data.SendPhone;
					userRecord.SendFaxNo = data.SendFaxNo;
					userRecord.SendBy = data.SendBy;
					userRecord.SendCompany = data.SendCompany;
					userRecord.SendCustAddr = data.SendCustAddr;
					userRecord.SendInvNo = data.SendInvNo;
					userRecord.SendCountry = data.SendCountry;
					userRecord.SendCity = data.SendCity;
					userRecord.SendState = data.SendState;
					userRecord.SendPostDist = data.SendPostDist;
					userRecord.SendEBy = data.SendEBy;
					userRecord.SendECompany = data.SendECompany;
					userRecord.SendECustAddr = data.SendECustAddr;
					userRecord.SendEInvNo = data.SendEInvNo;
					userRecord.SendECountry = data.SendECountry;
					userRecord.SendECity = data.SendECity;
					userRecord.SendEState = data.SendEState;
					userRecord.SendEPostDist = data.SendEPostDist;
					userRecord.SendRemark = data.SendRemark;
					userRecord.RecPhone = data.RecPhone;
					userRecord.RecMPhone = data.RecMPhone;
					userRecord.DestNo = data.DestNo;
					if (userRecord.CName != data.CName)
					{
						var sdData = db.ShdetDetail.Where(x => x.ShdetNo == data.LadingNo);
						foreach (var sd in sdData)
						{
							sd.Dest = data.CName;
							db.Entry(sd).State = EntityState.Modified;
						}
					}
					userRecord.CName = data.CName;
					userRecord.Type = data.Type;
					if (userRecord.CocustomTyp != data.CocustomTyp)
					{
						var sdData = db.ShdetDetail.Where(x => x.ShdetNo == data.LadingNo);
						foreach (var sd in sdData)
						{
							sd.CocustomTyp = data.CocustomTyp;
							db.Entry(sd).State = EntityState.Modified;
						}
					}
					userRecord.CocustomTyp = data.CocustomTyp;
					userRecord.RecBy = data.RecBy;
					userRecord.RecCompany = data.RecCompany;
					userRecord.RecChAddr = data.RecChAddr;
					userRecord.RecInvNo = data.RecInvNo;
					userRecord.RecCountry = data.RecCountry;
					userRecord.RecCity = data.RecCity;
					userRecord.RecState = data.RecState;
					userRecord.RecPostDist = data.RecPostDist;
					userRecord.RecRemark = data.RecRemark;
					userRecord.SectorNo = data.SectorNo;
					userRecord.SectorName = data.SectorName;
					userRecord.SStatNo = data.SStatNo;
					userRecord.SStatName = data.SStatName;
					if (userRecord.SDate != data.SDate)
					{
						var shData = db.ShdetHeader.Where(x => x.ShdetNo == data.LadingNo).FirstOrDefault();
						shData.SDate = data.SDate;
						db.Entry(shData).State = EntityState.Modified;
					}
					userRecord.SDate = data.SDate;
					userRecord.AStatNo = data.AStatNo;
					userRecord.AStatName = data.AStatName;
					userRecord.Hsno = data.Hsno;
					userRecord.ProductNo = data.ProductNo;
					userRecord.ProductName = data.ProductName;
					userRecord.ForIndonesian = data.ForIndonesian;
					userRecord.PiecesNo = data.PiecesNo;
					userRecord.Qty = data.Qty;
					userRecord.Weight = data.Weight;
					userRecord.Volume = data.Volume;
					userRecord.Currency = data.Currency;
					userRecord.Cost = data.Cost;
					userRecord.CostCurrency = data.Currency;
					if (userRecord.CcNo != data.CcNo)
					{
						var sdData = db.ShdetDetail.Where(x => x.ShdetNo == data.LadingNo);
						foreach (var sd in sdData)
						{
							sd.CcNo = data.CcNo;
							db.Entry(sd).State = EntityState.Modified;
						}
					}
					userRecord.CcNo = data.CcNo;
					userRecord.PayCustNo = data.PayCustNo;
					userRecord.PayCustCHName = data.PayCustCHName;
					userRecord.Freight = data.Freight;
					userRecord.FreightCurrency = data.Currency;
					userRecord.FuelCosts = data.FuelCosts;
					userRecord.ToPayment = data.ToPayment;
					userRecord.ToPaymentCurrency = data.Currency;
					userRecord.AgentPay = data.AgentPay;
					userRecord.AgentPayCurrency = data.Currency;
					userRecord.ProdIdPay = data.ProdIdPay;
					userRecord.CustomsPay = data.CustomsPay;
					userRecord.InsurancePay = data.InsurancePay;
					userRecord.OtherPayTax = data.OtherPayTax;
					userRecord.OtherPayNoTax = data.OtherPayNoTax;
					userRecord.Length = data.Length;
					userRecord.Width = data.Width;
					userRecord.Height = data.Height;
					userRecord.Total = data.Total;
					userRecord.TotalCurrency = data.Currency;
					userRecord.Remark = data.Remark;
					userRecord.Remark2 = data.Remark2;
					userRecord.PrintLang = data.PrintLang;
					userRecord.IsInLading = data.IsInLading;
					if (data.AStatNo.IsNotEmpty())
					{
						var sArealD = db.ORG_Stat.Where(x => x.StatNo == data.SStatNo).Select(x => x.AreaID).FirstOrDefault();
						var aAreaID = db.ORG_Stat.Where(x => x.StatNo == data.AStatNo).Select(x => x.AreaID).FirstOrDefault();
						userRecord.ImOrEx = sArealD == aAreaID ? "Im" : "Ex";
					}
					//以下系統自填
					userRecord.UpdateTime = DateTime.Now;
					userRecord.UpdateBy = User.Identity.Name;
					db.Entry(userRecord).State = EntityState.Modified;
					try
					{
						db.SaveChanges();
						trans.Commit();
						result.Ok = DataModifyResultType.Success;
						result.Message = "OK";
						//if (tab == "Tab3")
							//SavePML_TWN(ladingNo);
					}
					catch (Exception e)
					{
						result.Ok = DataModifyResultType.Faild;
						result.Message = e.Message;
						trans.Rollback();
					}
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region Delete Lading
		[Authorize]
		public ActionResult DeleteLading(Bill_Lading data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var deleteData = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == data.LadingNo);
				var dtlData = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo);
				var shdetNo = db.ShdetHeader.Where(x => x.LadingNo == data.LadingNo).Select(x => x.ShdetNo).FirstOrDefault();
				var sHData = db.ShdetHeader.FirstOrDefault(x => x.ShdetNo == shdetNo);
				var sDData = db.ShdetDetail.Where(x => x.ShdetNo == shdetNo);
				var sPData = db.ShdetProd.Where(x => x.ShdetNo == shdetNo);
				if (sPData.Count() != 0)
					foreach (var s in sPData)
					{
						s.IsDelete = true;
						s.DeletedBy = User.Identity.Name;
						s.DeletedDate = DateTime.Now;
						db.Entry(s).State = EntityState.Modified;
					}
				if (sDData.Count() != 0)
					foreach (var s in sDData)
					{
						s.IsDelete = true;
						s.DeletedBy = User.Identity.Name;
						s.DeletedDate = DateTime.Now;
						db.Entry(s).State = EntityState.Modified;
					}
				if (sHData != null)
				{
					sHData.IsDelete = true;
					sHData.DeletedBy = User.Identity.Name;
					sHData.DeletedDate = DateTime.Now;
					db.Entry(sHData).State = EntityState.Modified;
				}
				if (dtlData.Count() != 0)
					foreach (var d in dtlData)
					{
						d.IsDelete = true;
						d.DeletedBy = User.Identity.Name;
						d.DeletedTime = DateTime.Now;
						db.Entry(d).State = EntityState.Modified;
					}

				if (deleteData != null)
				{
					//以下系統自填
					deleteData.DeletedTime = DateTime.Now;
					deleteData.DeletedBy = User.Identity.Name;
					deleteData.IsDelete = true;
					db.Entry(deleteData).State = EntityState.Modified;
				}
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
					trans.Rollback();
				}
				try
				{
					result.Ok = DataModifyResultType.Success;
					db.SaveChanges();
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
		#endregion

		#region Bill_Lading_Record Grid
		public ActionResult GetRecordJSON(Bill_Lading_Record data, int page = 1, int rows = 40)
		{

			var recordData =
				from br in db.Bill_Lading_Record.Where(x => x.IsDelete == false)
				join b in db.Bill_Lading.Where(x => x.IsDelete == false)
				on br.LadingNo equals b.LadingNo into ps
				from b in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = br.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					SNo = br.SNo,
					RecordDate = br.RecordDate,
					RecordType = br.RecordType,
					StatNo = br.StatNo,
					Remark = br.Remark,
					CreateBy = br.CreateBy,
					CreateTime = br.CreateTime,
					UpdateBy = br.UpdateBy,
					UpdateTime = br.UpdateTime,
					DeletedBy = br.DeletedBy,
					DeletedTime = br.DeletedTime,
					IsDelete = br.IsDelete
				};
			if (data.LadingNo.IsNotEmpty())
				recordData = recordData.Where(x => x.LadingNo == data.LadingNo);
			if (data.SNo > 0)
				recordData = recordData.Where(x => x.SNo == data.SNo);
			var records = recordData.Count();
			recordData = recordData.OrderBy(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = recordData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		#region Add Record
		[Authorize]
		public ActionResult AddRecord(Bill_Lading_Record data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var userRecord = db.Bill_Lading_Record.OrderByDescending(x => x.SNo).FirstOrDefault(x => x.LadingNo == data.LadingNo);
			var saveData = new Bill_Lading_Record()
			{
				LadingNo = data.LadingNo,
				SNo = userRecord == null ? 1 : userRecord.SNo + 1,
				RecordDate = data.RecordDate,
				RecordType = data.RecordType,
				StatNo = ((UserLoginInfo)Session["UserLoginInfo"]).statNo,
				Remark = data.Remark,
				CreateTime = DateTime.Now,
				CreateBy = User.Identity.Name,
				IsDelete = false,
			};
			db.Bill_Lading_Record.Add(saveData);
			var result = new ResultHelper();
			try
			{
				db.SaveChanges();
				result.Ok = DataModifyResultType.Success;
				result.Message = "SNo:" + saveData.SNo;
			}
			catch (Exception e)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = e.Message;
			}

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region Edit Record
		[Authorize]
		public ActionResult EditRecord(Bill_Lading_Record data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var editData = db.Bill_Lading_Record.FirstOrDefault(x => x.LadingNo == data.LadingNo && x.SNo == data.SNo);
			editData.RecordDate = data.RecordDate;
			editData.RecordType = data.RecordType;
			editData.Remark = data.Remark;
			editData.StatNo = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
			editData.UpdateBy = User.Identity.Name;
			editData.UpdateTime = DateTime.Now;

			var result = new ResultHelper();
			try
			{
				db.Entry(editData).State = EntityState.Modified;
				db.SaveChanges();
				result.Ok = DataModifyResultType.Success;
			}
			catch (Exception e)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = e.Message;
			}

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		#endregion

		#region Delete Record
		[Authorize]
		public ActionResult DeleteRecord(Bill_Lading_Record data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var deleteData = db.Bill_Lading_Record.FirstOrDefault(x => x.LadingNo == data.LadingNo && x.SNo == data.SNo);
				//以下系統自填
				deleteData.IsDelete = true;
				deleteData.DeletedTime = DateTime.Now;
				deleteData.DeletedBy = User.Identity.Name;
				db.Entry(deleteData).State = EntityState.Modified;
				try
				{
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
				}
				catch (Exception e)
				{
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					result.Message = e.Message;
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region 回寫哲盟
		public void UpdateLadingNo_Type(string original_LadingNo_Type, string LadingNo_Type)
		{
			SqlConnection dataConnection = new SqlConnection();
			String sqlConnectionStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PML_TWN"].ConnectionString;
			string sqlstr = $"UPDATE manifest SET MANI_ID = {LadingNo_Type}, JOBNO = {LadingNo_Type} WHERE JOBNO = {original_LadingNo_Type}";
			SqlCommand SqlCmd = new SqlCommand(sqlstr, dataConnection);
			try
			{
				dataConnection.ConnectionString = sqlConnectionStr;

				dataConnection.Open();

				SqlCmd.ExecuteNonQuery();

				dataConnection.Close();
			}
			catch (Exception e)
			{
				//insertResult = e.Message;
			}
			finally
			{
				SqlCmd.Cancel();
				dataConnection.Close();
				dataConnection.Dispose();
			}
		}

		public void SavePML_TWN(string LadingNo)
		{
			var statNoSession = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
			var masData = db.Bill_Lading.Where(x => x.LadingNo == LadingNo).FirstOrDefault();
			var dtlData = db.DeclCust_Main.Where(x => x.LadingNo == LadingNo);
			var custData = db.ORG_Cust.Where(x => x.CustNo == masData.SendCustNo).FirstOrDefault();
			var custNo = custData == null ? "" : custData.InvNo != null ? custData.InvNo : custData.IDNo != null ? custData.IDNo : custData.Phone;
			var dtlStr = "";
			var dtlseq = 0;
			foreach (var dtl in dtlData)
			{
				dtlseq++;
				dtlStr = dtlStr + $"{dtlseq};{dtl.BagNo};{dtl.CleCusCode};{dtl.ProductNo};{dtl.ProductName};{dtl.Type};{dtl.Pcs};{dtl.GrossWeight};{dtl.Weight};{dtl.Price};";
			}
			var SdData = db.ShdetDetail.Where(x => x.ShdetNo == LadingNo && x.IsDelete == false).OrderBy(x => x.sNo);
			var SdCount = SdData.Count();
			var SectorNo = SdCount != 0 ? SdData.Select(x => x.SectorNo).FirstOrDefault() : " ";
			var PrintLang = db.ORG_Hub.Where(x => x.HubNo == masData.HubNo).Select(x => x.PrintLang).FirstOrDefault();
			var HubName = masData.HubNo != null ? db.ORG_Hub.Where(x => x.HubNo == masData.HubNo).Select(x => x.HubName).FirstOrDefault() : "";
			var Addr = PrintLang != "en" ? masData.RecChAddr : "";
			var EnAddr = PrintLang == "en" ? masData.RecChAddr : "";
			var Company = PrintLang != "en" ? masData.RecCompany : "";
			var EnCompany = PrintLang == "en" ? masData.RecCompany : "";
			var SendCompany = PrintLang == "en" ? masData.SendECompany : masData.SendCompany;
			var SendBy = PrintLang == "en" ? masData.SendEBy : masData.SendBy;
			var SendAddr = PrintLang == "en" ? masData.SendECustAddr : masData.SendCustAddr;
			SqlConnection dataConnection = new SqlConnection();
			String sqlConnectionStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PML_TWN"].ConnectionString;
			var Type = masData.Type == null ? "" : masData.Type == "0" ? "DOC" : "SPX";
			string sqlstr =
				$"exec st_intelink_yd_jobno_Import_Check2019062101;1 '{masData.LadingNo_Type}','pml','{ statNoSession}','NEWPML','{masData.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")}','{masData.CcNo}','{custNo}','{SendCompany}','{SendBy}','{masData.SendPhone}','{SendAddr}','{SectorNo}','','','{masData.DestNo}','{masData.CName}',{masData.Volume ?? 0},{masData.Weight ?? 0},{masData.Freight ?? 0},'{ masData.Currency}','{Type}',{masData.ToPayment ?? 0},{masData.InsurancePay ?? 0},{masData.AgentPay ?? 0},{masData.OtherPayTax ?? 0},0,'NEWPML','{Company}','{masData.RecInvNo}','{Addr}','{masData.RecBy}','{masData.RecPhone}','{masData.HubName}','{masData.SStatNo}','{masData.SStatName}','{masData.Remark}',{masData.PiecesNo ?? 0},1,'{(masData.WarehouseRDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")}','{masData.WarehouseRNo}','','{masData.ProductName}','{EnCompany}','','{masData.RecCity}','{masData.RecState}',0,'{masData.Hsno}',{masData.ProdIdPay ?? 0},{masData.CustomsPay ?? 0},{masData.OtherPayNoTax ?? 0},{masData.Freight ?? 0 + masData.CustomsPay ?? 0 + masData.ProdIdPay ?? 0 + masData.InsurancePay ?? 0 + masData.OtherPayTax ?? 0 + masData.OtherPayNoTax ?? 0 + masData.ToPayment ?? 0 + masData.AgentPay ?? 0 },{masData.FuelCosts ?? 0 },'{masData.PayCustNo}','{masData.PayCustCHName}','{masData.Currency}',{masData.Cost ?? 0 },'{masData.Currency}','{masData.RecPostDist}','{masData.RecCountry}','{EnAddr}',{masData.Qty ?? 0 },'','','{dtlStr}',1,{masData.Length ?? 0},{masData.Width ?? 0},{masData.Height ?? 0},'{masData.ForIndonesian}'";

			SqlCommand SqlCmd = new SqlCommand(sqlstr, dataConnection);
			try
			{
				dataConnection.ConnectionString = sqlConnectionStr;

				dataConnection.Open();

				SqlCmd.ExecuteNonQuery();

				dataConnection.Close();
			}
			catch (Exception e)
			{
				//insertResult = e.Message;
			}
			finally
			{
				SqlCmd.Cancel();
				dataConnection.Close();
				dataConnection.Dispose();
			}
		}

		#endregion
		#endregion

		#region 調派
		#region SD GRID
		[Authorize]
		public ActionResult GetSDJSON(int? sNo, int page = 1, int rows = 40, string ShdetNo = "", string CustNo = "", string LadingNo = "")
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
			if (LadingNo != "")
			{
				var shdetNo = db.ShdetHeader.Where(x => x.LadingNo == LadingNo).Select(x => x.ShdetNo).FirstOrDefault();
				if (shdetNo != null)
					ShdetNo = shdetNo;
			}

			var data = from d in db.ShdetDetail.Where(x => x.IsDelete == false)
					   join s in db.ORG_Sector
					   on d.SectorNo equals s.SectorNo into ps
					   from s in ps.DefaultIfEmpty()
					   join v in db.ORG_Vehicle
					   on d.CarID equals v.CarID into ps1
					   from v in ps1.DefaultIfEmpty()
					   join h in db.ShdetHeader
					   on d.ShdetNo equals h.ShdetNo into ps2
					   from h in ps2.DefaultIfEmpty()
					   join b in db.Bill_Lading
					   on d.ShdetNo equals b.LadingNo into ps3
					   from b in ps3.DefaultIfEmpty()
					   where statNoList.Contains(d.StatNo) || statNoList.Contains(d.CallStatNo) || statNoList.Contains(b.SStatNo) || statNoList.Contains(b.AStatNo)
					   select new
					   {
						   d.ShdetNo,
						   d.CustNo,
						   d.sNo,
						   d.CarryName,
						   d.Code5,
						   d.Code7,
						   d.Add_1,
						   d.Add_2,
						   d.Add_3,
						   d.Add_4,
						   d.Add_5,
						   d.Add_6,
						   CustAddrFull = d.CustAddr + (d.Add_1 == 0 ? null : (d.Add_1 + "段")) + (d.Add_2 == 0 ? null : (d.Add_2 + "巷")) + (d.Add_3 == 0 ? null : (d.Add_3 + "弄")) + (d.Add_4 == "" || d.Add_4 == null ? null : (d.Add_4 + "號")) + (d.Add_5 == 0 ? null : (d.Add_5 + "樓")) + (d.Add_6 == null || d.Add_6 == "" ? null : d.Add_6),
						   d.CustAddr,
						   d.CustENAddr1,
						   d.CustENAddr2,
						   d.City,
						   d.State,
						   d.Country,
						   d.CtcSale,
						   d.Tel,
						   d.Clerk,
						   d.PickUpAreaNo,
						   d.EndDate,
						   d.Remark2,
						   d.Remark3,
						   d.SectorNo,
						   s.SectorName,
						   d.CallType,
						   d.StatNo,
						   h.IsDesp,
						   h.IsCancel,
						   h.IsReply,
						   d.IsRedy,
						   h.IsFinish,
						   d.ShdetBy,
						   d.ShdetDate,
						   d.CancelBy,
						   d.CancelDate,
						   d.ReplyBy,
						   d.ReplyDate,
						   d.UpdatedBy,
						   d.UpdatedDate,
						   d.DeletedBy,
						   d.DeletedDate,
						   d.IsDelete,
						   d.WeigLevel,
						   d.CocustomTyp,
						   d.HubNo,
						   d.CcNo,
						   d.Charge,
						   d.RedyDate,
						   d.Dest,
						   d.RedyTime,
						   d.CarID,
						   v.CarNO,
						   d.CallStatNo,
						   h.LadingNo,
						   d.ADate,
						   d.ATime,
						   d.SectorPhone,
					   };

			if (ShdetNo != null)
				data = data.Where(x => x.ShdetNo == ShdetNo);
			if (CustNo != "")
				data = data.Where(x => x.CustNo == CustNo);
			if (sNo > 0)
				data = data.Where(x => x.sNo == sNo);

			int records = data.Count();
			data = data.OrderBy(o => o.ShdetNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Records = records,
				Data = data,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		#region SP GRID
		[Authorize]
		public ActionResult GetSPJSON(int page = 1, int rows = 40, string ShdetNo = "", string CustNo = "", int sDtlNo = 0)
		{
			var data = db.ShdetProd.Where(x => x.IsDelete == false);

			if (ShdetNo != null)
				data = data.Where(x => x.ShdetNo == ShdetNo);
			if (CustNo != "")
				data = data.Where(x => x.CustNo == CustNo);
			if (sDtlNo != 0) //Detail的sNo
				data = data.Where(x => x.sDtlNo == sDtlNo);

			var records = data.Count();
			data = data.OrderBy(o => o.ShdetNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Records = records,
				Data = data,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		#region SectorMission GRID
		[Authorize]
		public ActionResult GetSectorMission(string sectorNo, DateTime? start_date = null, DateTime? end_date = null, int page = 1, int rows = 40)
		{
			var data = from SD in db.ShdetDetail.Where(x => x.IsDelete == false && x.RedyDate != null)
					   join p in db.ORG_PickUpArea
					   on SD.PickUpAreaNo equals p.PickUpAreaNo into ps
					   from p in ps.DefaultIfEmpty()
					   select new
					   {
						   SD.sNo,
						   SD.SectorNo,
						   SD.RedyTime,
						   p.PickUpAreaName,
						   SD.CarryName,
						   SD.RedyDate,
					   };

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				data = data.Where(x => DbFunctions.TruncateTime(x.RedyDate).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.RedyDate).Value.CompareTo(sDate) >= 0);
			}
			data = data.Where(x => x.SectorNo == sectorNo);
			var sData = data.ToList().Select((x, index) => new ShdetDetail()
			{
				Index = index,
				SectorNo = x.SectorNo,
				RedyTime = x.RedyTime,
				PickUpAreaName = x.PickUpAreaName,
				CarryName = x.CarryName,
				RedyDate = x.RedyDate,
			}) as IEnumerable<ShdetDetail>;
			int records = sData.Count();
			sData = sData.OrderBy(o => o.RedyTime).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Records = records,
				Data = sData,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
		#endregion

		#region Add SD
		[Authorize]
		public ActionResult AddShdetDetail(ShdetDetail data, bool IsDesp = false)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var blData = db.Bill_Lading.Where(x => x.LadingNo == data.ShdetNo).FirstOrDefault();
				var lastShdet = db.ShdetDetail.Where(x => x.ShdetNo == data.ShdetNo).OrderByDescending(x => x.sNo).FirstOrDefault();
				var saveData = new ShdetDetail();
				saveData.LadingNo_Type = blData.LadingNo_Type;
				saveData.ShdetNo = data.ShdetNo;
				saveData.CustNo = data.CustNo;
				saveData.sNo = lastShdet == null ? 1 : lastShdet.sNo + 1;
				saveData.CarryName = data.CarryName;
				saveData.Code5 = data.Code5;
				saveData.Code7 = data.Code7;
				saveData.Add_1 = data.Add_1;
				saveData.Add_2 = data.Add_2;
				saveData.Add_3 = data.Add_3;
				saveData.Add_4 = data.Add_4;
				saveData.Add_5 = data.Add_5;
				saveData.Add_6 = data.Add_6;
				saveData.CustAddr = data.CustAddr;
				saveData.CtcSale = data.CtcSale;
				saveData.Tel = data.Tel;
				saveData.PickUpAreaNo = data.PickUpAreaNo;
				saveData.EndDate = data.EndDate;
				saveData.SectorNo = data.SectorNo;
				saveData.StatNo = data.StatNo;
				if (IsDesp == true)
				{
					saveData.ShdetBy = User.Identity.Name;
					saveData.ShdetDate = DateTime.Now;
					var shData = db.ShdetHeader.Where(x => x.ShdetNo == data.ShdetNo).FirstOrDefault();
					shData.IsDesp = true;
					db.Entry(shData).State = EntityState.Modified;
				}
				saveData.IsFinish = data.IsFinish == null ? false : data.IsFinish;
				if (data.IsFinish == true)
				{
					saveData.FinishBy = User.Identity.Name;
					saveData.FinishDate = DateTime.Now;
				}
				saveData.IsCancel = data.IsCancel == null ? false : data.IsCancel;
				if (data.IsCancel == true)
				{
					saveData.CancelBy = User.Identity.Name;
					saveData.CancelDate = DateTime.Now;
				}
				saveData.IsRedy = data.IsRedy == null ? false : data.IsRedy;
				saveData.WeigLevel = data.WeigLevel;
				saveData.CocustomTyp = data.CocustomTyp;
				saveData.HubNo = data.HubNo;
				saveData.Dest = data.Dest;
				saveData.CcNo = data.CcNo;
				saveData.RedyDate = data.RedyDate;
				saveData.RedyTime = data.RedyTime;
				saveData.CarID = data.CarID;
				saveData.CallStatNo = data.CallStatNo;
				saveData.ADate = data.ADate;
				saveData.ATime = data.ATime;
				saveData.SectorPhone = data.SectorPhone;
				//以下系統自填
				saveData.CreatedDate = DateTime.Now;
				saveData.CreatedBy = User.Identity.Name;
				saveData.IsDelete = false;

				db.ShdetDetail.Add(saveData);
				try
				{
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "sNo:" + saveData.sNo;
				}
				catch (Exception e)
				{
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					result.Message = e.Message;
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region Edit SD
		[Authorize]
		public ActionResult EditShdetDetail(ShdetDetail data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var editData = db.ShdetDetail.FirstOrDefault(x => x.ShdetNo == data.ShdetNo && x.sNo == data.sNo);
				editData.CustNo = data.CustNo;
				editData.CarryName = data.CarryName;
				editData.Code5 = data.Code5;
				editData.Code7 = data.Code7;
				editData.Add_1 = data.Add_1;
				editData.Add_2 = data.Add_2;
				editData.Add_3 = data.Add_3;
				editData.Add_4 = data.Add_4;
				editData.Add_5 = data.Add_5;
				editData.Add_6 = data.Add_6;
				editData.CustAddr = data.CustAddr;
				editData.CtcSale = data.CtcSale;
				editData.Tel = data.Tel;
				editData.PickUpAreaNo = data.PickUpAreaNo;
				editData.EndDate = data.EndDate;
				editData.SectorNo = data.SectorNo;
				editData.StatNo = data.StatNo;
				if (editData.IsFinish != true && data.IsFinish == true)
				{
					editData.FinishBy = User.Identity.Name;
					editData.FinishDate = DateTime.Now;
				}
				editData.IsFinish = data.IsFinish;
				if (data.IsCancel == true)
				{
					editData.CancelBy = User.Identity.Name;
					editData.CancelDate = DateTime.Now;
				}
				editData.IsCancel = editData.IsCancel;
				editData.IsRedy = data.IsRedy;
				editData.WeigLevel = data.WeigLevel;
				editData.CocustomTyp = data.CocustomTyp;
				editData.Dest = data.Dest;
				editData.CcNo = data.CcNo;
				editData.RedyDate = data.RedyDate;
				editData.RedyTime = data.RedyTime;
				if (editData.CarID != data.CarID)
				{
					var SPData = db.ShdetProd.Where(x => x.ShdetNo == data.ShdetNo && x.sDtlNo == data.sNo);
					foreach (var s in SPData)
					{
						s.CarID = data.CarID;
						db.Entry(s).State = EntityState.Modified;
					}
				}
				editData.CarID = data.CarID;
				editData.CallStatNo = data.CallStatNo;
				editData.ADate = data.ADate;
				editData.ATime = data.ATime;
				editData.SectorPhone = data.SectorPhone;
				//以下系統自填
				editData.UpdatedDate = DateTime.Now;
				editData.UpdatedBy = User.Identity.Name;
				db.Entry(editData).State = EntityState.Modified;
				try
				{
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
				}
				catch (Exception e)
				{
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					result.Message = e.Message;
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region Delete SD
		[Authorize]
		public ActionResult DeleteShdetDetail(ShdetDetail data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var deleteData = db.ShdetDetail.FirstOrDefault(x => x.ShdetNo == data.ShdetNo && x.sNo == data.sNo);
				//以下系統自填
				deleteData.IsDelete = true;
				deleteData.DeletedDate = DateTime.Now;
				deleteData.DeletedBy = User.Identity.Name;
				db.Entry(deleteData).State = EntityState.Modified;
				try
				{
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
				}
				catch (Exception e)
				{
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					result.Message = e.Message;
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion
		#endregion

		#region 檔案上傳FTP && FileTable資料表
		[Authorize]
		public ActionResult GetFileTableJSON(BL_FileTable data, int page = 1, int rows = 40)
		{
			var bl_FileTable = db.BL_FileTable.Where(x => x.IsDelete == false);

			if (data.LadingNo.IsNotEmpty())
				bl_FileTable = bl_FileTable.Where(x => x.LadingNo == data.LadingNo);

			var bl_FileTableData = bl_FileTable.ToList().Select((x, index) => new BL_FileTable()
			{
				Index = index,
				LadingNo = x.LadingNo,
				SNo = x.SNo,
				FileName = x.FileName,
				UploadBy = x.UploadBy,
				UploadTime = x.UploadTime,
				DeletedBy = x.DeletedBy,
				DeletedTime = x.DeletedTime,
				IsDelete = x.IsDelete,
			}) as IEnumerable<BL_FileTable>;

			bl_FileTableData = bl_FileTableData.OrderBy(o => o.SNo).Skip((page - 1) * rows).Take(rows);
			int records = bl_FileTable.Count();

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = bl_FileTableData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Upload(HttpPostedFileBase file, string fileLadingNo)
		{
			var result = new ResultHelper();
			if (file.ContentLength > 0)
			{
				try
				{
					//暫存位置
					System.IO.Directory.CreateDirectory(FileTablePath + fileLadingNo);
					string savedName = Path.Combine(FileTablePath + fileLadingNo, file.FileName);
					file.SaveAs(savedName);
					try
					{
						// Check 紀錄表是否為第一個檔案，是：建立FTP資料夾
						if (!db.BL_FileTable.Any(x => x.LadingNo == fileLadingNo && x.IsDelete == false))
						{
							string MKD_uri = path + fileLadingNo;
							FtpWebRequest MKD_FtpReq = (FtpWebRequest)FtpWebRequest.Create(new Uri(MKD_uri));
							MKD_FtpReq.UseBinary = true;
							MKD_FtpReq.Credentials = new NetworkCredential(username, password);
							MKD_FtpReq.Method = WebRequestMethods.Ftp.MakeDirectory;
							FtpWebResponse MKD_Response = (FtpWebResponse)MKD_FtpReq.GetResponse();
							MKD_Response.Close();
						}
						if (db.BL_FileTable.Any(x => x.LadingNo == fileLadingNo && x.FileName == file.FileName && x.IsDelete == false))
						{
							result.Ok = DataModifyResultType.Faild;
							result.Message = "【" + file.FileName + "】，已存在相同檔案名稱！";
						}
						else
						{
							//上傳檔案至FTP
							string STOR_url = path + fileLadingNo + "//";
							FtpWebRequest STOR_FtpReq = null;
							try
							{
								FileInfo fileInfo = new FileInfo(FileTablePath + fileLadingNo + "\\" + file.FileName);
								using (FileStream fs = fileInfo.OpenRead())
								{
									long length = fs.Length;
									STOR_FtpReq = (FtpWebRequest)FtpWebRequest.Create(new Uri(STOR_url + fileInfo.Name));
									STOR_FtpReq.Credentials = new NetworkCredential(username, password);
									STOR_FtpReq.KeepAlive = false;
									STOR_FtpReq.Method = WebRequestMethods.Ftp.UploadFile;
									STOR_FtpReq.UseBinary = true;

									using (Stream stream = STOR_FtpReq.GetRequestStream())
									{
										//設定緩衝大小
										int BufferLength = 20480;
										byte[] b = new byte[BufferLength];
										int i;
										while ((i = fs.Read(b, 0, BufferLength)) > 0)
										{
											stream.Write(b, 0, i);
										}
										result.Ok = DataModifyResultType.Success;
										result.Message = "【" + file.FileName + "】，上傳檔案成功！";
									}
								}
							}
							catch (Exception ex)
							{
								string RMD_uri = path + fileLadingNo;
								FtpWebRequest RMD_FtpReq = (FtpWebRequest)FtpWebRequest.Create(new Uri(RMD_uri));
								RMD_FtpReq.Credentials = new NetworkCredential(username, password);
								RMD_FtpReq.Method = WebRequestMethods.Ftp.RemoveDirectory;
								FtpWebResponse RMD_Response = (FtpWebResponse)RMD_FtpReq.GetResponse();
								RMD_Response.Close();
								result.Ok = DataModifyResultType.Faild;
								result.Message = "上傳檔案失敗：" + ex.Message;
							}
						}
					}
					catch (Exception ex)
					{
						result.Ok = DataModifyResultType.Faild;
						result.Message = "建立目錄失敗：" + ex.Message;
					}
					System.IO.File.Delete(FileTablePath + fileLadingNo + "\\" + file.FileName);
					System.IO.Directory.Delete(FileTablePath + fileLadingNo);
				}
				catch (System.IO.IOException e)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = e.Message;
				}
			}
			//寫入紀錄表
			if (result.Ok == DataModifyResultType.Success)
			{
				var lastData = db.BL_FileTable.Where(x => x.LadingNo == fileLadingNo).OrderByDescending(x => x.SNo).FirstOrDefault();
				var saveData = new BL_FileTable();
				saveData.LadingNo = fileLadingNo;
				saveData.SNo = lastData == null ? 1 : lastData.SNo + 1;
				saveData.FileName = file.FileName;
				saveData.UploadBy = User.Identity.Name;
				saveData.UploadTime = DateTime.Now;
				saveData.IsDelete = false;
				db.BL_FileTable.Add(saveData);
				db.SaveChanges();
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public void Download(string fileLadingNo, string FileName)
		{
			var result = new ResultHelper();
			try
			{
				string RETR_uri = path + fileLadingNo + "\\" + FileName;
				FtpWebRequest RETR_FtpReq = (FtpWebRequest)FtpWebRequest.Create(new Uri(RETR_uri));
				RETR_FtpReq.UseBinary = true;
				RETR_FtpReq.Credentials = new NetworkCredential(username, password);
				RETR_FtpReq.Method = WebRequestMethods.Ftp.DownloadFile;
				FtpWebResponse RETR_Response = (FtpWebResponse)RETR_FtpReq.GetResponse();
				Stream responseStream = RETR_Response.GetResponseStream();//取得FTP伺服器回傳的資料流
				string fileName = Path.GetFileName(RETR_FtpReq.RequestUri.AbsolutePath);
				if (fileName.Length == 0)
				{
					StreamReader reader = new StreamReader(responseStream);
					throw new Exception(reader.ReadToEnd());
				}
				else
				{
					byte[] buffer = new byte[20480];
					int read = 0;
					MemoryStream ms = new MemoryStream();
					while ((read = responseStream.Read(buffer, 0, 20480)) > 0)
					{
						ms.Write(buffer, 0, read);
					}
					ms.Flush();
					Response.Clear();
					Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + FileName));
					Response.BinaryWrite(ms.ToArray());
				}

			}
			catch (Exception ex)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "下載檔案失敗：" + ex.Message;
			}
			//string filepath = FileTablePath + LadingNo + "\\" + FileName;
			//string filename = System.IO.Path.GetFileName(filepath);
			//Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
			//return File(iStream, "application/zip", filename);
		}

		[Authorize]
		public ActionResult DeleteFile(string fileLadingNo, string FileName, int SNo)
		{
			var result = new ResultHelper();
			try
			{
				string DELE_uri = path + fileLadingNo + "\\" + FileName;
				FtpWebRequest DELE_FtpReq = (FtpWebRequest)FtpWebRequest.Create(new Uri(DELE_uri));
				DELE_FtpReq.UseBinary = true;
				DELE_FtpReq.Credentials = new NetworkCredential(username, password);
				DELE_FtpReq.KeepAlive = false;
				DELE_FtpReq.Method = WebRequestMethods.Ftp.DeleteFile;
				FtpWebResponse DELE_response = (FtpWebResponse)DELE_FtpReq.GetResponse();
				DELE_response.Close();
				//Check紀錄表是否為最後一個檔案，是：移除資料夾
				if (db.BL_FileTable.Where(x => x.LadingNo == fileLadingNo && x.IsDelete == false).Count() == 1)
				{
					string RMD_uri = path + fileLadingNo;
					FtpWebRequest RMD_FtpReq = (FtpWebRequest)FtpWebRequest.Create(new Uri(RMD_uri));
					RMD_FtpReq.Credentials = new NetworkCredential(username, password);
					RMD_FtpReq.Method = WebRequestMethods.Ftp.RemoveDirectory;
					FtpWebResponse RMD_response = (FtpWebResponse)RMD_FtpReq.GetResponse();
					RMD_response.Close();
				}
				result.Ok = DataModifyResultType.Success;
				result.Message = "【" + FileName + "】，刪除檔案成功！";
			}
			catch (Exception ex)
			{
				result.Ok = DataModifyResultType.Success;
				result.Message = "刪除檔案失敗：" + ex.Message;
			}
			//寫入紀錄表
			if (result.Ok == DataModifyResultType.Success)
			{
				var editData = db.BL_FileTable.Where(x => x.LadingNo == fileLadingNo && x.FileName == FileName && x.SNo == SNo).FirstOrDefault();
				editData.IsDelete = true;
				editData.DeletedBy = User.Identity.Name; ;
				editData.DeletedTime = DateTime.Now;
				db.Entry(editData).State = EntityState.Modified;
				db.SaveChanges();
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region other function
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

		public class LadingCompare : IEqualityComparer<Bill_Lading>
		{
			bool IEqualityComparer<Bill_Lading>.Equals(Bill_Lading x, Bill_Lading y)
			{
				return (x.LadingNo == y.LadingNo && x.LadingDate == y.LadingDate);
			}

			int IEqualityComparer<Bill_Lading>.GetHashCode(Bill_Lading obj)
			{
				int hash = 17;
				hash = hash * 23 + (obj.LadingNo).GetHashCode();
				hash = hash * 23 + (obj.LadingDate).GetHashCode();
				return hash;
			}
		}

		public class MainCompare : IEqualityComparer<DeclCust_Main>
		{
			bool IEqualityComparer<DeclCust_Main>.Equals(DeclCust_Main x, DeclCust_Main y)
			{
				return (x.LadingNo == y.LadingNo && x.sNo == y.sNo);
			}

			int IEqualityComparer<DeclCust_Main>.GetHashCode(DeclCust_Main obj)
			{
				int hash = 17;
				hash = hash * 23 + (obj.sNo).GetHashCode();
				hash = hash * 23 + (obj.LadingNo).GetHashCode();
				return hash;
			}
		}

		[Authorize]
		public ActionResult SetDesp(string ShdetNo)
		{
			var result = new ResultHelper();
			var SHData = db.ShdetHeader.Where(x => x.ShdetNo == ShdetNo).FirstOrDefault();
			if (CheckProd(ShdetNo))
			{
				SHData.IsDesp = true;
				SHData.ShdetBy = User.Identity.Name;
				SHData.ShdetDate = DateTime.Now;
				db.Entry(SHData).State = EntityState.Modified;
				db.SaveChanges();
				try
				{
					result.Ok = DataModifyResultType.Success;
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
				result.Message = "尚有取件地址無取件貨物資料。";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		private Boolean CheckProd(string ShdetNo)
		{
			var SDData = db.ShdetDetail.Where(x => x.ShdetNo == ShdetNo);
			var completeData = false;
			foreach (var s in SDData)
			{
				completeData = db.ShdetProd.Any(x => x.ShdetNo == s.ShdetNo && x.CustNo == s.CustNo && x.sDtlNo == s.sNo);
				if (!completeData)
					return false;
			}
			return true;
		}

		#endregion

		#region 提單列印
		//電子提單
		public ActionResult Report(string id)
		{
			Printed(id);
			var data =
				from b in db.Bill_Lading.Where(x => x.LadingNo == id && x.IsDelete == false)
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on b.HubNo equals h.HubNo into ps
				from h in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCompany = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCompany ?? " " : (h.PrintLang == "en") ? b.SendECompany ?? " " : " ",
					SendCustNo = b.SendCustNo ?? " ",
					SendCustAddr = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCustAddr ?? " " : (h.PrintLang == "en") ? b.SendECustAddr ?? " " : " ",
					RecCompany = b.RecCompany ?? " ",
					RecChAddr = b.RecChAddr ?? " ",
					RecBy = b.RecBy ?? " ",
					RecPhone = b.RecPhone ?? " ",
					SStatNo = b.SStatNo ?? " ",
					AStatNo = b.AStatNo ?? " ",
					DestNo = b.DestNo ?? " ",
					Qty = b.Qty ?? 0,
					Weight = b.Weight ?? 0,
					HubPName = h.HubPName ?? " ",
					HubName = h.HubName ?? " ",
					Volume = b.Volume ?? 0,
					CcNo = b.CcNo ?? " ",
					Currency = b.Currency ?? " ",
					ToPayment = b.ToPayment ?? 0,
					Remark = b.Remark ?? " ",
					//Type = b.Type ?? " ",
				};
			var viewData = data.ToList().Select((x, index) => new BillLadingReport()
			{
				LadingNo = x.LadingNo,
				LadingNo_Type = x.LadingNo_Type,
				LadingDate = string.Format("{0:yyyy/MM/dd}", x.LadingDate),
				SendCompany = x.SendCompany,
				SendCustNo = x.SendCustNo,
				SendCustAddr = x.SendCustAddr,
				RecCompany = x.RecCompany,
				RecChAddr = x.RecChAddr,
				RecBy = x.RecBy,
				RecPhone = x.RecPhone,
				SStatNo = x.SStatNo,
				AStatNo = x.AStatNo,
				DestNo = x.DestNo,
				Qty = (int)x.Qty,
				Weight = x.Weight,
				HubPName = x.HubPName,
				HubName = x.HubName,
				Volume = x.Volume,
				CcNo = x.CcNo,
				Currency = x.Currency,
				ToPayment = x.ToPayment,
				Remark = x.Remark,
				//Type = x.Type == "0" ? "文件　　　" : x.Type == "1" ? "包裹5KG以下" : x.Type == "2" ? "箱貨5KG以上" : x.Type == "3" ? "木箱　　　" : x.Type == "4" ? "棧板　　　" : "",
			}) as IEnumerable<BillLadingReport>;

			ViewData.Model = viewData.FirstOrDefault();
			return View();
		}

		//電子提單A5
		public ActionResult Report2(string id)
		{
			Printed(id);
			var data =
				from b in db.Bill_Lading.Where(x => x.LadingNo == id && x.IsDelete == false)
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on b.HubNo equals h.HubNo into ps
				from h in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCompany = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCompany ?? " " : (h.PrintLang == "en") ? b.SendECompany ?? " " : " ",
					SendCustNo = b.SendCustNo ?? " ",
					SendCustAddr = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCustAddr ?? " " : (h.PrintLang == "en") ? b.SendECustAddr ?? " " : " ",
					RecCompany = b.RecCompany ?? " ",
					RecChAddr = b.RecChAddr ?? " ",
					RecBy = b.RecBy ?? " ",
					RecPhone = b.RecPhone ?? " ",
					SStatNo = b.SStatNo ?? " ",
					AStatNo = b.AStatNo ?? " ",
					DestNo = b.DestNo ?? " ",
					Qty = b.Qty ?? 0,
					Weight = b.Weight ?? 0,
					HubPName = h.HubPName ?? " ",
					HubName = h.HubName ?? " ",
					Volume = b.Volume ?? 0,
					CcNo = b.CcNo ?? " ",
					Currency = b.Currency ?? " ",
					ToPayment = b.ToPayment ?? 0,
					Remark = b.Remark ?? " ",
					//Type = b.Type ?? " ",
				};
			var viewData = data.ToList().Select((x, index) => new BillLadingReport()
			{
				LadingNo = x.LadingNo,
				LadingNo_Type = x.LadingNo_Type,
				LadingDate = string.Format("{0:yyyy/MM/dd}", x.LadingDate),
				SendCompany = x.SendCompany,
				SendCustNo = x.SendCustNo,
				SendCustAddr = x.SendCustAddr,
				RecCompany = x.RecCompany,
				RecChAddr = x.RecChAddr,
				RecBy = x.RecBy,
				RecPhone = x.RecPhone,
				SStatNo = x.SStatNo,
				AStatNo = x.AStatNo,
				DestNo = x.DestNo,
				Qty = (int)x.Qty,
				Weight = x.Weight,
				HubPName = x.HubPName,
				HubName = x.HubName,
				Volume = x.Volume,
				CcNo = x.CcNo,
				Currency = x.Currency,
				ToPayment = x.ToPayment,
				Remark = x.Remark,
				//Type = x.Type == "0" ? "文件　　　" : x.Type == "1" ? "包裹5KG以下" : x.Type == "2" ? "箱貨5KG以上" : x.Type == "3" ? "木箱　　　" : x.Type == "4" ? "棧板　　　" : "",
			}) as IEnumerable<BillLadingReport>;

			ViewData.Model = viewData.FirstOrDefault();
			return View();
		}

		//PML
		public ActionResult Report3(string id, string account)
		{
			var ID = id.Split('|')[0];
			var Type = id.Split('|')[1];
			Printed(ID);
			var SdData = db.ShdetDetail.Where(x => x.ShdetNo == ID && x.IsDelete == false).OrderBy(x => x.sNo);
			var SdCount = SdData.Count();
			var SectorName = SdCount != 0 ? db.ORG_Sector.Where(s => s.SectorNo == SdData.Select(x => x.SectorNo).FirstOrDefault()).Select(x => x.SectorName).FirstOrDefault() : " ";
			var Sector = SdCount == 0 ? "" : SdCount == 1 ? SectorName : SectorName + "(多地取件)";
			var PrintBy = db.SYS_User.Where(x => x.Account == account).Select(x => x.UserName).FirstOrDefault();
			var data =
				from b in db.Bill_Lading.Where(x => x.LadingNo == ID && x.IsDelete == false)
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on b.HubNo equals h.HubNo into ps
				from h in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCustNo = b.SendCustNo ?? " ",
					SendCompany = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCompany ?? " " : (h.PrintLang == "en") ? b.SendECompany ?? " " : " ",
					SendBy = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendBy ?? " " : (h.PrintLang == "en") ? b.SendEBy ?? " " : " ",
					SendInvNo = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendInvNo ?? " " : (h.PrintLang == "en") ? b.SendEInvNo ?? " " : " ",
					SendCustAddr = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCustAddr ?? " " : (h.PrintLang == "en") ? b.SendECustAddr ?? " " : " ",
					SendPhone = b.SendPhone ?? " ",
					SendFaxNo = b.SendFaxNo ?? " ",
					RecCompany = b.RecCompany ?? " ",
					RecChAddr = b.RecChAddr ?? " ",
					RecBy = b.RecBy ?? " ",
					RecInvNo = b.RecInvNo ?? " ",
					RecPhone = b.RecPhone ?? " ",
					RecMPhone = b.RecMPhone,
					SStatNo = b.SStatNo ?? " ",
					AStatNo = b.AStatNo ?? " ",
					DestNo = b.DestNo ?? " ",
					Qty = b.Qty ?? 0,
					Weight = b.Weight ?? 0,
					HubNo = h.HubNo ?? " ",
					HubName = h.HubName ?? " ",
					HubPName = h.HubPName ?? " ",
					Volume = b.Volume ?? 0,
					CcNo = b.CcNo ?? " ",
					Currency = b.Currency ?? " ",
					ToPayment = b.ToPayment ?? 0,
					Remark = b.Remark ?? " ",
					Type = b.Type ?? " ",
					ProductName = b.ProductName ?? " ",
					CocustomTyp = b.CocustomTyp == 0 ? "不報關" : b.CocustomTyp == 1 ? "正式報關" : b.CocustomTyp == 2 ? "簡易報關" : b.CocustomTyp == 3 ? "正式報關+後段核銷" : b.CocustomTyp == 4 ? "簡易報關+後段核銷" : b.CocustomTyp == 5 ? "不報關+後段核銷" : b.CocustomTyp == 6 ? "其他" : " ",
					SendRemark = b.SendRemark ?? " ",
					PiecesNo = b.PiecesNo ?? 0,
					Sale = b.Sale ?? " ",
					SDate = b.SDate,
				};
			var viewData = data.ToList().Select((x, index) => new BillLadingReport()
			{
				LadingNo = x.LadingNo,
				LadingNo_Type = x.LadingNo_Type,
				LadingDate = string.Format("{0:yyyy/MM/dd}", x.LadingDate),
				SendCustNo = x.SendCustNo,
				SendCompany = x.SendCompany,
				SendBy = x.SendBy,
				SendInvNo = x.SendInvNo,
				SendCustAddr = "　　　" + x.SendCustAddr,
				SendPhone = x.SendPhone,
				SendFaxNo = x.SendFaxNo,
				RecCompany = x.RecCompany,
				RecChAddr = "　　　" + x.RecChAddr,
				RecBy = x.RecBy,
				RecInvNo = x.RecInvNo,
				RecPhone = x.RecPhone,
				RecMPhone = x.RecMPhone,
				SStatNo = x.SStatNo,
				AStatNo = x.AStatNo,
				DestNo = x.DestNo,
				Qty = (int)x.Qty,
				Weight = x.Weight,
				HubNo = x.HubNo,
				HubName = x.HubName,
				HubPName = x.HubPName,
				Volume = x.Volume,
				CcNo = x.CcNo,
				Currency = x.Currency,
				ToPayment = x.ToPayment,
				Remark = x.Remark,
				Type = x.Type,
				ProductName = x.ProductName,
				CocustomTyp = x.CocustomTyp,
				SendRemark = x.SendRemark,
				PiecesNo = x.PiecesNo,
				Sale = x.Sale,
				SDate = string.Format("{0:yyyy/MM/dd}", x.SDate),
				Sector = Sector,
				PrintBy = PrintBy,
				PrintTime = DateTime.Now.ToDateTimeString(),

			}) as IEnumerable<BillLadingReport>;

			ViewData.Model = viewData.FirstOrDefault();
			ViewBag.PageSize = "E-Report3_" + Type;
			return View();
		}

		// 紙本 PML
		public ActionResult Report4(string id)
		{
			Printed(id);
			var data =
				from b in db.Bill_Lading.Where(x => x.LadingNo == id && x.IsDelete == false)
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on b.HubNo equals h.HubNo into ps
				from h in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCustNo = b.SendCustNo ?? " ",
					SendCompany = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCompany ?? " " : (h.PrintLang == "en") ? b.SendECompany ?? " " : " ",
					SendCustAddr = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCustAddr ?? " " : (h.PrintLang == "en") ? b.SendECustAddr ?? " " : " ",
					SendCity = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCity ?? " " : (h.PrintLang == "en") ? b.SendECity ?? " " : " ",
					SendPostDist = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendPostDist ?? " " : (h.PrintLang == "en") ? b.SendEPostDist ?? " " : " ",
					SendState = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendState ?? " " : (h.PrintLang == "en") ? b.SendEState ?? " " : " ",
					SendCountry = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCountry ?? " " : (h.PrintLang == "en") ? b.SendECountry ?? " " : " ",
					SendBy = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendBy ?? " " : (h.PrintLang == "en") ? b.SendEBy ?? " " : " ",
					SendPhone = b.SendPhone ?? " ",
					SendFaxNo = b.SendFaxNo ?? " ",
					SendInvNo = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendInvNo ?? " " : (h.PrintLang == "en") ? b.SendEInvNo ?? " " : " ",
					RecInvNo = b.RecInvNo,
					RecCompany = b.RecCompany ?? " ",
					RecChAddr = b.RecChAddr ?? " ",
					RecCity = b.RecCity,
					RecPostDist = b.RecPostDist,
					RecState = b.RecState,
					RecCountry = b.RecCountry,
					RecBy = b.RecBy ?? " ",
					RecPhone = b.RecPhone ?? " ",
					RecMPhone = b.RecMPhone ?? " ",
				};
			var viewData = data.ToList().Select((x, index) => new BillLadingReport()
			{
				LadingNo = x.LadingNo,
				LadingNo_Type = x.LadingNo_Type,
				LadingDate = string.Format("{0:yyyy/MM/dd}", x.LadingDate),
				SendCustNo = x.SendCustNo,
				SendCompany = x.SendCompany,
				SendCustAddr = "　　　" + x.SendCustAddr,
				SendCity = x.SendCity,
				SendPostDist = x.SendPostDist,
				SendState = x.SendState,
				SendCountry = x.SendCountry,
				SendBy = x.SendBy,
				SendPhone = x.SendPhone,
				SendFaxNo = x.SendFaxNo,
				SendInvNo = x.SendInvNo,
				RecInvNo = x.RecInvNo,
				RecCompany = x.RecCompany,
				RecChAddr = "　　　" + x.RecChAddr,
				RecCity = x.RecCity,
				RecPostDist = x.RecPostDist,
				RecState = x.RecState,
				RecCountry = x.RecCountry,
				RecBy = x.RecBy,
				RecPhone = x.RecPhone,
				RecMPhone = x.RecMPhone
			}) as IEnumerable<BillLadingReport>;

			ViewData.Model = viewData.FirstOrDefault();
			return View();
		}

		// TNT
		public ActionResult Report5(string id)
		{
			Printed(id);
			var data =
				from b in db.Bill_Lading.Where(x => x.LadingNo == id && x.IsDelete == false)
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on b.HubNo equals h.HubNo into ps
				from h in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCustNo = b.SendCustNo ?? " ",
					SendCompany = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCompany ?? " " : (h.PrintLang == "en") ? b.SendECompany ?? " " : " ",
					SendCustAddr = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCustAddr ?? " " : (h.PrintLang == "en") ? b.SendECustAddr ?? " " : " ",
					SendCity = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCity ?? " " : (h.PrintLang == "en") ? b.SendECity ?? " " : " ",
					SendPostDist = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendPostDist ?? " " : (h.PrintLang == "en") ? b.SendEPostDist ?? " " : " ",
					SendState = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendState ?? " " : (h.PrintLang == "en") ? b.SendEState ?? " " : " ",
					SendCountry = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCountry ?? " " : (h.PrintLang == "en") ? b.SendECountry ?? " " : " ",
					SendBy = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendBy ?? " " : (h.PrintLang == "en") ? b.SendEBy ?? " " : " ",
					SendPhone = b.SendPhone ?? " ",
					SendInvNo = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendInvNo ?? " " : (h.PrintLang == "en") ? b.SendEInvNo ?? " " : " ",
					RecCompany = b.RecCompany ?? " ",
					RecChAddr = b.RecChAddr ?? " ",
					RecCity = b.RecCity,
					RecPostDist = b.RecPostDist,
					RecState = b.RecState,
					RecCountry = b.RecCountry,
					RecBy = b.RecBy ?? " ",
					RecPhone = b.RecPhone ?? " ",
				};
			var viewData = data.ToList().Select((x, index) => new BillLadingReport()
			{
				LadingNo = x.LadingNo,
				LadingNo_Type = x.LadingNo_Type,
				LadingDate = string.Format("{0:yyyy/MM/dd}", x.LadingDate),
				SendCustNo = x.SendCustNo,
				SendCompany = x.SendCompany,
				SendCustAddr = "　　　" + x.SendCustAddr,
				SendCity = x.SendCity,
				SendPostDist = x.SendPostDist,
				SendState = x.SendState,
				SendCountry = x.SendCountry,
				SendBy = x.SendBy,
				SendPhone = x.SendPhone,
				SendInvNo = x.SendInvNo,
				RecCompany = x.RecCompany,
				RecChAddr = "　　　" + x.RecChAddr,
				RecCity = x.RecCity,
				RecPostDist = x.RecPostDist,
				RecState = x.RecState,
				RecCountry = x.RecCountry,
				RecBy = x.RecBy,
				RecPhone = x.RecPhone,
			}) as IEnumerable<BillLadingReport>;

			ViewData.Model = viewData.FirstOrDefault();
			return View();
		}

		// EMS
		public ActionResult Report6(string id)
		{
			Printed(id);
			var data =
				from b in db.Bill_Lading.Where(x => x.LadingNo == id && x.IsDelete == false)
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on b.HubNo equals h.HubNo into ps
				from h in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCustNo = b.SendCustNo ?? " ",
					SendCompany = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCompany ?? " " : (h.PrintLang == "en") ? b.SendECompany ?? " " : " ",
					SendCustAddr = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCustAddr ?? " " : (h.PrintLang == "en") ? b.SendECustAddr ?? " " : " ",
					SendCity = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCity ?? " " : (h.PrintLang == "en") ? b.SendECity ?? " " : " ",
					SendPostDist = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendPostDist ?? " " : (h.PrintLang == "en") ? b.SendEPostDist ?? " " : " ",
					SendState = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendState ?? " " : (h.PrintLang == "en") ? b.SendEState ?? " " : " ",
					SendCountry = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCountry ?? " " : (h.PrintLang == "en") ? b.SendECountry ?? " " : " ",
					SendBy = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendBy ?? " " : (h.PrintLang == "en") ? b.SendEBy ?? " " : " ",
					SendPhone = b.SendPhone ?? " ",
					SendInvNo = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendInvNo ?? " " : (h.PrintLang == "en") ? b.SendEInvNo ?? " " : " ",
					RecCompany = b.RecCompany ?? " ",
					RecChAddr = b.RecChAddr ?? " ",
					RecCity = b.RecCity,
					RecPostDist = b.RecPostDist,
					RecState = b.RecState,
					RecCountry = b.RecCountry,
					RecBy = b.RecBy ?? " ",
					RecPhone = b.RecPhone ?? " ",
				};
			var viewData = data.ToList().Select((x, index) => new BillLadingReport()
			{
				LadingNo = x.LadingNo,
				LadingNo_Type = x.LadingNo_Type,
				LadingDate = string.Format("{0:yyyy/MM/dd}", x.LadingDate),
				SendCustNo = x.SendCustNo,
				SendCompany = x.SendCompany,
				SendCustAddr = x.SendCustAddr,
				SendCity = x.SendCity,
				SendPostDist = x.SendPostDist,
				SendState = x.SendState,
				SendCountry = x.SendCountry,
				SendBy = x.SendBy,
				SendPhone = x.SendPhone,
				SendInvNo = x.SendInvNo,
				RecCompany = x.RecCompany,
				RecChAddr = x.RecChAddr,
				RecCity = x.RecCity,
				RecPostDist = x.RecPostDist,
				RecState = x.RecState,
				RecCountry = x.RecCountry,
				RecBy = x.RecBy,
				RecPhone = x.RecPhone,
			}) as IEnumerable<BillLadingReport>;

			ViewData.Model = viewData.FirstOrDefault();
			return View();
		}

		//BEST
		public ActionResult Report7(string id)
		{
			Printed(id);
			var SdData = db.ShdetDetail.Where(x => x.ShdetNo == id && x.IsDelete == false).OrderBy(x => x.sNo);
			var SdCount = SdData.Count();
			var SectorName = SdCount != 0 ? db.ORG_Sector.Where(s => s.SectorNo == SdData.Select(x => x.SectorNo).FirstOrDefault()).Select(x => x.SectorName).FirstOrDefault() : " ";
			var Sector = SdCount == 0 ? "" : SdCount == 1 ? SectorName : SectorName + "(多地取件)";
			var data =
				from b in db.Bill_Lading.Where(x => x.LadingNo == id && x.IsDelete == false)
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on b.HubNo equals h.HubNo into ps
				from h in ps.DefaultIfEmpty()
				select new
				{
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					SendCustNo = b.SendCustNo ?? " ",
					SendCompany = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCompany ?? " " : (h.PrintLang == "en") ? b.SendECompany ?? " " : " ",
					SendBy = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendBy ?? " " : (h.PrintLang == "en") ? b.SendEBy ?? " " : " ",
					SendInvNo = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendInvNo ?? " " : (h.PrintLang == "en") ? b.SendEInvNo ?? " " : " ",
					SendCustAddr = (h.PrintLang == null || h.PrintLang == "zh") ? b.SendCustAddr ?? " " : (h.PrintLang == "en") ? b.SendECustAddr ?? " " : " ",
					SendPhone = b.SendPhone ?? " ",
					SendFaxNo = b.SendFaxNo ?? " ",
					RecCompany = b.RecCompany ?? " ",
					RecChAddr = b.RecChAddr ?? " ",
					RecBy = b.RecBy ?? " ",
					RecInvNo = b.RecInvNo ?? " ",
					RecPhone = b.RecPhone ?? " ",
					RecMPhone = b.RecMPhone,
					SStatNo = b.SStatNo ?? " ",
					AStatNo = b.AStatNo ?? " ",
					DestNo = b.DestNo ?? " ",
					Qty = b.Qty ?? 0,
					Weight = b.Weight ?? 0,
					HubNo = h.HubNo ?? " ",
					HubName = h.HubName ?? " ",
					HubPName = h.HubPName ?? " ",
					Volume = b.Volume ?? 0,
					CcNo = b.CcNo ?? " ",
					Currency = b.Currency ?? " ",
					ToPayment = b.ToPayment ?? 0,
					Remark = b.Remark ?? " ",
					Type = b.Type ?? " ",
					ProductName = b.ProductName ?? " ",
					CocustomTyp = b.CocustomTyp == 0 ? "不報關" : b.CocustomTyp == 1 ? "正式報關" : b.CocustomTyp == 2 ? "簡易報關" : b.CocustomTyp == 3 ? "正式報關+後段核銷" : b.CocustomTyp == 4 ? "簡易報關+後段核銷" : b.CocustomTyp == 5 ? "不報關+後段核銷" : b.CocustomTyp == 6 ? "其他" : " ",
					SendRemark = b.SendRemark ?? " ",
					PiecesNo = b.PiecesNo ?? 0,
					Sale = b.Sale ?? " ",
					SDate = b.SDate,
				};
			var viewData = data.ToList().Select((x, index) => new BillLadingReport()
			{
				LadingNo = x.LadingNo,
				LadingNo_Type = x.LadingNo_Type,
				LadingDate = string.Format("{0:yyyy/MM/dd}", x.LadingDate),
				SendCustNo = x.SendCustNo,
				SendCompany = x.SendCompany,
				SendBy = x.SendBy,
				SendInvNo = x.SendInvNo,
				SendCustAddr = x.SendCustAddr,
				SendPhone = x.SendPhone,
				SendFaxNo = x.SendFaxNo,
				RecCompany = x.RecCompany,
				RecChAddr = x.RecChAddr,
				RecBy = x.RecBy,
				RecInvNo = x.RecInvNo,
				RecPhone = x.RecPhone,
				RecMPhone = x.RecMPhone,
				SStatNo = x.SStatNo,
				AStatNo = x.AStatNo,
				DestNo = x.DestNo,
				Qty = (int)x.Qty,
				Weight = x.Weight,
				HubNo = x.HubNo,
				HubName = x.HubName,
				HubPName = x.HubPName,
				Volume = x.Volume,
				CcNo = x.CcNo,
				Currency = x.Currency,
				ToPayment = x.ToPayment,
				Remark = x.Remark,
				Type = x.Type,
				ProductName = x.ProductName,
				CocustomTyp = x.CocustomTyp,
				SendRemark = x.SendRemark,
				PiecesNo = x.PiecesNo,
				Sale = x.Sale,
				SDate = string.Format("{0:yyyy/MM/dd}", x.SDate),
				Sector = Sector,
				PrintTime = DateTime.Now.ToDateTimeString(),
			}) as IEnumerable<BillLadingReport>;

			ViewData.Model = viewData.FirstOrDefault();
			return View();
		}

		public string SubStr(string a_SrcStr, int a_StartIndex, int a_Cnt)
		{
			Encoding l_Encoding = Encoding.GetEncoding("big5", new EncoderExceptionFallback(), new DecoderReplacementFallback(""));
			byte[] l_byte = l_Encoding.GetBytes(a_SrcStr);
			if (a_Cnt <= 0)
				return "";
			//例若長度10
			//若a_StartIndex傳入9 -> ok, 10 ->不行
			if (a_StartIndex + 1 > l_byte.Length)
				return "";
			else
			{
				//若a_StartIndex傳入9 , a_Cnt 傳入2 -> 不行 -> 改成 9,1
				if (a_StartIndex + a_Cnt > l_byte.Length)
					a_Cnt = l_byte.Length - a_StartIndex;
			}
			return l_Encoding.GetString(l_byte, a_StartIndex, a_Cnt);
		}
		[Authorize]
		public void Pdf(string id)
		{
			var account = User.Identity.Name;
			var ID = id.Split('|')[0];
			var type = id.Split('|')[1].Split('—')[0];
			var color = id.Split('—')[1];
			var ladingNo_Type = db.Bill_Lading.Where(x => x.LadingNo == ID && x.IsDelete == false).Select(x => x.LadingNo_Type).FirstOrDefault();
			string fileNameWithOutExtention = Guid.NewGuid().ToString();
			var eee = Request.Url.AbsoluteUri;
			//string reportUrl = type == "1" ? Request.Url.AbsoluteUri.Replace("Pdf/", "Report/").Replace("%7C1", "").Replace("%E2%80%940", "") : type == "2" ? Request.Url.AbsoluteUri.Replace("Pdf/", "Report2/").Replace("%7C2", "").Replace("%E2%80%940", "") : Request.Url.AbsoluteUri.Replace("Pdf/", "Report3?id=").Replace("%7C3", "").Replace("%E2%80%94", "%7C") + "&account=" + account;
			string reportUrl = "";
			switch (type)
			{
				case "1":
					reportUrl = Request.Url.AbsoluteUri.Replace("Pdf/", "Report/").Replace("%7C1", "").Replace("%E2%80%940", "");
					break;
				case "2":
					reportUrl = Request.Url.AbsoluteUri.Replace("Pdf/", "Report2/").Replace("%7C2", "").Replace("%E2%80%940", "");
					break;
				case "3":
					reportUrl = Request.Url.AbsoluteUri.Replace("Pdf/", "Report3?id=").Replace("%7C3", "").Replace("%E2%80%94", "%7C") + "&account=" + account;
					break;
				case "7":
					reportUrl = Request.Url.AbsoluteUri.Replace("Pdf/", "Report7/").Replace("%7C7", "").Replace("%E2%80%940", "");
					break;
				default:
					break;
			}
			//string reportUrl = "http://exp.pml-intl.com//New_Bill_Lading/Report/" + id;

			string filePath = FileTablePath + fileNameWithOutExtention + ".pdf";

			//執行wkhtmltopdf.exe
			if (type == "1")
			{
				Process p = System.Diagnostics.Process.Start(
					@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe",
					@"-T 0 -R 0 -B 0 -L 0 --page-width 78 --page-height 182 --minimum-font-size 11  --disable-smart-shrinking" +
					" " + reportUrl +
					" " + filePath
					);
				p.WaitForExit();
			}
			else if (type == "2")
			{
				Process p = System.Diagnostics.Process.Start(
					@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe",
					@"-T 0 -R 0 -B 0 -L 0 --page-width 154 --page-height 182 --minimum-font-size 11  --disable-smart-shrinking" +
					" " + reportUrl +
					" " + filePath
					);
				p.WaitForExit();
			}
			else if (type == "3" || type == "7")
			{
				Process p = System.Diagnostics.Process.Start(
					@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe",
					@"-T 3 -R 0 -B 0 -L 0 --page-width 209 --page-height 148 --minimum-font-size 11  --disable-smart-shrinking" +
					" " + reportUrl +
					" " + filePath
					);
				p.WaitForExit();
			}



			//把檔案讀進串流
			FileStream fs = new FileStream(filePath, FileMode.Open);
			byte[] file = new byte[fs.Length];
			fs.Read(file, 0, file.Length);
			fs.Close();

			System.IO.File.Delete(filePath);

			//Response給用戶端下載
			Response.Clear();
			Response.AddHeader("content-disposition", "attachment; filename=" + ladingNo_Type + ".pdf");//強制下載
			Response.ContentType = "application/octet-stream";
			Response.BinaryWrite(file);
			Response.End();
		}

		public void Printed(string id)
		{
			var blData = db.Bill_Lading.Where(x => x.LadingNo == id).FirstOrDefault();
			if (blData.Printed == null || blData.Printed == false)
			{
				blData.Printed = true;
				db.Entry(blData).State = EntityState.Modified;
				db.SaveChanges();
			}
		}
		#endregion

		#region 財務資訊
		[Authorize]
		public ActionResult GetTB_temp_Acc1GridJSON(string PTNO = "", string WATERNO = "", string CcNo = "", int page = 1, int rows = 40)
		{
			//var insertResult = "";
			SqlConnection dataConnection = new SqlConnection();
			String sqlConnectionStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ACB2010"].ConnectionString;
			string sqlstr = "SELECT {0} FROM TB_temp_Acc1 A1 JOIN TB_temp_Accno An ON An.PTNO = A1.PTNO WHERE 1 = 1";
			List<SqlParameter> data = new List<SqlParameter>();
			int totalRecord = 0;

			if (!string.IsNullOrEmpty(PTNO))
			{
				sqlstr += " AND A1.PTNO = @PTNO";
				data.Add(new SqlParameter("@PTNO", PTNO));
			}
			if (!string.IsNullOrEmpty(WATERNO))
			{
				sqlstr += " AND A1.WATERNO = @WATERNO";
				data.Add(new SqlParameter("@WATERNO", WATERNO));
			}
			if (!string.IsNullOrEmpty(CcNo.Trim()))
			{
				sqlstr += " AND A1.CcNo = @CcNo";
				data.Add(new SqlParameter("@CcNo", CcNo.Trim()));
			}

			DataTable dtAcc = new DataTable();
			SqlDataReader sqlReader;


			SqlCommand SqlCmd = new SqlCommand(sqlstr, dataConnection);
			try
			{
				dataConnection.ConnectionString = sqlConnectionStr;

				dataConnection.Open();

				SqlCmd.CommandText = string.Format(sqlstr, " COUNT(*) ");

				SqlCmd.Parameters.AddRange(data.ToArray<SqlParameter>());

				totalRecord = (int)SqlCmd.ExecuteScalar();

				SqlCmd.CommandText = string.Format(sqlstr + " ORDER BY  WATERNO OFFSET " + ((page - 1) * rows).ToString() + " ROWS FETCH NEXT " + rows.ToString() + " ROWS ONLY", "  A1.*,An.OKED ");

				sqlReader = SqlCmd.ExecuteReader();

				dtAcc.Load(sqlReader);

				dataConnection.Close();

				//insertResult = "OK";

			}
			catch (Exception e)
			{
				//insertResult = e.Message;
			}
			finally
			{
				SqlCmd.Cancel();
				dataConnection.Close();
				dataConnection.Dispose();
			}

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = dtAcc,
				Records = totalRecord,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (totalRecord - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		#endregion

		#region Barcode
		[AllowAnonymous]
		public void Barcode(string barCode)
		{
			Response.ContentType = "image/gif";
			Barcode bc = new Barcode();
			bc.IncludeLabel = true;//顯示文字標籤
			bc.LabelFont = new Font("Verdana", 16,FontStyle.Bold);//文字標籤字型、大小
			bc.Width = 180;//寬度
			bc.Height = 100;//高度
			Image img = bc.Encode(TYPE.CODE128, $"{barCode}", bc.Width, bc.Height);//產生影像
			img.Save(Response.OutputStream, ImageFormat.Gif);
			Response.End();
		}
		#endregion
	}
}
