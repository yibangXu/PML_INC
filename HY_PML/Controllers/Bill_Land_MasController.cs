using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class Bill_Land_MasController : Controller
	{
		private string path = WebConfigurationManager.AppSettings["path"]; //目標路徑
		private string username = WebConfigurationManager.AppSettings["username"]; //ftp使用者名稱
		private string password = WebConfigurationManager.AppSettings["password"]; //ftp密碼
		string FileTablePath = WebConfigurationManager.AppSettings["FileTablePath"];
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
			ViewBag.Title = "理貨單";
			ViewBag.ControllerName = "Bill_Land_Mas";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.FormCustomJsNew = "$('#addProd').show()";
			ViewBag.FormCustomJsEdit = "$('#addProd').hide()";

			//子表
			ViewBag.Title2 = "";
			ViewBag.ControllerName2 = "Bill_Land_Mas";
			ViewBag.AddFunc2 = "AddDetail";
			ViewBag.EditFunc2 = "EditDetail";
			ViewBag.DelFunc2 = "DeleteDetail";
			ViewBag.FormPartialName2 = "_ElementInForm2";
			ViewBag.FormCustomJsNew2 = @"$('.prodEdit').hide();$('#addProdDtl').show();
$('#dSheetNo').textbox('setValue', row.SheetNo);
$('#dSheetNo').textbox('setText', row.SheetNo);
$('#dMasterNo').textbox('setValue', row.MasterNo);
$('#dMasterNo').textbox('setText', row.MasterNo);";
			ViewBag.FormCustomJsEdit2 = "$('.prodEdit').show();$('#addProdDtl').hide()";

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

		public class ProdList
		{
			public string ShdetNo { set; get; }
			public string CustNo { set; get; }
			public int sDtlNo { set; get; }
			public int sNo { set; get; }
		}

		[Authorize]
		public ActionResult Add(Bill_Land_Mas data, string multiList)
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
					var userRecord = new Bill_Land_Mas();
					userRecord.MasterNo = data.MasterNo;
					userRecord.FlightNo = data.FlightNo;
					userRecord.Remark = data.Remark;
					userRecord.StatNo = data.StatNo;
					userRecord.HubNo = data.HubNo;

					//編流水號
					{
						var prefix = DateTime.Now.ToString("yyyyMMdd");
						var lastSheetNo = db.Bill_Land_Mas.Where(x => x.SheetNo.Contains(prefix)).OrderByDescending(x => x.SheetNo).Select(x => x.SheetNo).FirstOrDefault();
						if (lastSheetNo != null)
							userRecord.SheetNo = prefix + (Convert.ToInt32(lastSheetNo.Substring(8, 2)) + 1).ToString().PadLeft(2, '0');
						else
							userRecord.SheetNo = prefix + "1".PadLeft(2, '0');
					}
					//以下系統自填
					userRecord.CreateTime = DateTime.Now;
					userRecord.CreateBy = User.Identity.Name;
					userRecord.IsDelete = false;

					db.Bill_Land_Mas.Add(userRecord);
					db.SaveChanges();
					result.Ok = DataModifyResultType.Warning;
					result.Message = "主檔儲存完成";

					//新增子表貨物
					if (multiList != null)
					{
						var prods = multiList.Split('|').Select(prod => prod.Split(','));
						var prodDtl = prods.Select((p, index) => new Bill_Land_Dtl()
						{
							SheetNo = userRecord.SheetNo,
							SNo = index + 1,
							ShdetNo = p[0],
							CustomerNo = p[1],
							SDtlNo = Convert.ToInt32(p[2]),
							ProdNo = Convert.ToInt32(p[3]),
							CreateBy = User.Identity.Name,
							CreateTime = DateTime.Now,
							Remark = p[4],
							IsDelete = false
						});
						db.Bill_Land_Dtl.AddRange(prodDtl);

						//回寫貨物單號
						foreach (var i in prodDtl)
						{
							var p = db.ShdetProd.FirstOrDefault(x => x.ShdetNo == i.ShdetNo && x.CustNo == i.CustomerNo && x.sDtlNo == i.SDtlNo && x.sNo == i.ProdNo);
							p.SheetNo = i.SheetNo;
							p.SSNo = i.SNo;
							p.UpdatedBy = User.Identity.Name;
							p.UpdatedDate = DateTime.Now;
							db.Entry(p).State = EntityState.Modified;
						};
					}
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "主表子表儲存完成";
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
		public ActionResult AddDetail(Bill_Land_Dtl data, string multiList)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				try
				{
					//新增子表貨物
					var prods = multiList.Split('|').Select(prod => prod.Split(','));
					var seq = db.Bill_Land_Dtl.Where(x => x.SheetNo == data.SheetNo).OrderByDescending(x => x.SNo).Select(x => x.SNo).FirstOrDefault();
					var prodDtl = prods.Select((p, index) => new Bill_Land_Dtl()
					{
						SheetNo = data.SheetNo,
						MasterNo = data.MasterNo,
						SNo = seq + index + 1,
						ShdetNo = p[0],
						CustomerNo = p[1],
						SDtlNo = Convert.ToInt32(p[2]),
						ProdNo = Convert.ToInt32(p[3]),
						CreateBy = User.Identity.Name,
						CreateTime = DateTime.Now,
						Remark = p[4],
						IsDelete = false
					});
					db.Bill_Land_Dtl.AddRange(prodDtl);

					//回寫貨物單號
					foreach (var i in prodDtl)
					{
						var p = db.ShdetProd.FirstOrDefault(x => x.ShdetNo == i.ShdetNo && x.CustNo == i.CustomerNo && x.sDtlNo == i.SDtlNo && x.sNo == i.ProdNo);
						p.SheetNo = i.SheetNo;
						p.SSNo = i.SNo;
						p.UpdatedBy = User.Identity.Name;
						p.UpdatedDate = DateTime.Now;
						db.Entry(p).State = EntityState.Modified;
					};


					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
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
		public ActionResult Edit(Bill_Land_Mas data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.Bill_Land_Mas.FirstOrDefault(x => x.SheetNo == data.SheetNo);
				if (userRecord != null)
				{
					userRecord.MasterNo = data.MasterNo;
					userRecord.FlightNo = data.FlightNo;
					userRecord.Remark = data.Remark;
					userRecord.StatNo = data.StatNo;
					userRecord.HubNo = data.HubNo;

					//以下系統自填
					userRecord.UpdateTime = DateTime.Now;
					userRecord.UpdateBy = User.Identity.Name;
					db.Entry(userRecord).State = EntityState.Modified;

					var dtl = db.Bill_Land_Dtl.Where(x => x.SheetNo == data.SheetNo);
					foreach (var d in dtl)
					{
						d.MasterNo = data.MasterNo;
						db.Entry(d).State = EntityState.Modified;
					}

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
		public ActionResult EditDetail(Bill_Land_Dtl data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.Bill_Land_Dtl.FirstOrDefault(x => x.SheetNo == data.SheetNo && x.SNo == data.SNo);
				if (userRecord != null)
				{
					userRecord.WriteOff = data.WriteOff;
					userRecord.BagNo = data.BagNo;
					userRecord.Remark = data.Remark;

					//以下系統自填
					userRecord.UpdateTime = DateTime.Now;
					userRecord.UpdateBy = User.Identity.Name;

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
					trans.Rollback();
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult EditHouseNo(string HouseNo, string SheetNo, string multiList)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();

			using (var trans = db.Database.BeginTransaction())
			{
				var dtl = multiList.Split(',');
				foreach (var i in dtl)
				{
					var sno = Convert.ToInt32(i);
					var target = db.Bill_Land_Dtl.FirstOrDefault(x => x.SheetNo == SheetNo && x.SNo == sno);
					target.HouseNo = HouseNo;
					target.UpdateBy = User.Identity.Name;
					target.UpdateTime = DateTime.Now;
					db.Entry(target).State = EntityState.Modified;
				}

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

		[Authorize]
		public ActionResult Delete(Bill_Land_Mas data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.Bill_Land_Mas.FirstOrDefault(x => x.SheetNo == data.SheetNo);
				var dtl = db.Bill_Land_Dtl.Where(x => x.SheetNo == data.SheetNo);
				foreach (var d in dtl)
				{
					d.IsDelete = true;
					d.DeleteBy = User.Identity.Name;
					d.DeleteTime = DateTime.Now;
					db.Entry(d).State = EntityState.Modified;
				}

				var prod = db.ShdetProd.Where(p => p.SheetNo == data.SheetNo);
				foreach (var p in prod)
				{
					p.SheetNo = null;
					p.SSNo = null;
					p.UpdatedBy = User.Identity.Name;
					p.UpdatedDate = DateTime.Now;
					db.Entry(p).State = EntityState.Modified;
				}

				if (userRecord != null)
				{
					//以下系統自填
					userRecord.DeleteTime = DateTime.Now;
					userRecord.DeleteBy = User.Identity.Name;
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
		public ActionResult DeleteDetail(Bill_Land_Dtl data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.Bill_Land_Dtl.FirstOrDefault(x => x.SheetNo == data.SheetNo && x.SNo == data.SNo);

			if (userRecord != null)
			{
				//以下系統自填
				userRecord.DeleteTime = DateTime.Now;
				userRecord.DeleteBy = User.Identity.Name;
				userRecord.IsDelete = true;

				var prod = db.ShdetProd.FirstOrDefault(x => x.SheetNo == data.SheetNo && x.SSNo == data.SNo);
				if (prod != null)
				{
					prod.SSNo = null;
					prod.SheetNo = null;
				}
				try
				{
					db.Entry(userRecord).State = EntityState.Modified;
					db.Entry(prod).State = EntityState.Modified;
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
		public ActionResult GetGridJSON(Bill_Land_Mas data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
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

			var billLand =
				from b in db.Bill_Land_Mas
				join s in db.ORG_Stat
				on b.StatNo equals s.StatNo into ps
				from s in ps.DefaultIfEmpty()
				join h in db.ORG_Hub
				on b.HubNo equals h.HubNo into ps1
				from h in ps1.DefaultIfEmpty()
				join u in db.SYS_User on b.CreateBy equals u.Account into ps2
				from u in ps2.DefaultIfEmpty()
				where b.IsDelete == false
				select new
				{
					SheetNo = b.SheetNo,
					FlightNo = b.FlightNo,
					MasterNo = b.MasterNo,
					StatNo = b.StatNo,
					StatName = s == null ? null : s.StatName,
					HubNo = b.HubNo,
					HubName = h == null ? null : h.HubName,
					Remark = b.Remark,
					CreateBy = u.UserName,
					CreateTime = b.CreateTime,
					UpdateBy = b.UpdateBy,
					UpdateTime = b.UpdateTime,
					DeleteBy = b.DeleteBy,
					DeleteTime = b.DeleteTime,
					IsDelete = b.IsDelete
				};

			billLand = billLand.Where(x => statNoList.Contains(x.StatNo));
			if (data.SheetNo.IsNotEmpty())
				billLand = billLand.Where(x => x.SheetNo.Contains(data.SheetNo));
			if (data.FlightNo.IsNotEmpty())
				billLand = billLand.Where(x => x.FlightNo.Contains(data.FlightNo));
			if (data.Remark.IsNotEmpty())
				billLand = billLand.Where(x => x.Remark.Contains(data.Remark));
			if (data.HubName.IsNotEmpty())
				billLand = billLand.Where(x => x.HubName == data.HubName);
			if (data.StatNo.IsNotEmpty())
				billLand = billLand.Where(x => x.StatNo == data.StatNo);
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				billLand = billLand.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			int records = billLand.Count();
			billLand = billLand.OrderByDescending(o => o.SheetNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = billLand,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetProdGridJSON(ShdetProd data, int page = 1, int rows = 40, string searchType = "1", DateTime? start_date = null, DateTime? end_date = null)
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

			var prod = from p in db.ShdetProd
					   join sd in db.ShdetDetail
					   on new { sNo = p.sDtlNo, CustNo = p.CustNo, ShdetNo = p.ShdetNo } equals new { sNo = sd.sNo, CustNo = sd.CustNo, ShdetNo = sd.ShdetNo }
					   join s in db.ShdetHeader
					   on new { p.CustNo, p.ShdetNo } equals new { s.CustNo, s.ShdetNo }
					   join h in db.ORG_Hub
					   on s.HubNo equals h.HubNo into ps
					   from h in ps.DefaultIfEmpty()
					   where p.IsDelete == false && !(p.SheetNo != null) && sd.IsDelete == false && s.IsDelete == false && s.IsDesp == true && s.IsCancel != true && sd.IsCancel != true && p.IsCancel != true
					   select new
					   {
						   ShdetNo = p.ShdetNo,
						   CustNo = p.CustNo,
						   CustCHName = s == null ? null : s.CustCHName,
						   CarryName = sd == null ? null : sd.CarryName,
						   ShdetDate = s == null ? null : s.ShdetDate,
						   sDtlNo = p.sDtlNo,
						   sNo = p.sNo,
						   Pcs = p.Pcs,
						   WeigLevel = p.WeigLevel,
						   Weig = p.Weig,
						   CocustomTyp = p.CocustomTyp,
						   HubNo = s == null ? null : s.HubNo,
						   HubName = h == null ? null : h.HubName,
						   CcNo = p.CcNo,
						   Charge = p.Charge,
						   Dest = p.Dest,
						   RedyDate = sd == null ? null : sd.RedyDate,
						   RedyTime = sd == null ? null : sd.RedyTime,
						   Remark1 = p.Remark1,
						   Remark3 = p.Remark3,
						   SectorNo = p.SectorNo,
						   CallType = p.CallType,
						   StatNo = sd == null ? null : sd.StatNo,
						   CallStatNo = sd == null ? null : sd.CallStatNo,
						   CarID = p.CarID,
						   ReplyComment = p.ReplyComment,
						   fLen = p.fLen,
						   fWidth = p.fWidth,
						   fHeight = p.fHeight,
						   iNum = p.iNum,
						   iTotNum = p.iTotNum,
						   p.CreatedDate,
						   CocustomTypStr = sd == null ? null : sd.CocustomTyp == 0 ? "不報關" : sd.CocustomTyp == 1 ? "正式報關" : sd.CocustomTyp == 2 ? "簡易報關" : sd.CocustomTyp == 3 ? "其他" : "",
						   SDate = s == null ? null : s.SDate,
					   };
			prod = prod.Where(x => statNoList.Contains(x.CallStatNo));

			if (data.ShdetNo.IsNotEmpty())
				prod = prod.Where(x => x.ShdetNo.Contains(data.ShdetNo));
			if (data.CustNo.IsNotEmpty())
				prod = prod.Where(x => x.CustNo.Contains(data.CustNo));
			if (data.HubName.IsNotEmpty())
				prod = prod.Where(x => x.HubName.Contains(data.HubName));
			if (data.StatNo.IsNotEmpty())
				prod = prod.Where(x => x.StatNo.Contains(data.StatNo));
			if (data.CallStatNo.IsNotEmpty())
				prod = prod.Where(x => x.CallStatNo.Contains(data.CallStatNo));
			if (data.CocustomTypStr.IsNotEmpty())
				prod = prod.Where(x => x.CocustomTypStr == data.CocustomTypStr);

			var sDate = Convert.ToDateTime(start_date.Value.Date.ToDateString() + " " + "00:00:00");
			var eDate = Convert.ToDateTime(end_date.Value.Date.ToDateString() + " " + "23:59:59");
			if (searchType == "1")
			{
				if (start_date != null && end_date != null)
				{
					prod = prod.Where(x => DbFunctions.TruncateTime(x.ShdetDate).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.ShdetDate).Value.CompareTo(sDate) >= 0);
				}
			}
			else if (searchType == "2")
			{
				if (start_date != null && end_date != null)
				{
					prod = prod.Where(x => DbFunctions.TruncateTime(x.RedyDate).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.RedyDate).Value.CompareTo(sDate) >= 0);
				}
			}
			else if (searchType == "3")
			{
				if (start_date != null && end_date != null)
				{
					prod = prod.Where(x => x.SDate != null);
					prod = prod.Where(x => DbFunctions.TruncateTime(x.SDate).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.SDate).Value.CompareTo(sDate) >= 0);
				}
			}

			var prodData = new List<Bill_Land_Prod>();
			var seq = 1;
			foreach (var i in prod.ToList())
			{
				var tData = new Bill_Land_Prod()
				{
					index = seq++,
					ShdetNo = i.ShdetNo,
					CustNo = i.CustNo,
					CustCHName = i.CustCHName,
					CarryName = i.CarryName,
					sDtlNo = i.sDtlNo,
					sNo = i.sNo,
					Pcs = i.Pcs,
					WeigLevel = i.WeigLevel,
					Weig = i.Weig,
					CocustomTyp = i.CocustomTyp,
					HubNo = i.HubNo,
					HubName = i.HubName,
					CcNo = i.CcNo,
					Charge = i.Charge,
					Dest = i.Dest,
					RedyDate = i.RedyDate,
					RedyTime = i.RedyTime,
					Remark1 = i.Remark1,
					Remark3 = i.Remark3,
					SectorNo = i.SectorNo,
					CallType = i.CallType,
					StatNo = i.StatNo,
					CallStatNo = i.CallStatNo,
					CarID = i.CarID,
					ShdetDate = i.ShdetDate,
					ReplyComment = i.ReplyComment,
					fLen = i.fLen,
					fWidth = i.fWidth,
					fHeight = i.fHeight,
					iNum = i.iNum,
					iTotNum = i.iTotNum,
					CreatDate = i.CreatedDate,
					CocustomTypStr = i.CocustomTypStr,
				};

				prodData.Add(tData);
			}

			var prodList = prodData as IEnumerable<Bill_Land_Prod>;

			int records = prodList.Count();
			prodList = prodList.OrderByDescending(x => x.CreatDate).ThenBy(o => o.StatNo).ThenBy(x => x.SectorNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = prodList,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		public ActionResult Report(string id)
		{
			var target = db.Bill_Land_Mas.FirstOrDefault(x => x.IsDelete == false && x.SheetNo == id);
			if (target != null)
			{
				//以下系統自填
				target.PrintTime = DateTime.Now;
				try
				{
					db.Entry(target).State = EntityState.Modified;
					db.SaveChanges();
				}
				catch (Exception e)
				{
				}
			}

			var datas = new List<Bill_Land_Report>();
			var bld = from b in db.Bill_Land_Dtl
					  join sh in db.ShdetHeader
					  on b.ShdetNo equals sh.ShdetNo into ps
					  from sh in ps.DefaultIfEmpty()
					  join prod in db.ShdetProd
					  on new { ShdetNo = b.ShdetNo, CustNo = b.CustomerNo, SDtlNo = b.SDtlNo, ProdNo = b.ProdNo }
					  equals new { ShdetNo = prod.ShdetNo, CustNo = prod.CustNo, SDtlNo = prod.sDtlNo, ProdNo = prod.sNo } into ps1
					  from prod in ps1.DefaultIfEmpty()
					  join sd in db.ShdetDetail
					  on new { ShdetNo = prod.ShdetNo, CustNo = prod.CustNo, SDtlNo = prod.sDtlNo }
					  equals new { ShdetNo = sd.ShdetNo, CustNo = sd.CustNo, SDtlNo = sd.sNo }
					  join s in db.ORG_Sector
					   on sd.SectorNo equals s.SectorNo into ps2
					  from s in ps2.DefaultIfEmpty()
					  join bl in db.Bill_Lading
					  on b.ShdetNo equals bl.LadingNo into ps3
					  from bl in ps3.DefaultIfEmpty()
					  where b.SheetNo == id && b.IsDelete == false
					  select new Bill_Land_Report_Items()
					  {
						  LadingNo = b == null ? sh.ShdetNo : bl.LadingNo_Type,
						  MasterNo = b.MasterNo,
						  HouseNo = b.HouseNo,
						  CarryName = sd.CarryName,
						  StatNo = sd.StatNo,
						  SectorName = s == null ? "" : s.SectorName,
						  WeigLevel = sd.WeigLevel == 0 ? "D" : "S",
						  Pcs = prod == null ? null : prod.Pcs,
						  Weight = prod == null ? null : prod.Weig,
						  iTotNum = prod == null ? null : prod.iTotNum,
						  BagNo = b.BagNo,
						  CocustomTyp = (sd.CocustomTyp == 0 ? "　" : (sd.CocustomTyp == 1 ? "正" : (sd.CocustomTyp == 2 ? "簡" : "　"))),
						  WriteOff = b.WriteOff,
						  Remark = b.Remark
					  };

			var bldgroup = from b in bld
						   group b by b.HouseNo;

			var blm = from b in db.Bill_Land_Mas
					  join h in db.ORG_Hub
					  on b.HubNo equals h.HubNo into ps
					  from h in ps.DefaultIfEmpty()
					  where b.IsDelete == false && b.SheetNo == id
					  select new
					  {
						  SheetNo = b.SheetNo,
						  MasterNo = b.MasterNo,
						  FlightNo = b.FlightNo,
						  HubNo = b.HubNo,
						  HubName = h == null ? "" : h.HubName,
						  StatNo = b.StatNo,
						  Remark = b.Remark,
						  PrintTime = b.PrintTime,
						  CreateBy = b.CreateBy,
						  CreateTime = b.CreateTime,
						  UpdateBy = b.UpdateBy,
						  UpdateTime = b.UpdateTime,
						  DeleteBy = b.DeleteBy,
						  DeleteTime = b.DeleteTime,
						  IsDelete = b.IsDelete
					  };

			var blmData = blm.FirstOrDefault();

			foreach (var i in bldgroup)
			{
				var tData = new Bill_Land_Report()
				{
					SheetNo = blmData.SheetNo,
					MasterNo = blmData.MasterNo,
					FlightNo = blmData.FlightNo,
					MasRemark = blmData.Remark,
					HubName = blmData.HubName,
					PrintTime = blmData.PrintTime,
					Items = i.ToList()
				};
				datas.Add(tData);
			}
			ViewData.Model = datas;
			return View();
		}

		public void Pdf(string id)
		{
			var ID = id.Split('|')[0];
			var type = id.Split('|')[1].Split('—')[0];
			var color = id.Split('—')[1];
			var account = User.Identity.Name;
			var blmData = db.Bill_Land_Mas.Where(x => x.SheetNo == ID).FirstOrDefault();
			var hubData = db.ORG_Hub.Where(x => x.HubNo == blmData.HubNo).FirstOrDefault();
			var fileName = string.Format("{0:yyyyMMdd_HHmmss}", blmData.CreateTime) + "-" + hubData.HubName + "-" + blmData.StatNo;
			string fileNameWithOutExtention = Guid.NewGuid().ToString();
			string reportUrl = Request.Url.AbsoluteUri.Replace("Pdf/", "Report3?id=").Replace("%7C3", "").Replace("%E2%80%94", "%7C") + "&account=" + account;
			//string reportUrl = "http://exp.pml-intl.com//New_Bill_Lading/Report/" + id;

			string filePath = FileTablePath + fileNameWithOutExtention + ".pdf";

			//執行wkhtmltopdf.exe
			Process p = System.Diagnostics.Process.Start(
				@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe",
				@"-T 3 -R 3 -B 3 -L 3 --page-width 209 --page-height 148 --minimum-font-size 11  --disable-smart-shrinking --print-media-type" +
				" " + reportUrl +
				" " + filePath
				);
			p.WaitForExit();


			//把檔案讀進串流
			FileStream fs = new FileStream(filePath, FileMode.Open);
			byte[] file = new byte[fs.Length];
			fs.Read(file, 0, file.Length);
			fs.Close();

			System.IO.File.Delete(filePath);

			//Response給用戶端下載
			Response.Clear();
			Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".pdf");//強制下載
			Response.ContentType = "application/octet-stream";
			Response.BinaryWrite(file);
			Response.End();
		}

		//PML
		public ActionResult Report3(string id, string account)
		{
			var ID = id.Split('|')[0];
			var Type = id.Split('|')[1];
			var viewData = new List<BillLadingReport>();
			var dtlData = db.Bill_Land_Dtl.Where(x => x.IsDelete == false && x.SheetNo == ID);
			foreach (var d in dtlData)
			{
				if (db.Bill_Lading.Any(x => x.LadingNo == d.ShdetNo))
				{
					var isInLading = db.Bill_Lading.Where(x => x.LadingNo == d.ShdetNo).Select(x => x.IsInLading).FirstOrDefault() ?? false;
					if (isInLading)
					{
						var SdData = db.ShdetDetail.Where(x => x.ShdetNo == d.ShdetNo && x.IsDelete == false).OrderBy(x => x.sNo);
						var SdCount = SdData.Count();
						var SectorName = SdCount != 0 ? db.ORG_Sector.Where(s => s.SectorNo == SdData.Select(x => x.SectorNo).FirstOrDefault()).Select(x => x.SectorName).FirstOrDefault() : " ";
						var Sector = SdCount == 0 ? "" : SdCount == 1 ? SectorName : SectorName + "(多地取件)";
						var PrintBy = db.SYS_User.Where(x => x.Account == account).Select(x => x.UserName).FirstOrDefault();

						var BlData =
							from b in db.Bill_Lading.Where(x => x.LadingNo == d.ShdetNo && x.IsDelete == false)
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
						var bd = BlData.FirstOrDefault();
						var temp = new BillLadingReport()
						{
							LadingNo = bd.LadingNo,
							LadingNo_Type = bd.LadingNo_Type,
							LadingDate = string.Format("{0:yyyy/MM/dd}", bd.LadingDate),
							SendCustNo = bd.SendCustNo,
							SendCompany = bd.SendCompany,
							SendBy = bd.SendBy,
							SendInvNo = bd.SendInvNo,
							SendCustAddr = "　　　" + bd.SendCustAddr,
							SendPhone = bd.SendPhone,
							SendFaxNo = bd.SendFaxNo,
							RecCompany = bd.RecCompany,
							RecChAddr = "　　　" + bd.RecChAddr,
							RecBy = bd.RecBy,
							RecInvNo = bd.RecInvNo,
							RecPhone = bd.RecPhone,
							RecMPhone = bd.RecMPhone,
							SStatNo = bd.SStatNo,
							AStatNo = bd.AStatNo,
							DestNo = bd.DestNo,
							Qty = (int)bd.Qty,
							Weight = bd.Weight,
							HubNo = bd.HubNo,
							HubName = bd.HubName,
							HubPName = bd.HubPName,
							Volume = bd.Volume,
							CcNo = bd.CcNo,
							Currency = bd.Currency,
							ToPayment = bd.ToPayment,
							Remark = bd.Remark,
							Type = bd.Type,
							ProductName = bd.ProductName,
							CocustomTyp = bd.CocustomTyp,
							SendRemark = bd.SendRemark,
							PiecesNo = bd.PiecesNo,
							Sale = bd.Sale,
							SDate = string.Format("{0:yyyy/MM/dd}", bd.SDate),
							Sector = Sector,
							PrintBy = PrintBy,
							PrintTime = DateTime.Now.ToDateTimeString(),
						};
						viewData.Add(temp);
					}
				}
			}
			ViewData.Model = viewData;
			ViewBag.PageSize = "E-Report3_" + Type;
			return View();
		}
	}
}