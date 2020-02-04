using HU.CSVFormatAttributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("TransportationR")]
	public partial class TransportationR
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

		[StringLength(8)]
		[CSVFieldName("收件外務員代號")]
		public string RSectorNo { set; get; }

		[StringLength(16)]
		[CSVFieldName("收件外務員名稱")]
		public string RSectorName { set; get; }

		[CSVFieldName("收件件數")]
		public int ReceivePcs { set; get; }


		[StringLength(10)]
		[CSVHiddenField]
		public string RStatNo { set; get; }

		[StringLength(20)]
		[CSVHiddenField]
		public string RStatName { set; get; }

		[Key]
		[Column(Order = 1)]
		[Required]
		[CSVFieldName("收件掃描時間")]
		[CSVDateTimeOnly]
		public DateTime ReceiveTime { set; get; }

		[Required]
		[StringLength(20)]
		[CSVFieldName("操作員")]
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