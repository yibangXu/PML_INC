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
	public class ShdetLController : Controller
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

		// GET: Currency
		[Authorize]
		public ActionResult Index()
		{
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "正式調派";
			//主表
			ViewBag.ControllerName = "ShdetL";
			ViewBag.AddFunc = "NewShdet";
			ViewBag.EditFunc = "EditShdet";
			ViewBag.DelFunc = "DeleteShdet";
			ViewBag.FormPartialName = "_ElementInForm";
			ViewBag.FormCustomJsEdit = @"if (row.IsRedy == '是'){$('#IsRedy').switchbutton('check');}else{$('#IsRedy').switchbutton('uncheck');}";
			//子表1
			ViewBag.ControllerName2 = "ShdetL";
			ViewBag.AddFunc2 = "NewShdet2";
			ViewBag.EditFunc2 = "EditShdet2";
			ViewBag.DelFunc2 = "DeleteShdet2";
			ViewBag.FormPartialName2 = "_ElementInForm2";
			ViewBag.FormCustomJsNew2 = @"$('#ProdShdetNo').val(row.ShdetNo);
$('#ProdCustNo').val(row.CustNo);$('#sDtlNo').val(row.sNo);";

			//子表2
			ViewBag.ControllerName3 = "ShdetL";
			ViewBag.AddFunc3 = "";
			ViewBag.EditFunc3 = "";
			ViewBag.DelFunc3 = "";
			ViewBag.FormPartialName3 = "";
			ViewBag.FormCustomJsEdit2 = $"$('#dtlIsFinish').prop('checked', row.IsFinish === 'true' ? true: false);";

			//得到所屬站點, 原本由外務員，20180706更改為帶出使用者的站點
			try
			{
				string cId = WebSiteHelper.CurrentUserID;
				int dbId = int.Parse(WebSiteHelper.CurrentUserDBID);
				var userInfo = from s in db.SYS_User
							   where s.IsDelete == false && s.ID == dbId
							   select s;
				string statNo = userInfo.First().StatNo;
				var statInfo = from s2 in db.ORG_Stat
							   where s2.IsDelete == false && s2.IsServer == true && s2.StatNo == statNo
							   select s2;
				if (statInfo.Count() == 1)
				{
					ViewBag.StatNo = statInfo.First().StatNo;
				}

				var sector = from s in db.ORG_Sector
							 where s.IsDelete == false
							 select s;

				var sectorDic = new Dictionary<string, string>();
				foreach (var i in sector)
					sectorDic.Add(i.SectorNo, i.SectorName);

				ViewBag.SectorNameDic = sectorDic;

				var cust = from c in db.ORG_Cust
						   where c.IsDelete == false
						   select c;
				var custDic = new Dictionary<string, string>();
				foreach (var i in cust)
					custDic.Add(i.CustNo, i.CustCName);
				ViewBag.CustCNameDic = custDic;
			}
			catch (Exception eGetLoginStat)
			{ }

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
		public ActionResult Reject(string ShdetNo)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();

			var target = db.ShdetHeader.FirstOrDefault(x => x.ShdetNo == ShdetNo);
			if (target != null)
			{
				target.IsDesp = false;
				target.RejectBy = User.Identity.Name;
				target.RejectDate = DateTime.Now;

				target.UpdatedBy = User.Identity.Name;
				target.UpdatedDate = DateTime.Now;

				try
				{
					db.Entry(target).State = EntityState.Modified;
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
		public ActionResult EditShdet2(ShdetProd data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

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
				//	userRecord.IsDesp = data.IsDesp;
				//	if (data.IsDesp == true)
				//	{
				//		userRecord.ShdetBy = User.Identity.Name;
				//		userRecord.ShdetDate = nowTime;
				//	}
				//}

				//if (userRecord.IsCancel != data.IsCancel)
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
				//	userRecord.IsReply = data.IsReply;
				//	if (data.IsReply == true)
				//	{
				//		userRecord.ReplyBy = User.Identity.Name;
				//		userRecord.ReplyDate = nowTime;
				//	}
				//}

				//if (userRecord.IsFinish != data.IsFinish)
				//{
				//	userRecord.IsFinish = data.IsFinish;
				//	if (data.IsFinish == true)
				//	{
				//		userRecord.FinishBy = User.Identity.Name;
				//		userRecord.FinishDate = nowTime;
				//	}
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
					{
						result.Ok = DataModifyResultType.Success;
						result.Message = "OK";
					}
					else
					{
						result.Ok = DataModifyResultType.Warning;
						result.Message = retVal.ToString();
					}

					if (CheckiTotNum(data.ShdetNo, data.CustNo, data.sNo) == true)
					{
						result.Ok = DataModifyResultType.Warning;
						result.Message = "材積超過安全值";
					}
					if (CheckWeig(data.ShdetNo, data.CustNo, data.sNo) == true)
					{
						result.Ok = DataModifyResultType.Warning;
						result.Message = "重量超過安全值";
					}
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
		public ActionResult DeleteShdet2(ShdetProd data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ShdetProd.FirstOrDefault(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sDtlNo == data.sDtlNo && x.sNo == data.sNo);
			if (userRecord.SheetNo == null)
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

						int retVal = 0;
						//retVal = (CheckAllProdFinished(userRecord.ShdetNo, userRecord.CustNo, userRecord.sDtlNo) == true) ? retVal + 2 : retVal;
						//retVal = (CheckAllDetailFinished(userRecord.ShdetNo) == true) ? retVal + 1 : retVal;
						if (retVal == 0)
						{
							result.Ok = DataModifyResultType.Success;
							result.Message = "OK";
						}
						else
						{
							result.Ok = DataModifyResultType.Warning;
							result.Message = retVal.ToString();
						}

						if (CheckiTotNum(data.ShdetNo, data.CustNo, data.sNo) == true)
						{
							result.Ok = DataModifyResultType.Warning;
							result.Message = "材積超過安全值";
						}
						if (CheckWeig(data.ShdetNo, data.CustNo, data.sNo) == true)
						{
							result.Ok = DataModifyResultType.Warning;
							result.Message = "重量超過安全值";
						}
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
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "貨物已轉理貨，無法刪除";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		private bool CheckiTotNum(string ShdetNo, string CustNo, int sNo)
		{
			var dtl = db.ShdetDetail.FirstOrDefault(x => x.ShdetNo == ShdetNo && x.CustNo == CustNo && x.sNo == sNo && x.IsDelete == false);
			var prod = db.ShdetProd.Where(x => x.ShdetNo == ShdetNo && x.CustNo == CustNo && x.sNo == sNo && x.IsDelete == false);
			if (dtl != null)
			{
				if (dtl.CarID != null)
				{
					var vehicle = db.ORG_Vehicle.FirstOrDefault(x => x.CarID == dtl.CarID);
					if (vehicle != null)
					{
						if (vehicle.ValueSafe < prod.Sum(x => x.iTotNum))
							return true;
						return false;
					}
					return false;
				}
				return false;
			}
			else
				return false;
		}

		private bool CheckWeig(string ShdetNo, string CustNo, int sNo)
		{
			var dtl = db.ShdetDetail.FirstOrDefault(x => x.ShdetNo == ShdetNo && x.CustNo == CustNo && x.sNo == sNo && x.IsDelete == false);
			var prod = db.ShdetProd.Where(x => x.ShdetNo == ShdetNo && x.CustNo == CustNo && x.sNo == sNo && x.IsDelete == false);
			if (dtl != null)
			{
				if (dtl.CarID != null)
				{
					var vehicle = db.ORG_Vehicle.FirstOrDefault(x => x.CarID == dtl.CarID);
					if (vehicle != null)
					{
						if (vehicle.LoadSafety < prod.Sum(x => x.Weig))
							return true;
						return false;
					}
					return false;
				}
				return false;
			}
			else
				return false;
		}

		[Authorize]
		public ActionResult EditShdet(ShdetMD data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				ShdetDetail userRecord = db.ShdetDetail.Where(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && data.sNo == data.sNo).FirstOrDefault();
				if (data.CarID != userRecord.CarID)
				{
					var pdData = db.ShdetProd.Where(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sDtlNo == data.sNo).ToList();
					foreach (var p in pdData)
					{
						p.CarID = data.CarID;
						db.Entry(p).State = EntityState.Modified;
					}
				}

				var dtl = db.ShdetDetail.FirstOrDefault(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sNo == data.sNo);
				if (dtl != null)
				{
					dtl.CarryName = data.CarryName;
					dtl.Code5 = data.Code5;
					dtl.Code7 = data.Code7;
					dtl.Add_6 = data.Add_6;
					dtl.CustAddr = data.CustAddr;
					dtl.CustENAddr1 = data.CustENAddr1;
					dtl.Country = data.Country;
					dtl.CtcSale = data.CtcSale;
					dtl.Tel = data.Tel;
					dtl.Clerk = data.Clerk;
					dtl.PickUpAreaNo = data.PickUpAreaNo;
					dtl.EndDate = data.EndDate;
					dtl.SectorNo = data.SectorNo;
					dtl.StatNo = data.StatNo;
					dtl.CallStatNo = data.CallStatNo;
					dtl.Add_1 = data.Add_1;
					dtl.Add_2 = data.Add_2;
					dtl.Add_3 = data.Add_3;
					dtl.Add_4 = data.Add_4;
					dtl.Add_5 = data.Add_5;
					dtl.CcNo = data.CcNo;
					dtl.RedyTime = data.RedyTime;
					dtl.CarID = data.CarID;
					dtl.WeigLevel = data.WeigLevel;
					dtl.CocustomTyp = data.CocustomTyp;
					dtl.Charge = data.Charge;
					if (data.RedyDate != null)
						dtl.RedyDate = data.RedyDate;
					else
						dtl.RedyDate = null;
					dtl.IsRedy = data.IsRedy;
					if (data.IsCancel == true)
					{
						var prod = db.ShdetProd.Any(x => x.ShdetNo == data.ShdetNo && x.SheetNo != null && x.IsDelete != false);
						if (prod)
						{
							result.Ok = DataModifyResultType.Warning;
							result.Message = "貨物已轉理貨，無法取消";
							return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
						}
						dtl.CancelDate = DateTime.Now;
						dtl.CancelBy = User.Identity.Name;
					}
					else if (data.IsCancel == false)
					{
						dtl.CancelDate = null;
						dtl.CancelBy = null;
					}
					dtl.IsCancel = data.IsCancel;
					if (data.IsFinish == true && dtl.IsFinish != true)
					{
						dtl.FinishDate = DateTime.Now;
						dtl.FinishBy = User.Identity.Name;
					}
					else if (data.IsFinish == false)
					{
						dtl.FinishDate = null;
						dtl.FinishBy = null;
					}
					dtl.IsFinish = data.IsFinish;
					if (data.ReplyComment != null && dtl.ReplyComment != data.ReplyComment)
					{
						dtl.ReplyComment = data.ReplyComment;
						dtl.IsReply = true;
						dtl.ReplyBy = User.Identity.Name;
						dtl.ReplyDate = DateTime.Now;
					}
					else if (data.ReplyComment == null)
					{
						dtl.ReplyComment = data.ReplyComment;
						dtl.IsReply = false;
						dtl.ReplyBy = null;
						dtl.ReplyDate = null;
					}
					dtl.UpdatedDate = DateTime.Now;
					dtl.UpdatedBy = User.Identity.Name;

					db.Entry(dtl).State = EntityState.Modified;
				}
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}

				var mas = db.ShdetHeader.FirstOrDefault(x => x.ShdetNo == data.ShdetNo);
				if (mas != null)
				{
					mas.HubNo = data.HubNo;
					mas.Clerk = data.Clerk;
					mas.Dest = data.Dest;
					mas.ReserveDate = data.ReserveDate;
					if (mas.SDate != data.SDate)
					{
						var blData = db.Bill_Lading.Where(x => x.LadingNo == data.ShdetNo).FirstOrDefault();
						if (blData != null)
						{
							blData.SDate = data.SDate;
							db.Entry(blData).State = EntityState.Modified;
							db.SaveChanges();
						}
						mas.SDate = data.SDate;
					}
					mas.UpdatedDate = DateTime.Now;
					mas.UpdatedBy = User.Identity.Name;
					db.Entry(mas).State = EntityState.Modified;
				}
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}

				try
				{
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";

					if (dtl.SectorNo == null)
					{
						result.Ok = DataModifyResultType.Warning;
						result.Message = "尚未指派外務員!";
					}
					int retVal = 0;
					//shdetFix
					//retVal = (CheckAllDetailFinished(dtl.ShdetNo) == true) ? retVal + 1 : retVal;
					if (retVal == 0)
					{
						result.Ok = DataModifyResultType.Success;
						result.Message = "OK";
					}
					else
					{
						result.Ok = DataModifyResultType.Warning;
						result.Message = retVal.ToString();
					}
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
		public ActionResult DeleteShdet(ShdetMD data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();

			var prod = db.ShdetProd.Any(x => x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sDtlNo == data.sNo && x.IsDelete == false);
			if (!prod)
			{
				var dtl = db.ShdetDetail.FirstOrDefault(x => x.IsDelete == false && x.ShdetNo == data.ShdetNo && x.CustNo == data.CustNo && x.sNo == data.sNo);
				if (dtl != null)
				{
					dtl.DeletedDate = DateTime.Now;
					dtl.DeletedBy = User.Identity.Name;
					dtl.IsDelete = true;

					try
					{
						db.Entry(dtl).State = EntityState.Modified;
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
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "尚有貨物資料，不允許刪除!";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		//用於提單輸入調派單號 關聯出資料
		[Authorize]
		public ActionResult GetGridJSON_ToBillLading(ShdetMD data, int page = 1, int rows = 40)
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

			var shdetData = from d in db.ShdetDetail
							join h in db.ShdetHeader
							on new { d.ShdetNo, d.CustNo } equals new { h.ShdetNo, h.CustNo }
							join hub in db.ORG_Hub
							on h.HubNo equals hub.HubNo into ps1
							from hub in ps1.DefaultIfEmpty()
							join cs in db.ORG_Stat
							on d.CallStatNo equals cs.StatNo into ps2
							from cs in ps2.DefaultIfEmpty()
							join s in db.ORG_Stat
							on d.StatNo equals s.StatNo into ps3
							from s in ps3.DefaultIfEmpty()
							join sec in db.ORG_Sector
							on d.SectorNo equals sec.SectorNo into ps4
							from sec in ps4.DefaultIfEmpty()
							join v in db.ORG_Vehicle
							on d.CarID equals v.CarID into ps5
							from v in ps5.DefaultIfEmpty()
							join pa in db.ORG_PickUpArea
							on d.PickUpAreaNo equals pa.PickUpAreaNo into ps6
							from pa in ps6.DefaultIfEmpty()
							join cuser in db.SYS_User
							on h.CreatedBy equals cuser.Account into ps7
							from cuser in ps7.DefaultIfEmpty()
							join canuser in db.SYS_User
							on h.CancelBy equals canuser.Account into ps8
							from canuser in ps8.DefaultIfEmpty()
							join dest in db.ORG_Dest
							on h.Dest equals dest.CName into ps9
							from dest in ps9.DefaultIfEmpty()
							where d.IsDelete == false && !h.CustNo.Contains("T") && h.LadingNo == null && h.IsDelete == false && h.IsDesp == true && h.ShdetNo == data.ShdetNo
							select new { h, d, s, hub, cs, sec, v, pa, canuser, cuser, dest };

			var today = DateTime.Now;

			var shdet = shdetData.ToList().Select((i, index) => new ShdetMD()
			{
				RowNumber = index,
				LadingNo = "L" + i.h.ShdetNo,
				DateTimeNow = DateTime.Now,
				ShdetNo = i.h.ShdetNo,
				CustNo = i.h.CustNo,
				CustCHName = i.h.CustCHName,
				HubNo = i.h.HubNo,
				HubName = i.hub == null ? null : i.hub.HubName,
				Dest = i.h.Dest,
				DestNo = i.dest == null ? null : i.dest.DestNo,
				CarryName = i.d.CarryName,
				RedyDate = i.d.RedyDate,
				RedyTime = i.d.RedyTime,
				CustAddrFull = i.d.CustAddr + (i.d.Add_1 == 0 ? null : (i.d.Add_1 + "段")) + (i.d.Add_2 == 0 ? null : (i.d.Add_2 + "巷")) + (i.d.Add_3 == 0 ? null : (i.d.Add_3 + "弄")) + (i.d.Add_4 == "" || i.d.Add_4 == null ? null : (i.d.Add_4 + "號")) + (i.d.Add_5 == 0 ? null : (i.d.Add_5 + "樓")) + (i.d.Add_6 == null || i.d.Add_6 == "" ? null : i.d.Add_6),
				CtcSale = i.d.CtcSale,
				Tel = i.d.Tel,
				PickUpAreaNo = i.d.PickUpAreaNo,
				PickUpAreaName = i.pa == null ? null : i.pa.PickUpAreaName,
				SectorNo = i.sec == null ? null : i.sec.SectorNo,
				SectorName = i.sec == null ? null : i.sec.SectorName,
				SectorPhone = i.sec == null ? null : i.sec.Phone,
				WeigLevel = i.d.WeigLevel,
				WeigLevelType = i.d.WeigLevel == -1 ? "" : i.d.WeigLevel == 0 ? "0.文件" : i.d.WeigLevel == 1 ? "1.包裹5KG以下" : "2.箱貨5KG以上",
				CocustomTyp = i.d.CocustomTyp,
				CallStatNo = i.d.CallStatNo,
				CallStatName = i.cs == null ? null : i.cs.StatName,
				CreatedBy = i.cuser == null ? null : i.cuser.UserName,
				CreatedDate = i.h.CreatedDate,
				ShdetDate = i.h.ShdetDate,
				IsFinish = i.h.IsFinish,
				FinishDate = i.h.FinishDate,
				IsCancel = i.h.IsCancel,
				CancelDate = i.h.CancelDate,
				CancelBy = i.canuser == null ? null : i.canuser.UserName,
				CarID = i.d.CarID,
				IsDesp = i.h.IsDesp,
				IsReply = i.h.IsReply,
				ShdetBy = i.h.ShdetBy,
				ReplyBy = i.h.ReplyBy,
				ReplyDate = i.h.ReplyDate,
				FinishBy = i.h.FinishBy,
				UpdatedBy = i.h.UpdatedBy,
				UpdatedDate = i.h.UpdatedDate,
				DeletedBy = i.h.DeletedBy,
				DeletedDate = i.h.DeletedDate,
				IsDelete = i.h.IsDelete,
				Clerk = i.h.Clerk,
				ReserveDate = i.h.ReserveDate,
				StatNo = i.d.StatNo,
				StatName = i.s == null ? null : i.s.StatName,
				sNo = i.d == null ? 0 : i.d.sNo,
				Add_1 = i.d.Add_1,
				Add_2 = i.d.Add_2,
				Add_3 = i.d.Add_3,
				Add_4 = i.d.Add_4,
				Add_5 = i.d.Add_5,
				Add_6 = i.d.Add_6,
				CustAddr = i.d.CustAddr,
				CustENAddr1 = i.d.CustENAddr1,
				Country = i.d.Country,
				CcNo = i.d.CcNo,
				Charge = i.d.Charge,
				Code5 = i.d.Code5,
				EndDate = i.d.EndDate,
				RejectBy = i.h.RejectBy,
				RejectDate = i.h.RejectDate,
			}) as IEnumerable<ShdetMD>;

			shdet = shdet.Where(x => statNoList.Contains(x.CallStatNo) || statNoList.Contains(x.StatNo));

			int records = shdet.Count();

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

		//用於提單開窗選取調派資料
		[Authorize]
		public ActionResult GetGridJSON_ToBillLading1(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
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
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;

			var shdet =
				from d in db.ShdetDetail.Where(x => x.RedyDate <= eDate && sDate <= x.RedyDate)
				join h in db.ShdetHeader
							on new { d.ShdetNo, d.CustNo } equals new { h.ShdetNo, h.CustNo }

				join hub in db.ORG_Hub
				on h.HubNo equals hub.HubNo into ps1
				from hub in ps1.DefaultIfEmpty()

				join cs in db.ORG_Stat
				on d.CallStatNo equals cs.StatNo into ps2
				from cs in ps2.DefaultIfEmpty()

				join s in db.ORG_Stat
				on d.StatNo equals s.StatNo into ps3
				from s in ps3.DefaultIfEmpty()

				join sec in db.ORG_Sector
				on d.SectorNo equals sec.SectorNo into ps4
				from sec in ps4.DefaultIfEmpty()

				join v in db.ORG_Vehicle
				on d.CarID equals v.CarID into ps5
				from v in ps5.DefaultIfEmpty()

				join pa in db.ORG_PickUpArea
				on d.PickUpAreaNo equals pa.PickUpAreaNo into ps6
				from pa in ps6.DefaultIfEmpty()

				join cuser in db.SYS_User
				on h.CreatedBy equals cuser.Account into ps7
				from cuser in ps7.DefaultIfEmpty()

				join canuser in db.SYS_User
				on h.CancelBy equals canuser.Account into ps8
				from canuser in ps8.DefaultIfEmpty()

				join dest in db.ORG_Dest
				on h.Dest equals dest.CName into ps9
				from dest in ps9.DefaultIfEmpty()

				where d.IsDelete == false && !h.CustNo.Contains("T") && h.LadingNo == null && h.IsDelete == false && h.IsDesp == true

				select new { h, d, s, hub, cs, sec, v, pa, canuser, cuser, dest };

			shdet = shdet.Where(x => statNoList.Contains(x.d.CallStatNo) || statNoList.Contains(x.d.StatNo));

			if (data.ShdetNo.IsNotEmpty())
				shdet = shdet.Where(x => x.h.ShdetNo.Contains(data.ShdetNo));
			if (data.CallStatName.IsNotEmpty())
			{
				shdet = shdet.Where(x => x.cs.StatName != null);
				shdet = shdet.Where(x => x.cs.StatName.Contains(data.CallStatName));
			}
			if (data.CustNo.IsNotEmpty())
				shdet = shdet.Where(x => x.d.CustNo.Contains(data.CustNo));
			if (data.CustCHName.IsNotEmpty())
				shdet = shdet.Where(x => x.h.CustCHName.Contains(data.CustCHName));
			if (data.CarryName.IsNotEmpty())
				shdet = shdet.Where(x => x.d.CarryName.Contains(data.CarryName));
			if (data.CtcSale.IsNotEmpty())
				shdet = shdet.Where(x => x.d.CtcSale.Contains(data.CtcSale));
			if (data.Dest.IsNotEmpty())
			{
				shdet = shdet.Where(x => x.h.Dest != null);
				shdet = shdet.Where(x => x.h.Dest.Contains(data.Dest));
			}
			if (data.HubName.IsNotEmpty())
			{
				shdet = shdet.Where(x => x.hub.HubName != null);
				shdet = shdet.Where(x => x.hub.HubName.Contains(data.HubName));
			}
			if (data.WeigLevel >= 0)
				shdet = shdet.Where(x => x.d.WeigLevel == data.WeigLevel);
			if (data.SectorName.IsNotEmpty())
			{
				shdet = shdet.Where(x => x.sec.SectorName != null);
				shdet = shdet.Where(x => x.sec.SectorName.Contains(data.SectorName));
			}
			if ((data.IsFinish == false && Request["IsFinish"] == "false") || data.IsFinish == true)
				shdet = shdet.Where(x => x.h.IsFinish == data.IsFinish);
			if ((data.IsCancel == false && Request["IsCancel"] == "false") || data.IsCancel == true)
				shdet = shdet.Where(x => x.h.IsCancel == data.IsCancel);
			if (data.CreatedBy.IsNotEmpty())
				shdet = shdet.Where(x => x.cuser.CreatedBy.Contains(data.CreatedBy));

			var shdetData = shdet.ToList().Select((i, index) => new ShdetMD()
			{
				RowNumber = index,
				LadingNo = "L" + i.h.ShdetNo,
				DateTimeNow = DateTime.Now,
				ShdetNo = i.h.ShdetNo,
				CustNo = i.h.CustNo,
				CustCHName = i.h.CustCHName,
				HubNo = i.h.HubNo,
				HubName = i.hub == null ? null : i.hub.HubName,
				Dest = i.h.Dest,
				DestNo = i.dest == null ? null : i.dest.DestNo,
				CarryName = i.d.CarryName,
				RedyDate = i.d.RedyDate,
				RedyTime = i.d.RedyTime,
				CustAddrFull = i.d.CustAddr + (i.d.Add_1 == 0 ? null : (i.d.Add_1 + "段")) + (i.d.Add_2 == 0 ? null : (i.d.Add_2 + "巷")) + (i.d.Add_3 == 0 ? null : (i.d.Add_3 + "弄")) + (i.d.Add_4 == "" || i.d.Add_4 == null ? null : (i.d.Add_4 + "號")) + (i.d.Add_5 == 0 ? null : (i.d.Add_5 + "樓")) + (i.d.Add_6 == null || i.d.Add_6 == "" ? null : i.d.Add_6),
				CtcSale = i.d.CtcSale,
				Tel = i.d.Tel,
				PickUpAreaNo = i.d.PickUpAreaNo,
				PickUpAreaName = i.pa == null ? null : i.pa.PickUpAreaName,
				SectorNo = i.sec == null ? null : i.sec.SectorNo,
				SectorName = i.sec == null ? null : i.sec.SectorName,
				SectorPhone = i.sec == null ? null : i.sec.Phone,
				WeigLevel = i.d.WeigLevel,
				WeigLevelType = i.d.WeigLevel == -1 ? "" : i.d.WeigLevel == 0 ? "0.文件" : i.d.WeigLevel == 1 ? "1.包裹5KG以下" : "2.箱貨5KG以上",
				CocustomTyp = i.d.CocustomTyp,
				CallStatNo = i.d.CallStatNo,
				CallStatName = i.cs == null ? null : i.cs.StatName,
				CreatedBy = i.cuser == null ? null : i.cuser.UserName,
				CreatedDate = i.h.CreatedDate,
				ShdetDate = i.h.ShdetDate,
				IsFinish = i.h.IsFinish,
				FinishDate = i.h.FinishDate,
				IsCancel = i.h.IsCancel,
				CancelDate = i.h.CancelDate,
				CancelBy = i.canuser == null ? null : i.canuser.UserName,
				CarID = i.d.CarID,
				IsDesp = i.h.IsDesp,
				IsReply = i.h.IsReply,
				ShdetBy = i.h.ShdetBy,
				ReplyBy = i.h.ReplyBy,
				ReplyDate = i.h.ReplyDate,
				FinishBy = i.h.FinishBy,
				UpdatedBy = i.h.UpdatedBy,
				UpdatedDate = i.h.UpdatedDate,
				DeletedBy = i.h.DeletedBy,
				DeletedDate = i.h.DeletedDate,
				IsDelete = i.h.IsDelete,
				Clerk = i.h.Clerk,
				ReserveDate = i.h.ReserveDate,
				StatNo = i.d.StatNo,
				StatName = i.s == null ? null : i.s.StatName,
				sNo = i.d == null ? 0 : i.d.sNo,
				Add_1 = i.d.Add_1,
				Add_2 = i.d.Add_2,
				Add_3 = i.d.Add_3,
				Add_4 = i.d.Add_4,
				Add_5 = i.d.Add_5,
				Add_6 = i.d.Add_6,
				CustAddr = i.d.CustAddr,
				CustENAddr1 = i.d.CustENAddr1,
				Country = i.d.Country,
				CcNo = i.d.CcNo,
				Charge = i.d.Charge,
				Code5 = i.d.Code5,
				EndDate = i.d.EndDate,
				RejectBy = i.h.RejectBy,
				RejectDate = i.h.RejectDate,
			}) as IEnumerable<ShdetMD>;

			//排序條件:
			//1.未派外務員
			//2.超過材積
			//3.超重
			//4.超過取件時間未完成
			//5.正常
			//6.取消

			if (data.CustAddrFull.IsNotEmpty())
				shdetData = shdetData.Where(x => x.CustAddrFull.Contains(data.CustAddrFull));
			int records = shdetData.Count();
			shdetData = shdetData.OrderByDescending(x => x.ShdetDate).ThenByDescending(x => x.SectorName == null)
							.ThenBy(x => x.ShdetNo)
							.Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = shdetData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON1(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
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

			var today = DateTime.Now;
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;

			var shdet = from d in db.ShdetDetail.Where(x => x.RedyDate <= eDate && sDate <= x.RedyDate && x.IsDelete == false)
						join h in db.ShdetHeader.Where(x => x.IsDelete == false && x.IsDesp == true)
						on new { d.ShdetNo, d.CustNo } equals new { h.ShdetNo, h.CustNo } into ps
						from h in ps.DefaultIfEmpty()

						join hub in db.ORG_Hub.Where(x => x.IsDelete == false)
						on h.HubNo equals hub.HubNo into ps1
						from hub in ps1.DefaultIfEmpty()

						join cs in db.ORG_Stat.Where(x => x.IsDelete == false)
						on d.CallStatNo equals cs.StatNo into ps2
						from cs in ps2.DefaultIfEmpty()

						join s in db.ORG_Stat.Where(x => x.IsDelete == false)
						on d.StatNo equals s.StatNo into ps3
						from s in ps3.DefaultIfEmpty()

						join sec in db.ORG_Sector.Where(x => x.IsDelete == false && (x.IsServer == true || x.IsLeave == true && x.IsServer == false))
						on d.SectorNo equals sec.SectorNo into ps4
						from sec in ps4.DefaultIfEmpty()

						join v in db.ORG_Vehicle.Where(x => x.IsDelete == false)
						on d.CarID equals v.CarID into ps5
						from v in ps5.DefaultIfEmpty()

						join pa in db.ORG_PickUpArea.Where(x => x.IsDelete == false)
						on d.PickUpAreaNo equals pa.PickUpAreaNo into ps6
						from pa in ps6.DefaultIfEmpty()

						join cuser in db.SYS_User.Where(x => x.IsDelete == false)
						on h.CreatedBy equals cuser.Account into ps7
						from cuser in ps7.DefaultIfEmpty()

						join updateuser in db.SYS_User.Where(x => x.IsDelete == false)
						on d.UpdatedBy equals updateuser.Account into ps8
						from updateuser in ps8.DefaultIfEmpty()

						join canuser in db.SYS_User.Where(x => x.IsDelete == false)
						on d.CancelBy equals canuser.Account into ps9
						from canuser in ps9.DefaultIfEmpty()

						join b in db.Bill_Lading.Where(x => x.IsDelete == false && x.ShdetNo != null)
						on h.LadingNo equals b.LadingNo into ps10
						from b in ps10.DefaultIfEmpty()

							//join gpProd in (from s in db.ShdetProd.Where(x => x.IsDelete == false)
							//				group s by new { s.CarID, s.RedyDate } into g
							//				select new ProdRow()
							//				{
							//					CarID = g.Key.CarID,
							//					RedyDate = g.Key.RedyDate,
							//					WeigTotal = g.Sum(x => x.Weig),
							//					iTotNumTotal = g.Sum(x => x.iTotNum),
							//					Count = g.Sum(x => x.Pcs)
							//				})
							//on new { d.CarID, d.RedyDate } equals new { gpProd.CarID, gpProd.RedyDate } into ps11
							//from gpProd in ps11.DefaultIfEmpty()
						select new
						{
							LadingNo_Type = b == null ? h.ShdetNo : b.LadingNo_Type,
							ShdetNo = h.ShdetNo,
							CustNo = h.CustNo,
							CustCHName = h.CustCHName,
							HubNo = h.HubNo,
							HubName = hub == null ? null : hub.HubName,
							Dest = h.Dest,
							CarryName = d.CarryName,
							RedyDate = d.RedyDate,
							RedyTime = d.RedyTime,
							IsRedy = d.IsRedy ?? false,
							CustAddrFull = d.CustAddr + (d.Add_1 == 0 ? null : (d.Add_1 + "段")) + (d.Add_2 == 0 ? null : (d.Add_2 + "巷")) + (d.Add_3 == 0 ? null : (d.Add_3 + "弄")) + (d.Add_4 == "" || d.Add_4 == null ? null : (d.Add_4 + "號")) + (d.Add_5 == 0 ? null : (d.Add_5 + "樓")) + (d.Add_6 == null || d.Add_6 == "" ? null : d.Add_6),
							CtcSale = d.CtcSale,
							Tel = d.Tel,
							PickUpAreaNo = d.PickUpAreaNo,
							PickUpAreaName = pa == null ? null : pa.PickUpAreaName,
							SectorNo = sec == null ? null : sec.SectorNo,
							SectorName = sec == null ? null : sec.SectorName,
							SectorPhone = sec == null ? null : sec.Phone,
							WeigLevel = d.WeigLevel,
							CocustomTyp = d.CocustomTyp,
							CocustomTypStr = d == null ? null : d.CocustomTyp == 0 ? "不報關" : d.CocustomTyp == 1 ? "正式報關" : d.CocustomTyp == 2 ? "簡易報關" : d.CocustomTyp == 3 ? "正式報關+後段核銷" : d.CocustomTyp == 4 ? "簡易報關+後段核銷" : d.CocustomTyp == 5 ? "不報關+後段核銷" : d.CocustomTyp == 6 ? "其他" : " ",
							CallStatNo = d.CallStatNo,
							CallStatName = cs == null ? null : cs.StatName,
							CreatedBy = cuser == null ? null : cuser.UserName,
							CreatedDate = h.CreatedDate,
							ShdetDate = h.ShdetDate,
							//ShdetFix h->d
							IsFinish = d.IsFinish ?? false,
							FinishBy = d.FinishBy,
							FinishDate = d.FinishDate,
							IsCancel = d.IsCancel ?? false,
							CancelDate = d.CancelDate,
							CancelBy = canuser == null ? null : canuser.UserName,
							IsReply = d.IsReply ?? false,
							ReplyBy = d.ReplyBy,
							ReplyDate = d.ReplyDate,
							ReplyComment = d.ReplyComment,
							//
							CarID = d.CarID,
							IsDesp = h.IsDesp,
							ShdetBy = h.ShdetBy,
							UpdatedBy = updateuser == null ? null : updateuser.UserName,
							UpdatedDate = h.UpdatedDate,
							DeletedBy = h.DeletedBy,
							DeletedDate = h.DeletedDate,
							IsDelete = h.IsDelete,
							Clerk = h.Clerk,
							ReserveDate = h.ReserveDate,
							SDate = h.SDate,
							StatNo = d.StatNo,
							StatName = s == null ? null : s.StatName,
							sNo = d == null ? 0 : d.sNo,
							//OverWeig = (gpProd != null && v != null) ? (gpProd.WeigTotal > v.LoadSafety ? true : false) : false,
							//OveriTotNum = (gpProd != null && v != null) ? (gpProd.iTotNumTotal > v.ValueSafe ? true : false) : false,
							Add_1 = d.Add_1,
							Add_2 = d.Add_2,
							Add_3 = d.Add_3,
							Add_4 = d.Add_4,
							Add_5 = d.Add_5,
							Add_6 = d.Add_6,
							CustAddr = d.CustAddr,
							CustENAddr1 = d.CustENAddr1,
							Country = d.Country,
							CcNo = d.CcNo,
							Charge = d.Charge,
							Code5 = d.Code5,
							Code7 = d.Code7,
							EndDate = d.EndDate,
							RejectBy = h.RejectBy,
							RejectDate = h.RejectDate,
							LadingNo = h.LadingNo,
							LoadSafety = v.LoadSafety,
							ValueSafe = v.ValueSafe
						};

			shdet = shdet.Where(x => x.IsDesp == true && (statNoList.Contains(x.CallStatNo) || statNoList.Contains(x.StatNo)));

			if (data.LadingNo_Type.IsNotEmpty())
			{
				shdet = shdet.Where(x => x.LadingNo_Type != null);
				shdet = shdet.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			}
			if (data.ShdetNo.IsNotEmpty())
				shdet = shdet.Where(x => x.ShdetNo.Contains(data.ShdetNo));
			if (data.CallStatName.IsNotEmpty())
				shdet = shdet.Where(x => x.CallStatName.Contains(data.CallStatName));
			if (data.CustNo.IsNotEmpty())
				shdet = shdet.Where(x => x.CustNo.Contains(data.CustNo));
			if (data.CustCHName.IsNotEmpty())
				shdet = shdet.Where(x => x.CustCHName.Contains(data.CustCHName));
			if (data.CarryName.IsNotEmpty())
				shdet = shdet.Where(x => x.CarryName.Contains(data.CarryName));
			if (data.CtcSale.IsNotEmpty())
				shdet = shdet.Where(x => x.CtcSale.Contains(data.CtcSale));
			if (data.Tel.IsNotEmpty())
				shdet = shdet.Where(x => x.Tel.Contains(data.Tel));
			if (data.Dest.IsNotEmpty())
				shdet = shdet.Where(x => x.Dest.Contains(data.Dest));
			if (data.HubName.IsNotEmpty())
				shdet = shdet.Where(x => x.HubName.Contains(data.HubName));
			if (data.WeigLevel >= 0)
				shdet = shdet.Where(x => x.WeigLevel == data.WeigLevel);
			if (data.SectorName.IsNotEmpty())
				shdet = shdet.Where(x => x.SectorName.Contains(data.SectorName));
			if (data.SectorPhone.IsNotEmpty())
				shdet = shdet.Where(x => x.SectorPhone.Contains(data.SectorPhone));
			if ((data.IsFinish == false && Request["IsFinish"] == "false") || data.IsFinish == true)
				shdet = shdet.Where(x => x.IsFinish == data.IsFinish);
			if ((data.IsCancel == false && Request["IsCancel"] == "false") || data.IsCancel == true)
				shdet = shdet.Where(x => x.IsCancel == data.IsCancel);
			if ((data.IsRedy == false && Request["IsRedy"] == "false") || data.IsRedy == true)
				shdet = shdet.Where(x => x.IsRedy == data.IsRedy);
			if (data.CreatedBy.IsNotEmpty())
				shdet = shdet.Where(x => x.CreatedBy.Contains(data.CreatedBy));
			if (data.UpdatedBy.IsNotEmpty())
				shdet = shdet.Where(x => x.UpdatedBy == data.UpdatedBy);
			if (data.PickUpAreaName.IsNotEmpty())
				shdet = shdet.Where(x => x.PickUpAreaName == data.PickUpAreaName);
			if (data.CustAddrFull.IsNotEmpty())
				shdet = shdet.Where(x => x.CustAddrFull.Contains(data.CustAddrFull));
			if (data.CocustomTypStr.IsNotEmpty())
				shdet = shdet.Where(x => x.CocustomTypStr == data.CocustomTypStr);

			//排序條件:
			//1.未派外務員
			//2.超過材積
			//3.超重
			//4.超過取件時間未完成
			//5.正常
			//6.取消
			int records = shdet.Count();
			shdet = shdet.OrderByDescending(x => x.ShdetDate).ThenByDescending(x => x.SectorName == null)
					  .ThenByDescending(x => x.IsFinish == false && x.RedyDate < today)
					  //.ThenBy(x => x.SectorName != null && x.OveriTotNum == false && x.OverWeig == false && ((x.IsFinish == false && Convert.ToDateTime(x.RedyDate.ToDateString() + "T" + x.RedyTime) >= today) || x.IsFinish == true) && x.IsCancel == false)
					  .ThenBy(x => x.ShdetNo);

			shdet = shdet.Skip((page - 1) * rows).Take(rows);
			var shdetData = shdet.AsEnumerable().Select((i, index) => new ShdetMD()
			{
				RowNumber = index,
				LadingNo_Type = i.LadingNo_Type,
				ShdetNo = i.ShdetNo,
				CustNo = i.CustNo,
				CustCHName = i.CustCHName,
				HubNo = i.HubNo,
				HubName = i.HubName,
				Dest = i.Dest,
				CarryName = i.CarryName,
				RedyDate = i.RedyDate,
				RedyTime = i.RedyTime,
				IsRedy = i.IsRedy,
				CustAddrFull = i.CustAddrFull,
				CtcSale = i.CtcSale,
				Tel = i.Tel,
				PickUpAreaNo = i.PickUpAreaNo,
				PickUpAreaName = i.PickUpAreaName,
				SectorNo = i.SectorNo,
				SectorName = i.SectorName,
				SectorPhone = i.SectorPhone,
				WeigLevel = i.WeigLevel,
				CocustomTyp = i.CocustomTyp,
				CocustomTypStr = i.CocustomTypStr,
				CallStatNo = i.CallStatNo,
				CallStatName = i.CallStatName,
				CreatedBy = i.CreatedBy,
				CreatedDate = i.CreatedDate,
				ShdetDate = i.ShdetDate,
				IsFinish = i.IsFinish,
				FinishDate = i.FinishDate,
				IsCancel = i.IsCancel,
				CancelDate = i.CancelDate,
				CancelBy = i.CancelBy,
				CarID = i.CarID,
				IsDesp = i.IsDesp,
				IsReply = i.IsReply,
				ReplyComment = i.ReplyComment,
				ShdetBy = i.ShdetBy,
				ReplyBy = i.ReplyBy,
				ReplyDate = i.ReplyDate,
				FinishBy = i.FinishBy,
				UpdatedBy = i.UpdatedBy,
				UpdatedDate = i.UpdatedDate,
				DeletedBy = i.DeletedBy,
				DeletedDate = i.DeletedDate,
				IsDelete = i.IsDelete,
				Clerk = i.Clerk,
				ReserveDate = i.ReserveDate,
				SDate = i.SDate,
				StatNo = i.StatNo,
				StatName = i.StatName,
				sNo = i.sNo,
				OverWeig = db.ShdetProd.Where(x => x.CarID == i.CarID && x.RedyDate == i.RedyDate && x.IsDelete == false).Sum(x => x.Weig) > i.LoadSafety ? true : false,
				OveriTotNum = db.ShdetProd.Where(x => x.CarID == i.CarID && x.RedyDate == i.RedyDate && x.IsDelete == false).Sum(x => x.iTotNum) > i.ValueSafe ? true : false,
				OverTime = (i.RedyDate != null && i.RedyTime != null) ? ((i.IsFinish == false && Convert.ToDateTime(i.RedyDate.Value.ToString("yyyy/MM/dd") + "T" + i.RedyTime) < today) ? true : false) : false,
				Add_1 = i.Add_1,
				Add_2 = i.Add_2,
				Add_3 = i.Add_3,
				Add_4 = i.Add_4,
				Add_5 = i.Add_5,
				Add_6 = i.Add_6,
				CustAddr = i.CustAddr,
				CustENAddr1 = i.CustENAddr1,
				Country = i.Country,
				CcNo = i.CcNo,
				Charge = i.Charge,
				Code5 = i.Code5,
				Code7 = i.Code7,
				EndDate = i.EndDate,
				RejectBy = i.RejectBy,
				RejectDate = i.RejectDate,
				LadingNo = i.LadingNo,
			});

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = shdetData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON2(int page = 1, int rows = 40, string ShdetNo = "", string CustNo = "", int sDtlNo = 0)
		{
			var data = db.ShdetProd.Where(x => x.IsDelete == false).Select(x => new
			{
				ShdetNo = x.ShdetNo,
				CustNo = x.CustNo,
				sDtlNo = x.sDtlNo,
				sNo = x.sNo,
				Pcs = x.Pcs,
				WeigLevel = x.WeigLevel,
				Weig = x.Weig,
				CocustomTyp = x.CocustomTyp,
				HubNo = x.HubNo,
				CcNo = x.CcNo,
				Charge = x.Charge,
				Dest = x.Dest,
				RedyDate = x.RedyDate,
				RedyTime = x.RedyTime,
				Remark1 = x.Remark1,
				Remark3 = x.Remark3,
				SectorNo = x.SectorNo,
				CallType = x.CallType,
				StatNo = x.StatNo,
				CarID = x.CarID,
				CreatedBy = x.CreatedBy,
				CreatedDate = x.CreatedDate,
				UpdatedBy = x.UpdatedBy,
				UpdatedDate = x.UpdatedDate,
				DeletedBy = x.DeletedBy,
				DeletedDate = x.DeletedDate,
				IsDelete = x.IsDelete,
				fLen = x.fLen,
				fWidth = x.fWidth,
				fHeight = x.fHeight,
				iNum = x.iNum,
				iTotNum = x.iTotNum,
				SheetNo = x.SheetNo,
				SSNo = x.SSNo,
				//ShdetBy = x.ShdetBy,
				//ShdetDate = x.ShdetDate,
				//IsDesp = x.IsDesp ?? false,
				//IsCancel = x.IsCancel ?? false,
				//IsReply = x.IsReply ?? false,
				//IsFinish = x.IsFinish ?? false,
				//CancelBy = x.CancelBy,
				//CancelDate = x.CancelDate,
				//ReplyBy = x.ReplyBy,
				//ReplyDate = x.ReplyDate,
				//ReplyComment = x.ReplyComment,
				//FinishBy = x.FinishBy,
				//FinishDate = x.FinishDate,
				//PhoneCheckTime = x.PhoneCheckTime,
				//Status = x.Status,
				//StatusTime = x.StatusTime,


			});

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

		public class ProdRow
		{
			public string CarID { set; get; }
			public DateTime? RedyDate { set; get; }
			public double? WeigTotal { set; get; }
			public double? iTotNumTotal { set; get; }
			public int? Count { set; get; }
			public int? SheetNoTotal { set; get; }
		}

		[Authorize]
		public ActionResult GetGridJSON3(ShdetVehicle data, int page = 1, int rows = 40)
		{
			var vehicle = from d in db.ShdetDetail.Where(x => x.IsDelete == false && x.ShdetNo == data.ShdetNo && x.sNo == data.sDtlNo)
						  join v in db.ORG_Vehicle.Where(x => x.IsDelete == false)
						  on d.CarID equals v.CarID into ps
						  from v in ps.DefaultIfEmpty()
						  join s in db.ORG_Sector.Where(x => x.IsDelete == false)
						  on d.SectorNo equals s.SectorNo into ps1
						  from s in ps1.DefaultIfEmpty()
						  join gpProd in (from s in db.ShdetProd.Where(x => x.IsDelete == false)
										  group s by new { s.CarID, s.RedyDate } into g
										  select new ProdRow()
										  {
											  CarID = g.Key.CarID,
											  RedyDate = g.Key.RedyDate,
											  WeigTotal = g.Sum(x => x.Weig),
											  iTotNumTotal = g.Sum(x => x.iTotNum),
											  Count = g.Sum(x => x.Pcs),
											  SheetNoTotal = g.Count(),
										  })
						on new { d.CarID, d.RedyDate } equals new { gpProd.CarID, gpProd.RedyDate } into ps2
						  from gpProd in ps2.DefaultIfEmpty()
						  select new ShdetVehicle()
						  {
							  ShdetNo = d.ShdetNo,
							  CustNo = d.CustNo,
							  sNo = d.sNo,
							  CarID = v == null ? null : v.CarID,
							  CarNo = v == null ? null : v.CarNO,
							  Phone = s == null ? null : s.Phone,
							  CarKind = v == null ? null : v.CarKind,
							  LoadSafety = v == null ? null : v.LoadSafety,
							  WeigTotal = gpProd.WeigTotal ?? 0,
							  ValueSafe = v == null ? null : v.ValueSafe,
							  ValueTotal = gpProd.iTotNumTotal ?? 0,
							  Count = gpProd.Count ?? 0,
							  OveriTotNum = gpProd.WeigTotal > v.LoadSafety ? true : false,
							  OverWeig = gpProd.iTotNumTotal > v.ValueSafe ? true : false,
							  TotalSheetNo = gpProd.SheetNoTotal
						  };

			if (data.ShdetNo.IsNotEmpty())
				vehicle = vehicle.Where(x => x.ShdetNo.Contains(data.ShdetNo));
			if (data.CustNo.IsNotEmpty())
				vehicle = vehicle.Where(x => x.CustNo.Contains(data.CustNo));
			if (data.sNo > 0)
				vehicle = vehicle.Where(x => x.sNo == data.sNo);

			if (data.CarID.IsEmpty())
				vehicle = vehicle.Where(x => x.CarID == "★");
			if (data.CarID.IsNotEmpty())
			{
				vehicle = vehicle.Where(x => x.CarID != null);
				vehicle = vehicle.Where(x => x.CarID.Contains(data.CarID));
			}
			if (data.CarNo.IsNotEmpty())
			{
				vehicle = vehicle.Where(x => x.CarNo != null);
				vehicle = vehicle.Where(x => x.CarNo.Contains(data.CarNo));
			}
			if (data.CarKind.IsNotEmpty())
			{
				vehicle = vehicle.Where(x => x.CarKind != null);
				vehicle = vehicle.Where(x => x.CarKind.Contains(data.CarKind));
			}

			int records = vehicle.Count();
			vehicle = vehicle.OrderBy(x => x.CarID).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "OK",
				Records = records,
				Pages = page,
				Data = vehicle,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};

			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetCreateBy(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var dataList = new List<ShdetMD>();
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;
			var shdetDataQ = from d in db.ShdetDetail.Where(x => x.IsDelete == false && x.RedyDate <= eDate && sDate <= x.RedyDate)
							 join cUser in db.SYS_User
							 on d.CreatedBy equals cUser.Account into ps
							 from cUser in ps.DefaultIfEmpty()
							 join h in db.ShdetHeader.Where(x => x.IsDelete == false)
							 on d.ShdetNo equals h.ShdetNo into ps2
							 from h in ps2.DefaultIfEmpty()
							 where h.IsDesp == true
							 select new ShdetMD()
							 {
								 CreatedBy = cUser.UserName,
								 CreatedDate = h.CreatedDate,
							 };

			var shdetDataE = shdetDataQ as IEnumerable<ShdetMD>;
			var shdetData = shdetDataE.Distinct(new CreateByCompare()).OrderBy(x => x.CreatedDate);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = shdetData,
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		public class CreateByCompare : IEqualityComparer<ShdetMD>
		{
			bool IEqualityComparer<ShdetMD>.Equals(ShdetMD x, ShdetMD y)
			{
				return x.CreatedBy == y.CreatedBy;
			}

			int IEqualityComparer<ShdetMD>.GetHashCode(ShdetMD obj)
			{
				int hash = 17;
				hash = hash * 23 + (obj.CreatedBy).GetHashCode();
				return hash;
			}
		}

		[Authorize]
		public ActionResult GetUpdatedBy(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var dataList = new List<ShdetMD>();
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;
			var shdetDataQ = from d in db.ShdetDetail.Where(x => x.IsDelete == false && x.UpdatedBy != null && x.RedyDate <= eDate && sDate <= x.RedyDate)
							 join cUser in db.SYS_User
							 on d.UpdatedBy equals cUser.Account into ps
							 from cUser in ps.DefaultIfEmpty()
							 join h in db.ShdetHeader
							 on d.ShdetNo equals h.ShdetNo into ps2
							 from h in ps2.DefaultIfEmpty()
							 where h.IsDesp == true
							 select new ShdetMD()
							 {
								 UpdatedBy = cUser.UserName ?? "",
								 UpdatedDate = d.UpdatedDate,
								 CreatedDate = h.CreatedDate,
							 };

			var shdetDataE = shdetDataQ as IEnumerable<ShdetMD>;
			var shdetData = shdetDataE.Distinct(new UpdatedByCompare()).OrderBy(x => x.CreatedDate);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = shdetData,
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		public class UpdatedByCompare : IEqualityComparer<ShdetMD>
		{
			bool IEqualityComparer<ShdetMD>.Equals(ShdetMD x, ShdetMD y)
			{
				return x.UpdatedBy == y.UpdatedBy;
			}

			int IEqualityComparer<ShdetMD>.GetHashCode(ShdetMD obj)
			{
				int hash = 17;
				hash = hash * 23 + (obj.UpdatedBy).GetHashCode();
				return hash;
			}
		}

		[Authorize]
		public ActionResult GetGridJSON_Sector(ShdetMD data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var dataList = new List<ShdetMD>();
			var sDate = start_date.Value.Date;
			var eDate = end_date.Value.Date;
			var shdetDataQ = from d in db.ShdetDetail.Where(x => x.IsDelete == false && x.SectorNo != null && x.RedyDate <= eDate && sDate <= x.RedyDate)
							 join s in db.ORG_Sector
							 on d.SectorNo equals s.SectorNo into ps
							 from s in ps.DefaultIfEmpty()
							 join h in db.ShdetHeader
							 on d.ShdetNo equals h.ShdetNo into ps2
							 from h in ps2.DefaultIfEmpty()
							 where h.IsDesp == true
							 select new ShdetMD()
							 {
								 RedyDate = d.RedyDate,
								 ReserveDate = h.ReserveDate,
								 SectorNo = s.SectorNo,
								 SectorName = s.SectorName,
								 CreatedDate = d.CreatedDate,
							 };

			var shdetDataE = shdetDataQ as IEnumerable<ShdetMD>;
			var shdetData = shdetDataE.Distinct(new SectorCompare()).OrderBy(x => x.CreatedDate);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = shdetData,
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		public class SectorCompare : IEqualityComparer<ShdetMD>
		{
			bool IEqualityComparer<ShdetMD>.Equals(ShdetMD x, ShdetMD y)
			{
				return x.SectorName == y.SectorName;
			}

			int IEqualityComparer<ShdetMD>.GetHashCode(ShdetMD obj)
			{
				int hash = 17;
				hash = hash * 23 + (obj.SectorName).GetHashCode();
				return hash;
			}
		}

		private bool CheckAllProdFinished(string ShdetNo, string CustNo, int sDtlNo)
		{
			if (db.ShdetProd.Any(x => x.IsDelete == false && x.ShdetNo == ShdetNo && x.CustNo == CustNo && x.sDtlNo == sDtlNo))
			{
				var finishRecord = from record in db.ShdetProd
								   where record.IsFinish != true && record.IsDelete == false && record.ShdetNo == ShdetNo && record.CustNo == CustNo && record.sDtlNo == sDtlNo
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
			}
			else
			{
				// do nothing
			}
			return false;
		}

		private bool CheckAllDetailFinished(string ShdetNo)
		{
			if (db.ShdetDetail.Any(x => x.IsDelete == false && x.ShdetNo == ShdetNo))
			{
				var finishRecord = from record in db.ShdetDetail
								   where record.IsFinish != true && record.IsDelete == false && record.ShdetNo == ShdetNo
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
			}
			else
			{
				// do nothing
			}
			return false;
		}
	}
}