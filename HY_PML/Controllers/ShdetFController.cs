using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class ShdetFController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

		public ActionResult _ShdetQueryBlock()
		{
			return PartialView();
		}
		public ActionResult _ElementInForm()
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

		// GET: Currency
		[Authorize]
		public ActionResult Index(string QueryNo = "")
		{
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			//頁面的抬頭
			ViewBag.Title = "預約調派";
			ViewBag.Title2 = "明細資料";
			ViewBag.Title3 = "貨物資料";

			ViewBag.ThisControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

			//about grid
			ViewBag.Grid1ActionName = "GetGridJSON1";
			ViewBag.GridColumnDefinition = @"
{ field: 'IsDesp', title: '調派狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; }},
{ field: 'ShdetNo', title: '調派編號'},
{ field: 'CustNo', title: '客戶代號' },
{ field: 'CustCHName', title: '客戶名稱'},
{ field: 'ReserveDate', title: '預約日期', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD'); }},
{ field: 'Clerk', title: '業務專員' },
{ field: 'HubNo', title: '路線' },
{ field: 'Dest', title: '目的地' },
{ field: 'ShdetBy', title: '調派人'},
{ field: 'ShdetDate', title: '調派時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); }},
{ field: 'IsCancel', title: '取消狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; }},
{ field: 'CancelBy', title: '取消人'},
{ field: 'CancelDate', title: '取消時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); }},
{ field: 'IsReply', title: '回覆狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; }},
{ field: 'ReplyBy', title: '回覆人'},
{ field: 'ReplyDate', title: '回覆時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); }},
{ field: 'IsFinish', title: '完成狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; }},
{ field: 'FinishBy', title: '完成人'},
{ field: 'FinishDate', title: '完成時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); }},
{ field: 'UpdatedBy', title: '修改人員'},
{ field: 'UpdatedDate', title: '修改時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); }},
";
			ViewBag.Grid2ActionName = "GetGridJSON2";
			ViewBag.GridColumnDefinition2 = @"
{ field: 'sNo', title: '序號', styler: function (value, row, index) { return 'background-color:blue;color:yellow;'; }},
{ field: 'CarryName', title: '取件名稱' },
{ field: 'Code5', title: '郵政5碼' },
{ field: 'CustAddrFull', title: '完整地址' },
{ field: 'CustENAddr1', title: '英文地址' },
{ field: 'Country', title: '國家' },
{ field: 'CtcSale', title: '業務部聯絡人' },
{ field: 'Tel', title: '電話' },
{ field: 'PickUpAreaNo', title: '調度區域' },
{ field: 'CallDate', title: '叫件時間' },
{ field: 'EndDate', title: '截止時間' },
{ field: 'SectorNo', title: '外務員代號' },
{ field: 'SectorName', title: '外務員' },
{ field: 'WeigLevel', title: '貨物類型' },
{ field: 'CocustomTyp', title: '報關類型' },
{ field: 'CcNo', title: '付款方式' },
{ field: 'Charge', title: '收款金額' },
{ field: 'RedyTime', title: '可取件時間' },
{ field: 'StatNo', title: '叫件站點' },
{ field: 'CarID', title: '車輛ID' },
{ field: 'CallStatNo', title: '取件站點' },
{ field: 'UpdatedBy', title: '修改人員'},
{ field: 'UpdatedDate', title: '修改時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); }},
";
			ViewBag.TablePKCol2 = ",ShdetNo:row.ShdetNo,CustNo:row.CustNo,sNo:row.sNo";

			ViewBag.Grid3ActionName = "GetGridJSON3";
			ViewBag.GridColumnDefinition3 = @"
{ field: 'sNo', title: '序號' },
{ field: 'Pcs', title: '件數' },
{ field: 'iTotNum', title: '總材數' },
{ field: 'Weig', title: '重量' },
{ field: 'IsDesp', title: '調派狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; }  },
{ field: 'ShdetBy', title: '調派人' },
{ field: 'ShdetDate', title: '調派時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); }},
{ field: 'IsCancel', title: '取消狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; } },
{ field: 'CancelBy', title: '取消人' },
{ field: 'CancelDate', title: '取消時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); } },
{ field: 'IsReply', title: '回覆狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; } },
{ field: 'ReplyBy', title: '回覆人' },
{ field: 'ReplyDate', title: '回覆時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); } },
{ field: 'IsFinish', title: '完成狀態', formatter: function (value, row, index) { if (value == true) return '是'; return '否'; } },
{ field: 'FinishBy', title: '完成人' },
{ field: 'FinishDate', title: '完成時間', formatter: function (value) { if (value != null) return moment(value).format('YYYY-MM-DD hh:mm:ss'); } },
{ field: 'ReplyComment', title: '回覆說明' },
{ field: 'Remark1', title: '內部說明' },
{ field: 'Remark3', title: '注意事項' },
";
			ViewBag.TablePKCol3 = ",ShdetNo:row.ShdetNo,CustNo:row.CustNo,sNo:row.sNo";

			//Grid搜尋的提示字
			ViewBag.GridSearchPromptText = "調派編號:";
			ViewBag.GridShowSelectContentJs = "row.ShdetNo";

			ViewBag.FormWidth = "750px";
			ViewBag.FormHeight = "570px";

			ViewBag.FormPartialName2 = "_ElementInForm2";
			ViewBag.FormActionNameNew2 = "NewShdet2";
			ViewBag.FormActionNameEdit2 = "EditShdet2";
			ViewBag.FormActionNameDelete2 = "DeleteShdet2";
			ViewBag.FormTitleNew2 = "新增明細資料";
			ViewBag.FormTitleEdit2 = "修改明細資料";

			ViewBag.FormPartialName3 = "_ElementInForm3";
			ViewBag.FormActionNameNew3 = "NewShdet3";
			ViewBag.FormActionNameEdit3 = "EditShdet3";
			ViewBag.FormActionNameDelete3 = "DeleteShdet3";
			ViewBag.FormTitleNew3 = "新增貨物資料";
			ViewBag.FormTitleEdit3 = "修改貨物資料";

			ViewBag.FormCustomJsNew = "";
			ViewBag.FormCustomJsEdit = "";

			ViewBag.FormCustomJsEdit2 =
				"if (retRow2.IsRedy == '是'){$('#IsRedy').switchbutton('check');}else{$('#IsRedy').switchbutton('uncheck');}";
			//由快捷查詢
			if (QueryNo != "")
			{
				ViewBag.QuickNo = QueryNo;
			}

			//得到所屬站點, 原本由外務員，20180706更改為帶出使用者的站點
			try
			{
				string cId = WebSiteHelper.CurrentUserID;
				int dbId = int.Parse(WebSiteHelper.CurrentUserDBID);
				var userInfo = from s in db.SYS_User
							   where s.IsDelete == false && s.ID == dbId
							   select s;
				string statNo = userInfo.First().StatNo;
				ViewBag.StatNo = statNo;
			}
			catch (Exception eGetLoginStat)
			{ }

			//權限控管
			if (WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
			{
				return View();
			}
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}
		}

		[Authorize]
		public ActionResult getSelectionjqGrid(string formId, string inputFields, string inputValues, string id, string funcName)
		{
			ViewBag.formId = formId;
			ViewBag.inputValues = inputValues.Split(',');
			ViewBag.inputFields = inputFields.Split(',');
			ViewBag.id = id;
			ViewBag.funcName = funcName;
			return View();
		}

		[Authorize]
		public ActionResult NewShdet(ShdetMD data, bool IsNewLading = false)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			JObject result;

			//2019-12-09，提單2.0加入快速新增，IsNewLading用於判斷來源為調派OR提單2.0。
			using (var trans = db.Database.BeginTransaction())
			{
				var ladingNo = "";
				var blData = data.LadingNo_Type == null ? null : db.Bill_Lading.FirstOrDefault(x => x.LadingNo_Type == data.LadingNo_Type && x.IsDelete == false);
				var shdetDtlData = data.LadingNo_Type == null ? null : db.ShdetDetail.FirstOrDefault(x => x.LadingNo_Type == data.LadingNo_Type);
				if ((shdetDtlData != null || blData != null) && blData.SDate != null && IsNewLading == true)
				{
					trans.Rollback();
					result = new JObject { { "errorMsg", "已存在此筆單號【" + data.LadingNo_Type + "】，請確認！" } };
				}
				else
				{
					if (IsNewLading && blData == null)
					{
						var saveData = new Bill_Lading();
						string cId = WebSiteHelper.CurrentUserID;
						int dbId = int.Parse(WebSiteHelper.CurrentUserDBID);
						var userInfo = from s in db.SYS_User
									   where s.IsDelete == false && s.ID == dbId
									   select s;
						string statNo = userInfo.First().StatNo;
						////電子提單規則：
						////第１碼為遠達公司（Ｐ）
						////第２碼為出口國
						////	第３－８碼為西元年月日（２０１９取１９）
						////第９－１２為流水號
						////	第１３碼為驗證碼　（年＋月＋日＋流水號）／２９　取小數位第二位
						//var areaID = db.ORG_Stat.Where(x => x.StatNo == statNo).Select(x => x.AreaID).FirstOrDefault();
						//var areaNo = db.ORG_Area.Where(x => x.ID == areaID).Select(x => x.AreaNo).FirstOrDefault().Substring(0, 1); ;
						//var date = DateTime.Now.ToDateString().Replace("/", "").Substring(2);
						//var lastLadingNo = db.Bill_Lading.Where(x => x.LadingNo.Contains("P" + areaNo + date)).OrderByDescending(x => x.CreateTime).Select(x => x.LadingNo).FirstOrDefault();
						//var sN = lastLadingNo != null ? String.Format("{0:0000}", int.Parse(lastLadingNo.Substring(8, 4)) + 1) : "0001";
						//var vCode = String.Format("{0:0.00}", ((decimal.Parse(date.Substring(0, 2)) + decimal.Parse(date.Substring(2, 2)) + decimal.Parse(date.Substring(4, 2)) + decimal.Parse(sN)) / 29)).ToString().Split('.')[1].Substring(1, 1);
						//ladingNo = "P" + areaNo + date + sN + vCode;

						//2019-12-11 如台灣 TW18601567前面TW碼 為區域提單碼， 1860156 為流水號， X為條碼驗證碼 X = (186 + 01 + 56) / 29 = 8.379310344827586 取小數點第二碼7為驗證碼
						var areaID = db.ORG_Stat.Where(x => x.StatNo == statNo).Select(x => x.AreaID).FirstOrDefault();
						var areaNo = db.ORG_Area.Where(x => x.ID == areaID).Select(x => x.AreaCode).FirstOrDefault();
						var lastLadingNo = db.Bill_Lading.Where(x => x.LadingNo.Substring(0, 2) == areaNo).OrderByDescending(x=> x.LadingNo).Select(x => x.LadingNo).FirstOrDefault();
						var sN = lastLadingNo != null ? String.Format("{0:0000000}", int.Parse(lastLadingNo.Substring(2, 7)) + 1) : "0000001";
						var vCode = String.Format("{0:0.00}", (Math.Floor((decimal.Parse(sN.Substring(0, 3)) + decimal.Parse(sN.Substring(3, 2)) + decimal.Parse(sN.Substring(5, 2))) / 29 * 100)) / 100).ToString().Split('.')[1].Substring(1, 1);
						ladingNo = areaNo + sN + vCode;
						saveData.LadingNo_Type = data.LadingNo_Type != null ? data.LadingNo_Type : ladingNo;
						saveData.LadingNo = ladingNo;
						saveData.ShdetNo = ladingNo;
						saveData.LadingDate = Convert.ToDateTime(DateTime.Now.ToDateString());
						saveData.SDate = data.SDate;
						saveData.SStatNo = statNo;
						saveData.SStatName = db.ORG_Stat.Where(x => x.StatNo == statNo).Select(x => x.StatName).FirstOrDefault();
						saveData.DestNo = db.ORG_Dest.Where(x => x.CName == data.Dest).Select(x => x.DestNo).FirstOrDefault();
						saveData.CName = data.Dest;
						saveData.HubNo = data.HubNo;
						saveData.Source = "Web";
						saveData.IsConfirm = true;
						saveData.IsCheck = false;
						saveData.IsReview = false;
						saveData.CreateTime = DateTime.Now;
						saveData.CreateBy = User.Identity.Name;
						saveData.IsDelete = false;
						var custData = db.ORG_Cust.Where(x => x.CustNo == data.CustNo).FirstOrDefault();
						var custDtlData = db.ORG_CustDetail.Where(x => x.CustNo == data.CustNo).OrderByDescending(x => x.sNo).FirstOrDefault();
						saveData.SendCustNo = data.CustNo;
						saveData.SendCHName = data.CustCHName;
						saveData.SendCustLevel = custData.CustLevel;
						saveData.SendPhone = custData.Phone;
						saveData.SendFaxNo = custData.FaxNo;
						saveData.SendBy = custDtlData.SendBy;
						saveData.SendCompany = custData.CustCName;
						saveData.SendCustAddr = custDtlData.CustAddr
							   + (custDtlData.Add_1 == 0 || custDtlData.Add_1 == null ? null : (custDtlData.Add_1 + "段"))
							   + (custDtlData.Add_2 == 0 || custDtlData.Add_2 == null ? null : (custDtlData.Add_2 + "巷"))
							   + (custDtlData.Add_3 == 0 || custDtlData.Add_3 == null ? null : (custDtlData.Add_3 + "弄"))
							   + (custDtlData.Add_4 == "" || custDtlData.Add_4 == null ? null : (custDtlData.Add_4 + "號"))
							   + (custDtlData.Add_5 == 0 || custDtlData.Add_5 == null ? null : (custDtlData.Add_5 + "樓"))
							   + (custDtlData.Add_6 == "" || custDtlData.Add_6 == null ? null : custDtlData.Add_6);
						saveData.SendCity = custDtlData.City;
						saveData.SendCountry = custDtlData.Country;
						saveData.SendState = custDtlData.State;
						saveData.SendPostDist = custDtlData.PostDist;
						saveData.SendEBy = custDtlData.ESendBy;
						saveData.SendECompany = custData.CustEName1;
						saveData.SendECustAddr = custDtlData.CustENAddr1;
						saveData.SendECity = custDtlData.ECity;
						saveData.SendECountry = custDtlData.ECountry;
						saveData.SendEState = custDtlData.EState;
						saveData.SendEPostDist = custDtlData.EPostDist;
						db.Bill_Lading.Add(saveData);
					}
					else if (IsNewLading && blData != null)
					{
						ladingNo = blData.LadingNo;
						blData.LadingDate = Convert.ToDateTime(DateTime.Now.ToDateString());
						blData.SDate = data.SDate;
						blData.HubNo = data.HubNo;
						blData.HubName = data.HubName;
						blData.DestNo = db.ORG_Dest.Where(x => x.IsDelete == true && x.CName == data.Dest).Select(x => x.DestNo).FirstOrDefault();
						blData.CName = data.Dest;
						blData.CocustomTyp = data.CocustomTyp;
						blData.Type = data.WeigLevel.ToString();
						blData.CcNo = data.CcNo;
						blData.CreateTime = DateTime.Now;
						blData.CreateBy = User.Identity.Name;
						blData.UpdateTime = DateTime.Now;
						blData.UpdateBy = User.Identity.Name;
						db.Entry(blData).State = EntityState.Modified;
					}

					DateTime nowTimeStamp = DateTime.Now;
					String slNowTimeStamp = String.Format("{0:yyyyMMdd}", nowTimeStamp);
					String slSysCustNo = "";

					//新增不收款客戶
					if (Request["newCust"] == "true")
					{
						ORG_Cust userRecord2 = new ORG_Cust();
						userRecord2.CustNo = "__" + String.Format("{0:yyyyMMdd}", DateTime.Now);
						userRecord2.CarryName = Request["CarryName"];
						userRecord2.CustCHName = Request["CustCHName"];
						userRecord2.CustCName = Request["CustCHName"];
						userRecord2.CustEName1 = Request["CustCHName"];
						userRecord2.CustEName2 = Request["CustCHName"];
						userRecord2.IsCommon = true;
						userRecord2.IsServer = true;
						userRecord2.CreatedDate = DateTime.Now;
						userRecord2.CreatedBy = User.Identity.Name;
						userRecord2.IsDelete = false;
						userRecord2.Remark = "臨時客戶";
						try
						{
							db.ORG_Cust.Add(userRecord2);
							db.SaveChanges();
						}
						catch (Exception e)
						{
							trans.Rollback();
							result = new JObject { { "errorMsg", (e as SqlException).Message } };
						}
						try
						{
							var JustRecord = from jr in db.ORG_Cust
											 where jr.CustNo == userRecord2.CustNo
											 orderby jr.ID descending
											 select jr;

							ORG_Cust firstRow = JustRecord.First();
							firstRow.CustNo = ((Request["Post3Code"].ToString()).Substring(0, 3) + String.Format("{0:0000000}", firstRow.ID) + "T").Trim();
							firstRow.Remark = "";
							db.Entry(firstRow).State = EntityState.Modified;
							slSysCustNo = firstRow.CustNo;
							db.SaveChanges();
							ORG_CustDetail userRecord3 = new ORG_CustDetail();
							userRecord3.CustNo = slSysCustNo;
							userRecord3.CarryName = Request["CarryName"];
							userRecord3.CustCHName = Request["CustCHName"];
							userRecord3.CustCName = Request["CustCHName"];
							userRecord3.CustEName1 = Request["CustCHName"];
							userRecord3.CustEName2 = Request["CustCHName"];
							userRecord3.IsMas = true;
							userRecord3.IsCommon = true;
							userRecord3.IsServer = true;
							userRecord3.CreatedDate = DateTime.Now;
							userRecord3.CreatedBy = User.Identity.Name;
							userRecord3.IsDelete = false;
							userRecord3.Remark = "臨時客戶";
							db.ORG_CustDetail.Add(userRecord3);
							db.SaveChanges();
						}
						catch (Exception e)
						{
							trans.Rollback();
							result = new JObject { { "errorMsg", (e as SqlException).Message } };
						}
					}

					var findMaxSeqNum = from o in db.ShdetHeader
										where (o.ShdetNo.StartsWith(slNowTimeStamp))
										orderby o.ShdetNo descending
										select o;
					int maxSeqNum = 0;
					if (findMaxSeqNum != null)
					{
						if (findMaxSeqNum.Count() > 0)
						{
							string slTempMaxShdetNo = findMaxSeqNum.First().ShdetNo;
							int.TryParse(WebSiteHelper.GetLast(slTempMaxShdetNo, 4), out maxSeqNum);
						}
					}
					maxSeqNum += 1;
					ShdetHeader userRecord = new ShdetHeader();
					if (IsNewLading)
					{
						userRecord.ShdetNo = ladingNo;
						userRecord.LadingNo = ladingNo;
					}
					else
						userRecord.ShdetNo = slNowTimeStamp + String.Format("{0:0000}", maxSeqNum);
					if (Request["newCust"] == "true")
					{
						userRecord.CustNo = slSysCustNo;
					}
					else
					{
						userRecord.CustNo = Request["CustNo"];
					}
					userRecord.CustCHName = Request["CustCHName"];
					userRecord.Clerk = Request["Clerk"];
					userRecord.HubNo = Request["HubNo"];
					userRecord.Dest = Request["Dest"];

					DateTime dtReserveDate = nowTimeStamp;
					if ((Request["ReserveDate"] != null) && (Request["ReserveDate"] != ""))
					{
						DateTime.TryParse(Request["ReserveDate"], out dtReserveDate);
						userRecord.ReserveDate = dtReserveDate;
					}
					DateTime dtSDate = nowTimeStamp;
					if ((Request["ReserveDate"] != null) && (Request["ReserveDate"] != ""))
					{
						DateTime.TryParse(Request["SDate"], out dtSDate);
						userRecord.SDate = dtSDate;
					}
					if (true)
					{
						bool blTarget;
						blTarget = (Request["IsDesp"] == "true") ? true : false;
						if (blTarget != userRecord.IsDesp)
						{
							userRecord.IsDesp = blTarget;
							if (blTarget == true)
							{
								userRecord.ShdetBy = User.Identity.Name;
								userRecord.ShdetDate = DateTime.Now;
							}
						}
						//shdetFix
						//blTarget = (Request["IsCancel"] == "true") ? true : false;
						//if (blTarget != userRecord.IsCancel)
						//{
						//	userRecord.IsCancel = blTarget;
						//	if (blTarget == true)
						//	{
						//		userRecord.CancelBy = User.Identity.Name;
						//		userRecord.CancelDate = DateTime.Now;
						//	}
						//}
						//blTarget = (Request["IsReply"] == "true") ? true : false;
						//if (blTarget != userRecord.IsReply)
						//{
						//	userRecord.IsReply = blTarget;
						//	if (blTarget == true)
						//	{
						//		userRecord.ReplyBy = User.Identity.Name;
						//		userRecord.ReplyDate = DateTime.Now;
						//	}
						//}
						//blTarget = (Request["IsFinish"] == "true") ? true : false;
						//if (blTarget != userRecord.IsFinish)
						//{
						//	userRecord.IsFinish = blTarget;
						//	if (blTarget == true)
						//	{
						//		userRecord.FinishBy = User.Identity.Name;
						//		userRecord.FinishDate = DateTime.Now;
						//	}
						//}
					}

					//以下系統自填
					userRecord.CreatedDate = nowTimeStamp;
					userRecord.CreatedBy = User.Identity.Name;
					userRecord.IsDelete = false;
					db.ShdetHeader.Add(userRecord);

					try
					{
						db.SaveChanges();
						//modified by jie,20180913
						result = new JObject { { "message", "ok" }, { "ShdetNo", userRecord.ShdetNo }, { "CustNo", userRecord.CustNo } };
						//modified by jie,20180913
						trans.Commit();
						if (IsNewLading)
						{
							//2020-01-02 測試轉哲盟LOG過多問題 暫不開啟
							//var New_Bill_LadingCtrl = new New_Bill_LadingController();
							//New_Bill_LadingCtrl.ControllerContext = ControllerContext;
							//New_Bill_LadingCtrl.SavePML_TWN(userRecord.ShdetNo);
						}
					}
					catch (Exception e)
					{
						trans.Rollback();
						result = new JObject { { "errorMsg", (e as SqlException).Message } };
					}
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult NewShdet2()
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{

			}
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}

			if (Request["ShdetNo"] != "" && Request["CustNo"] != "" && Request["sNo"] != "")
			{ }
			else
			{
				return Content(String.Format(slLogoutHtml, "無資料...", Request.ApplicationPath));
			}

			string slShdetNo = Request["ShdetNo"];
			string slCustNo = Request["CustNo"];
			int nlSNo;

			if (Request["sNo"] == "新增")
			{
				int maxSeqNum = 0;
				var findMaxSeqNum = from o in db.ShdetDetail
									where o.ShdetNo == slShdetNo && o.CustNo == slCustNo
									orderby o.sNo descending
									select o;
				if (findMaxSeqNum != null)
				{
					if (findMaxSeqNum.Count() > 0)
					{
						int slTempMaxsNo = findMaxSeqNum.First().sNo;
						maxSeqNum = slTempMaxsNo;
					}
				}
				maxSeqNum += 1;
				nlSNo = maxSeqNum;
			}
			else
			{

				int.TryParse(Request["sNo"], out nlSNo);
			}

			DateTime nowTimeStamp = DateTime.Now;
			ShdetDetail userRecord = new ShdetDetail();
			var blData = db.Bill_Lading.Where(x => x.LadingNo == slShdetNo).FirstOrDefault();
			userRecord.LadingNo_Type = blData == null ? slShdetNo : blData.LadingNo_Type;
			userRecord.ShdetNo = slShdetNo;
			userRecord.CustNo = slCustNo;
			userRecord.sNo = nlSNo;
			userRecord.CarryName = Request["CarryName"];
			userRecord.Code5 = Request["Code5"];
			userRecord.Code7 = Request["Code7"];
			userRecord.Add_6 = Request["add_6"];
			userRecord.CustAddr = Request["CustAddr"];
			userRecord.CustENAddr1 = Request["CustENAddr1"];
			userRecord.CustENAddr2 = Request["CustENAddr2"];
			userRecord.City = Request["City"];
			userRecord.State = Request["State"];
			userRecord.Country = Request["Country"];
			userRecord.CtcSale = Request["CtcSale"];
			userRecord.Tel = Request["Tel"];
			userRecord.Clerk = Request["Clerk"];
			userRecord.PickUpAreaNo = Request["PickUpAreaNo"];
			userRecord.EndDate = Request["EndDate"];
			userRecord.Remark2 = Request["Remark2"];
			userRecord.Remark3 = Request["Remark3"];
			userRecord.SectorNo = Request["SectorNo"];
			userRecord.CallType = Request["CallType"];
			userRecord.StatNo = Request["StatNo"];
			userRecord.CallStatNo = Request["CallStatNo"];

			int nlAdd1;
			int.TryParse(Request["add_1"], out nlAdd1);
			userRecord.Add_1 = nlAdd1;
			int nlAdd2;
			int.TryParse(Request["add_2"], out nlAdd2);
			userRecord.Add_2 = nlAdd2;
			int nlAdd3;
			int.TryParse(Request["add_3"], out nlAdd3);
			userRecord.Add_3 = nlAdd3;
			//int nlAdd4;
			//int.TryParse(Request["add_4"], out nlAdd4);
			userRecord.Add_4 = Request["add_4"];
			int nlAdd5;
			int.TryParse(Request["add_5"], out nlAdd5);
			userRecord.Add_5 = nlAdd5;

			//20180208 modified

			userRecord.CcNo = Request["CcNo"];
			userRecord.RedyTime = Request["RedyTime"];
			userRecord.CarID = Request["CarID"];
			int nlWeigLevel;
			int.TryParse(Request["WeigLevel"], out nlWeigLevel);
			userRecord.WeigLevel = nlWeigLevel;
			int nlCocustomTyp;
			int.TryParse(Request["CocustomTyp"], out nlCocustomTyp);
			userRecord.CocustomTyp = nlCocustomTyp;
			float flCharge;
			float.TryParse(Request["Charge"], out flCharge);
			userRecord.Charge = flCharge;
			DateTime dtRedy = nowTimeStamp;
			if ((Request["RedyDate"] != null) && (Request["RedyDate"] != ""))
			{
				DateTime.TryParse(Request["RedyDate"], out dtRedy);
				userRecord.RedyDate = dtRedy;
			}
			userRecord.IsRedy = Request["IsRedy"] == "true" ? true : false;
			bool blTarget;
			blTarget = (Request["d_IsDesp"] == "true") ? true : false;
			userRecord.IsDesp = blTarget;
			if (blTarget == true)
			{
				userRecord.ShdetBy = User.Identity.Name;
				userRecord.ShdetDate = nowTimeStamp;
			}
			DateTime dtADate = nowTimeStamp;
			DateTime.TryParse(Request["ADate"], out dtADate);
			userRecord.ADate = dtADate;
			userRecord.ATime = Request["ATime"];
			//shdetFix
			//blTarget = (Request["d_IsCancel"] == "true") ? true : false;
			//userRecord.IsCancel = blTarget;
			//if (blTarget == true)
			//{
			//	userRecord.CancelBy = User.Identity.Name;
			//	userRecord.CancelDate = nowTimeStamp;
			//}
			//blTarget = (Request["d_IsReply"] == "true") ? true : false;
			//userRecord.IsReply = blTarget;
			//if (blTarget == true)
			//{
			//	userRecord.ReplyBy = User.Identity.Name;
			//	userRecord.ReplyDate = nowTimeStamp;
			//}
			//blTarget = (Request["d_IsFinish"] == "true") ? true : false;
			//userRecord.IsFinish = blTarget;
			//if (blTarget == true)
			//{
			//	userRecord.FinishBy = User.Identity.Name;
			//	userRecord.FinishDate = nowTimeStamp;
			//}

			//以下系統自填
			userRecord.CreatedDate = nowTimeStamp;
			userRecord.CreatedBy = User.Identity.Name;
			userRecord.IsDelete = false;

			JObject result;
			try
			{
				db.ShdetDetail.Add(userRecord);
				db.SaveChanges();
				int retVal = 0;
				//retVal = (CheckAllDetailFinished(userRecord.ShdetNo) == true) ? retVal + 1 : retVal;
				if (retVal == 0)
					result = new JObject {
						{ "message", "ok" },
						{ "ShdetNo", userRecord.ShdetNo },
						{ "sNo", userRecord.sNo },
						{ "CarryName", userRecord.CarryName },
						{ "CarID", userRecord.CarID },
						{ "RedyDate", userRecord.RedyDate },
					}; //modified by jie,20180910
				else
					result = new JObject { { "message", retVal.ToString() } };

			}
			catch (Exception e)
			{
				result = new JObject { { "errorMsg", (e as SqlException).Message } };
				//throw;
			}

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			//return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult NewShdet3(ShdetProd data)
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{ }
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}
			var result = new ResultHelper();

			int maxSeqNum = 0;
			var findMaxSeqNum = db.ShdetProd.Where(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo).OrderByDescending(x => x.sNo).Select(x => x.sNo).FirstOrDefault();
			if (findMaxSeqNum > 0)
				maxSeqNum = findMaxSeqNum;
			maxSeqNum += 1;

			DateTime nowTime = DateTime.Now;
			var userRecord = new ShdetProd();

			userRecord.ShdetNo = data.ShdetNo;
			userRecord.CustNo = data.CustNo;
			userRecord.sDtlNo = data.sDtlNo;
			userRecord.sNo = maxSeqNum;

			userRecord.HubNo = data.HubNo;
			userRecord.CcNo = data.CcNo;
			userRecord.Dest = data.Dest;
			userRecord.RedyTime = data.RedyTime;
			userRecord.Remark1 = data.Remark1;
			userRecord.Remark3 = data.Remark3;
			userRecord.SectorNo = data.SectorNo;
			userRecord.CallType = data.CallType;
			userRecord.StatNo = data.StatNo;
			userRecord.CarID = data.CarID;
			userRecord.ReplyComment = data.ReplyComment;
			userRecord.Pcs = data.Pcs;
			userRecord.WeigLevel = data.WeigLevel;
			userRecord.CocustomTyp = data.CocustomTyp;
			userRecord.iTotNum = data.iTotNum;
			userRecord.fLen = data.fLen;
			userRecord.fWidth = data.fWidth;
			userRecord.Weig = data.Weig;
			userRecord.fHeight = data.fHeight;
			userRecord.Charge = data.Charge;

			if (data.RedyDate != null)
				userRecord.RedyDate = data.RedyDate;
			else
				userRecord.RedyDate = nowTime;

			userRecord.IsDesp = data.IsDesp;
			if (data.IsDesp == true)
			{
				userRecord.ShdetBy = User.Identity.Name;
				userRecord.ShdetDate = nowTime;
			}
			//shdetFix
			//userRecord.IsCancel = data.IsCancel;
			//if (data.IsCancel == true)
			//{
			//	userRecord.CancelBy = User.Identity.Name;
			//	userRecord.CancelDate = nowTime;
			//}

			//userRecord.IsReply = data.IsReply;
			//if (data.IsReply == true)
			//{
			//	userRecord.ReplyBy = User.Identity.Name;
			//	userRecord.ReplyDate = nowTime;
			//}

			//userRecord.IsFinish = data.IsFinish;
			//if (data.IsFinish == true)
			//{
			//	userRecord.FinishBy = User.Identity.Name;
			//	userRecord.FinishDate = nowTime;
			//}

			//以下系統自填
			userRecord.CreatedDate = nowTime;
			userRecord.CreatedBy = User.Identity.Name;
			userRecord.IsDelete = false;

			try
			{
				db.ShdetProd.Add(userRecord);
				db.SaveChanges();
				result.Ok = DataModifyResultType.Success;
				result.Message = "OK";

				int retVal = 0;
				//retVal = (CheckAllProdFinished(userRecord.ShdetNo, userRecord.CustNo, userRecord.sDtlNo) == true) ? retVal + 2 : retVal;
				//retVal = (CheckAllDetailFinished(userRecord.ShdetNo) == true) ? retVal + 1 : retVal;
				if (retVal == 0)
					result.Message = "OK";
				else
					result.Message = retVal.ToString();
			}
			catch (Exception e)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = (e as SqlException).Message;
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		public class Prod
		{
			public int Pcs { get; set; }
			public float fLen { get; set; }
			public float fWidth { get; set; }
			public float fHeight { get; set; }
			public float iTotNum { get; set; }
			public int WeigLevel { get; set; }
			public float Weig { get; set; }
			public int CocustomTyp { get; set; }
			public string HubNo { get; set; }
			public string CcNo { get; set; }
			public float Charge { get; set; }
			public string Dest { get; set; }
			public string RedyDate { get; set; }
			public string RedyTime { get; set; }
			public string SectorNo { get; set; }
			public string CallType { get; set; }
			public string StatNo { get; set; }
			public string CarID { get; set; }
			public string Remark1 { get; set; }
			public string Remark3 { get; set; }
			public string IsDesp { get; set; }
			public string IsCancel { get; set; }
			public string IsReply { get; set; }
			public string IsFinish { get; set; }
			public string ShdetNo { get; set; }
			public string CustNo { get; set; }
			public int sDtlNo { get; set; }
			public string CarryName { get; set; }//modified by jie,20180910
		}

		[Authorize]
		public ActionResult NewShdet3Batch(int total, List<Prod> rows)
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{

			}
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}

			JObject result;
			int R1 = 0;
			int R2 = 0;
			int R3 = 0;
			int CountOK = 0;
			int CountFAIL = 0;
			string errMessage = "";

			for (int i = 0; i < total; i++)
			{

				//string slShdetNo = Request["ShdetNo"];
				//string slCustNo = Request["CustNo"];

				//int nlDtlNo;
				//int.TryParse(Request["sDtlNo"], out nlDtlNo);

				int nlSNo;
				if (true)
				{
					int maxSeqNum = 0;
					int nlsDtlNo = rows[i].sDtlNo;
					string slShdetNo = rows[i].ShdetNo;
					string slCustNo = rows[i].CustNo;
					var findMaxSeqNum = from o in db.ShdetProd
										where o.ShdetNo == slShdetNo && o.CustNo == slCustNo && o.sDtlNo == nlsDtlNo
										orderby o.sNo descending
										select o;
					if (findMaxSeqNum != null)
					{
						if (findMaxSeqNum.Count() > 0)
						{
							int slTempMaxsNo = findMaxSeqNum.First().sNo;
							maxSeqNum = slTempMaxsNo;
						}
					}
					maxSeqNum += 1;
					nlSNo = maxSeqNum;
				}


				DateTime nowTimeStamp = DateTime.Now;
				ShdetProd userRecord = new ShdetProd();

				userRecord.ShdetNo = rows[i].ShdetNo;
				userRecord.CustNo = rows[i].CustNo;
				userRecord.sDtlNo = rows[i].sDtlNo;
				userRecord.sNo = nlSNo;

				userRecord.HubNo = rows[i].HubNo;
				userRecord.CcNo = rows[i].CcNo;
				userRecord.Dest = rows[i].Dest;
				if (rows[i].RedyTime != null)
					userRecord.RedyTime = rows[i].RedyTime;
				userRecord.Remark1 = rows[i].Remark1;
				userRecord.Remark3 = rows[i].Remark3;
				userRecord.SectorNo = rows[i].SectorNo;
				userRecord.CallType = rows[i].CallType;
				userRecord.StatNo = rows[i].StatNo;
				userRecord.CarID = rows[i].CarID;

				//int nlPcs;
				//int.TryParse(Request["Pcs"], out nlPcs);
				userRecord.Pcs = rows[i].Pcs;

				userRecord.fLen = rows[i].fLen;
				userRecord.fWidth = rows[i].fWidth;
				userRecord.fHeight = rows[i].fHeight;
				userRecord.iTotNum = rows[i].iTotNum;

				//int nlWeigLevel;
				//int.TryParse(Request["WeigLevel"], out nlWeigLevel);
				userRecord.WeigLevel = rows[i].WeigLevel;
				//int nlCocustomTyp;
				//int.TryParse(Request["CocustomTyp"], out nlCocustomTyp);
				userRecord.CocustomTyp = rows[i].CocustomTyp;

				//float flWeig;
				//float.TryParse(Request["Weig"], out flWeig);
				userRecord.Weig = rows[i].Weig;
				//float flCharge;
				//float.TryParse(Request["Charge"], out flCharge);
				userRecord.Charge = rows[i].Charge;

				DateTime dtRedy = nowTimeStamp;
				if ((rows[i].RedyDate != null) && (rows[i].RedyDate != ""))
				{
					DateTime.TryParse(rows[i].RedyDate, out dtRedy);
					userRecord.RedyDate = dtRedy;
				}

				bool blTarget;
				blTarget = (rows[i].IsDesp == "true" | rows[i].IsDesp == "是") ? true : false;
				userRecord.IsDesp = blTarget;
				if (blTarget == true)
				{
					userRecord.ShdetBy = User.Identity.Name;
					userRecord.ShdetDate = nowTimeStamp;
				}
				blTarget = (rows[i].IsCancel == "true" | rows[i].IsCancel == "是") ? true : false;
				userRecord.IsCancel = blTarget;
				if (blTarget == true)
				{
					userRecord.CancelBy = User.Identity.Name;
					userRecord.CancelDate = nowTimeStamp;
				}
				blTarget = (rows[i].IsReply == "true" | rows[i].IsReply == "是") ? true : false;
				userRecord.IsReply = blTarget;
				if (blTarget == true)
				{
					userRecord.ReplyBy = User.Identity.Name;
					userRecord.ReplyDate = nowTimeStamp;
				}
				blTarget = (rows[i].IsFinish == "true" || rows[i].IsFinish == "是") ? true : false;
				userRecord.IsFinish = blTarget;
				if (blTarget == true)
				{
					userRecord.FinishBy = User.Identity.Name;
					userRecord.FinishDate = nowTimeStamp;
				}

				//以下系統自填
				userRecord.CreatedDate = nowTimeStamp;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;


				try
				{
					db.ShdetProd.Add(userRecord);
					db.SaveChanges();
					int retVal = 0;
					//retVal = (CheckAllProdFinished(userRecord.ShdetNo, userRecord.CustNo, userRecord.sDtlNo) == true) ? retVal + 2 : retVal;
					//retVal = (CheckAllDetailFinished(userRecord.ShdetNo) == true) ? retVal + 1 : retVal;

					CountOK += 1;

					if (retVal == 0) { }
					//
					else if (retVal == 1)
						R1 += 1;
					else if (retVal == 2)
						R2 += 1;
					else if (retVal == 3)
						R3 += 1;
					//if (retVal == 0)
					//    result = new JObject { { "message", "ok" } };
					//else
					//    result = new JObject { { "message", retVal.ToString() } };
				}
				catch (Exception e)
				{
					//result = new JObject { { "errorMsg", e.Message } };
					CountFAIL += 1;
					errMessage += "件數：" + userRecord.Pcs.ToString() + "商品新增錯誤。";
					//throw;
				}


			}


			result = new JObject { { "OK", CountOK }, { "FAIL", CountFAIL }, { "R1", R1 }, { "R2", R2 }, { "R3", R3 }, { "errorMsg", errMessage } };

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			//return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		private bool CheckAllProdFinished(string ShdetNo, string CustNo, int sDtlNo)
		{
			var finishRecord = from record in db.ShdetProd
							   where record.IsFinish == false && record.IsDelete == false && record.ShdetNo == ShdetNo && record.CustNo == CustNo && record.sDtlNo == sDtlNo
							   select record;

			if (finishRecord.Count() == 0)
			{
				// all prod are finish , update shdetdetail
				var shdetDetail = db.ShdetDetail.Find(ShdetNo, CustNo, sDtlNo);
				if (shdetDetail != null)
				{
					try
					{
						bool OriginalFinish = (shdetDetail.IsFinish == true) ? true : false;
						shdetDetail.IsFinish = true;
						db.Entry(shdetDetail).State = EntityState.Modified;
						db.SaveChanges();
						if (OriginalFinish == false)
							return true;
					}
					catch (Exception e)
					{
						//throw;
					}
				}

			}
			else
			{
				// do nothing
			}

			return false;
		}

		private bool CheckAllDetailFinished(string ShdetNo)
		{
			var finishRecord = from record in db.ShdetDetail
							   where record.IsFinish == false && record.IsDelete == false && record.ShdetNo == ShdetNo
							   select record;

			if (finishRecord.Count() == 0)
			{
				// all detail are finish , update shdetheader
				var shdetHeader = db.ShdetHeader.Find(ShdetNo);
				if (shdetHeader != null)
				{
					try
					{
						bool OriginalFinish = (shdetHeader.IsFinish == true) ? true : false;
						shdetHeader.IsFinish = true;
						db.Entry(shdetHeader).State = EntityState.Modified;
						db.SaveChanges();
						if (OriginalFinish == false)
							return true;
					}
					catch (Exception e)
					{
						//throw;
					}
				}

			}
			else
			{
				// do nothing
			}

			return false;
		}

		[Authorize]
		public ActionResult EditShdet()
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{

			}
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}

			//int id;
			JObject result;
			//int.TryParse(Request["ShdetNo"], out id);
			ShdetHeader userRecord = db.ShdetHeader.Find(Request["ShdetNo"]);


			if (userRecord != null)
			{

				//沒有detail才能改
				var detailRecord = from dr in db.ShdetDetail
								   where dr.ShdetNo == userRecord.ShdetNo && dr.CustNo == userRecord.CustNo && dr.IsDelete == false
								   select dr;
				if (detailRecord.Count() == 0)
				{
					userRecord.CustNo = Request["CustNo"];
					userRecord.CustCHName = Request["CustCHName"];
				}
				userRecord.Clerk = Request["Clerk"];
				userRecord.HubNo = Request["HubNo"];
				userRecord.Dest = Request["Dest"];
				//有按下收件鈕，檢查 detail 與  prod 至少有一筆資料
				if (Request["goDesp"] == "yes")
				{
					if (detailRecord.Count() == 0)
					{
						result = new JObject { { "errorMsg", "明細無資料" } };
						return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
					}
					var prodRecord = from pr in db.ShdetProd
									 where pr.ShdetNo == userRecord.ShdetNo && pr.IsDelete == false
									 select pr;
					if (prodRecord.Count() == 0)
					{
						result = new JObject { { "errorMsg", "貨物無資料" } };
						return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
					}
				}

				//進階權限
				//if (WebSiteHelper.IsPermissioned("Index", "ShdetL"))
				DateTime dtReserveDate = DateTime.Now;
				if ((Request["ReserveDate"] != null) && (Request["ReserveDate"] != ""))
				{
					DateTime.TryParse(Request["ReserveDate"], out dtReserveDate);
					userRecord.ReserveDate = dtReserveDate;
				}

				DateTime dtSDate = DateTime.Now;
				if ((Request["SDate"] != null) && (Request["SDate"] != ""))
				{
					DateTime.TryParse(Request["SDate"], out dtSDate);
					if (dtSDate != userRecord.SDate)
					{
						var shdetNo = Request["ShdetNo"];
						var blData = db.Bill_Lading.Where(x => x.LadingNo == shdetNo).FirstOrDefault();
						if (blData != null)
						{
							blData.SDate = dtSDate;
							db.Entry(blData).State = EntityState.Modified;
							db.SaveChanges();
						}
						userRecord.SDate = dtSDate;
					}
				}

				if (true)
				{
					bool blTarget;
					blTarget = (Request["IsDesp"] == "true") ? true : false;
					if (blTarget != userRecord.IsDesp)
					{
						userRecord.IsDesp = blTarget;
						if (blTarget == true)
						{
							var shdetNo = Request["ShdetNo"];
							var hasProd = db.ShdetProd.Any(x => x.ShdetNo == shdetNo && x.IsDelete == false);
							if (!hasProd)
							{
								result = new JObject { { "errorMsg", "此筆調派單號底下無貨物資料，無法派件" } };
								return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
							}

							userRecord.ShdetBy = User.Identity.Name;
							userRecord.ShdetDate = DateTime.Now;
						}
					}
					//shdetFix
					//blTarget = (Request["IsCancel"] == "true") ? true : false;
					//if (blTarget != userRecord.IsCancel)
					//{
					//    userRecord.IsCancel = blTarget;
					//    if (blTarget == true)
					//    {
					//        var prod = db.ShdetProd.Any(x => x.ShdetNo == userRecord.ShdetNo && x.ShdetNo != null && x.SSNo != null);
					//        if (prod)
					//        {
					//            result = new JObject { { "errorMsg", "貨物已轉理貨，無法取消" } };
					//            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
					//        }
					//        userRecord.CancelBy = User.Identity.Name;
					//        userRecord.CancelDate = DateTime.Now;
					//    }
					//}
					//blTarget = (Request["IsReply"] == "true") ? true : false;
					//if (blTarget != userRecord.IsReply)
					//{
					//    userRecord.IsReply = blTarget;
					//    if (blTarget == true)
					//    {
					//        userRecord.ReplyBy = User.Identity.Name;
					//        userRecord.ReplyDate = DateTime.Now;
					//    }
					//}
					//blTarget = (Request["IsFinish"] == "true") ? true : false;
					//if (blTarget != userRecord.IsFinish)
					//{
					//    userRecord.IsFinish = blTarget;
					//    if (blTarget == true)
					//    {
					//        userRecord.FinishBy = User.Identity.Name;
					//        userRecord.FinishDate = DateTime.Now;
					//    }
					//}

				}

				//以下系統自填
				userRecord.UpdatedDate = DateTime.Now;
				userRecord.UpdatedBy = User.Identity.Name;
				//userRecord.IsDelete = false;               
				try
				{
					//db.SYS_User.Add(userRecord);
					db.Entry(userRecord).State = EntityState.Modified;
					db.SaveChanges();
					result = new JObject { { "message", "ok" } };
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
					//return Content(JsonConvert.SerializeObject(result), "application/json");
				}
				catch (Exception e)
				{
					result = new JObject { { "errorMsg", e.Message } };
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
					//return Content(JsonConvert.SerializeObject(result), "application/json");
					//throw;
				}
			}

			result = new JObject { { "errorMsg", "找不到資料!" } };
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
			//return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult EditShdet2(ShdetDetail data)
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{

			}
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}

			int nlSNo;
			JObject result;
			int.TryParse(Request["sNo"], out nlSNo);
			ShdetDetail userRecord = db.ShdetDetail.Find(Request["ShdetNo"], Request["CustNo"], nlSNo);
			if (data.CarID != userRecord.CarID)
			{
				var pdData = db.ShdetProd.Where(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sDtlNo == data.sNo).ToList();
				foreach (var p in pdData)
				{
					p.CarID = data.CarID;
					db.Entry(p).State = EntityState.Modified;
				}
			}
			if (userRecord != null)
			{
				DateTime nowTimeStamp = DateTime.Now;

				userRecord.CarryName = Request["CarryName"];
				userRecord.Code5 = Request["Code5"];
				userRecord.Code7 = Request["Code7"];
				userRecord.Add_6 = Request["add_6"];
				userRecord.CustAddr = Request["CustAddr"];
				userRecord.CustENAddr1 = Request["CustENAddr1"];
				userRecord.CustENAddr2 = Request["CustENAddr2"];
				userRecord.City = Request["City"];
				userRecord.State = Request["State"];
				userRecord.Country = Request["Country"];
				userRecord.CtcSale = Request["CtcSale"];
				userRecord.Tel = Request["Tel"];
				userRecord.Clerk = Request["Clerk"];
				userRecord.PickUpAreaNo = Request["PickUpAreaNo"];
				//userRecord.C_CallDate_ = Request["CallDate"];
				userRecord.EndDate = Request["EndDate"];
				userRecord.Remark2 = Request["Remark2"];
				userRecord.Remark3 = Request["Remark3"];
				userRecord.SectorNo = Request["SectorNo"];
				userRecord.CallType = Request["CallType"];
				userRecord.StatNo = Request["StatNo"];
				userRecord.CallStatNo = Request["CallStatNo"];
				int nlAdd1;
				int.TryParse(Request["add_1"], out nlAdd1);
				userRecord.Add_1 = nlAdd1;
				int nlAdd2;
				int.TryParse(Request["add_2"], out nlAdd2);
				userRecord.Add_2 = nlAdd2;
				int nlAdd3;
				int.TryParse(Request["add_3"], out nlAdd3);
				userRecord.Add_3 = nlAdd3;
				//int nlAdd4;
				//int.TryParse(Request["add_4"], out nlAdd4);
				userRecord.Add_4 = Request["add_4"];
				int nlAdd5;
				int.TryParse(Request["add_5"], out nlAdd5);
				userRecord.Add_5 = nlAdd5;

				//modified 20180208
				//userRecord.HubNo = Request["HubNo"];
				userRecord.CcNo = Request["CcNo"];
				//userRecord.Dest = Request["Dest"];
				userRecord.RedyTime = Request["RedyTime"];
				userRecord.CarID = Request["CarID"];
				int nlWeigLevel;
				int.TryParse(Request["WeigLevel"], out nlWeigLevel);
				userRecord.WeigLevel = nlWeigLevel;
				int nlCocustomTyp;
				int.TryParse(Request["CocustomTyp"], out nlCocustomTyp);
				userRecord.CocustomTyp = nlCocustomTyp;
				float flCharge;
				float.TryParse(Request["Charge"], out flCharge);
				userRecord.Charge = flCharge;
				DateTime dtRedy = nowTimeStamp;
				if ((Request["RedyDate"] != null) && (Request["RedyDate"] != ""))
				{
					DateTime.TryParse(Request["RedyDate"], out dtRedy);
					userRecord.RedyDate = dtRedy;
				}
				userRecord.IsRedy = Request["IsRedy"] == "true" ? true : false;
				bool blTarget;
				blTarget = (Request["d_IsDesp"] == "true") ? true : false;
				if (blTarget != userRecord.IsDesp)
				{
					userRecord.IsDesp = blTarget;
					if (blTarget == true)
					{
						userRecord.ShdetBy = User.Identity.Name;
						userRecord.ShdetDate = nowTimeStamp;
					}
				}
				if (Request["ReplyComment"] != "" && userRecord.ReplyComment != Request["ReplyComment"])
				{
					userRecord.ReplyComment = Request["ReplyComment"];
					userRecord.IsReply = true;
					userRecord.ReplyBy = User.Identity.Name;
					userRecord.ReplyDate = nowTimeStamp;
				}
				//shdetFix
				//blTarget = (Request["d_IsCancel"] == "true") ? true : false;
				//if (blTarget != userRecord.IsCancel)
				//{
				//	userRecord.IsCancel = blTarget;
				//	if (blTarget == true)
				//	{
				//		userRecord.CancelBy = User.Identity.Name;
				//		userRecord.CancelDate = nowTimeStamp;
				//	}
				//}
				//blTarget = (Request["d_IsFinish"] == "true") ? true : false;
				//if (blTarget != userRecord.IsFinish)
				//{
				//	userRecord.IsFinish = blTarget;
				//	if (blTarget == true)
				//	{
				//		userRecord.FinishBy = User.Identity.Name;
				//		userRecord.FinishDate = nowTimeStamp;
				//	}
				//}

				//以下系統自填
				userRecord.UpdatedDate = DateTime.Now;
				userRecord.UpdatedBy = User.Identity.Name;

				var prod = db.ShdetProd.Where(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sDtlNo == data.sNo);
				foreach (var i in prod)
				{
					i.CarID = data.CarID;
					i.RedyDate = data.RedyDate;
					db.Entry(i).State = EntityState.Modified;
				}

				try
				{
					db.Entry(userRecord).State = EntityState.Modified;
					db.SaveChanges();
					int retVal = 0;
					//retVal = (CheckAllDetailFinished(userRecord.ShdetNo) == true) ? retVal + 1 : retVal;
					if (retVal == 0)
						result = new JObject { { "message", "ok" } };
					else
						result = new JObject { { "message", retVal.ToString() } };
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}
				catch (Exception e)
				{
					result = new JObject { { "errorMsg", e.Message } };
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}
			}

			result = new JObject { { "errorMsg", "找不到資料!" } };
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult EditShdet3(ShdetProd data)
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{ }
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}
			var result = new ResultHelper();
			var userRecord = db.ShdetProd.FirstOrDefault(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sDtlNo == data.sDtlNo && x.sNo == data.sNo && x.IsDelete == false);
			if (userRecord != null)
			{
				DateTime nowTime = DateTime.Now;

				userRecord.HubNo = data.HubNo;
				userRecord.CcNo = data.CcNo;
				userRecord.Dest = data.Dest;
				userRecord.RedyTime = data.RedyTime;
				userRecord.Remark1 = data.Remark1;
				userRecord.Remark3 = data.Remark3;
				userRecord.SectorNo = data.SectorNo;
				userRecord.CallType = data.CallType;
				userRecord.StatNo = data.StatNo;
				userRecord.ReplyComment = data.ReplyComment;
				userRecord.Pcs = data.Pcs;
				userRecord.WeigLevel = data.WeigLevel;
				userRecord.CocustomTyp = data.CocustomTyp;
				userRecord.iTotNum = data.iTotNum;
				userRecord.fLen = data.fLen;
				userRecord.fHeight = data.fHeight;
				userRecord.fWidth = data.fWidth;
				userRecord.Weig = data.Weig;
				userRecord.Charge = data.Charge;

				if (data.RedyDate != null)
					userRecord.RedyDate = data.RedyDate;
				else
					userRecord.RedyDate = nowTime;

				//shdetFix
				//if (userRecord.IsDesp != data.IsDesp)
				//{
				//    userRecord.IsDesp = data.IsDesp;
				//    if (data.IsDesp == true)
				//    {
				//        userRecord.ShdetBy = User.Identity.Name;
				//        userRecord.ShdetDate = nowTime;
				//    }
				//}

				//if (userRecord.IsCancel != data.IsCancel && data.IsCancel != null)
				//{
				//	userRecord.IsCancel = data.IsCancel;
				//	if (data.IsCancel == true)
				//	{
				//		if (data.SheetNo != null && data.SSNo != null)
				//		{
				//			result.Ok = DataModifyResultType.Warning;
				//			result.Message = "貨物已轉理貨，無法取消";
				//			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				//		}
				//		userRecord.CancelBy = User.Identity.Name;
				//		userRecord.CancelDate = nowTime;
				//	}
				//}

				//if (userRecord.IsReply != data.IsReply)
				//{
				//    userRecord.IsReply = data.IsReply;
				//    if (data.IsReply == true)
				//    {
				//        userRecord.ReplyBy = User.Identity.Name;
				//        userRecord.ReplyDate = nowTime;
				//    }
				//}

				//if (userRecord.IsFinish != data.IsFinish)
				//{
				//    userRecord.IsFinish = data.IsFinish;
				//    if (data.IsFinish == true)
				//    {
				//        userRecord.FinishBy = User.Identity.Name;
				//        userRecord.FinishDate = nowTime;
				//    }
				//}

				//以下系統自填
				userRecord.UpdatedDate = DateTime.Now;
				userRecord.UpdatedBy = User.Identity.Name;
				try
				{
					db.Entry(userRecord).State = EntityState.Modified;
					db.SaveChanges();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";

					int retVal = 0;
					//retVal = (CheckAllProdFinished(userRecord.ShdetNo, userRecord.CustNo, userRecord.sDtlNo) == true) ? retVal + 2 : retVal;
					//retVal = (CheckAllDetailFinished(userRecord.ShdetNo) == true) ? retVal + 1 : retVal;
					if (retVal == 0)
						result.Message = "OK";
					else
						result.Message = retVal.ToString();
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
				result.Message = "no data find";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult DeleteShdet(string id)
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString())) { }
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}
			var result = new ResultHelper();
			var userRecord = db.ShdetHeader.FirstOrDefault(x => x.ShdetNo == id);
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
					result.Ok = DataModifyResultType.Success;
					result.Message = e.Message;
				}
			}
			else
			{
				result.Ok = DataModifyResultType.Success;
				result.Message = "找不到資料!";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult DeleteShdet2()
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{

			}
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}

			int nlSNo;
			JObject result;
			int.TryParse(Request["sNo"], out nlSNo);
			ShdetDetail userRecord = db.ShdetDetail.Find(Request["ShdetNo"], Request["CustNo"], nlSNo);

			if (userRecord != null)
			{
				//以下系統自填
				userRecord.DeletedDate = DateTime.Now;
				userRecord.DeletedBy = User.Identity.Name;
				userRecord.IsDelete = true;
				try
				{
					//db.SYS_User.Add(userRecord);
					db.Entry(userRecord).State = EntityState.Modified;
					db.SaveChanges();
					//CheckAllDetailFinished(userRecord.ShdetNo);
					result = new JObject { { "message", "ok" } };
				}
				catch (Exception e)
				{
					result = new JObject { { "errorMsg", e.Message } };
				}
			}

			result = new JObject { { "errorMsg", "找不到資料!" } };
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult DeleteShdet3(string ShdetNo, string CustNo, int sDtlNo, int sNo)
		{
			//權限控管
			if (WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString())) { }
			else
			{
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			}
			var result = new ResultHelper();
			var userRecord = db.ShdetProd.FirstOrDefault(x => x.ShdetNo == ShdetNo && x.CustNo == CustNo && x.sDtlNo == sDtlNo && x.sNo == sNo);

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

					//Todo Alan
					int retVal = 0;
					//retVal = (CheckAllProdFinished(userRecord.ShdetNo, userRecord.CustNo, userRecord.sDtlNo) == true) ? retVal + 2 : retVal;
					//retVal = (CheckAllDetailFinished(userRecord.ShdetNo) == true) ? retVal + 1 : retVal;
					if (retVal == 0)
						result.Message = "OK";
					else
						result.Message = retVal.ToString();
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
		public ActionResult GetGridJSON1(ShdetMD data, int page = 1, int rows = 40, string from = "", bool searchType = true, DateTime? sredy_date = null, DateTime? eredy_date = null, string sredy_time = null, string eredy_time = null, string QueryNo = null)
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

			var shdetData = from h in db.ShdetHeader.Where(x => x.IsDelete == false)
							join d in db.ShdetDetail.Where(x => x.IsDelete == false)
							on new { h.ShdetNo, h.CustNo } equals new { d.ShdetNo, d.CustNo } into ps
							from d in ps.DefaultIfEmpty()

							join c in db.ORG_Cust
							on h.CustNo equals c.CustNo into ps2
							from c in ps2.DefaultIfEmpty()

							join se in db.ORG_Sector
							on d.SectorNo equals se.SectorNo into ps3
							from se in ps3.DefaultIfEmpty()

							join hub in db.ORG_Hub
							on h.HubNo equals hub.HubNo into ps4
							from hub in ps4.DefaultIfEmpty()

							join cs in db.ORG_Stat
							on d.CallStatNo equals cs.StatNo into ps5
							from cs in ps5.DefaultIfEmpty()

							join s in db.ORG_Stat
							on d.StatNo equals s.StatNo into ps6
							from s in ps6.DefaultIfEmpty()

							join u in db.SYS_User
							on h.CreatedBy equals u.Account into ps7
							from u in ps7.DefaultIfEmpty()

							join updateuser in db.SYS_User.Where(x => x.IsDelete == false)
							on d.UpdatedBy equals updateuser.Account into ps8
							from updateuser in ps8.DefaultIfEmpty()

							join b in db.Bill_Lading.Where(x => x.IsDelete == false)
							on h.LadingNo equals b.LadingNo into ps9
							from b in ps9.DefaultIfEmpty()
								//shdetFix
							select new ShdetMD()
							{
								LadingNo_Type = h.LadingNo == null ? h.ShdetNo : b.LadingNo_Type,
								ShdetNo = h.ShdetNo,
								CustNo = h.CustNo,
								CustCHName = h.CustCHName,
								Remark5 = c == null ? null : c.Remark5,
								Dest = h.Dest,
								IsDesp = h.IsDesp,
								//IsCancel = h.IsCancel,
								//IsReply = h.IsReply,
								//IsFinish = h.IsFinish,
								ShdetBy = h.ShdetBy,
								ShdetDate = h.ShdetDate,
								//CancelBy = h.CancelBy,
								//CancelDate = h.CancelDate,
								//ReplyBy = h.ReplyBy,
								//ReplyDate = h.ReplyDate,
								//FinishBy = h.FinishBy,
								//FinishDate = h.FinishDate,
								CreatedBy = u.UserName,
								CreatedByNo = u.Account,
								CreatedDate = h.CreatedDate,
								UpdatedBy = updateuser.UserName,
								UpdatedDate = h.UpdatedDate,
								DeletedBy = h.DeletedBy,
								DeletedDate = h.DeletedDate,
								IsDelete = h.IsDelete,
								Clerk = h.Clerk,
								ReserveDate = h.ReserveDate,
								SDate = h.SDate,
								RedyDate = d == null ? null : d.RedyDate,
								HubNo = h.HubNo,
								HubName = hub == null ? null : hub.HubName,
								CallStatNo = d == null ? null : d.CallStatNo,
								CallStatName = cs == null ? null : cs.StatName,
								StatNo = d == null ? null : d.StatNo,
								StatName = s == null ? null : s.StatName,
								sNo = d == null ? 0 : d.sNo,
								RedyTime = d == null ? null : d.RedyTime,
								IsRedy = d == null ? null : d.IsRedy,
								SectorNo = se == null ? null : se.SectorNo,
								SectorName = se == null ? null : se.SectorName,
								CocustomTyp = d == null ? null : d.CocustomTyp,
								CocustomTypStr = d == null ? null : d.CocustomTyp == 0 ? "不報關" : d.CocustomTyp == 1 ? "正式報關" : d.CocustomTyp == 2 ? "簡易報關" : d.CocustomTyp == 3 ? "正式報關+後段核銷" : d.CocustomTyp == 4 ? "簡易報關+後段核銷" : d.CocustomTyp == 5 ? "不報關+後段核銷" : d.CocustomTyp == 6 ? "其他" : " ",
							};
			if (data.CustCHName.IsNotEmpty())
				shdetData = shdetData.Where(x => x.CustCHName.Contains(data.CustCHName));

			var shdet = shdetData.ToList().Distinct(new ShdetCompare()).Select((r, i) => new ShdetMD()
			{
				RowNumber = i,
				LadingNo_Type = r.LadingNo_Type,
				ShdetNo = r.ShdetNo,
				CustNo = r.CustNo,
				CustCHName = r.CustCHName,
				Remark5 = r.Remark5,
				Dest = r.Dest,
				IsDesp = r.IsDesp,
				//IsCancel = r.IsCancel,
				//IsReply = r.IsReply,
				//IsFinish = r.IsFinish,
				ShdetBy = r.ShdetBy,
				ShdetDate = r.ShdetDate,
				//CancelBy = r.CancelBy,
				//CancelDate = r.CancelDate,
				//ReplyBy = r.ReplyBy,
				//ReplyDate = r.ReplyDate,
				//FinishBy = r.FinishBy,
				//FinishDate = r.FinishDate,
				CreatedBy = r.CreatedBy,
				CreatedByNo = r.CreatedByNo,
				CreatedDate = r.CreatedDate,
				UpdatedBy = r.UpdatedBy,
				UpdatedDate = r.UpdatedDate,
				DeletedBy = r.DeletedBy,
				DeletedDate = r.DeletedDate,
				IsDelete = r.IsDelete,
				Clerk = r.Clerk,
				ReserveDate = r.ReserveDate,
				SDate = r.SDate,
				RedyDate = r.RedyDate,
				HubNo = r.HubNo,
				HubName = r.HubName,
				CallStatNo = r.CallStatNo,
				CallStatName = r.CallStatName,
				StatNo = r.StatNo,
				StatName = r.StatName,
				sNo = r.sNo,
				RedyTime = r.RedyTime,
				SectorNo = r.SectorNo,
				SectorName = r.SectorName,
				CocustomTyp = r.CocustomTyp,
				CocustomTypStr = r.CocustomTypStr,
			});

			if (from == "正式調派")
				shdet = shdet.Where(x => x.IsDesp == true);

			shdet = shdet.Where(x => statNoList.Contains(x.CallStatNo) || statNoList.Contains(x.StatNo));

			if (QueryNo.IsNotEmpty())
				shdet = shdet.Where(x => x.ShdetNo.Contains(QueryNo.Trim()));
			else
			{
				if (data.LadingNo_Type.IsNotEmpty())
				{
					shdet = shdet.Where(x => x.LadingNo_Type != null);
					shdet = shdet.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
				}
				if (data.ShdetNo.IsNotEmpty())
					shdet = shdet.Where(x => x.ShdetNo.Contains(data.ShdetNo));
				if (data.HubName.IsNotEmpty())
				{
					shdet = shdet.Where(x => x.HubName != null);
					shdet = shdet.Where(x => x.HubName.Contains(data.HubName));
				}
				if (data.StatName.IsNotEmpty())
				{
					shdet = shdet.Where(x => x.StatName != null);
					shdet = shdet.Where(x => x.StatName.Contains(data.StatName));
				}
				if (data.CallStatName.IsNotEmpty())
				{
					shdet = shdet.Where(x => x.CallStatName != null);
					shdet = shdet.Where(x => x.CallStatName.Contains(data.CallStatName));
				}
				if (data.CustNo.IsNotEmpty())
					shdet = shdet.Where(x => x.CustNo.Contains(data.CustNo));

				if (data.UpdatedBy.IsNotEmpty())
					shdet = shdet.Where(x => x.UpdatedBy == data.UpdatedBy);
				if (data.CreatedBy.IsNotEmpty())
					shdet = shdet.Where(x => x.CreatedBy == data.CreatedBy);

				//if (data.CustCHName.IsNotEmpty())
				//	shdet = shdet.Where(x => x.CustCHName.Contains(data.CustCHName));

				if (data.SectorNo.IsNotEmpty())
					shdet = shdet.Where(x => x.SectorNo == data.SectorNo);

				if (data.SectorName.IsNotEmpty())
					shdet = shdet.Where(x => x.SectorName == data.SectorName);


				if (data.IsDesp != null)
					shdet = shdet.Where(x => x.IsDesp == data.IsDesp);

				if (searchType)
				{
					if (sredy_date != null && eredy_date != null)
						shdet = shdet.Where(x => (x.ReserveDate <= Convert.ToDateTime(eredy_date).AddDays(1) && sredy_date <= x.ReserveDate));
				}
				else
				{
					if (sredy_date != null && eredy_date != null)
						shdet = shdet.Where(x => x.RedyDate <= eredy_date && sredy_date <= x.RedyDate);

					if (sredy_time.IsNotEmpty() && eredy_time.IsNotEmpty() && sredy_time.CompareTo(eredy_time) < 0)
					{
						shdet = shdet.Where(x => x.RedyTime != null);
						shdet = shdet.Where(x => x.RedyTime.CompareTo(eredy_time) <= 0 && x.RedyTime.CompareTo(sredy_time) >= 0);
					}
				}
				if (data.CocustomTypStr.IsNotEmpty())
					shdet = shdet.Where(x => x.CocustomTypStr == data.CocustomTypStr);
			}
			int records = shdet.Count();
			shdet = shdet.OrderBy(x => x.ShdetNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = shdet,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		public class ShdetCompare : IEqualityComparer<ShdetMD>
		{
			public bool Equals(ShdetMD x, ShdetMD y)
			{
				return (x.ShdetNo == y.ShdetNo && x.CustNo == y.CustNo && x.StatNo == y.StatNo && x.CallStatNo == y.CallStatNo && x.RedyDate == y.RedyDate);
			}

			public int GetHashCode(ShdetMD obj)
			{
				int hash = 17;
				hash = hash * 23 + (obj.ShdetNo ?? "").GetHashCode();
				hash = hash * 23 + (obj.CustNo ?? "").GetHashCode();
				hash = hash * 23 + (obj.StatNo ?? "").GetHashCode();
				hash = hash * 23 + (obj.CallStatNo ?? "").GetHashCode();
				hash = hash * 23 + obj.RedyDate.GetHashCode();
				return hash;
			}
		}

		[Authorize]
		public ActionResult GetGridJSON2(int? sNo, int page = 1, int rows = 40, string ShdetNo = "", string CustNo = "", string LadingNo = "")
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

			var data = from d in db.ShdetDetail
					   join s in db.ORG_Sector
					   on d.SectorNo equals s.SectorNo into ps
					   from s in ps.DefaultIfEmpty()
					   join v in db.ORG_Vehicle
					   on d.CarID equals v.CarID into ps1
					   from v in ps1.DefaultIfEmpty()
					   join h in db.ShdetHeader
					   on d.ShdetNo equals h.ShdetNo into ps2
					   from h in ps2.DefaultIfEmpty()
					   where d.IsDelete == false && (statNoList.Contains(d.StatNo) || statNoList.Contains(d.CallStatNo))
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
						   d.IsDesp,
						   d.IsCancel,
						   d.IsReply,
						   d.IsRedy,
						   d.IsFinish,
						   d.ShdetBy,
						   d.ShdetDate,
						   d.CancelBy,
						   d.CancelDate,
						   d.ReplyBy,
						   d.ReplyDate,
						   d.ReplyComment,
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
						   d.FinishBy,
						   d.FinishDate,
						   d.PhoneCheckTime,
						   d.Status,
						   d.StatusTime,
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

		[Authorize]
		public ActionResult GetGridJSON3(int page = 1, int rows = 40, string ShdetNo = "", string CustNo = "", int sDtlNo = 0)
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

		[Authorize]
		public ActionResult GetGridJSON4(int page = 1, int rows = 40, string start_date = "", string end_date = "")
		{
			int total_count;

			start_date = (start_date == "") ? WebSiteHelper.MyFormatDateString(DateTime.Now) : start_date;
			end_date = (end_date == "") ? WebSiteHelper.MyFormatDateString(DateTime.Now) : end_date;
			DateTime sDate = DateTime.Now;
			DateTime eDate = DateTime.Now;
			DateTime.TryParse(start_date, out sDate);
			DateTime.TryParse(end_date, out eDate);
			var prod = from pd in db.ShdetProd
					   join pm in db.ShdetHeader on pd.ShdetNo equals pm.ShdetNo into headers
					   from pm in headers
					   join dt in db.ShdetDetail on new { no = pd.ShdetNo, sno = pd.sDtlNo } equals new { no = dt.ShdetNo, sno = dt.sNo } into details
					   from dt in details
					   where pd.IsDelete == false && DbFunctions.TruncateTime(pd.RedyDate) <= DbFunctions.TruncateTime(eDate) && DbFunctions.TruncateTime(sDate) <= DbFunctions.TruncateTime(pd.RedyDate)
					   select new { pd, dt, pm };

			total_count = prod.Count();
			prod = prod.OrderBy(o => o.pd.ShdetNo).Skip((page - 1) * rows).Take(rows);
			//var users = db.SYS_User.OrderBy(x => x.Account);
			JArray ja = new JArray();
			Dictionary<string, string> userDictionary = new Dictionary<string, string>();
			var userInfo = from user in db.SYS_User
						   select user;
			foreach (var userItem in userInfo)
			{
				if (!userDictionary.ContainsKey(userItem.Account))
					userDictionary.Add(userItem.Account, userItem.UserName);
			}

			var sector = from s in db.ORG_Sector
						 where s.IsDelete == false
						 select s;

			var sectorDic = new Dictionary<string, string>();
			foreach (var i in sector)
				sectorDic.Add(i.SectorNo, i.SectorName);

			foreach (var item in prod)
			{
				var tempCancelBy = item.pd == null ? null : ((item.pd.CancelBy == null) ? "" : item.pd.CancelBy);
				var tempUpdatedBy = item.pd == null ? null : ((item.pd.UpdatedBy == null) ? "" : item.pd.UpdatedBy);
				var tempDeleteBy = item.pd == null ? null : ((item.pd.DeletedBy == null) ? "" : item.pd.DeletedBy);
				var tempShdetBy = item.pd == null ? null : ((item.pd.ShdetBy == null) ? "" : item.pd.ShdetBy);
				var tempCreatedBy = item.pd == null ? null : ((item.pd.CreatedBy == null) ? "" : item.pd.CreatedBy);
				var tempFinishBy = item.pd == null ? null : ((item.pd.FinishBy == null) ? "" : item.pd.FinishBy);
				var tempReplyBy = item.pd == null ? null : ((item.pd.ReplyBy == null) ? "" : item.pd.ReplyBy);
				var tempSectorNo = (item.dt.SectorNo == null) ? "" : item.dt.SectorNo;

				var jobj = new JObject {

					{"CustCHName",item.pm.CustCHName},
					{"RedyTime",item.pd.RedyTime},
					{"PickUpAreaNo",item.dt.PickUpAreaNo},
					{"SectorNo",item.dt.SectorNo},
					{"SectorName",(item.dt == null)? null : (sectorDic.ContainsKey(tempSectorNo))? sectorDic[tempSectorNo]:""},
					{"Remark1",item.pd.Remark1},
					{"Remark3",item.pd.Remark3},

					{"DtStatNo",item.dt.StatNo},
					{"ProdStatNo",item.pd.StatNo},

					{"ShdetNo",item.pm.ShdetNo},
					{"CustNo",item.pm.CustNo},

					{"IsDesp",item.pd.IsDesp},
					{"IsCancel",item.pd.IsCancel},
					{"IsReply",item.pd.IsReply},
					{"IsFinish",item.pd.IsFinish},
					{"ShdetBy",(userDictionary.ContainsKey(tempShdetBy))?userDictionary[tempShdetBy]:""  },
					{"ShdetDate",WebSiteHelper.MyFormatDateString(item.pd.ShdetDate)},
					{"CancelBy",(userDictionary.ContainsKey(tempCancelBy))?userDictionary[tempCancelBy]:""  },
					{"CancelDate",WebSiteHelper.MyFormatDateString(item.pd.CancelDate)},
					{"ReplyBy",(userDictionary.ContainsKey(tempReplyBy))?userDictionary[tempReplyBy]:""  },
					{"ReplyDate",WebSiteHelper.MyFormatDateString(item.pd.ReplyDate)},
					{"ReplyComment",item.pd.ReplyComment},
					{"FinishBy",(userDictionary.ContainsKey(tempFinishBy))?userDictionary[tempFinishBy]:""  },
					{"FinishDate",WebSiteHelper.MyFormatDateString(item.pd.FinishDate)},
					{"CreatedDate",WebSiteHelper.MyFormatDateString(item.pd.CreatedDate) },
					{"CreatedBy",(userDictionary.ContainsKey(tempCreatedBy))?userDictionary[tempCreatedBy]:""   },
					{"UpdatedDate" ,WebSiteHelper.MyFormatDateString(item.pd.UpdatedDate)},
					{"UpdatedBy",(userDictionary.ContainsKey(tempUpdatedBy))?userDictionary[tempUpdatedBy]:""   },
					{"DeletedDate",WebSiteHelper.MyFormatDateString(item.pd.DeletedDate) },
					{"DeletedBy",(userDictionary.ContainsKey(tempDeleteBy))?userDictionary[tempDeleteBy]:""   },
					{"IsDelete",item.pd.IsDelete },

				};
				ja.Add(jobj);
			}

			JObject result = new JObject {
				{"total",total_count},
				{"rows",ja}
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetCreateBy(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null, bool searchType = true)
		{
			var dataList = new List<ShdetMD>();
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;
			var eDate2 = end_date.Value.Date.AddDays(1);
			var shdetData = from d in db.ShdetDetail.Where(x => x.IsDelete == false)
							join cUser in db.SYS_User
							on d.CreatedBy equals cUser.Account into ps
							from cUser in ps.DefaultIfEmpty()
							join h in db.ShdetHeader.Where(x => x.IsDelete == false)
							on d.ShdetNo equals h.ShdetNo into ps2
							from h in ps2.DefaultIfEmpty()
							select new
							{
								RedyDate = d.RedyDate,
								ReserveDate = h.ReserveDate,
								CreatedBy = cUser.UserName,
								CreatedDate = d.CreatedDate,
							};

			var shdetGData = searchType == true ?
				from d in shdetData
				where d.ReserveDate <= eDate2 && sDate <= d.ReserveDate
				group d by new { d.CreatedBy } :
				from d in shdetData
				where d.RedyDate <= eDate && sDate <= d.RedyDate
				group d by new { d.CreatedBy };

			foreach (var s in shdetGData)
			{
				var CData = s.OrderByDescending(x => x.CreatedDate).Last();
				var temp = new ShdetMD
				{
					RedyDate = CData.RedyDate,
					ReserveDate = CData.ReserveDate,
					CreatedBy = CData.CreatedBy,
					CreatedDate = CData.CreatedDate,
				};
				dataList.Add(temp);
			}

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = dataList,
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetUpdatedBy(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null, bool searchType = true)
		{
			var dataList = new List<ShdetMD>();
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;
			var eDate2 = end_date.Value.Date.AddDays(1);
			var shdetData = from d in db.ShdetDetail.Where(x => x.IsDelete == false && x.UpdatedBy != null)
							join cUser in db.SYS_User
							on d.UpdatedBy equals cUser.Account into ps
							from cUser in ps.DefaultIfEmpty()
							join h in db.ShdetHeader.Where(x => x.IsDelete == false)
							on d.ShdetNo equals h.ShdetNo into ps2
							from h in ps2.DefaultIfEmpty()
							select new
							{
								RedyDate = d.RedyDate,
								ReserveDate = h.ReserveDate,
								UpdatedBy = cUser.UserName,
								UpdatedDate = d.UpdatedDate,
								CreatedDate = h.CreatedDate,
							};

			var shdetGData = searchType == true ?
				from d in shdetData
				where d.ReserveDate <= eDate2 && sDate <= d.ReserveDate
				group d by new { d.UpdatedBy } :
				from d in shdetData
				where d.RedyDate <= eDate && sDate <= d.RedyDate
				group d by new { d.UpdatedBy };

			foreach (var s in shdetGData)
			{
				var CData = s.OrderByDescending(x => x.UpdatedDate).Last();
				var temp = new ShdetMD
				{
					RedyDate = CData.RedyDate,
					ReserveDate = CData.ReserveDate,
					UpdatedBy = CData.UpdatedBy,
					UpdatedDate = CData.UpdatedDate,
					CreatedDate = CData.CreatedDate
				};
				dataList.Add(temp);
			}

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = dataList,
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON_Sector(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null, bool searchType = true)
		{
			var dataList = new List<ShdetMD>();
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;
			var eDate2 = end_date.Value.Date.AddDays(1);
			var shdetData = from d in db.ShdetDetail.Where(x => x.IsDelete == false && x.SectorNo != null)
							join s in db.ORG_Sector
							on d.SectorNo equals s.SectorNo into ps
							from s in ps.DefaultIfEmpty()
							join h in db.ShdetHeader.Where(x => x.IsDelete == false)
							on d.ShdetNo equals h.ShdetNo into ps2
							from h in ps2.DefaultIfEmpty()
							select new
							{
								RedyDate = d.RedyDate,
								ReserveDate = h.ReserveDate,
								SectorNo = s.SectorNo,
								SectorName = s.SectorName,
								CreatedDate = h.CreatedDate,
							};

			var shdetGData = searchType == true ?
				from d in shdetData
				where d.ReserveDate <= eDate2 && sDate <= d.ReserveDate
				group d by new { d.SectorName } :
				from d in shdetData
				where d.RedyDate <= eDate && sDate <= d.RedyDate
				group d by new { d.SectorName };

			foreach (var s in shdetGData)
			{
				var CData = s.OrderByDescending(x => x.CreatedDate).Last();
				var temp = new ShdetMD
				{
					RedyDate = CData.RedyDate,
					ReserveDate = CData.ReserveDate,
					SectorNo = CData.SectorNo,
					SectorName = CData.SectorName,
					CreatedDate = CData.CreatedDate,
				};
				dataList.Add(temp);
			}

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = dataList,
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON_HistoryShdetDetail(ShdetDetail filter, string id = "", int page = 1, int rows = 40)
		{
			if (id == "") { return Content(String.Format(slLogoutHtml, "無資料...", Request.ApplicationPath)); }

			var dtlList = new List<ShdetDetail>();

			var shdetdetail = from s in db.ShdetDetail
							  join sec in db.ORG_Sector
							  on s.SectorNo equals sec.SectorNo into ps
							  from sec in ps.DefaultIfEmpty()
							  join stat in db.ORG_Stat
							  on s.StatNo equals stat.StatNo into ps1
							  from stat in ps1.DefaultIfEmpty()
							  join cs in db.ORG_Stat
							  on s.CallStatNo equals cs.StatNo into ps2
							  from cs in ps2.DefaultIfEmpty()
							  join v in db.ORG_Vehicle
							  on s.CarID equals v.CarID into ps3
							  from v in ps3.DefaultIfEmpty()
							  join p in db.ORG_PickUpAreaAddress
							  on s.Code7 equals p.Code7 into ps4
							  from p in ps4.DefaultIfEmpty()
							  where s.IsDelete == false && s.CustNo == id
							  //orderby s.ShdetNo descending, s.CreatedDate descending
							  select new
							  {
								  Type = "歷史",
								  ShdetNo = s.ShdetNo,
								  CustNo = s.CustNo,
								  sNo = s.sNo,
								  CarryName = s.CarryName,
								  Code5 = s.Code5,
								  Code7 = s.Code7,
								  Add_1 = s.Add_1,
								  Add_2 = s.Add_2,
								  Add_3 = s.Add_3,
								  Add_4 = s.Add_4,
								  Add_5 = s.Add_5,
								  Add_6 = s.Add_6,
								  CustAddr = s.CustAddr,
								  CustAddrFull = s.CustAddr + (s.Add_1 == 0 ? null : (s.Add_1 + "段")) + (s.Add_2 == 0 ? null : (s.Add_2 + "巷")) + (s.Add_3 == 0 ? null : (s.Add_3 + "弄")) + (s.Add_4 == "" || s.Add_4 == null ? null : (s.Add_4 + "號")) + (s.Add_5 == 0 ? null : (s.Add_5 + "樓")) + (s.Add_6 == null || s.Add_6 == "" ? null : s.Add_6),
								  CustENAddr1 = s.CustENAddr1,
								  CustENAddr2 = s.CustENAddr2,
								  City = s.City,
								  State = s.State,
								  Country = s.Country,
								  CtcSale = s.CtcSale,
								  Tel = s.Tel,
								  Clerk = s.Clerk,
								  PickUpAreaNo = p.PickUpAreaNo,
								  CancelDate = s.CancelDate,
								  EndDate = s.EndDate,
								  Remark2 = s.Remark2,
								  Remark3 = s.Remark3,
								  SectorNo = s.SectorNo,
								  SectorName = sec == null ? null : sec.SectorName,
								  CallType = s.CallType,
								  s.StatNo,
								  StatName = stat == null ? null : stat.StatName,
								  s.CallStatNo,
								  CallStatName = cs == null ? null : cs.StatName,
								  v.CarID,
								  RedyDate = s.RedyDate,
								  RedyTime = s.RedyTime,
								  CreatedDate = s.CreatedDate
							  };

			var dtlgroup = from d in shdetdetail
						   group d by new { d.CarryName, d.CustAddr };
			foreach (var i in dtlgroup)
			{
				var data = i.OrderBy(x => x.CreatedDate).Last();
				var RedyDT = Convert.ToDateTime(Convert.ToDateTime(data.RedyDate).ToString("yyyy-MM-dd") + " " + data.RedyTime);
				var sectorNoData = "";
				var sectorNameData = "";
				var sData = db.ORG_Sector.FirstOrDefault(x => x.PickUpAreaNo == data.PickUpAreaNo && x.IsServer == true && x.IsDelete == false);
				if (sData != null)
				{
					sectorNoData = sData.SectorNo;
					sectorNameData = sData.SectorName;
				}
				else
				{
					sectorNoData = data.SectorNo;
					sectorNameData = data.SectorName;
				}

				var sectorNo = "";
				var sectorName = "";
				var plateNO = "";
				var carID = "";
				var callStatNo = "";
				var callStatName = "";
				var endTime = "";
				var pickUpAreaName = "";

				if (sectorNoData != "" && db.ORG_Sector.Where(x => x.SectorNo == sectorNoData).Select(x => x.IsServer).FirstOrDefault() == true)
				{

					var sector = from a in db.ORG_SectorAbsent.Where(x => x.IsDelete == false && (DbFunctions.TruncateTime(x.StartDT) <= DbFunctions.TruncateTime(RedyDT)) && (DbFunctions.TruncateTime(x.EndDT) >= DbFunctions.TruncateTime(RedyDT)))
								 where a.IsDelete == false && a.SectorNo == sectorNoData
								 select new
								 {
									 a.SectorNo,
									 a.AgentSectorNo,
								 };
					if (sector.Count() > 0)
					{
						sectorNo = sector.Select(x => x.AgentSectorNo).FirstOrDefault();
						if (sectorNo != null)
							sectorName = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == sectorNo).SectorName;
						else
						{
							sectorNo = data.SectorNo;
							sectorName = data.SectorName;
						}
					}
					else
					{
						sectorNo = data.SectorNo;
						sectorName = data.SectorName;
					}
					plateNO = db.ORG_Sector.Where(s => s.SectorNo == sectorNo).Select(s => s.PlateNO).FirstOrDefault();
					carID = db.ORG_Vehicle.Where(v => v.CarNO == plateNO).Select(v => v.CarID).FirstOrDefault();
					callStatNo = db.ORG_Sector.Where(s => s.SectorNo == sectorNo).Select(s => s.StatNo).FirstOrDefault();
					callStatName = db.ORG_Stat.Where(s => s.StatNo == callStatNo).Select(s => s.StatName).FirstOrDefault();
					endTime = db.ORG_Sector.Where(s => s.SectorNo == sectorNo).Select(s => s.EndTime).FirstOrDefault();
				}
				pickUpAreaName = db.ORG_PickUpArea.Where(p => p.PickUpAreaNo == data.PickUpAreaNo).Select(p => p.PickUpAreaName).FirstOrDefault();


				var temp = new ShdetDetail()
				{
					Type = data.Type,
					ShdetNo = data.ShdetNo,
					CustNo = data.CustNo,
					sNo = data.sNo,
					CarryName = data.CarryName,
					Code5 = data.Code5,
					Code7 = data.Code7,
					Add_1 = data.Add_1,
					Add_2 = data.Add_2,
					Add_3 = data.Add_3,
					Add_4 = data.Add_4,
					Add_5 = data.Add_5,
					Add_6 = data.Add_6,
					CustAddr = data.CustAddr,
					CustAddrFull = data.CustAddr + (data.Add_1 == 0 ? null : (data.Add_1 + "段")) + (data.Add_2 == 0 ? null : (data.Add_2 + "巷")) + (data.Add_3 == 0 ? null : (data.Add_3 + "弄")) + (data.Add_4 == "" | data.Add_4 == null ? null : (data.Add_4 + "號")) + (data.Add_5 == 0 ? null : (data.Add_5 + "樓")) + (data.Add_6 == null || data.Add_6 == "" ? null : data.Add_6),
					CustENAddr1 = data.CustENAddr1,
					CustENAddr2 = data.CustENAddr2,
					City = data.City,
					State = data.State,
					Country = data.Country,
					CtcSale = data.CtcSale,
					Tel = data.Tel,
					Clerk = data.Clerk,
					PickUpAreaNo = data.PickUpAreaNo,
					PickUpAreaName = pickUpAreaName,
					CancelDate = data.CancelDate,
					EndDate = endTime,
					Remark2 = data.Remark2,
					Remark3 = data.Remark3,
					SectorNo = sectorNo,
					SectorName = sectorName,
					CallType = data.CallType,
					StatNo = data.StatNo,
					StatName = data.StatName,
					CallStatNo = callStatNo,
					CallStatName = callStatName,
					CarID = carID,
					CreatedDate = data.CreatedDate
				};
				dtlList.Add(temp);
			}

			var custdtl = from d in db.ORG_CustDetail
						  join m in db.ORG_Cust
						  on d.CustNo equals m.CustNo
						  join a in db.ORG_PickUpArea
						  on m.PickUpAreaID equals a.ID into ps
						  from a in ps.DefaultIfEmpty()
						  where d.CustNo == id && d.IsDelete == false
						  select new { a, d, m };

			foreach (var i in custdtl)
			{
				var pickUpAreaNo = db.ORG_PickUpAreaAddress.Where(x => x.Code5 == i.d.Code5).Select(x => x.PickUpAreaNo).FirstOrDefault();
				var pickUpAreaName = db.ORG_PickUpArea.Where(x => x.PickUpAreaNo == pickUpAreaNo).Select(x => x.PickUpAreaName).FirstOrDefault();
				var temp = new ShdetDetail()
				{
					Type = "主檔",
					ShdetNo = null,
					CustNo = i.d.CustNo,
					CarryName = i.d.CarryName,
					Code5 = i.d.Code5,
					Add_1 = i.d.Add_1,
					Add_2 = i.d.Add_2,
					Add_3 = i.d.Add_3,
					Add_4 = i.d.Add_4,
					Add_5 = i.d.Add_5,
					Add_6 = i.d.Add_6,
					CustAddr = i.d.CustAddr,
					CustAddrFull = i.d.CustAddr + (i.d.Add_1 == 0 ? null : (i.d.Add_1 + "段")) + (i.d.Add_2 == 0 ? null : (i.d.Add_2 + "巷")) + (i.d.Add_3 == 0 ? null : (i.d.Add_3 + "弄")) + (i.d.Add_4 == "" || i.d.Add_4 == null ? null : (i.d.Add_4 + "號")) + (i.d.Add_5 == 0 ? null : (i.d.Add_5 + "樓")) + (i.d.Add_6 == null || i.d.Add_6 == "" ? null : i.d.Add_6),
					CustENAddr1 = i.d.CustENAddr1,
					CustENAddr2 = i.d.CustENAddr2,
					City = i.d.City,
					State = i.d.State,
					Country = i.d.Country,
					CtcSale = i.d.CtcSale,
					PickUpAreaNo = pickUpAreaNo,
					PickUpAreaName = pickUpAreaName,
					Tel = "",
					CreatedDate = DateTime.Now,
				};
				dtlList.Add(temp);
			}


			var shdet = dtlList as IEnumerable<ShdetDetail>;

			if (filter.CarryName.IsNotEmpty())
				shdet = shdet.Where(x => x.CarryName.Contains(filter.CarryName));
			if (filter.CustAddrFull.IsNotEmpty())
				shdet = shdet.Where(x => x.CustAddrFull.Contains(filter.CustAddrFull));
			if (filter.Tel.IsNotEmpty())
				shdet = shdet.Where(x => x.Tel.Contains(filter.Tel));

			var records = shdet.Count();
			shdet = shdet.Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = shdet,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};

			return Content(JsonConvert.SerializeObject(result), "application/json");
		}


		[Authorize]
		public ActionResult PickUpAreaNoChanged(ORG_Sector data, int page = 1, int rows = 40)
		{
			var sectorNoData = "";
			var sectorNameData = "";
			var RedyDT = Convert.ToDateTime(Convert.ToDateTime(data.RedyDate).ToString("yyyy-MM-dd") + " " + data.RedyTime);
			var sectorNo = db.ORG_PickUpArea.Where(x => x.PickUpAreaNo == data.PickUpAreaNo && x.IsServer == true && x.IsDelete == false).Select(x => x.SectorNo).FirstOrDefault();
			var endTime = db.ORG_PickUpArea.Where(x => x.PickUpAreaNo == data.PickUpAreaNo && x.IsDelete == false).Select(x => x.DateEnd).FirstOrDefault();
			if (sectorNo != null)
			{
				sectorNoData = sectorNo;
				sectorNameData = db.ORG_Sector.Where(x => x.SectorNo == sectorNo).Select(x => x.SectorName).FirstOrDefault();
			}
			else
			{
				sectorNoData = "";
				sectorNameData = "";
			}

			var reSector = new ReSector();
			if (sectorNoData != "" && db.ORG_Sector.Where(x => x.SectorNo == sectorNoData).Select(x => x.IsServer).FirstOrDefault() == true)
			{
				var sector = from a in db.ORG_SectorAbsent.Where(x => x.IsDelete == false && x.StartDT <= RedyDT && x.EndDT >= RedyDT)
							 where a.IsDelete == false && a.SectorNo == sectorNoData
							 select new
							 {
								 a.SectorNo,
								 a.AgentSectorNo,
							 };
				var plateNO = "";
				if (sector.Count() > 0)
				{
					reSector.SectorNo = sector.Select(x => x.AgentSectorNo).FirstOrDefault();
					if (reSector.SectorNo != null)
					{
						reSector.SectorName = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).SectorName;
						reSector.EndTime = endTime;
						plateNO = db.ORG_Sector.Where(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).Select(x => x.PlateNO).FirstOrDefault(); ;
						reSector.PlateNO = plateNO;
						reSector.CarID = db.ORG_Vehicle.Where(x => x.CarNO == plateNO && x.IsDelete == false).Select(x => x.CarID).FirstOrDefault();
						reSector.SectorStat = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).StatNo;
						reSector.SectorPhone = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).Phone;

					}
					else
					{
						reSector.SectorNo = sectorNoData;
						reSector.SectorName = sectorNameData;
						reSector.EndTime = endTime;
						plateNO = db.ORG_Sector.Where(x => x.SectorNo == data.SectorNo && x.IsDelete == false).Select(x => x.PlateNO).FirstOrDefault();
						reSector.PlateNO = plateNO;
						reSector.CarID = db.ORG_Vehicle.Where(x => x.CarNO == plateNO && x.IsDelete == false).Select(x => x.CarID).FirstOrDefault();
						reSector.SectorStat = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).StatNo;
						reSector.SectorPhone = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).Phone;
					}
				}
				else
				{
					reSector.SectorNo = sectorNoData;
					reSector.SectorName = sectorNameData;
					reSector.EndTime = endTime;
					plateNO = db.ORG_Sector.Where(x => x.SectorNo == sectorNoData && x.IsDelete == false).Select(x => x.PlateNO).FirstOrDefault();
					reSector.PlateNO = plateNO;
					reSector.CarID = db.ORG_Vehicle.Where(x => x.CarNO == plateNO && x.IsDelete == false).Select(x => x.CarID).FirstOrDefault();
					reSector.SectorStat = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).StatNo;
					reSector.SectorPhone = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == reSector.SectorNo && x.IsDelete == false).Phone;
				}
			}
			else
			{
				reSector.SectorNo = null;
				reSector.SectorName = null;
				reSector.EndTime = endTime;
				reSector.PlateNO = null;
				reSector.CarID = null;
				reSector.SectorStat = null;
				reSector.SectorPhone = null;
			}


			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = reSector,
				Pages = 1,
				Records = 1,
				TotalPage = 1

			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		public partial class ReSector
		{
			public string SectorNo { get; set; }
			public string SectorName { get; set; }
			public string SectorStat { get; set; }
			public string EndTime { get; set; }
			public string CarID { get; set; }
			public string PlateNO { get; set; }
			public string SectorPhone { get; set; }
		}

		[Authorize]
		public ActionResult checkTodaySameCust(string custNo, string statNo)
		{

			int findCount = 0;
			var findTarget = from a in db.ShdetHeader
							 from b in db.ShdetDetail
							 where a.ShdetNo == b.ShdetNo && a.IsDelete == false && DbFunctions.TruncateTime(a.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now) && a.CustNo == custNo && b.StatNo == statNo
							 select new { header = a, detail = b };

			if (findTarget != null)
			{
				if (findTarget.Count() > 0)
					findCount = findTarget.Count();
			}

			JObject result;
			try
			{
				result = new JObject { { "message", findCount.ToString() } };
			}
			catch (Exception e)
			{
				result = new JObject { { "errorMsg", e.Message } };
			}

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

	}
}
