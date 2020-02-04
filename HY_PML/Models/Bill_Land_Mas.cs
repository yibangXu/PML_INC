using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("Bill_Land_Mas")]
    public partial class Bill_Land_Mas
    {
        [Required]
        [StringLength(10)]
        [Key]
        public string SheetNo { set; get; }
        [StringLength(22)]
        public string MasterNo { set; get; }
        [StringLength(7)]
        public string FlightNo { set; get; }
        [StringLength(10)]
        public string HubNo { set; get; }
        [NotMapped]
        public string HubName { set; get; }
        [StringLength(10)]
        public string StatNo { set; get; }
        [NotMapped]
        public string StatName { set; get; }
        public string Remark { set; get; }
        public DateTime? PrintTime { set; get; }
        [Required]
        [StringLength(20)]
        public string CreateBy { set; get; }
        [Required]
        public DateTime CreateTime { set; get; }
        [StringLength(20)]
        public string UpdateBy { set; get; }
        public DateTime? UpdateTime { set; get; }
        [StringLength(20)]
        public string DeleteBy { set; get; }
        public DateTime? DeleteTime { set; get; }
        [Required]
        public bool IsDelete { set; get; }
    }
}