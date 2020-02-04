using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("BL_Report_Mas")]
	public partial class BL_Report_Mas
	{
		[Required]
		[StringLength(20)]
		[Key]
		public string ReportNo { set; get; }

		[StringLength(20)]
		public string MasterNo { set; get; }

		[StringLength(10)]
		public string FlightNo { set; get; }

		[StringLength(10)]
		public string HubNo { set; get; }

		[NotMapped]
		public string HubCode { set; get; }

		[NotMapped]
		public string HubName { set; get; }

		public int ReportID { set; get; }

		[NotMapped]
		public string ReportCName { set; get; }

		[StringLength(10)]
		public string SStatNo { set; get; }

		[NotMapped]
		public string SStatName { set; get; }

		[StringLength(10)]
		public string AStatNo { set; get; }

		[NotMapped]
		public string AStatName { set; get; }

		public string Remark { set; get; }

		public DateTime CreatedDate { get; set; }

		[StringLength(50)]
		public string CreatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[StringLength(50)]
		public string UpdatedBy { get; set; }

		public DateTime? DeletedDate { get; set; }

		[StringLength(50)]
		public string DeletedBy { get; set; }

		public bool IsDelete { get; set; }
	}
}