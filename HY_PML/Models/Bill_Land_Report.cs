using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HY_PML.Models
{
    public class Bill_Land_Report
    {
        public string SheetNo { set; get; }
        public string MasterNo { set; get; }
        public string FlightNo { set; get; }
        public string MasRemark { set; get; }
        public string HubName { set; get; }
        public DateTime? PrintTime { set; get; }
        public List<Bill_Land_Report_Items> Items { set; get; }
    }

    public class Bill_Land_Report_Items
    {
		public string LadingNo { set; get; }
		public string MasterNo { set; get; }
        public string HouseNo { set; get; }
        public string CarryName { set; get; }
        public string StatNo { set; get; }
        public string SectorName { set; get; }
        public string WeigLevel { set; get; }
        public int? Pcs { set; get; }
        public double? Weight { set; get; }
        public double? iTotNum { set; get; }
        public int? Price { set; get; }
        public string BagNo { set; get; }
        public string CocustomTyp { set; get; }
        public string WriteOff { set; get; }
        public string Remark { set; get; }
    }
}