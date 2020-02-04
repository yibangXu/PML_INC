using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("Bill_Lading_Record")]
	public partial class Bill_Lading_Record
	{
		[Required]
		[StringLength(20)]
		[Key]
		[Column(Order = 0)]
		public string LadingNo { set; get; }
		[Key]
		[Column(Order = 1)]
		public int SNo { set; get; }

		public DateTime RecordDate{ set; get; }

		public int RecordType { set; get; }

		public string StatNo { set; get; }

		public string Remark { set; get; }

		public string CreateBy { set; get; }

		public DateTime CreateTime { set; get; }

		public string UpdateBy { set; get; }

		public DateTime? UpdateTime { set; get; }

		public string DeletedBy { set; get; }

		public DateTime? DeletedTime { set; get; }

		public bool IsDelete { set; get; }

		[NotMapped]
		public int Index { set; get; }
	}
}