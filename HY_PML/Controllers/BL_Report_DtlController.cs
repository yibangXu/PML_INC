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
	public class BL_Report_DtlController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

		[Authorize]
		public ActionResult getMultiCbjqGrid(string gridId, string formId, string dlgId, string postUrl, string param)
		{
			ViewBag.gridId = gridId;
			ViewBag.formId = formId;
			ViewBag.dlgId = dlgId;
			ViewBag.postUrl = postUrl;
			ViewBag.sheetNo = param;
			return View();
		}

		[Authorize]
		public ActionResult GetGridJSON(BL_Report_Dtl data, int page = 1, int rows = 40)
		{
			var billReportDtl =
				from br in db.BL_Report_Dtl.Where(x => x.IsDelete == false)
				join m in db.DeclCust_Main.Where(x => x.IsDelete == false)
				on new { br.LadingNo, LadingSNo = br.LadingSNo } equals new { m.LadingNo, LadingSNo = m.sNo } into ps
				from m in ps.DefaultIfEmpty()
				join b in db.Bill_Lading.Where(x => x.IsDelete == false)
				on br.LadingNo equals b.LadingNo into ps1
				from b in ps1.DefaultIfEmpty()
				join d in db.ORG_Dest.Where(x => x.IsDelete == false)
				on b.DestNo equals d.DestNo into ps2
				from d in ps2.DefaultIfEmpty()
				join bl in db.BL_Report_Mas.Where(x => x.IsDelete == false)
				on br.ReportNo equals bl.ReportNo into ps3
				from bl in ps3.DefaultIfEmpty()
				join h in db.ORG_Hub.Where(x => x.IsDelete == false)
				on bl.HubNo equals h.HubNo into ps4
				from h in ps4.DefaultIfEmpty()
				select new
				{
					ReportNo = br.ReportNo,
					SNo = br.SNo,
					DtlBagNo = m.BagNo,
					LadingNo = br.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					LadingSNo = br.LadingSNo,
					SendCHName = b.SendCHName,
					SendBy = b.SendBy,
					SendPhone = b.SendPhone,
					SendCustAddr = b.SendCustAddr,
					RecCompany = b.RecCompany,
					RecBy = b.RecBy,
					RecPhone = b.RecPhone,
					RecChAddr = b.RecChAddr,
					DtlProductName = m.ProductName,
					DtlPcs = m.Pcs == null ? 0 : m.Pcs,
					DtlWeight = m.Weight == null ? 0 : m.Weight,
					DtlLength = m.Length == null ? 0 : m.Length,
					DtlWidth = m.Width == null ? 0 : m.Width,
					DtlHeight = m.Height == null ? 0 : m.Height,
					DtlGrossWeight = m.GrossWeight == null ? 0 : m.GrossWeight,
					DestNo = d.DestNo,
					CName = d.CName,
					DtlType = m.Type,
					Cost = b.Cost == null ? 0 : b.Cost,
					CcNo = b.CcNo,
					PayCustCHName = b.PayCustCHName,
					ToPayment = b.ToPayment == null ? 0 : b.ToPayment,
					Freight = b.Freight == null ? 0 : b.Freight,
					Remark = m.Remark,
				};

			if (data.ReportNo.IsNotEmpty())
				billReportDtl = billReportDtl.Where(x => x.ReportNo.Contains(data.ReportNo));
			if (data.DtlBagNo.IsNotEmpty())
				billReportDtl = billReportDtl.Where(x => x.DtlBagNo.Contains(data.DtlBagNo));
			if (data.LadingNo.IsNotEmpty())
				billReportDtl = billReportDtl.Where(x => x.LadingNo.Contains(data.LadingNo));
			if (data.LadingNo_Type.IsNotEmpty())
				billReportDtl = billReportDtl.Where(x => x.LadingNo_Type.Contains(data.LadingNo_Type));

			int records = billReportDtl.Count();
			billReportDtl = billReportDtl.OrderByDescending(o => o.SNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = billReportDtl,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}