using HU.CSVFormatAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("TransportationA")]
	public partial class TransportationA
	{

		[Key]
		[Column(Order = 0)]
		[Required]
		[StringLength(20)]
		[CSVFieldName("運務單號")]
		public string TransportationNo { set; get; }

		[Required]
		[StringLength(20)]
		[CSVHiddenField]
		public string LadingNo { set; get; }

		[NotMapped]
		[CSVFieldName("提單單號")]
		public string LadingNo_Type { set; get; }

		[StringLength(10)]
		[CSVHiddenField]
		public string AStatNo { set; get; }

		[StringLength(20)]
		[CSVHiddenField]
		public string AStatName { set; get; }

		[Key]
		[Column(Order = 1)]
		[Required]
		[CSVDateTimeOnly]
		[CSVFieldName("到件掃描時間")]
		public DateTime ArrivalTime { set; get; }

		[StringLength(10)]
		[CSVFieldName("上一站代號")]
		public string LastStatNo { set; get; }

		[StringLength(20)]
		[CSVFieldName("上一站名稱")]
		public string LastStatName { set; get; }

		[CSVFieldName("到件件數")]
		public int ArrivalPcs{ set; get; }

		[CSVFieldName("明細件數")]
		public int LadingPcs { set; get; }

		[CSVFieldName("明細重量")]
		public decimal? LadingWeight { set; get; }

		[CSVFieldName("複核重量")]
		public decimal ReviewWeight	{ set; get; }

		[CSVFieldName("到付款")]
		public decimal ToPayment { set; get; }

		[CSVFieldName("代收款")]
		public decimal AgentPay { set; get; }

		[Required]
		[StringLength(20)]
		[CSVHiddenField]
		public string CreateBy { set; get; }

		[Required]
		[CSVHiddenField]
		public DateTime CreateTime { set; get; }

		[StringLength(20)]
		[CSVHiddenField]
		public string UpdateBy { set; get; }

		[CSVHiddenField]
		public DateTime? UpdateTime { set; get; }

		[StringLength(20)]
		[CSVHiddenField]
		public string DeletedBy { set; get; }

		[CSVHiddenField]
		public DateTime? DeletedTime { set; get; }

		[Required]
		[CSVHiddenField]
		public bool IsDelete { set; get; }

		[CSVHiddenField]
		public bool IsCheck { set; get; }

		[NotMapped]
		[CSVHiddenField]
		public bool isAdd { set; get; }

		[NotMapped]
		[CSVHiddenField]
		public int Index { set; get; }
	}
}