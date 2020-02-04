namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SYS_User
    {
        public int ID { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public byte[] Password { get; set; }

        [NotMapped]
        public string PasswordStr { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public DateTime RegisterDate { get; set; }

        public DateTime? ActiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string SecurityQuestion { get; set; }

        public string SecurityAnswer { get; set; }

        [StringLength(10)]
        public string StatNo { get; set; }

        [NotMapped]
        public string StatName { get; set; }

        [StringLength(10)]
        public string DepartNo { get; set; }

        [NotMapped]
        public string DepartName { get; set; }

        [StringLength(10)]
        public string UserGroupNo { get; set; }

        [NotMapped]
        public string UserGroupName { get; set; }

        public bool IsActive { get; set; }

        [StringLength(100)]
        public string Remark { get; set; }

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

		public DateTime? LastLoginTime { get; set; }

		public int? LoginFrequency { get; set; }
	}
}
