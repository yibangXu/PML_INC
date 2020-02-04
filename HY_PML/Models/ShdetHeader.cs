namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShdetHeader")]
    public partial class ShdetHeader
    {
        [Key]
        [StringLength(20)]
        public string ShdetNo { get; set; }

        [Required]
        [StringLength(20)]
        public string CustNo { get; set; }

        [StringLength(100)]
        public string CustCHName { get; set; }

        [StringLength(10)]
        public string HubNo { get; set; }

        [NotMapped]
        public string HubName { get; set; }

        [NotMapped]
        public string CallStatName { get; set; }

        [StringLength(30)]
        public string Dest { get; set; }

        public bool IsDesp { get; set; }

        public bool IsCancel { get; set; }

        public bool IsReply { get; set; }

        public bool IsFinish { get; set; }

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

        [StringLength(30)]
        public string Clerk { get; set; }

        public DateTime? ReserveDate { get; set; }

        public string RejectBy { get; set; }

        public DateTime? RejectDate { get; set; }

		public DateTime? SDate { get; set; }

		[StringLength(20)]
		public string LadingNo { get; set; }

	}
}
