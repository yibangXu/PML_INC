using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("BL_FileTable")]
	public partial class BL_FileTable
	{
		[Required]
		[StringLength(20)]
		[Key]
		[Column(Order = 0)]
		public string LadingNo { set; get; }
		[Key]
		[Column(Order = 1)]
		public int SNo { set; get; }
		public string FileName { set; get; }

		public string UploadBy { set; get; }

		public DateTime UploadTime { set; get; }

		public string DeletedBy { set; get; }

		public DateTime? DeletedTime { set; get; }

		public bool IsDelete { set; get; }

		[NotMapped]
		public int Index { set; get; }

	}
}