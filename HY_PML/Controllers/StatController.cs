using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class StatController : Controller
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
			ViewBag.Title = "站點資料";
            ViewBag.ControllerName = "Stat";
            ViewBag.FormCustomJsEdit = "$('#StatNo').textbox('readonly')";

            //權限控管
            if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
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
        public ActionResult Add(ORG_Stat data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();

            var duplicated = db.ORG_Stat.Any(x => x.StatNo == data.StatNo);
            if (duplicated)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "已存在重複的代號";
            }
            else
            {
                var userRecord = new ORG_Stat();
                userRecord.StatNo = data.StatNo;
                userRecord.StatName = data.StatName;
                userRecord.SendDirector = data.SendDirector;
                userRecord.SendTel = data.SendTel;
                userRecord.SendDirectorHand = data.SendDirectorHand;
                userRecord.SendFax = data.SendFax;
                userRecord.Tel = data.Tel;
                userRecord.SendTime = data.SendTime;
                userRecord.Remark = data.Remark;
                userRecord.SendArea = data.SendArea;
                userRecord.SendAreaNo = data.SendAreaNo;
                userRecord.List = data.List;

                userRecord.Stattype = data.Stattype; ;
                userRecord.AreaID = data.AreaID;
                userRecord.CenterID = data.CenterID;
                userRecord.DestID = data.DestID;
                userRecord.StorageID = data.StorageID;
                userRecord.CurrencyID = data.CurrencyID;
                userRecord.N1 = data.N1;
                userRecord.N2 = data.N2;
                userRecord.N3 = data.N3;
                userRecord.CccCharge = data.CccCharge;
                userRecord.CcodCharge = data.CcodCharge;

                userRecord.Isnetwork = data.Isnetwork;
                userRecord.IsWww = data.IsWww;
                userRecord.IsCod = data.IsCod;
                userRecord.IsReturn = data.IsReturn;
                userRecord.IsToday = data.IsToday;
                userRecord.IsCc = data.IsCc;
                userRecord.IsServer = data.IsServer;

                //以下系統自填
                userRecord.CreatedDate = DateTime.Now;
                userRecord.CreatedBy = User.Identity.Name;
                userRecord.IsDelete = false;

                try
                {
                    db.ORG_Stat.Add(userRecord);

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
        public ActionResult Edit(ORG_Stat data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            var userRecord = db.ORG_Stat.FirstOrDefault(x => x.ID == data.ID);

            if (userRecord != null)
            {
                userRecord.StatName = data.StatName;
                userRecord.SendDirector = data.SendDirector;
                userRecord.SendTel = data.SendTel;
                userRecord.SendDirectorHand = data.SendDirectorHand;
                userRecord.SendFax = data.SendFax;
                userRecord.Tel = data.Tel;
                userRecord.SendTime = data.SendTime;
                userRecord.Remark = data.Remark;
                userRecord.SendArea = data.SendArea;
                userRecord.SendAreaNo = data.SendAreaNo;
                userRecord.List = data.List;

                userRecord.Stattype = data.Stattype; ;
                userRecord.AreaID = data.AreaID;
                userRecord.CenterID = data.CenterID;
                userRecord.DestID = data.DestID;
                userRecord.StorageID = data.StorageID;
                userRecord.CurrencyID = data.CurrencyID;
                userRecord.N1 = data.N1;
                userRecord.N2 = data.N2;
                userRecord.N3 = data.N3;
                userRecord.CccCharge = data.CccCharge;
                userRecord.CcodCharge = data.CcodCharge;

                userRecord.Isnetwork = data.Isnetwork;
                userRecord.IsWww = data.IsWww;
                userRecord.IsCod = data.IsCod;
                userRecord.IsReturn = data.IsReturn;
                userRecord.IsToday = data.IsToday;
                userRecord.IsCc = data.IsCc;
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
        public ActionResult Delete(ORG_Stat data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            var result = new ResultHelper();
            var userRecord = db.ORG_Stat.FirstOrDefault(x => x.ID == data.ID);

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
        public ActionResult GetGridJSON(ORG_Stat data, int page = 1, int rows = 40)
        {
            var stat = from s in db.ORG_Stat
                       join a in db.ORG_Area
                       on s.AreaID equals a.ID into ps
                       from a in ps.DefaultIfEmpty()
                       join c in db.ORG_Currency
                       on s.CurrencyID equals c.ID into ps2
                       from c in ps2.DefaultIfEmpty()
                       join d in db.ORG_Dest
                       on s.DestID equals d.ID into ps3
                       from d in ps3.DefaultIfEmpty()
					   join u in db.SYS_User
					   on s.CreatedBy equals u.Account into ps4
					   from u in ps4.DefaultIfEmpty()
                       where s.IsDelete == false
                       select new
                       {
                           s.ID,
                           s.StatNo,
                           s.StatName,
                           s.Stattype,
                           s.AreaID,
                           AreaNo = a == null ? null : a.AreaNo,
                           s.CenterID,
                           s.DestID,
                           DestNo = d == null ? null : d.DestNo,
                           s.N1,
                           s.N2,
                           s.N3,
                           s.Isnetwork,
                           s.SendDirector,
                           s.SendTel,
                           s.SendDirectorHand,
                           s.SendFax,
                           s.Tel,
                           s.StorageID,
                           s.SendTime,
                           s.IsWww,
                           s.IsCod,
                           s.IsReturn,
                           s.IsToday,
                           s.IsCc,
                           s.CccCharge,
                           s.CcodCharge,
                           s.CurrencyID,
                           CurrencyNo = c == null ? null : c.CurrencyNo,
                           s.Remark,
                           s.SendArea,
                           s.SendAreaNo,
                           s.List,
                           s.IsServer,
                           CreatedBy = u.UserName,
                           s.CreatedDate,
                           s.UpdatedBy,
                           s.UpdatedDate,
                           s.DeletedBy,
                           s.DeletedDate,
                           s.IsDelete
                       };

            if (data.StatNo.IsNotEmpty())
                stat = stat.Where(x => x.StatNo.Contains(data.StatNo));
            if (data.StatName.IsNotEmpty())
                stat = stat.Where(x => x.StatName.Contains(data.StatNo));
            if (data.SendDirector.IsNotEmpty())
                stat = stat.Where(x => x.SendDirector.Contains(data.SendDirector));
            if (data.SendTel.IsNotEmpty())
                stat = stat.Where(x => x.SendTel.Contains(data.SendTel));
            if (data.SendDirectorHand.IsNotEmpty())
                stat = stat.Where(x => x.SendDirectorHand.Contains(data.SendDirectorHand));
            if (data.SendFax.IsNotEmpty())
                stat = stat.Where(x => x.SendFax.Contains(data.SendFax));
            if (data.Tel.IsNotEmpty())
                stat = stat.Where(x => x.Tel.Contains(data.Tel));
            if (data.SendTime.IsNotEmpty())
                stat = stat.Where(x => x.SendTime.Contains(data.SendTime));
            if ((data.Isnetwork == false && Request["Isnetwork"] == "false") || data.Isnetwork == true)
                stat = stat.Where(x => x.Isnetwork == data.Isnetwork);
            if ((data.IsWww == false && Request["IsWww"] == "false") || data.IsWww == true)
                stat = stat.Where(x => x.IsWww == data.IsWww);
            if ((data.IsCod == false && Request["IsCod"] == "false") || data.IsCod == true)
                stat = stat.Where(x => x.IsCod == data.IsCod);
            if ((data.IsReturn == false && Request["IsReturn"] == "false") || data.IsReturn == true)
                stat = stat.Where(x => x.IsReturn == data.IsReturn);
            if ((data.IsToday == false && Request["IsToday"] == "false") || data.IsToday == true)
                stat = stat.Where(x => x.IsToday == data.IsToday);
            if ((data.IsCc == false && Request["IsCc"] == "false") || data.IsCc == true)
                stat = stat.Where(x => x.IsCc == data.IsCc);
            if ((data.IsServer == false && Request["IsServer"] == "false") || data.IsServer == true)
                stat = stat.Where(x => x.IsServer == data.IsServer);
            if (data.AreaNo.IsNotEmpty())
                stat = stat.Where(x => x.AreaNo.Contains(data.AreaNo));
            if (data.DestNo.IsNotEmpty())
                stat = stat.Where(x => x.DestNo.Contains(data.DestNo));
            if (data.CurrencyNo.IsNotEmpty())
                stat = stat.Where(x => x.CurrencyNo.Contains(data.CurrencyNo));

            var records = stat.Count();
            stat = stat.OrderBy(x => x.StatNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Message = "OK",
                Records = records,
                Pages = page,
                Data = stat,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [Authorize]
        public ActionResult GetDropdownList(ORG_Stat data, int page = 1, int rows = 40)
        {
            var statNoList = new List<string>();
            var statNoSession = ((UserLoginInfo)Session["UserLoginInfo"]).statNo;
            if (User.Identity.Name != "hyAdmin" && (statNoSession != "hyAdmin") && (statNoSession != "TNNCON"))
            {
                if (statNoSession != "" && statNoSession != null)
                {
                    statNoList.Add(statNoSession);
                }
            }
            else
            {
                statNoList = db.ORG_Stat.Where(x => x.IsDelete == false).Select(x => x.StatNo).ToList();
                statNoList.Add("");
                statNoList.Add(null);
            }

            var stat = db.ORG_Stat.Where(x => x.IsDelete == false).Select(s => new 
            {
                ID = s.ID,
                StatNo = s.StatNo,
                StatName = s.StatName,
                Stattype = s.Stattype,
                AreaID = s.AreaID,
                CenterID = s.CenterID,
                DestID = s.DestID,
                N1 = s.N1,
                N2 = s.N2,
                N3 = s.N3,
                Isnetwork = s.Isnetwork,
                SendDirector = s.SendDirector,
                SendTel = s.SendTel,
                SendDirectorHand = s.SendDirectorHand,
                SendFax = s.SendFax,
                Tel = s.Tel,
                StorageID = s.StorageID,
                SendTime = s.SendTime,
                IsWww = s.IsWww,
                IsCod = s.IsCod,
                IsReturn = s.IsReturn,
                IsToday = s.IsToday,
                IsCc = s.IsCc,
                CccCharge = s.CccCharge,
                CcodCharge = s.CcodCharge,
                CurrencyID = s.CurrencyID,
                Remark = s.Remark,
                SendArea = s.SendArea,
                SendAreaNo = s.SendAreaNo,
                List = s.List,
                IsServer = s.IsServer,
                CreatedBy = s.CreatedBy,
                CreatedDate = s.CreatedDate,
                UpdatedBy = s.UpdatedBy,
                UpdatedDate = s.UpdatedDate,
                DeletedBy = s.DeletedBy,
                DeletedDate = s.DeletedDate,
                IsDelete = s.IsDelete,
            });

            stat = stat.Where(x => statNoList.Contains(x.StatNo));

            var records = stat.Count();
            stat = stat.OrderBy(x => x.StatNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Message = "OK",
                Records = records,
                Pages = page,
                Data = stat,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}