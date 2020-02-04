using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
    public class VehicleController : Controller
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
			//頁面的抬頭
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "車輛資料";
			ViewBag.ControllerName = "Vehicle";
			ViewBag.FormPartialName = "_ElementInForm";
			ViewBag.FormCustomJsEdit = "$('#CarID').textbox('readonly')";
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
		public ActionResult Add(ORG_Vehicle data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			bool duplicated = db.ORG_Vehicle.Any(x => x.CarID == data.CarID);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複車輛代號";
			}
			else
			{
				ORG_Vehicle userRecord = new ORG_Vehicle();
				userRecord.CarID = data.CarID;
				userRecord.CarNO = data.CarNO;
				userRecord.CarKind = data.CarKind;
				userRecord.CarBrand = data.CarBrand;
				userRecord.CheckDT = data.CheckDT;
				userRecord.Oil = data.Oil;
				userRecord.LoadLimit = data.LoadLimit;
				userRecord.LoadSafety = data.LoadSafety;
				userRecord.ValueMax = data.ValueMax;
				userRecord.ValueSafe = data.ValueSafe;
				userRecord.BoxLength = data.BoxLength;
				userRecord.BoxWidth = data.BoxWidth;
				userRecord.BoxHeight = data.BoxHeight;
				userRecord.StopDT = data.StopDT;
				userRecord.ReMark = data.ReMark;

				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;
				try
				{
					db.ORG_Vehicle.Add(userRecord);
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
		public ActionResult Edit(ORG_Vehicle data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			ORG_Vehicle userRecord = db.ORG_Vehicle.FirstOrDefault(x => x.CarID == data.CarID);

			if (userRecord != null)
			{
				userRecord.CarID = data.CarID;
				userRecord.CarNO = data.CarNO;
				userRecord.CarKind = data.CarKind;
				userRecord.CarBrand = data.CarBrand;
				userRecord.CheckDT = data.CheckDT;
				userRecord.Oil = data.Oil;
				userRecord.LoadLimit = data.LoadLimit;
				userRecord.LoadSafety = data.LoadSafety;
				userRecord.ValueMax = data.ValueMax;
				userRecord.ValueSafe = data.ValueSafe;
				userRecord.BoxLength = data.BoxLength;
				userRecord.BoxWidth = data.BoxWidth;
				userRecord.BoxHeight = data.BoxHeight;
				userRecord.StopDT = data.StopDT;
				userRecord.ReMark = data.ReMark;

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
		public ActionResult Delete(ORG_Vehicle data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
			  this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			ORG_Vehicle userRecord = db.ORG_Vehicle.FirstOrDefault(x => x.CarID == data.CarID);

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
		public ActionResult GetGridJSON(ORG_Vehicle data, int page = 1, int rows = 40)
		{
			var car = from v in db.ORG_Vehicle.Where(x => x.IsDelete == false)
					  join u in db.SYS_User on v.CreatedBy equals u.Account into ps
					  from u in ps.DefaultIfEmpty()
					  select new 
                      {
						  CarID = v.CarID,
						  CarNO = v.CarNO,
						  CarKind = v.CarKind,
						  CarBrand = v.CarBrand,
						  CheckDT = v.CheckDT,
						  Oil = v.Oil,
						  LoadLimit = v.LoadLimit,
						  LoadSafety = v.LoadSafety,
						  ValueMax = v.ValueMax,
						  ValueSafe = v.ValueSafe,
						  BoxLength = v.BoxLength,
						  BoxWidth = v.BoxWidth,
						  BoxHeight = v.BoxHeight,
						  StopDT = v.StopDT,
						  ReMark = v.ReMark,
						  CreatedBy = u.UserName,
						  CreatedDate = v.CreatedDate,
						  UpdatedBy = v.UpdatedBy,
						  UpdatedDate = v.UpdatedDate,
						  DeletedBy = v.DeletedBy,
						  DeletedDate = v.DeletedDate,
						  IsDelete = v.IsDelete,
					  };
			if (data.CarID.IsNotEmpty())
				car = car.Where(x => x.CarID.Contains(data.CarID));
			if (data.CarNO.IsNotEmpty())
				car = car.Where(x => x.CarNO.Contains(data.CarNO));
			if (data.CarKind.IsNotEmpty())
				car = car.Where(x => x.CarKind.Contains(data.CarKind));
			if (data.CarBrand.IsNotEmpty())
				car = car.Where(x => x.CarBrand.Contains(data.CarBrand));

			int records = car.Count();
			car = car.OrderBy(x => x.CarID).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "OK",
				Records = records,
				Pages = page,
				Data = car,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}