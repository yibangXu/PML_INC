using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class ProductController : Controller
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
			ViewBag.Title = "物品資料";
			ViewBag.ControllerName = "Product";
			ViewBag.FormPartialName = "_ElementInForm";

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
		public ActionResult Add(ORG_Product data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			bool duplicated = db.ORG_Product.Any(x => x.ProductNo == data.ProductNo);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複的物品代碼";
			}
			else
			{
				ORG_Product userRecord = new ORG_Product();
				userRecord.ProductNo = data.ProductNo;
				userRecord.ProductName = data.ProductName;
				userRecord.ProductEName = data.ProductEName;
				userRecord.ProductEName2 = data.ProductEName2;
				userRecord.Price = data.Price;
				userRecord.Unit = data.Unit;
				userRecord.TaxWay = data.TaxWay;
				userRecord.Country = data.Country;
				userRecord.Hsno = data.Hsno;

				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;

				try
				{
					db.ORG_Product.Add(userRecord);
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
		public ActionResult Edit(ORG_Product data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			ORG_Product userRecord = db.ORG_Product
				.FirstOrDefault(x => x.ID == data.ID);

			if (userRecord != null)
			{
				userRecord.ProductNo = data.ProductNo;
				userRecord.ProductName = data.ProductName;
				userRecord.ProductEName = data.ProductEName;
				userRecord.ProductEName2 = data.ProductEName2;
				userRecord.Price = data.Price;
				userRecord.Unit = data.Unit;
				userRecord.TaxWay = data.TaxWay;
				userRecord.Country = data.Country;
				userRecord.Hsno = data.Hsno;

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
		public ActionResult Delete(ORG_Product data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			ORG_Product userRecord = db.ORG_Product.FirstOrDefault(x => x.ID == data.ID);
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
		public ActionResult GetGridJSON(ORG_Product data, int page = 1, int rows = 40)
		{
			var product = from p in db.ORG_Product.Where(x => x.IsDelete == false)
						  join u in db.SYS_User
							on p.CreatedBy equals u.Account into ps
						  from u in ps.DefaultIfEmpty()
						  select new
						  {

							  ID = p.ID,
							  ProductNo = p.ProductNo,
							  ProductName = p.ProductName,
							  ProductEName = p.ProductEName,
							  ProductEName2 = p.ProductEName2,
							  Price = p.Price,
							  Unit = p.Unit,
							  TaxWay = p.TaxWay,
							  Country = p.Country,
							  Hsno = p.Hsno,
							  CreatedDate = p.CreatedDate,
							  CreatedBy = u.UserName,
							  UpdatedDate = p.UpdatedDate,
							  UpdatedBy = p.UpdatedBy,
							  DeletedDate = p.DeletedDate,
							  DeletedBy = p.DeletedBy,
							  IsDelete = p.IsDelete,
						  };

			if (data.ProductNo.IsNotEmpty())
				product = product.Where(x => x.ProductNo.Contains(data.ProductNo));
			if (data.ProductName.IsNotEmpty())
				product = product.Where(x => x.ProductName.Contains(data.ProductName));
			if (data.ProductEName.IsNotEmpty())
				product = product.Where(x => x.ProductEName.Contains(data.ProductEName));
			if (data.ProductEName2.IsNotEmpty())
				product = product.Where(x => x.ProductEName2.Contains(data.ProductEName2));
			if (data.Unit.IsNotEmpty())
				product = product.Where(x => x.Unit.Contains(data.Unit));
			if (data.TaxWay.IsNotEmpty())
				product = product.Where(x => x.TaxWay.Contains(data.TaxWay));
			if (data.Country.IsNotEmpty())
				product = product.Where(x => x.Country.Contains(data.Country));
			if (data.Hsno.IsNotEmpty())
				product = product.Where(x => x.Hsno.Contains(data.Hsno));

			int records = product.Count();
			product = product.OrderBy(x => x.ProductNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "OK",
				Records = records,
				Pages = page,
				Data = product,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}