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
	public class Bill_Land_DtlController : Controller
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
		public ActionResult GetGridJSON(Bill_Land_Dtl data, int page = 1, int rows = 40)
		{
			var billLand =
				from d in db.Bill_Land_Dtl
				join s in db.ShdetHeader.Where(x => x.IsDelete == false)
				on new { d.ShdetNo, CustNo = d.CustomerNo } equals new { s.ShdetNo, CustNo = s.CustNo } into ps
				from s in ps.DefaultIfEmpty()
				join prod in db.ShdetProd.Where(x => x.IsDelete == false)
				on new { ShdetNo = d.ShdetNo, CustNo = d.CustomerNo, SDtlNo = d.SDtlNo, ProdNo = d.ProdNo }
				equals new { ShdetNo = prod.ShdetNo, CustNo = prod.CustNo, SDtlNo = prod.sDtlNo, ProdNo = prod.sNo } into ps1
				from prod in ps1.DefaultIfEmpty()
				join sd in db.ShdetDetail.Where(x => x.IsDelete == false)
				on new { ShdetNo = prod.ShdetNo, CustNo = prod.CustNo, SDtlNo = prod.sDtlNo }
				equals new { ShdetNo = sd.ShdetNo, CustNo = sd.CustNo, SDtlNo = sd.sNo }
				join b in db.Bill_Lading
				on d.ShdetNo equals b.LadingNo into ps2
				from b in ps2.DefaultIfEmpty()
				where d.IsDelete == false
				select new
				{
					SheetNo = d.SheetNo,
					SNo = d.SNo,
					MasterNo = d.MasterNo,
					HouseNo = d.HouseNo,
					ShdetNo = b == null ? d.ShdetNo : b.LadingNo_Type,
					CustomerNo = d.CustomerNo,
					CustCHName = s == null ? null : s.CustCHName,
					SDtlNo = d.SDtlNo,
					ProdNo = d.ProdNo,
					WriteOff = d.WriteOff,
					BagNo = d.BagNo,
					Remark = d.Remark,
					CreateBy = d.CreateBy,
					CreateTime = d.CreateTime,
					UpdateBy = d.UpdateBy,
					UpdateTime = d.UpdateTime,
					DeleteBy = d.DeleteBy,
					DeleteTime = d.DeleteTime,
					IsDelete = d.IsDelete,
					CocustomTyp = (sd.CocustomTyp == 0 ? "　" : (sd.CocustomTyp == 1 ? "正" : (sd.CocustomTyp == 2 ? "簡" : "　"))),
					Pcs = prod.Pcs,
					IsLading = s.LadingNo != null ? true : false,
				};

			if (data.SheetNo.IsNotEmpty())
				billLand = billLand.Where(x => x.SheetNo.Contains(data.SheetNo));
			if (data.HouseNo.IsNotEmpty())
				billLand = billLand.Where(x => x.HouseNo.Contains(data.HouseNo));
			if (data.ShdetNo.IsNotEmpty())
				billLand = billLand.Where(x => x.ShdetNo.Contains(data.ShdetNo));
			if (data.CustCHName.IsNotEmpty())
				billLand = billLand.Where(x => x.CustCHName.Contains(data.CustCHName));
			if (data.WriteOff.IsNotEmpty())
				billLand = billLand.Where(x => x.WriteOff.Contains(data.WriteOff));
			if (data.BagNo.IsNotEmpty())
				billLand = billLand.Where(x => x.BagNo.Contains(data.BagNo));
			if (data.Remark.IsNotEmpty())
				billLand = billLand.Where(x => x.Remark.Contains(data.Remark));

			var customOrder = new List<string> { "正", "簡", "　" };
			int records = billLand.Count();
			var billLandData = billLand.ToList().OrderBy(x => Array.IndexOf(customOrder.ToArray(), x.CocustomTyp)).ThenBy(x => x.BagNo == null ? "1" : "0").ThenBy(x => x.BagNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = billLandData,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}