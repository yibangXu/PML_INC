namespace HY_PML.Models.XSLXHelper
{
    public class TotalsData
    {
        /// <summary>
        /// 件數
        /// </summary>
        public int Pcs { set; get; }
        /// <summary>
        /// 重量(小數點後一位)
        /// </summary>
        public double Weight { set; get; }
        /// <summary>
        /// 資料筆數
        /// </summary>
        public int Count { set; get; }
		/// <summary>
		/// PP
		/// </summary>
		public decimal PP { set; get; }
		/// <summary>
		/// CC
		/// </summary>
		public decimal CC { set; get; }
		/// <summary>
		/// Amount
		/// </summary>
		public decimal Amount { set; get; }
		/// <summary>
		/// TPE件數
		/// </summary>
		public int pPcs { set; get; }
		/// <summary>
		/// TAO件數
		/// </summary>
		public int aPcs { set; get; }
		/// <summary>
		/// TXG件數
		/// </summary>
		public int xPcs { set; get; }
		/// <summary>
		/// TNN件數
		/// </summary>
		public int tPcs { set; get; }
		/// <summary>
		/// KHH件數
		/// </summary>
		public int kPcs { set; get; }
	}
}