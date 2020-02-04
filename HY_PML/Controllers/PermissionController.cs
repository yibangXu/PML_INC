using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class PermissionController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

		[Authorize]
		public ActionResult Index()
		{
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "權限維護";
			ViewBag.ControllerName = "Permission";

			//權限控管
			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		[Authorize]
		public ActionResult NewUser()
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			SYS_User userRecord = new SYS_User();

			userRecord.Account = Request["Account"];
			userRecord.Password = System.Text.Encoding.Default.GetBytes(Request["Password"]);
			if (Request["RegisterDate"] != "")
				userRecord.RegisterDate = Convert.ToDateTime(Request["RegisterDate"]);
			else
				userRecord.RegisterDate = DateTime.Now;
			if (Request["ActiveDate"] != "")
				userRecord.ActiveDate = Convert.ToDateTime(Request["ActiveDate"]);
			if (Request["ExpiryDate"] != "")
				userRecord.ExpiryDate = Convert.ToDateTime(Request["ExpiryDate"]);
			userRecord.SecurityQuestion = Request["SecurityQuestion"];
			userRecord.SecurityAnswer = Request["SecurityAnswer"];
			//以下系統自填
			userRecord.CreatedDate = DateTime.Now;
			userRecord.CreatedBy = User.Identity.Name;
			userRecord.IsDelete = false;

			JObject result;
			try
			{
				db.SYS_User.Add(userRecord);
				db.SaveChanges();
				result = new JObject { { "message", "ok" } };
			}
			catch (Exception e)
			{
				result = new JObject { { "errorMsg", e.Message } };
			}

			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult DeleteUser()
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			int id;
			JObject result;
			int.TryParse(Request["ID"], out id);
			SYS_User userRecord = db.SYS_User.Find(id);

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
					result = new JObject { { "message", "ok" } };
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}
				catch (Exception e)
				{
					result = new JObject { { "errorMsg", e.Message } };
					return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
				}
			}

			result = new JObject { { "errorMsg", "找不到資料!" } };
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult EditPermission(string uid, string checkNodeId)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			int user_id;
			int.TryParse(uid, out user_id);

			// find menuitemid in sys_actinfo
			ArrayList MenuItemIdArray = new ArrayList();
			var slMenuItemIdArray = checkNodeId.Split(',');
			foreach (var item in slMenuItemIdArray)
			{
				int nlId;
				int.TryParse(item, out nlId);
				MenuItemIdArray.Add(nlId);
			}
			//find menuItem parent
			int[] MenuItemIdAry = (int[])MenuItemIdArray.ToArray(typeof(int));
			var menuItem = from mItem in db.SYS_MenuItem
						   where MenuItemIdAry.Contains(mItem.ID)
						   select mItem;
			foreach (var item in menuItem)
			{
				if (item.ParentID != null)
				{
					if (MenuItemIdArray.Contains(item.ParentID))
					{ }
					else
					{
						MenuItemIdArray.Add(item.ParentID);
						var TypeID = db.SYS_MenuItem.Where(x => x.ID == item.ParentID).Select(x => x.ParentID).FirstOrDefault();
						if (TypeID != null)
							MenuItemIdArray.Add(TypeID);
					}
				}
			}

			MenuItemIdAry = (int[])MenuItemIdArray.ToArray(typeof(int));
			var actinfo = from ainfo in db.SYS_ActInfo
						  where MenuItemIdAry.Contains(ainfo.ActionID)
						  select ainfo;

			//find the user  actuser record
			var actuser = from auser in db.SYS_ActUser
						  where auser.UserID == user_id
						  select auser;
			ArrayList ActUserActIdArray = new ArrayList();
			foreach (var item in actuser)
			{
				ActUserActIdArray.Add(int.Parse(item.ActID.ToString()));
			}

			int[] ActUserActIdAry = (int[])ActUserActIdArray.ToArray(typeof(int));
			ArrayList ActInfoIdArray = new ArrayList();
			foreach (var item in actinfo)
			{
				ActInfoIdArray.Add(item.ID);
			}

			DateTime dtNowStamp = DateTime.Now;

			//先全部的actuser的actiontype設 0 
			foreach (var item in actuser)
			{
				item.ActionType = false;
				item.IsDelete = false;
				item.UpdatedDate = dtNowStamp;
				item.UpdatedBy = User.Identity.Name;
				db.Entry(item).State = EntityState.Modified;
			}

			//看看 actinfo  是否有在 actuser中
			foreach (var item in ActInfoIdArray)
			{
				//有， 設 actiontype為1
				if (ActUserActIdAry.Contains((int)item))
				{
					foreach (var item2 in actuser)
					{
						if ((int)item2.ActID == (int)item)
						{
							item2.ActionType = true;
							item2.IsDelete = false;
							item2.UpdatedDate = dtNowStamp;
							item2.UpdatedBy = User.Identity.Name;
							db.Entry(item2).State = EntityState.Modified;
							break;
						}
					}

				}
				else  //沒有， 新增actuser，且 actiontype設為 1
				{
					SYS_ActUser newActUser = new SYS_ActUser();
					newActUser.ActID = short.Parse(item.ToString());
					newActUser.UserID = (short)user_id;
					newActUser.ActionType = true;
					newActUser.CreatedDate = dtNowStamp;
					newActUser.CreatedBy = User.Identity.Name;
					newActUser.IsDelete = false;
					db.SYS_ActUser.Add(newActUser);
				}
			}

			db.SaveChanges();

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Message = "存檔成功!"
			};
			return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult getPermissionJson(string uid = "")
		{
			if (uid == "")
				return Content("no data");

			DataTable dtPermission = WebSiteHelper.MyUserActionInfo(uid);
			JArray ja = new JArray();
			int nlTargetColIndex = 0;
			int nlParentColIndex = 0;
			int nlActionColIndex = 0;
			for (int j = 0; j < dtPermission.Columns.Count; j++)
			{
				if (dtPermission.Columns[j].ColumnName == "menuID")
				{
					nlTargetColIndex = j;
				}
				if (dtPermission.Columns[j].ColumnName == "menuParentID")
				{
					nlParentColIndex = j;
				}
				if (dtPermission.Columns[j].ColumnName == "menuAction")
				{
					nlActionColIndex = j;
				}
			}
			for (int i = 0; i < dtPermission.Rows.Count; i++)
			{
				var item = new JObject {
						{ "id",dtPermission.Rows[i].ItemArray[nlTargetColIndex].ToString()}
					};

				if (dtPermission.Rows[i].ItemArray[nlParentColIndex] != null)
				{
					if (dtPermission.Rows[i].ItemArray[nlParentColIndex].ToString() == "")
					{
					}
					else if (dtPermission.Rows[i].ItemArray[nlActionColIndex].ToString() == "Index")
					{
					}
					else
					{
						ja.Add(item);
					}
				}
			}
			return Content(JsonConvert.SerializeObject(ja), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult getPermissionTree()
		{
			JArray ja = new JArray();
			var menu = from mu in db.SYS_Menu
					   where mu.IsDelete == false
					   orderby mu.Name
					   select mu;

			foreach (var menuRecord in menu)
			{
				var menuItem = from r in db.SYS_MenuItem
							   where r.MenuID == menuRecord.ID && r.ParentID == null && r.IsDelete == false
							   orderby r.Sequence
							   select r;
				foreach (var menuItemParent in menuItem)
				{
					var childItem = from c in db.SYS_MenuItem
									where c.MenuID == menuRecord.ID && c.ParentID == menuItemParent.ID && c.IsDelete == false
									orderby c.Sequence
									select c;
					JArray childNode = new JArray();
					foreach (var childItemRecord in childItem)
					{
						var actionItem = from s in db.SYS_MenuItem
										 where s.ParentID == childItemRecord.ID && s.IsDelete == false && s.Action != "Index"
										 orderby s.Sequence
										 select s;
						JArray actionNode = new JArray();
						foreach (var a in actionItem)
						{
							var node2 = new JObject {
							{ "id",a.ID},
							{ "parent",childItemRecord.ID},
							{ "text",a.Caption},
							//{ "action",a.Action},
							{ "icon", "jstree-file"}
							};
							ja.Add(node2);
						}
						var node = new JObject {
							{ "id",childItemRecord.ID},
							{"parent",menuItemParent.ID },
							{ "text",childItemRecord.Caption},
							//{ "href", String.Format("./{0}/{1}", childItemRecord.Controller, childItemRecord.Action)},
							{ "icon", "jstree-file"},
							//{ "actionArray",actionNode},
						};

						ja.Add(node);
					}

					var nodeParent = new JObject {
						{ "id",menuItemParent.ID},
						{"parent","#" },
						{ "text",menuItemParent.Caption},
						//{ "children",childNode},
						{ "icon", "jstree-folder"},
					};
					//權限控管                    
					ja.Add(nodeParent);
				}
			}
			return Content(JsonConvert.SerializeObject(ja), "application/json");
		}

		[Authorize]
		public ActionResult GetGridJSON(SYS_User data, int page = 1, int rows = 40)
		{
			var users = from o in db.SYS_User
						join s in db.ORG_Stat on o.StatNo equals s.StatNo into ps
						from s in ps.DefaultIfEmpty()
						join d in db.ORG_Depart on o.DepartNo equals d.DepartNo into ps2
						from d in ps2.DefaultIfEmpty()
						join ug in db.SYS_UserGroup on o.UserGroupNo equals ug.UserGroupNo into ps3
						from ug in ps3.DefaultIfEmpty()
						where o.IsDelete == false
						select new
						{
							o.ID,
							o.Account,
							o.UserName,
							o.RegisterDate,
							o.ActiveDate,
							o.ExpiryDate,
							o.SecurityQuestion,
							o.SecurityAnswer,
							o.CreatedDate,
							o.CreatedBy,
							o.UpdatedDate,
							o.UpdatedBy,
							o.DeletedBy,
							o.DeletedDate,
							o.IsDelete,
							s.StatNo,
							s.StatName,
							d.DepartNo,
							d.DepartName,
							ug.UserGroupNo,
							ug.UserGroupName,
							o.IsActive,
							o.Remark
						};

			if (data.UserName.IsNotEmpty())
				users = users.Where(x => x.UserName.Contains(data.UserName));
			if (data.Account.IsNotEmpty())
				users = users.Where(x => x.Account.Contains(data.Account));
			if (data.UserGroupName.IsNotEmpty())
				users = users.Where(x => x.UserGroupName.Contains(data.UserGroupName));
			if (data.DepartName.IsNotEmpty())
				users = users.Where(x => x.DepartName.Contains(data.DepartName));
			if (data.StatName.IsNotEmpty())
				users = users.Where(x => x.StatName.Contains(data.StatName));
			if ((data.IsActive == false && Request["IsActive"]?.ToLower() == "false") || data.IsActive == true)
				users = users.Where(x => x.IsActive == data.IsActive);

			var records = users.Count();
			users = users.OrderBy(o => o.UserGroupNo).ThenBy(o => o.Account).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = users,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}