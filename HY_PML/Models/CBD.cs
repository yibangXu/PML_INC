using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("CBD")]
	public partial class CBD
	{
		public string Type { set; get; }

		[Required]
		[StringLength(11)]
		[Key]
		public string CRNo { set; get; }

		[StringLength(20)]
		public string CustNo { set; get; }

		[StringLength(60)]
		public string CustName { set; get; }

		[StringLength(20)]
		public string LadingNo { set; get; }

		public decimal? Total { set; get; }

		public decimal? CashRecv { set; get; }

		public DateTime? RecvTime { set; get; }

		[StringLength(20)]
		public string Receiver { set; get; }

		[StringLength(60)]
		public string BillBank { set; get; }

		[StringLength(20)]
		public string BillNo { set; get; }

		public DateTime? BillDueDate { set; get; }

		public decimal? BillAmount { set; get; }

		public decimal? discount { set; get; }

		[StringLength(200)]
		public string Reason { set; get; }

		[StringLength(200)]
		public string Remark { set; get; }

		public bool IsCheck { set; get; }

		[StringLength(8)]
		public string CheckBy { set; get; }

		public DateTime? CheckTime { set; get; }

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

	}
}