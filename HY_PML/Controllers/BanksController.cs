using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class BanksController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;
		public ActionResult _ElementInForm()
		{
			return PartialView();
		}

		// GET: Currency
		[Authorize]
		public ActionResult Index()
		{
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "銀行資料";
			ViewBag.ControllerName = "Banks";
			ViewBag.FormPartialName = "_ElementInForm";

			//權限控管
			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		[Authorize]
		public ActionResult Add(ORG_Banks data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();
			var duplicated = db.ORG_Currency.Any(x => x.CurrencyNo == data.BankNo && x.IsDelete == false);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複的幣別代碼";
			}
			else
			{
				var userRecord = new ORG_Banks();
				userRecord.BankNo = data.BankNo;
				userRecord.BankName = data.BankName;
				userRecord.BankAcc = data.BankAcc;
				userRecord.Balance = data.Balance;

				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;

				try
				{
					db.ORG_Banks.Add(userRecord);
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
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Edit(ORG_Banks data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Banks.FirstOrDefault(x => x.ID == data.ID);

			if (userRecord != null)
			{
				userRecord.BankName = data.BankName;
				userRecord.BankAcc = data.BankAcc;
				userRecord.Balance = data.Balance;

				//以下系統自填
				userRecord.UpdatedDate = DateTime.Now;
				userRecord.UpdatedBy = User.Identity.Name;
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
		public ActionResult Delete(ORG_Banks data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Banks.FirstOrDefault(x => x.ID == data.ID);
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
		public ActionResult GetGridJSON(ORG_Banks data, int page = 1, int rows = 40)
		{
			var bank =
				from b in db.ORG_Banks.Where(x => x.IsDelete == false)
				join u in db.SYS_User on b.CreatedBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				select new
				{
					ID = b.ID,
					BankNo = b.BankNo,
					BankAcc = b.BankAcc,
					BankName = b.BankName,
					Balance = b.Balance,
					CreatedBy = u.UserName,
					CreatedDate = b.CreatedDate,
					UpdatedBy = b.UpdatedBy,
					UpdatedDate = b.UpdatedDate,
					DeletedBy = b.DeletedBy,
					DeletedDate = b.DeletedDate,
					IsDelete = b.IsDelete,

				};

			if (data.BankNo.IsNotEmpty())
				bank = bank.Where(x => x.BankNo.Contains(data.BankNo));
			if (data.BankName.IsNotEmpty())
				bank = bank.Where(x => x.BankName.Contains(data.BankName));
			if (data.BankAcc.IsNotEmpty())
				bank = bank.Where(x => x.BankAcc.Contains(data.BankAcc));

			int records = bank.Count();
			bank = bank.OrderBy(o => o.BankNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = bank,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}