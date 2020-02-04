namespace HY_PML.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	public partial class ORG_Consin
	{
		public int ID { get; set; }

		[StringLength(10)]
		public string ConsinNo { get; set; }

		[StringLength(20)]
		public string CustNo { get; set; }

		[NotMapped]
		public string CustCName { get; set; }

		[StringLength(60)]
		public string ConsinComp { get; set; }

		[StringLength(60)]
		public string Cnconsin { get; set; }

		[StringLength(200)]
		public string Cnaddr { get; set; }

		[StringLength(60)]
		public string Enconsin1 { get; set; }

		[StringLength(30)]
		public string Enconsin2 { get; set; }

		[StringLength(30)]
		public string Enaddr1 { get; set; }

		[StringLength(30)]
		public string Enaddr2 { get; set; }

		[StringLength(100)]
		public string Enaddr3 { get; set; }

		[StringLength(50)]
		public string City { get; set; }

		[StringLength(30)]
		public string State { get; set; }

		[StringLength(30)]
		public string Country { get; set; }

		[StringLength(30)]
		public string Zip { get; set; }

		[StringLength(60)]
		public string Tel { get; set; }

		[StringLength(100)]
		public string Consinee { get; set; }

		[StringLength(30)]
		public string UnifyNo { get; set; }

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

		[StringLength(5)]
		public string Code5 { get; set; }

		public int? Add_1 { get; set; }

		public int? Add_2 { get; set; }

		public int? Add_3 { get; set; }

		public int? Add_4 { get; set; }

		public int? Add_5 { get; set; }

		[StringLength(30)]
		public string Add_6 { get; set; }

		[StringLength(60)]
		public string MPhone { get; set; }

		[NotMapped]
		public string Phone { get; set; }
	}
}
