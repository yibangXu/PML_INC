using HY_PML.helper;
using HY_PML.Models;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace HY_PML.Controllers
{
	public class DeclCust_SubController : Controller
	{
		private PML db = new PML();
		string slLogoutHtml = WebSiteHelper.ReturnHtmlString;

		public ActionResult GetGridJSON(DeclCust_Sub data, int page = 1, int rows = 40)
		{
			var declCustSub =
				from d in db.DeclCust_Sub
				where d.IsDelete == false
				select new
				{
					sNo = d.sNo,
					LadingNo = d.LadingNo,
					BagNo = d.BagNo,
					CleCusCode = d.CleCusCode,
					ProductName = d.ProductName,
					Type = d.Type,
					Qty = d.Qty,
					Weight = d.Weight,
					GrossWeight = d.GrossWeight,
					Price = d.Price,
					Remark = d.Remark,
					Length = d.Length,
					Width = d.Width,
					Height = d.Height,
					CreateBy = d.CreateBy,
					CreateTime = d.CreateTime,
					UpdateBy = d.UpdateBy,
					UpdateTime = d.UpdateTime,
					DeletedBy = d.DeletedBy,
					DeletedTime = d.DeletedTime,
					IsDelete = d.IsDelete,
				};

			if (data.LadingNo != null)
				declCustSub = declCustSub.Where(x => x.LadingNo == data.LadingNo);
			int records = declCustSub.Count();
			declCustSub = declCustSub.OrderBy(o => o.sNo).Skip((page - 1) * rows).Take(rows);

			var result = new ResultHelper()
			{
				Ok = DataModifyResultType.Success,
				Data = declCustSub,
				Records = records,
				Pages = page,
				TotalPage = rows <= 0 ? 1 : (records - 1) / rows + 1
			};
			return Content(JsonConvert.SerializeObject(result), "application/json");
		}
	}
}