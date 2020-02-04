namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_PickUpArea
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string PickUpAreaNo { get; set; }

        [Required]
        [StringLength(30)]
        public string PickUpAreaName { get; set; }

        [StringLength(5)]
        public string DateEnd { get; set; }

        public int? StatID { get; set; }

        [NotMapped]
        public string StatNo { get; set; }

        [StringLength(250)]
        public string PickUpAreaAddress { get; set; }

        [StringLength(100)]
        public string Remark { get; set; }

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
		public string SectorNo { get; set; }

		[NotMapped]
		public string SectorName { get; set; }

		[NotMapped]
        public string Code5 { get; set; }

		[NotMapped]
		public string Code7 { get; set; }

		[NotMapped]
        public string CityAreaRoadRow { get; set; }
    }
}
