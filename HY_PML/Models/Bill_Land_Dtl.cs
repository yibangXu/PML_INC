using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HY_PML.Models
{
    [Table("Bill_Land_Dtl")]
    public partial class Bill_Land_Dtl
    {
        [Required]
        [StringLength(10)]
        [Column(Order = 0)]
        [Key]
        public string SheetNo { set; get; }
        [Required]
        [Column(Order = 1)]
        [Key]
        public int SNo { set; get; }
        [StringLength(22)]
        public string MasterNo { set; get; }
        [StringLength(12)]
        public string HouseNo { set; get; }
        [Required]
        public string ShdetNo { set; get; }
        [Required]
        public string CustomerNo { set; get; }
        [NotMapped]
        public string CustCHName { set; get; }
        [Required]
        public int SDtlNo { set; get; }
        [Required]
        public int ProdNo { set; get; }
        [StringLength(10)]
        public string WriteOff { set; get; }
        [StringLength(15)]
        public string BagNo { set; get; }
        public string Remark { set; get; }
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