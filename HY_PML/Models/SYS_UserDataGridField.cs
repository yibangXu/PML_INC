namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SYS_UserDataGridField
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short UserID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string Controller { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string Action { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string DataGridField { get; set; }

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
