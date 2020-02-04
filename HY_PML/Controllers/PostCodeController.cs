using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class PostCodeController : Controller
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
			ViewBag.Title = "地址基本檔";
			ViewBag.ControllerName = "PostCode";
			ViewBag.FormPartialName = "_ElementInForm";

			//權限控管
			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		[Authorize]
		public ActionResult getSelectionGrid(string retID1, string retVal1, string retType1, string retID2, string retVal2, string retType2, string multiCheck = "")
		{
			ViewBag.retID1 = retID1;
			ViewBag.retVal1 = retVal1;
			ViewBag.retType1 = retType1;
			ViewBag.retID2 = retID2;
			ViewBag.retVal2 = retVal2;
			ViewBag.retType2 = retType2;

			//viewBag setting

			//頁面的抬頭
			ViewBag.Title = "地址基本檔";

			ViewBag.ThisControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

			//about grid
			ViewBag.GridShowGridActionName = "GetGridJSON";
			ViewBag.GridColumnDefinition = "{ field: 'Code5', title: '3+2郵遞區號' },{ field: 'CityCode', title: '縣市代碼' },{ field: 'CityName', title: '縣市' },{ field: 'AreaName', title: '地區名稱' },{ field: 'RoadName', title: '路段名稱' },{ field: 'RowName', title: '路號' },{ field: 'PickUpAreaNo', title: '調度區域代號'},{ field: 'Code7', title: '郵政7碼'  } ";

			if (multiCheck != "")
			{
				ViewBag.MultiCheck = "yes";
				ViewBag.GridColumnDefinition = "{field:'ck',checkbox:true}," + ViewBag.GridColumnDefinition;
			}

			//Grid搜尋的提示字
			ViewBag.GridSearchPromptText = "3+2郵遞區號:";
			ViewBag.GridShowSelectContentJs = "'('+row.Code5+')'+row.CityName+row.AreaName+row.RoadName+row.RowName+'('+row.Code7+')'";

			ViewBag.FormWidth = "750px";
			ViewBag.FormHeight = "570px";
			ViewBag.FormCustomJsNew = "";
			ViewBag.FormCustomJsEdit = "";

			//end viewBag setting
			return View();
		}

		[Authorize]
		public ActionResult getSelectionGrid20(string id, string retID1, string retVal1, string retType1, string retID2, string retVal2, string retType2, string retID3, string retVal3, string retType3, string retID4, string retVal4, string retType4, string retID5, string retVal5, string retType5, string retID6, string retVal6, string retType6, string retID7, string retVal7, string retType7, string retID8, string retVal8, string retType8, string retID9, string retVal9, string retType9, string retID10, string retVal10, string retType10, string retID11, string retVal11, string retType11, string retID12, string retVal12, string retType12, string retID13, string retVal13, string retType13, string retID14, string retVal14, string retType14, string retID15, string retVal15, string retType15, string retID16, string retVal16, string retType16, string retID17, string retVal17, string retType17, string retID18, string retVal18, string retType18, string retID19, string retVal19, string retType19, string retID20, string retVal20, string retType20, string target)
		{
			ViewBag.retID1 = retID1;
			ViewBag.retVal1 = retVal1;
			ViewBag.retType1 = retType1;
			ViewBag.retID2 = retID2;
			ViewBag.retVal2 = retVal2;
			ViewBag.retType2 = retType2;
			ViewBag.retID3 = retID3;
			ViewBag.retVal3 = retVal3;
			ViewBag.retType3 = retType3;

			ViewBag.retID4 = retID4;
			ViewBag.retVal4 = retVal4;
			ViewBag.retType4 = retType4;

			ViewBag.retID5 = retID5;
			ViewBag.retVal5 = retVal5;
			ViewBag.retType5 = retType5;

			ViewBag.retID6 = retID6;
			ViewBag.retVal6 = retVal6;
			ViewBag.retType6 = retType6;

			ViewBag.retID7 = retID7;
			ViewBag.retVal7 = retVal7;
			ViewBag.retType7 = retType7;

			ViewBag.retID8 = retID8;
			ViewBag.retVal8 = retVal8;
			ViewBag.retType8 = retType8;

			ViewBag.retID9 = retID9;
			ViewBag.retVal9 = retVal9;
			ViewBag.retType9 = retType9;

			ViewBag.retID10 = retID10;
			ViewBag.retVal10 = retVal10;
			ViewBag.retType10 = retType10;

			ViewBag.retID11 = retID11;
			ViewBag.retVal11 = retVal11;
			ViewBag.retType11 = retType11;

			ViewBag.retID12 = retID12;
			ViewBag.retVal12 = retVal12;
			ViewBag.retType12 = retType12;

			ViewBag.retID13 = retID13;
			ViewBag.retVal13 = retVal13;
			ViewBag.retType13 = retType13;

			ViewBag.retID14 = retID14;
			ViewBag.retVal14 = retVal14;
			ViewBag.retType14 = retType14;

			ViewBag.retID15 = retID15;
			ViewBag.retVal15 = retVal15;
			ViewBag.retType15 = retType15;

			ViewBag.retID16 = retID16;
			ViewBag.retVal16 = retVal16;
			ViewBag.retType16 = retType16;

			ViewBag.retID17 = retID17;
			ViewBag.retVal17 = retVal17;
			ViewBag.retType17 = retType17;

			ViewBag.retID18 = retID18;
			ViewBag.retVal18 = retVal18;
			ViewBag.retType18 = retType18;

			ViewBag.retID19 = retID19;
			ViewBag.retVal19 = retVal19;
			ViewBag.retType19 = retType19;

			ViewBag.retID20 = retID20;
			ViewBag.retVal20 = retVal20;
			ViewBag.retType20 = retType20;

			//viewBag setting

			//頁面的抬頭
			ViewBag.Title = "地址基本檔";

			ViewBag.ThisControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

			//about grid
			ViewBag.GridShowGridActionName = "GetGridJSON";
			if (target != null && target != "")
				ViewBag.GridParaMeter = "?target=" + target;
			ViewBag.GridColumnDefinition = "{ field: 'Code5', title: '3+2郵遞區號' },{ field: 'CityCode', title: '縣市代碼' },{ field: 'CityName', title: '縣市' },{ field: 'AreaName', title: '地區名稱' },{ field: 'RoadName', title: '路段名稱' },{ field: 'RowName', title: '路號' },{ field: 'Code7', title: '郵政7碼' } ,{ field: 'PickUpAreaNo', title: '調度區域代號' }";
			//Grid搜尋的提示字
			ViewBag.GridSearchPromptText = "3+2郵遞區號:";
			ViewBag.GridShowSelectContentJs = "'('+row.Code5+')'+row.CityName+row.AreaName+row.RoadName+row.RowName+'('+row.Code7+')'";

			ViewBag.FormWidth = "750px";
			ViewBag.FormHeight = "570px";
			ViewBag.FormCustomJsNew = "";
			ViewBag.FormCustomJsEdit = "";

			ViewBag.UsePickDetail20 = "yes";
			//end viewBag setting
			return View();

		}

		[Authorize]
		public ActionResult Add(ORG_PostCode data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();

			var duplicated = db.ORG_PostCode.Any(x => x.Code5 == data.Code5 && x.CityName == data.CityName && x.AreaName == data.AreaName && x.RoadName == data.RoadName && x.RowName == data.RowName && x.IsDelete == false);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複資料!";
			}
			else
			{
				ORG_PostCode userRecord = new ORG_PostCode();
				var Code7 = db.ORG_PostCode.Where(x => x.Code5 == data.Code5).OrderByDescending(x => x.Code7).Select(x => x.Code7).FirstOrDefault();
				userRecord.CityCode = data.CityCode;
				userRecord.Code5 = data.Code5;
				userRecord.CityName = data.CityName;
				userRecord.AreaName = data.AreaName;
				userRecord.RoadName = data.RoadName;
				userRecord.RowName = data.RowName;
				userRecord.Code7 = Code7 == null ? data.Code5 + "00" : (Int32.Parse(Code7) + 1).ToString();
				userRecord.CreatedDate = DateTime.Now;
				userRecord.CreatedBy = User.Identity.Name;

				//以下系統自填
				userRecord.IsDelete = false;

				try
				{
					db.ORG_PostCode.Add(userRecord);
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
		public ActionResult Edit(ORG_PostCode data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();

			ORG_PostCode userRecord = db.ORG_PostCode.FirstOrDefault(x => x.Code5 == data.Code5 && x.CityName == data.CityName && x.AreaName == data.AreaName && x.RoadName == data.RoadName && x.RowName == data.RowName);

			if (userRecord != null)
			{
				ORG_PostCode userRocordNew = new ORG_PostCode();
				userRocordNew.CityCode = data.CityCode;
				userRocordNew.Code5 = data.Code5;
				userRocordNew.CityName = data.CityName;
				userRocordNew.AreaName = data.AreaName;
				userRocordNew.RoadName = data.RoadName;
				userRocordNew.RowName = data.RowName;
				userRocordNew.Code7 = data.Code7;

				try
				{
					db.ORG_PostCode.Remove(userRecord);
					db.ORG_PostCode.Add(userRocordNew);
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
		public ActionResult Delete(ORG_PostCode data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			var result = new ResultHelper();

			ORG_PostCode userRecord = db.ORG_PostCode.FirstOrDefault(x => x.Code5 == data.Code5 && x.CityName == data.CityName && x.AreaName == data.AreaName && x.RoadName == data.RoadName && x.RowName == data.RowName);

			if (userRecord != null)
			{
				//以下系統自填
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
		public ActionResult GetDistinctCityName()
		{
			var cityNames = (from r in db.ORG_PostCode
							 select r.CityName).Distinct();
			JArray ja = new JArray();
			foreach (var item in cityNames)
			{
				var jobj = new JObject {
					{"id",item.ToString() },
					{"text",item.ToString()}
				};
				ja.Add(jobj);
			}
			return Content(JsonConvert.SerializeObject(ja), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetDistinctAreaName(string cityName = "")
		{
			var areaNames = (from r in db.ORG_PostCode
							 where r.CityName == cityName
							 select r.AreaName).Distinct();
			JArray ja = new JArray();
			foreach (var item in areaNames)
			{
				var jobj = new JObject {
					{"id",item.ToString() },
					{"text",item.ToString()}
				};
				ja.Add(jobj);
			}
			return Content(JsonConvert.SerializeObject(ja), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetRoadName(string cityName = "", string areaName = "")
		{
			var roadNames = (from r in db.ORG_PostCode
							 where r.CityName == cityName && r.AreaName == areaName
							 select r.RoadName).Distinct();
			JArray ja = new JArray();
			foreach (var item in roadNames)
			{
				var jobj = new JObject {
					{"id",item.ToString() },
					{"text",item.ToString()}
				};
				ja.Add(jobj);
			}
			return Content(JsonConvert.SerializeObject(ja), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult GetGridJSON(int page = 1, int rows = 40, string target = "", string cityName = "", string areaName = "", string roadName = "")
		{
			IQueryable<ORG_PostCode> PostCode;
			int total_count;

			if (cityName != "")
			{
				if (roadName != "")
				{
					PostCode = from o in db.ORG_PostCode
							   where (o.CityName.Contains(cityName) && o.AreaName.Contains(areaName) && o.RoadName.Contains(roadName)) && o.IsDelete == false
							   select o;
				}
				else if (areaName != "")
				{
					PostCode = from o in db.ORG_PostCode
							   where (o.CityName.Contains(cityName) && o.AreaName.Contains(areaName)) && o.IsDelete == false
							   select o;
				}
				else
				{
					PostCode = from o in db.ORG_PostCode
							   where (o.CityName.Contains(cityName)) && o.IsDelete == false
							   select o;

				}
				total_count = PostCode.Count();
				PostCode = PostCode.OrderBy(o => o.Code5).Skip((page - 1) * rows).Take(rows);

			}
			else if (target != "")
			{
				PostCode = from o in db.ORG_PostCode
						   where (o.Code5.Contains(target)) && o.IsDelete == false
						   select o;
				total_count = PostCode.Count();
				PostCode = PostCode.OrderBy(o => o.Code7).Skip((page - 1) * rows).Take(rows);

			}
			else
			{
				PostCode = from o in db.ORG_PostCode
						   where o.IsDelete == false
						   select o;
				total_count = PostCode.Count();
				PostCode = PostCode.OrderBy(o => o.Code7).Skip((page - 1) * rows).Take(rows);
			}
			JArray ja = new JArray();
			foreach (var item in PostCode)
			{
				var pickUpArea = from pd in db.ORG_PickUpAreaAddress
								 join pm in db.ORG_PickUpArea on pd.PickUpAreaNo equals pm.PickUpAreaNo into ps
								 from pm in ps.DefaultIfEmpty()
								 where pd.IsDelete == false && pm.IsDelete == false && pd.Code5 == item.Code5 && pd.CityAreaRoadRow == (item.CityName + item.AreaName + item.RoadName + item.RowName)
								 orderby pm.CreatedDate descending
								 select new { pd, pm };
				var slPickUpAreaNo = "";
				var slDateEnd = "";
				if (pickUpArea.Count() > 0)
				{
					slPickUpAreaNo = pickUpArea.First().pd.PickUpAreaNo;
					slDateEnd = pickUpArea.First().pm.DateEnd;
				}

				var jobj = new JObject {
					{"CityCode",item.CityCode },
					{"Code5",item.Code5},
					{"Code3",item.Code5.Substring(0,3)},
					{"CityName",item.CityName},
					{"AreaName",item.AreaName},
					{"RoadName",item.RoadName},
					{"RowName",item.RowName},
					{"Code7",item.Code7},
					{"CityAreaRoadRow","(" + item.Code5 + ")"+item.CityName+item.AreaName+item.RoadName+item.RowName},
					{"AdrressSimpleVer",item.CityName+item.AreaName+item.RoadName },
					{"IsDelete",item.IsDelete },
					{"PickUpAreaNo",slPickUpAreaNo },
					{"DateEnd",slDateEnd }
				};
				ja.Add(jobj);
			}

			JObject result = new JObject {
				{"total",total_count},
				{"rows",ja}
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON_New(ORG_PostCode data, int page = 1, int rows = 40)
		{
			var postCode =
				from pc in db.ORG_PostCode.Where(x => x.IsDelete == false)
				join u in db.SYS_User on pc.CreatedBy equals u.Account into ps
				from u in ps.DefaultIfEmpty()
				select new
				{
					CityCode = pc.CityCode,
					Code5 = pc.Code5,
					CityName = pc.CityName,
					AreaName = pc.AreaName,
					RoadName = pc.RoadName,
					RowName = pc.RowName,
					Code7 = pc.Code7,
					CreatedDate=pc.CreatedDate,
					CreatedBy=pc.CreatedBy,
					IsDelete=pc.IsDelete,
					UserName = u == null ? "系統匯入" : u.UserName
				};

			if (data.Code5.IsNotEmpty())
				postCode = postCode.Where(x => x.Code5.Contains(data.Code5));
			if (data.CityName.IsNotEmpty())
				postCode = postCode.Where(x => x.CityName.Contains(data.CityName));
			if (data.CityCode.IsNotEmpty())
				postCode = postCode.Where(x => x.CityCode.Contains(data.CityCode));
			if (data.RoadName.IsNotEmpty())
				postCode = postCode.Where(x => x.RoadName.Contains(data.RoadName));
			if (data.AreaName.IsNotEmpty())
				postCode = postCode.Where(x => x.AreaName.Contains(data.AreaName));
			if (data.RowName.IsNotEmpty())
				postCode = postCode.Where(x => x.RowName.Contains(data.RowName));
			if (data.Code7.IsNotEmpty())
				postCode = postCode.Where(x => x.Code7.Contains(data.Code7));

			var records = postCode.Count();
			postCode = postCode.OrderBy(o => o.Code7).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = postCode,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}