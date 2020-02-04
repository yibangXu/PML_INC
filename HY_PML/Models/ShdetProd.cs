namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShdetProd")]
    public partial class ShdetProd
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string ShdetNo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string CustNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sDtlNo { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sNo { get; set; }

        public int? Pcs { get; set; }

        public int? WeigLevel { get; set; }

        public double? Weig { get; set; }

        public int? CocustomTyp { get; set; }

        [StringLength(10)]
        public string HubNo { get; set; }
        [NotMapped]
        public string HubName { get; set; }

        [StringLength(10)]
        public string CcNo { get; set; }

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

        [StringLength(8)]
        public string SectorNo { get; set; }

        [StringLength(10)]
        public string CallType { get; set; }

        [StringLength(10)]
        public string StatNo { get; set; }
        [NotMapped]
        public string CallStatNo { get; set; }

        [StringLength(4)]
        public string CarID { get; set; }

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

        public string ReplyComment { get; set; }

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

        public double? fLen { get; set; }

        public double? fWidth { get; set; }

        public double? fHeight { get; set; }

        public double? iNum { get; set; }

        public double? iTotNum { get; set; }

        [StringLength(10)]
        public string SheetNo { get; set; }

        public int? SSNo { get; set; }

		public DateTime? PhoneCheckTime { set; get; }

		public string Status { set; get; }

		public DateTime? StatusTime { set; get; }

		[NotMapped]
		public string CocustomTypStr { set; get; }
	}
}
