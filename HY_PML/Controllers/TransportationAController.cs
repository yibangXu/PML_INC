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
	public class TransportationAController : Controller
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
			ViewBag.Title = "到件資料";
			ViewBag.AddFunc = "Add";
			ViewBag.EditFunc = "Edit";
			ViewBag.DelFunc = "Delete";
			ViewBag.ControllerName = "TransportationA";
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
				$"$('#isAdd').textbox('setValue', 'flase');";
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
		public ActionResult Add(TransportationA data, string[] dtl)
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
					var newData = new List<TransportationA>();
					JavaScriptSerializer js = new JavaScriptSerializer();
					foreach (var d in datalist)
					{
						newData.Add(js.Deserialize<TransportationA>(d));
					}
					newData = newData.Select((x, Index) => new TransportationA()
					{
						TransportationNo = db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).FirstOrDefault() == null ? "T-" + x.LadingNo :
						"T-" + db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).Select(b => b.LadingNo).FirstOrDefault(),
						LadingNo = db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).FirstOrDefault() == null ? x.LadingNo :
						 db.Bill_Lading.Where(b => b.LadingNo_Type == x.LadingNo).Select(b => b.LadingNo).FirstOrDefault(),
						AStatNo = statNo,
						AStatName = statName,
						ArrivalTime = x.ArrivalTime,
						LastStatNo = x.LastStatNo,
						LastStatName = x.LastStatName,
						ArrivalPcs = x.ArrivalPcs,
						LadingPcs = x.LadingPcs,
						LadingWeight = x.LadingWeight,
						ReviewWeight = x.ReviewWeight,
						ToPayment = x.ToPayment,
						AgentPay = x.AgentPay,
						CreateTime = DateTime.Now,
						CreateBy = User.Identity.Name,
						IsDelete = false,
						IsCheck = x.IsCheck,
					}).ToList();
					db.TransportationA.AddRange(newData);
					db.SaveChanges();
					foreach (var n in newData)
					{
						var ladingData = db.Bill_Lading.Where(x => x.LadingNo == n.LadingNo).FirstOrDefault();
						if (ladingData != null)
						{
							ladingData.Status = "到件";
							ladingData.StatusTime = n.ArrivalTime;
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
		public ActionResult Edit(TransportationA data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var editData = db.TransportationA.FirstOrDefault(x => x.TransportationNo == data.TransportationNo && x.ArrivalTime == data.ArrivalTime);

			if (editData != null)
			{
				editData.LastStatNo = data.LastStatNo;
				editData.LastStatName = data.LastStatName;
				editData.ArrivalPcs = data.ArrivalPcs;
				editData.ReviewWeight = data.ReviewWeight;
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
		public ActionResult Delete(TransportationA data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var deletedData = db.TransportationA.FirstOrDefault(x => x.TransportationNo == data.TransportationNo && x.ArrivalTime == data.ArrivalTime);
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
		public ActionResult GetGridJSON(TransportationA data, int page = 1, int rows = 40, DateTime? start_date = null, DateTime? end_date = null)
		{
			var transportationA =
				from a in db.TransportationA.Where(x => x.IsDelete == false)
				join u in db.SYS_User on a.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				join bl in db.Bill_Lading on a.LadingNo equals bl.LadingNo into ps2
				from bl in ps2.DefaultIfEmpty()
				select new
				{
					TransportationNo = a.TransportationNo,
					LadingNo = a.LadingNo,
					LadingNo_Type = bl != null ? bl.LadingNo_Type : a.LadingNo,
					AStatNo = a.AStatNo,
					AStatName = a.AStatName,
					ArrivalTime = a.ArrivalTime,
					LastStatNo = a.LastStatNo,
					LastStatName = a.LastStatName,
					ArrivalPcs = a.ArrivalPcs,
					LadingPcs = a.LadingPcs,
					LadingWeight = a.LadingWeight,
					ReviewWeight = a.ReviewWeight,
					ToPayment = a.ToPayment,
					AgentPay = a.AgentPay,
					CreateBy = u.UserName,
					CreateTime = a.CreateTime,
					UpdateBy = a.UpdateBy,
					UpdateTime = a.UpdateTime,
					DeletedBy = a.DeletedBy,
					DeletedTime = a.DeletedTime,
					IsDelete = a.IsDelete,
					IsCheck = a.IsCheck,
				};

			if (data.LadingNo != null)
				transportationA = transportationA.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type != null)
				transportationA = transportationA.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));
			if (data.LastStatNo != null)
				transportationA = transportationA.Where(x => x.LastStatNo == data.LastStatNo);
			if (data.LastStatName != null)
				transportationA = transportationA.Where(x => x.LastStatName == data.LastStatName);
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				transportationA = transportationA.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			var records = transportationA.Count();
			transportationA = transportationA.OrderBy(o => o.CreateTime).Skip((page - 1) * rows).Take(rows);

			var dataList = new List<TransportationA>();
			var Index = 0;
			foreach (var t in transportationA)
			{
				Index = ++Index;
				var tData = new TransportationA()
				{
					Index = Index,
					TransportationNo = t.TransportationNo,
					LadingNo = t.LadingNo,
					LadingNo_Type = t.LadingNo_Type,
					AStatNo = t.AStatNo,
					AStatName = t.AStatName,
					ArrivalTime = t.ArrivalTime,
					LastStatNo = t.LastStatNo,
					LastStatName = t.LastStatName,
					ArrivalPcs = t.ArrivalPcs,
					LadingPcs = t.LadingPcs,
					LadingWeight = t.LadingWeight,
					ReviewWeight = t.ReviewWeight,
					ToPayment = t.ToPayment,
					AgentPay = t.AgentPay,
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
			var transportationAData = dataList as IEnumerable<TransportationA>;

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "OK",
				Records = records,
				Pages = page,
				Data = transportationAData,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetTransportationAData(string LadingNo)
		{
			var transportationA =
				   from a in db.TransportationA.Where(x => x.IsDelete == false)
				   join u in db.SYS_User on a.CreateBy equals u.Account into ps
				   from u in ps.DefaultIfEmpty()
				   join bl in db.Bill_Lading on a.LadingNo equals bl.LadingNo into ps2
				   from bl in ps2.DefaultIfEmpty()
				   select new
				   {
					   TransportationNo = a.TransportationNo,
					   LadingNo = a.LadingNo,
					   LadingNo_Type = bl != null ? bl.LadingNo_Type : a.LadingNo,
					   AStatNo = a.AStatNo,
					   AStatName = a.AStatName,
					   ArrivalTime = a.ArrivalTime,
					   LastStatNo = a.LastStatNo,
					   LastStatName = a.LastStatName,
					   ArrivalPcs = a.ArrivalPcs,
					   LadingPcs = a.LadingPcs,
					   LadingWeight = a.LadingWeight,
					   ReviewWeight = a.ReviewWeight,
					   ToPayment = a.ToPayment,
					   AgentPay = a.AgentPay,
					   CreateBy = u.UserName,
					   CreateTime = a.CreateTime,
					   UpdateBy = a.UpdateBy,
					   UpdateTime = a.UpdateTime,
					   DeletedBy = a.DeletedBy,
					   DeletedTime = a.DeletedTime,
					   IsDelete = a.IsDelete,
					   IsCheck = a.IsCheck,
				   };
			var data = transportationA.Where(x => x.LadingNo_Type == LadingNo && x.IsDelete == false).FirstOrDefault();
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
		public ActionResult CheckData(IEnumerable<TransportationA> data)
		{
			var errorMsg = new StringBuilder();
			var error1 = new List<string>();
			var error2 = new List<string>();
			if (data.First().isAdd)
			{
				var groupData = data.GroupBy(g => g.LadingNo).Select(sl => new TransportationA
				{
					LadingNo = sl.First().LadingNo,
					ArrivalPcs = sl.Sum(s => s.ArrivalPcs),
				});
				foreach (var g in groupData)
				{
					var ladingData = db.Bill_Lading.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).FirstOrDefault();
					var DeliveryData = db.TransportationD.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).ToArray();
					var ArrivalData = db.TransportationA.Where(x => x.LadingNo == g.LadingNo && x.IsDelete == false).ToArray();
					if (ladingData == null)
						error1.Add(g.LadingNo);
					else
					{
						if (ArrivalData.Sum(s => s.ArrivalPcs) > 0)
						{
							if (ArrivalData.Sum(s => s.ArrivalPcs) + g.ArrivalPcs > DeliveryData.Sum(s => s.DeliveryPcs))
								error2.Add(g.LadingNo);
						}
						else
						{
							if (g.ArrivalPcs > DeliveryData.Sum(s => s.DeliveryPcs))
								error2.Add(g.LadingNo);
						}
					}
				}
			}
			else
			{
				var editData = data.First();
				var originData = db.TransportationA.Where(x => x.LadingNo == editData.LadingNo && x.ArrivalTime == editData.ArrivalTime).FirstOrDefault();
				var DeliveryData = db.TransportationD.Where(x => x.LadingNo == editData.LadingNo && x.IsDelete == false).ToArray();
				var ArrivalData = db.TransportationA.Where(x => x.LadingNo == editData.LadingNo && x.IsDelete == false).ToArray();
				if (ArrivalData.Sum(s => s.ArrivalPcs) - originData.ArrivalPcs + editData.ArrivalPcs > DeliveryData.Sum(s => s.DeliveryPcs))
					error2.Add(editData.LadingNo);

			}
			if (error1.Count > 0)
				errorMsg.Append("【提單單號】不存在：").Append("<br>").Append(String.Join("，", error1));
			if (error2.Count > 0)
			{
				if (error1.Count > 0)
					errorMsg.Append("<br>");
				errorMsg.Append("到件總數，超過出貨總數：").Append("<br>").Append(String.Join("，", error2));
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

		public FileResult GetListCSV(TransportationA filter, DateTime? start_date = null, DateTime? end_date = null)
		{
			var transportationA =
				from a in db.TransportationA.Where(x => x.IsDelete == false)
				join u in db.SYS_User on a.CreateBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				join bl in db.Bill_Lading on a.LadingNo equals bl.LadingNo into ps2
				from bl in ps2.DefaultIfEmpty()
				select new
				{
					TransportationNo = a.TransportationNo,
					LadingNo = a.LadingNo,
					LadingNo_Type = bl != null ? bl.LadingNo_Type : a.LadingNo,
					AStatNo = a.AStatNo,
					AStatName = a.AStatName,
					ArrivalTime = a.ArrivalTime,
					LastStatNo = a.LastStatNo,
					LastStatName = a.LastStatName,
					ArrivalPcs = a.ArrivalPcs,
					LadingPcs = a.LadingPcs,
					LadingWeight = a.LadingWeight,
					ReviewWeight = a.ReviewWeight,
					ToPayment = a.ToPayment,
					AgentPay = a.AgentPay,
					CreateBy = u.UserName,
					CreateTime = a.CreateTime,
					UpdateBy = a.UpdateBy,
					UpdateTime = a.UpdateTime,
					DeletedBy = a.DeletedBy,
					DeletedTime = a.DeletedTime,
					IsDelete = a.IsDelete,
					IsCheck = a.IsCheck,
				};

			if (filter.LadingNo != null)
				transportationA = transportationA.Where(x => x.LadingNo.Contains(filter.LadingNo));
			if (filter.LadingNo_Type != null)
				transportationA = transportationA.Where(x => x.LadingNo_Type.Contains(filter.LadingNo_Type));
			if (filter.LastStatNo != null)
				transportationA = transportationA.Where(x => x.LastStatNo == filter.LastStatNo);
			if (filter.LastStatName != null)
				transportationA = transportationA.Where(x => x.LastStatName == filter.LastStatName);
			if (start_date != null && end_date != null)
			{
				var sDate = start_date.Value.Date;
				var eDate = end_date.Value.Date;
				transportationA = transportationA.Where(x => DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(eDate) <= 0 && DbFunctions.TruncateTime(x.CreateTime).Value.CompareTo(sDate) >= 0);
			}

			var records = transportationA.Count();

			var dataList = new List<TransportationA>();
			var Index = 0;
			foreach (var t in transportationA)
			{
				Index = ++Index;
				var tData = new TransportationA()
				{
					Index = Index,
					TransportationNo = t.TransportationNo,
					LadingNo = t.LadingNo,
					LadingNo_Type = t.LadingNo_Type,
					AStatNo = t.AStatNo,
					AStatName = t.AStatName,
					ArrivalTime = t.ArrivalTime,
					LastStatNo = t.LastStatNo,
					LastStatName = t.LastStatName,
					ArrivalPcs = t.ArrivalPcs,
					LadingPcs = t.LadingPcs,
					LadingWeight = t.LadingWeight,
					ReviewWeight = t.ReviewWeight,
					ToPayment = t.ToPayment,
					AgentPay = t.AgentPay,
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
			var transportationAData = dataList as IEnumerable<TransportationA>;
			var Time = DateTime.Now.ToString("yyyyMMdd");
			var content = System.Text.Encoding.UTF8.GetBytes(CSVExporter.ExportCSVContent(transportationAData));
			return File(content, "text/csv", "到貨資料_" + Time + ".csv");
		}
	}
}