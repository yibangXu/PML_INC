namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ORG_PostCode
    {
        [Required]
        [StringLength(3)]
        public string CityCode { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(5)]
        public string Code5 { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CityName { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string AreaName { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string RoadName { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string RowName { get; set; }

        [StringLength(7)]
        public string Code7 { get; set; }

		public DateTime CreatedDate { get; set; }

		[StringLength(50)]
		public string CreatedBy { get; set; }

		public bool IsDelete { get; set; }
    }
}
