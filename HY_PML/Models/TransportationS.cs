using HU.CSVFormatAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("TransportationS")]
	public partial class TransportationS
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
		public string SStatNo { set; get; }

		[StringLength(20)]
		[CSVHiddenField]
		public string SStatName { set; get; }

		[Key]
		[Column(Order = 1)]
		[Required]
		[CSVFieldName("派件掃描時間")]
		public DateTime SendTime { set; get; }

		[StringLength(8)]
		[CSVFieldName("派件外務員代號")]
		public string SSectorNo { set; get; }

		[StringLength(16)]
		[CSVFieldName("派件外務員名稱")]
		public string SSectorName { set; get; }

		[StringLength(16)]
		[CSVFieldName("車牌號碼")]
		public string PlateNo { set; get; }

		[CSVFieldName("派件件數")]
		public int SendPcs { set; get; }

		[CSVFieldName("明細件數")]
		public int LadingPcs { set; get; }

		[StringLength(200)]
		[CSVFieldName("備註")]
		public string Remark { set; get; }

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