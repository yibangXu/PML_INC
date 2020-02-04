using System.ComponentModel;

namespace HY_PML.Models.Export
{
    /// <summary>
    /// 適用於：
    /// 清關明細表-大陸、大陸特貨、香港、澳門、柬埔寨，
    /// 清關明細表-越南、河內
    /// </summary>
    public class D_Clearance
    {
        [Description("袋號")]
        public string BagNo { set; get; }
        [Description("運單號碼")]
        public string MasterNo { set; get; }
        [Description("物品名稱")]
        public string Product { set; get; }
        [Description("件數")]
        public int Pcs { set; get; }
        [Description("重量" + "\n" + "(公斤)")]
        public double Weight { set; get; }
        [Description("類別")]
        public string Type { set; get; }
        //public string Currency { set; get; }
        [Description("備註欄")]
        public string Remark { set; get; }
    }
}