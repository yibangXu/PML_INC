using HY_PML.helper;
using HY_PML.Models;
//using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class FeelistController : Controller
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
			//頁面的抬頭
			ViewBag.Title = "費用列表";
			ViewBag.ControllerName = "Feelist";
			ViewBag.FormPartialName = "_ElementInForm";
			//權限控管
			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(),
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		[Authorize]
		public ActionResult Add(ORG_Feelist data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				  this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			bool duplicated = db.ORG_Feelist.Any(x => x.FeeNo == data.FeeNo);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複費用代號";
			}
			else
			{
				ORG_Feelist userRecord = new ORG_Feelist();
				userRecord.FeeNo = data.FeeNo;
				userRecord.FeeName = data.FeeName;
				userRecord.Remark = data.Remark;
				userRecord.Way = data.Way;
				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;

				try
				{
					db.ORG_Feelist.Add(userRecord);
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
		public ActionResult Edit(ORG_Feelist data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			ORG_Feelist userRecord = db.ORG_Feelist.FirstOrDefault(x => x.ID == data.ID);


			if (userRecord != null)
			{
				userRecord.FeeNo = data.FeeNo;
				userRecord.FeeName = data.FeeName;
				userRecord.Remark = data.Remark;
				userRecord.Way = data.Way;

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
			return Content(JsonConvert.SerializeObject(result)
				, WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}


		[Authorize]
		public ActionResult Delete(ORG_Feelist data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			ResultHelper result = new ResultHelper();
			ORG_Feelist userRecord = db.ORG_Feelist.FirstOrDefault(x => x.ID == data.ID);
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
			return Content(JsonConvert.SerializeObject(result),
				WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetGridJSON(ORG_Feelist data, int page = 1, int rows = 40, string target = "")
		{
			var fee = from f in db.ORG_Feelist.Where(x => x.IsDelete == false)
					  join u in db.SYS_User on f.CreatedBy equals u.Account into ps
					  from u in ps.DefaultIfEmpty()
					  select new
					  {
						  ID = f.ID,
						  FeeNo = f.FeeNo,
						  FeeName = f.FeeName,
						  Way = f.Way,
						  Remark = f.Remark,
						  CreatedBy = u.UserName,
						  CreatedDate = f.CreatedDate,
						  UpdatedBy = f.UpdatedBy,
						  UpdatedDate = f.UpdatedDate,
						  DeletedBy = f.DeletedBy,
						  DeletedDate = f.DeletedDate,
						  IsDelete = f.IsDelete,
					  };
			if (data.FeeNo.IsNotEmpty())
				fee = fee.Where(x => x.FeeNo.Contains(data.FeeNo));
			if (data.FeeName.IsNotEmpty())
				fee = fee.Where(x => x.FeeName.Contains(data.FeeName));
			if ((data.Way == false && Request["Way"] == "false") || data.Way == true)
				fee = fee.Where(x => x.Way == data.Way);

			int records = fee.Count();
			fee = fee.OrderBy(x => x.FeeNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Records = records,
				Pages = page,
				Data = fee,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};

			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}