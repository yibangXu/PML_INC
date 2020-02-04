using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("DeclCust_Main")]
	public partial class DeclCust_Main
	{

		[Required]
		[Key]
		[Column(Order = 1)]
		public int sNo { set; get; }

		[Required]
		[Key]
		[StringLength(20)]
		[Column(Order = 0)]
		public string LadingNo { set; get; }

		[StringLength(5)]
		public string CustType { set; get; }

		[StringLength(20)]
		public string Flight { set; get; }

		[StringLength(20)]
		public string LadNo { set; get; }

		[StringLength(20)]
		public string BagNo { set; get; }

		[StringLength(20)]
		public string CleCusCode { set; get; }

		[StringLength(20)]
		public string CusCoode { set; get; }

		[StringLength(10)]
		public string ProductNo { set; get; }

		[StringLength(60)]
		public string ProductName { set; get; }

		[StringLength(50)]
		public string ProductEName { set; get; }

		[StringLength(20)]
		public string Country { set; get; }
		
		[StringLength(5)]
		public string Type { set; get; }
		
		[StringLength(20)]
		public string HSNo { set; get; }

		public decimal Qty { set; get; }
		
		[StringLength(10)]
		public string Unit { set; get; }

		public decimal? GrossWeight { set; get; }

		public decimal? Weight { set; get; }

		public decimal Price { set; get; }

		public decimal Total { set; get; }
		
		[StringLength(3)]
		public string Currency { set; get; }

		public int? Pcs { set; get; }

		[StringLength(10)]
		public string PcsNo { set; get; }

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