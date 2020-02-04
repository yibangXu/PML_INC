using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("Export_Bill")]
	public partial class Export_Bill
	{
		[Required]
		[StringLength(30)]
		[Key]
		public string ExBillNo { set; get; }

		[StringLength(20)]
		public string LadingNo { set; get; }

		public DateTime? LadingDate { set; get; }

		[StringLength(10)]
		public string StatNo { set; get; }

		[StringLength(20)]
		public string StatName { set; get; }

		[StringLength(20)]
		public string CustNo { set; get; }

		[StringLength(60)]
		public string CHName { set; get; }

		[StringLength(10)]
		public string CcNo { set; get; }

		[StringLength(8)]
		public string DestNo { set; get; }

		[StringLength(50)]
		public string CName { set; get; }

		[StringLength(5)]
		public string Type { set; get; }

		public int PiecesNo { set; get; }

		public decimal? Weight { set; get; }

		public decimal? Freight { set; get; }

		public decimal? CustomsPay { set; get; }

		public decimal? Tariff { set; get; }

		public decimal? ProdIdPay { set; get; }

		public decimal? InsurancePay { set; get; }

		public decimal? OtherPayTax { set; get; }

		public decimal? OtherPayNoTax { set; get; }

		public decimal? ToPayment { set; get; }

		[StringLength(3)]
		public string ToPaymentCurrency { set; get; }

		public decimal? AgentPay { set; get; }

		[StringLength(3)]
		public string AgentPayCurrency { set; get; }

		[StringLength(200)]
		public string Remark { set; get; }

		public decimal? Total { set; get; }

		public bool? IsFinance { set; get; }

		[StringLength(8)]
		public string FinanceBy { set; get; }

		public DateTime? FinanceTime { set; get; }

		[Required]
		[StringLength(8)]
		public string CreateBy { set; get; }

		[Required]
		public DateTime CreateTime { set; get; }

		[StringLength(8)]
		public string UpdateBy { set; get; }

		public DateTime? UpdateTime { set; get; }

		[StringLength(8)]
		public string DeletedBy { set; get; }

		public DateTime? DeletedTime { set; get; }

		[Required]
		public bool IsDelete { set; get; }

		public string WATERNO { set; get; }

		[NotMapped]
		public string LadingNo_Type { set; get; }
	}
}