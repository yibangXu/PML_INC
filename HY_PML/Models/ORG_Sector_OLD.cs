namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Sector_OLD
    {
        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string SectorNo { get; set; }

        [Required]
        [StringLength(16)]
        public string SectorName { get; set; }

        public int Group { get; set; }

        public float? Lano { get; set; }

        [StringLength(30)]
        public string Phone { get; set; }

        public int? StatID { get; set; }

        public float? Quoteno { get; set; }

        public int? PickUpAreaID { get; set; }

        [StringLength(8)]
        public string Vehicletype { get; set; }

        [StringLength(16)]
        public string PlateNO { get; set; }

        [StringLength(16)]
        public string GpsNO { get; set; }

        public bool IsLeave { get; set; }

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
    }
}
