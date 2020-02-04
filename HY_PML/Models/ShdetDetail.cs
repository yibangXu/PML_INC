namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShdetDetail")]
    public partial class ShdetDetail
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
        public int sNo { get; set; }

        [Required]
        [StringLength(100)]
        public string CarryName { get; set; }

        [StringLength(5)]
        public string Code5 { get; set; }

		[StringLength(7)]
		public string Code7 { get; set; }

		public int? Add_1 { get; set; }

        public int? Add_2 { get; set; }

        public int? Add_3 { get; set; }

		[StringLength(10)]
		public string Add_4 { get; set; }

        public int? Add_5 { get; set; }

        [StringLength(30)]
        public string Add_6 { get; set; }

        public string CustAddr { get; set; }

        [NotMapped]
        public string CustAddrFull { get; set; }

        public string CustENAddr1 { get; set; }

        public string CustENAddr2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string CtcSale { get; set; }

        [StringLength(30)]
        public string Tel { get; set; }

        [StringLength(30)]
        public string Clerk { get; set; }

        [StringLength(20)]
        public string PickUpAreaNo { get; set; }

        [NotMapped]
        public string PickUpAreaName { get; set; }

        [StringLength(5)]
        public string EndDate { get; set; }

        [StringLength(100)]
        public string Remark2 { get; set; }

        [StringLength(100)]
        public string Remark3 { get; set; }

        [StringLength(8)]
        public string SectorNo { get; set; }

        [NotMapped]
        public string SectorName { get; set; }

        [StringLength(10)]
        public string CallType { get; set; }

        [StringLength(10)]
        public string StatNo { get; set; }

        [NotMapped]
        public string StatName { get; set; }

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

        public int? WeigLevel { get; set; }

        public int? CocustomTyp { get; set; }

        [StringLength(10)]
        public string HubNo { get; set; }

        [StringLength(10)]
        public string CcNo { get; set; }

        public double? Charge { get; set; }

        [StringLength(30)]
        public string Dest { get; set; }

        public DateTime? RedyDate { get; set; }

        [StringLength(5)]
        public string RedyTime { get; set; }

		public bool? IsRedy { get; set; }

		[StringLength(4)]
        public string CarID { get; set; }

        [StringLength(10)]
        public string CallStatNo { get; set; }

		public DateTime? ADate { get; set; }

		[StringLength(5)]
		public string ATime { get; set; }

		[StringLength(30)]
		public string SectorPhone { get; set; }

		public DateTime? PhoneCheckTime { set; get; }

		public string Status { set; get; }

		public DateTime? StatusTime { set; get; }

		public string ReplyComment { get; set; }

		public string LadingNo_Type { get; set; }

		[NotMapped]
		public string CallStatName { get; set; }

		[NotMapped]
		public string Type { get; set; }

		[NotMapped]
		public int Index { set; get; }
	}
}
