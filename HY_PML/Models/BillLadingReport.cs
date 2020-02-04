using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HY_PML.Models
{
	public class BillLadingReport
	{
		public string LadingNo { set; get; }
		public string LadingNo_Type { set; get; }
		public string SendCustNo { set; get; }
		public string SendCHName { set; get; }
		public string SendCompany { set; get; }
		public string SendCustAddr { set; get; }
		public string SendCustAddr2 { set; get; }
		public string SendCustAddr3 { set; get; }
		public string SendBy { set; get; }
		public string SendPhone { set; get; }
		public string SendFaxNo { set; get; }
		public string SendCountry { set; get; }
		public string SendState { set; get; }
		public string SendCity { set; get; }
		public string SendPostDist { set; get; }
		public string SendInvNo { set; get; }
		public string RecCompany { set; get; }
		public string RecChAddr { set; get; }
		public string RecChAddr2 { set; get; }
		public string RecChAddr3 { set; get; }
		public string RecChAddr4 { set; get; }
		public string RecBy { set; get; }
		public string RecPhone { set; get; }
		public string RecMPhone { set; get; }
		public string RecCountry { set; get; }
		public string RecState { set; get; }
		public string RecCity { set; get; }
		public string RecPostDist { set; get; }
		public string RecInvNo { set; get; }
		public string DestNo { set; get; }
		public int Qty { set; get; }
		public string CcNo { set; get; }
		public string Type { set; get; }
		public decimal? Weight { set; get; }
		public decimal? Volume { set; get; }
		public string Currency { set; get; }
		public decimal? ToPayment { set; get; }
		public string Remark { set; get; }
		public string LadingDate { set; get; }
		public string HubNo { set; get; }
		public string HubName { set; get; }
		public string HubPName { set; get; }
		public string SStatNo { set; get; }
		public string AStatNo { set; get; }
		public string ProductName { set; get; }
		public string CocustomTyp { set; get; }
		public string SendRemark { set; get; }
		public string PrintBy { set; get; }
		public string PrintTime { set; get; }
		public int PiecesNo { set; get; }
		public string Sale { set; get; }
		public string SDate { set; get; }
		public string Sector { set; get; }
	}
}