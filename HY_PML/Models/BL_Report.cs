using System;
using System.Collections.Generic;

namespace HY_PML.Models
{
	public class BL_Report
	{
		public string ReportNo { set; get; }
		public string MasterNo { set; get; }
		public string FlightNo { set; get; }
		public string HubNo { set; get; }
		public string HubCode { set; get; }
		public string HubName { set; get; }
		public int ReportID { set; get; }
		public string ReportCName { set; get; }
		public string SStatNo { set; get; }
		public string SStatName { set; get; }
		public string AStatNo { set; get; }
		public string AStatName { set; get; }
		public DateTime CreatedDate { get; set; }
		public List<BL_Report_Items> Items { set; get; }
	}

	public class BL_Report_Items
	{
		public string ReportNo { set; get; }

		public int SNo { set; get; }

		public string LadingNo { set; get; }

		public int LadingSNo { set; get; }

		public string DtlBagNo { set; get; }

		public string SendCustNo { set; get; }

		public string SendCHName { set; get; }

		public string SendEName1 { set; get; }

		public string SendBy { set; get; }

		public string SendPhone { set; get; }

		public string SendCustAddr { set; get; }

		public string SendENAddr1 { set; get; }

		public string RecCompany { set; get; }

		public string RecBy { set; get; }

		public string RecPhone { set; get; }

		public string RecChAddr { set; get; }

		public string RecPostDist { set; get; }

		public string RecCity { set; get; }

		public string RecState { set; get; }

		public decimal Volume { set; get; }

		public string DestNo { set; get; }

		public string CName { set; get; }

		public string DtlType { set; get; }

		public decimal Cost { set; get; }

		public decimal CostCurrency { set; get; }

		public string CcNo { set; get; }

		public string PayCustNo { set; get; }

		public string PayCustCHName { set; get; }

		public decimal ToPayment { set; get; }

		public string ToPaymentCurrency { set; get; }

		public decimal Freight { set; get; }

		public string FreightCurrency { set; get; }

		public string Type { set; get; }

		public string sStatNo { set; get; }

		public string aStatNo { set; get; }

		public string Remark { set; get; }

		public string Remark2 { set; get; }

		public string DtlProductNo { set; get; }

		public string DtlProductName { set; get; }

		public int DtlPcs { set; get; }

		public decimal DtlQty { set; get; }

		public decimal DtlPrice { set; get; }

		public decimal DtlWeight { set; get; }

		public decimal DtlLength { set; get; }

		public decimal DtlWidth { set; get; }

		public decimal DtlHeight { set; get; }

		public string DtlHSNo { set; get; }

		public decimal DtlGrossWeight { set; get; }
	}
}