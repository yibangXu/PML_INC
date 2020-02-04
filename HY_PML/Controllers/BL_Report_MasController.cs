using ClosedXML.Excel;
using HY_PML.helper;
using HY_PML.Models;
using HY_PML.Models.Export;
using HY_PML.Models.XSLXHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class BL_Report_MasController : Controller
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
			var statNoSession = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
			var areaIDSession = db.ORG_Stat.FirstOrDefault(x => x.StatNo == statNoSession).AreaID;
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "提單報表";
			ViewBag.ControllerName = "BL_Report_Mas";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.FormCustomJsNew = $@"$('#addLading').show();$('.masAdd').show();$('.isMas').textbox('setValue', 'true');$('#SAreaID').textbox('setValue', '{areaIDSession}');";
			ViewBag.FormCustomJsEdit = @"$('#addLading').hide();$('.masAdd').hide()";

			//子表
			ViewBag.Title2 = "";
			ViewBag.ControllerName2 = "BL_Report_Mas";
			ViewBag.AddFunc2 = "AddDetail";
			ViewBag.EditFunc2 = "EditDetail";
			ViewBag.DelFunc2 = "DeleteDetail";
			ViewBag.FormPartialName2 = "_ElementInForm2";
			ViewBag.FormCustomJsNew2 = @"$('.ladingEdit').hide();$('#addLading').show();
