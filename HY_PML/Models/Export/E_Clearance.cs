using System.ComponentModel;

namespace HY_PML.Models.Export
{
    /// <summary>
    /// 適用於：
    /// 清關明細表-印尼
    /// </summary>
    public class E_Clearance
    {
        [Description("No")]
        public string BagNo { set; get; }
        [Description("Hawb No")]
        public string MasterNo { set; get; }
        [Description("PKG")]
        public int Pkg { set; get; }
        [Description("DESCRIPTION")]
        public string Product { set; get; }
        [Description("PCS")]
        public int Pcs { set; get; }
        [Description("KG")]
        public double Weight { set; get; }
        [Description("STATION")]
        public string Remark2 { set; get; }
    }
}