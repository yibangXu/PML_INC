using System.ComponentModel;

namespace HY_PML.Models.Export
{
    /// <summary>
    /// 適用於：
    /// 派件明細表-大陸、大陸特貨、香港、澳門、柬埔寨，
    /// 派件明細表-越南
    /// </summary>
    public class A_Piece
    {
        [Description("袋號")]
        public string BagNo { set; get; }
        [Description("運單號碼")]
        public string MasterNo { set; get; }
        [Description("寄件公司")]
        public string StatName { set; get; }
        [Description("收件公司")]
        public string Receiver { set; get; }
        [Description("物品名稱")]
        public string Product { set; get; }
        [Description("件數")]
        public int Pcs { set; get; }
        [Description("重量"+"\n"+"(公斤)")]
        public double Weight { set; get; }
        [Description("類別")]
        public string Type { set; get; }
        [Description("寄件地址")]
        public string MailingAddr { set; get; }
        [Description("收件電話")]
        public string Recipient { set; get; }
        [Description("收件地址")]
        public string RecipientAddr { set; get; }
        [Description("付款" + "\n" + "方式")]
        public string Cc { set; get; }
        [Description("金額")]
        public string Amount { set; get; }
        [Description("幣別")]
        public string Currency { set; get; }
        [Description("備註欄")]
        public string Remark { set; get; }
    }
}