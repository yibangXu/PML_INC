using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class UserController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

		public ActionResult _ElementInForm()
		{
			return PartialView();
		}

		// GET: User
		[Authorize]
		public ActionResult Index()
		{
			//頁面的抬頭
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "用戶管理";
			ViewBag.ControllerName = "User";
			ViewBag.FormPartialName = "_ElementInForm";
			ViewBag.FormCustomJsEdit = "$('#UserNo').textbox('readonly')";

			//權限控管
			if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
			return View();
		}

		[Authorize]
		public ActionResult Add(SYS_User data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var duplicated = db.SYS_User.Any(x => x.Account == data.Account && x.IsDelete == false);
			if (duplicated)
			{
				result.Ok = DataModifyResultType.Faild;
				result.Message = "已存在重複的帳號代碼";
			}
			else
			{
				using (var trans = db.Database.BeginTransaction())
				{
					//新增使用者
					SYS_User userRecord = new SYS_User();
					userRecord.Account = data.Account;
					userRecord.UserName = data.UserName;
					userRecord.Password = System.Text.Encoding.Default.GetBytes(data.PasswordStr);
					userRecord.RegisterDate = data.RegisterDate;
					userRecord.ActiveDate = data.ActiveDate;
					userRecord.ExpiryDate = data.ExpiryDate;
					userRecord.SecurityQuestion = data.SecurityQuestion;
					userRecord.SecurityAnswer = data.SecurityAnswer;
					userRecord.StatNo = data.StatNo;
					userRecord.DepartNo = data.DepartNo;
					userRecord.UserGroupNo = data.UserGroupNo;
					userRecord.Remark = data.Remark;
					userRecord.IsActive = data.IsActive;

					//以下系統自填
					userRecord.CreatedDate = DateTime.Now;
					userRecord.CreatedBy = User.Identity.Name;
					userRecord.IsDelete = false;
					db.SYS_User.Add(userRecord);
					db.SaveChanges();

					//取得群組權限ActID
					var actID = db.SYS_ActUserGroup.Where(x => x.UserGroupNo == data.UserGroupNo && x.IsDelete == false).Select(x => x.ActID).ToList();
					var userID = db.SYS_User.Where(x => x.Account == data.Account).Select(x => x.ID).FirstOrDefault().ToString();
					var addActUserData = new List<SYS_ActUser>();
					foreach (var a in actID)
					{
						var addData = new SYS_ActUser()
						{
							ActID = a,
							UserID = Int16.Parse(userID),
							ActionType = true,
							CreatedDate = DateTime.Now,
							CreatedBy = User.Identity.Name,
							IsDelete = false,
						};
						addActUserData.Add(addData);
					}
					db.SYS_ActUser.AddRange(addActUserData);
					db.SaveChanges();

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
			}
			return Content(JsonConvert.SerializeObject(result),
				WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Edit(SYS_User data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index",
				this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			using (var trans = db.Database.BeginTransaction())
			{
				SYS_User userRecord = db.SYS_User.FirstOrDefault(x => x.ID == data.ID); ;

				if (userRecord != null)
				{
					userRecord.Account = data.Account;
					userRecord.UserName = data.UserName;
					if (data.PasswordStr != null)
						userRecord.Password = System.Text.Encoding.Default.GetBytes(data.PasswordStr);
					userRecord.RegisterDate = data.RegisterDate;
					userRecord.ActiveDate = data.ActiveDate;
					userRecord.ExpiryDate = data.ExpiryDate;
					userRecord.SecurityQuestion = data.SecurityQuestion;
					userRecord.SecurityAnswer = data.SecurityAnswer;
					userRecord.StatNo = data.StatNo;
					userRecord.DepartNo = data.DepartNo;
					if (userRecord.UserGroupNo != data.UserGroupNo)
					{
						var actID = db.SYS_ActUserGroup.Where(x => x.UserGroupNo == data.UserGroupNo && x.IsDelete == false).Select(x => x.ActID).ToList();
						var userID = db.SYS_User.Where(x => x.Account == data.Account).Select(x => x.ID).FirstOrDefault();
						var originActID = db.SYS_ActUser.Where(x => x.UserID == userID && x.IsDelete == false).Select(x => x.ActID).ToList();
						foreach(var o in originActID)
						{
							var oData=db.SYS_ActUser.Where(x => x.UserID == userID && x.ActID == o).FirstOrDefault();
							oData.IsDelete = true;
							oData.DeletedDate = DateTime.Now;
							oData.DeletedBy = User.Identity.Name;
							db.Entry(oData).State = EntityState.Modified;
							db.SaveChanges();
						}
						foreach(var a in actID)
						{
							var addActUserData = new List<SYS_ActUser>();
							var aData = db.SYS_ActUser.Where(x => x.UserID == userID && x.ActID == a).FirstOrDefault();
							if (aData != null)
							{
								aData.UpdatedDate = DateTime.Now;
								aData.UpdatedBy = User.Identity.Name;
								aData.IsDelete = false;
								aData.DeletedDate = null;
								aData.DeletedBy = null;
								db.Entry(aData).State = EntityState.Modified;
								db.SaveChanges();
							}
							else
							{
								var addData = new SYS_ActUser()
								{
									ActID = a,
									UserID = Convert.ToInt16(userID),
									ActionType = true,
									CreatedDate = DateTime.Now,
									CreatedBy = User.Identity.Name,
									IsDelete = false,
								};
								addActUserData.Add(addData);
							}
							db.SYS_ActUser.AddRange(addActUserData);
							db.SaveChanges();
						}
					}
					userRecord.UserGroupNo = data.UserGroupNo;
					userRecord.Remark = data.Remark;
					userRecord.IsActive = data.IsActive;

					//以下系統自填
					userRecord.UpdatedDate = DateTime.Now;
					userRecord.UpdatedBy = User.Identity.Name;

					db.Entry(userRecord).State = EntityState.Modified;
					db.SaveChanges();
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
				else
				{
					trans.Rollback();
					result.Ok = DataModifyResultType.Faild;
					result.Message = "找不到資料!";
				}
			}
			return Content(JsonConvert.SerializeObject(result),
				WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
		}

		[Authorize]
		public ActionResult Delete(SYS_User data)
		{
			//權限控管
			if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
				return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

			var result = new ResultHelper();
			var userRecord = db.SYS_User.FirstOrDefault(x => x.ID == data.ID);

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
							o.SecurityAnswer,
							o.SecurityQuestion,
							o.CreatedBy,
							o.CreatedDate,
							o.UpdatedBy,
							o.UpdatedDate,
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
							o.Remark,
							o.LastLoginTime,
							LoginFrequency = o.LoginFrequency ?? 0
						};

			if (data.Account.IsNotEmpty())
				users = users.Where(x => x.Account.Contains(data.Account));
			if (data.UserName.IsNotEmpty())
				users = users.Where(x => x.UserName.Contains(data.UserName));
			if (data.StatName.IsNotEmpty())
				users = users.Where(x => x.StatName.Contains(data.StatName));
			if (data.DepartNo.IsNotEmpty())
				users = users.Where(x => x.DepartNo.Contains(data.DepartNo));
			if (data.DepartName.IsNotEmpty())
				users = users.Where(x => x.DepartName.Contains(data.DepartName));
			if (data.UserGroupNo.IsNotEmpty())
				users = users.Where(x => x.UserGroupNo == data.UserGroupNo);
			if (data.UserGroupName.IsNotEmpty())
				users = users.Where(x => x.UserGroupName.Contains(data.UserGroupName));
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