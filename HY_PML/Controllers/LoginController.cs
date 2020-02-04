using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HY_PML.Controllers
{
	public class LoginController : Controller
	{
		private PML db = new PML();
		// GET: Login
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(string user_id, string user_password)
		{
			string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

			//20180704新增系統內建全權限使用者
			if ((user_id == "hyAdmin") && (user_password == "hyAdmin0704"))
			{
				Session.RemoveAll();
				FormsAuthenticationTicket ticketHY = new FormsAuthenticationTicket(1,
				  "hyAdmin",//你想要存放在 User.Identy.Name 的值，通常是使用者帳號
				  DateTime.Now,
				  DateTime.Now.AddHours(12),
				  false,//將管理者登入的 Cookie 設定成 Session Cookie
				  "-999",//userdata看你想存放啥
				  FormsAuthentication.FormsCookiePath);

				string encTicketHY = FormsAuthentication.Encrypt(ticketHY);

				Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicketHY));
				setUserInfoSession("hyAdmin", "hyAdmin", "-999", "hyAdmin");
				if (Request["ReturnUrl"] == null)
					return RedirectToAction("Index", "Login");

				return Redirect(Request["ReturnUrl"]);
			}

			var record = from r in db.SYS_User
						 where r.Account == user_id && r.IsDelete == false
						 select r;
			if (record == null)
			{
				return Content(String.Format(slLogoutHtml, "無此帳號...", Request.ApplicationPath));
			}
			if (record.Count() == 0)
			{
				return Content(String.Format(slLogoutHtml, "無此帳號...", Request.ApplicationPath));
			}

			if (record.Count() > 1)
			{
				//duplicate acount name
				var inputPassword = System.Text.Encoding.Default.GetBytes(user_password);
				record = from rec in record
						 where rec.Password == inputPassword
						 select rec;
				if (record.Count() > 1)
				{
					return Content(String.Format(slLogoutHtml, "帳號重複...", Request.ApplicationPath));
				}
			}
			var temp = System.Text.Encoding.ASCII.GetString(record.First().Password);
			if (System.Text.Encoding.ASCII.GetString(record.First().Password) != user_password)
			{
				return Content(String.Format(slLogoutHtml, "密碼錯誤...", Request.ApplicationPath));
			}

			if (record.First().ActiveDate == null)
			{
				return Content(String.Format(slLogoutHtml, "帳號未啟用...", Request.ApplicationPath));
			}

			//啟用日期與今日判斷
			int nlToday;
			int.TryParse(WebSiteHelper.MyFormatDateString(DateTime.Now).Replace("/", ""), out nlToday);
			int nlTarget;
			int.TryParse(WebSiteHelper.MyFormatDateString(record.First().ActiveDate).Replace("/", ""), out nlTarget);
			if (nlToday < nlTarget)
			{
				return Content(String.Format(slLogoutHtml, "帳號未啟用...", Request.Url.AbsoluteUri));
			}

			//是否逾期
			if (record.First().ExpiryDate != null)
			{
				int.TryParse(WebSiteHelper.MyFormatDateString(record.First().ExpiryDate).Replace("/", ""), out nlTarget);
				if (nlToday >= nlTarget)
				{
					return Content(String.Format(slLogoutHtml, "帳號已逾期...", Request.ApplicationPath));
				}
			}
			//是否作廢
			if (record.First().IsDelete == true)
			{
				return Content(String.Format(slLogoutHtml, "帳號已刪除...", Request.ApplicationPath));
			}
			//是否啟用
			if (record.First().IsActive == false)
			{
				return Content(String.Format(slLogoutHtml, "帳號無權限...", Request.ApplicationPath));
			}

			Session.RemoveAll();
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
			  record.First().Account,//你想要存放在 User.Identy.Name 的值，通常是使用者帳號
			  DateTime.Now,
			  DateTime.Now.AddHours(12),
			  false,//將管理者登入的 Cookie 設定成 Session Cookie
			  record.First().ID.ToString(),//userdata看你想存放啥
			  FormsAuthentication.FormsCookiePath);

			string encTicket = FormsAuthentication.Encrypt(ticket);

			Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
			setUserInfoSession(record.First().UserName, record.First().Account, record.First().ID.ToString(), record.First().StatNo);
			setUserLoginInfo(record.First().UserName, record.First().Account, record.First().ID.ToString(), record.First().StatNo);
			if (Request["ReturnUrl"] == null)
				return RedirectToAction("Index", "Login");

			return Redirect(Request["ReturnUrl"]);
		}

		private void setUserInfoSession(string name, string account, string dbId, string statNo)
		{
			Session["UserLoginInfo"] = new UserLoginInfo(name, account, dbId, statNo);
		}
		private void setUserLoginInfo(string name, string account, string dbId, string statNo)
		{
			var useData = db.SYS_User.Where(x => x.Account == account && x.UserName == name).FirstOrDefault();
			useData.LastLoginTime = DateTime.Now;
			useData.LoginFrequency = (useData.LoginFrequency ?? 0) + 1;
			db.Entry(useData).State = EntityState.Modified;
			db.SaveChanges();
		}

		/// <summary>
		/// 防客戶端session timeout 
		/// </summary>
		/// <param name="refreshQuery">參數</param>       
		/// <returns></returns>
		public ActionResult getRequery(string refreshQuery)
		{
			return Content("");
		}

		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			//清除所有的 session
			Session.RemoveAll();

			//建立一個同名的 Cookie 來覆蓋原本的 Cookie
			HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
			cookie1.Expires = DateTime.Now.AddYears(-1);
			Response.Cookies.Add(cookie1);

			//建立 ASP.NET 的 Session Cookie 同樣是為了覆蓋
			HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
			cookie2.Expires = DateTime.Now.AddYears(-1);
			Response.Cookies.Add(cookie2);

			return RedirectToAction("Index", "Login");
		}

		[Authorize]
		public ActionResult GetnNaviTreeJSON()
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
						var node = new JObject {
							{ "id",childItemRecord.ID},
							{ "text",childItemRecord.Caption},
							{ "href", String.Format("./{0}/{1}", childItemRecord.Controller, childItemRecord.Action)},
							{ "icon", "jstree-file"}
					};
						//權限控管                       
						if (WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString(), "menuID", childItemRecord.ID.ToString()))
						{
							childNode.Add(node);
						}
					}

					var nodeParent = new JObject {
						{ "id",menuItemParent.ID},
						{ "text",menuItemParent.Caption},
						{ "children",childNode},
						{ "icon", "jstree-folder"}
					};
					//權限控管                    
					if (WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString(), "menuID", menuItemParent.ID.ToString()))
					{
						ja.Add(nodeParent);
					}
				}
			}
			return Content(JsonConvert.SerializeObject(ja), "application/json");
		}
	}

	public class UserLoginInfo
	{
		public string name;
		public string account;
		public string dbId;
		public string statNo;

		public UserLoginInfo(string name, string account, string dbId, string statNo)
		{
			this.name = name;
			this.account = account;
			this.dbId = dbId;
			this.statNo = statNo;
		}
	}
}