using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("BL_Report_Dtl")]
	public partial class BL_Report_Dtl
	{
		[Required]
		[StringLength(20)]
		[Column(Order = 0)]
		[Key]
		public string ReportNo { set; get; }
		[Required]
		[Column(Order = 1)]
		[Key]
		public int SNo { set; get; }

		[StringLength(20)]
		public string LadingNo { set; get; }

		[NotMapped]
		public string LadingNo_Type { set; get; }

		public int LadingSNo { set; get; }

		public DateTime CreatedDate { get; set; }

		[StringLength(50)]
		public string CreatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[StringLength(50)]
		public string UpdatedBy { get; set; }

		public DateTime? DeletedDate { get; set; }

		[StringLength(50)]
		public string DeletedBy { get; set; }

		public bool IsDelete { get; set; }

		[NotMapped]
		public string DtlBagNo { set; get; }

		[NotMapped]
		public string SendCustNo { set; get; }

		[NotMapped]
		public string SendCHName { set; get; }

		[NotMapped]
		public string SendBy { set; get; }

		[NotMapped]
		public string SendPhone { set; get; }

		[NotMapped]
		public string SendCustAddr { set; get; }

		[NotMapped]
		public string RecCompany { set; get; }

		[NotMapped]
		public string RecBy { set; get; }

		[NotMapped]
		public string RecPhone { set; get; }

		[NotMapped]
		public string RecChAddr { set; get; }

		[NotMapped]
		public string DtlProductNo { set; get; }

		[NotMapped]
		public string DtlProductName { set; get; }

		[NotMapped]
		public int DtlPcs { set; get; }

		[NotMapped]
		public decimal DtlWeight { set; get; }

		[NotMapped]
		public decimal DtlLength { set; get; }

		[NotMapped]
		public decimal DtlWidth { set; get; }

		[NotMapped]
		public decimal DtlHeight { set; get; }

		[NotMapped]
		public decimal DtlGrossWeight { set; get; }

		[NotMapped]
		public decimal Volume { set; get; }

		[NotMapped]
		public string DestNo { set; get; }

		[NotMapped]
		public string CName { set; get; }

		[NotMapped]
		public string DtlType { set; get; }

		[NotMapped]
		public decimal Cost { set; get; }

		[NotMapped]
		public decimal CostCurrency { set; get; }

		[NotMapped]
		public string CcNo { set; get; }

		[NotMapped]
		public string PayCustNo { set; get; }

		[NotMapped]
		public decimal ToPayment { set; get; }

		[NotMapped]
		public string ToPaymentCurrency { set; get; }

		[NotMapped]
		public decimal Freight { set; get; }

		[NotMapped]
		public string FreightCurrency { set; get; }

		[NotMapped]
		public string Remark { set; get; }

	}
}