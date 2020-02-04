using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class ConsinController : Controller
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
			ViewBag.Title = "收件人資料";
			ViewBag.ControllerName = "Consin";
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
		public ActionResult getSelectionjqGrid2(string formId, string inputFields, string inputValues)
		{
			ViewBag.formId = formId;
			ViewBag.inputValues = inputValues.Split(',');
			ViewBag.inputFields = inputFields.Split(',');
			return View();
		}


		[Authorize]
		public ActionResult Add(ORG_Consin data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var userRecord = new ORG_Consin();
			userRecord.ConsinNo = data.ConsinNo;
			userRecord.CustNo = data.CustNo;
			userRecord.ConsinComp = data.ConsinComp;
			userRecord.Enconsin1 = data.Enconsin1;
			userRecord.Enconsin2 = data.Enconsin2;
			userRecord.Enaddr1 = data.Enaddr1;
			userRecord.Enaddr2 = data.Enaddr2;
			userRecord.Enaddr3 = data.Enaddr3;
			userRecord.City = data.City;
			userRecord.State = data.State;
			userRecord.Country = data.Country;
			userRecord.Zip = data.Zip;
			userRecord.Tel = data.Tel;
			userRecord.MPhone = data.MPhone;
			userRecord.Consinee = data.Consinee;
			userRecord.UnifyNo = data.UnifyNo;
			userRecord.Code5 = data.Code5;
			userRecord.Add_1 = data.Add_1;
			userRecord.Add_2 = data.Add_2;
			userRecord.Add_3 = data.Add_3;
			userRecord.Add_4 = data.Add_4;
			userRecord.Add_5 = data.Add_5;
			userRecord.Add_6 = data.Add_6;
			userRecord.Cnaddr = data.Cnaddr;
			//以下系統自填
			userRecord.CreatedDate = DateTime.Now;
			userRecord.CreatedBy = User.Identity.Name;
			userRecord.IsDelete = false;

			var result = new ResultHelper();
			try
			{
				db.ORG_Consin.Add(userRecord);
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
		public ActionResult Edit(ORG_Consin data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Consin.FirstOrDefault(x => x.ID == data.ID);

			if (userRecord != null)
			{
				userRecord.CustNo = data.CustNo;
				userRecord.ConsinComp = data.ConsinComp;
				userRecord.Enconsin1 = data.Enconsin1;
				userRecord.Enconsin2 = data.Enconsin2;
				userRecord.Enaddr1 = data.Enaddr1;
				userRecord.Enaddr2 = data.Enaddr2;
				userRecord.Enaddr3 = data.Enaddr3;
				userRecord.City = data.City;
				userRecord.State = data.State;
				userRecord.Country = data.Country;
				userRecord.Zip = data.Zip;
				userRecord.Tel = data.Tel;
				userRecord.MPhone = data.MPhone;
				userRecord.Consinee = data.Consinee;
				userRecord.UnifyNo = data.UnifyNo;
				userRecord.Code5 = data.Code5;
				userRecord.Add_1 = data.Add_1;
				userRecord.Add_2 = data.Add_2;
				userRecord.Add_3 = data.Add_3;
				userRecord.Add_4 = data.Add_4;
				userRecord.Add_5 = data.Add_5;
				userRecord.Add_6 = data.Add_6;
				userRecord.Cnaddr = data.Cnaddr;

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
		public ActionResult Delete(ORG_Consin data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.ORG_Consin.FirstOrDefault(x => x.ID == data.ID);
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
		public ActionResult GetGridJSON(ORG_Consin data, int page = 1, int rows = 40)
		{
			var consin =
				from con in db.ORG_Consin
				join cust in db.ORG_Cust
				on con.CustNo equals cust.CustNo into ps
				from cust in ps.DefaultIfEmpty()
				join u in db.SYS_User
				on con.CreatedBy equals u.Account into ps2
				from u in ps2.DefaultIfEmpty()
				where con.IsDelete == false
				select new
				{
					ID = con.ID,
					ConsinNo = con.ConsinNo,
					CustNo = con.CustNo,
					CustCName = cust != null ? cust.CustCName : null,
					ConsinComp = con.ConsinComp,
					Enconsin1 = con.Enconsin1,
					Enconsin2 = con.Enconsin2,
					Enaddr1 = con.Enaddr1,
					Enaddr2 = con.Enaddr2,
					Enaddr3 = con.Enaddr3,
					Cnconsin = con.Cnconsin,
					Cnaddr = con.Cnaddr,
					City = con.City,
					State = con.State,
					Country = con.Country,
					Zip = con.Zip,
					Tel = con.Tel,
					MPhone = con.MPhone,
					Consinee = con.Consinee,
					UnifyNo = con.UnifyNo,
					Code5 = con.Code5,
					Add_1 = con.Add_1,
					Add_2 = con.Add_2,
					Add_3 = con.Add_3,
					Add_4 = con.Add_4,
					Add_5 = con.Add_5,
					Add_6 = con.Add_6,
					CreatedBy = u.UserName,
					CreatedDate = con.CreatedDate,
					UpdatedBy = con.UpdatedBy,
					UpdatedDate = con.UpdatedDate,
					DeletedBy = con.DeletedBy,
					DeletedDate = con.DeletedDate,
					IsDelete = con.IsDelete,
				};

			if (data.ConsinNo.IsNotEmpty())
				consin = consin.Where(x => x.ConsinNo.Contains(data.ConsinNo));
			if (data.CustNo.IsNotEmpty())
				consin = consin.Where(x => x.CustNo.Contains(data.CustNo));
			if (data.CustCName.IsNotEmpty())
				consin = consin.Where(x => x.CustCName.Contains(data.CustCName));
			if (data.ConsinComp.IsNotEmpty())
				consin = consin.Where(x => x.ConsinComp.Contains(data.ConsinComp));
			if (data.Enconsin1.IsNotEmpty())
				consin = consin.Where(x => x.Enconsin1.Contains(data.Enconsin1));
			if (data.Enconsin2.IsNotEmpty())
				consin = consin.Where(x => x.Enconsin2.Contains(data.Enconsin2));
			if (data.Enaddr1.IsNotEmpty())
				consin = consin.Where(x => x.Enaddr1.Contains(data.Enaddr1));
			if (data.Enaddr2.IsNotEmpty())
				consin = consin.Where(x => x.Enaddr2.Contains(data.Enaddr2));
			if (data.Enaddr3.IsNotEmpty())
				consin = consin.Where(x => x.Enaddr3.Contains(data.Enaddr3));
			if (data.City.IsNotEmpty())
				consin = consin.Where(x => x.City.Contains(data.City));
			if (data.State.IsNotEmpty())
				consin = consin.Where(x => x.State.Contains(data.State));
			if (data.Country.IsNotEmpty())
				consin = consin.Where(x => x.Country.Contains(data.Country));
			if (data.Zip.IsNotEmpty())
				consin = consin.Where(x => x.Zip.Contains(data.Zip));
			if (data.Tel.IsNotEmpty())
				consin = consin.Where(x => x.Tel.Contains(data.Tel));
			if (data.Consinee.IsNotEmpty())
				consin = consin.Where(x => x.Consinee.Contains(data.Consinee));
			if (data.UnifyNo.IsNotEmpty())
				consin = consin.Where(x => x.UnifyNo.Contains(data.UnifyNo));
			if (data.Code5.IsNotEmpty())
				consin = consin.Where(x => x.Code5.Contains(data.Code5));
			if (data.Phone.IsNotEmpty())
				consin = consin.Where(x => x.Tel== data.Phone);
			if (data.Cnaddr.IsNotEmpty())
				consin = consin.Where(x => x.Cnaddr.Contains(data.Cnaddr));

			var records = consin.Count();
			consin = consin.OrderBy(o => o.ConsinNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "OK",
				Records = records,
				Pages = page,
				Data = consin,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}