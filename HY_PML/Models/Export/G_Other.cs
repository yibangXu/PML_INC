using System.ComponentModel;

namespace HY_PML.Models.Export
{
    /// <summary>
    /// 適用於：
    /// 印尼海關明細表（Tata_EXPRESS_form_Final）
    /// </summary>
    public class G_Other
    {
		[Description("No")]
		public string BagNo { set; get; }
		[Description("Hawb No")]
		public string MasterNo { set; get; }
		[Description("Shipper Name")]
		public string SendCHName { set; get; }
		[Description("Shipper Address")]
		public string SendCustAddr { set; get; }
		[Description("Consignee Name")]
		public string RecCompany { set; get; }
		[Description("Consignee Address")]
		public string RecChAddr { set; get; }
		[Description("PKG")]
		public int Pkg { set; get; }
		[Description("DESCRIPTION")]
		public string Product { set; get; }
		[Description("HS CODE")]
		public string HSNo { set; get; }
		[Description("PCS")]
		public int Pcs { set; get; }
		[Description("KG")]
		public double Weight { set; get; }
		[Description("FOB Value")]
		public string DtlPrice { set; get; }
		[Description("CN")]
		public string Remark { set; get; }
		[Description("Consignee City")]
		public string RecCity { set; get; }
		[Description("Consignee Region")]
		public string RecState { set; get; }
		[Description("Consignee ZIP Code")]
		public string RecPostDist { set; get; }
		[Description("Contact Person")]
		public string RecBy { set; get; }
		[Description("Contact Phone")]
		public string RecPhone { set; get; }
	}
}