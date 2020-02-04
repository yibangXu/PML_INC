namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Company
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string CompanyNo { get; set; }

        [Required]
        [StringLength(40)]
        public string CompanyName { get; set; }

        [StringLength(40)]
        public string CompanyEName { get; set; }

        [Required]
        [StringLength(40)]
        public string GuiNa { get; set; }

        [Required]
        [StringLength(14)]
        public string GuiNo { get; set; }

        [StringLength(14)]
        public string TaxNo { get; set; }

        [StringLength(20)]
        public string Tel { get; set; }

        [StringLength(20)]
        public string Fax { get; set; }

        [StringLength(40)]
        public string CAddr { get; set; }

        [StringLength(100)]
        public string EAddr { get; set; }

        [StringLength(60)]
        public string GuiAddr { get; set; }

        [StringLength(100)]
        public string Url { get; set; }

        [StringLength(30)]
        public string NetWork { get; set; }

        [StringLength(30)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Ip { get; set; }

        [StringLength(30)]
        public string IpBak { get; set; }

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
