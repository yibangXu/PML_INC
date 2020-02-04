using ExcelDataReader;
using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HY_PML.Controllers
{
	public class Bill_LadingController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;
		public ActionResult _ElementInForm1()
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

			ViewBag.Title = "提單資料";
			ViewBag.dlgWidth = "960px";
			ViewBag.ControllerName = "Bill_Lading";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.SaveRecFunc = "SaveRec";
			ViewBag.FormCustomJsNew =
				$"$('.ImOrEx').prop('disabled',false);" +
				$"$('#SStatNo1').textbox('setValue', '{statNo}');" +
				$"$('#SStatName1').textbox('setValue', '{statName}');" +
				$"$('#SStatNo2').textbox('setValue', '{statNo}'); " +
				$"$('#SStatName2').textbox('setValue', '{statName}');";
			//ViewBag.FormCustomJsEdit = "$('.ImOrEx').prop('disabled', true);";
			ViewBag.FormCustomJsEdit = "";
			ViewBag.PageParam = isTw ? "2" : "1";

			//子表
			ViewBag.Title2 = isTw ? "報關資料(次)" : "報關資料(主)";
			ViewBag.ControllerName2 = "Bill_Lading";
			ViewBag.AddFunc2 = isTw ? "AddDetail3" : "AddDetail2";
			ViewBag.EditFunc2 = isTw ? "EditDetail3" : "EditDetail2";
			ViewBag.DelFunc2 = isTw ? "DeleteDetail3" : "DeleteDetail2";
			ViewBag.FormPartialName2 = isTw ? "_ElementInForm" : "_ElementInForm2";
			ViewBag.FormCustomJsNew2 = @"$('.prodEdit').hide();$('#addProdDtl').show();$('#dLadingNo').textbox('setValue', row.LadingNo);";
			ViewBag.FormCustomJsEdit2 = "$('.prodEdit').show();$('#addProdDtl').hide()";
			ViewBag.IsTw = isTw;

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

			var billLading =
				from b in db.Bill_Lading
				join h in db.ORG_Hub
				on b.HubNo equals h.HubNo into ps1
				from h in ps1.DefaultIfEmpty()
				join u in db.SYS_User on b.CreateBy equals u.Account into ps2
				from u in ps2.DefaultIfEmpty()
				where statNoList.Contains(b.SStatNo) && b.IsDelete == false
				select new
				{
					LadingNo = b.LadingNo,
					WarehouseRNo = b.WarehouseRNo,
					HubNo = b.HubNo,
					HubName = h == null ? null : h.HubName,
					WarehouseRDate = b.WarehouseRDate,
					TransferNo = b.TransferNo,
					LadingDate = b.LadingDate,
					OrderNo = b.OrderNo,
					SendCustNo = b.SendCustNo,
					SendCHName = b.SendCHName,
					SendPhone = b.SendPhone,
					SendBy = b.SendBy,
					SendCustAddr = b.SendCustAddr,
					RecPhone = b.RecPhone,
					RecCustEName1 = b.RecCustEName1,
					RecCustEName2 = b.RecCustEName2,
					RecBy = b.RecBy,
					RecCompany = b.RecCompany,
					RecCustCHName = b.RecCustCHName,
					RecCustENAddr1 = b.RecCustENAddr1,
					RecCustENAddr2 = b.RecCustENAddr2,
					RecChAddr = b.RecChAddr,
					RecInvNo = b.RecInvNo,
					RecCity = b.RecCity,
					RecState = b.RecState,
					RecCountry = b.RecCountry,
					RecPostDist = b.RecPostDist,
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
				};

			if (data.LadingNo.IsNotEmpty())
				billLading = billLading.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.WarehouseRNo.IsNotEmpty())
				billLading = billLading.Where(x => x.WarehouseRNo.Contains(data.WarehouseRNo));
			if (data.HubName.IsNotEmpty())
				billLading = billLading.Where(x => x.HubName == data.HubName);
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				billLading = billLading.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			int records = billLading.Count();
			billLading = billLading.OrderBy(o => o.LadingNo).Skip((page - 1) * rows).Take(rows);

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

		#region 主表Add
		[Authorize]
		public ActionResult Add(Bill_Lading data, string[] dtl1, string[] dtl2)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var userRecord = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == data.LadingNo);
				if (userRecord == null)
				{
					var saveData = new Bill_Lading();
					saveData.LadingNo = data.LadingNo;
					saveData.WarehouseRNo = data.WarehouseRNo;
					saveData.HubNo = data.HubNo;
					saveData.WarehouseRDate = data.WarehouseRDate;
					saveData.TransferNo = data.TransferNo;
					saveData.LadingDate = data.LadingDate;
					saveData.OrderNo = data.OrderNo;
					saveData.SendCustNo = data.SendCustNo;
					saveData.SendCHName = data.SendCHName;
					saveData.SendPhone = data.SendPhone;
					saveData.SendBy = data.SendBy;
					saveData.SendCustAddr = data.SendCustAddr;
					saveData.RecPhone = data.RecPhone;
					saveData.RecCustEName1 = data.RecCustEName1;
					saveData.RecCustEName2 = data.RecCustEName2;
					saveData.RecBy = data.RecBy;
					saveData.RecCompany = data.RecCompany;
					saveData.RecCustCHName = data.RecCustCHName;
					saveData.RecCustENAddr1 = data.RecCustENAddr1;
					saveData.RecCustENAddr2 = data.RecCustENAddr2;
					saveData.RecChAddr = data.RecChAddr;
					saveData.RecInvNo = data.RecInvNo;
					saveData.RecCity = data.RecCity;
					saveData.RecState = data.RecState;
					saveData.RecCountry = data.RecCountry;
					saveData.RecPostDist = data.RecPostDist;
					saveData.SectorNo = data.SectorNo;
					saveData.SectorName = data.SectorName;
					saveData.SStatNo = data.SStatNo;
					saveData.SStatName = data.SStatName;
					saveData.AStatName = data.AStatName;
					saveData.AStatNo = data.AStatNo;
					saveData.DestNo = data.DestNo;
					saveData.CName = data.CName;
					saveData.Type = data.Type;
					saveData.ProductNo = data.ProductNo;
					saveData.ProductName = data.ProductName;
					saveData.PiecesNo = data.PiecesNo;
					saveData.Cost = data.Cost;
					saveData.CostCurrency = data.CostCurrency;
					saveData.CcNo = data.CcNo;
					saveData.PayCustNo = data.PayCustNo;
					saveData.PayCustCHName = data.PayCustCHName;
					saveData.Freight = data.Freight;
					saveData.FreightCurrency = data.FreightCurrency;
					saveData.FuelCosts = data.FuelCosts;
					saveData.ToPayment = data.ToPayment;
					saveData.ToPaymentCurrency = data.ToPaymentCurrency;
					saveData.AgentPay = data.AgentPay;
					saveData.AgentPayCurrency = data.AgentPayCurrency;
					saveData.ProdIdPay = data.ProdIdPay;
					saveData.CustomsPay = data.CustomsPay;
					saveData.InsurancePay = data.InsurancePay;
					saveData.OtherPayTax = data.OtherPayTax;
					saveData.OtherPayNoTax = data.OtherPayNoTax;
					saveData.Total = data.Total;
					saveData.ToPaymentCurrency = data.TotalCurrency;
					saveData.Remark = data.Remark;
					saveData.Remark2 = data.Remark2;
					saveData.ShdetNo = data.ShdetNo;
					saveData.ImOrEx = data.ImOrEx;

					//DeclCust_Main 處理
					if (dtl2 != null && dtl2.Count() > 0)
					{
						var datalist = dtl2[0].Replace("[", "").Replace("]", "").Replace("},{", "}㊣{").Split('㊣');
						var newData = new List<DeclCust_Main>();
						JavaScriptSerializer js = new JavaScriptSerializer();
						foreach (var d in datalist)
						{
							newData.Add(js.Deserialize<DeclCust_Main>(d));
						}
						var oldData = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo && x.IsDelete == false).ToArray();
						var cmp = new MainCompare();

						var addData = newData.Except(oldData, cmp);
						addData = addData.Select((x, Index) => new DeclCust_Main()
						{
							sNo = Index + 1,
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
						saveData.Qty = addData.Sum(x => x.Qty);
						saveData.Weight = addData.Sum(x => x.Weight);
						saveData.Volume = addData.Sum(x => x.GrossWeight);
					}

					//DeclCust_Sub 處理
					if (dtl1 != null && dtl1.Count() > 0)
					{
						var datalist = dtl1[0].Replace("[", "").Replace("]", "").Replace("},{", "}㊣{").Split('㊣');
						var newData = new List<DeclCust_Sub>();
						JavaScriptSerializer js = new JavaScriptSerializer();
						foreach (var d in datalist)
						{
							newData.Add(js.Deserialize<DeclCust_Sub>(d));
						}
						var oldData = db.DeclCust_Sub.Where(x => x.LadingNo == data.LadingNo && x.IsDelete == false).ToArray();
						var cmp = new SubCompare();
						var addData = newData.Except(oldData, cmp);
						addData = addData.Select((x, Index) => new DeclCust_Sub()
						{
							sNo = Index + 1,
							LadingNo = x.LadingNo,
							BagNo = x.BagNo,
							CleCusCode = x.CleCusCode,
							ProductName = x.ProductName,
							Type = x.Type,
							Qty = x.Qty,
							Weight = x.Weight,
							GrossWeight = x.GrossWeight,
							Price = x.Price,
							Length = x.Length,
							Width = x.Width,
							Height = x.Height,
							Remark = x.Remark,
							CreateTime = DateTime.Now,
							CreateBy = User.Identity.Name,
							IsDelete = false,
						});
						db.DeclCust_Sub.AddRange(addData);
						saveData.Qty = addData.Sum(x => x.Qty);
						saveData.Weight = addData.Sum(x => x.Weight);
						saveData.Volume = addData.Sum(x => x.GrossWeight);
					}
					//以下系統自填
					saveData.Source = "Web";
					saveData.IsConfirm = true;
					saveData.IsCheck = false;
					saveData.IsReview = false;
					saveData.CreateTime = DateTime.Now;
					saveData.CreateBy = User.Identity.Name;
					saveData.IsDelete = false;

					db.Bill_Lading.Add(saveData);
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
					result.Message = "已存在此提單單號!";
					trans.Rollback();
				}
			}


			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region 子表(主)Add
		[Authorize]
		public ActionResult AddDetail2(DeclCust_Main data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var seq = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo).OrderByDescending(x => x.sNo).Select(x => x.sNo).FirstOrDefault();

				var saveData = new DeclCust_Main();
				saveData.LadingNo = data.LadingNo;
				saveData.sNo = seq + 1;
				saveData.CustType = data.CustType;
				saveData.Flight = data.Flight;
				saveData.LadNo = data.LadNo;
				saveData.BagNo = data.BagNo;
				saveData.CleCusCode = data.CleCusCode;
				saveData.CusCoode = data.CusCoode;
				saveData.ProductNo = data.ProductNo;
				saveData.ProductName = data.ProductName;
				saveData.ProductEName = data.ProductEName;
				saveData.Country = data.Country;
				saveData.Type = data.Type;
				saveData.HSNo = data.HSNo;
				saveData.Qty = data.Qty;
				saveData.Unit = data.Unit;
				saveData.GrossWeight = data.GrossWeight;
				saveData.Weight = data.Weight;
				saveData.Price = data.Price;
				saveData.Total = data.Total;
				saveData.Currency = data.Currency;
				saveData.Pcs = data.Pcs;
				saveData.PcsNo = data.PcsNo;
				saveData.Remark = data.Remark;
				saveData.UpdateBy = data.UpdateBy;
				saveData.UpdateTime = data.UpdateTime;
				saveData.DeletedBy = data.DeletedBy;
				saveData.DeletedTime = data.DeletedTime;
				saveData.IsDelete = data.IsDelete;

				//以下系統自填
				saveData.CreateTime = DateTime.Now;
				saveData.CreateBy = User.Identity.Name;
				saveData.IsDelete = false;

				db.DeclCust_Main.Add(saveData);
				try
				{
					db.SaveChanges();
					trans.Commit();
					result.Ok = DataModifyResultType.Success;
					result.Message = "OK";
				}
				catch (DbEntityValidationException e)
				{
					var dataError = new StringBuilder();
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					//foreach (var eve in e.EntityValidationErrors)
					//{
					//	Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
					//		eve.Entry.Entity.GetType().Name, eve.Entry.Entity.GetType().Name);
					//	foreach (var ve in eve.ValidationErrors)
					//	{
					//		Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
					//			ve.PropertyName, ve.ErrorMessage);
					//		dataError.Append(ve.ErrorMessage);
					//	}
					//	result.Message = dataError.ToString();
					//}
					result.Message = e.Message;
				}
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}
		#endregion

		#region 子表(次)Add
		[Authorize]
		public ActionResult AddDetail3(DeclCust_Sub data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var seq = db.DeclCust_Sub.Where(x => x.LadingNo == data.LadingNo).OrderByDescending(x => x.sNo).Select(x => x.sNo).FirstOrDefault();

				var saveData = new DeclCust_Sub();
				saveData.LadingNo = data.LadingNo;
				saveData.sNo = seq + 1;
				saveData.BagNo = data.BagNo;
				saveData.CleCusCode = data.CleCusCode;
				saveData.ProductName = data.ProductName;
				saveData.Type = data.Type;
				saveData.Qty = data.Qty;
				saveData.GrossWeight = data.GrossWeight;
				saveData.Weight = data.Weight;
				saveData.Price = data.Price;
				saveData.Remark = data.Remark;
				saveData.UpdateBy = data.UpdateBy;
				saveData.UpdateTime = data.UpdateTime;
				saveData.DeletedBy = data.DeletedBy;
				saveData.DeletedTime = data.DeletedTime;
				saveData.IsDelete = data.IsDelete;

				//以下系統自填
				saveData.CreateTime = DateTime.Now;
				saveData.CreateBy = User.Identity.Name;
				saveData.IsDelete = false;

				db.DeclCust_Sub.Add(saveData);
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

		#region 主表Edit
		[Authorize]
		public ActionResult Edit(Bill_Lading data, string[] dtl1, string[] dtl2)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));


			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				//DeclCust_Main 處理
				if (dtl2 != null && dtl2.Count() > 0)
				{
					var datalist = dtl2[0].Replace("[", "").Replace("]", "").Replace("},{", "}㊣{").Split('㊣');
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
					var originLastData = oldData.OrderByDescending(x => x.sNo).FirstOrDefault();
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

				//DeclCust_Sub 處理
				if (dtl1 != null && dtl1.Count() > 0)
				{
					var datalist = dtl1[0].Replace("[", "").Replace("]", "").Replace("},{", "}㊣{").Split('㊣');
					var newData = new List<DeclCust_Sub>();
					JavaScriptSerializer js = new JavaScriptSerializer();
					foreach (var d in datalist)
					{
						newData.Add(js.Deserialize<DeclCust_Sub>(d));
					}
					var oldData = db.DeclCust_Sub.Where(x => x.LadingNo == data.LadingNo && x.IsDelete == false).ToArray();
					var cmp = new SubCompare();
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
						var originData = db.DeclCust_Sub.FirstOrDefault(x => x.LadingNo == e.LadingNo && x.sNo == e.sNo);

						originData.BagNo = e.BagNo;
						originData.CleCusCode = e.CleCusCode;
						originData.ProductName = e.ProductName;
						originData.Type = e.Type;
						originData.Qty = e.Qty;
						originData.Weight = e.Weight;
						originData.GrossWeight = e.GrossWeight;
						originData.Price = e.Price;
						originData.Length = e.Length;
						originData.Width = e.Width;
						originData.Height = e.Height;
						originData.Remark = e.Remark;

						originData.UpdateBy = User.Identity.Name;
						originData.UpdateTime = DateTime.Now;

						db.Entry(originData).State = EntityState.Modified;
					}
					var Seq = oldData.OrderByDescending(x => x.sNo).FirstOrDefault().sNo;
					addData = addData.Select((x, Index) => new DeclCust_Sub()
					{
						sNo = Seq + Index + 1,
						LadingNo = x.LadingNo,
						BagNo = x.BagNo,
						CleCusCode = x.CleCusCode,
						ProductName = x.ProductName,
						Type = x.Type,
						Qty = x.Qty,
						Weight = x.Weight,
						GrossWeight = x.GrossWeight,
						Price = x.Price,
						Length = x.Length,
						Width = x.Width,
						Height = x.Height,
						Remark = x.Remark,

						CreateTime = DateTime.Now,
						CreateBy = User.Identity.Name,
						IsDelete = false,
					});
					db.DeclCust_Sub.AddRange(addData);
					data.Qty = addData.Sum(x => x.Qty) + editData.Sum(x => x.Qty);
					data.Weight = addData.Sum(x => x.Weight) + editData.Sum(x => x.Weight);
					data.Volume = addData.Sum(x => x.GrossWeight) + editData.Sum(x => x.GrossWeight);
				}

				var userRecord = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == data.LadingNo && x.IsDelete == false);
				if (userRecord != null)
				{
					userRecord.WarehouseRNo = data.WarehouseRNo;
					userRecord.HubNo = data.HubNo;
					userRecord.WarehouseRDate = data.WarehouseRDate;
					userRecord.TransferNo = data.TransferNo;
					userRecord.LadingDate = data.LadingDate;
					userRecord.OrderNo = data.OrderNo;
					userRecord.SendCustNo = data.SendCustNo;
					userRecord.SendCHName = data.SendCHName;
					userRecord.SendPhone = data.SendPhone;
					userRecord.SendBy = data.SendBy;
					userRecord.SendCustAddr = data.SendCustAddr;
					userRecord.RecPhone = data.RecPhone;
					userRecord.RecCustEName1 = data.RecCustEName1;
					userRecord.RecCustEName2 = data.RecCustEName2;
					userRecord.RecBy = data.RecBy;
					userRecord.RecCompany = data.RecCompany;
					userRecord.RecCustCHName = data.RecCustCHName;
					userRecord.RecCustENAddr1 = data.RecCustENAddr1;
					userRecord.RecCustENAddr2 = data.RecCustENAddr2;
					userRecord.RecChAddr = data.RecChAddr;
					userRecord.RecInvNo = data.RecInvNo;
					userRecord.RecCity = data.RecCity;
					userRecord.RecState = data.RecState;
					userRecord.RecCountry = data.RecCountry;
					userRecord.RecPostDist = data.RecPostDist;
					userRecord.SectorNo = data.SectorNo;
					userRecord.SectorName = data.SectorName;
					userRecord.SStatNo = data.SStatNo;
					userRecord.SStatName = data.SStatName;
					userRecord.AStatName = data.AStatName;
					userRecord.AStatNo = data.AStatNo;
					userRecord.DestNo = data.DestNo;
					userRecord.CName = data.CName;
					userRecord.Type = data.Type;
					userRecord.ProductNo = data.ProductNo;
					userRecord.ProductName = data.ProductName;
					userRecord.PiecesNo = data.PiecesNo;
					userRecord.Qty = data.Qty;
					userRecord.Weight = data.Weight;
					userRecord.Volume = data.Volume;
					userRecord.Cost = data.Cost;
					userRecord.CostCurrency = data.CostCurrency;
					userRecord.CcNo = data.CcNo;
					userRecord.PayCustNo = data.PayCustNo;
					userRecord.PayCustCHName = data.PayCustCHName;
					userRecord.Freight = data.Freight;
					userRecord.FreightCurrency = data.FreightCurrency;
					userRecord.FuelCosts = data.FuelCosts;
					userRecord.ToPayment = data.ToPayment;
					userRecord.ToPaymentCurrency = data.ToPaymentCurrency;
					userRecord.AgentPay = data.AgentPay;
					userRecord.AgentPayCurrency = data.AgentPayCurrency;
					userRecord.ProdIdPay = data.ProdIdPay;
					userRecord.CustomsPay = data.CustomsPay;
					userRecord.InsurancePay = data.InsurancePay;
					userRecord.OtherPayTax = data.OtherPayTax;
					userRecord.OtherPayNoTax = data.OtherPayNoTax;
					userRecord.Length = data.Length;
					userRecord.Width = data.Width;
					userRecord.Height = data.Height;
					userRecord.Total = data.Total;
					userRecord.TotalCurrency = data.TotalCurrency;
					userRecord.Remark = data.Remark;
					userRecord.Remark2 = data.Remark2;
					userRecord.ShdetNo = data.ShdetNo;

					//以下系統自填
					userRecord.UpdateTime = DateTime.Now;
					userRecord.UpdateBy = User.Identity.Name;
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
		#endregion

		#region 子表(主)Edit
		[Authorize]
		public ActionResult EditDetail2(DeclCust_Main data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var editData = db.DeclCust_Main.FirstOrDefault(x => x.LadingNo == x.LadingNo && x.sNo == data.sNo);
				if (editData != null)
				{
					editData.LadingNo = data.LadingNo;
					editData.sNo = data.sNo;
					editData.CustType = data.CustType;
					editData.Flight = data.Flight;
					editData.LadNo = data.LadNo;
					editData.BagNo = data.BagNo;
					editData.CleCusCode = data.CleCusCode;
					editData.CusCoode = data.CusCoode;
					editData.ProductNo = data.ProductNo;
					editData.ProductName = data.ProductName;
					editData.ProductEName = data.ProductEName;
					editData.Country = data.Country;
					editData.Type = data.Type;
					editData.HSNo = data.HSNo;
					editData.Qty = data.Qty;
					editData.Unit = data.Unit;
					editData.GrossWeight = data.GrossWeight;
					editData.Weight = data.Weight;
					editData.Price = data.Price;
					editData.Total = data.Total;
					editData.Currency = data.Currency;
					editData.Pcs = data.Pcs;
					editData.PcsNo = data.PcsNo;
					editData.Remark = data.Remark;

					//以下系統自填
					editData.UpdateBy = User.Identity.Name;
					editData.UpdateTime = DateTime.Now;
					db.Entry(editData).State = EntityState.Modified;
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
		#endregion

		#region 子表(次)Edit
		[Authorize]
		public ActionResult EditDetail3(DeclCust_Sub data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var editData = db.DeclCust_Sub.Where(x => x.LadingNo == x.LadingNo && x.sNo == data.sNo).FirstOrDefault();
				if (editData != null)
				{

					editData.LadingNo = data.LadingNo;
					editData.sNo = data.sNo;
					editData.BagNo = data.BagNo;
					editData.CleCusCode = data.CleCusCode;
					editData.ProductName = data.ProductName;
					editData.Type = data.Type;
					editData.Qty = data.Qty;
					editData.GrossWeight = data.GrossWeight;
					editData.Weight = data.Weight;
					editData.Price = data.Price;
					editData.Remark = data.Remark;

					//以下系統自填
					editData.UpdateBy = User.Identity.Name;
					editData.UpdateTime = DateTime.Now;
					db.Entry(editData).State = EntityState.Modified;
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

		#endregion

		#region 主表Delete
		[Authorize]
		public ActionResult Delete(Bill_Lading data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var deleteData = db.Bill_Lading.FirstOrDefault(x => x.LadingNo == data.LadingNo);
				var dtl2 = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo);
				var dtl1 = db.DeclCust_Sub.Where(x => x.LadingNo == data.LadingNo);

				foreach (var d in dtl1)
				{
					d.IsDelete = true;
					d.DeletedBy = User.Identity.Name;
					d.DeletedTime = DateTime.Now;
					db.Entry(d).State = EntityState.Modified;
				}

				foreach (var d in dtl2)
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
					try
					{
						db.Entry(deleteData).State = EntityState.Modified;
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
		#endregion

		#region 子表(主)Delete
		[Authorize]
		public ActionResult DeleteDetail2(DeclCust_Main data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var deleteData = db.DeclCust_Main.Where(x => x.LadingNo == data.LadingNo && x.sNo == data.sNo).FirstOrDefault();
				if (deleteData != null)
				{
					//以下系統自填
					deleteData.DeletedTime = DateTime.Now;
					deleteData.DeletedBy = User.Identity.Name;
					deleteData.IsDelete = true;
					db.Entry(deleteData).State = EntityState.Modified;

					try
					{
						db.Entry(deleteData).State = EntityState.Modified;
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
		#endregion

		#region 子表(次)Delete
		[Authorize]
		public ActionResult DeleteDetail3(DeclCust_Sub data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var deleteData = db.DeclCust_Sub.Where(x => x.LadingNo == data.LadingNo && x.sNo == data.sNo).FirstOrDefault();
				if (deleteData != null)
				{
					//以下系統自填
					deleteData.DeletedTime = DateTime.Now;
					deleteData.DeletedBy = User.Identity.Name;
					deleteData.IsDelete = true;
					db.Entry(deleteData).State = EntityState.Modified;

					try
					{
						db.Entry(deleteData).State = EntityState.Modified;
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
		#endregion

		[Authorize]
		public ActionResult SaveRec(ORG_Consin data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				var existedStatus = db.ORG_Consin.Any(x => x.IsDelete == false && x.Consinee == data.Consinee && x.Tel == data.Tel && x.Cnaddr == data.Cnaddr);
				if (!existedStatus)
				{
					var saveData = new ORG_Consin();
					saveData.Consinee = data.Consinee;
					saveData.Tel = data.Tel;
					saveData.ConsinComp = data.ConsinComp;
					saveData.Cnaddr = data.Cnaddr;
					saveData.UnifyNo = data.UnifyNo;
					saveData.Country = data.Country;
					saveData.Zip = data.Zip;

					//以下系統自填
					saveData.CreatedDate = DateTime.Now;
					saveData.CreatedBy = User.Identity.Name;
					saveData.IsDelete = false;

					db.Entry(saveData).State = EntityState.Added;
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
					result.Message = "已存在相同收件人資料!";
					trans.Rollback();
				}
			}

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetLadingData(string param)
		{
			var data = db.Bill_Lading.Where(x => x.LadingNo_Type == param && x.IsDelete == false).FirstOrDefault();
			if (data == null)
				data = db.Bill_Lading.Where(x => x.TransferNo == param && x.IsDelete == false).FirstOrDefault();
			var result = new ResultHelper();

			result.Data = data;
			result.Ok = DataModifyResultType.Success;
			result.Message = "OK";

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
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

		public class SubCompare : IEqualityComparer<DeclCust_Sub>
		{
			bool IEqualityComparer<DeclCust_Sub>.Equals(DeclCust_Sub x, DeclCust_Sub y)
			{
				return (x.LadingNo == y.LadingNo && x.sNo == y.sNo);
			}

			int IEqualityComparer<DeclCust_Sub>.GetHashCode(DeclCust_Sub obj)
			{
				int hash = 17;
				hash = hash * 23 + (obj.sNo).GetHashCode();
				hash = hash * 23 + (obj.LadingNo).GetHashCode();
				return hash;
			}
		}

		#region 提單匯入
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Upload(HttpPostedFileBase upload)
		{
			var result = new ResultHelper();
			if (ModelState.IsValid)
			{
				if (upload != null && upload.ContentLength > 0)
				{
					Stream stream = upload.InputStream;
					ExcelDataReader.IExcelDataReader reader = null;
					if (upload.FileName.EndsWith(".xls") || upload.FileName.EndsWith(".xlsx"))
					{
						//取得登入者 站點 && 是否為台灣
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

						var billLadingData = new List<Bill_Lading>();
						var mainData = new List<DeclCust_Main>();
						var subData = new List<DeclCust_Sub>();

						if (upload.FileName.EndsWith(".xls"))
							reader = ExcelReaderFactory.CreateBinaryReader(stream);
						else if (upload.FileName.EndsWith(".xlsx"))
							reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
						for (int r = 0; r < reader.ResultsCount; r++)
						{
							var filedata = reader.AsDataSet().Tables[r].AsEnumerable().ToList();
							var Type = GetImportType(filedata[0].ItemArray[0].ToString() + filedata[0].ItemArray[1].ToString());
							var errorMesg = CheckData(filedata, Type);
							if (errorMesg == "")
							{
								using (var trans = db.Database.BeginTransaction())
								{
									switch (Type)
									{
										case "越南提單":
											for (int i = 1; i < filedata.Count(); i++)
											{
												var data = filedata[i].ItemArray;
												var destNo = data[7].ToString().Trim();
												var date = data[2].ToString().Split('/')[2] + "-" + data[2].ToString().Split('/')[1] + "-" + data[2].ToString().Split('/')[0];
												if (data[2].ToString() != "" && data[3].ToString() != "")
													billLadingData.Add(new Bill_Lading()
													{
														ImOrEx = statNo == data[5].ToString().Trim() ? "Im" : "Ex",
														LadingDate = Convert.ToDateTime(date),
														LadingNo = data[3].ToString().Trim(),
														SStatNo = data[5].ToString().Trim(),
														SStatName = data[6].ToString().Trim(),
														DestNo = data[7].ToString().Trim(),
														CName = db.ORG_Dest.Where(x => x.DestNo == destNo).Select(x => x.ChName).FirstOrDefault(),
														CcNo = data[8].ToString().Trim(),
														SendCHName = data[9].ToString().Trim(),
														RecCompany = data[10].ToString().Trim(),
														Type = data[11].ToString().Trim(),
														ProductNo = data[12].ToString().Trim(),
														ProductName = data[13].ToString().Trim(),
														PiecesNo = Convert.ToInt32(data[14].ToString().Trim()),
														Weight = data[15].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[15].ToString().Trim()),
														Volume = data[16].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[16].ToString().Trim()),
														Freight = data[17].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[17].ToString().Trim()),
														ToPaymentCurrency = data[18].ToString().Trim(),
														ToPayment = data[19].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[19].ToString().Trim()),
														AgentPayCurrency = data[20].ToString().Trim(),
														AgentPay = data[21].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[21].ToString().Trim()),
														SendCustAddr = data[22].ToString(),
														RecPhone = data[23].ToString().Trim(),
														RecBy = data[24].ToString(),
														RecChAddr = data[25].ToString(),
														Remark = data[26].ToString(),
														//以下系統自填
														Source = "Web",
														IsConfirm = true,
														IsCheck = false,
														IsReview = false,
														CreateTime = Convert.ToDateTime(date),
														CreateBy = User.Identity.Name,
														IsDelete = false,
													});

												if (isTw == true)
												{
													if (data[2].ToString() != "" && data[3].ToString() != "")
														mainData.Add(new DeclCust_Main()
														{
															sNo = 1,
															LadingNo = data[3].ToString().Trim(),
															BagNo = data[1].ToString().Trim(),
															ProductNo = data[12].ToString().Trim(),
															ProductName = data[13].ToString().Trim(),
															//以下系統自填
															CreateTime = DateTime.Now,
															CreateBy = User.Identity.Name,
															IsDelete = false,
														});
												}
												else
												{
													if (data[2].ToString() != "" && data[3].ToString() != "")
														subData.Add(new DeclCust_Sub()
														{
															sNo = 1,
															LadingNo = data[3].ToString().Trim(),
															BagNo = data[1].ToString().Trim(),
															ProductName = data[13].ToString().Trim(),
															//以下系統自填
															CreateTime = DateTime.Now,
															CreateBy = User.Identity.Name,
															IsDelete = false,
														});
												}
											}
											break;
										case "印尼提單":
											for (int i = 1; i < filedata.Count(); i++)
											{
												var data = filedata[i].ItemArray;
												var destNo = data[7].ToString().Trim();
												if (data[2].ToString() != "" && data[3].ToString() != "")
													billLadingData.Add(new Bill_Lading()
													{
														ImOrEx = statNo == data[5].ToString().Trim() ? "Im" : "Ex",
														LadingDate = Convert.ToDateTime(data[2].ToString()),
														LadingNo = data[3].ToString().Trim(),
														SStatNo = data[5].ToString().Trim(),
														SStatName = data[6].ToString().Trim(),
														DestNo = data[7].ToString().Trim(),
														CName = db.ORG_Dest.Where(x => x.DestNo == destNo).Select(x => x.ChName).FirstOrDefault(),
														CcNo = data[8].ToString().Trim(),
														SendCHName = data[9].ToString().Trim(),
														RecCompany = data[10].ToString().Trim(),
														Type = data[11].ToString().Trim(),
														ProductNo = data[12].ToString().Trim(),
														ProductName = data[13].ToString().Trim(),
														PiecesNo = Convert.ToInt32(data[14].ToString().Trim().Replace("PKG", "")),
														Weight = data[15].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[15].ToString().Trim()),
														Volume = data[16].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[16].ToString().Trim()),
														Freight = data[17].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[17].ToString().Trim()),
														FreightCurrency = data[18].ToString().Trim(),
														ToPayment = data[19].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[19].ToString().Trim()),
														ToPaymentCurrency = data[20].ToString().Trim(),
														SendCustAddr = data[22].ToString(),
														RecPhone = data[23].ToString().Trim(),
														RecBy = data[24].ToString(),
														RecChAddr = data[25].ToString(),
														Remark = data[26].ToString(),
														SendPhone = data[27].ToString(),
														//以下系統自填
														Source = "Web",
														IsConfirm = true,
														IsCheck = false,
														IsReview = false,
														CreateTime = Convert.ToDateTime(data[2].ToString()),
														CreateBy = User.Identity.Name,
														IsDelete = false,
													});

												if (isTw == true)
												{
													if (data[2].ToString() != "" && data[3].ToString() != "")
														mainData.Add(new DeclCust_Main()
														{
															sNo = 1,
															LadingNo = data[3].ToString().Trim(),
															BagNo = data[1].ToString().Trim(),
															ProductNo = data[12].ToString().Trim(),
															ProductName = data[13].ToString().Trim(),
															//以下系統自填
															CreateTime = DateTime.Now,
															CreateBy = User.Identity.Name,
															IsDelete = false,
														});
												}
												else
												{
													if (data[2].ToString() != "" && data[3].ToString() != "")
														subData.Add(new DeclCust_Sub()
														{
															sNo = 1,
															LadingNo = data[3].ToString().Trim(),
															BagNo = data[1].ToString().Trim(),
															ProductName = data[13].ToString().Trim(),
															//以下系統自填
															CreateTime = DateTime.Now,
															CreateBy = User.Identity.Name,
															IsDelete = false,
														});
												}
											}
											break;
										case "哲盟提單":
											for (int i = 1; i < filedata.Count(); i++)
											{
												var data = filedata[i].ItemArray;
												var destNo = data[7].ToString().Trim();
												var hubName = data[9].ToString().Trim() == "" ? "㊣" : data[9].ToString().Trim();
												var hub = db.ORG_Hub.Where(x => x.HubName == hubName).FirstOrDefault();
												if (data[3].ToString() != "" && data[4].ToString() != "")
													billLadingData.Add(new Bill_Lading()
													{
														ImOrEx = statNo == data[5].ToString().Trim() ? "Im" : "Ex",
														LadingDate = Convert.ToDateTime(data[3].ToString()),
														LadingNo = data[4].ToString().Trim(),
														SStatNo = data[5].ToString().Trim(),
														SStatName = data[6].ToString().Trim(),
														SendCustNo = data[7].ToString().Trim(),
														SendCHName = data[8].ToString().Trim(),
														HubNo = hub == null ? null : hub.HubNo,
														SendBy = data[10].ToString().Trim(),
														SendPhone = data[11].ToString(),
														SendCustAddr = data[12].ToString(),
														DestNo = data[13].ToString().Trim(),
														CName = data[14].ToString().Trim(),
														SectorNo = data[15].ToString().Trim(),
														RecBy = data[17].ToString(),
														RecPhone = data[18].ToString().Trim(),
														RecCompany = data[19].ToString().Trim(),
														RecChAddr = data[20].ToString(),
														PiecesNo = Convert.ToInt32(data[21].ToString().Trim()),
														Volume = data[22].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[22].ToString().Trim()),
														Weight = data[23].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[23].ToString().Trim()),
														Type = data[24].ToString().Trim(),
														CcNo = data[25].ToString().Trim(),
														Freight = data[26].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[26].ToString().Trim()),
														ToPayment = data[27].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[27].ToString().Trim()),
														ToPaymentCurrency = data[28].ToString().Trim(),
														AgentPay = data[29].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[29].ToString().Trim()),
														FuelCosts = data[30].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[30].ToString().Trim()),
														CustomsPay = data[31].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[31].ToString().Trim()),
														InsurancePay = data[32].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[32].ToString().Trim()),
														ProdIdPay = data[33].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[33].ToString().Trim()),
														OtherPayTax = data[34].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[33].ToString().Trim()),
														OtherPayNoTax = data[35].ToString().Trim() == "" ? 0 : Convert.ToDecimal(data[33].ToString().Trim()),
														AStatNo = data[37].ToString().Trim(),
														AStatName = data[38].ToString().Trim(),
														Remark = data[41].ToString(),
														WarehouseRNo = data[44].ToString().Trim(),
														WarehouseRDate = Convert.ToDateTime(data[45].ToString().Trim()),
														ProductNo = data[46].ToString().Trim(),
														ProductName = data[47].ToString().Trim(),

														//以下系統自填
														Source = "Web",
														IsConfirm = true,
														IsCheck = false,
														IsReview = false,
														CreateTime = Convert.ToDateTime(data[3].ToString()),
														CreateBy = User.Identity.Name,
														IsDelete = false,
													});

												if (isTw == true)
												{
													if (data[2].ToString() != "" && data[3].ToString() != "")
														mainData.Add(new DeclCust_Main()
														{
															sNo = 1,
															LadingNo = data[4].ToString().Trim(),
															BagNo = data[1].ToString().Trim(),
															ProductNo = data[46].ToString().Trim(),
															ProductName = data[47].ToString().Trim(),
															//以下系統自填
															CreateTime = DateTime.Now,
															CreateBy = User.Identity.Name,
															IsDelete = false,
														});
												}
												else
												{
													if (data[2].ToString() != "" && data[3].ToString() != "")
														subData.Add(new DeclCust_Sub()
														{
															sNo = 1,
															LadingNo = data[4].ToString().Trim(),
															BagNo = data[1].ToString().Trim(),
															ProductName = data[47].ToString().Trim(),
															//以下系統自填
															CreateTime = DateTime.Now,
															CreateBy = User.Identity.Name,
															IsDelete = false,
														});
												}
											}
											break;
									}
									//存入資料

									db.Bill_Lading.AddRange(billLadingData);
									if (isTw)
										db.DeclCust_Main.AddRange(mainData);
									else
										db.DeclCust_Sub.AddRange(subData);
									try
									{
										db.SaveChanges();
										trans.Commit();
										result.Ok = DataModifyResultType.Success;
										result.Message = "上傳成功！";
									}
									catch (Exception e)
									{
										trans.Rollback();
										result.Ok = DataModifyResultType.Faild;
										result.Message = e.Message;
									}
								}
							}
							else
							{
								result.Ok = DataModifyResultType.Faild;
								result.Message = errorMesg;
							}
						}

					}
					else
					{
						result.Ok = DataModifyResultType.Faild;
						result.Message = "檔案格式錯誤！";
					}
					reader.Close();
				}
				else
				{
					result.Ok = DataModifyResultType.Faild;
					result.Message = "尚未選擇檔案！";
					ModelState.Clear();
				}

			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		private string GetImportType(string param)
		{
			var Type = "";
			switch (param)
			{
				case "袋號":
					Type = "越南提單";
					break;
				case "NO":
					Type = "印尼提單";
					break;
				case "選擇檢核":
					Type = "哲盟提單";
					break;
				default:
					Type = "無對應格式";
					break;
			}
			return Type;
		}

		public string CheckData(List<DataRow> filedata, string Type)
		{
			string ErrorMsg = "";

			if (Type == "無對應格式")
			{
				ErrorMsg = "無對應格式";
			}
			else
			{
				var ladingNoData = new List<LadingNoData>();

				for (int i = 1; i < filedata.Count(); i++)
				{
					int position = 0;
					switch (Type)
					{
						case "越南提單":
						case "印尼提單":
							position = 3;
							break;
						case "哲盟提單":
							position = 4;
							break;
					}
					ladingNoData.Add(new LadingNoData()
					{
						No = i,
						LadingNo = filedata[i].ItemArray[position].ToString(),
					});
				}
				var GroupData = ladingNoData.GroupBy(x => x.LadingNo).ToList();
				if (GroupData.Count() != ladingNoData.Count())
				{
					foreach (var g in GroupData)
					{
						if (g.Count() > 1)
						{
							ErrorMsg = ErrorMsg + "第" + g.ToList()[0].No + "筆，提單號碼：" + g.ToList()[0].LadingNo + "<br>";
						}
					}
					ErrorMsg = "『檔案中』存在重覆提單號碼！請檢查！<br>" + ErrorMsg;
				}
				else
				{
					foreach (var l in ladingNoData)
					{
						if ((db.Bill_Lading.Where(x => x.LadingNo == l.LadingNo && x.IsDelete == false).FirstOrDefault()) != null)
						{
							ErrorMsg = ErrorMsg + "第" + l.No + "筆，提單號碼：" + l.LadingNo + "<br>";
						}
					}
					if (ErrorMsg != "")
						ErrorMsg = "『資料庫』存在重覆提單號碼！請檢查！<br>" + ErrorMsg;
				}
			}
			return ErrorMsg;
		}

		public partial class LadingNoData
		{
			public int No { set; get; }

			public string LadingNo { set; get; }
		}
		#endregion
	}
}
