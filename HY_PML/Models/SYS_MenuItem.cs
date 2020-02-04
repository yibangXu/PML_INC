namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SYS_MenuItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SYS_MenuItem()
        {
            SYS_MenuItem1 = new HashSet<SYS_MenuItem>();
        }

        public int ID { get; set; }

        public int MenuID { get; set; }

        public int? ParentID { get; set; }

        public int Sequence { get; set; }

        public string IconPath { get; set; }

        [Required]
        [StringLength(50)]
        public string Caption { get; set; }

        [NotMapped]
        public string PCaption { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Description { get; set; }

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

        public virtual SYS_Menu SYS_Menu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_MenuItem> SYS_MenuItem1 { get; set; }

        public virtual SYS_MenuItem SYS_MenuItem2 { get; set; }
    }
}