$('#dReportNo').textbox('setValue', row.ReportNo);
$('#dHubName').textbox('setValue', row.HubName);
$('#dSStatNo').textbox('setValue', row.SStatNo);
$('.isMas').textbox('setValue', 'false');";
			ViewBag.FormCustomJsEdit2 = "$('.ladingEdit').show();$('#addLadingDtl').hide()";

			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		[Authorize]
		public ActionResult getMultiCbjqGrid(string gridId, string formId, string dlgId, string postUrl)
		{
			ViewBag.gridId = gridId;
			ViewBag.formId = formId;
			ViewBag.dlgId = dlgId;
			ViewBag.postUrl = postUrl;
			return View();
		}

		[Authorize]
		public ActionResult Add(BL_Report_Mas data, string multiList)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			if (db.Database.Exists())
			{
				result.Ok = DataModifyResultType.Warning;
				result.Message = "資料庫已開啟連線";
			}

			using (var trans = db.Database.BeginTransaction())
			{
				try
				{
					var userRecord = new BL_Report_Mas();
					userRecord.MasterNo = data.MasterNo;
					userRecord.FlightNo = data.FlightNo;
					userRecord.HubNo = data.HubNo;
					userRecord.ReportID = data.ReportID;
					userRecord.SStatNo = data.SStatNo;
					userRecord.AStatNo = data.AStatNo;
					userRecord.Remark = data.Remark;

					//編流水號
					{
						var reportCode = db.ORG_Report_Mgmt.Where(x => x.ID == data.ReportID).Select(x => x.ReportCode).FirstOrDefault();
						var prefix = DateTime.Now.ToString("yyyyMMdd") + reportCode;
						var lastReportNo = db.BL_Report_Mas.Where(x => x.ReportNo.Contains(prefix) && x.SStatNo == data.SStatNo).OrderByDescending(x => x.ReportNo).Select(x => x.ReportNo).FirstOrDefault();
						if (lastReportNo != null)
							userRecord.ReportNo = prefix + (Convert.ToInt32(lastReportNo.Substring(11, 1)) + 1).ToString() + "-" + data.SStatNo;
						else
							userRecord.ReportNo = prefix + "1" + "-" + data.SStatNo;
					}
					//以下系統自填
					userRecord.CreatedDate = DateTime.Now;
					userRecord.CreatedBy = User.Identity.Name;
					userRecord.IsDelete = false;

					db.BL_Report_Mas.Add(userRecord);
					db.SaveChanges();
					result.Ok = DataModifyResultType.Warning;
					result.Message = "提單報表主檔儲存完成";
					if (multiList != null)
					{
						var ladings = multiList.Split('|').Select(lading => lading.Split(','));
						int ladingSNo = 0;

						//報表子檔
						var dtls = ladings.Select((l, index) => new BL_Report_Dtl()
						{
							ReportNo = userRecord.ReportNo,
							SNo = index + 1,
							LadingNo = l[0],
							LadingSNo = int.TryParse(l[1], out ladingSNo) == true ? ladingSNo : 1,
							CreatedDate = DateTime.Now,
							CreatedBy = User.Identity.Name,
							IsDelete = false,
						});
						db.BL_Report_Dtl.AddRange(dtls);

						//回填提單
						var isBackfill = db.ORG_Report_Mgmt.Where(x => x.ID == userRecord.ReportID).Select(x => x.IsBackfill).FirstOrDefault();
						if (isBackfill == true)
							foreach (var l in ladings)
							{
								var ladingNo = l[0];
								var backfill = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == ladingNo);
								backfill.WarehouseRNo = userRecord.ReportNo;
								backfill.WarehouseRDate = DateTime.Now;
								backfill.UpdateBy = User.Identity.Name;
								backfill.UpdateTime = DateTime.Now;
								db.Entry(backfill).State = EntityState.Modified;
							}
					}

					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "提單報表子檔儲存完成";
				}
				catch (DbEntityValidationException dbve)
				{
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					result.Message = dbve.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
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

		[Authorize]
		public ActionResult AddDetail(BL_Report_Dtl data, string multiList)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				try
				{

					var ladings = multiList.Split('|').Select(lading => lading.Split(','));
					string ErrorString = "";
					foreach (var l in ladings)
					{
						int ladingSNoCheck = 0;
						int.TryParse(l[1], out ladingSNoCheck);
						string ladingNoCheck = l[0];
						var duplicated = db.BL_Report_Dtl.Any(x => x.ReportNo == data.ReportNo && x.LadingNo == ladingNoCheck && x.LadingSNo == ladingSNoCheck);
						if (duplicated)
							ErrorString = ErrorString + "編號" + l[2] + "　";
					}
					if (ErrorString != "")
					{
						trans.Rollback();
						result.Ok = DataModifyResultType.Faild;
						result.Message = "報表已存在下列提單資料，勿重復加入：" + ErrorString;
					}
					else
					{
						var seq = db.BL_Report_Dtl.Where(x => x.ReportNo == data.ReportNo).OrderByDescending(x => x.SNo).Select(x => x.SNo).FirstOrDefault();
						int ladingSNo = 0;

						//報表子檔
						var dtls = ladings.Select((l, index) => new BL_Report_Dtl()
						{
							ReportNo = data.ReportNo,
							SNo = index + seq + 1,
							LadingNo = l[0],
							LadingSNo = int.TryParse(l[1], out ladingSNo) == true ? ladingSNo : 1,
							CreatedDate = DateTime.Now,
							CreatedBy = User.Identity.Name,
							IsDelete = false,
						});
						db.BL_Report_Dtl.AddRange(dtls);

						//回填提單
						var reportID = db.BL_Report_Mas.FirstOrDefault(x => x.ReportNo == data.ReportNo).ReportID;
						var isBackfill = db.ORG_Report_Mgmt.Where(x => x.ID == reportID).Select(x => x.IsBackfill).FirstOrDefault();
						if (isBackfill == true)
							foreach (var l in ladings)
							{
								var ladingNo = l[0];
								var backfill = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == ladingNo);
								backfill.WarehouseRNo = data.ReportNo;
								backfill.WarehouseRDate = DateTime.Now;
								backfill.UpdateBy = User.Identity.Name;
								backfill.UpdateTime = DateTime.Now;
								db.Entry(backfill).State = EntityState.Modified;
							}

						db.SaveChanges();
						trans.Commit();
						result.Ok = DataModifyResultType.Success;
						result.Message = "OK";
					}
				}
				catch (Exception e)
				{
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					result.Message = (e as SqlException).Message;
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Edit(BL_Report_Mas data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.BL_Report_Mas.FirstOrDefault(x => x.ReportNo == data.ReportNo);
				if (userRecord != null)
				{
					userRecord.MasterNo = data.MasterNo;
					userRecord.FlightNo = data.FlightNo;
					userRecord.Remark = data.Remark;

					//以下系統自填
					userRecord.UpdatedDate = DateTime.Now;
					userRecord.UpdatedBy = User.Identity.Name;
					db.Entry(userRecord).State = EntityState.Modified;

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
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
					trans.Rollback();
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult EditDetail(BL_Report_Dtl data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Delete(BL_Report_Mas data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.BL_Report_Mas.FirstOrDefault(x => x.ReportNo == data.ReportNo);
				var dtl = db.BL_Report_Dtl.Where(x => x.ReportNo == data.ReportNo);
				var isBackfill = db.ORG_Report_Mgmt.Where(x => x.ID == userRecord.ReportID).Select(x => x.IsBackfill).FirstOrDefault();
				foreach (var d in dtl)
				{
					d.IsDelete = true;
					d.DeletedBy = User.Identity.Name;
					d.DeletedDate = DateTime.Now;
					db.Entry(d).State = EntityState.Modified;
				}
				if (isBackfill == true)
					foreach (var d in dtl)
					{
						var backfill = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == d.LadingNo);
						backfill.WarehouseRNo = null;
						backfill.WarehouseRDate = null;
						backfill.UpdateBy = User.Identity.Name;
						backfill.UpdateTime = DateTime.Now;
						db.Entry(backfill).State = EntityState.Modified;
					}

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
						trans.Commit();
					}
					catch (Exception e)
					{
						result.Ok = DataModifyResultType.Faild;
						result.Message = e.Message;
						trans.Rollback();
					}
				}
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
					trans.Rollback();
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult DeleteDetail(BL_Report_Dtl data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var userRecord = db.BL_Report_Dtl.FirstOrDefault(x => x.ReportNo == data.ReportNo && x.SNo == data.SNo);
			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				if (userRecord != null)
				{
					//以下系統自填
					userRecord.DeletedDate = DateTime.Now;
					userRecord.DeletedBy = User.Identity.Name;
					userRecord.IsDelete = true;

					//回填提單
					var reportID = db.BL_Report_Mas.FirstOrDefault(x => x.ReportNo == data.ReportNo).ReportID;
					var isBackfill = db.ORG_Report_Mgmt.Where(x => x.ID == reportID).Select(x => x.IsBackfill).FirstOrDefault();
					if (isBackfill == true)
					{
						var backfill = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == data.LadingNo);
						backfill.WarehouseRNo = null;
						backfill.WarehouseRDate = null;
						backfill.UpdateBy = User.Identity.Name;
						backfill.UpdateTime = DateTime.Now;
						db.Entry(backfill).State = EntityState.Modified;
					}
					try
					{
						db.Entry(userRecord).State = EntityState.Modified;
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
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetGridJSON(BL_Report_Mas data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
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

			var billReport =
				from b in db.BL_Report_Mas.Where(x => x.IsDelete == false)
				join s in db.ORG_Stat.Where(x => x.IsDelete == false)
				on b.SStatNo equals s.StatNo into ps
				from s in ps.DefaultIfEmpty()
				join a in db.ORG_Stat.Where(x => x.IsDelete == false)
			   on b.SStatNo equals a.StatNo into ps1
				from a in ps1.DefaultIfEmpty()
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
			   on b.HubNo equals h.HubNo into ps2
				from h in ps2.DefaultIfEmpty()
				join r in db.ORG_Report_Mgmt.Where(x => x.IsDelete == false)
				on b.ReportID equals r.ID into ps3
				from r in ps3.DefaultIfEmpty()
				join u in db.SYS_User on b.DeletedBy equals u.Account into ps4
				from u in ps4.DefaultIfEmpty()
				join cuser in db.SYS_User on b.CreatedBy equals cuser.Account into ps5
				from cuser in ps5.DefaultIfEmpty()
				select new
				{
					ReportNo = b.ReportNo,
					MasterNo = b.MasterNo,
					FlightNo = b.FlightNo,
					HubNo = b.HubNo,
					HubCode = h == null ? null : h.HubCode,
					HubName = h == null ? null : h.HubName,
					ReportID = b.ReportID,
					ReportCName = r == null ? null : r.ReportCName,
					SStatNo = b.SStatNo,
					SStatName = s == null ? null : s.StatName,
					AStatNo = b.AStatNo,
					AStatName = a == null ? null : a.StatName,
					Remark = b.Remark,
					CreatedBy = cuser.UserName,
					CreatedDate = b.CreatedDate,
					UpdatedBy = b.UpdatedBy,
					UpdatedDate = b.UpdatedDate,
					DeletedBy = u.UserName,
					DeletedDate = b.DeletedDate,
					IsDelete = b.IsDelete
				};

			if (data.ReportNo.IsNotEmpty())
				billReport = billReport.Where(x => x.ReportNo.Contains(data.ReportNo));
			if (data.MasterNo.IsNotEmpty())
				billReport = billReport.Where(x => x.MasterNo.Contains(data.MasterNo));
			if (data.FlightNo.IsNotEmpty())
				billReport = billReport.Where(x => x.FlightNo == data.FlightNo);
			if (data.HubName.IsNotEmpty())
				billReport = billReport.Where(x => x.HubName == data.HubName);
			if (data.HubName.IsNotEmpty())
				billReport = billReport.Where(x => x.HubName == data.HubName);
			if (data.ReportCName.IsNotEmpty())
				billReport = billReport.Where(x => x.ReportCName == data.ReportCName);
			if (data.SStatName.IsNotEmpty())
				billReport = billReport.Where(x => x.SStatName == data.SStatName);
			if (data.AStatName.IsNotEmpty())
				billReport = billReport.Where(x => x.AStatName == data.AStatName);
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				billReport = billReport.Where(x => DbFunctions.TruncateTime(x.CreatedDate).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreatedDate).Value.CompareTo(sDate) >= 0);
			}

			int records = billReport.Count();
			billReport = billReport.OrderByDescending(o => o.ReportNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = billReport,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetLadingGridJSON(Bill_Lading data, int page = 1, int rows = 100, DateTime? start_date = null, DateTime? end_date = null)
		{
			var statNoList = new List<string>();
			var statNoSession = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
			var areaIDSession = db.ORG_Stat.FirstOrDefault(x => x.StatNo == statNoSession).AreaID;

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

			var billLadingData =
								from m in db.DeclCust_Main.Where(x => x.IsDelete == false)
								join b in db.Bill_Lading.Where(x => x.IsDelete == false)
								on m.LadingNo equals b.LadingNo into ps
								from b in ps.DefaultIfEmpty()
								join h in db.ORG_Hub
								on b.HubNo equals h.HubNo into ps1
								from h in ps1.DefaultIfEmpty()
								join s in db.ORG_Stat.Where(x => x.IsDelete == false)
								on b.SStatNo equals s.StatNo into ps2
								from s in ps2.DefaultIfEmpty()
								where statNoList.Contains(b.SStatNo)
								select new
								{
									sNo = m.sNo,
									LadingNo = b.LadingNo,
									LadingNo_Type = b.LadingNo_Type,
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
									RecCountry = b.RecCountry,
									RecPostDist = b.RecPostDist,
									SectorNo = b.SectorNo,
									SectorName = b.SectorName,
									SStatNo = b.SStatNo,
									SStatName = b.SStatName,
									SAreaID = s.AreaID,
									AStatNo = b.AStatNo,
									AStatName = b.AStatName,
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
									IsConfirm = b.IsConfirm,
									ConfirmBy = b.ConfirmBy,
									IsCheck = b.IsCheck,
									CheckBy = b.CheckBy,
									DtlsNo = m.sNo,
									DtlCustType = m.CustType,
									DtlFlight = m.Flight,
									DtlBagNo = m.BagNo,
									DtlCleCusCode = m.CleCusCode,
									DtlCusCoode = m.CusCoode,
									DtlProductNo = m.ProductNo,
									DtlProductName = m.ProductName,
									DtlProductEName = m.ProductEName,
									DtlCountry = m.Country,
									DtlType = m.Type,
									DtlHSNo = m.HSNo,
									DtlQty = m.Qty,
									DtlUnit = m.Unit,
									DtlGrossWeight = m.GrossWeight,
									DtlWeight = m.Weight,
									DtlPrice = m.Price,
									DtlTotal = m.Total,
									DtlCurrency = m.Currency,
									DtlPcs = m.Pcs,
									DtlPcsNo = m.PcsNo,
									DtlRemark = m.Remark,
									DtlLength = m.Length,
									DtlWidth = m.Width,
									DtlHeight = m.Height
								};

			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				billLadingData = billLadingData.Where(x => DbFunctions.TruncateTime(x.LadingDate).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.LadingDate).Value.CompareTo(sDate) >= 0);
			}
			if (data.WarehouseRNo.IsNotEmpty())
				billLadingData = billLadingData.Where(x => x.WarehouseRNo == data.WarehouseRNo);
			if (data.LadingNo.IsNotEmpty())
				billLadingData = billLadingData.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type.IsNotEmpty())
				billLadingData = billLadingData.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.HubName.IsNotEmpty())
				billLadingData = billLadingData.Where(x => x.HubName == data.HubName);
			if (User.Identity.Name == "hyAdmin" || statNoSession == "hyAdmin" || statNoSession == "TNNCON")
			{
				billLadingData = billLadingData.Where(x => x.SAreaID == areaIDSession);
				if (data.StatNo != "TNNCON")
					billLadingData = billLadingData.Where(x => x.SStatNo == data.StatNo);
			}
			if (data.SStatNo.IsNotEmpty())
				billLadingData = billLadingData.Where(x => x.SStatNo == data.SStatNo);
			if (data.AStatNo.IsNotEmpty())
				billLadingData = billLadingData.Where(x => x.AStatName == data.AStatName);

			var billLading = billLadingData.ToList().OrderBy(i => i.LadingDate).Select((i, index) => new
			{
				RowNumber = index + 1,
				sNo = i.sNo,
				LadingNo = i.LadingNo,
				WarehouseRNo = i.WarehouseRNo,
				HubNo = i.HubNo,
				HubName = i.HubName,
				WarehouseRDate = i.WarehouseRDate,
				TransferNo = i.TransferNo,
				LadingDate = i.LadingDate,
				SendCustNo = i.SendCustNo,
				SendCHName = i.SendCHName,
				SendPhone = i.SendPhone,
				SendBy = i.SendBy,
				SendCustAddr = i.SendCustAddr,
				RecPhone = i.RecPhone,
				RecBy = i.RecBy,
				RecCompany = i.RecCompany,
				RecCustCHName = i.RecCustCHName,
				RecChAddr = i.RecChAddr,
				RecInvNo = i.RecInvNo,
				RecCountry = i.RecCountry,
				RecPostDist = i.RecPostDist,
				SectorNo = i.SectorNo,
				SectorName = i.SectorName,
				SStatNo = i.SStatNo,
				SStatName = i.SStatName,
				AStatNo = i.AStatNo,
				AStatName = i.AStatName,
				DestNo = i.DestNo,
				CName = i.CName,
				Type = i.Type,
				ProductNo = i.ProductNo,
				ProductName = i.ProductName,
				PiecesNo = i.PiecesNo,
				Qty = i.Qty,
				Weight = i.Weight,
				Volume = i.Volume,
				CcNo = i.CcNo,
				PayCustNo = i.PayCustNo,
				PayCustCHName = i.PayCustCHName,
				ToPayment = i.ToPayment,
				ToPaymentCurrency = i.ToPaymentCurrency,
				AgentPay = i.AgentPay,
				AgentPayCurrency = i.AgentPayCurrency,
				Remark = i.Remark,
				IsConfirm = i.IsConfirm,
				ConfirmBy = i.ConfirmBy,
				IsCheck = i.IsCheck,
				CheckBy = i.CheckBy,
				DtlsNo = i.DtlsNo,
				DtlCustType = i.DtlCustType,
				DtlFlight = i.DtlFlight,
				DtlBagNo = i.DtlBagNo,
				DtlCleCusCode = i.DtlCleCusCode,
				DtlCusCoode = i.DtlCusCoode,
				DtlProductNo = i.DtlProductNo,
				DtlProductName = i.DtlProductName,
				DtlProductEName = i.DtlProductEName,
				DtlCountry = i.DtlCountry,
				DtlType = i.DtlType,
				DtlHSNo = i.DtlHSNo,
				DtlQty = i.DtlQty,
				DtlUnit = i.DtlUnit,
				DtlGrossWeight = i.DtlGrossWeight,
				DtlWeight = i.DtlWeight,
				DtlPrice = i.DtlPrice,
				DtlTotal = i.DtlTotal,
				DtlCurrency = i.DtlCurrency,
				DtlPcs = i.DtlPcs,
				DtlPcsNo = i.DtlPcsNo,
				DtlRemark = i.DtlRemark,
				DtlLength = i.DtlLength,
				DtlWidth = i.DtlWidth,
				DtlHeight = i.DtlHeight
			});

			int records = billLading.Count();
			billLading = billLading.OrderBy(o => o.LadingDate).Skip((page - 1) * rows).Take(rows);

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

		public ActionResult Report(string id)
		{
			var datas = new List<BL_Report>();
			var itemDatas = new List<BL_Report_Items>();
			var Items =
				from brd in db.BL_Report_Dtl.Where(x => x.ReportNo == id && x.IsDelete == false)
				join m in db.DeclCust_Main.Where(x => x.IsDelete == false)
				on new { brd.LadingNo, LadingSNo = brd.LadingSNo } equals new { m.LadingNo, LadingSNo = m.sNo } into ps
				from m in ps.DefaultIfEmpty()

				join b in db.Bill_Lading.Where(x => x.IsDelete == false)
				on brd.LadingNo equals b.LadingNo into ps1
				from b in ps1.DefaultIfEmpty()

				join d in db.ORG_Dest.Where(x => x.IsDelete == false)
				on b.DestNo equals d.DestNo into ps2
				from d in ps2.DefaultIfEmpty()

				join br in db.BL_Report_Mas
				on brd.ReportNo equals br.ReportNo into ps3
				from br in ps3.DefaultIfEmpty()

				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on br.HubNo equals h.HubNo into ps4
				from h in ps4.DefaultIfEmpty()

				join c in db.ORG_Cust.Where(x => x.IsDelete == false)
				on b.SendCustNo equals c.CustNo into ps5
				from c in ps5.DefaultIfEmpty()
				select new BL_Report_Items
				{
					ReportNo = brd.ReportNo,
					SNo = brd.SNo,
					LadingNo = b.LadingNo_Type,
					LadingSNo = brd.LadingSNo,
					SendCHName = b.SendCHName,
					SendBy = b.SendBy,
					SendPhone = b.SendPhone,
					SendCustAddr = b.SendCustAddr,
					SendEName1 = c == null ? null : c.CustEName1,
					SendENAddr1 = c == null ? null : c.CustENAddr1,
					RecCompany = b.RecCompany,
					RecBy = b.RecBy,
					RecPhone = b.RecPhone,
					RecChAddr = b.RecChAddr,
					RecPostDist = b.RecPostDist,
					RecCity = b.RecCity,
					RecState = b.RecState,
					DestNo = d.DestNo,
					CName = d.CName,
					Cost = b.Cost == null ? 0 : b.Cost.Value,
					CcNo = b.CcNo,
					PayCustCHName = b.PayCustCHName,
					ToPayment = b.ToPayment == null ? 0 : b.ToPayment.Value,
					Freight = b.Freight == null ? 0 : b.Freight.Value,
					Type = b.Type,
					DtlBagNo = m.BagNo,
					DtlType = m.Type,
					DtlProductName = m.ProductName,
					DtlPcs = m.Pcs == null ? 0 : m.Pcs.Value,
					DtlPrice = m.Price,
					DtlQty = m.Qty,
					DtlWeight = m.Weight == null ? 0 : m.Weight.Value,
					DtlLength = m.Length == null ? 0 : m.Length.Value,
					DtlWidth = m.Width == null ? 0 : m.Width.Value,
					DtlHeight = m.Height == null ? 0 : m.Height.Value,
					DtlGrossWeight = m.GrossWeight == null ? 0 : m.GrossWeight.Value,
					DtlHSNo = m.HSNo,
					Remark = b.Remark,
					Remark2 = b.Remark2,
					sStatNo = b.SStatNo,
					aStatNo = b.AStatNo,
				};
			itemDatas.AddRange(Items);

			var mas = from b in db.BL_Report_Mas.Where(x => x.ReportNo == id && x.IsDelete == false)
					  join s in db.ORG_Stat.Where(x => x.IsDelete == false)
					  on b.SStatNo equals s.StatNo into ps
					  from s in ps.DefaultIfEmpty()
					  join a in db.ORG_Stat.Where(x => x.IsDelete == false)
					 on b.SStatNo equals a.StatNo into ps1
					  from a in ps1.DefaultIfEmpty()
					  join h in db.ORG_Hub.Where(x => x.IsDelete == false)
					 on b.HubNo equals h.HubNo into ps2
					  from h in ps2.DefaultIfEmpty()
					  join r in db.ORG_Report_Mgmt.Where(x => x.IsDelete == false)
					  on b.ReportID equals r.ID into ps3
					  from r in ps3.DefaultIfEmpty()
					  select new
					  {
						  ReportNo = b.ReportNo,
						  CreatedDate = b.CreatedDate,
						  MasterNo = b.MasterNo,
						  FlightNo = b.FlightNo,
						  HubNo = b.HubNo,
						  HubCode = h == null ? null : h.HubCode,
						  HubName = h == null ? null : h.HubName,
						  ReportID = b.ReportID,
						  ReportCName = r == null ? null : r.ReportCName,
						  SStatNo = b.SStatNo,
						  SStatName = s == null ? null : s.StatName,
						  AStatNo = b.AStatNo,
						  AStatName = a == null ? null : a.StatName,
					  };
			var MasData = mas.FirstOrDefault();

			var tData = new BL_Report()
			{
				ReportNo = MasData.ReportNo,
				CreatedDate = MasData.CreatedDate,
				MasterNo = MasData.MasterNo,
				FlightNo = MasData.FlightNo,
				HubNo = MasData.HubNo,
				HubCode = MasData.HubCode,
				HubName = MasData.HubName,
				ReportID = MasData.ReportID,
				ReportCName = MasData.ReportCName,
				SStatNo = MasData.SStatNo,
				SStatName = MasData.SStatName,
				AStatNo = MasData.AStatNo,
				AStatName = MasData.AStatName,
				Items = itemDatas,
			};
			datas.Add(tData);

			ViewData.Model = datas;
			return View();
		}

		public void Excel(string id)
		{
			#region 資料整理
			var datas = new List<BL_Report>();
			var itemDatas = new List<BL_Report_Items>();
			var Items =
				from brd in db.BL_Report_Dtl.Where(x => x.ReportNo == id && x.IsDelete == false)
				join m in db.DeclCust_Main.Where(x => x.IsDelete == false)
				on new { brd.LadingNo, LadingSNo = brd.LadingSNo } equals new { m.LadingNo, LadingSNo = m.sNo } into ps
				from m in ps.DefaultIfEmpty()

				join b in db.Bill_Lading.Where(x => x.IsDelete == false)
				on brd.LadingNo equals b.LadingNo into ps1
				from b in ps1.DefaultIfEmpty()

				join d in db.ORG_Dest.Where(x => x.IsDelete == false)
				on b.DestNo equals d.DestNo into ps2
				from d in ps2.DefaultIfEmpty()

				join br in db.BL_Report_Mas
				on brd.ReportNo equals br.ReportNo into ps3
				from br in ps3.DefaultIfEmpty()

				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on br.HubNo equals h.HubNo into ps4
				from h in ps4.DefaultIfEmpty()

				join c in db.ORG_Cust.Where(x => x.IsDelete == false)
				on b.SendCustNo equals c.CustNo into ps5
				from c in ps5.DefaultIfEmpty()
				select new BL_Report_Items
				{
					ReportNo = brd.ReportNo,
					SNo = brd.SNo,
					LadingNo = b.LadingNo_Type,
					LadingSNo = brd.LadingSNo,
					SendCHName = b.SendCHName,
					SendBy = b.SendBy,
					SendPhone = b.SendPhone,
					SendCustAddr = b.SendCustAddr,
					SendEName1 = c == null ? null : c.CustEName1,
					SendENAddr1 = c == null ? null : c.CustENAddr1,
					RecCompany = b.RecCompany,
					RecBy = b.RecBy,
					RecPhone = b.RecPhone,
					RecChAddr = b.RecChAddr,
					RecPostDist = b.RecPostDist,
					RecCity = b.RecCity,
					RecState = b.RecState,
					DestNo = d.DestNo,
					CName = d.CName,
					Cost = b.Cost == null ? 0 : b.Cost.Value,
					CcNo = b.CcNo,
					PayCustCHName = b.PayCustCHName,
					ToPayment = b.ToPayment == null ? 0 : b.ToPayment.Value,
					ToPaymentCurrency = b.ToPaymentCurrency,
					Freight = b.Freight == null ? 0 : b.Freight.Value,
					FreightCurrency = b.FreightCurrency,
					Type = b.Type,
					DtlBagNo = m.BagNo,
					DtlType = m.Type,
					DtlProductName = m.ProductName,
					DtlPcs = m.Pcs == null ? 0 : m.Pcs.Value,
					DtlPrice = m.Price,
					DtlQty = m.Qty,
					DtlWeight = m.Weight == null ? 0 : m.Weight.Value,
					DtlLength = m.Length == null ? 0 : m.Length.Value,
					DtlWidth = m.Width == null ? 0 : m.Width.Value,
					DtlHeight = m.Height == null ? 0 : m.Height.Value,
					DtlGrossWeight = m.GrossWeight == null ? 0 : m.GrossWeight.Value,
					DtlHSNo = m.HSNo,
					Remark = b.Remark,
					Remark2 = b.Remark2,
					sStatNo = b.SStatNo,
					aStatNo = b.AStatNo,
				};
			itemDatas.AddRange(Items);

			var mas = from b in db.BL_Report_Mas.Where(x => x.ReportNo == id && x.IsDelete == false)
					  join s in db.ORG_Stat.Where(x => x.IsDelete == false)
					  on b.SStatNo equals s.StatNo into ps
					  from s in ps.DefaultIfEmpty()
					  join a in db.ORG_Stat.Where(x => x.IsDelete == false)
					 on b.SStatNo equals a.StatNo into ps1
					  from a in ps1.DefaultIfEmpty()
					  join h in db.ORG_Hub.Where(x => x.IsDelete == false)
					 on b.HubNo equals h.HubNo into ps2
					  from h in ps2.DefaultIfEmpty()
					  join r in db.ORG_Report_Mgmt.Where(x => x.IsDelete == false)
					  on b.ReportID equals r.ID into ps3
					  from r in ps3.DefaultIfEmpty()
					  select new
					  {
						  ReportNo = b.ReportNo,
						  CreatedDate = b.CreatedDate,
						  MasterNo = b.MasterNo,
						  FlightNo = b.FlightNo,
						  HubNo = b.HubNo,
						  HubCode = h == null ? null : h.HubCode,
						  HubName = h == null ? null : h.HubName,
						  ReportID = b.ReportID,
						  ReportCName = r == null ? null : r.ReportCName,
						  SStatNo = b.SStatNo,
						  SStatName = s == null ? null : s.StatName,
						  AStatNo = b.AStatNo,
						  AStatName = a == null ? null : a.StatName,
					  };
			var MasData = mas.FirstOrDefault();

			var tData = new BL_Report()
			{
				ReportNo = MasData.ReportNo,
				CreatedDate = MasData.CreatedDate,
				MasterNo = MasData.MasterNo,
				FlightNo = MasData.FlightNo,
				HubNo = MasData.HubNo,
				HubCode = MasData.HubCode,
				HubName = MasData.HubName,
				ReportID = MasData.ReportID,
				ReportCName = MasData.ReportCName,
				SStatNo = MasData.SStatNo,
				SStatName = MasData.SStatName,
				AStatNo = MasData.AStatNo,
				AStatName = MasData.AStatName,
				Items = itemDatas,
			};
			datas.Add(tData);

			#endregion

			var result = new ResultHelper();
			var xSLXHelper = new XSLXHelper();
			XLWorkbook xLWorkbook = new XLWorkbook();
			switch (datas[0].ReportCName)
			{
				#region  派件印尼
				case "印尼PIBK派件明細表":
				case "印尼FTZ派件明細表":
					var bViceTitles = new BViceTitles()
					{
						MAWBNo = tData.MasterNo,
						FlightNo = tData.FlightNo,
						FlightDate = tData.CreatedDate,
					};
					var c_Piece = new List<C_Piece>();
					foreach (var i in itemDatas)
					{
						var cdata = new C_Piece()
						{
							BagNo = i.DtlBagNo,
							MasterNo = i.LadingNo,
							Sender = i.SendEName1 + "\n" + i.SendENAddr1 + "\n" + "TEL : " + i.SendPhone + "\n" + "ATTN : " + i.SendBy,
							Receiver = i.RecCompany + "\n" + i.RecChAddr + "\n" + "TEL : " + i.RecPhone + "\n" + "ATTN : " + i.RecBy,
							Pkg = i.DtlPcs,
							Product = i.DtlProductName,
							Pcs = (int)i.DtlQty,
							Weight = (double)i.DtlWeight,
							UnitPrice = i.DtlPrice.ToString(),
							Pp = i.CcNo == "PP" ? i.Freight.ToString() : null,
							Cc = i.CcNo == "CC" ? i.ToPayment.ToString() : null,
							Remark = i.Remark,
							Remark2 = i.Remark2,
						};
						c_Piece.Add(cdata);
					}
					var totalsData4 = new TotalsData()
					{
						Weight = (double)itemDatas.Sum(x => x.DtlWeight),
						PP = itemDatas.Sum(x => x.Freight),
						CC = itemDatas.Sum(x => x.ToPayment),
					};
					xSLXHelper.Export(c_Piece, totalsData4, datas[0].ReportCName, XSLXType.派件印尼, bViceTitles);
					break;
				#endregion

				#region 清關印尼
				case "印尼PIBK清關明細表":
				case "印尼FTZ清關明細表":
					var bViceTitles2 = new BViceTitles()
					{
						MAWBNo = tData.MasterNo,
						FlightNo = tData.FlightNo,
						FlightDate = tData.CreatedDate,
					};
					var e_Clearance = new List<E_Clearance>();
					foreach (var i in itemDatas)
					{
						var edata = new E_Clearance()
						{
							BagNo = i.DtlBagNo,
							MasterNo = i.LadingNo,
							Pkg = i.DtlPcs,
							Product = i.DtlProductName,
							Pcs = (int)i.DtlQty,
							Weight = (double)i.DtlWeight,
							Remark2 = i.Remark2
						};
						e_Clearance.Add(edata);
					}
					var totalsData5 = new TotalsData()
					{
						Weight = (double)itemDatas.Sum(x => x.DtlWeight),
						Pcs = itemDatas.Sum(x => x.DtlPcs),
					};
					xSLXHelper.Export(e_Clearance, totalsData5, datas[0].ReportCName, XSLXType.清關印尼, bViceTitles2);
					break;
				#endregion

				#region 清關印尼海關
				case "印尼海關PIBK清關明細表":
				case "印尼海關FTZ清關明細表":
					var bViceTitles3 = new BViceTitles()
					{
						MAWBNo = tData.MasterNo,
						FlightNo = tData.FlightNo,
						FlightDate = tData.CreatedDate,
					};
					var f_Clearance = new List<F_Clearance>();
					foreach (var i in itemDatas)
					{
						var fdata = new F_Clearance()
						{
							BagNo = i.DtlBagNo,
							MasterNo = i.LadingNo,
							Sender = i.SendEName1 + "\n" + i.SendENAddr1 + "\n" + "TEL : " + i.SendPhone + "\n" + "ATTN : " + i.SendBy,
							Receiver = i.RecCompany + "\n" + i.RecChAddr + "\n" + "TEL : " + i.RecPhone + "\n" + "ATTN : " + i.RecBy,
							Pkg = i.DtlPcs,
							Product = i.DtlProductName,
							Pcs = (int)i.DtlQty,
							Weight = (double)i.DtlWeight,
							UnitPrice = i.DtlPrice.ToString(),
							Remark = i.Remark,
							Remark2 = i.Remark2,
						};
						f_Clearance.Add(fdata);
					}
					var totalsData = new TotalsData()
					{
						Weight = (double)itemDatas.Sum(x => x.DtlWeight),
						Pcs = itemDatas.Sum(x => x.DtlPcs),
						pPcs = itemDatas.Where(x => x.sStatNo == "TPE").Sum(x => x.DtlPcs),
						aPcs = itemDatas.Where(x => x.sStatNo == "TAO").Sum(x => x.DtlPcs),
						xPcs = itemDatas.Where(x => x.sStatNo == "TXG").Sum(x => x.DtlPcs),
						tPcs = itemDatas.Where(x => x.sStatNo == "TNN").Sum(x => x.DtlPcs),
						kPcs = itemDatas.Where(x => x.sStatNo == "KHH").Sum(x => x.DtlPcs),
					};
					xSLXHelper.Export(f_Clearance, totalsData, datas[0].ReportCName, XSLXType.清關印尼海關, bViceTitles3);
					break;
				#endregion

				#region 印尼海關明細表
				case "印尼海關PIBK明細表":
				case "印尼海關FTZ明細表":
					var g_Other = new List<G_Other>();
					foreach (var i in itemDatas)
					{
						var gdata = new G_Other()
						{
							BagNo = i.DtlBagNo,
							MasterNo = i.LadingNo,
							SendCHName = i.SendEName1,
							SendCustAddr = i.SendENAddr1,
							RecCompany = i.RecCompany,
							RecChAddr = i.RecChAddr,
							Pkg = i.DtlPcs,
							Product = i.DtlProductName,
							HSNo = i.DtlHSNo,
							Pcs = (int)i.DtlQty,
							Weight = (double)i.DtlWeight,
							DtlPrice = i.DtlPrice.ToString(),
							Remark = i.Remark,
							RecCity = i.RecCity,
							RecState = i.RecState,
							RecPostDist = i.RecPostDist,
							RecBy = i.RecBy,
							RecPhone = i.RecPhone,
						};
						g_Other.Add(gdata);
					}
					var totalsData8 = new TotalsData()
					{
						Weight = (double)itemDatas.Sum(x => x.DtlWeight),
						Pcs = itemDatas.Sum(x => x.DtlPcs),
					};
					xSLXHelper.Export(g_Other, totalsData8, datas[0].ReportCName, XSLXType.印尼海關明細表, null);
					break;
				#endregion

				#region 派件大陸
				case "CN-SF順豐大陸派件明細表":
				case "CN-承鼎海特快大陸派件明細表":
				case "CN-承鼎小貿大陸派件明細表":
				case "CN-慧璞小貿大陸派件明細表":
				case "CN-欣浩小貿大陸派件明細表":
				case "CN-欣浩特貨大陸派件明細表":
				case "CN-欣浩大貿大陸派件明細表":
				case "香港派件明細表":
				case "香港海快派件明細表":
				case "澳門派件明細表":
				case "柬埔寨派件明細表":
				case "越南派件明細表":
					var masterNo = "";
					var clearanceCo = "";
					switch (tData.ReportCName)
					{
						case "CN-SF順豐大陸派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "順豐";
							break;
						case "CN-承鼎海特快大陸派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "承鼎";
							break;
						case "CN-承鼎小貿大陸派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "承鼎";
							break;
						case "CN-慧璞小貿大陸派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "慧璞";
							break;
						case "CN-欣浩小貿大陸派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "欣浩";
							break;
						case "CN-欣浩特貨大陸派件明細表":
							masterNo = tData.ReportNo;
							clearanceCo = "欣浩";
							break;
						case "CN-欣浩大貿大陸派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "欣浩";
							break;
						case "香港派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "";
							break;
						case "香港海快派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "";
							break;
						case "澳門派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "泓力";
							break;
						case "柬埔寨派件明細表":
							masterNo = tData.ReportNo;
							clearanceCo = "思邦";
							break;
						case "越南派件明細表":
							masterNo = tData.MasterNo;
							clearanceCo = "立大";
							break;
					}

					var aViceTitles = new AViceTitles()
					{
						ViceTitles = tData.ReportCName == "越南派件明細表" ? tData.HubName + "出貨明細表" : tData.ReportCName,
						SheetDate = DateTime.Now,
						MasterNo = masterNo,
						ClearanceCo = clearanceCo,
						FlightNo = tData.FlightNo,
						FlightDate = tData.CreatedDate,
					};
					var a_Piece = new List<A_Piece>();
					decimal amount = 0;
					foreach (var i in itemDatas)
					{
						var adata = new A_Piece()
						{
							BagNo = i.DtlBagNo,
							MasterNo = i.LadingNo,
							StatName = i.SendCHName,
							Receiver = i.RecCompany,
							Product = i.DtlProductName,
							Pcs = i.DtlPcs,
							Weight = (double)i.DtlWeight,
							Type = i.DtlType,
							MailingAddr = i.SendCustAddr,
							Recipient = i.RecPhone + " / " + i.RecBy,
							RecipientAddr = i.RecChAddr,
							Cc = i.CcNo,
							Amount = i.CcNo == "PP" ? i.Freight.ToString() : (i.CcNo == "CC" ? i.ToPayment.ToString() : ""),
							Currency = i.CcNo == "PP" ? i.FreightCurrency : (i.CcNo == "CC" ? i.ToPaymentCurrency : ""),
							Remark = i.Remark,
						};
						a_Piece.Add(adata);
						amount = amount + (i.CcNo == "PP" ? i.Freight : (i.CcNo == "CC" ? i.ToPayment : 0));
					}
					var totalsData2 = new TotalsData()
					{
						Count = a_Piece.Count(),
						Pcs = a_Piece.Sum(x => x.Pcs),
						Weight = a_Piece.Sum(x => x.Weight),
						Amount = amount,
					};
					if (tData.ReportCName == "越南派件明細表" || tData.ReportCName == "柬埔寨派件明細表")
						xSLXHelper.Export(a_Piece, totalsData2, datas[0].ReportCName, XSLXType.派件越南, aViceTitles);
					else
						xSLXHelper.Export(a_Piece, totalsData2, datas[0].ReportCName, XSLXType.派件大陸, aViceTitles);
					break;
				#endregion

				#region 河內派件明細表
				case "河內派件明細表":
					var bViceTitles4 = new BViceTitles()
					{
						MAWBNo = tData.MasterNo,
						FlightNo = tData.FlightNo,
						FlightDate = tData.CreatedDate,
					};
					var b_Piece = new List<B_Piece>();
					foreach (var i in itemDatas)
					{
						var bdata = new B_Piece()
						{
							BagNo = i.DtlBagNo,
							MasterNo = i.LadingNo,
							Sender = i.SendEName1 + "\n" + i.SendENAddr1 + "\n" + "TEL : " + i.SendPhone + "\n" + "ATTN : " + i.SendBy,
							Receiver = i.RecCompany + "\n" + i.RecChAddr + "\n" + "TEL : " + i.RecPhone + "\n" + "ATTN : " + i.RecBy,
							Pkg = i.DtlPcs,
							Product = i.DtlProductName,
							Pcs = (int)i.DtlQty,
							Weight = (double)i.DtlWeight,
							UnitPrice = i.DtlPrice.ToString(),
							Pp = i.CcNo == "PP" ? i.Freight.ToString() : null,
							Cc = i.CcNo == "CC" ? i.ToPayment.ToString() : null,
							Remark = i.Remark,
							Stat = i.sStatNo,
						};
						b_Piece.Add(bdata);
					}
					var totalsData6 = new TotalsData()
					{
						Weight = (double)itemDatas.Sum(x => x.DtlWeight),
						PP = itemDatas.Sum(x => x.Freight),
						CC = itemDatas.Sum(x => x.ToPayment),
					};
					xSLXHelper.Export(b_Piece, totalsData6, datas[0].ReportCName, XSLXType.派件河內, bViceTitles4);
					break;
				#endregion

				#region 清關大陸
				case "CN-SF順豐大陸清關明細表":
				case "CN-承鼎海特快大陸清關明細表":
				case "CN-承鼎小貿大陸清關明細表":
				case "CN-慧璞小貿大陸清關明細表":
				case "CN-欣浩小貿大陸清關明細表":
				case "CN-欣浩特貨大陸清關明細表":
				case "CN-欣浩大貿大陸清關明細表":
				case "香港清關明細表":
				case "香港海快清關明細表":
				case "澳門清關明細表":
				case "柬埔寨清關明細表":
				case "越南清關明細表":
				case "河內清關明細表":
					var masterNo2 = "";
					var clearanceCo2 = "";
					switch (tData.ReportCName)
					{
						case "CN-SF順豐大陸清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "順豐";
							break;
						case "CN-承鼎海特快大陸清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "承鼎";
							break;
						case "CN-承鼎小貿大陸清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "承鼎";
							break;
						case "CN-慧璞小貿大陸清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "慧璞";
							break;
						case "CN-欣浩小貿大陸清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "欣浩";
							break;
						case "CN-欣浩特貨大陸清關明細表":
							masterNo2 = tData.ReportNo;
							clearanceCo2 = "欣浩";
							break;
						case "CN-欣浩大貿大陸清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "欣浩";
							break;
						case "香港清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "";
							break;
						case "香港海快清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "";
							break;
						case "澳門清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "泓力";
							break;
						case "柬埔寨清關明細表":
							masterNo2 = tData.ReportNo;
							clearanceCo2 = "思邦";
							break;
						case "越南清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "立大";
							break;
						case "河內清關明細表":
							masterNo2 = tData.MasterNo;
							clearanceCo2 = "Smart Cargo Service Co., Ltd.";
							break;
					}
					var aViceTitles2 = new AViceTitles()
					{
						ViceTitles = tData.ReportCName,
						From = tData.ReportCName == "越南清關明細表" || tData.ReportCName == "河內清關明細表" ? "台南總公司" : "KHH",
						SheetDate = DateTime.Now,
						MasterNo = masterNo2,
						ClearanceCo = clearanceCo2,
						FlightNo = tData.FlightNo,
						FlightDate = tData.CreatedDate,
					};
					var d_Clearance = new List<D_Clearance>();
					foreach (var i in itemDatas)
					{
						var dData = new D_Clearance()
						{
							BagNo = i.DtlBagNo,
							MasterNo = i.LadingNo,
							Product = i.DtlProductName,
							Pcs = i.DtlPcs,
							Weight = (double)i.DtlWeight,
							Type = i.DtlType,
							Remark = i.Remark,
						};
						d_Clearance.Add(dData);
					}
					var totalsData3 = new TotalsData()
					{
						Count = itemDatas.Count(),
						Pcs = itemDatas.Sum(x => x.DtlPcs),
						Weight = (double)itemDatas.Sum(x => x.DtlWeight),
					};
					xSLXHelper.Export(d_Clearance, totalsData3, datas[0].ReportCName, XSLXType.清關大陸, aViceTitles2);
					break;
				#endregion

				#region JEMS出貨明細表
				case "EMS出貨明細表":
					var cViceTitles = new CViceTitles()
					{
						Weight = (double)itemDatas.Sum(x => x.DtlWeight),
						FlightDate = tData.CreatedDate,
					};
					var h_Other = new List<H_Other>();
					var seq = 1;
					foreach (var i in itemDatas)
					{
						var hData = new H_Other()
						{
							BagNo = seq++.ToString(),
							LadingNo = i.LadingNo,
							SendCHName = i.SendCHName,
							GrossWeight = Math.Round(i.DtlGrossWeight, 1).ToString(),
							Length = Math.Round(i.DtlLength, 1).ToString(),
							Width = Math.Round(i.DtlWidth, 1).ToString(),
							Height = Math.Round(i.DtlHeight, 1).ToString(),
							Cuft = (Math.Round(i.DtlLength * i.DtlWidth * i.DtlHeight * 10 / 6000) / 10).ToString(),
							DestNo = i.DestNo,
							CName = i.CName,
							Dox = i.Type == "0" || i.Type == "DOX" ? "V" : "",
							Spx = i.Type == "1" || i.Type == "2" || i.Type == "SPX" ? "V" : "",
							Pcs = i.DtlPcs.ToString(),
							Signer = "",
						};
						h_Other.Add(hData);
					}
					var totalsData7 = new TotalsData()
					{
						Weight = (double)itemDatas.Sum(x => x.DtlGrossWeight),
						Pcs = itemDatas.Sum(x => x.DtlPcs),
					};
					xSLXHelper.Export(h_Other, totalsData7, datas[0].ReportCName, XSLXType.JEMS出貨明細表, cViceTitles);
					break;
					#endregion
			}
		}
	}
}