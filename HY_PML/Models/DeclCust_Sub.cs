using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("DeclCust_Sub")]
	public partial class DeclCust_Sub
	{
		[Required]
		[Key]
		[Column(Order = 1)]
		public int sNo { set; get; }

		[Required]
		[StringLength(20)]
		[Key]
		[Column(Order = 0)]
		public string LadingNo { set; get; }

		[StringLength(20)]
		public string BagNo { set; get; }

		[StringLength(20)]
		public string CleCusCode { set; get; }

		[StringLength(60)]
		public string ProductName { set; get; }

		[StringLength(5)]
		public string Type { set; get; }

		public decimal Qty { set; get; }

		public decimal Weight { set; get; }

		public decimal GrossWeight { set; get; }

		public decimal Price { set; get; }

		[StringLength(200)]
		public string Remark { set; get; }

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

		public decimal? Length { set; get; }

		public decimal? Width { set; get; }

		public decimal? Height { set; get; }
	}
}