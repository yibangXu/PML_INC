namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Currency
    {
        public int ID { get; set; }

        [Required]
        [StringLength(3)]
        public string CurrencyNo { get; set; }

        [Required]
        [StringLength(8)]
        public string CurrencyName { get; set; }

        public float Exch { get; set; }

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
