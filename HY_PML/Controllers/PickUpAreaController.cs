using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	//Todo Alan 子表功能
	public class PickUpAreaController : Controller
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
			ViewBag.Title = "調度區域資料";
			ViewBag.ControllerName = "PickUpArea";

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
		public ActionResult Add(ORG_PickUpArea data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var duplicated = db.ORG_PickUpArea.Any(x => x.PickUpAreaNo == data.PickUpAreaNo && x.IsDelete == false);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複的調度區域代碼";
			}
			else
			{
				using (var trans = db.Database.BeginTransaction())
				{
					ORG_PickUpArea userRecord = new ORG_PickUpArea();
					userRecord.PickUpAreaNo = data.PickUpAreaNo;
					userRecord.PickUpAreaName = data.PickUpAreaName;
					userRecord.DateEnd = data.DateEnd;
					userRecord.PickUpAreaAddress = data.PickUpAreaAddress;
					userRecord.Remark = data.Remark;
					userRecord.StatID = data.StatID;
					userRecord.IsServer = data.IsServer;
					userRecord.SectorNo = data.SectorNo;

					//address , 副表
					if (Request["CityAreaRoadRow"] != null)
					{
						string[] addrData = (Request["CityAreaRoadRow"]).ToString().Split(',');
						foreach (var item in addrData)
						{
							if (item.IsNotEmpty())
							{
								var code5 = item.Substring(1, 5);
								var code7 = db.ORG_PostCode.Where(x => x.Code5 == item.Substring(1, 5) && x.CityName + x.AreaName + x.RoadName + x.RowName == item.Substring(7)).Select(x => x.Code7).FirstOrDefault();
								var cityAreaRoadRow = item.Substring(7);
								var existed = db.ORG_PickUpAreaAddress.Where(x => x.Code5 == code5 && x.Code7 == code7 && x.CityAreaRoadRow == cityAreaRoadRow && x.IsDelete == false).FirstOrDefault();
								if (existed != null)
								{
									if (existed.PickUpAreaNo != userRecord.PickUpAreaNo)
									{
										existed.IsDelete = true;
										existed.DeletedBy = User.Identity.Name;
										existed.DeletedDate = DateTime.Now;

										db.Entry(existed).State = EntityState.Modified;
									}
								}
								var pAddr = new ORG_PickUpAreaAddress();
								pAddr.Code5 = item.Substring(1, 5);
								pAddr.Code7 = db.ORG_PickUpAreaAddress.Where(x => x.Code5 == item.Substring(1, 5) && x.CityAreaRoadRow == item.Substring(7)).Select(x => x.Code7).FirstOrDefault();
								pAddr.CityAreaRoadRow = item.Substring(7);
								pAddr.PickUpAreaNo = userRecord.PickUpAreaNo;
								pAddr.CreatedDate = DateTime.Now;
								pAddr.CreatedBy = User.Identity.Name;
								pAddr.IsDelete = false;
								db.ORG_PickUpAreaAddress.Add(pAddr);
							}
						}
					}

					//以下系統自填
					userRecord.CreatedDate = DateTime.Now;
					userRecord.CreatedBy = User.Identity.Name;
					userRecord.IsDelete = false;
					db.ORG_PickUpArea.Add(userRecord);

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
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Edit(ORG_PickUpArea data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_PickUpArea userRecord = db.ORG_PickUpArea.FirstOrDefault(x => x.ID == data.ID);

			if (userRecord != null)
			{
				using (var trans = db.Database.BeginTransaction())
				{
					userRecord.PickUpAreaName = data.PickUpAreaName;
					userRecord.DateEnd = data.DateEnd;
					userRecord.PickUpAreaAddress = data.PickUpAreaAddress;
					userRecord.Remark = data.Remark;
					userRecord.StatID = data.StatID;
					userRecord.IsServer = data.IsServer;
					userRecord.SectorNo = data.SectorNo;

					//address , 副表，先刪再加
					var addr = from adr in db.ORG_PickUpAreaAddress
							   where adr.PickUpAreaNo == userRecord.PickUpAreaNo
							   select adr;
					db.ORG_PickUpAreaAddress.RemoveRange(addr);
					db.SaveChanges();

					if (Request["CityAreaRoadRow"] != null)
					{
						string[] addrData = (Request["CityAreaRoadRow"]).ToString().Split(',');
						foreach (var item in addrData)
						{
							if (item.IsNotEmpty())
							{
								var code5 = item.Substring(1, 5);
								var code7 = db.ORG_PostCode.Where(x => x.Code5 == item.Substring(1, 5) && x.CityName + x.AreaName + x.RoadName + x.RowName == item.Substring(7)).Select(x => x.Code7).FirstOrDefault();
								var cityAreaRoadRow = item.Substring(7);
								var existed = db.ORG_PickUpAreaAddress.Where(x => x.Code5 == code5 && x.Code7 == code7 && x.CityAreaRoadRow == cityAreaRoadRow && x.IsDelete == false).FirstOrDefault();
								if (existed != null)
								{
									if (existed.PickUpAreaNo != userRecord.PickUpAreaNo)
									{
										existed.IsDelete = true;
										existed.DeletedBy = User.Identity.Name;
										existed.DeletedDate = DateTime.Now;

										db.Entry(existed).State = EntityState.Modified;
									}
								}
								var pAddr = new ORG_PickUpAreaAddress();
								pAddr.Code5 = code5;
								pAddr.Code7 = code7;
								pAddr.CityAreaRoadRow = cityAreaRoadRow;
								pAddr.PickUpAreaNo = userRecord.PickUpAreaNo;
								pAddr.CreatedDate = DateTime.Now;
								pAddr.CreatedBy = User.Identity.Name;
								pAddr.IsDelete = false;
								db.ORG_PickUpAreaAddress.Add(pAddr);
							}
						}
					}

					//以下系統自填
					userRecord.UpdatedDate = DateTime.Now;
					userRecord.UpdatedBy = User.Identity.Name;
					db.Entry(userRecord).State = EntityState.Modified;
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
			}
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "找不到資料!";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Delete(ORG_PickUpArea data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			ORG_PickUpArea userRecord = db.ORG_PickUpArea.FirstOrDefault(x => x.ID == data.ID);

			if (userRecord != null)
			{
				var addr = from adr in db.ORG_PickUpAreaAddress
						   where adr.PickUpAreaNo == userRecord.PickUpAreaNo
						   select adr;
				db.ORG_PickUpAreaAddress.RemoveRange(addr);

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
		public ActionResult CheckExisted(ORG_PickUpArea data)
		{
			var result = new ResultHelper();
			string Msg = "";
			//address , 副表
			if (Request["CityAreaRoadRow"] != null)
			{
				string[] code5 = (Request["CityAreaRoadRow"]).ToString().Split(',');
				foreach (var item in code5)
				{
					if (item.IsNotEmpty())
					{
						var existed = db.ORG_PickUpAreaAddress.Where(x => x.Code5 == item.Substring(1, 5) && x.CityAreaRoadRow == item.Substring(7) && x.IsDelete == false).FirstOrDefault();
						if (existed != null)
						{
							if (existed.PickUpAreaNo != data.PickUpAreaNo)
							{
								Msg = Msg + existed.PickUpAreaNo + ":" + item + "<br>";
							}
						}
					}
				}
			}
			if (Msg == "")
			{
				result.Ok = DataModifyResultType.Success;
				result.Message = "OK";
			}
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = Msg;
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		protected class AreaRow
		{
			public string PickUpAreaNo { set; get; }
			public string Code5 { set; get; }
			public string CityAreaRoadRow { set; get; }
		}

		[Authorize]
		public ActionResult GetGridJSON(ORG_PickUpArea data, int page = 1, int rows = 40)
		{
			var paGroup = from pa in db.ORG_PickUpAreaAddress.Where(x => x.IsDelete == false)
						  group pa by pa.PickUpAreaNo;

			var paData = new List<AreaRow>();
			foreach (var i in paGroup)
			{
				var tData = new AreaRow()
				{
					PickUpAreaNo = i.Key,
					Code5 = String.Join(",", i.Select(x => x.Code5).ToArray()),
					CityAreaRoadRow = String.Join(",", i.Select(x => "(" + x.Code5 + ")" + x.CityAreaRoadRow).ToArray())
				};
				paData.Add(tData);
			}

			var areaData = from a in db.ORG_PickUpArea
						   join s in db.ORG_Stat on a.StatID equals s.ID into ps
						   from s in ps.DefaultIfEmpty()
						   join u in db.SYS_User on a.CreatedBy equals u.Account into ps2
						   from u in ps2.DefaultIfEmpty()
						   join ss in db.ORG_Sector on a.SectorNo equals ss.SectorNo into ps3
						   from ss in ps3.DefaultIfEmpty()
						   where a.IsDelete == false
						   select new { a, s, u, ss };

			var dataList = new List<ORG_PickUpArea>();
			foreach (var i in areaData)
			{
				var pa = paData.FirstOrDefault(x => x.PickUpAreaNo == i.a.PickUpAreaNo);
				var tData = new ORG_PickUpArea()
				{
					ID = i.a.ID,
					PickUpAreaNo = i.a.PickUpAreaNo,
					PickUpAreaName = i.a.PickUpAreaName,
					PickUpAreaAddress = i.a.PickUpAreaAddress,
					DateEnd = i.a.DateEnd,
					StatID = i.a.StatID,
					StatNo = i.s == null ? null : i.s.StatNo,
					IsServer = i.a.IsServer,
					Remark = i.a.Remark,
					CreatedBy = i.u.UserName,
					CreatedDate = i.a.CreatedDate,
					UpdatedBy = i.a.UpdatedBy,
					UpdatedDate = i.a.UpdatedDate,
					DeletedBy = i.a.DeletedBy,
					DeletedDate = i.a.DeletedDate,
					IsDelete = i.a.IsDelete,
					SectorNo = i.a.SectorNo,
					SectorName = i.ss == null ? null : i.ss.SectorName,
					Code5 = pa == null ? null : pa.Code5,
					CityAreaRoadRow = pa == null ? null : pa.CityAreaRoadRow
				};
				dataList.Add(tData);
			}
			var area = dataList as IEnumerable<ORG_PickUpArea>;

			if (data.PickUpAreaNo.IsNotEmpty())
				area = area.Where(x => x.PickUpAreaNo.Contains(data.PickUpAreaNo));
			if (data.PickUpAreaName.IsNotEmpty())
				area = area.Where(x => x.PickUpAreaName.Contains(data.PickUpAreaName));
			if (data.CityAreaRoadRow.IsNotEmpty())
				area = area.Where(x => x.CityAreaRoadRow != null && x.CityAreaRoadRow.Contains(data.CityAreaRoadRow));
			if ((data.IsServer == false && Request["IsServer"]?.ToLower() == "false") || data.IsServer == true)
				area = area.Where(x => x.IsServer == data.IsServer);
			if (data.StatNo.IsNotEmpty())
				area = area.Where(x => x.StatID == Convert.ToInt32(data.StatNo));
			if (data.SectorName.IsNotEmpty())
				area = area.Where(x => x.SectorName == data.SectorName);

			var records = dataList.Count();
			area = area.OrderBy(o => o.PickUpAreaNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = area,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON_Sector(ORG_PickUpArea data, int page = 1, int rows = 40)
		{
			var dataList = new List<ORG_PickUpArea>();
			var pData = from p in db.ORG_PickUpArea.Where(x => x.IsDelete == false && x.SectorNo != null)
						join s in db.ORG_Sector
						on p.SectorNo equals s.SectorNo into ps
						from s in ps.DefaultIfEmpty()
						select new
						{
							SectorNo = s.SectorNo,
							SectorName = s.SectorName,
						};

			var GpData = from g in pData group g by new { g.SectorName };

			foreach (var g in GpData)
			{
				var gpData = g.Last();
				var temp = new ORG_PickUpArea
				{
					SectorNo = gpData.SectorNo,
					SectorName = gpData.SectorName,
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
		public ActionResult BatchAccessFromFile()
		{
			string[] lines;
			string[] stringSeparators = new string[] { "\r\n" };
			DataTable dtTarget = new DataTable();
			dtTarget.Columns.Add("PostCode");
			dtTarget.Columns.Add("AddrFull");
			dtTarget.Columns.Add("PickUpAreaNo");
			DataTable dtFailRecord = new DataTable();
			dtFailRecord.Columns.Add("PostCode");
			dtFailRecord.Columns.Add("AddrFull");
			dtFailRecord.Columns.Add("PickUpAreaNo");
			DateTime Stamp = DateTime.Now;
			int countOK = 0;
			int countFail = 0;

			if (WebSiteHelper.CurrentUserID.ToUpper().Contains("ADMIN") != true)
			{
				return Content("Not Allowed...");
			}

			var pickAreaNoMaster = from pm in db.ORG_PickUpArea
								   where pm.IsDelete == false
								   select pm;
			List<string> pMNoList = new List<string>();
			foreach (var itemPM in pickAreaNoMaster)
			{
				pMNoList.Add(itemPM.PickUpAreaNo);
			}

			try
			{
				using (StreamReader sr = new StreamReader(Server.MapPath(Url.Content("~/MediaTemp/TainanPickUpArea.csv")), System.Text.Encoding.Default))
				{
					lines = sr.ReadToEnd().Split(stringSeparators, StringSplitOptions.None);
				}
				foreach (var item in lines)
				{
					string[] cellItems = item.Split(',');
					if (cellItems.Length >= 6)
					{
						if (cellItems[5] != "非服務區")
						{
							string[] pickUpInfo = cellItems[5].Split(' ');
							string addrFull = cellItems[1] + cellItems[2] + cellItems[3] + cellItems[4];
							dtTarget.Rows.Add(new string[] { cellItems[0], addrFull, pickUpInfo[0] });
						}
					}
				}
			}
			catch (Exception)
			{ }
			for (int i = 0; i < dtTarget.Rows.Count - 1; i++)
			{
				if (pMNoList.Contains(dtTarget.Rows[i]["PickUpAreaNo"].ToString()))
				{
					//不用加主檔
				}
				else
				{
					//加主檔
					var pMaster = new ORG_PickUpArea();
					pMaster.PickUpAreaNo = dtTarget.Rows[i]["PickUpAreaNo"].ToString();
					pMaster.PickUpAreaName = dtTarget.Rows[i]["PickUpAreaNo"].ToString();
					pMaster.CreatedDate = Stamp;
					pMaster.IsDelete = false;
					try
					{
						db.ORG_PickUpArea.Add(pMaster);
						db.SaveChanges();
						pMNoList.Add(dtTarget.Rows[i]["PickUpAreaNo"].ToString());
					}
					catch (Exception)
					{ }
				}

				var pDetail = new ORG_PickUpAreaAddress();
				pDetail.PickUpAreaNo = dtTarget.Rows[i]["PickUpAreaNo"].ToString();
				pDetail.Code5 = dtTarget.Rows[i]["PostCode"].ToString();
				pDetail.CityAreaRoadRow = dtTarget.Rows[i]["AddrFull"].ToString();
				pDetail.CreatedDate = Stamp;
				pDetail.IsDelete = false;
				try
				{

					db.ORG_PickUpAreaAddress.Add(pDetail);
					db.SaveChanges();
					countOK += 1;
				}
				catch (Exception)
				{
					countFail += 1;
					dtFailRecord.ImportRow(dtTarget.Rows[i]);
				}
			}
			BatchReturnInfo result = new BatchReturnInfo(countOK, countFail, dtFailRecord);
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}

	public class BatchReturnInfo
	{
		public int OK;
		public int Fail;
		public DataTable FailRecord;
		public BatchReturnInfo(int ok, int fail, DataTable dtTarget)
		{
			this.OK = ok;
			this.Fail = fail;
			this.FailRecord = dtTarget;
		}
	}
}