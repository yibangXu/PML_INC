namespace HY_PML.Models
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class ShdetMD
	{
		public int RowNumber { get; set; }

		public string ShdetNo { get; set; }

		public string CustNo { get; set; }


		public string CustCHName { get; set; }

		public string Remark5 { get; set; }

		public string HubNo { get; set; }

		public string HubName { get; set; }

		public string Dest { get; set; }


		public string DestNo { get; set; }

		public bool? IsDesp { get; set; }

		public bool IsCancel { get; set; }

		public bool IsReply { get; set; }

		public bool IsFinish { get; set; }

		public string ShdetBy { get; set; }

		public DateTime? ShdetDate { get; set; }

		public string CancelBy { get; set; }

		public DateTime? CancelDate { get; set; }

		public string ReplyBy { get; set; }

		public DateTime? ReplyDate { get; set; }

		public string ReplyComment { get; set; }

		public string FinishBy { get; set; }

		public DateTime? FinishDate { get; set; }

		public string CreatedBy { get; set; }

		public string CreatedByNo { get; set; }

		public DateTime CreatedDate { get; set; }

		public string UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		public string DeletedBy { get; set; }

		public DateTime? DeletedDate { get; set; }

		public bool IsDelete { get; set; }

		public string Clerk { get; set; }

		public DateTime? ReserveDate { get; set; }

		public DateTime? SDate { get; set; }

		public string CallStatNo { get; set; }

		public string CallStatName { get; set; }

		public string StatNo { get; set; }

		public string StatName { get; set; }

		public int sNo { get; set; }

		public DateTime? RedyDate { get; set; }

		public string RedyTime { get; set; }

		public bool? IsRedy { get; set; }

		public string CarryName { get; set; }

		public string CarID { get; set; }

		public string CustAddrFull { get; set; }

		public string CtcSale { get; set; }

		public string Tel { get; set; }

		public string PickUpAreaNo { get; set; }

		public string PickUpAreaName { get; set; }

		public string SectorNo { get; set; }

		public string SectorName { get; set; }

		public string SectorPhone { get; set; }

		public int? WeigLevel { get; set; }

		public string WeigLevelType { get; set; }

		public bool OverWeig { get; set; }

		public bool OveriTotNum { get; set; }

		public bool OverTime { get; set; }

		public int? Add_1 { get; set; }
		public int? Add_2 { get; set; }
		public int? Add_3 { get; set; }
		public string Add_4 { get; set; }
		public int? Add_5 { get; set; }

		public string Add_6 { get; set; }
		public string CustAddr { get; set; }
		public string CustENAddr1 { get; set; }
		public string Country { get; set; }
		public string CcNo { get; set; }
		public double? Charge { get; set; }
		public string Code5 { get; set; }
		public string Code7 { get; set; }
		public string EndDate { get; set; }
		public int? CocustomTyp { get; set; }
		public string CocustomTypStr { get; set; }


		public string RejectBy { get; set; }

		public DateTime? RejectDate { get; set; }
		public string LadingNo { get; set; }
		public DateTime? DateTimeNow { get; set; }
		[NotMapped]
		public string LadingNo_Type { get; set; }
	}
}
