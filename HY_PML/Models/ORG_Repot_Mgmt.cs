namespace HY_PML.Models
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public partial class ORG_Report_Mgmt
	{
		public int ID { get; set; }

		[Required]
		[StringLength(20)]
		public string ReportCName { get; set; }

		[StringLength(20)]
		public string ReportEName { get; set; }

		[StringLength(10)]
		public string ReportCode { get; set; }

		[StringLength(10)]
		public string HubNo { get; set; }

		public bool IsBackfill { get; set; }

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

		[NotMapped]
		public string HubName { get; set; }
	}
}
