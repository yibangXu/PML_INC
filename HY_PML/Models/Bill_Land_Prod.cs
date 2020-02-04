namespace HY_PML.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Bill_Land_Prod
    {
        public int index { get; set; }

        public string ShdetNo { get; set; }

        public string CustNo { get; set; }

        public string CustCHName { get; set; }

        public string CarryName { get; set; }

        public DateTime? ShdetDate { set; get; }

        public int sDtlNo { get; set; }

        public int sNo { get; set; }

        public int? Pcs { get; set; }

        public int? WeigLevel { get; set; }

        public double? Weig { get; set; }

        public int? CocustomTyp { get; set; }

        public string HubNo { get; set; }

        public string HubName { get; set; }

        public string CcNo { get; set; }

        public double? Charge { get; set; }

        public string Dest { get; set; }

        public DateTime? RedyDate { get; set; }

        public string RedyTime { get; set; }

        public string Remark1 { get; set; }

        public string Remark3 { get; set; }

        public string ReplyComment { get; set; }

        public string SectorNo { get; set; }

        public string CallType { get; set; }

        public string StatNo { get; set; }

        public string CallStatNo { get; set; }

        public string CarID { get; set; }

        public double? fLen { get; set; }

        public double? fWidth { get; set; }

        public double? fHeight { get; set; }

        public double? iNum { get; set; }

        public double? iTotNum { get; set; }

        public string SheetNo { get; set; }

        public int? SSNo { get; set; }

        public DateTime CreatDate { get; set; }

		public string CocustomTypStr { set; get; }
	}
}
