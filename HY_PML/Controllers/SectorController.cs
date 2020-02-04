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
	public class SectorController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

		public ActionResult _ElementInForm()
		{
			return PartialView();
		}

		//GET: Currency
		[Authorize]
		public ActionResult Index()
		{
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "外務員資料";
			ViewBag.ControllerName = "Sector";
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
		public ActionResult getSelectionjqGrid_QuickAdd(string formId, string inputFields, string inputValues)
		{
			ViewBag.formId = formId;
			ViewBag.inputValues = inputValues.Split(',');
			ViewBag.inputFields = inputFields.Split(',');
			return View();
		}
		[Authorize]
		public ActionResult getSelectionjqGrid_Add(string formId, string inputFields, string inputValues)
		{
			ViewBag.formId = formId;
			ViewBag.inputValues = inputValues.Split(',');
			ViewBag.inputFields = inputFields.Split(',');
			return View();
		}

		[Authorize]
		public ActionResult Add(ORG_Sector data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();

			var duplicated = db.ORG_Sector.Any(x => x.SectorNo == data.SectorNo);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複的外務員代號";
			}
			else
			{
				ORG_Sector userRecord = new ORG_Sector();

				userRecord.SectorNo = data.SectorNo;
				userRecord.SectorName = data.SectorName;
				userRecord.PlateNO = data.PlateNO;
				userRecord.Phone = data.Phone;
				userRecord.StatNo = data.StatNo;
				userRecord.PickUpAreaNo = data.PickUpAreaNo;
				userRecord.PhonePrivate = data.PhonePrivate;
				userRecord.RecTime = data.RecTime;
				userRecord.EndTime = data.EndTime;
				userRecord.Latitude = data.Latitude;
				userRecord.Longitude = data.Longitude;
				userRecord.RecRange = data.RecRange;
				userRecord.TargetKM = data.TargetKM;
				userRecord.TargetNum = data.TargetNum;
				userRecord.IsLeave = data.IsLeave;
				userRecord.IsServer = data.IsServer;
				//以下系統自填
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;
				userRecord.IsDelete = false;

				try
				{
					db.ORG_Sector.Add(userRecord);
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
		public ActionResult Edit(ORG_Sector data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_Sector userRecord = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == data.SectorNo);

			if (userRecord != null)
			{
				userRecord.SectorName = data.SectorName;
				userRecord.PlateNO = data.PlateNO;
				userRecord.Phone = data.Phone;
				userRecord.StatNo = data.StatNo;
				userRecord.PickUpAreaNo = data.PickUpAreaNo;
				userRecord.PhonePrivate = data.PhonePrivate;
				userRecord.RecTime = data.RecTime;
				userRecord.EndTime = data.EndTime;
				userRecord.Latitude = data.Latitude;
				userRecord.Longitude = data.Longitude;
				userRecord.RecRange = data.RecRange;
				userRecord.TargetKM = data.TargetKM;
				userRecord.TargetNum = data.TargetNum;
				userRecord.IsLeave = data.IsLeave;
				userRecord.IsServer = data.IsServer;

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
		public ActionResult Delete(ORG_Sector data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_Sector userRecord = db.ORG_Sector.FirstOrDefault(x => x.SectorNo == data.SectorNo);
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
		public ActionResult GetGridJSON(ORG_Sector data, int page = 1, int rows = 40)
		{
			var sectors = from s in db.ORG_Sector.Where(x => x.IsDelete == false)
						  join v in db.ORG_Vehicle.Where(x => x.IsDelete == false)
						  on s.PlateNO equals v.CarNO into ps
						  from v in ps.DefaultIfEmpty()
						  join u in db.SYS_User.Where(x => x.IsDelete == false)
						  on s.CreatedBy equals u.Account into ps2
						  from u in ps2.DefaultIfEmpty()
						  select new
						  {
							  SectorNo = s.SectorNo,
							  SectorName = s.SectorName,
							  StatNo = s.StatNo,
							  PlateNO = s.PlateNO,
							  Phone = s.Phone,
							  PhonePrivate = s.PhonePrivate,
							  RecRange = s.RecRange,
							  RecTime = s.RecTime,
							  EndTime = s.EndTime,
							  Latitude = s.Latitude,
							  Longitude = s.Longitude,
							  TargetKM = s.TargetKM,
							  TargetNum = s.TargetNum,
							  IsServer = s.IsServer,
							  IsLeave = s.IsLeave,
							  IsDelete = s.IsDelete,
							  CreatedBy = u.UserName,
							  CreatedDate = s.CreatedDate,
							  UpdatedBy = s.UpdatedBy,
							  UpdatedDate = s.UpdatedDate,
							  DeletedBy = s.DeletedBy,
							  DeletedDate = s.DeletedDate,
							  CarID = v.CarID
						  };
			if (data.SectorNo.IsNotEmpty())
				sectors = sectors.Where(x => x.SectorNo.Contains(data.SectorNo));
			if (data.SectorName.IsNotEmpty())
				sectors = sectors.Where(x => x.SectorName.Contains(data.SectorName));
			if (data.PlateNO.IsNotEmpty())
				sectors = sectors.Where(x => x.PlateNO.Contains(data.PlateNO));
			if (data.Phone.IsNotEmpty())
				sectors = sectors.Where(x => x.Phone.Contains(data.Phone));
			if (data.StatNo.IsNotEmpty())
				sectors = sectors.Where(x => x.StatNo.Contains(data.StatNo));
			if (data.PhonePrivate.IsNotEmpty())
				sectors = sectors.Where(x => x.PhonePrivate.Contains(data.PhonePrivate));
			if (data.RecTime.IsNotEmpty())
				sectors = sectors.Where(x => x.RecTime.Contains(data.RecTime));
			if (data.EndTime.IsNotEmpty())
				sectors = sectors.Where(x => x.EndTime.Contains(data.EndTime));
			if (data.Latitude.IsNotEmpty())
				sectors = sectors.Where(x => x.Latitude.Contains(data.Latitude));
			if (data.Longitude.IsNotEmpty())
				sectors = sectors.Where(x => x.Longitude.Contains(data.Longitude));
			if ((data.IsLeave == false && Request["IsLeave"] == "false") || data.IsLeave == true)
				sectors = sectors.Where(x => x.IsLeave == data.IsLeave);
			if ((data.IsServer == false && Request["IsServer"] == "false") || data.IsServer == true)
				sectors = sectors.Where(x => x.IsServer == data.IsServer);

			var dataList = new List<ORG_Sector>();
			foreach (var i in sectors)
			{
				var tData = new ORG_Sector()
				{
					SectorNo = i.SectorNo,
					SectorName = i.SectorName,
					StatNo = i.StatNo,
					PlateNO = i.PlateNO,
					Phone = i.Phone,
					PhonePrivate = i.PhonePrivate,
					PickUpAreaNo = String.Join(",", db.ORG_PickUpArea.Where(x => x.SectorNo == i.SectorNo && x.IsDelete == false).Select(x => x.PickUpAreaNo).ToArray()),
					RecRange = i.RecRange,
					RecTime = i.RecTime,
					EndTime = i.EndTime,
					Latitude = i.Latitude,
					Longitude = i.Longitude,
					TargetKM = i.TargetKM,
					TargetNum = i.TargetNum,
					IsServer = i.IsServer,
					IsLeave = i.IsLeave,
					IsDelete = i.IsDelete,
					CreatedBy = i.CreatedBy,
					CreatedDate = i.CreatedDate,
					UpdatedBy = i.UpdatedBy,
					UpdatedDate = i.UpdatedDate,
					DeletedBy = i.DeletedBy,
					DeletedDate = i.DeletedDate,
					CarID = i.CarID,
				};
				dataList.Add(tData);
			}
			var sector = dataList as IEnumerable<ORG_Sector>;
			if (data.PickUpAreaNo.IsNotEmpty())
				sector = sector.Where(x => x.PickUpAreaNo.Contains(data.PickUpAreaNo));
			int records = sector.Count();
			sector = sector.OrderBy(x => x.SectorNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = sector,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON3(ORG_Sector data, int page = 1, int rows = 40, string date = "", string time = "")
		{
			DateTime RedyDateTime = (date != "" && time != "") ? DateTime.Parse(date + " " + time) : DateTime.Now;
			var sector = from s in db.ORG_Sector.Where(x => x.IsDelete == false)
						 join v in db.ORG_Vehicle.Where(x => x.IsDelete == false)
						 on s.PlateNO equals v.CarNO into ps
						 from v in ps.DefaultIfEmpty()
						 join a in db.ORG_SectorAbsent.Where(x => x.IsDelete == false && (DbFunctions.TruncateTime(x.StartDT) <= DbFunctions.TruncateTime(RedyDateTime)) && (DbFunctions.TruncateTime(x.EndDT) >= DbFunctions.TruncateTime(RedyDateTime)))
						on s.SectorNo equals a.SectorNo into ps2
						 from a in ps2.DefaultIfEmpty()
						 //join p in db.ORG_PickUpArea.Where(x => x.IsDelete == false)
						 //on s.SectorNo equals p.SectorNo into ps3
						 //from p in ps3.DefaultIfEmpty()
						 where s.IsDelete == false && s.IsServer
						 select new
						 {
							 s.SectorNo,
							 s.SectorName,
							 s.StatNo,
							 s.PlateNO,
							 s.Phone,
							 s.PhonePrivate,
							 //p.PickUpAreaNo,
							 s.RecRange,
							 s.RecTime,
							 s.EndTime,
							 s.Latitude,
							 s.Longitude,
							 s.TargetKM,
							 s.TargetNum,
							 s.IsServer,
							 s.IsLeave,
							 s.IsDelete,
							 s.CreatedBy,
							 s.CreatedDate,
							 s.UpdatedBy,
							 s.UpdatedDate,
							 s.DeletedBy,
							 s.DeletedDate,
							 CarID = v.CarID,
							 IsOff = a == null ? false : true
						 };
			if (data.SectorNo.IsNotEmpty())
				sector = sector.Where(x => x.SectorNo.Contains(data.SectorNo));
			if (data.SectorName.IsNotEmpty())
				sector = sector.Where(x => x.SectorName.Contains(data.SectorName));
			if (data.PlateNO.IsNotEmpty())
				sector = sector.Where(x => x.PlateNO.Contains(data.PlateNO));
			if (data.Phone.IsNotEmpty())
				sector = sector.Where(x => x.Phone.Contains(data.Phone));
			if (data.StatNo.IsNotEmpty())
				sector = sector.Where(x => x.StatNo.Contains(data.StatNo));
			//if (data.PickUpAreaNo.IsNotEmpty())
			//	sector = sector.Where(x => x.PickUpAreaNo.Contains(data.PickUpAreaNo));
			if (data.PhonePrivate.IsNotEmpty())
				sector = sector.Where(x => x.PhonePrivate.Contains(data.PhonePrivate));
			if (data.RecTime.IsNotEmpty())
				sector = sector.Where(x => x.RecTime.Contains(data.RecTime));
			if (data.EndTime.IsNotEmpty())
				sector = sector.Where(x => x.EndTime.Contains(data.EndTime));
			if (data.Latitude.IsNotEmpty())
				sector = sector.Where(x => x.Latitude.Contains(data.Latitude));
			if (data.Longitude.IsNotEmpty())
				sector = sector.Where(x => x.Longitude.Contains(data.Longitude));
			if ((data.IsLeave == false && Request["IsLeave"] == "false") || data.IsLeave == true)
				sector = sector.Where(x => x.IsLeave == data.IsLeave);
			if ((data.IsServer == false && Request["IsServer"] == "false") || data.IsServer == true)
				sector = sector.Where(x => x.IsServer == data.IsServer);
			if (data.IsOff != null)
				sector = sector.Where(x => x.IsOff == data.IsOff);

			int records = sector.Count();
			sector = sector.OrderBy(x => x.SectorNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = sector,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}
