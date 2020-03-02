using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("Bill_Lading")]
	public partial class Bill_Lading
	{
		[Required]
		[StringLength(30)]
		[Key]
		public string LadingNo { set; get; }

		[StringLength(30)]
		public string LadingNo_Type { set; get; }

		[StringLength(30)]
		public string LadingNo_Batch { set; get; }

		public DateTime? LadingDate { set; get; }

		[StringLength(20)]
		public string WarehouseRNo { set; get; }

		public DateTime? WarehouseRDate { set; get; }

		[StringLength(10)]
		public string HubNo { set; get; }

		[StringLength(20)]
		public string TransferNo { set; get; }

		[StringLength(20)]
		public string OrderNo { set; get; }

		[StringLength(20)]
		public string Sale { set; get; }

		[StringLength(60)]
		public string SalePhone { set; get; }

		[StringLength(60)]
		public string CreatePhone { set; get; }

		[StringLength(20)]
		public string SendCustNo { set; get; }

		[StringLength(100)]
		public string SendCHName { set; get; }

		[StringLength(10)]
		public string SendCustLevel { set; get; }

		[StringLength(60)]
		public string SendPhone { set; get; }

		[StringLength(60)]
		public string SendFaxNo { set; get; }

		[StringLength(60)]
		public string SendBy { set; get; }

		[StringLength(100)]
		public string SendCompany { set; get; }

		[StringLength(200)]
		public string SendCustAddr { set; get; }

		[StringLength(30)]
		public string SendInvNo { set; get; }

		[StringLength(30)]
		public string SendCountry { set; get; }

		[StringLength(30)]
		public string SendCity { set; get; }

		[StringLength(30)]
		public string SendState { set; get; }

		[StringLength(6)]
		public string SendPostDist { set; get; }

		[StringLength(60)]
		public string SendEBy { set; get; }

		[StringLength(150)]
		public string SendECompany { set; get; }

		[StringLength(200)]
		public string SendECustAddr { set; get; }

		[StringLength(30)]
		public string SendEInvNo { set; get; }

		[StringLength(30)]
		public string SendECountry { set; get; }

		[StringLength(30)]
		public string SendECity { set; get; }

		[StringLength(30)]
		public string SendEState { set; get; }

		[StringLength(6)]
		public string SendEPostDist { set; get; }

		[StringLength(200)]
		public string SendRemark { set; get; }

		[StringLength(60)]
		public string RecPhone { set; get; }

		[StringLength(60)]
		public string RecMPhone { set; get; }

		[StringLength(8)]
		public string DestNo { set; get; }

		[StringLength(50)]
		public string CName { set; get; }

		[StringLength(5)]
		public string Type { set; get; }

		public int? CocustomTyp { get; set; }

		[StringLength(100)]
		public string RecBy { set; get; }

		[StringLength(100)]
		public string RecCompany { set; get; }

		[StringLength(200)]
		public string RecChAddr { set; get; }

		[StringLength(30)]
		public string RecInvNo { set; get; }

		[StringLength(30)]
		public string RecCountry { set; get; }

		[StringLength(30)]
		public string RecCity { set; get; }

		[StringLength(30)]
		public string RecState { set; get; }

		[StringLength(6)]
		public string RecPostDist { set; get; }

		[StringLength(200)]
		public string RecRemark { set; get; }

		[StringLength(8)]
		public string SectorNo { set; get; }

		[StringLength(16)]
		public string SectorName { set; get; }

		[StringLength(10)]
		public string SStatNo { set; get; }

		[StringLength(20)]
		public string SStatName { set; get; }

		public DateTime? SDate { set; get; }

		[StringLength(10)]
		public string AStatNo { set; get; }

		[StringLength(20)]
		public string AStatName { set; get; }

		[StringLength(10)]
		public string Hsno { get; set; }

		[StringLength(10)]
		public string ProductNo { set; get; }

		[StringLength(60)]
		public string ProductName { set; get; }

		[StringLength(10)]
		public string ForIndonesian { get; set; }

		public int? PiecesNo { set; get; }

		public decimal? Qty { set; get; }

		public decimal? Weight { set; get; }

		public decimal? Volume { set; get; }

		[StringLength(3)]
		public string Currency { set; get; }

		public decimal? Cost { set; get; }

		[StringLength(3)]
		public string CostCurrency { set; get; }

		[StringLength(10)]
		public string CcNo { set; get; }

		[StringLength(30)]
		public string PayCustNo { set; get; }

		[StringLength(100)]
		public string PayCustCHName { set; get; }

		public decimal? Freight { set; get; }

		[StringLength(3)]
		public string FreightCurrency { set; get; }

		public decimal? FuelCosts { set; get; }

		public decimal? ToPayment { set; get; }

		[StringLength(3)]
		public string ToPaymentCurrency { set; get; }

		public decimal? AgentPay { set; get; }

		[StringLength(3)]
		public string AgentPayCurrency { set; get; }

		public decimal? ProdIdPay { set; get; }

		public decimal? CustomsPay { set; get; }

		public decimal? InsurancePay { set; get; }

		public decimal? OtherPayTax { set; get; }

		public decimal? OtherPayNoTax { set; get; }

		public decimal? Length { set; get; }

		public decimal? Width { set; get; }

		public decimal? Height { set; get; }

		public decimal? Total { set; get; }

		[StringLength(3)]
		public string TotalCurrency { set; get; }

		[StringLength(200)]
		public string Remark { set; get; }

		[StringLength(200)]
		public string Remark2 { set; get; }

		public DateTime? PhoneCheckTime { set; get; }

		public string ImOrEx { set; get; }

		public string Status { set; get; }

		public DateTime? StatusTime { set; get; }

		public string Source { set; get; }

		public bool? IsConfirm { set; get; }

		[StringLength(8)]
		public string ConfirmBy { set; get; }

		public bool? IsCheck { set; get; }

		[StringLength(8)]
		public string CheckBy { set; get; }

		public DateTime? CheckTime { set; get; }

		public bool? IsReview { set; get; }

		[StringLength(8)]
		public string ReviewBy { set; get; }

		public DateTime? ReviewTime { set; get; }

		[StringLength(8)]
		public string CreateBy { set; get; }

		public DateTime? CreateTime { set; get; }

		[StringLength(8)]
		public string UpdateBy { set; get; }

		public DateTime? UpdateTime { set; get; }

		[StringLength(8)]
		public string DeletedBy { set; get; }

		public DateTime? DeletedTime { set; get; }

		[Required]
		public bool IsDelete { set; get; }

		[StringLength(20)]
		public string ShdetNo { set; get; }

		[StringLength(10)]
		public string PrintLang { set; get; }

		public bool? IsInLading { set; get; }

		public bool? Printed { set; get; }

		[StringLength(100)]
		public string RecCustCHName { set; get; }

		[StringLength(60)]
		public string RecCustEName1 { set; get; }

		[StringLength(30)]
		public string RecCustEName2 { set; get; }

		[StringLength(30)]
		public string RecCustENAddr1 { set; get; }

		[StringLength(30)]
		public string RecCustENAddr2 { set; get; }

		[StringLength(100)]
		public string RecCustENAddr3 { set; get; }

		[NotMapped]
		public string StatNo { set; get; }

		[NotMapped]
		public string HubName { set; get; }

		[NotMapped]
		public bool? IsDesp { set; get; }

		[NotMapped]
		public bool? IsFinish { set; get; }

		[NotMapped]
		public bool? IsFinance { set; get; }

		[NotMapped]
		public string PermitStatNo { set; get; }

		[NotMapped]
		public string S_SectorNo { set; get; }

		[NotMapped]
		public string PickUpAreaNo { set; get; }

		[NotMapped]
		public int? RecordType { set; get; }

		[NotMapped]
		public decimal? SumQty { set; get; }

		[NotMapped]
		public int? SumPiecesNo { set; get; }

		[NotMapped]
		public decimal? SumVolume { set; get; }

		[NotMapped]
		public decimal? SumWeight { set; get; }

		[NotMapped]
		public decimal? SumFreight { set; get; }

		[NotMapped]
		public decimal? SumToPayment { set; get; }

		[NotMapped]
		public decimal? SumAgentPay { set; get; }

		[NotMapped]
		public decimal? SumFuelCosts { set; get; }

		[NotMapped]
		public decimal? SumCustomsPay { set; get; }

		[NotMapped]
		public decimal? SumInsurancePay { set; get; }

		[NotMapped]
		public decimal? SumProdIdPay { set; get; }

		[NotMapped]
		public decimal? SumOtherPayTax { set; get; }

		[NotMapped]
		public decimal? SumOtherPayNoTax { set; get; }

		public int? BatchPcs { set; get; }
	}
}