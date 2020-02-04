using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class CustController : Controller
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
			var statNo = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
			//主表
			ViewBag.Title = "客戶基本資料";
			ViewBag.ControllerName = "Cust";
			ViewBag.AddFunc = "AddCust";
			ViewBag.EditFunc = "EditCust";
			ViewBag.DelFunc = "DeleteCust";
			ViewBag.FormPartialName = "_ElementInForm";
			ViewBag.dlgWidth = "880px";
			ViewBag.FormCustomJsNew = $"$('#IsServer').switchbutton('check');$('#IsMas').switchbutton('check');$('#StatNo').textbox('setValue', '{statNo}');";

			//子表
			ViewBag.Title2 = "客戶其他資料";
			ViewBag.ControllerName2 = "Cust";
			ViewBag.AddFunc2 = "AddCustDetail";
			ViewBag.EditFunc2 = "EditCustDetail";
			ViewBag.DelFunc2 = "DeleteCustDetail";
			ViewBag.FormPartialName2 = "_ElementInForm2";
			ViewBag.FormCustomJsNew2 = $"$('#sNo').text('');$('#CustNo').val(row.CustNo);$('#IsServer_dtl').switchbutton('check');$('#StatNo_dtl').textbox('setValue', '{statNo}');";
			ViewBag.FormCustomJsEdit2 = $"$('#sNo').text(row.sNo);RedyDayOfWeekCheck();$('#StatNo_dtl').textbox('setValue', '{statNo}');";

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
		public ActionResult getSelectionjqGrid2(string formId, string inputFields, string inputValues)
		{
			ViewBag.formId = formId;
			ViewBag.inputValues = inputValues.Split(',');
			ViewBag.inputFields = inputFields.Split(',');
			return View();
		}

		[Authorize]
		public ActionResult AddCust(ORG_Cust data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			if (data.Add_1 == null && data.Add_2 == null && data.Add_3 == null && data.Add_4 == null && data.Add_5 == null && data.Add_6 == null)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "段巷弄號樓其他，至少輸入一欄位";
				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			}
			if (data.InvNo.IsNotEmpty() || data.IDNo.IsNotEmpty())
			{
				if (data.InvNo.IsNotEmpty() && data.InvNo.Length > 8)
				{

					result.Ok = DataModifyResultType.Faild;
					result.Message = "統編長度最大為8碼";
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}
				if (data.InvNo.IsNotEmpty())
				{
					var invNoDuplicated = db.ORG_Cust.Any(x => x.InvNo == data.InvNo && x.IsDelete == false);
					if (invNoDuplicated)
					{
						var target = from c in db.ORG_Cust
									 join s in db.ORG_Stat
									 on c.StatID equals s.ID
									 where c.InvNo == data.InvNo
									 select s.StatName;

						result.Ok = DataModifyResultType.Faild;
						result.Message = $"此統一編號已由{target.FirstOrDefault()}站點建立，如欲使用，請由\"{target.FirstOrDefault()}\"開啟共用功能!";
						return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
					}
				}

				if (data.IDNo.IsNotEmpty() && data.IDNo.Length > 10)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "身分證字號長度最大為10碼";
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}
				if (data.IDNo.IsNotEmpty())
				{
					var iDNoDuplicated = db.ORG_Cust.Any(x => x.IDNo == data.IDNo && x.IsDelete == false);
					if (iDNoDuplicated)
					{
						var target = from c in db.ORG_Cust
									 join s in db.ORG_Stat
									 on c.StatID equals s.ID
									 where c.IDNo == data.IDNo
									 select s.StatName;

						result.Ok = DataModifyResultType.Faild;
						result.Message = $"此身分證字號已由{target.FirstOrDefault()}站點建立，如欲使用，請由\"{target.FirstOrDefault()}\"開啟共用功能!";
						return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
					}
				}
			}
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "統一編號或身分證字號，至少輸入一項。";
				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			}

			var statID = db.ORG_Stat.FirstOrDefault(x => x.IsDelete == false && x.StatNo == data.StatNo).ID;

			using (var trans = db.Database.BeginTransaction())
			{
				List<string> RedyDayOfWeekList = new List<string>();
				for (int i = 0; i <= 6; i++)
				{
					if (Request["RedyDayOfWeek_" + i.ToString()] == "true")
					{
						RedyDayOfWeekList.Add(i.ToString());
					}
				}
				//Add Cust
				var userRecord = new ORG_Cust();
				userRecord.CustNo = "__" + String.Format("{0:yyyyMMdd}", DateTime.Now); ;
				userRecord.CustLevel = data.CustLevel;
				userRecord.CustCHName = data.CustCHName;
				userRecord.CustEName1 = data.CustEName1;
				userRecord.CustEName2 = data.CustEName2;
				userRecord.CustCName = data.CustCName;
				userRecord.CustEName = data.CustEName;
				userRecord.CarryName = data.CarryName;
				userRecord.Code5 = data.Code5;
				userRecord.Code7 = data.Code7;
				userRecord.Add_1 = data.Add_1;
				userRecord.Add_2 = data.Add_2;
				userRecord.Add_3 = data.Add_3;
				userRecord.Add_4 = data.Add_4;
				userRecord.Add_5 = data.Add_5;
				userRecord.Add_6 = data.Add_6;
				userRecord.CustAddr = data.CustAddr;
				userRecord.CustENAddr1 = data.CustENAddr1;
				userRecord.CustENAddr2 = data.CustENAddr2;
				userRecord.SendBy = data.SendBy;
				userRecord.InvNo = data.InvNo;
				userRecord.City = data.City;
				userRecord.Country = data.Country;
				userRecord.State = data.State;
				userRecord.PostDist = data.PostDist;
				userRecord.ESendBy = data.ESendBy;
				userRecord.EInvNo = data.EInvNo;
				userRecord.ECity = data.ECity;
				userRecord.ECountry = data.ECountry;
				userRecord.EState = data.EState;
				userRecord.EPostDist = data.EPostDist;
				userRecord.IDNo = data.IDNo;
				userRecord.Email = data.Email;
				userRecord.Phone = data.Phone;
				userRecord.FaxNo = data.FaxNo;
				userRecord.CtcAcc = data.CtcAcc;
				userRecord.AccPhone = data.AccPhone;
				userRecord.CtcSale = data.CtcSale;
				userRecord.CtcSale2 = data.CtcSale2;
				userRecord.CtcSale3 = data.CtcSale3;
				userRecord.Account = data.Account;
				userRecord.PayTerm = data.PayTerm;
				userRecord.CcID = data.CcID;
				userRecord.PayDate = data.PayDate;
				if (data.SignDate != null)
					userRecord.SignDate = data.SignDate;
				else
					userRecord.SignDate = DateTime.Now;
				userRecord.SuspDate = data.SuspDate;
				userRecord.Discnt = data.Discnt;
				userRecord.DiscntOut = data.DiscntOut;
				userRecord.QuoType = data.QuoType;
				userRecord.Balance = data.Balance;
				userRecord.IsinVoice = data.IsinVoice;
				userRecord.InvTitle = data.InvTitle;
				userRecord.InvAddr = data.InvAddr;
				userRecord.StatID = statID;
				// userRecord.StatID = data.StatID;
				userRecord.IsAgent = data.IsAgent;
				userRecord.IsCommon = data.IsCommon;
				userRecord.IsServer = true;
				userRecord.IsMas = true;
				userRecord.PickUpAreaID = data.PickUpAreaID;
				userRecord.Remark5 = data.Remark5;
				userRecord.SectorNo = data.SectorNo;
				userRecord.Remark = data.Remark;
				userRecord.DayOfWeek = data.DayOfWeek;
				userRecord.RedyDayWeekly = string.Join(",", RedyDayOfWeekList.ToArray());
				userRecord.RedyTime = data.RedyTime;
				userRecord.CustLevel = data.CustLevel;
				userRecord.Code5_C = data.Code5_C;
				userRecord.Code7_C = data.Code7_C;
				userRecord.Add_1_C = data.Add_1_C;
				userRecord.Add_2_C = data.Add_2_C;
				userRecord.Add_3_C = data.Add_3_C;
				userRecord.Add_4_C = data.Add_4_C;
				userRecord.Add_5_C = data.Add_5_C;
				userRecord.Add_6_C = data.Add_6_C;
				userRecord.CustAddr_C = data.CustAddr_C;
				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;
				try
				{
					db.ORG_Cust.Add(userRecord);
					db.SaveChanges();
					var target = db.ORG_Cust.OrderByDescending(x => x.ID).FirstOrDefault(x => x.CustNo == userRecord.CustNo);
					target.CustNo = (data.Code5).Substring(0, 3) + target.ID.ToString().PadLeft(7, '0');
					userRecord.CustNo = target.CustNo;
					db.Entry(target).State = EntityState.Modified;
					db.SaveChanges();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
				}
				catch (Exception e)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = (e as SqlException).Message;
					trans.Rollback();
				}

				var custDetail = new ORG_CustDetail();
				var maxSeqNum = db.ORG_CustDetail.Where(x => x.CustNo == userRecord.CustNo).OrderByDescending(x => x.sNo).Select(x => x.sNo).FirstOrDefault();

				maxSeqNum += 1;
				custDetail.sNo = maxSeqNum;
				custDetail.CustNo = userRecord.CustNo;
				custDetail.CustLevel = data.CustLevel;
				custDetail.CustCHName = data.CustCHName;
				custDetail.CustEName1 = data.CustEName1;
				custDetail.CustEName2 = data.CustEName2;
				custDetail.CustCName = data.CustCName;
				custDetail.CustEName = data.CustEName;
				custDetail.CarryName = data.CarryName;
				custDetail.Code5 = data.Code5;
				custDetail.Code7 = data.Code7;
				custDetail.Add_1 = data.Add_1;
				custDetail.Add_2 = data.Add_2;
				custDetail.Add_3 = data.Add_3;
				custDetail.Add_4 = data.Add_4;
				custDetail.Add_5 = data.Add_5;
				custDetail.Add_6 = data.Add_6;
				custDetail.CustAddr = data.CustAddr;
				custDetail.CustENAddr1 = data.CustENAddr1;
				custDetail.CustENAddr2 = data.CustENAddr2;
				custDetail.SendBy = data.SendBy;
				custDetail.InvNo = data.InvNo;
				custDetail.City = data.City;
				custDetail.Country = data.Country;
				custDetail.State = data.State;
				custDetail.PostDist = data.PostDist;
				custDetail.ESendBy = data.ESendBy;
				custDetail.EInvNo = data.EInvNo;
				custDetail.ECity = data.ECity;
				custDetail.ECountry = data.ECountry;
				custDetail.EState = data.EState;
				custDetail.EPostDist = data.EPostDist;
				custDetail.IDNo = data.IDNo;
				custDetail.Email = data.Email;
				custDetail.Phone = data.Phone;
				custDetail.FaxNo = data.FaxNo;
				custDetail.CtcAcc = data.CtcAcc;
				custDetail.AccPhone = data.AccPhone;
				custDetail.CtcSale = data.CtcSale;
				custDetail.CtcSale2 = data.CtcSale2;
				custDetail.CtcSale3 = data.CtcSale3;
				custDetail.Account = data.Account;
				custDetail.PayTerm = data.PayTerm;
				custDetail.CcID = data.CcID;
				custDetail.PayDate = data.PayDate;
				if (data.SignDate != null)
					custDetail.SignDate = data.SignDate;
				else
					custDetail.SignDate = DateTime.Now;
				custDetail.SuspDate = data.SuspDate;
				custDetail.Discnt = data.Discnt;
				custDetail.DiscntOut = data.DiscntOut;
				custDetail.QuoType = data.QuoType;
				custDetail.Balance = data.Balance;
				custDetail.IsinVoice = data.IsinVoice;
				custDetail.InvTitle = data.InvTitle;
				custDetail.InvAddr = data.InvAddr;
				custDetail.StatID = statID;
				// custDetail.StatID = data.StatID;
				custDetail.IsAgent = data.IsAgent;
				custDetail.IsCommon = data.IsCommon;
				custDetail.IsServer = true;
				custDetail.IsMas = true;
				custDetail.PickUpAreaID = data.PickUpAreaID;
				custDetail.Remark5 = data.Remark5;
				custDetail.SectorNo = data.SectorNo;
				custDetail.Remark = data.Remark;
				custDetail.DayOfWeek = data.DayOfWeek;
				custDetail.RedyDayWeekly = string.Join(",", RedyDayOfWeekList.ToArray());
				custDetail.RedyTime = data.RedyTime;
				custDetail.CustLevel = data.CustLevel;
				custDetail.Code5_C = data.Code5_C;
				custDetail.Code7_C = data.Code7_C;
				custDetail.Add_1_C = data.Add_1_C;
				custDetail.Add_2_C = data.Add_2_C;
				custDetail.Add_3_C = data.Add_3_C;
				custDetail.Add_4_C = data.Add_4_C;
				custDetail.Add_5_C = data.Add_5_C;
				custDetail.Add_6_C = data.Add_6_C;
				custDetail.CustAddr_C = data.CustAddr_C;
				//以下系統自填
				custDetail.CreatedDate = DateTime.Now;
				custDetail.CreatedBy = User.Identity.Name;
				custDetail.IsDelete = false;

				try
				{
					db.ORG_CustDetail.Add(custDetail);
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
				}
				catch (Exception e)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = (e as SqlException).Message;
					trans.Rollback();
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		[Authorize]
		public ActionResult AddCustDetail(ORG_CustDetail data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			if (data.Add_1 == null && data.Add_2 == null && data.Add_3 == null && data.Add_4 == null && data.Add_5 == null && data.Add_6 == null)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "段巷弄號樓其他，至少輸入一欄位";
				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			}
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = new ORG_CustDetail();
				var maxSeqNum = db.ORG_CustDetail.Where(x => x.CustNo == data.CustNo).OrderByDescending(x => x.sNo).Select(x => x.sNo).FirstOrDefault();
				var statID = db.ORG_Stat.FirstOrDefault(x => x.IsDelete == false && x.StatNo == data.StatNo).ID;
				maxSeqNum += 1;
				userRecord.sNo = maxSeqNum;
				userRecord.CustNo = data.CustNo;
				userRecord.CustLevel = data.CustLevel;
				userRecord.CustCHName = data.CustCHName;
				userRecord.CustEName1 = data.CustEName1;
				userRecord.CustEName2 = data.CustEName2;
				userRecord.CustCName = data.CustCName;
				userRecord.CustEName = data.CustEName;
				userRecord.CarryName = data.CarryName;
				userRecord.Code5 = data.Code5;
				userRecord.Code7 = data.Code7;
				userRecord.Add_1 = data.Add_1;
				userRecord.Add_2 = data.Add_2;
				userRecord.Add_3 = data.Add_3;
				userRecord.Add_4 = data.Add_4;
				userRecord.Add_5 = data.Add_5;
				userRecord.Add_6 = data.Add_6;
				userRecord.CustAddr = data.CustAddr;
				userRecord.CustENAddr1 = data.CustENAddr1;
				userRecord.CustENAddr2 = data.CustENAddr2;
				userRecord.SendBy = data.SendBy;
				userRecord.InvNo = data.InvNo;
				userRecord.City = data.City;
				userRecord.Country = data.Country;
				userRecord.State = data.State;
				userRecord.PostDist = data.PostDist;
				userRecord.ESendBy = data.ESendBy;
				userRecord.EInvNo = data.EInvNo;
				userRecord.ECity = data.ECity;
				userRecord.ECountry = data.ECountry;
				userRecord.EState = data.EState;
				userRecord.EPostDist = data.EPostDist;
				userRecord.IDNo = data.IDNo;
				userRecord.Email = data.Email;
				userRecord.Phone = data.Phone;
				userRecord.FaxNo = data.FaxNo;
				userRecord.CtcAcc = data.CtcAcc;
				userRecord.AccPhone = data.AccPhone;
				userRecord.CtcSale = data.CtcSale;
				userRecord.CtcSale2 = data.CtcSale2;
				userRecord.CtcSale3 = data.CtcSale3;
				userRecord.Account = data.Account;
				userRecord.PayTerm = data.PayTerm;
				userRecord.CcID = data.CcID;
				userRecord.PayDate = data.PayDate;
				if (data.SignDate != null)
					userRecord.SignDate = data.SignDate;
				else
					userRecord.SignDate = DateTime.Now;
				userRecord.SuspDate = data.SuspDate;
				userRecord.Discnt = data.Discnt;
				userRecord.DiscntOut = data.DiscntOut;
				userRecord.QuoType = data.QuoType;
				userRecord.Balance = data.Balance;
				userRecord.IsinVoice = data.IsinVoice;
				userRecord.InvTitle = data.InvTitle;
				userRecord.InvAddr = data.InvAddr;
				userRecord.StatID = statID;
				// userRecord.StatID = data.StatID;
				userRecord.IsAgent = data.IsAgent;
				userRecord.IsCommon = data.IsCommon;
				userRecord.IsServer = data.IsServer;
				userRecord.IsMas = data.IsMas;
				if (data.IsMas == true)
				{
					var custDtlData = db.ORG_CustDetail.Where(x => x.CustNo == data.CustNo && x.IsMas == true).FirstOrDefault();
					custDtlData.IsMas = false;
					db.Entry(custDtlData).State = EntityState.Modified;
				}
				userRecord.PickUpAreaID = data.PickUpAreaID;
				userRecord.Remark5 = data.Remark5;
				userRecord.SectorNo = data.SectorNo;
				userRecord.Remark = data.Remark;
				userRecord.DayOfWeek = data.DayOfWeek;
				List<string> RedyDayOfWeekList = new List<string>();
				for (int i = 0; i <= 6; i++)
				{
					if (Request["RedyDayOfWeek_" + i.ToString()] == "true")
					{
						RedyDayOfWeekList.Add(i.ToString());
					}
				}
				userRecord.RedyDayWeekly = string.Join(",", RedyDayOfWeekList.ToArray());
				userRecord.RedyTime = data.RedyTime;
				userRecord.CustLevel = data.CustLevel;
				userRecord.Code5_C = data.Code5_C;
				userRecord.Code7_C = data.Code7_C;
				userRecord.Add_1_C = data.Add_1_C;
				userRecord.Add_2_C = data.Add_2_C;
				userRecord.Add_3_C = data.Add_3_C;
				userRecord.Add_4_C = data.Add_4_C;
				userRecord.Add_5_C = data.Add_5_C;
				userRecord.Add_6_C = data.Add_6_C;
				userRecord.CustAddr_C = data.CustAddr_C;
				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;
				db.ORG_CustDetail.Add(userRecord);
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

		//[Authorize]
		//public ActionResult EditCust(ORG_Cust data)
		//{
		//	//權限控管
		//	if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
		//		return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

		//	var result = new ResultHelper();
		//	if (data.Add_1 == null && data.Add_2 == null && data.Add_3 == null && data.Add_4 == null && data.Add_5 == null && data.Add_6 == null)
		//	{
		//		result.Ok = DataModifyResultType.Faild;
		//		result.Message = "段巷弄號樓其他，至少輸入一欄位";
		//		return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		//	}
		//	if (data.InvNo.IsNotEmpty() || data.IDNo.IsNotEmpty())
		//	{
		//		if (data.InvNo.IsNotEmpty() && data.InvNo.Length > 8)
		//		{

		//			result.Ok = DataModifyResultType.Faild;
		//			result.Message = "統編長度最大為8碼";
		//			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		//		}
		//		if (data.InvNo.IsNotEmpty())
		//		{
		//			var invNoDuplicated = db.ORG_Cust.Any(x => x.ID != data.ID && x.InvNo == data.InvNo && x.IsDelete == false);
		//			if (invNoDuplicated)
		//			{
		//				var target = from c in db.ORG_Cust
		//							 join s in db.ORG_Stat
		//							 on c.StatID equals s.ID
		//							 where c.InvNo == data.InvNo
		//							 select s.StatName;

		//				result.Ok = DataModifyResultType.Faild;
		//				result.Message = $"此統一編號已由{target.FirstOrDefault()}站點建立，如欲使用，請開啟共用功能!";
		//				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		//			}
		//		}

		//		if (data.IDNo.IsNotEmpty() && data.IDNo.Length > 10)
		//		{
		//			result.Ok = DataModifyResultType.Faild;
		//			result.Message = "身分證字號長度最大為10碼";
		//			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		//		}
		//		if (data.IDNo.IsNotEmpty())
		//		{
		//			var iDNoDuplicated = db.ORG_Cust.Any(x => x.ID != data.ID && x.IDNo == data.IDNo && x.IsDelete == false);
		//			if (iDNoDuplicated)
		//			{
		//				var target = from c in db.ORG_Cust
		//							 join s in db.ORG_Stat
		//							 on c.StatID equals s.ID
		//							 where c.IDNo == data.IDNo
		//							 select s.StatName;

		//				result.Ok = DataModifyResultType.Faild;
		//				result.Message = $"此身分證字號已由{target.FirstOrDefault()}站點建立，如欲使用，請開啟共用功能!";
		//				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		//			}
		//		}
		//	}
		//	else
		//	{
		//		result.Ok = DataModifyResultType.Faild;
		//		result.Message = "統一編號或身分證字號，至少輸入一項。";
		//		return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		//	}
		//	var userRecord = db.ORG_Cust.FirstOrDefault(x => x.ID == data.ID);

		//	if (userRecord != null)
		//	{
		//		userRecord.CustCName = data.CustCName;
		//		userRecord.CustEName = data.CustEName;
		//		userRecord.CustCHName = data.CustCHName;
		//		userRecord.CustEName1 = data.CustEName1;
		//		userRecord.CustEName2 = data.CustEName2;
		//		userRecord.CustAddr = data.CustAddr;
		//		userRecord.CustENAddr1 = data.CustENAddr1;
		//		userRecord.CustENAddr2 = data.CustENAddr2;
		//		userRecord.State = data.State;
		//		userRecord.Country = data.Country;
		//		userRecord.Email = data.Email;
		//		userRecord.PostDist = data.PostDist;
		//		userRecord.Phone = data.Phone;
		//		userRecord.FaxNo = data.FaxNo;
		//		userRecord.CtcAcc = data.CtcAcc;
		//		userRecord.CtcSale = data.CtcSale;
		//		userRecord.CtcSale2 = data.CtcSale2;
		//		userRecord.CtcSale3 = data.CtcSale3;
		//		userRecord.PayTerm = data.PayTerm;
		//		userRecord.Add_1 = data.Add_1;
		//		userRecord.Add_2 = data.Add_2;
		//		userRecord.Add_3 = data.Add_3;
		//		userRecord.Add_4 = data.Add_4;
		//		userRecord.Add_5 = data.Add_5;
		//		userRecord.Add_6 = data.Add_6;
		//		userRecord.CcID = data.CcID;
		//		userRecord.SuspDate = data.SuspDate;
		//		userRecord.Discnt = data.Discnt;
		//		userRecord.Account = data.Account;
		//		userRecord.InvNo = data.InvNo;
		//		userRecord.IDNo = data.IDNo;
		//		userRecord.InvTitle = data.InvTitle;
		//		userRecord.InvAddr = data.InvAddr;
		//		userRecord.PayDate = data.PayDate;
		//		userRecord.Remark = data.Remark;
		//		userRecord.QuoType = data.QuoType;
		//		userRecord.Balance = data.Balance;
		//		userRecord.DayOfWeek = data.DayOfWeek;
		//		userRecord.RedyTime = data.RedyTime;
		//		userRecord.IsAgent = data.IsAgent;
		//		userRecord.StatID = data.StatID;
		//		userRecord.DiscntOut = data.DiscntOut;
		//		userRecord.IsinVoice = data.IsinVoice;
		//		userRecord.PickUpAreaID = data.PickUpAreaID;
		//		userRecord.Remark5 = data.Remark5;
		//		userRecord.AccPhone = data.AccPhone;
		//		userRecord.SectorNo = data.SectorNo;
		//		if (data.SignDate != null)
		//			userRecord.SignDate = data.SignDate;
		//		else
		//			userRecord.SignDate = DateTime.Now;
		//		userRecord.IsServer = data.IsServer;
		//		userRecord.IsCommon = data.IsCommon;
		//		userRecord.CustLevel = data.CustLevel;

		//		//以下系統自填
		//		userRecord.UpdatedDate = DateTime.Now;
		//		userRecord.UpdatedBy = User.Identity.Name;
		//		try
		//		{
		//			db.Entry(userRecord).State = EntityState.Modified;
		//			db.SaveChanges();
		//			result.Ok = DataModifyResultType.Success;
		//			result.Message = "OK";

		//		}
		//		catch (Exception e)
		//		{
		//			result.Ok = DataModifyResultType.Faild;
		//			result.Message = e.Message;
		//		}
		//	}
		//	else
		//	{
		//		result.Ok = DataModifyResultType.Faild;
		//		result.Message = "找不到資料!";
		//	}
		//	return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		//}

		[Authorize]
		public ActionResult EditCustDetail(ORG_CustDetail data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();
			if (data.Add_1 == null && data.Add_2 == null && data.Add_3 == null && data.Add_4 == null && data.Add_5 == null && data.Add_6 == null)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "段巷弄號樓其他，至少輸入一欄位";
				return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			}
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.ORG_CustDetail.FirstOrDefault(x => x.sNo == data.sNo && x.CustNo == data.CustNo);

				if (userRecord != null)
				{
					userRecord.CustLevel = data.CustLevel;
					userRecord.CustCHName = data.CustCHName;
					userRecord.CustEName1 = data.CustEName1;
					userRecord.CustEName2 = data.CustEName2;
					userRecord.CustCName = data.CustCName;
					userRecord.CustEName = data.CustEName;
					userRecord.CarryName = data.CarryName;
					userRecord.Code5 = data.Code5;
					userRecord.Code7 = data.Code7;
					userRecord.Add_1 = data.Add_1;
					userRecord.Add_2 = data.Add_2;
					userRecord.Add_3 = data.Add_3;
					userRecord.Add_4 = data.Add_4;
					userRecord.Add_5 = data.Add_5;
					userRecord.Add_6 = data.Add_6;
					userRecord.CustAddr = data.CustAddr;
					userRecord.CustENAddr1 = data.CustENAddr1;
					userRecord.CustENAddr2 = data.CustENAddr2;
					userRecord.SendBy = data.SendBy;
					userRecord.InvNo = data.InvNo;
					userRecord.City = data.City;
					userRecord.Country = data.Country;
					userRecord.State = data.State;
					userRecord.PostDist = data.PostDist;
					userRecord.ESendBy = data.ESendBy;
					userRecord.EInvNo = data.EInvNo;
					userRecord.ECity = data.ECity;
					userRecord.ECountry = data.ECountry;
					userRecord.EState = data.EState;
					userRecord.EPostDist = data.EPostDist;
					userRecord.IDNo = data.IDNo;
					userRecord.Email = data.Email;
					userRecord.Phone = data.Phone;
					userRecord.FaxNo = data.FaxNo;
					userRecord.CtcAcc = data.CtcAcc;
					userRecord.AccPhone = data.AccPhone;
					userRecord.CtcSale = data.CtcSale;
					userRecord.CtcSale2 = data.CtcSale2;
					userRecord.CtcSale3 = data.CtcSale3;
					userRecord.Account = data.Account;
					userRecord.PayTerm = data.PayTerm;
					userRecord.CcID = data.CcID;
					userRecord.PayDate = data.PayDate;
					userRecord.SignDate = data.SignDate;
					userRecord.SuspDate = data.SuspDate;
					userRecord.Discnt = data.Discnt;
					userRecord.DiscntOut = data.DiscntOut;
					userRecord.QuoType = data.QuoType;
					userRecord.Balance = data.Balance;
					userRecord.IsinVoice = data.IsinVoice;
					userRecord.InvTitle = data.InvTitle;
					userRecord.InvAddr = data.InvAddr;
					userRecord.IsAgent = data.IsAgent;
					userRecord.IsCommon = data.IsCommon;
					userRecord.IsServer = data.IsServer;
					if ((userRecord.IsMas == false || userRecord.IsMas == null) && data.IsMas == true)
					{
						var custDtlData = db.ORG_CustDetail.Where(x => x.CustNo == data.CustNo && x.IsMas == true).FirstOrDefault();
						custDtlData.IsMas = false;
						db.Entry(custDtlData).State = EntityState.Modified;
					}
					userRecord.IsMas = data.IsMas;
					userRecord.PickUpAreaID = data.PickUpAreaID;
					userRecord.Remark5 = data.Remark5;
					userRecord.SectorNo = data.SectorNo;
					userRecord.Remark = data.Remark;
					userRecord.DayOfWeek = data.DayOfWeek;
					List<string> RedyDayOfWeekList = new List<string>();
					for (int i = 0; i <= 6; i++)
					{
						if (Request["RedyDayOfWeek_" + i.ToString()] == "true")
						{
							RedyDayOfWeekList.Add(i.ToString());
						}
					}
					userRecord.RedyDayWeekly = string.Join(",", RedyDayOfWeekList.ToArray());
					userRecord.RedyTime = data.RedyTime;
					userRecord.CustLevel = data.CustLevel;
					userRecord.Code5_C = data.Code5_C;
					userRecord.Code7_C = data.Code7_C;
					userRecord.Add_1_C = data.Add_1_C;
					userRecord.Add_2_C = data.Add_2_C;
					userRecord.Add_3_C = data.Add_3_C;
					userRecord.Add_4_C = data.Add_4_C;
					userRecord.Add_5_C = data.Add_5_C;
					userRecord.Add_6_C = data.Add_6_C;
					userRecord.CustAddr_C = data.CustAddr_C;
					//以下系統自填
					userRecord.UpdatedDate = DateTime.Now;
					userRecord.UpdatedBy = User.Identity.Name;
					db.Entry(userRecord).State = EntityState.Modified;
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
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult DeleteCust(ORG_Cust data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Cust.FirstOrDefault(x => x.ID == data.ID);

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
		public ActionResult DeleteCustDetail(ORG_CustDetail data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_CustDetail.FirstOrDefault(x => x.CustNo == data.CustNo && x.sNo == data.sNo);
			if (userRecord != null && userRecord.IsMas == true)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "為主資料不得刪除，請先變更主資料!";
			}
			else
			{
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
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetGridJSON(ORG_Cust data, int page = 1, int rows = 40)
		{
			//20180717新增，同站點人員 才可看到資料
			List<string> allowStatNoList = new List<string>();
			var statNo = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
			if ((statNo != "hyAdmin") && (statNo != "TNNCON"))
			{
				if (statNo != "" && statNo != null)
					allowStatNoList.Add(statNo);
			}
			else
			{
				var allStatNo = db.ORG_Stat.Where(x => x.IsDelete == false);
				foreach (var itemStat in allStatNo)
					allowStatNoList.Add(itemStat.StatNo);
				allowStatNoList.Add("");
				allowStatNoList.Add(null);
			}

			var cust =
				from c in db.ORG_Cust.Where(x => x.IsDelete == false)
				join cd in db.ORG_CustDetail.Where(x => x.IsMas == true) on c.CustNo equals cd.CustNo into ps
				from cd in ps.DefaultIfEmpty()
				join stat in db.ORG_Stat.Where(x => x.IsDelete == false) on cd.StatID equals stat.ID into ps2
				from stat in ps2.DefaultIfEmpty()
				join area in db.ORG_PickUpArea.Where(x => x.IsDelete == false) on cd.PickUpAreaID equals area.ID into ps3
				from area in ps3.DefaultIfEmpty()
				join sect in db.ORG_Sector.Where(x => x.IsDelete == false) on cd.SectorNo equals sect.SectorNo into ps4
				from sect in ps4.DefaultIfEmpty()
				join Cc in db.ORG_Cc.Where(x => x.IsDelete == false) on cd.CcID equals Cc.ID into ps5
				from Cc in ps5.DefaultIfEmpty()
				join u in db.SYS_User.Where(x => x.IsDelete == false) on cd.CreatedBy equals u.Account into ps6
				from u in ps6.DefaultIfEmpty()
				where cd.IsDelete == false && (allowStatNoList.Contains(stat.StatNo) || cd.IsCommon == true)
				select new
				{
					c.ID,
					c.CustNo,
					CustLevel = cd.CustLevel,
					CustCHName = cd.CustCHName,
					CustEName1 = cd.CustEName1,
					CustEName2 = cd.CustEName2,
					CustCName = cd.CustCName,
					CustEName = cd.CustEName,
					CarryName = cd.CarryName,
					Code5 = cd.Code5,
					Code7 = cd.Code7,
					Add_1 = cd.Add_1,
					Add_2 = cd.Add_2,
					Add_3 = cd.Add_3,
					Add_4 = cd.Add_4,
					Add_5 = cd.Add_5,
					Add_6 = cd.Add_6,
					CustAddr = cd.CustAddr,
					CustAddrFull = cd.CustAddr
						   + (cd.Add_1 == 0 || cd.Add_1 == null ? null : (cd.Add_1 + "段"))
						   + (cd.Add_2 == 0 || cd.Add_2 == null ? null : (cd.Add_2 + "巷"))
						   + (cd.Add_3 == 0 || cd.Add_3 == null ? null : (cd.Add_3 + "弄"))
						   + (cd.Add_4 == "" || cd.Add_4 == null ? null : (cd.Add_4 + "號"))
						   + (cd.Add_5 == 0 || cd.Add_5 == null ? null : (cd.Add_5 + "樓"))
						   + (cd.Add_6 == "" || cd.Add_6 == null ? null : cd.Add_6),
					CustENAddr1 = cd.CustENAddr1,
					CustENAddr2 = cd.CustENAddr2,
					SendBy = cd.SendBy,
					InvNo = cd.InvNo,
					Country = cd.Country,
					City = cd.City,
					State = cd.State,
					PostDist = cd.PostDist,
					ESendBy = cd.ESendBy,
					EInvNo = cd.EInvNo,
					ECountry = cd.ECountry,
					ECity = cd.ECity,
					EState = cd.EState,
					EPostDist = cd.EPostDist,
					IDNo = cd.IDNo,
					Email = cd.Email,
					Phone = cd.Phone,
					FaxNo = cd.FaxNo,
					CtcAcc = cd.CtcAcc,
					AccPhone = cd.AccPhone,
					CtcSale = cd.CtcSale,
					CtcSale2 = cd.CtcSale2,
					CtcSale3 = cd.CtcSale3,
					Account = cd.Account,
					PayTerm = cd.PayTerm,
					CcID = cd.CcID,
					PayDate = cd.PayDate,
					SignDate = cd.SignDate,
					SuspDate = cd.SuspDate,
					Discnt = cd.Discnt,
					DiscntOut = cd.DiscntOut,
					QuoType = cd.QuoType,
					Balance = cd.Balance,
					IsinVoice = cd.IsinVoice,
					InvTitle = cd.InvTitle,
					InvAddr = cd.InvAddr,
					StatID = cd.StatID,
					IsAgent = cd.IsAgent,
					IsCommon = cd.IsCommon,
					IsServer = cd.IsServer,
					PickUpAreaID = cd.PickUpAreaID,
					Remark5 = cd.Remark5,
					SectorNo = cd.SectorNo,
					Remark = cd.Remark,
					DayOfWeek = cd.DayOfWeek,
					RedyDayWeekly = cd.RedyDayWeekly,
					RedyTime = cd.RedyTime,
					CreatedDate = cd.CreatedDate,
					CreatedBy = u.UserName,
					UpdatedDate = cd.UpdatedDate,
					UpdatedBy = cd.UpdatedBy,
					DeletedDate = cd.DeletedDate,
					DeletedBy = cd.DeletedBy,
					IsDelete = cd.IsDelete,
					Code5_C = cd.Code5_C,
					Code7_C = cd.Code7_C,
					Add_1_C = cd.Add_1_C,
					Add_2_C = cd.Add_2_C,
					Add_3_C = cd.Add_3_C,
					Add_4_C = cd.Add_4_C,
					Add_5_C = cd.Add_5_C,
					Add_6_C = cd.Add_6_C,
					CustAddr_C = cd.CustAddr_C,
					CustAddrFull_C = cd.CustAddr_C
						   + (cd.Add_1_C == 0 || cd.Add_1_C == null ? null : (cd.Add_1_C + "段"))
						   + (cd.Add_2_C == 0 || cd.Add_2_C == null ? null : (cd.Add_2_C + "巷"))
						   + (cd.Add_3_C == 0 || cd.Add_3_C == null ? null : (cd.Add_3_C + "弄"))
						   + (cd.Add_4_C == "" || cd.Add_4_C == null ? null : (cd.Add_4_C + "號"))
						   + (cd.Add_5_C == 0 || cd.Add_5_C == null ? null : (cd.Add_5_C + "樓"))
						   + (cd.Add_6_C == "" || cd.Add_6_C == null ? null : cd.Add_6_C),
					CcNo = Cc == null ? null : Cc.CcNo,
					StatNo = stat == null ? null : stat.StatNo,
					SectorName = sect == null ? null : sect.SectorName,
					PickUpAreaNo = area == null ? null : area.PickUpAreaNo,
					PickUpAreaName = area == null ? null : area.PickUpAreaName,
					IsFormal = c.CustNo.Substring(c.CustNo.Length - 1, 1) == "T" ? false : true,
				};

			if (data.CustNo.IsNotEmpty())
				cust = cust.Where(x => x.CustNo.Contains(data.CustNo));
			if (data.CustCName.IsNotEmpty())
				cust = cust.Where(x => x.CustCName.Contains(data.CustCName));
			if (data.CustEName.IsNotEmpty())
				cust = cust.Where(x => x.CustEName.Contains(data.CustEName));
			if (data.CustCHName.IsNotEmpty())
				cust = cust.Where(x => x.CustCHName.Contains(data.CustCHName));
			if (data.IsFormal == true || data.IsFormal == false)
				cust = cust.Where(x => x.IsFormal == data.IsFormal);
			if ((data.IsCommon == false && Request["IsCommon"] == "false") || data.IsCommon == true)
				cust = cust.Where(x => x.IsCommon == data.IsCommon);
			if ((data.IsServer == false && Request["IsServer"] == "false") || data.IsServer == true)
				cust = cust.Where(x => x.IsServer == data.IsServer);
			if (data.Phone.IsNotEmpty())
				cust = cust.Where(x => x.Phone.Contains(data.Phone));
			if (data.CcNo.IsNotEmpty())
				cust = cust.Where(x => x.CcNo.Contains(data.CcNo));
			if (data.InvNo.IsNotEmpty())
				cust = cust.Where(x => x.InvNo.Contains(data.InvNo));
			if (data.IDNo.IsNotEmpty())
				cust = cust.Where(x => x.IDNo.Contains(data.IDNo));
			if (data.StatNo.IsNotEmpty())
				cust = cust.Where(x => x.StatNo.Contains(data.StatNo));
			if (data.SectorName.IsNotEmpty())
				cust = cust.Where(x => x.SectorName.Contains(data.SectorName));
			if (data.PickUpAreaName.IsNotEmpty())
				cust = cust.Where(x => x.PickUpAreaName.Contains(data.PickUpAreaName));
			if (data.CustAddrFull.IsNotEmpty())
				cust = cust.Where(x => x.CustAddrFull.Contains(data.CustAddrFull));

			var records = cust.Count();
			cust = cust.OrderBy(x => x.CustNo).Skip((page - 1) * rows).Take(rows);
			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = cust,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON2(ORG_CustDetail data, int page = 1, int rows = 40)
		{
			var CustDetail =
				from cd in db.ORG_CustDetail.Where(x => x.IsDelete == false)
				join stat in db.ORG_Stat.Where(x => x.IsDelete == false) on cd.StatID equals stat.ID into ps2
				from stat in ps2.DefaultIfEmpty()
				join area in db.ORG_PickUpArea.Where(x => x.IsDelete == false) on cd.PickUpAreaID equals area.ID into ps3
				from area in ps3.DefaultIfEmpty()
				join sect in db.ORG_Sector.Where(x => x.IsDelete == false) on cd.SectorNo equals sect.SectorNo into ps4
				from sect in ps4.DefaultIfEmpty()
				join Cc in db.ORG_Cc.Where(x => x.IsDelete == false) on cd.CcID equals Cc.ID into ps5
				from Cc in ps5.DefaultIfEmpty()
				join u in db.SYS_User.Where(x => x.IsDelete == false) on cd.CreatedBy equals u.Account into ps6
				from u in ps6.DefaultIfEmpty()
				select new
				{
					CustNo = cd.CustNo,
					sNo = cd.sNo,
					CustLevel = cd.CustLevel,
					CustCHName = cd.CustCHName,
					CustEName1 = cd.CustEName1,
					CustEName2 = cd.CustEName2,
					CustCName = cd.CustCName,
					CustEName = cd.CustEName,
					CarryName = cd.CarryName,
					Code5 = cd.Code5,
					Code7 = cd.Code7,
					Add_1 = cd.Add_1,
					Add_2 = cd.Add_2,
					Add_3 = cd.Add_3,
					Add_4 = cd.Add_4,
					Add_5 = cd.Add_5,
					Add_6 = cd.Add_6,
					CustAddr = cd.CustAddr,
					CustAddrFull = cd.CustAddr
						   + (cd.Add_1 == 0 || cd.Add_1 == null ? null : (cd.Add_1 + "段"))
						   + (cd.Add_2 == 0 || cd.Add_2 == null ? null : (cd.Add_2 + "巷"))
						   + (cd.Add_3 == 0 || cd.Add_3 == null ? null : (cd.Add_3 + "弄"))
						   + (cd.Add_4 == "" || cd.Add_4 == null ? null : (cd.Add_4 + "號"))
						   + (cd.Add_5 == 0 || cd.Add_5 == null ? null : (cd.Add_5 + "樓"))
						   + (cd.Add_6 == "" || cd.Add_6 == null ? null : cd.Add_6),
					CustENAddr1 = cd.CustENAddr1,
					CustENAddr2 = cd.CustENAddr2,
					SendBy = cd.SendBy,
					InvNo = cd.InvNo,
					Country = cd.Country,
					City = cd.City,
					State = cd.State,
					PostDist = cd.PostDist,
					ESendBy = cd.ESendBy,
					EInvNo = cd.EInvNo,
					ECountry = cd.ECountry,
					ECity = cd.ECity,
					EState = cd.EState,
					EPostDist = cd.EPostDist,
					IDNo = cd.IDNo,
					Email = cd.Email,
					Phone = cd.Phone,
					FaxNo = cd.FaxNo,
					CtcAcc = cd.CtcAcc,
					AccPhone = cd.AccPhone,
					CtcSale = cd.CtcSale,
					CtcSale2 = cd.CtcSale2,
					CtcSale3 = cd.CtcSale3,
					Account = cd.Account,
					PayTerm = cd.PayTerm,
					CcID = cd.CcID,
					PayDate = cd.PayDate,
					SignDate = cd.SignDate,
					SuspDate = cd.SuspDate,
					Discnt = cd.Discnt,
					DiscntOut = cd.DiscntOut,
					QuoType = cd.QuoType,
					Balance = cd.Balance,
					IsinVoice = cd.IsinVoice,
					InvTitle = cd.InvTitle,
					InvAddr = cd.InvAddr,
					StatID = cd.StatID,
					IsAgent = cd.IsAgent,
					IsCommon = cd.IsCommon,
					IsServer = cd.IsServer,
					IsMas = cd.IsMas ?? false,
					PickUpAreaID = cd.PickUpAreaID,
					Remark5 = cd.Remark5,
					SectorNo = cd.SectorNo,
					Remark = cd.Remark,
					DayOfWeek = cd.DayOfWeek,
					RedyDayWeekly = cd.RedyDayWeekly,
					RedyTime = cd.RedyTime,
					CreatedDate = cd.CreatedDate,
					CreatedBy = u.UserName,
					UpdatedDate = cd.UpdatedDate,
					UpdatedBy = cd.UpdatedBy,
					DeletedDate = cd.DeletedDate,
					DeletedBy = cd.DeletedBy,
					IsDelete = cd.IsDelete,
					Code5_C = cd.Code5_C,
					Code7_C = cd.Code7_C,
					Add_1_C = cd.Add_1_C,
					Add_2_C = cd.Add_2_C,
					Add_3_C = cd.Add_3_C,
					Add_4_C = cd.Add_4_C,
					Add_5_C = cd.Add_5_C,
					Add_6_C = cd.Add_6_C,
					CustAddr_C = cd.CustAddr_C,
					CustAddrFull_C = cd.CustAddr_C
						   + (cd.Add_1_C == 0 || cd.Add_1_C == null ? null : (cd.Add_1_C + "段"))
						   + (cd.Add_2_C == 0 || cd.Add_2_C == null ? null : (cd.Add_2_C + "巷"))
						   + (cd.Add_3_C == 0 || cd.Add_3_C == null ? null : (cd.Add_3_C + "弄"))
						   + (cd.Add_4_C == "" || cd.Add_4_C == null ? null : (cd.Add_4_C + "號"))
						   + (cd.Add_5_C == 0 || cd.Add_5_C == null ? null : (cd.Add_5_C + "樓"))
						   + (cd.Add_6_C == "" || cd.Add_6_C == null ? null : cd.Add_6_C),
					CcNo = Cc == null ? null : Cc.CcNo,
					StatNo = stat == null ? null : stat.StatNo,
					SectorName = sect == null ? null : sect.SectorName,
					PickUpAreaNo = area == null ? null : area.PickUpAreaNo,
					PickUpAreaName = area == null ? null : area.PickUpAreaName,
					IsFormal = cd.CustNo.Substring(cd.CustNo.Length - 1, 1) == "T" ? false : true,
				};
			if (data.CustNo.IsNotEmpty())
				CustDetail = CustDetail.Where(x => x.CustNo == data.CustNo);
			if (data.CustCHName.IsNotEmpty())
				CustDetail = CustDetail.Where(x => x.CustCHName.Contains(data.CustCHName));
			if (data.Phone.IsNotEmpty())
				CustDetail = CustDetail.Where(x => x.Phone.Contains(data.Phone));
			if (data.CarryName.IsNotEmpty())
				CustDetail = CustDetail.Where(x => x.CarryName.Contains(data.CarryName));
			if (data.InvNo.IsNotEmpty())
				CustDetail = CustDetail.Where(x => x.InvNo.Contains(data.InvNo));
			if (data.IsFormal != null)
				CustDetail = CustDetail.Where(x => x.IsFormal == data.IsFormal);

			var records = CustDetail.Count();
			CustDetail = CustDetail.OrderByDescending(o => o.IsFormal).ThenByDescending(o => o.IsMas).ThenBy(o => o.sNo).Skip((page - 1) * rows).Take(rows);

			var dataList = new List<ORG_CustDetail>();
			var Index = 0;
			foreach (var c in CustDetail)
			{
				Index = ++Index;
				var cData = new ORG_CustDetail()
				{
					Index = Index,
					CustNo = c.CustNo,
					sNo = c.sNo,
					CustLevel = c.CustLevel,
					CustCHName = c.CustCHName,
					CustEName1 = c.CustEName1,
					CustEName2 = c.CustEName2,
					CustCName = c.CustCName,
					CustEName = c.CustEName,
					CarryName = c.CarryName,
					Code5 = c.Code5,
					Code7 = c.Code7,
					Add_1 = c.Add_1,
					Add_2 = c.Add_2,
					Add_3 = c.Add_3,
					Add_4 = c.Add_4,
					Add_5 = c.Add_5,
					Add_6 = c.Add_6,
					CustAddr = c.CustAddr,
					CustAddrFull = c.CustAddrFull,
					CustENAddr1 = c.CustENAddr1,
					CustENAddr2 = c.CustENAddr2,
					SendBy = c.SendBy,
					InvNo = c.InvNo,
					Country = c.Country,
					City = c.City,
					State = c.State,
					PostDist = c.PostDist,
					ESendBy = c.ESendBy,
					EInvNo = c.EInvNo,
					ECountry = c.ECountry,
					ECity = c.ECity,
					EState = c.EState,
					EPostDist = c.EPostDist,
					IDNo = c.IDNo,
					Email = c.Email,
					Phone = c.Phone,
					FaxNo = c.FaxNo,
					CtcAcc = c.CtcAcc,
					AccPhone = c.AccPhone,
					CtcSale = c.CtcSale,
					CtcSale2 = c.CtcSale2,
					CtcSale3 = c.CtcSale3,
					Account = c.Account,
					PayTerm = c.PayTerm,
					CcID = c.CcID,
					PayDate = c.PayDate,
					SignDate = c.SignDate,
					SuspDate = c.SuspDate,
					Discnt = c.Discnt,
					DiscntOut = c.DiscntOut,
					QuoType = c.QuoType,
					Balance = c.Balance,
					IsinVoice = c.IsinVoice,
					InvTitle = c.InvTitle,
					InvAddr = c.InvAddr,
					StatID = c.StatID,
					IsAgent = c.IsAgent,
					IsCommon = c.IsCommon,
					IsServer = c.IsServer,
					IsMas = c.IsMas,
					PickUpAreaID = c.PickUpAreaID,
					Remark5 = c.Remark5,
					SectorNo = c.SectorNo,
					Remark = c.Remark,
					DayOfWeek = c.DayOfWeek,
					RedyDayWeekly = c.RedyDayWeekly,
					RedyTime = c.RedyTime,
					CreatedDate = c.CreatedDate,
					CreatedBy = c.CreatedBy,
					UpdatedDate = c.UpdatedDate,
					UpdatedBy = c.UpdatedBy,
					DeletedDate = c.DeletedDate,
					DeletedBy = c.DeletedBy,
					IsDelete = c.IsDelete,
					Code5_C = c.Code5_C,
					Code7_C = c.Code7_C,
					Add_1_C = c.Add_1_C,
					Add_2_C = c.Add_2_C,
					Add_3_C = c.Add_3_C,
					Add_4_C = c.Add_4_C,
					Add_5_C = c.Add_5_C,
					Add_6_C = c.Add_6_C,
					CustAddr_C = c.CustAddr_C,
					CustAddrFull_C = c.CustAddrFull_C,
					CcNo = c.CcNo,
					StatNo = c.StatNo,
					SectorName = c.SectorName,
					PickUpAreaNo = c.PickUpAreaNo,
					PickUpAreaName = c.PickUpAreaName,
					IsFormal = c.IsFormal
				};
				dataList.Add(cData);
			}
			var custDetailData = dataList as IEnumerable<ORG_CustDetail>;
			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = custDetailData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult FindCustByPhoneInvNo(string targetString)
		{
			IQueryable<ORG_Cust> userRecord;
			JObject jobj;
			userRecord = from u in db.ORG_Cust
						 where (u.IsDelete == false) && ((targetString == u.InvNo) || (targetString == u.Phone))
						 select u;
			if (userRecord.Count() > 0)
			{
				jobj = new JObject {
					{"result",1 },
					{"CustNo",userRecord.First().CustNo },
					{"CustCName",userRecord.First().CustCName },
					{"CustCHName",userRecord.First().CustCHName },
					{"Account",userRecord.First().Account }
				};
			}
			else
			{
				jobj = new JObject {
					{"result",0 }
				};
			}

			return Content(JsonConvert.SerializeObject(jobj), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
	}
}
