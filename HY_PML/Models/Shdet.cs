namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Shdet")]
    public partial class Shdet
    {
        public int ID { get; set; }

        [Required]
        [StringLength(12)]
        public string ShdetNo { get; set; }

        public int CustID { get; set; }

        [StringLength(100)]
        public string Shipp { get; set; }

        [StringLength(100)]
        public string Caddr { get; set; }

        [StringLength(6)]
        public string CityName { get; set; }

        [StringLength(6)]
        public string TownName { get; set; }

        [StringLength(50)]
        public string StreetName { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(30)]
        public string State { get; set; }

        [StringLength(30)]
        public string CtcSale { get; set; }

        [StringLength(30)]
        public string Tel { get; set; }

        [StringLength(30)]
        public string Clerk { get; set; }

        public int? PickUpAreaID { get; set; }

        [Column("'CallDate'")]
        public DateTime? C_CallDate_ { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(100)]
        public string Remark2 { get; set; }

        public int? Pcs { get; set; }

        public int? WeigLevel { get; set; }

        public double? Weig { get; set; }

        public int? CocustomTyp { get; set; }

        public int? HubID { get; set; }

        public int? CcID { get; set; }

        public double? Charge { get; set; }

        [StringLength(30)]
        public string Dest { get; set; }

        public DateTime? RedyDate { get; set; }

        [StringLength(5)]
        public string RedyTime { get; set; }

        [StringLength(100)]
        public string Remark1 { get; set; }

        [StringLength(100)]
        public string Remark3 { get; set; }

        public int? SectorID { get; set; }

        [StringLength(10)]
        public string CallType { get; set; }

        public int? StatID { get; set; }

        public bool? IsDesp { get; set; }

        public bool? IsCancel { get; set; }

        public bool? IsReply { get; set; }

        public bool? IsFinish { get; set; }

        [StringLength(50)]
        public string ShdetBy { get; set; }

        public DateTime? ShdetDate { get; set; }

        [StringLength(50)]
        public string CancelBy { get; set; }

        public DateTime? CancelDate { get; set; }

        [StringLength(50)]
        public string ReplyBy { get; set; }

        public DateTime? ReplyDate { get; set; }

        [StringLength(50)]
        public string FinishBy { get; set; }

        public DateTime? FinishDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsDelete { get; set; }
    }
}
