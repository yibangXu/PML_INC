using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class CompanyController : Controller
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
			ViewBag.Title = " 公司主檔 ";
            ViewBag.ControllerName = "Company";

            //權限控管
            if (!WebSiteHelper.IsPermissioned(this.ControllerContext.RouteData.Values["action"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));
            return View();
        }

        [Authorize]
        public ActionResult Add(ORG_Company data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            ResultHelper result = new ResultHelper();
            bool duplicated = db.ORG_Company.Any(x => x.CompanyNo == data.CompanyNo);
            if (duplicated)
            {
                result.Ok = DataModifyResultType.Faild;
                result.Message = "已存在重複的公司代碼";
            }
            else
            {
                ORG_Company userRecord = new ORG_Company();

                userRecord.CompanyNo = data.CompanyNo;
                userRecord.CompanyName = data.CompanyName;
                userRecord.CompanyEName = data.CompanyEName;
                userRecord.GuiNa = data.GuiNa;
                userRecord.GuiNo = data.GuiNo;
                userRecord.TaxNo = data.TaxNo;
                userRecord.Tel = data.Tel;
                userRecord.Fax = data.Fax;
                userRecord.CAddr = data.CAddr;
                userRecord.EAddr = data.EAddr;
                userRecord.GuiAddr = data.GuiAddr;
                userRecord.Url = data.Url;
                userRecord.NetWork = data.NetWork;
                userRecord.Email = data.Email;
                userRecord.Ip = data.Ip;
                userRecord.IpBak = data.IpBak;

                //以下系統自填
                userRecord.CreatedDate = DateTime.Now;
                userRecord.CreatedBy = User.Identity.Name;
                userRecord.IsDelete = false;

                try
                {
                    db.ORG_Company.Add(userRecord);
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
        public ActionResult Edit(ORG_Company data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            ResultHelper result = new ResultHelper();
            ORG_Company userRecord = db.ORG_Company
                .FirstOrDefault(x => x.ID == data.ID);

            if (userRecord != null)
            {
                userRecord.CompanyNo = data.CompanyNo;
                userRecord.CompanyName = data.CompanyName;
                userRecord.CompanyEName = data.CompanyEName;
                userRecord.GuiNa = data.GuiNa;
                userRecord.GuiNo = data.GuiNo;
                userRecord.TaxNo = data.TaxNo;
                userRecord.Tel = data.Tel;
                userRecord.Fax = data.Fax;
                userRecord.CAddr = data.CAddr;
                userRecord.EAddr = data.EAddr;
                userRecord.GuiAddr = data.GuiAddr;
                userRecord.Url = data.Url;
                userRecord.NetWork = data.NetWork;
                userRecord.Email = data.Email;
                userRecord.Ip = data.Ip;
                userRecord.IpBak = data.IpBak;

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
        public ActionResult Delete(ORG_Company data)
        {
            //權限控管
            if (!WebSiteHelper.IsPermissioned("Index", this.ControllerContext.RouteData.Values["controller"].ToString()))
                return Content(String.Format(slLogoutHtml, "無權限...", Request.ApplicationPath));

            ResultHelper result = new ResultHelper();
            ORG_Company userRecord = db.ORG_Company.FirstOrDefault(x => x.ID == data.ID);
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
        public ActionResult GetGridJSON(ORG_Company data, int page = 1, int rows = 40)
        {
            var company = db.ORG_Company.Where(x => x.IsDelete == false);

            if (data.CompanyNo.IsNotEmpty())
                company = company.Where(x => x.CompanyNo.Contains(data.CompanyNo));
            if (data.CompanyName.IsNotEmpty())
                company = company.Where(x => x.CompanyName.Contains(data.CompanyName));
            if (data.CompanyEName.IsNotEmpty())
                company = company.Where(x => x.CompanyEName.Contains(data.CompanyEName));
            if (data.GuiNa.IsNotEmpty())
                company = company.Where(x => x.GuiNa.Contains(data.GuiNa));
            if (data.GuiNo.IsNotEmpty())
                company = company.Where(x => x.GuiNo.Contains(data.GuiNo));
            if (data.TaxNo.IsNotEmpty())
                company = company.Where(x => x.TaxNo.Contains(data.TaxNo));
            if (data.Tel.IsNotEmpty())
                company = company.Where(x => x.Tel.Contains(data.Tel));
            if (data.Fax.IsNotEmpty())
                company = company.Where(x => x.Fax.Contains(data.Fax));
            if (data.CAddr.IsNotEmpty())
                company = company.Where(x => x.CAddr.Contains(data.CAddr));
            if (data.EAddr.IsNotEmpty())
                company = company.Where(x => x.EAddr.Contains(data.EAddr));
            if (data.GuiAddr.IsNotEmpty())
                company = company.Where(x => x.GuiAddr.Contains(data.GuiAddr));
            if (data.Url.IsNotEmpty())
                company = company.Where(x => x.Url.Contains(data.Url));
            if (data.NetWork.IsNotEmpty())
                company = company.Where(x => x.NetWork.Contains(data.NetWork));
            if (data.Email.IsNotEmpty())
                company = company.Where(x => x.Email.Contains(data.Email));
            if (data.Ip.IsNotEmpty())
                company = company.Where(x => x.Ip.Contains(data.Ip));
            if (data.IpBak.IsNotEmpty())
                company = company.Where(x => x.IpBak.Contains(data.IpBak));

            int records = company.Count();
            company = company.OrderBy(x => x.CompanyNo).Skip((page - 1) * rows).Take(rows);

            var result = new ResultHelper()
            {
                Ok = DataModifyResultType.Success,
                Message = "OK",
                Records = records,
                Pages = page,
                Data = company,
                TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
            };
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}