using HY_PML.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace HY_PML.helper
{
	public class WebSiteHelper
	{
		public static string CurrentUserID
		{
			get
			{
				var httpContext = HttpContext.Current;
				var identity = httpContext.User.Identity as FormsIdentity;
				if (identity == null)
				{
					return string.Empty;
				}
				else
				{
					var userID = identity.Name;
					return userID;
				}
			}
		}

		public static string CurrentUserDBID
		{
			get
			{
				var httpContext = HttpContext.Current;
				var identity = httpContext.User.Identity as FormsIdentity;
				if (identity == null)
				{
					return string.Empty;
				}
				else
				{
					var userDBID = identity.Ticket.UserData;
					return userDBID;
				}
			}
		}

		public static string ReturnHtmlString
		{
			get
			{
				return "<!DOCTYPE html><html><head><base href='{1}' /></head><body>{0}<a href='./Login/Index'>返回登入頁</a></body></html>";
			}
		}

		public static DataTable MyUserActionInfo(string userID)
		{
			DataTable dtTarget = new DataTable();
			PML db = new PML();
			int nlUserID;
			int.TryParse(userID, out nlUserID);

			var actUser = from au in db.SYS_ActUser
						  from u in db.SYS_User
						  from ai in db.SYS_ActInfo
						  from m in db.SYS_MenuItem
						  where au.UserID == nlUserID && au.UserID == u.ID && au.ActID == ai.ID && ai.ActionID == m.ID && au.IsDelete == false && au.ActionType == true && u.IsDelete == false && ai.IsDelete == false && ai.ActionType == true && m.IsDelete == false
						  select new { au, m };
			dtTarget.Columns.Add("menuCaption");
			dtTarget.Columns.Add("menuParentID");
			dtTarget.Columns.Add("menuController");
			dtTarget.Columns.Add("menuAction");
			dtTarget.Columns.Add("userID");
			dtTarget.Columns.Add("menuID");
			dtTarget.Columns.Add("actInfoID");
			dtTarget.Columns.Add("actUserID");

			if (actUser != null)
			{
				if (actUser.Count() > 0)
				{
					foreach (var record in actUser)
					{
						var dr = dtTarget.NewRow();
						dr["menuCaption"] = record.m.Caption;
						dr["menuParentID"] = record.m.ParentID;
						dr["menuController"] = record.m.Controller;
						dr["menuAction"] = record.m.Action;
						dr["userID"] = record.au.UserID;
						dr["menuID"] = record.m.ID;
						dr["actInfoID"] = record.au.ActID;
						dr["actUserID"] = record.au.ID;
						dtTarget.Rows.Add(dr);
					}
				}
			}

			return dtTarget;
		}

		public static DataTable MyUserGroupActionInfo(string userGroupNo)
		{
			DataTable dtTarget = new DataTable();
			PML db = new PML();

			var actUser = from au in db.SYS_ActUserGroup
						  from u in db.SYS_UserGroup
						  from ai in db.SYS_ActInfo
						  from m in db.SYS_MenuItem
						  where au.UserGroupNo == userGroupNo && au.UserGroupNo == u.UserGroupNo && au.ActID == ai.ID && ai.ActionID == m.ID && au.IsDelete == false && au.IsDelete == false && u.IsDelete == false && ai.IsDelete == false && ai.ActionType == true && m.IsDelete == false && u.IsActive == true
						  select new { au, m };
			dtTarget.Columns.Add("menuCaption");
			dtTarget.Columns.Add("menuParentID");
			dtTarget.Columns.Add("menuController");
			dtTarget.Columns.Add("menuAction");
			dtTarget.Columns.Add("userGroupNo");
			dtTarget.Columns.Add("menuID");
			dtTarget.Columns.Add("actInfoID");
			//dtTarget.Columns.Add("actUserID");

			if (actUser != null)
			{
				if (actUser.Count() > 0)
				{
					foreach (var record in actUser)
					{
						var dr = dtTarget.NewRow();
						dr["menuCaption"] = record.m.Caption;
						dr["menuParentID"] = record.m.ParentID;
						dr["menuController"] = record.m.Controller;
						dr["menuAction"] = record.m.Action;
						dr["userGroupNo"] = record.au.UserGroupNo;
						dr["menuID"] = record.m.ID;
						dr["actInfoID"] = record.au.ActID;
						//dr["actUserID"] = record.au.ID;
						dtTarget.Rows.Add(dr);
					}
				}
			}

			return dtTarget;
		}

		public static bool IsFieldValueInDataTable(string fieldName, string fieldValue, DataTable dtTarget)
		{
			//check field name
			bool checkPoint;
			checkPoint = false;
			for (int i = 0; i < dtTarget.Columns.Count; i++)
			{
				if (dtTarget.Columns[i].ColumnName == fieldName)
				{
					checkPoint = true;
				}
			}
			if (checkPoint == false)
			{
				return false;
			}
			//check value
			checkPoint = false;
			for (int i = 0; i < dtTarget.Rows.Count; i++)
			{
				var dr = dtTarget.Rows[i];
				if (dr[fieldName].ToString() == fieldValue)
				{
					//查parent
					if (dr["menuParentID"] != null)
					{
						if (dr["menuParentID"].ToString().Trim() != "")
						{
							for (int j = 0; j < dtTarget.Rows.Count; j++)
							{
								var dr2 = dtTarget.Rows[j];
								if (dr2["menuID"].ToString() == dr["menuParentID"].ToString()) { checkPoint = true; }
							}
						}
						else
						{
							checkPoint = true;
						}
					}
					else
					{
						checkPoint = true;
					}
					if (checkPoint == true)
					{
						return true;
					}
				}
			}

			return false;
		}
		public static bool IsFieldValueInDataTable(Dictionary<string, string> dcFieldNameValue, DataTable dtTarget)
		{
			//check field name
			bool checkPoint;
			checkPoint = false;

			foreach (KeyValuePair<string, string> pair in dcFieldNameValue)
			{
				checkPoint = false;
				for (int i = 0; i < dtTarget.Columns.Count; i++)
				{
					if (dtTarget.Columns[i].ColumnName == pair.Key) { checkPoint = true; }
				}
				if (checkPoint == false)
				{
					return false;
				}
			}

			//check value

			for (int i = 0; i < dtTarget.Rows.Count; i++)
			{
				var dr = dtTarget.Rows[i];
				int checkOkNum = 0;
				foreach (KeyValuePair<string, string> pair in dcFieldNameValue)
				{
					if (dr[pair.Key].ToString() == pair.Value)
					{
						//查parent
						checkPoint = false;
						if (dr["menuParentID"] != null)
						{
							if (dr["menuParentID"].ToString().Trim() != "")
							{
								for (int j = 0; j < dtTarget.Rows.Count; j++)
								{
									var dr2 = dtTarget.Rows[j];
									if (dr2["menuID"].ToString() == dr["menuParentID"].ToString()) { checkPoint = true; }
								}
							}
							else
							{
								checkPoint = true;
							}
						}
						else
						{
							checkPoint = true;
						}
						if (checkPoint == true)
						{
							checkOkNum += 1;
						}
					}
				}
				if (checkOkNum == dcFieldNameValue.Count)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsPermissioned(string actionName, string controllerName, string fieldName = "", string fieldValue = "")
		{
			var db = new PML();
			var httpContext = HttpContext.Current;
			var identity = httpContext.User.Identity as FormsIdentity;
			var user_data = identity.Ticket.UserData;
			//20180704新增系統內建全權限使用者
			if (user_data == "-999")
			{
				return true;
			}

			var userID = Convert.ToInt32(user_data);
			var userGroupNo = db.SYS_User.FirstOrDefault(x => x.ID == userID && x.IsDelete == false).UserGroupNo;

			DataTable dtPermission = MyUserActionInfo(user_data);
			DataTable dtGroupPermission = MyUserGroupActionInfo(userGroupNo);

			//取得權限--以上
			Dictionary<string, string> dcTarget = new Dictionary<string, string>();
			dcTarget.Add("menuController", controllerName);
			dcTarget.Add("menuAction", actionName);

			if (fieldName != "")
			{
				if (IsFieldValueInDataTable(fieldName, fieldValue, dtPermission))
				{
					return true;
				}
				else
				{
					if (IsFieldValueInDataTable(fieldName, fieldValue, dtGroupPermission))
					{
						return true;
					}
					return false;
				}
			}

			if (IsFieldValueInDataTable(dcTarget, dtPermission))
			{
				return true;
			}
			else
			{
				if (IsFieldValueInDataTable(dcTarget, dtGroupPermission))
				{
					return true;
				}
				return false;
			}
		}

		public static string MyFormatDateString(DateTime? target)
		{
			try
			{
				return String.Format("{0:yyyy/MM/dd}", target);
			}
			catch (Exception e)
			{
				return "";
			}
		}

		public static string MyFormatDateTimeString(DateTime? target)
		{
			try
			{
				return String.Format("{0:yyyy/MM/dd HH:mm:ss}", target);
			}
			catch (Exception e)
			{
				return "";
				//throw;
			}
		}

		public static DataTable LinqQueryToDataTable<T>(IEnumerable<T> query)
		{
			DataTable tbl = new DataTable();
			PropertyInfo[] props = null;
			foreach (T item in query)
			{
				if (props == null) //尚未初始化
				{
					Type t = item.GetType();
					props = t.GetProperties();
					foreach (PropertyInfo pi in props)
					{
						Type colType = pi.PropertyType;
						//針對Nullable<>特別處理
						if (colType.IsGenericType
							&& colType.GetGenericTypeDefinition() == typeof(Nullable<>))
							colType = colType.GetGenericArguments()[0];
						//建立欄位
						tbl.Columns.Add(pi.Name, colType);
					}
				}
				DataRow row = tbl.NewRow();
				foreach (PropertyInfo pi in props)
					row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
				tbl.Rows.Add(row);
			}
			return tbl;
		}

		public static string GetLast(string source, int last)
		{
			return last >= source.Length ? source : source.Substring(source.Length - last);
		}

		public static string ResponseAjaxContentType(string browserType)
		{
			if (browserType.Contains("IE") || browserType.Contains("Internet"))
				return "text/htm";
			else
				return "application/json";
		}

		public static string GetActionStr(string controllerName)
		{
			var db = new PML();
			string cId = WebSiteHelper.CurrentUserID;
			int dbId = int.Parse(WebSiteHelper.CurrentUserDBID);
			var userAct = from au in db.SYS_ActUser.Where(au => au.IsDelete == false && au.ActionType == true && au.UserID == dbId)
						  from m in db.SYS_MenuItem.Where(m => m.Controller == controllerName && m.Action != "Index")
						  join ai in db.SYS_ActInfo on m.ID equals ai.ActionID into ps
						  from ai in ps.DefaultIfEmpty()
						  where au.ActID == ai.ID
						  select new
						  {
							  Act = m.Action,
						  };
			return String.Join(",", userAct.Select(x => x.Act).ToArray());
		}
	}
}