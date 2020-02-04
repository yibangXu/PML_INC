using System;

namespace HY_PML.Models
{
    public class ShdetVehicle
    {
        public string ShdetNo { get; set; }
        public string CustNo { get; set; }
        public int sDtlNo { get; set; }
        public int sNo { get; set; }
        public string CarID { get; set; }
        public string Phone { get; set; }
        public string CarKind { get; set; }
        public string CarNo { get; set; }
        public int? LoadSafety { get; set; }
        public double? WeigTotal { get; set; }
        public double? ValueSafe { get; set; }
        public double? ValueTotal { get; set; }
        public int? Count { get; set; }
        public bool OverWeig { set; get; }
        public bool OveriTotNum { set; get; }
        public int? TotalSheetNo { set; get; }
		public DateTime? PhoneCheckTime { set; get; }
		public string Status { set; get; }
		public DateTime? StatusTime { set; get; }
	}
}