using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("Transportation")]
	public partial class Transportation
	{

		[Required]
		[StringLength(20)]
		[Key]
		[Column(Order = 0)]
		public string TransportationNo { set; get; }

		[Required]
		[StringLength(20)]
		public string LadingNo { set; get; }

		[StringLength(8)]
		public string RSectorNo { set; get; }

		[StringLength(16)]
		public string RSectorName { set; get; }

		public int LadingPcs { set; get; }

		public int ReceivePcs { set; get; }

		[StringLength(10)]
		public string NextStatNo { set; get; }

		[StringLength(20)]
		public string NextStatName { set; get; }
		
		public int DeliveryPcs { set; get; }

		public decimal ToPayment { set; get; }

		public decimal AgentPay { set; get; }

		[StringLength(10)]
		public string LastStatNo { set; get; }

		[StringLength(20)]
		public string LastStatName { set; get; }

		public int ArrivalPcs{ set; get; }

		public decimal LadingWeight { set; get; }

		public decimal ReviewWeight	{ set; get; }

		[StringLength(8)]
		public string SSectorNo { set; get; }

		[StringLength(16)]
		public string SSectorName { set; get; }

		public int SendPcs { set; get; }

		[StringLength(200)]
		public string Remark { set; get; }

		public int Status { set; get; }

		[Required]
		[Key]
		[Column(Order = 1)]
		public DateTime ReceiveTime { set; get; }

		public DateTime? DeliveryTime { set; get; }

		public DateTime? ArrivalTime { set; get; }

		public DateTime? SendTime { set; get; }

		[Required]
		[StringLength(20)]
		public string CreateBy { set; get; }

		[Required]
		public DateTime CreateTime { set; get; }

		[StringLength(20)]
		public string UpdateBy { set; get; }

		public DateTime? UpdateTime { set; get; }

		[StringLength(20)]
		public string DeletedBy { set; get; }

		public DateTime? DeletedTime { set; get; }

		[Required]
		public bool IsDelete { set; get; }

		[StringLength(20)]
		public string TransportNo { set; get; }
	}
}