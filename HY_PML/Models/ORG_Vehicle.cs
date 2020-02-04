namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Vehicle
    {
        [Key]
        [StringLength(4)]
        public string CarID { get; set; }

        [StringLength(20)]
        public string CarNO { get; set; }

        [StringLength(20)]
        public string CarKind { get; set; }

        [StringLength(20)]
        public string CarBrand { get; set; }

        public DateTime? CheckDT { get; set; }

        public int? Oil { get; set; }

        public int? LoadLimit { get; set; }

        public int? LoadSafety { get; set; }

        public double? ValueMax { get; set; }

        public double? ValueSafe { get; set; }

        public int? BoxLength { get; set; }

        public int? BoxWidth { get; set; }

        public int? BoxHeight { get; set; }

        public DateTime? StopDT { get; set; }

        [StringLength(100)]
        public string ReMark { get; set; }

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
