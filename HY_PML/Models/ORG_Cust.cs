namespace HY_PML.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	public partial class ORG_Cust
	{
		public int ID { get; set; }

		[Required]
		public string CustNo { get; set; }

		public string CustLevel { get; set; }

		public string CustCHName { get; set; }

		public string CustEName1 { get; set; }

		public string CustEName2 { get; set; }

		public string CustCName { get; set; }

		public string CustEName { get; set; }

		public string CarryName { get; set; }

		[StringLength(5)]
		public string Code5 { get; set; }

		[StringLength(7)]
		public string Code7 { get; set; }

		public int? Add_1 { get; set; }

		public int? Add_2 { get; set; }

		public int? Add_3 { get; set; }

		public string Add_4 { get; set; }

		public int? Add_5 { get; set; }

		[StringLength(30)]
		public string Add_6 { get; set; }

		public string CustAddr { get; set; }

		public string CustENAddr1 { get; set; }

		public string CustENAddr2 { get; set; }

		public string SendBy { get; set; }

		public string InvNo { get; set; }

		public string City { get; set; }

		public string Country { get; set; }

		public string State { get; set; }

		public string PostDist { get; set; }

		public string ESendBy { get; set; }

		public string EInvNo { get; set; }

		public string ECity { get; set; }

		public string ECountry { get; set; }

		public string EState { get; set; }

		public string EPostDist { get; set; }

		public string IDNo { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public string FaxNo { get; set; }

		public string CtcAcc { get; set; }

		public string AccPhone { get; set; }

		public string CtcSale { get; set; }

		public string CtcSale2 { get; set; }

		public string CtcSale3 { get; set; }

		public string Account { get; set; }

		public string PayTerm { get; set; }

		public int? CcID { get; set; }

		public string PayDate { get; set; }

		public DateTime? SignDate { get; set; }

		public string SuspDate { get; set; }

		public string Discnt { get; set; }

		public string DiscntOut { get; set; }

		public string QuoType { get; set; }

		public string Balance { get; set; }

		public string IsinVoice { get; set; }

		public string InvTitle { get; set; }

		public string InvAddr { get; set; }

		public int? StatID { get; set; }

		public string IsAgent { get; set; }

		public bool? IsCommon { get; set; }

		public bool? IsServer { get; set; }

		public bool? IsMas { get; set; }

		public int? PickUpAreaID { get; set; }

		public string Remark5 { get; set; }

		public string SectorNo { get; set; }

		public string Remark { get; set; }

		public string DayOfWeek { get; set; }

		public string RedyDayWeekly { get; set; }

		public string RedyTime { get; set; }

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

		[StringLength(5)]
		public string Code5_C { get; set; }

		[StringLength(7)]
		public string Code7_C { get; set; }

		public int? Add_1_C { get; set; }

		public int? Add_2_C { get; set; }

		public int? Add_3_C { get; set; }

		public string Add_4_C { get; set; }

		public int? Add_5_C { get; set; }

		public string SalesArea { get; set; }

		[StringLength(30)]
		public string Add_6_C { get; set; }

		public string CustAddr_C { get; set; }

		[NotMapped]
		public string CcNo { get; set; }

		[NotMapped]
		public string StatNo { get; set; }

		[NotMapped]
		public string SectorName { get; set; }

		[NotMapped]
		public string PickUpAreaNo { get; set; }

		[NotMapped]
		public string PickUpAreaName { get; set; }

		[NotMapped]
		public string CustAddrFull { get; set; }

		[NotMapped]
		public string CustAddrFull_C { get; set; }

		[NotMapped]
		public bool? IsFormal { set; get; }
	}
}
