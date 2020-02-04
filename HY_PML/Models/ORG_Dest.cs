namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Dest
    {
        public int ID { get; set; }

        [Required]
        [StringLength(8)]
        public string DestNo { get; set; }

        [Required]
        [StringLength(50)]
        public string CName { get; set; }

        [NotMapped]
        public string Dest { get; set; }

        [StringLength(30)]
        public string ChName { get; set; }

        [StringLength(40)]
        public string Zone { get; set; }

        [StringLength(30)]
        public string State { get; set; }

        [StringLength(30)]
        public string Country { get; set; }

        [StringLength(40)]
        public string Areas { get; set; }

        [StringLength(40)]
        public string Zip { get; set; }

        [StringLength(40)]
        public string Tel { get; set; }

        public int? StatID { get; set; }

        [NotMapped]
        public string StatNo { get; set; }

        public int? AreaID { get; set; }

        [NotMapped]
        public string AreaNo { get; set; }

        public int? CurrencyID { get; set; }

        [NotMapped]
        public string CurrencyNo { get; set; }

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
