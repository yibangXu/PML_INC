namespace HY_PML.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	public partial class ORG_Hub
	{
		public int ID { get; set; }

		[Required]
		[StringLength(10)]
		public string HubNo { get; set; }

		[StringLength(15)]
		public string HubCode { get; set; }

		[Required]
		[StringLength(20)]
		public string HubName { get; set; }

		public int CustID { get; set; }

		[NotMapped]
		public string CustNo { get; set; }

		public bool IsServer { get; set; }

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


		[StringLength(10)]
		public string PrintLang { get; set; }

		[StringLength(20)]
		public string HubPName { get; set; }

	}
}
