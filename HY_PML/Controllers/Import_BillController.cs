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
	public class Import_BillController : Controller
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
			ViewBag.Title = "進口帳單";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "Import_Bill";
			ViewBag.FormCustomJsNew =
				"$('#LadingNo').textbox('readonly', false); " +
				"$('#LadingDate').textbox('readonly', false); ";
			ViewBag.FormCustomJsEdit =
				"$('#LadingNo').textbox('readonly', true); " +
				"$('#LadingDate').textbox('readonly', true); " +
				"$('.editlock').prop('readonly', true);" +
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
		public ActionResult Add(Import_Bill data)
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
					var saveData = new Import_Bill();
					saveData.ImBillNo = "IM-" + data.LadingNo;
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
					saveData.OtherPayTax = data.OtherPayTax;
					saveData.OtherPayNoTax = data.OtherPayNoTax;
					saveData.ToPayment = data.ToPayment;
					saveData.ToPaymentCurrency = data.ToPaymentCurrency;
					saveData.AgentPay = data.AgentPay;
					saveData.AgentPayCurrency = data.AgentPayCurrency;
					saveData.Remark = data.Remark;
					saveData.Total = data.Freight + data.CustomsPay + data.CustomsPay + data.Tariff + data.OtherPayTax + data.OtherPayNoTax + data.ToPayment + data.AgentPay;

					//以下系統自填
					saveData.CreateTime = DateTime.Now;
					saveData.CreateBy = User.Identity.Name;
					saveData.IsDelete = false;
					db.Import_Bill.Add(saveData);
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
		public ActionResult Edit(Import_Bill data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var dbFunctions = new DBFunctions();
			List<SqlParameter> sqldata = new List<SqlParameter>();
			StringBuilder sqlstr = new StringBuilder();
			var sqlresult = "OK";

			var editData = db.Import_Bill.FirstOrDefault(x => x.ImBillNo == data.ImBillNo && x.IsDelete == false);
			using (var trans = db.Database.BeginTransaction())
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
					editData.OtherPayTax = data.OtherPayTax;
					editData.OtherPayNoTax = data.OtherPayNoTax;
					editData.ToPayment = data.ToPayment;
					editData.ToPaymentCurrency = data.ToPaymentCurrency;
					editData.AgentPay = data.AgentPay;
					editData.AgentPayCurrency = data.AgentPayCurrency;
					editData.Remark = data.Remark;
					editData.Total = data.Freight ?? 0 + data.CustomsPay ?? 0 + data.Tariff ?? 0 + data.OtherPayTax ?? 0 + data.OtherPayNoTax ?? 0 + data.ToPayment ?? 0 + data.AgentPay ?? 0;
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
INSERT INTO TB_temp_Accno ( [DAMOUNT], [CAMOUNT], [PTNO], [REMARK], [VTYPE] ,[OKED]) VALUES( @DAMOUNT ,@CAMOUNT ,@PTNO ,@REMARK, @VTYPE ,@OKED); 
IF @@Error <> 0 BEGIN SET @chk = 1 END
INSERT INTO TB_temp_Acc1 ( [DC], [SUBNO], [SUBST], [IDNO], [MONEYS], [PTNO], [TransferBy], [TransferTime], [CcNo], [DestNo], [PType], [PiecesNo], [Weight],[Freight], [CustomsPay], [Tariff], [OtherPayTax], [OtherPayNoTax], [ToPayment], [ToPaymentCurrency], [AgentPay], [AgentPayCurrency],[LadingDate],[AStatNo],[SendCHName],[CName],[StatNo],[SectorNo],[SectorName],[HubName],[SendCustAddr],[SendECustAddr],[AStatName],[StatName],[HubNo]) 
VALUES(@DC, @SUBNO, @SUBST, @IDNO, @MONEYS, @dPTNO, @TransferBy, @TransferTime, @CcNo, @DestNo, @PType, @PiecesNo, @Weight, @Freight, @CustomsPay, @Tariff, @OtherPayTax, @OtherPayNoTax, @ToPayment, @ToPaymentCurrency, @AgentPay, @AgentPayCurrency,@LadingDate,@AStatNo,@SendCHName,@CName,@StatNo,@SectorNo,@SectorName,@HubName,@SendCustAddr,@SendECustAddr,@AStatName,@StatName,@HubNo); 
IF @@Error <> 0 BEGIN SET @chk = 1 END");

						sqldata.Add(new SqlParameter("@DAMOUNT", editData.Total ?? 0));
						sqldata.Add(new SqlParameter("@CAMOUNT", '0'));
						sqldata.Add(new SqlParameter("@PTNO", ladingData.LadingNo_Type));
						sqldata.Add(new SqlParameter("@REMARK", editData.Remark ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@VTYPE", "進口帳單"));
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
						sqldata.Add(new SqlParameter("@CName", editData.CName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@StatNo", editData.StatNo ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SectorNo", transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorNo ?? (object)DBNull.Value)));
						sqldata.Add(new SqlParameter("@SectorName", transportationSData == null ? (object)DBNull.Value : (transportationSData.SSectorName ?? (object)DBNull.Value)));
						sqldata.Add(new SqlParameter("@HubName", hubName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendCustAddr", ladingData.SendCHName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@SendECustAddr", ladingData.SendECustAddr ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@AStatName", ladingData.AStatName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@StatName", editData.StatName ?? (object)DBNull.Value));
						sqldata.Add(new SqlParameter("@HubNo", ladingData.HubNo ?? (object)DBNull.Value));

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
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Delete(Import_Bill data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.Import_Bill.FirstOrDefault(x => x.ImBillNo == data.ImBillNo && x.IsDelete == false); ;
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
		public ActionResult GetGridJSON(Import_Bill data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var importBill =
				from b in db.Import_Bill.Where(x => x.IsDelete == false)
				join u in db.SYS_User on b.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				join bl in db.Bill_Lading on b.LadingNo equals bl.LadingNo into ps2
				from bl in ps2.DefaultIfEmpty()
				select new
				{
					ImBillNo = b.ImBillNo,
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
				importBill = importBill.Where(x => x.IsFinance == data.IsFinance);
			if (data.LadingNo != null)
				importBill = importBill.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type != null)
				importBill = importBill.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.StatNo.IsNotEmpty())
				importBill = importBill.Where(x => x.StatNo == data.StatNo);
			if (data.StatName.IsNotEmpty())
				importBill = importBill.Where(x => x.StatName == data.StatName);
			if (data.CHName != null)
				importBill = importBill.Where(x => x.CHName.Contains(data.CHName));

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				importBill = importBill.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			int records = importBill.Count();
			importBill = importBill.OrderBy(o => o.LadingNo).Skip((page - 1) * rows).Take(rows);
			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = importBill,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult Finance(string[] ImBillNo)
		{
			var result = new ResultHelper();
			var repeat = false;
			using (var trans = db.Database.BeginTransaction())
			{
				foreach (var i in ImBillNo)
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
						foreach (var ib in ImBillNo)
						{
							i++;
							var imBillData = db.Import_Bill.Where(x => x.ImBillNo == ib).FirstOrDefault();
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