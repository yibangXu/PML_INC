using System.ComponentModel;

namespace HY_PML.Models.Export
{
	/// <summary>
	/// 適用於：
	/// J.EMS出貨明細表
	/// </summary>
	public class H_Other
	{
		[Description("編號")]
		public string BagNo { set; get; }
		[Description("郵件編號(B/L NO)")]
		public string LadingNo { set; get; }
		[Description("寄件人")]
		public string SendCHName { set; get; }
		[Description("重量")]
		public string GrossWeight { set; get; }
		[Description("長")]
		public string Length { set; get; }
		[Description("寬")]
		public string Width { set; get; }
		[Description("高")]
		public string Height { set; get; }
		[Description("材積")]
		public string Cuft { set; get; }
		[Description("簡碼")]
		public string DestNo { set; get; }
		[Description("目的地(國家)")]
		public string CName { set; get; }
		[Description("文件")]
		public string Dox { set; get; }
		[Description("包裹")]
		public string Spx { set; get; }
		[Description("件數")]
		public string Pcs { set; get; }
		[Description("郵局簽收人")]
		public string Signer { set; get; }
	}
}