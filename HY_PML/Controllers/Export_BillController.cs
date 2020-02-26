using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class Export_BillController : Controller
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
			ViewBag.Title = "出口帳單";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "Export_Bill";
			ViewBag.FormCustomJsNew =
				"$('#LadingNo').textbox('readonly', false); " +
				"$('#LadingDate').textbox('readonly', false); ";
			ViewBag.FormCustomJsEdit =
				"$('#LadingNo').textbox('readonly', true); " +
				"$('#LadingDate').textbox('readonly', true);" +
				"if (row.IsFinance == '已轉'){$('#IsFinance').switchbutton('check');	$('.fitem>span>input').attr('readonly', true); $('.fitem>span>textarea').attr('readonly', true); $('.easyui-switchbutton.switchbutton-f').switchbutton('readonly');$('.fitem>a').css('visibility', 'hidden'); $('.fitem>span>span>a').css('visibility', 'hidden');$('.fitem>span>span>a>a').css('visibility', 'hidden'); $('#dlg-buttons>a').css('visibility', 'hidden')}else{$('#IsFinance').switchbutton('uncheck');$('.fitem>span>input').attr('readonly', false); $('.fitem>span>textarea').attr('readonly', false); $('.easyui-switchbutton.switchbutton-f').switchbutton('readonly',false);$('.fitem>a').css('visibility', 'visible'); $('.fitem>span>span>a').css('visibility', 'visible'); $('.fitem>span>span>a>a').css('visibility', 'visible'); $('#dlg-buttons>a').css('visibility', 'visible')}";
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
		public ActionResult Add(Export_Bill data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var ErrorMsg = new StringBuilder();

			//取得登入者 站點資訊
			string cId = WebSiteHelper.CurrentUserID;
			int dbId = int.Parse(WebSiteHelper.CurrentUserDBID);
			var userInfo = from s in db.SYS_User
						   where s.IsDelete == false && s.ID == dbId
						   select s;
			string statNo = userInfo.First().StatNo;
			var statData = db.ORG_Stat.Where(x => x.StatNo == statNo).FirstOrDefault();
			string statName = statData.StatName;

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.Import_Bill.FirstOrDefault(x => x.LadingNo == data.LadingNo && x.IsDelete == false);
				if (userRecord == null)
				{
					var saveData = new Export_Bill();
					saveData.ExBillNo = "Ex-" + data.LadingNo;
					saveData.LadingNo = data.LadingNo;
					saveData.LadingDate = data.LadingDate;
					saveData.StatNo = data.StatNo;
					saveData.StatName = data.StatName;
					saveData.CustNo = data.CustNo;
					saveData.CHName = data.CHName;
					saveData.CcNo = data.CcNo;
					saveData.DestNo = data.DestNo;
					saveData.CName = data.CName;
					saveData.Type = data.Type;
					saveData.PiecesNo = data.PiecesNo;
					saveData.Weight = data.Weight;
					saveData.Freight = data.Freight;
					saveData.CustomsPay = data.CustomsPay;
					saveData.Tariff = data.Tariff;
					saveData.ProdIdPay = data.ProdIdPay;
					saveData.InsurancePay = data.InsurancePay;
					saveData.OtherPayTax = data.OtherPayTax;
					saveData.OtherPayNoTax = data.OtherPayNoTax;
					saveData.ToPayment = data.ToPayment;
					saveData.ToPaymentCurrency = data.ToPaymentCurrency;
					saveData.AgentPay = data.AgentPay;
					saveData.AgentPayCurrency = data.AgentPayCurrency;
					saveData.Remark = data.Remark;
					saveData.Total = data.Freight + data.CustomsPay + data.Tariff + data.ProdIdPay + data.InsurancePay + data.OtherPayTax + data.OtherPayNoTax + data.ToPayment + data.AgentPay;

					//以下系統自填
					saveData.CreateTime = DateTime.Now;
					saveData.CreateBy = User.Identity.Name;
					saveData.IsDelete = false;
					saveData.IsFinance = false;
					db.Export_Bill.Add(saveData);
				}
			}
			try
			{
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
		public ActionResult Edit(Export_Bill data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var dbFunctions = new DBFunctions();
			List<SqlParameter> sqldata = new List<SqlParameter>();
			StringBuilder sqlstr = new StringBuilder();
			var sqlresult = "OK";

			var editData = db.Export_Bill.FirstOrDefault(x => x.ExBillNo == data.ExBillNo && x.IsDelete == false);
			using (var trans = db.Database.BeginTransaction())
			{
				if (editData != null)
				{
					editData.StatNo = data.StatNo;
					editData.StatName = data.StatName;
					editData.CustNo = data.CustNo;
					editData.CHName = data.CHName;
					editData.CcNo = data.CcNo;
					editData.DestNo = data.DestNo;
					editData.CName = data.CName;
					editData.Type = data.Type;
					editData.PiecesNo = data.PiecesNo;
					editData.Weight = data.Weight;
					editData.Freight = data.Freight;
					editData.CustomsPay = data.CustomsPay;
					editData.Tariff = data.Tariff;
					editData.ProdIdPay = data.ProdIdPay;
					editData.InsurancePay = data.InsurancePay;
					editData.OtherPayTax = data.OtherPayTax;
					editData.OtherPayNoTax = data.OtherPayNoTax;
					editData.ToPayment = data.ToPayment;
					editData.ToPaymentCurrency = data.ToPaymentCurrency;
					editData.AgentPay = data.AgentPay;
					editData.AgentPayCurrency = data.AgentPayCurrency;
					editData.Remark = data.Remark;
					editData.Total = data.Freight ?? 0 + data.CustomsPay ?? 0 + data.Tariff ?? 0 + data.ProdIdPay ?? 0 + data.InsurancePay ?? 0 + data.OtherPayTax ?? 0 + data.OtherPayNoTax ?? 0 + data.ToPayment ?? 0 + data.AgentPay ?? 0;
					if (data.IsFinance == true && editData.IsFinance == false)
					{
						editData.IsFinance = true;
						editData.FinanceBy = User.Identity.Name;
						editData.FinanceTime = DateTime.Now;

						var ladingData = db.Bill_Lading.Where(x => x.LadingNo == editData.LadingNo).FirstOrDefault();
						var hubName = db.ORG_Hub.Where(x => x.HubNo == ladingData.HubNo).Select(x => x.HubName).FirstOrDefault();
						var transportationSData = db.TransportationS.Where(x => x.LadingNo == editData.LadingNo).FirstOrDefault();
						sqlstr.Append("DECLARE @chk tinyint SET @chk = 0 Begin Transaction");
						sqlstr.AppendFormat(@"
INSERT INTO TB_temp_Accno ([DAMOUNT],[CAMOUNT],[PTNO],[REMARK],[VTYPE],[OKED]) 
VALUES(@DAMOUNT,@CAMOUNT,@PTNO,@REMARK,@VTYPE, @OKED); 
IF @@Error <> 0 BEGIN SET @chk = 1 END
INSERT INTO TB_temp_Acc1 ([DC], [SUBNO], [SUBST], [IDNO], [MONEYS], [PTNO], [TransferBy], [TransferTime], [CcNo], [DestNo], [PType], [PiecesNo], [Weight], [Freight], [CustomsPay], [Tariff], [OtherPayTax], [OtherPayNoTax], [ToPayment], [ToPaymentCurrency], [AgentPay], [AgentPayCurrency],[LadingDate],[AStatNo],[SendCHName],[CName],[ProdIdPay],[InsurancePay],[StatNo],[SectorNo],[SectorName],[HubName],[SendCustAddr],[SendECustAddr],[AStatName],[StatName],[HubNo],[SendCustNo], [SStatName], [RecCompany], [SendBy], [SendEBy], [RecBy], [Volume]) 
VALUES(@DC, @SUBNO, @SUBST, @IDNO, @MONEYS, @dPTNO, @TransferBy, @TransferTime, @CcNo, @DestNo, @PType, @PiecesNo, @Weight, @Freight, @CustomsPay, @Tariff, @OtherPayTax, @OtherPayNoTax, @ToPayment, @ToPaymentCurrency, @AgentPay, @AgentPayCurrency,@LadingDate,@AStatNo,@SendCHName,@CName,@ProdIdPay,@InsurancePay,@StatNo,@SectorNo,@SectorName,@HubName,@SendCustAddr,@SendECustAddr,@AStatName,@StatName,@HubNo,@SendCustNo,@SStatName,@RecCompany,@SendBy,@SendEBy,@RecBy,@Volume); 
IF @@Error <> 0 BEGIN SET @chk = 1 END ");
						sqldata.Add(new SqlParameter("@DAMOUNT", editData.Total ?? 0));
						sqldata.Add(new SqlParameter("@CAMOUNT", '0'));
						sqldata.Add(new SqlParameter("@PTNO", ladingData.LadingNo_Type));
						sqldata.Add(new SqlParameter("@REMARK", editData.Remark ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@VTYPE", "出口帳單"));
						sqldata.Add(new SqlParameter("@DC", "借"));
						sqldata.Add(new SqlParameter("@SUBNO", "1123.001"));
						sqldata.Add(new SqlParameter("@SUBST", "應收帳款"));
						sqldata.Add(new SqlParameter("@IDNO", editData.CustNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@MONEYS", editData.Total ?? 0));
						sqldata.Add(new SqlParameter("@dPTNO", ladingData.LadingNo_Type));
						sqldata.Add(new SqlParameter("@TransferBy", User.Identity.Name));
						sqldata.Add(new SqlParameter("@TransferTime", DateTime.Now));
						sqldata.Add(new SqlParameter("@CcNo", editData.CcNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@DestNo", editData.DestNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@PType", editData.Type ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@PiecesNo", editData.PiecesNo));
						sqldata.Add(new SqlParameter("@Weight", editData.Weight ?? 0));
						sqldata.Add(new SqlParameter("@Freight", editData.Freight ?? 0));
						sqldata.Add(new SqlParameter("@CustomsPay", editData.CustomsPay ?? 0));
						sqldata.Add(new SqlParameter("@Tariff", editData.Tariff ?? 0));
						sqldata.Add(new SqlParameter("@OtherPayTax", editData.OtherPayTax ?? 0));
						sqldata.Add(new SqlParameter("@OtherPayNoTax", editData.OtherPayNoTax ?? 0));
						sqldata.Add(new SqlParameter("@ToPayment", editData.ToPayment ?? 0));
						sqldata.Add(new SqlParameter("@ToPaymentCurrency", editData.ToPaymentCurrency ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@AgentPay", editData.AgentPay ?? 0));
						sqldata.Add(new SqlParameter("@AgentPayCurrency", editData.AgentPayCurrency ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@OKED", "否"));
						sqldata.Add(new SqlParameter("@LadingDate", editData.LadingDate));
						sqldata.Add(new SqlParameter("@AStatNo", ladingData.AStatNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendCHName", ladingData.SendCHName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@CName", editData.CName ?? ""));
						sqldata.Add(new SqlParameter("@ProdIdPay", editData.ProdIdPay ?? 0));
						sqldata.Add(new SqlParameter("@InsurancePay", editData.InsurancePay ?? 0));
						sqldata.Add(new SqlParameter("@StatNo", editData.StatNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SectorNo", transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorNo ?? (object)DBNull.Value)));
						sqldata.Add(new SqlParameter("@SectorName", transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorName ?? (object)DBNull.Value)));
						sqldata.Add(new SqlParameter("@HubName", hubName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendCustAddr", ladingData.SendCustAddr ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendECustAddr", ladingData.SendECustAddr ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@AStatName", ladingData.AStatName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@StatName", editData.StatName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@HubNo", ladingData.HubNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendCustNo", ladingData.SendCustNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SStatName ", ladingData.SStatName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@RecCompany ", ladingData.RecCompany ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendBy", ladingData.SendBy ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendEBy", ladingData.SendEBy ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@RecBy", ladingData.RecBy ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@Volume", ladingData.Volume ?? (object)DBNull.Value));

						sqlstr.Append(@"
IF @chk <> 0 BEGIN Rollback Transaction END 
ELSE BEGIN Commit Transaction END");
						sqlresult = dbFunctions.FinanceInsert(sqldata, sqlstr.ToString());
					}
					else
						editData.IsFinance = false;

					if (sqlresult != "OK")
					{
						result.Ok = DataModifyResultType.Faild;
						result.Message = sqlresult;
						trans.Rollback();
					}
					else
					{
						//以下系統自填
						editData.UpdateTime = DateTime.Now;
						editData.UpdateBy = User.Identity.Name;
						try
						{
							db.Entry(editData).State = EntityState.Modified;
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
		public ActionResult Delete(Export_Bill data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.Export_Bill.FirstOrDefault(x => x.ExBillNo == data.ExBillNo && x.IsDelete == false); ;
			if (deletedData != null)
			{
				if (deletedData.IsFinance == true)
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "已轉入會計系統，不允許刪除!";
				}
				else
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
			}
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "找不到資料!";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetGridJSON(Export_Bill data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var exportBill =
				from b in db.Export_Bill.Where(x => x.IsDelete == false)
				join u in db.SYS_User on b.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				join bl in db.Bill_Lading on b.LadingNo equals bl.LadingNo into ps2
				from bl in ps2.DefaultIfEmpty()
				select new
				{
					ExBillNo = b.ExBillNo,
					LadingNo = b.LadingNo,
					LadingNo_Type = bl.LadingNo_Type,
					LadingDate = b.LadingDate,
					StatNo = b.StatNo,
					StatName = b.StatName,
					CustNo = b.CustNo,
					CHName = b.CHName,
					CcNo = b.CcNo,
					DestNo = b.DestNo,
					CName = b.CName,
					Type = b.Type,
					PiecesNo = b.PiecesNo,
					Weight = b.Weight,
					Freight = b.Freight,
					CustomsPay = b.CustomsPay,
					Tariff = b.Tariff,
					ProdIdPay = b.ProdIdPay,
					InsurancePay = b.InsurancePay,
					OtherPayTax = b.OtherPayTax,
					OtherPayNoTax = b.OtherPayNoTax,
					ToPayment = b.ToPayment,
					ToPaymentCurrency = b.ToPaymentCurrency,
					AgentPay = b.AgentPay,
					AgentPayCurrency = b.AgentPayCurrency,
					Remark = b.Remark,
					Total = b.Total,
					IsFinance = b.IsFinance,
					FinanceBy = b.FinanceBy,
					FinanceTime = b.FinanceTime,
					CreateBy = u.UserName,
					CreateTime = b.CreateTime,
					UpdateBy = b.UpdateBy,
					UpdateTime = b.UpdateTime,
					DeletedBy = b.DeletedBy,
					DeletedTime = b.DeletedTime,
					IsDelete = b.IsDelete,

				};

			if (data.IsFinance != null)
				exportBill = exportBill.Where(x => x.IsFinance == data.IsFinance);
			if (data.LadingNo != null)
				exportBill = exportBill.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type != null)
				exportBill = exportBill.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.StatNo.IsNotEmpty())
				exportBill = exportBill.Where(x => x.StatNo == data.StatNo);
			if (data.StatName.IsNotEmpty())
				exportBill = exportBill.Where(x => x.StatName == data.StatName);
			if (data.CHName != null)
				exportBill = exportBill.Where(x => x.CHName.Contains(data.CHName));

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				exportBill = exportBill.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			int records = exportBill.Count();
			exportBill = exportBill.OrderBy(o => o.LadingNo).Skip((page - 1) * rows).Take(rows);
			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = exportBill,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult Finance(string[] ExBillNo)
		{
			var result = new ResultHelper();
			var repeat = false;
			using (var trans = db.Database.BeginTransaction())
			{
				foreach (var E in ExBillNo)
				{
					var data = db.Export_Bill.Where(x => x.ExBillNo == E && x.IsDelete == false).FirstOrDefault();
					if (data.IsFinance == true)
						repeat = true;
					data.IsFinance = true;
					data.FinanceTime = DateTime.Now;
					data.FinanceBy = User.Identity.Name;
					data.UpdateTime = DateTime.Now;
					data.UpdateBy = User.Identity.Name;
					db.Entry(data).State = EntityState.Modified;
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
						foreach (var eb in ExBillNo)
						{
							i++;
							var exBillData = db.Export_Bill.Where(x => x.ExBillNo == eb).FirstOrDefault();
							var ladingData = db.Bill_Lading.Where(x => x.LadingNo == exBillData.LadingNo).FirstOrDefault();
							var hubName = db.ORG_Hub.Where(x => x.HubNo == ladingData.HubNo).Select(x => x.HubName).FirstOrDefault();
							var transportationSData = db.TransportationS.Where(x => x.LadingNo == exBillData.LadingNo).FirstOrDefault();
							sqlstr.AppendFormat(@"
INSERT INTO TB_temp_Accno ( [DAMOUNT], [CAMOUNT], [PTNO], [REMARK], [VTYPE],[OKED])
VALUES(@DAMOUNT{0}, @CAMOUNT{1}, @PTNO{2}, @REMARK{3}, @VTYPE{4},@OKED{27}); 
IF @@Error <> 0 BEGIN SET @chk = 1 END
INSERT INTO TB_temp_Acc1  ( [DC], [SUBNO], [SUBST], [IDNO], [MONEYS], [PTNO], [TransferBy], [TransferTime] , [CcNo], [DestNo], [PType], [PiecesNo], [Weight], [Freight], [CustomsPay], [Tariff], [OtherPayTax], [OtherPayNoTax], [ToPayment], [ToPaymentCurrency], [AgentPay], [AgentPayCurrency],[LadingDate],[AStatNo],[SendCHName],[CName],[ProdIdPay],[InsurancePay],[StatNo] ,[SectorNo],[SectorName],[HubName],[SendCustAddr],[SendECustAddr],[AStatName],[StatName],[HubNo],[SendCustNo], [SStatName], [RecCompany], [SendBy], [SendEBy], [RecBy], [Volume]) 
VALUES(@DC{5}, @SUBNO{6}, @SUBST{7}, @IDNO{8}, @MONEYS{9}, @dPTNO{10}, @TransferBy{11}, @TransferTime{12}, @CcNo{13}, @DestNo{14}, @PType{15}, @PiecesNo{16}, @Weight{17}, @Freight{18}, @CustomsPay{19}, @Tariff{20}, @OtherPayTax{21}, @OtherPayNoTax{22}, @ToPayment{23}, @ToPaymentCurrency{24}, @AgentPay{25}, @AgentPayCurrency{26},@LadingDate{28},@AStatNo{29},@SendCHName{30},@CName{31},@ProdIdPay{32},@InsurancePay{33},@StatNo{34},@SectorNo{35},@SectorName{36},@HubName{37},@SendCustAddr{38},@SendECustAddr{39},@AStatName{40},@StatName{41},@HubNo{42},@SendCustNo{43},@SStatName{44},@RecCompany{45},@SendBy{46},@SendEBy{47},@RecBy{48},@Volume{49}); 
IF @@Error <> 0 BEGIN SET @chk = 1 END ", i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i, i);

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
							sqldata.Add(new SqlParameter("@SendCustNo" + i, ladingData.SendCustNo ?? (object)DBNull.Value));
							sqldata.Add(new SqlParameter("@SStatName " + i, ladingData.SStatName ?? (object)DBNull.Value));
							sqldata.Add(new SqlParameter("@RecCompany " + i, ladingData.RecCompany ?? (object)DBNull.Value));
							sqldata.Add(new SqlParameter("@SendBy" + i, ladingData.SendBy ?? (object)DBNull.Value));
							sqldata.Add(new SqlParameter("@SendEBy" + i, ladingData.SendEBy ?? (object)DBNull.Value));
							sqldata.Add(new SqlParameter("@RecBy" + i, ladingData.RecBy ?? (object)DBNull.Value));
							sqldata.Add(new SqlParameter("@Volume" + i, ladingData.Volume ?? (object)DBNull.Value));
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
