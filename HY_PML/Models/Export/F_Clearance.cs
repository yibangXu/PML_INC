using System.ComponentModel;

namespace HY_PML.Models.Export
{
    /// <summary>
    /// 適用於：
    /// 清關明細表-印尼海關
    /// </summary>
    public class F_Clearance
    {
        [Description("No")]
        public string BagNo { set; get; }
        [Description("Hawb No")]
        public string MasterNo { set; get; }
        [Description("SHIPPER")]
        public string Sender { set; get; }
        [Description("CNEE")]
        public string Receiver { set; get; }
        [Description("PKG")]
        public int Pkg { set; get; }
        [Description("DESCRIPTION")]
        public string Product { set; get; }
        [Description("PCS")]
        public int Pcs { set; get; }
        [Description("KG")]
        public double Weight { set; get; }
        [Description("VALUE" + "\n" + "(USD)")]
        public string UnitPrice { set; get; }
        [Description("Remark")]
        public string Remark { set; get; }
        [Description("STATION")]
        public string Remark2 { set; get; }
    }
}