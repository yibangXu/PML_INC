namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Sector
    {
        [Key]
        [StringLength(10)]
        public string SectorNo { get; set; }

        [Required]
        [StringLength(16)]
        public string SectorName { get; set; }

        [StringLength(16)]
        public string PlateNO { get; set; }

        [StringLength(30)]
        public string Phone { get; set; }

        [StringLength(10)]
        public string StatNo { get; set; }

        [StringLength(20)]
        public string PickUpAreaNo { get; set; }

        public bool IsLeave { get; set; }

        [StringLength(30)]
        public string PhonePrivate { get; set; }

        [StringLength(5)]
        public string RecTime { get; set; }

        [StringLength(5)]
        public string EndTime { get; set; }

        [StringLength(50)]
        public string Latitude { get; set; }

        [StringLength(50)]
        public string Longitude { get; set; }

        public int? RecRange { get; set; }

        public int? TargetKM { get; set; }

        public int? TargetNum { get; set; }

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

		[NotMapped]
		public DateTime? RedyDate { get; set; }

		[NotMapped]
		[StringLength(5)]
		public string RedyTime { get; set; }

		[NotMapped]
		public string CarID { get; set; }

		[NotMapped]
		public bool? IsOff { get; set; }

	}
}
