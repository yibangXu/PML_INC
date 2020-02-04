using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class DeclCust_MainController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

		public ActionResult GetGridJSON(DeclCust_Main data, int page = 1, int rows = 40)
		{
			var declCustMain =
				from d in db.DeclCust_Main
				join b in db.Bill_Lading
				on d.LadingNo equals b.LadingNo into ps
				from b in ps.DefaultIfEmpty()
				where d.IsDelete == false
				select new
				{
					sNo = d.sNo,
					LadingNo = d.LadingNo,
					LadingNo_Type = b.LadingNo_Type,
					CustType = d.CustType,
					Flight = d.Flight,
					LadNo = d.LadNo,
					BagNo = d.BagNo,
					CleCusCode = d.CleCusCode,
					CusCoode = d.CusCoode,
					ProductNo = d.ProductNo,
					ProductName = d.ProductName,
					ProductEName = d.ProductEName,
					Country = d.Country,
					Type = d.Type,
					HSNo = d.HSNo,
					Qty = d.Qty,
					Unit = d.Unit,
					GrossWeight = d.GrossWeight,
					Weight = d.Weight,
					Price = d.Price,
					Length = d.Length,
					Width = d.Width,
					Height = d.Height,
					Total = d.Total,
					Currency = d.Currency,
					Pcs = d.Pcs,
					PcsNo = d.PcsNo,
					Remark = d.Remark,
					CreateBy = d.CreateBy,
					CreateTime = d.CreateTime,
					UpdateBy = d.UpdateBy,
					UpdateTime = d.UpdateTime,
					DeletedBy = d.DeletedBy,
					DeletedTime = d.DeletedTime,
					IsDelete = d.IsDelete,
				};

			if (data.LadingNo != null)
				declCustMain = declCustMain.Where(x => x.LadingNo == data.LadingNo);
			int records = declCustMain.Count();
			declCustMain = declCustMain.OrderBy(o => o.sNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = declCustMain,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}