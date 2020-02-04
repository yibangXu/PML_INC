using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class SectorAbsentController : Controller
    {
        private PML db = new PML();
        string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

        public ActionResult _ElementInForm()
        {
            return PartialView();
        }

        public ActionResult _ElementInForm2()
        {
            return PartialView();
        }

        // GET: Currency
        [Authorize]
        public ActionResult Index()
        {
			ViewBag.UserAct = WebSiteHelper.GetActionStr(this.ControllerContext.RouteData.Values["controller"].ToString());
			ViewBag.Title = "外務員休假資料";
            ViewBag.ControllerName = "";
            ViewBag.AddFunc = "";
            ViewBag.EditFunc = "";
            ViewBag.DelFunc = "";

            ViewBag.ControllerName2 = "SectorAbsent";
            ViewBag.AddFunc2 = "Add";
            ViewBag.EditFunc2 = "Edit";
            ViewBag.DelFunc2 = "Delete";
			ViewBag.FormPartialName2 = "_ElementInForm2";
			ViewBag.FormCustomJsNew2 = "$('#SectorNo').textbox('setValue',rowid);";

            //權限控管
            if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            return View();
        }

        [Authorize]
        public ActionResult Add(ORG_SectorAbsent data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            var result = new ResultHelper();

            ORG_SectorAbsent userRecord = new ORG_SectorAbsent();
            userRecord.SectorNo = data.SectorNo;
            userRecord.AgentSectorNo = data.AgentSectorNo;
            userRecord.Remark = data.Remark;

            if (data.StartDT != null)
                userRecord.StartDT = data.StartDT;
            else
                userRecord.StartDT = DateTime.Now;

            if (data.EndDT != null)
                userRecord.EndDT = data.EndDT;
            else
                userRecord.EndDT = DateTime.Now;

            if (data.BackDT != null)
                userRecord.BackDT = data.BackDT;
            else
                userRecord.BackDT = DateTime.Now;

            userRecord.IsServer = data.IsServer;

            //以下系統自填
            userRecord.CreatedDate = DateTime.Now;
            userRecord.CreatedBy = User.Identity.Name;
            userRecord.IsDelete = false;

            try
            {
                db.ORG_SectorAbsent.Add(userRecord);
                db.SaveChanges();
                result.Ok = DataModifyResultType.Success;
                result.Message = "OK";
            }
            catch (Exception e)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = e.Message;
            }
            return Content(JsonConvert.SerializeObject(result), WebSiteHelper.ResponseAjaxContentType(Request.Browser.Type));
        }

        [Authorize]
        public ActionResult Edit(ORG_SectorAbsent data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            var result = new ResultHelper();

            ORG_SectorAbsent userRecord = db.ORG_SectorAbsent.FirstOrDefault(x => x.ID == data.ID);

            if (userRecord != null)
            {
                userRecord.AgentSectorNo = data.AgentSectorNo;
                userRecord.Remark = data.Remark;

                if (data.StartDT != null)
                    userRecord.StartDT = data.StartDT;
                else
                    userRecord.StartDT = DateTime.Now;

                if (data.EndDT != null)
                    userRecord.EndDT = data.EndDT;
                else
                    userRecord.EndDT = DateTime.Now;

                if (data.BackDT != null)
                    userRecord.BackDT = data.BackDT;
                else
                    userRecord.BackDT = DateTime.Now;

                userRecord.IsServer = data.IsServer;

                //以下系統自填
                userRecord.UpdatedDate = DateTime.Now;
                userRecord.UpdatedBy = User.Identity.Name;
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
        public ActionResult Delete(ORG_SectorAbsent data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            var result = new ResultHelper();
            ORG_SectorAbsent userRecord = db.ORG_SectorAbsent.FirstOrDefault(x => x.ID == data.ID);

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
        public ActionResult GetGridJSON(ORG_SectorAbsent data, int page = 1, int rows = 40)
        {
            var sab = from o in db.ORG_SectorAbsent
                      join s in db.ORG_Sector
                      on o.AgentSectorNo equals s.SectorNo into ps
                      from s in ps.DefaultIfEmpty()
					  join u in db.SYS_User on o.CreatedBy equals u.Account into ps2
					  from u in ps2.DefaultIfEmpty()
					  where o.IsDelete == false && o.SectorNo == data.SectorNo
                      select new
                      {
                          o.ID,
                          SectorNo = o.SectorNo,
                          StartDT = o.StartDT,
                          EndDT = o.EndDT,
                          AgentSectorNo = o.AgentSectorNo,
                          AgentSectorName = s.SectorName,
                          BackDT = o.BackDT,
                          Remark = o.Remark,
                          IsServer = o.IsServer,
                          CreatedDate = o.CreatedDate,
                          CreatedBy = u.UserName,
                          UpdatedDate = o.UpdatedDate,
                          UpdatedBy = o.UpdatedBy,
                          DeletedDate = o.DeletedDate,
                          DeletedBy = o.DeletedBy,
                          IsDelete = o.IsDelete,
                      };

            var records = sab.Count();
            sab = sab.OrderBy(o => o.ID).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Message = "OK",
                Records = records,
                Pages = page,
                Data = sab,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [Authorize]
        public ActionResult GetSectorGridJSON(ORG_Sector data, int page = 1, int rows = 40)
        {
            var sector = from s in db.ORG_Sector
                         join a in db.ORG_SectorAbsent.Where(x => x.IsDelete == false && (DbFunctions.TruncateTime(x.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)) && (DbFunctions.TruncateTime(x.EndDT) >= DbFunctions.TruncateTime(DateTime.Now)))
                         on s.SectorNo equals a.SectorNo into ps
                         from a in ps.DefaultIfEmpty()
                         where s.IsDelete == false
                         select new
                         {
                             s.SectorNo,
                             s.SectorName,
                             s.IsServer,
                             s.IsLeave,
                             AgentSectorNo = a == null ? null : a.AgentSectorNo,
                             IsOff = a == null ? false : true
                         };

            if (data.SectorNo.IsNotEmpty())
                sector = sector.Where(x => x.SectorNo.Contains(data.SectorNo));
            if (data.SectorName.IsNotEmpty())
                sector = sector.Where(x => x.SectorName.Contains(data.SectorName));

            int records = sector.Count();
            sector = sector.OrderBy(x => x.SectorNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Data = sector,
                Records = records,
                Pages = page,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}