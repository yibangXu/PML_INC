namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_PickUpAreaAddress
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string PickUpAreaNo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(5)]
        public string Code5 { get; set; }

		[StringLength(7)]
		public string Code7 { get; set; }

		[Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string CityAreaRoadRow { get; set; }

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
