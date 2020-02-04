using HUSite;
using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HY_PML.Controllers
{
	public class TransportationSController : Controller
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
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "派件資料";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "TransportationS";
			ViewBag.FormCustomJsNew =
				$"$('.pull-left').css('display', 'inline');" +
				$"$('#dtlGridAdd').css('display', 'inline');" +
				$"$('#IsCheck').css('display', 'inline');" +
				$"$('#IsCheckLabel').css('display', 'inline');" +
				$"$('#LadingNo').textbox('readonly', false);" +
				$"$('#isAdd').textbox('setValue', 'true');";
			ViewBag.FormCustomJsEdit =
				$"$('.pull-left').css('display', 'none');" +
				$"$('#dtlGridAdd').css('display', 'none');" +
				$"$('#IsCheck').css('display', 'none');" +
				$"$('#IsCheckLabel').css('display', 'none');" +
				$"$('#LadingNo').textbox('readonly', true);" +
				$"$('#isAdd').textbox('setValue', 'false');";
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
		public ActionResult Add(TransportationS data, string[] dtl)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var ErrorMsg = new StringBuilder();

			//取得登入者 站點資訊
			string cId = WebSiteHelper.CurrentUserID;
			int dbId = int.Parse(WebSiteHelper.CurrentUserDBID);
			var userInfo = from s in db.SYS_User
						   where s.IsDelete == false && s.ID == dbId
						   select s;
			string statNo = userInfo.First().StatNo;
			var statData = db.ORG_Stat.Where(x => x.StatNo == statNo).FirstOrDefault();
			string statName = statData.StatName;

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				if (dtl != null && dtl.Count() > 0)
				{
					var datalist = dtl[0].Replace("[", "").Replace("]", "").Replace("},{", "}㊣{").Split('㊣');
					var newData = new List<TransportationS>();
					JavaScriptSerializer js = new JavaScriptSerializer();
					foreach (var d in datalist)
					{
						newData.Add(js.Deserialize<TransportationS>(d));
					}
					newData = newData.Select((x, Index) => new TransportationS()
					{
						TransportationNo = db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).FirstOrDefault() == null ? "T-" + x.LadingNo :
						"T-" + db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).Select(b => b.LadingNo).FirstOrDefault(),
						LadingNo = db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).FirstOrDefault() == null ? x.LadingNo :
						 db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).Select(b => b.LadingNo).FirstOrDefault(),
						SStatNo = statNo,
						SStatName = statName,
						SendTime = x.SendTime,
						SSectorNo = x.SSectorNo,
						SSectorName = x.SSectorName,
						PlateNo = x.PlateNo,
						SendPcs = x.SendPcs,
						LadingPcs = x.LadingPcs,
						Remark = x.Remark,
						CreateTime = DateTime.Now,
						CreateBy = User.Identity.Name,
						IsDelete = false,
						IsCheck = x.IsCheck,
					}).ToList();
					db.TransportationS.AddRange(newData);
					db.SaveChanges();
					foreach (var n in newData)
					{
						var ladingData = db.Bill_Lading.Where(x => x.LadingNo == n.LadingNo).FirstOrDefault();
						if (ladingData != null)
						{
							ladingData.Status = "派件";
							ladingData.StatusTime = n.SendTime;
							db.Entry(ladingData).State = EntityState.Modified;
							db.SaveChanges();
						}
					}
				}
				try
				{
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

		[Authorize]
		public ActionResult Edit(TransportationS data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var editData = db.TransportationS.FirstOrDefault(x => x.TransportationNo == data.TransportationNo && x.SendTime == data.SendTime);

			if (editData != null)
			{

				editData.SSectorNo = data.SSectorNo;
				editData.SSectorName = data.SSectorName;
				editData.PlateNo = data.PlateNo;
				editData.SendPcs = data.SendPcs;
				editData.Remark = data.Remark;
				//以下系統自填
				editData.UpdateTime = DateTime.Now;
				editData.UpdateBy = User.Identity.Name;
				try
				{
					db.Entry(editData).State = EntityState.Modified;
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
		public ActionResult Delete(TransportationS data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.TransportationS.FirstOrDefault(x => x.TransportationNo == data.TransportationNo && x.SendTime == data.SendTime);
			if (deletedData != null)
			{
				//以下系統自填
				deletedData.DeletedTime = DateTime.Now;
				deletedData.DeletedBy = User.Identity.Name;
				deletedData.IsDelete = true;
				try
				{
					db.Entry(deletedData).State = EntityState.Modified;
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
		public ActionResult GetGridJSON(TransportationS data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var transportationS =
				from s in db.TransportationS.Where(x => x.IsDelete == false)
				join u in db.SYS_User on s.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				join bl in db.Bill_Lading on s.LadingNo equals bl.LadingNo into ps2
				from bl in ps2.DefaultIfEmpty()
				select new
				{
					TransportationNo = s.TransportationNo,
					LadingNo = s.LadingNo,
					LadingNo_Type = bl != null ? bl.LadingNo_Type : s.LadingNo,
					SStatNo = s.SStatNo,
					SStatName = s.SStatName,
					SendTime = s.SendTime,
					SSectorNo = s.SSectorNo,
					SSectorName = s.SSectorName,
					PlateNo = s.PlateNo,
					SendPcs = s.SendPcs,
					LadingPcs = s.LadingPcs,
					Remark = s.Remark,
					CreateBy = u.UserName,
					CreateTime = s.CreateTime,
					UpdateBy = s.UpdateBy,
					UpdateTime = s.UpdateTime,
					DeletedBy = s.DeletedBy,
					DeletedTime = s.DeletedTime,
					IsDelete = s.IsDelete,
					IsCheck = s.IsCheck,
				};

			if (data.LadingNo != null)
				transportationS = transportationS.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type != null)
				transportationS = transportationS.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.SSectorNo != null)
				transportationS = transportationS.Where(x => x.SSectorNo.Contains(data.SSectorNo));
			if (data.SSectorName != null)
				transportationS = transportationS.Where(x => x.SSectorName.Contains(data.SSectorName));
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				transportationS = transportationS.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

            var records = transportationS.Count();
            transportationS = transportationS.OrderBy(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);

			var dataList = new List<TransportationS>();
			var Index = 0;
			foreach (var t in transportationS)
			{
				Index = ++Index;
				var tData = new TransportationS()
				{
					Index = Index,
					TransportationNo = t.TransportationNo,
					LadingNo_Type = t.LadingNo_Type,
					LadingNo = t.LadingNo,
					SStatNo = t.SStatNo,
					SStatName = t.SStatName,
					SendTime = t.SendTime,
					SSectorNo = t.SSectorNo,
					SSectorName = t.SSectorName,
					PlateNo = t.PlateNo,
					SendPcs = t.SendPcs,
					LadingPcs = t.LadingPcs,
					Remark = t.Remark,
					CreateBy = t.CreateBy,
					CreateTime = t.CreateTime,
					UpdateBy = t.UpdateBy,
					UpdateTime = t.UpdateTime,
					DeletedBy = t.DeletedBy,
					DeletedTime = t.DeletedTime,
					IsDelete = t.IsDelete,
					IsCheck = t.IsCheck,

				};
				dataList.Add(tData);
			}
			var transportationSData = dataList as IEnumerable<TransportationS>;

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "OK",
				Records = records,
				Pages = page,
				Data = transportationSData,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult CheckData(IEnumerable<TransportationS> data)
		{
			var errorMsg = new StringBuilder();
			var error1 = new List<string>();
			var error2 = new List<string>();
			if (data.First().isAdd)
			{
				var groupData = data.GroupBy(g => g.LadingNo).Select(sl => new TransportationS
				{
					LadingNo = sl.First().LadingNo,
					SendPcs = sl.Sum(s => s.SendPcs),
				});
				foreach (var g in groupData)
				{
					var ladingData = db.Bill_Lading.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).FirstOrDefault();

					var ArrivalData = db.TransportationA.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).ToArray();
					var SendData = db.TransportationS.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).ToArray();
					if (ladingData == null)
						error1.Add(g.LadingNo);
					else
					{
						if (SendData.Sum(s => s.SendPcs) > 0)
						{
							if (SendData.Sum(s => s.SendPcs) + g.SendPcs > ArrivalData.Sum(s => s.ArrivalPcs))
								error2.Add(g.LadingNo);
						}
						else
						{
							if (g.SendPcs > ArrivalData.Sum(s => s.ArrivalPcs))
								error2.Add(g.LadingNo);
						}
					}
				}
			}
			else
			{
				var editData = data.First();
				var originData = db.TransportationS.Where(x => x.LadingNo == editData.LadingNo && x.SendTime == editData.SendTime).FirstOrDefault();
				var ArrivalData = db.TransportationA.Where(x => x.LadingNo == editData.LadingNo && x.IsDelete == false).ToArray();
				var SendData = db.TransportationS.Where(x => x.LadingNo == editData.LadingNo && x.IsDelete == false).ToArray();
				if (SendData.Sum(s => s.SendPcs) - originData.SendPcs + editData.SendPcs > ArrivalData.Sum(s => s.ArrivalPcs))
					error2.Add(editData.LadingNo);

			}
			if (error1.Count > 0)
				errorMsg.Append("【提單單號】不存在：").Append("<br>").Append(String.Join("，", error1));
			if (error2.Count > 0)
			{
				if (error1.Count > 0)
					errorMsg.Append("<br>");
				errorMsg.Append("派件總數，超過到件總數：").Append("<br>").Append(String.Join("，", error2));
			}
			var result = new ResultHelper();
			if (errorMsg.Length > 0)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = errorMsg.ToString();
			}
			else
			{
				result.Ok = DataModifyResultType.Success;
				result.Message = "OK";
			}
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		public FileResult GetListCSV(TransportationS filter, DateTime? start_date = null, DateTime? end_date = null)
		{
			var transportationS =
				from s in db.TransportationS.Where(x => x.IsDelete == false)
				join u in db.SYS_User on s.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				join bl in db.Bill_Lading on s.LadingNo equals bl.LadingNo into ps2
				from bl in ps2.DefaultIfEmpty()
				select new
				{
					TransportationNo = s.TransportationNo,
					LadingNo = s.LadingNo,
					LadingNo_Type = bl != null ? bl.LadingNo_Type : s.LadingNo,
					SStatNo = s.SStatNo,
					SStatName = s.SStatName,
					SendTime = s.SendTime,
					SSectorNo = s.SSectorNo,
					SSectorName = s.SSectorName,
					PlateNo = s.PlateNo,
					SendPcs = s.SendPcs,
					LadingPcs = s.LadingPcs,
					Remark = s.Remark,
					CreateBy = u.UserName,
					CreateTime = s.CreateTime,
					UpdateBy = s.UpdateBy,
					UpdateTime = s.UpdateTime,
					DeletedBy = s.DeletedBy,
					DeletedTime = s.DeletedTime,
					IsDelete = s.IsDelete,
					IsCheck = s.IsCheck,
				};

			if (filter.LadingNo != null)
				transportationS = transportationS.Where(x => x.LadingNo.Contains(filter.LadingNo));
			if (filter.LadingNo_Type != null)
				transportationS = transportationS.Where(x => x.LadingNo_Type.Contains(filter.LadingNo_Type));
			if (filter.SSectorNo != null)
				transportationS = transportationS.Where(x => x.SSectorNo.Contains(filter.SSectorNo));
			if (filter.SSectorName != null)
				transportationS = transportationS.Where(x => x.SSectorName.Contains(filter.SSectorName));
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				transportationS = transportationS.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			var records = transportationS.Count();

			var dataList = new List<TransportationS>();
			var Index = 0;
			foreach (var t in transportationS)
			{
				Index = ++Index;
				var tData = new TransportationS()
				{
					Index = Index,
					TransportationNo = t.TransportationNo,
					LadingNo_Type = t.LadingNo_Type,
					LadingNo = t.LadingNo,
					SStatNo = t.SStatNo,
					SStatName = t.SStatName,
					SendTime = t.SendTime,
					SSectorNo = t.SSectorNo,
					SSectorName = t.SSectorName,
					PlateNo = t.PlateNo,
					SendPcs = t.SendPcs,
					LadingPcs = t.LadingPcs,
					Remark = t.Remark,
					CreateBy = t.CreateBy,
					CreateTime = t.CreateTime,
					UpdateBy = t.UpdateBy,
					UpdateTime = t.UpdateTime,
					DeletedBy = t.DeletedBy,
					DeletedTime = t.DeletedTime,
					IsDelete = t.IsDelete,
					IsCheck = t.IsCheck,

				};
				dataList.Add(tData);
			}
			var transportationSData = dataList as IEnumerable<TransportationS>;
			var Time = DateTime.Now.ToString("yyyyMMdd");
			var content = System.Text.Encoding.UTF8.GetBytes(CSVExporter.ExportCSVContent(transportationSData));
			return File(content, "text/csv", "派件資料_" + Time + ".csv");
		}
	}
}