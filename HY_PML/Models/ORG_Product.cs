namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Product
    {
        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string ProductNo { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string ProductEName { get; set; }

        [StringLength(50)]
        public string ProductEName2 { get; set; }

        public float? Price { get; set; }

        [StringLength(30)]
        public string Unit { get; set; }

        [StringLength(30)]
        public string TaxWay { get; set; }

        [StringLength(30)]
        public string Country { get; set; }

        [StringLength(10)]
        public string Hsno { get; set; }

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
