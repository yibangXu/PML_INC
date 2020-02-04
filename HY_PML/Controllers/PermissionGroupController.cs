using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class PermissionGroupController : Controller
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
			//頁面的抬頭
			ViewBag.Title = "群組權限";
            ViewBag.ControllerName = "PermissionGroup";

            //權限控管
            if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            return View();
        }

        [Authorize]
        public ActionResult Edit(string uid, string checkNodeId)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

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
                    }
                }
            }
            //所有權限
            MenuItemIdAry = (int[])MenuItemIdArray.ToArray(typeof(int));


            var actinfo = from ainfo in db.SYS_ActInfo
                          where MenuItemIdAry.Contains(ainfo.ActionID)
                          select ainfo;

            var actinfoList = actinfo.Select(x => x.ID).ToList();

            //find the user  actuser record
            var actGroupuser = from auser in db.SYS_ActUserGroup
                               where auser.UserGroupNo == uid
                               select auser;

            var groupUser = from auser in db.SYS_User
                            where auser.UserGroupNo == uid
                            select auser;

            //把權限回寫到個人權限頁面
            foreach (var item in groupUser)
            {
                var user = db.SYS_ActUser.Where(x => x.UserID == item.ID);
                var count = user.Count();
                if (user.Count() > 0)
                {
                    var userId = user.FirstOrDefault().UserID;
                    foreach (var i in user)
                    {
                        i.IsDelete = true;
                        db.Entry(i).State = EntityState.Modified;
                    }

                    foreach (var act in actinfoList)
                    {
                        var action = user.FirstOrDefault(x => x.ActID == act);
                        if (action != null)
                        {
                            action.IsDelete = false;
                            action.ActionType = true;
                            db.Entry(action).State = EntityState.Modified;
                        }
                        else
                        {
                            var newAction = new SYS_ActUser()
                            {
                                ActID = Convert.ToInt16(act),
                                UserID = Convert.ToInt16(userId),
                                ActionType = true,
                                CreatedDate = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                IsDelete = false
                            };
                            db.SYS_ActUser.Add(newAction);
                            db.Entry(newAction).State = EntityState.Added;
                        }
                    }
                }
                else
                {
                    var userId = db.SYS_User.FirstOrDefault(x => x.ID == item.ID).ID;
                    var newUserAct = new List<SYS_ActUser>();

                    foreach (var i in actinfoList)
                    {
                        var temp = new SYS_ActUser()
                        {
                            ActID = Convert.ToInt16(i),
                            UserID = Convert.ToInt16(userId),
                            ActionType = true,
                            CreatedDate = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            IsDelete = false
                        };
                        newUserAct.Add(temp);
                    }

                    db.SYS_ActUser.AddRange(newUserAct);
                }
            }

            ArrayList ActUserActIdArray = new ArrayList();
            foreach (var item in actGroupuser)
            {
                ActUserActIdArray.Add(int.Parse(item.ActID.ToString()));
            }
            int[] ActUserActIdAry = (int[])ActUserActIdArray.ToArray(typeof(int));
            //actinfo turn arraylist
            ArrayList ActInfoIdArray = new ArrayList();
            foreach (var item in actinfo)
            {
                ActInfoIdArray.Add(item.ID);
            }

            DateTime dtNowStamp = DateTime.Now;

            //先全部的actuserGroup的 isdelete 設 true 
            foreach (var item in actGroupuser)
            {
                item.IsDelete = true;
                db.Entry(item).State = EntityState.Modified;
            }

            //看看 actinfo  是否有在 actuser中
            foreach (var item in ActInfoIdArray)
            {
                //有， 設 isdelete為 false
                if (ActUserActIdAry.Contains((int)item))
                {
                    foreach (var item2 in actGroupuser)
                    {
                        if ((int)item2.ActID == (int)item)
                        {
                            item2.IsDelete = false;
                            db.Entry(item2).State = EntityState.Modified;
                            break;
                        }
                    }

                }
                else  //沒有， 新增actuserGroup , isdelete false
                {
                    SYS_ActUserGroup newActUserGroup = new SYS_ActUserGroup();
                    newActUserGroup.ActID = short.Parse(item.ToString());
                    newActUserGroup.UserGroupNo = uid;
                    newActUserGroup.CreatedDate = dtNowStamp;
                    newActUserGroup.CreatedBy = User.Identity.Name;
                    newActUserGroup.IsDelete = false;
                    db.SYS_ActUserGroup.Add(newActUserGroup);
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
        public ActionResult getPermissionJson(string userGroupNo = "")
        {
            if (userGroupNo == "")
                return Content("no data");

            DataTable dtPermission = WebSiteHelper.MyUserGroupActionInfo(userGroupNo);
            JArray ja = new JArray();
            int nlTargetColIndex = 0;
            int nlParentColIndex = 0;
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
                    else
                    {
                        ja.Add(item);
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(ja), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult GetGridJSON(SYS_UserGroup data, int page = 1, int rows = 40)
        {
            var userGroups = db.SYS_UserGroup.Where(o => o.IsDelete == false && o.IsActive == true);

            if (data.UserGroupNo.IsNotEmpty())
                userGroups = userGroups.Where(x => x.UserGroupNo.Contains(data.UserGroupNo));
            if (data.UserGroupName.IsNotEmpty())
                userGroups = userGroups.Where(x => x.UserGroupName.Contains(data.UserGroupName));
            if (data.Remark.IsNotEmpty())
                userGroups = userGroups.Where(x => x.Remark.Contains(data.Remark));

            var records = userGroups.Count();
            userGroups = userGroups.OrderBy(o => o.UserGroupNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Data = userGroups,
                Records = records,
                Pages = page,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}