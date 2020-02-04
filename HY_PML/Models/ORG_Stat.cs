namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_Stat
    {
        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string StatNo { get; set; }

        [Required]
        [StringLength(20)]
        public string StatName { get; set; }

        public int Stattype { get; set; }

        public int? AreaID { get; set; }

        [NotMapped]
        public string AreaNo { get; set; }

        public int? CenterID { get; set; }

        public int? DestID { get; set; }

        [NotMapped]
        public string DestNo { get; set; }

        public float? N1 { get; set; }

        public float? N2 { get; set; }

        public float? N3 { get; set; }

        public bool Isnetwork { get; set; }

        [StringLength(50)]
        public string SendDirector { get; set; }

        [StringLength(50)]
        public string SendTel { get; set; }

        [StringLength(50)]
        public string SendDirectorHand { get; set; }

        [StringLength(50)]
        public string SendFax { get; set; }

        [StringLength(50)]
        public string Tel { get; set; }

        public int? StorageID { get; set; }

        [StringLength(50)]
        public string SendTime { get; set; }

        public bool IsWww { get; set; }

        public bool IsCod { get; set; }

        public bool IsReturn { get; set; }

        public bool IsToday { get; set; }

        public bool IsCc { get; set; }

        public float CccCharge { get; set; }

        public float? CcodCharge { get; set; }

        public int? CurrencyID { get; set; }

        [NotMapped]
        public string CurrencyNo { get; set; }

        [StringLength(100)]
        public string Remark { get; set; }

        public string SendArea { get; set; }

        public string SendAreaNo { get; set; }

        public string List { get; set; }

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
