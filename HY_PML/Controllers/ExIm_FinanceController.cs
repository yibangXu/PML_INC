using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace HY_PML.Controllers
{
	public class ExIm_FinanceController : Controller
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
			ViewBag.Title = "進/出口帳單";
			ViewBag.AddFunc = "";
			ViewBag.EditFunc = "";
			ViewBag.DelFunc = "";
			ViewBag.ControllerName = "ExIm_Finance";
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
		public ActionResult GetGridJSON(ExIm_Finance data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var exIm_Finance =
				from b in db.Bill_Lading.Where(x => x.IsDelete == false && x.IsReview == true)
				join e in db.Export_Bill on b.LadingNo equals e.LadingNo into ps
				from e in ps.DefaultIfEmpty()
				join i in db.Import_Bill on b.LadingNo equals i.LadingNo into ps2
				from i in ps2.DefaultIfEmpty()
				join ue in db.SYS_User on e.CreateBy equals ue.Account into ps3
				from ue in ps3.DefaultIfEmpty()
				join ui in db.SYS_User on i.CreateBy equals ui.Account into ps4
				from ui in ps4.DefaultIfEmpty()
				select new
				{
					ImOrEx = b.ImOrEx,
					BillNo = b.ImOrEx == "Ex" ? e.ExBillNo : i.ImBillNo,
					LadingNo = b.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingDate = b.LadingDate,
					StatNo = b.ImOrEx == "Ex" ? e.StatNo : i.StatNo,
					StatName = b.ImOrEx == "Ex" ? e.StatName : i.StatName,
					CustNo = b.ImOrEx == "Ex" ? e.CustNo : i.CustNo,
					CHName = b.ImOrEx == "Ex" ? e.CHName : i.CHName,
					CcNo = b.ImOrEx == "Ex" ? e.CcNo : i.CcNo,
					DestNo = b.ImOrEx == "Ex" ? e.DestNo : i.DestNo,
					CName = b.ImOrEx == "Ex" ? e.CName : i.CName,
					Type = b.ImOrEx == "Ex" ? e.Type : i.Type,
					PiecesNo = b.ImOrEx == "Ex" ? e.PiecesNo : i.PiecesNo,
					Weight = b.ImOrEx == "Ex" ? e.Weight : i.Weight,
					Freight = b.ImOrEx == "Ex" ? e.Freight : i.Freight,
					CustomsPay = b.ImOrEx == "Ex" ? e.CustomsPay : i.CustomsPay,
					Tariff = b.ImOrEx == "Ex" ? e.Tariff : i.Tariff,
					ProdIdPay = e != null ? e.ProdIdPay : 0,
					InsurancePay = e != null ? e.InsurancePay : 0,
					OtherPayTax = b.ImOrEx == "Ex" ? e.OtherPayTax : i.OtherPayTax,
					OtherPayNoTax = b.ImOrEx == "Ex" ? e.OtherPayNoTax : i.OtherPayNoTax,
					ToPayment = b.ImOrEx == "Ex" ? e.ToPayment : i.ToPayment,
					ToPaymentCurrency = b.ImOrEx == "Ex" ? e.ToPaymentCurrency : i.ToPaymentCurrency,
					AgentPay = b.ImOrEx == "Ex" ? e.AgentPay : i.AgentPay,
					AgentPayCurrency = b.ImOrEx == "Ex" ? e.AgentPayCurrency : i.AgentPayCurrency,
					Remark = b.ImOrEx == "Ex" ? e.Remark : i.Remark,
					Total = b.ImOrEx == "Ex" ? e.Total : i.Total,
					IsFinance = b.ImOrEx == "Ex" ? e.IsFinance : i.IsFinance,
					FinanceBy = b.ImOrEx == "Ex" ? e.FinanceBy : i.FinanceBy,
					FinanceTime = b.ImOrEx == "Ex" ? e.FinanceTime : i.FinanceTime,
					CreateBy = b.ImOrEx == "Ex" ? ue.UserName : ui.UserName,
					CreateTime = b.ImOrEx == "Ex" ? e.CreateTime : i.CreateTime,
					UpdateBy = b.ImOrEx == "Ex" ? e.UpdateBy : i.UpdateBy,
					UpdateTime = b.ImOrEx == "Ex" ? e.UpdateTime : i.UpdateTime,
					DeletedBy = b.ImOrEx == "Ex" ? e.DeletedBy : i.DeletedBy,
					DeletedTime = b.ImOrEx == "Ex" ? e.DeletedTime : i.DeletedTime,
					IsDelete = b.ImOrEx == "Ex" ? e.IsDelete : i.IsDelete,
				};

			if (data.IsFinance != null)
				exIm_Finance = exIm_Finance.Where(x => x.IsFinance == data.IsFinance);
			if (data.LadingNo != null)
				exIm_Finance = exIm_Finance.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type != null)
				exIm_Finance = exIm_Finance.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.StatNo.IsNotEmpty())
				exIm_Finance = exIm_Finance.Where(x => x.StatNo == data.StatNo);
			if (data.StatName.IsNotEmpty())
				exIm_Finance = exIm_Finance.Where(x => x.StatName == data.StatName);
			if (data.CHName != null)
				exIm_Finance = exIm_Finance.Where(x => x.CHName.Contains(data.CHName));
			if (data.ImOrEx != null)
				exIm_Finance = exIm_Finance.Where(x => x.ImOrEx == data.ImOrEx);
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				exIm_Finance = exIm_Finance.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			int records = exIm_Finance.Count();
			exIm_Finance = exIm_Finance.OrderBy(o => o.LadingNo).Skip((page - 1) * rows).Take(rows);
			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = exIm_Finance,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult Finance(string[] BillNo)
		{
			var result = new ResultHelper();
			var repeat = false;
			using (var trans = db.Database.BeginTransaction())
			{
				foreach (var i in BillNo)
				{
					if (i.Contains("Im-"))
					{
						var data = db.Import_Bill.Where(x => x.ImBillNo == i && x.IsDelete == false).FirstOrDefault();
						if (data.IsFinance == true)
							repeat = true;
						data.IsFinance = true;
						data.FinanceTime = DateTime.Now;
						data.FinanceBy = User.Identity.Name;
						data.UpdateTime = DateTime.Now;
						data.UpdateBy = User.Identity.Name;
						db.Entry(data).State = EntityState.Modified;
					}
					else if (i.Contains("EX-"))
					{
						var data = db.Export_Bill.Where(x => x.ExBillNo == i && x.IsDelete == false).FirstOrDefault();
						if (data.IsFinance == true)
							repeat = true;
						data.IsFinance = true;
						data.FinanceTime = DateTime.Now;
						data.FinanceBy = User.Identity.Name;
						data.UpdateTime = DateTime.Now;
						data.UpdateBy = User.Identity.Name;
						db.Entry(data).State = EntityState.Modified;
					}
				}
				if (repeat == true)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "選擇資料中，已存在轉入過之資料";
					trans.Rollback();
				}
				else
				{
					try
					{
						var dbFunctions = new DBFunctions();
						List<SqlParameter> sqldata = new List<SqlParameter>();
						StringBuilder sqlstr = new StringBuilder();
						var sqlresult = "";
						var i = 0;
						sqlstr.Append("DECLARE @chk tinyint SET @chk = 0 Begin Transaction");
						foreach (var b in BillNo)
						{
							i++;
							if (b.Contains("IM-"))
							{
								var imBillData = db.Import_Bill.Where(x => x.ImBillNo == b).FirstOrDefault();
								var ladingData = db.Bill_Lading.Where(x => x.LadingNo == imBillData.LadingNo).FirstOrDefault();
								var hubName = db.ORG_Hub.Where(x => x.HubNo == ladingData.HubNo).Select(x => x.HubName).FirstOrDefault();
								var transportationSData = db.TransportationS.Where(x => x.LadingNo == imBillData.LadingNo).FirstOrDefault();
								sqlstr.AppendFormat(@"
INSERT INTO TB_temp_Accno ( [DAMOUNT], [CAMOUNT], [PTNO], [REMARK], [VTYPE],[OKED] ) 
VALUES( @DAMOUNT{0}, @CAMOUNT{1}, @dPTNO{2}, @REMARK{3}, @VTYPE{4} , @OKED{27} ); 
IF @@Error <> 0 BEGIN SET @chk = 1 END 
INSERT INTO TB_temp_Acc1  ( [DC], [SUBNO], [SUBST], [IDNO], [MONEYS], [PTNO], [TransferBy], [TransferTime], [CcNo], [DestNo], [PType], [PiecesNo], [Weight], [Freight], [CustomsPay], [Tariff], [OtherPayTax], [OtherPayNoTax], [ToPayment], [ToPaymentCurrency], [AgentPay], [AgentPayCurrency] ,[LadingDate],[AStatNo],[SendCHName],[CName],[StatNo],[SectorNo],[SectorName],[HubName],[SendCustAddr],[SendECustAddr],[AStatName],[StatName],[HubNo] ) 
VALUES(@DC{5}, @SUBNO{6}, @SUBST{7}, @IDNO{8}, @MONEYS{9}, @dPTNO{10}, @TransferBy{11}, @TransferTime{12}, @CcNo{13}, @DestNo{14}, @PType{15}, @PiecesNo{16}, @Weight{17}, @Freight{18}, @CustomsPay{19}, @Tariff{20}, @OtherPayTax{21}, @OtherPayNoTax{22}, @ToPayment{23}, @ToPaymentCurrency{24}, @AgentPay{25}, @AgentPayCurrency{26} ,@LadingDate{28},@AStatNo{29},@SendCHName{30},@CName{31},@StatNo{32},@SectorNo{33},@SectorName{34},@HubName{35},@SendCustAddr{36},@SendECustAddr{37},@AStatName{38},@StatName{39},@HubNo{40}); 
IF @@Error <> 0 BEGIN SET @chk = 1 END ", i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i);
								sqldata.Add(new SqlParameter("@DAMOUNT" + i, imBillData.Total ?? 0));
								sqldata.Add(new SqlParameter("@CAMOUNT" + i, '0'));
								sqldata.Add(new SqlParameter("@PTNO" + i, ladingData.LadingNo_Type));
								sqldata.Add(new SqlParameter("@REMARK" + i, imBillData.Remark ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@VTYPE" + i, "進口帳單"));
								sqldata.Add(new SqlParameter("@DC" + i, "借"));
								sqldata.Add(new SqlParameter("@SUBNO" + i, "1123.001"));
								sqldata.Add(new SqlParameter("@SUBST" + i, "應收帳款"));
								sqldata.Add(new SqlParameter("@IDNO" + i, imBillData.CustNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@MONEYS" + i, imBillData.Total ?? 0));
								sqldata.Add(new SqlParameter("@dPTNO" + i, ladingData.LadingNo_Type));
								sqldata.Add(new SqlParameter("@TransferBy" + i, User.Identity.Name));
								sqldata.Add(new SqlParameter("@TransferTime" + i, DateTime.Now));
								sqldata.Add(new SqlParameter("@CcNo" + i, imBillData.CcNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@DestNo" + i, imBillData.DestNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@PType" + i, imBillData.Type ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@PiecesNo" + i, imBillData.PiecesNo));
								sqldata.Add(new SqlParameter("@Weight" + i, imBillData.Weight ?? 0));
								sqldata.Add(new SqlParameter("@Freight" + i, imBillData.Freight ?? 0));
								sqldata.Add(new SqlParameter("@CustomsPay" + i, imBillData.CustomsPay ?? 0));
								sqldata.Add(new SqlParameter("@Tariff" + i, imBillData.Tariff ?? 0));
								sqldata.Add(new SqlParameter("@OtherPayTax" + i, imBillData.OtherPayTax ?? 0));
								sqldata.Add(new SqlParameter("@OtherPayNoTax" + i, imBillData.OtherPayNoTax ?? 0));
								sqldata.Add(new SqlParameter("@ToPayment" + i, imBillData.ToPayment ?? 0));
								sqldata.Add(new SqlParameter("@ToPaymentCurrency" + i, imBillData.ToPaymentCurrency ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@AgentPay" + i, imBillData.AgentPay ?? 0));
								sqldata.Add(new SqlParameter("@AgentPayCurrency" + i, imBillData.AgentPayCurrency ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@OKED" + i, "否"));
								sqldata.Add(new SqlParameter("@LadingDate" + i, imBillData.LadingDate));
								sqldata.Add(new SqlParameter("@AStatNo" + i, ladingData.AStatNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@SendCHName" + i, ladingData.SendCHName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@CName" + i, imBillData.CName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@StatNo" + i, imBillData.StatNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@SectorNo" + i, transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorNo ?? (object)DBNull.Value)));
								sqldata.Add(new SqlParameter("@SectorName" + i, transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorName ?? (object)DBNull.Value)));
								sqldata.Add(new SqlParameter("@HubName" + i, hubName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@SendCustAddr" + i, ladingData.SendCHName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@SendECustAddr" + i, ladingData.SendECustAddr ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@AStatName" + i, ladingData.AStatName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@StatName" + i, imBillData.StatName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@HubNo" + i, ladingData.HubNo ?? (object)DBNull.Value));
							}
							else if (b.Contains("EX-"))
							{
								var exBillData = db.Export_Bill.Where(x => x.ExBillNo == b).FirstOrDefault();
								var ladingData = db.Bill_Lading.Where(x => x.LadingNo == exBillData.LadingNo).FirstOrDefault();
								var hubName = db.ORG_Hub.Where(x => x.HubNo == ladingData.HubNo).Select(x => x.HubName).FirstOrDefault();
								var transportationSData = db.TransportationS.Where(x => x.LadingNo == exBillData.LadingNo).FirstOrDefault();
								sqlstr.AppendFormat(@"
INSERT INTO TB_temp_Accno ( [DAMOUNT], [CAMOUNT], [PTNO], [REMARK], [VTYPE],[OKED])
VALUES(@DAMOUNT{0}, @CAMOUNT{1}, @PTNO{2}, @REMARK{3}, @VTYPE{4},@OKED{27}); 
IF @@Error <> 0 BEGIN SET @chk = 1 END
INSERT INTO TB_temp_Acc1  ( [DC], [SUBNO], [SUBST], [IDNO], [MONEYS], [PTNO], [TransferBy], [TransferTime] , [CcNo], [DestNo], [PType], [PiecesNo], [Weight], [Freight], [CustomsPay], [Tariff], [OtherPayTax], [OtherPayNoTax], [ToPayment], [ToPaymentCurrency], [AgentPay], [AgentPayCurrency],[LadingDate],[AStatNo],[SendCHName],[CName],[ProdIdPay],[InsurancePay],[StatNo] ,[SectorNo],[SectorName],[HubName],[SendCustAddr],[SendECustAddr],[AStatName],[StatName],[HubNo]) 
VALUES(@DC{5}, @SUBNO{6}, @SUBST{7}, @IDNO{8}, @MONEYS{9}, @dPTNO{10}, @TransferBy{11}, @TransferTime{12}, @CcNo{13}, @DestNo{14}, @PType{15}, @PiecesNo{16}, @Weight{17}, @Freight{18}, @CustomsPay{19}, @Tariff{20}, @OtherPayTax{21}, @OtherPayNoTax{22}, @ToPayment{23}, @ToPaymentCurrency{24}, @AgentPay{25}, @AgentPayCurrency{26},@LadingDate{28},@AStatNo{29},@SendCHName{30},@CName{31},@ProdIdPay{32},@InsurancePay{33},@StatNo{34},@SectorNo{35},@SectorName{36},@HubName{37},@SendCustAddr{38},@SendECustAddr{39},@AStatName{40},@StatName{41},@HubNo{42}); 
IF @@Error <> 0 BEGIN SET @chk = 1 END ", i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i);

								sqldata.Add(new SqlParameter("@DAMOUNT" + i, exBillData.Total ?? 0));
								sqldata.Add(new SqlParameter("@CAMOUNT" + i, '0'));
								sqldata.Add(new SqlParameter("@PTNO" + i, ladingData.LadingNo_Type));
								sqldata.Add(new SqlParameter("@REMARK" + i, exBillData.Remark ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@VTYPE" + i, "出口帳單"));
								sqldata.Add(new SqlParameter("@DC" + i, "借"));
								sqldata.Add(new SqlParameter("@SUBNO" + i, "1123.001"));
								sqldata.Add(new SqlParameter("@SUBST" + i, "應收帳款"));
								sqldata.Add(new SqlParameter("@IDNO" + i, exBillData.CustNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@MONEYS" + i, exBillData.Total ?? 0));
								sqldata.Add(new SqlParameter("@dPTNO" + i, ladingData.LadingNo_Type));
								sqldata.Add(new SqlParameter("@TransferBy" + i, User.Identity.Name));
								sqldata.Add(new SqlParameter("@TransferTime" + i, DateTime.Now));
								sqldata.Add(new SqlParameter("@CcNo" + i, exBillData.CcNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@DestNo" + i, exBillData.DestNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@PType" + i, exBillData.Type ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@PiecesNo" + i, exBillData.PiecesNo));
								sqldata.Add(new SqlParameter("@Weight" + i, exBillData.Weight ?? 0));
								sqldata.Add(new SqlParameter("@Freight" + i, exBillData.Freight ?? 0));
								sqldata.Add(new SqlParameter("@CustomsPay" + i, exBillData.CustomsPay ?? 0));
								sqldata.Add(new SqlParameter("@Tariff" + i, exBillData.Tariff ?? 0));
								sqldata.Add(new SqlParameter("@OtherPayTax" + i, exBillData.OtherPayTax ?? 0));
								sqldata.Add(new SqlParameter("@OtherPayNoTax" + i, exBillData.OtherPayNoTax ?? 0));
								sqldata.Add(new SqlParameter("@ToPayment" + i, exBillData.ToPayment ?? 0));
								sqldata.Add(new SqlParameter("@ToPaymentCurrency" + i, exBillData.ToPaymentCurrency ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@AgentPay" + i, exBillData.AgentPay ?? 0));
								sqldata.Add(new SqlParameter("@AgentPayCurrency" + i, exBillData.AgentPayCurrency ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@OKED" + i, "否"));
								sqldata.Add(new SqlParameter("@LadingDate" + i, exBillData.LadingDate));
								sqldata.Add(new SqlParameter("@AStatNo" + i, ladingData == null ? (object)DBNull.Value : (ladingData.AStatNo ?? (object)DBNull.Value)));
								sqldata.Add(new SqlParameter("@SendCHName" + i, ladingData == null ? (object)DBNull.Value : (ladingData.SendCHName ?? (object)DBNull.Value)));
								sqldata.Add(new SqlParameter("@CName" + i, exBillData.CName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@ProdIdPay" + i, exBillData.ProdIdPay ?? 0));
								sqldata.Add(new SqlParameter("@InsurancePay" + i, exBillData.InsurancePay ?? 0));
								sqldata.Add(new SqlParameter("@StatNo" + i, exBillData.StatNo ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@SectorNo" + i, transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorNo ?? (object)DBNull.Value)));
								sqldata.Add(new SqlParameter("@SectorName" + i, transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorName ?? (object)DBNull.Value)));
								sqldata.Add(new SqlParameter("@HubName" + i, hubName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@SendCustAddr" + i, ladingData.SendCustAddr ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@SendECustAddr" + i, ladingData.SendECustAddr ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@AStatName" + i, ladingData.AStatName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@StatName" + i, exBillData.StatName ?? (object)DBNull.Value));
								sqldata.Add(new SqlParameter("@HubNo" + i, ladingData.HubNo ?? (object)DBNull.Value));
							}
						}
						sqlstr.Append(@"
IF @chk <> 0 BEGIN Rollback Transaction END 
ELSE BEGIN Commit Transaction END");

						sqlresult = dbFunctions.FinanceInsert(sqldata, sqlstr.ToString());

						if (sqlresult == "OK")
						{
							db.SaveChanges();
							result.Ok = DataModifyResultType.Success;
							result.Message = "OK";
							trans.Commit();
						}
						else
						{
							result.Ok = DataModifyResultType.Faild;
							result.Message = sqlresult;
							trans.Rollback();
						}
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
	}
}