using HU.CSVFormatAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("TransportationD")]
	public partial class TransportationD
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

		[StringLength(20)]
		[CSVFieldName("轉運單號")]
		public string TransportNo { set; get; }

		[StringLength(10)]
		[CSVHiddenField]
		public string DStatNo { set; get; }

		[StringLength(20)]
		[CSVHiddenField]
		public string DStatName { set; get; }

		[Key]
		[Column(Order = 1)]
		[Required]
		[CSVDateTimeOnly]
		[CSVFieldName("出貨掃描時間")]
		public DateTime? DeliveryTime { set; get; }


		[StringLength(10)]
		[CSVFieldName("下一站代號")]
		public string NextStatNo { set; get; }

		[StringLength(20)]
		[CSVFieldName("下一站名稱")]
		public string NextStatName { set; get; }

		[CSVFieldName("出貨件數")]
		public int DeliveryPcs { set; get; }

		[CSVFieldName("明細件數")]
		public int LadingPcs { set; get; }

		[CSVFieldName("到付款")]
		public decimal? ToPayment { set; get; }

		[CSVFieldName("代收款")]
		public decimal? AgentPay { set; get; }

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