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
	public class TransportationRController : Controller
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
			ViewBag.Title = "收件資料";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "TransportationR";
			ViewBag.FormCustomJsNew =
				$"$('.pull-left').css('display', 'inline');" +
				$"$('#dtlGridAdd').css('display', 'inline');" +
				$"$('#edit-btn').css('display', 'inline');" +
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
			if (!WebSiteHelper.IsPermissioned("Export", this.ControllerContext.RouteData.Values["controller"].ToString()))
			{
				ViewBag.Export = "false";
			}
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
		public ActionResult Add(TransportationR data, string[] dtl)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

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
					var newData = new List<TransportationR>();
					JavaScriptSerializer js = new JavaScriptSerializer();
					foreach (var d in datalist)
					{
						newData.Add(js.Deserialize<TransportationR>(d));
					}
					newData = newData.Select((x, Index) => new TransportationR()
					{
						TransportationNo = db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).FirstOrDefault() == null ? "T-" + x.LadingNo :
						"T-" + db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).Select(b => b.LadingNo).FirstOrDefault(),
						LadingNo = db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).FirstOrDefault() == null ? x.LadingNo :
						 db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).Select(b => b.LadingNo).FirstOrDefault(),
						ReceiveTime = x.ReceiveTime,
						RStatNo = statNo,
						RStatName = statName,
						RSectorNo = x.RSectorNo,
						RSectorName = x.RSectorName,
						ReceivePcs = x.ReceivePcs,
						CreateTime = DateTime.Now,
						CreateBy = User.Identity.Name,
						IsDelete = false,
						IsCheck = x.IsCheck,
					}).ToList();
					db.TransportationR.AddRange(newData);
					db.SaveChanges();
					foreach (var n in newData)
					{
						var ladingData = db.Bill_Lading.Where(x => x.LadingNo == n.LadingNo).FirstOrDefault();
						if (ladingData != null)
						{
							ladingData.Status = "收件";
							ladingData.StatusTime = n.ReceiveTime;
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
		public ActionResult Edit(TransportationR data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var editData = db.TransportationR.FirstOrDefault(x => x.TransportationNo == data.TransportationNo && x.ReceiveTime == data.ReceiveTime);

			if (editData != null)
			{
				editData.RSectorNo = data.RSectorNo;
				editData.RSectorName = data.RSectorName;
				editData.ReceivePcs = data.ReceivePcs;

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
		public ActionResult Delete(TransportationR data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.TransportationR.FirstOrDefault(x => x.TransportationNo == data.TransportationNo && x.ReceiveTime == data.ReceiveTime);
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
		public ActionResult GetGridJSON(TransportationR data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var transportationR =
				from r in db.TransportationR.Where(x => x.IsDelete == false)
				join u in db.SYS_User on r.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				join bl in db.Bill_Lading on r.LadingNo equals bl.LadingNo into ps2
				from bl in ps2.DefaultIfEmpty()
				select new
				{
					TransportationNo = r.TransportationNo,
					LadingNo = r.LadingNo,
					LadingNo_Type = bl != null ? bl.LadingNo_Type : r.LadingNo,
					RStatNo = r.RStatNo,
					RStatName = r.RStatName,
					ReceiveTime = r.ReceiveTime,
					RSectorNo = r.RSectorNo,
					RSectorName = r.RSectorName,
					ReceivePcs = r.ReceivePcs,
					CreateBy = u.UserName,
					CreateTime = r.CreateTime,
					UpdateBy = r.UpdateBy,
					UpdateTime = r.UpdateTime,
					DeletedBy = r.DeletedBy,
					DeletedTime = r.DeletedTime,
					IsDelete = r.IsDelete,
					IsCheck = r.IsCheck,
				};

			if (data.LadingNo != null)
				transportationR = transportationR.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type != null)
				transportationR = transportationR.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.RSectorNo != null)
				transportationR = transportationR.Where(x => x.RSectorNo.Contains(data.RSectorNo));
			if (data.RSectorName != null)
				transportationR = transportationR.Where(x => x.RSectorName.Contains(data.RSectorName));
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				transportationR = transportationR.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			var records = transportationR.Count();
			transportationR = transportationR.OrderBy(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);

			var dataList = new List<TransportationR>();
			var Index = 0;
			foreach (var t in transportationR)
			{
				Index = ++Index;
				var tData = new TransportationR()
				{
					Index = Index,
					TransportationNo = t.TransportationNo,
					LadingNo = t.LadingNo,
					LadingNo_Type = t.LadingNo_Type,
					RStatNo = t.RStatNo,
					RStatName = t.RStatName,
					ReceiveTime = t.ReceiveTime,
					RSectorNo = t.RSectorNo,
					RSectorName = t.RSectorName,
					ReceivePcs = t.ReceivePcs,
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
			var transportationRData = dataList as IEnumerable<TransportationR>;

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "OK",
				Records = records,
				Pages = page,
				Data = transportationRData,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetTransportationRData(string LadingNo)
		{
			var transportationR =
			from r in db.TransportationR.Where(x => x.IsDelete == false)
			join u in db.SYS_User on r.CreateBy equals u.Account into ps
			from u in ps.DefaultIfEmpty()
			join bl in db.Bill_Lading on r.LadingNo equals bl.LadingNo into ps2
			from bl in ps2.DefaultIfEmpty()
			select new
			{
				TransportationNo = r.TransportationNo,
				LadingNo = r.LadingNo,
				LadingNo_Type = bl != null ? bl.LadingNo_Type : r.LadingNo,
				RStatNo = r.RStatNo,
				RStatName = r.RStatName,
				ReceiveTime = r.ReceiveTime,
				RSectorNo = r.RSectorNo,
				RSectorName = r.RSectorName,
				ReceivePcs = r.ReceivePcs,
				CreateBy = u.UserName,
				CreateTime = r.CreateTime,
				UpdateBy = r.UpdateBy,
				UpdateTime = r.UpdateTime,
				DeletedBy = r.DeletedBy,
				DeletedTime = r.DeletedTime,
				IsDelete = r.IsDelete,
				IsCheck = r.IsCheck,
			};
			var data = transportationR.Where(x => x.LadingNo_Type == LadingNo && x.IsDelete == false).FirstOrDefault();
			var result = new ResultHelper();
			if (data != null)
			{
				result.Data = data;
				result.Ok = DataModifyResultType.Success;
				result.Message = "OK";
			}
			else
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "查無資料";
			}

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult CheckData(IEnumerable<TransportationR> data)
		{
			var errorMsg = new StringBuilder();
			var error1 = new List<string>();
			var error2 = new List<string>();
			if (data.First().isAdd)
			{
				var groupData = data.GroupBy(g => g.LadingNo).Select(sl => new TransportationR
				{
					LadingNo = sl.First().LadingNo,
					ReceivePcs = sl.Sum(s => s.ReceivePcs),
				});
				foreach (var g in groupData)
				{
					var ladingData = db.Bill_Lading.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).FirstOrDefault();
					var ReceiveData = db.TransportationR.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).ToArray();
					if (ladingData == null)
						error1.Add(g.LadingNo);
					else
					{
						if (ReceiveData.Sum(s => s.ReceivePcs) > 0)
						{
							if (ReceiveData.Sum(s => s.ReceivePcs) + g.ReceivePcs > ladingData.PiecesNo)
								error2.Add(g.LadingNo);
						}
						else
						{
							if (g.ReceivePcs > ladingData.PiecesNo)
								error2.Add(g.LadingNo);
						}
					}
				}
			}
			else
			{
				var editData = data.First();
				var originData = db.TransportationR.Where(x => x.LadingNo == editData.LadingNo && x.ReceiveTime == editData.ReceiveTime).FirstOrDefault();
				var ladingData = db.Bill_Lading.Where(x => x.LadingNo == editData.LadingNo && x.IsDelete == false).FirstOrDefault();
				var ReceiveData = db.TransportationR.Where(x => x.LadingNo == editData.LadingNo && x.IsDelete == false).ToArray();
				if (ReceiveData.Sum(s => s.ReceivePcs) - originData.ReceivePcs + editData.ReceivePcs > ladingData.PiecesNo)
					error2.Add(editData.LadingNo);

			}
			if (error1.Count > 0)
				errorMsg.Append("【提單單號】不存在：").Append("<br>").Append(String.Join("，", error1));
			if (error2.Count > 0)
			{
				if (error1.Count > 0)
					errorMsg.Append("<br>");
				errorMsg.Append("收件總數，超過提單件數：").Append("<br>").Append(String.Join("，", error2));
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

		public FileResult GetListCSV(TransportationR filter, DateTime? start_date = null, DateTime? end_date = null)
		{
			var transportationR =
			from r in db.TransportationR.Where(x => x.IsDelete == false)
			join u in db.SYS_User on r.CreateBy equals u.Account into ps
			from u in ps.DefaultIfEmpty()
			join bl in db.Bill_Lading on r.LadingNo equals bl.LadingNo into ps2
			from bl in ps2.DefaultIfEmpty()
			select new
			{
				TransportationNo = r.TransportationNo,
				LadingNo = r.LadingNo,
				LadingNo_Type = bl != null ? bl.LadingNo_Type : r.LadingNo,
				RStatNo = r.RStatNo,
				RStatName = r.RStatName,
				ReceiveTime = r.ReceiveTime,
				RSectorNo = r.RSectorNo,
				RSectorName = r.RSectorName,
				ReceivePcs = r.ReceivePcs,
				CreateBy = u.UserName,
				CreateTime = r.CreateTime,
				UpdateBy = r.UpdateBy,
				UpdateTime = r.UpdateTime,
				DeletedBy = r.DeletedBy,
				DeletedTime = r.DeletedTime,
				IsDelete = r.IsDelete,
				IsCheck = r.IsCheck,
			};

			if (filter.LadingNo != null)
				transportationR = transportationR.Where(x => x.LadingNo.Contains(filter.LadingNo));
			if (filter.LadingNo_Type != null)
				transportationR = transportationR.Where(x => x.LadingNo_Type.Contains(filter.LadingNo_Type));
			if (filter.RSectorNo != null)
				transportationR = transportationR.Where(x => x.RSectorNo.Contains(filter.RSectorNo));
			if (filter.RSectorName != null)
				transportationR = transportationR.Where(x => x.RSectorName.Contains(filter.RSectorName));
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				transportationR = transportationR.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			var records = transportationR.Count();

			var dataList = new List<TransportationR>();
			var Index = 0;
			foreach (var t in transportationR)
			{
				Index = ++Index;
				var tData = new TransportationR()
				{
					Index = Index,
					TransportationNo = t.TransportationNo,
					LadingNo = t.LadingNo,
					LadingNo_Type = t.LadingNo_Type,
					RStatNo = t.RStatNo,
					RStatName = t.RStatName,
					ReceiveTime = t.ReceiveTime,
					RSectorNo = t.RSectorNo,
					RSectorName = t.RSectorName,
					ReceivePcs = t.ReceivePcs,
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
			var transportationRData = dataList as IEnumerable<TransportationR>;

			var Time = DateTime.Now.ToString("yyyyMMdd");
			var content = System.Text.Encoding.UTF8.GetBytes(CSVExporter.ExportCSVContent(transportationRData));
			return File(content, "text/csv", "收件資料_" + Time + ".csv");
		}
	}
}