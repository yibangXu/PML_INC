namespace HY_PML.Models.XSLXHelper
{
    public class Totals
    {
        /// <summary>
        /// 起始點與結束點(橫)
        /// </summary>
        public int Row { set; get; }

        #region TOTAL標題
        /// <summary>
        /// TOTAL標題的起始點(縱)
        /// </summary>
        public int Label_sColumn { set; get; }
        /// <summary>
        /// TOTAL標題的結束點(縱)
        /// </summary>
        public int Label_eColumn { set; get; }
        #endregion

        #region TOTAL值
        /// <summary>
        /// 總共有幾袋(件數)
        /// </summary>
        public int Total { set; get; }
       
        /// <summary>
        /// TOTAL值的起始點(縱)
        /// </summary>
        public int Total_sColumn { set; get; }
        /// <summary>
        /// TOTAL值的結束點(縱)
        /// </summary>
        public int Total_eColumn { set; get; }
		#endregion

		#region 件數
		/// <summary>
		/// 件數
		/// </summary>
		public double Pcs { set; get; }
		/// <summary>
		/// 件數的起始點(縱)
		/// </summary>
		public int Pcs_sColumn { set; get; }
        /// <summary>
        /// 件數的結束點(縱)
        /// </summary>
        public int Pcs_eColumn { set; get; }
        #endregion

        #region 重量
        /// <summary>
        /// 重量(小數點後一位)
        /// </summary>
        public double Weight { set; get; }
        /// <summary>
        /// 重量的起始點(縱)
        /// </summary>
        public int Weight_sColumn { set; get; }
        /// <summary>
        /// 重量的結束點(縱)
        /// </summary>
        public int Weight_eColumn { set; get; }
		#endregion

		#region PP
		/// <summary>
		/// PP(小數點後一位)
		/// </summary>
		public decimal PP { set; get; }
		/// <summary>
		/// 重量的起始點(縱)
		/// </summary>
		public int PP_sColumn { set; get; }
		/// <summary>
		/// 重量的結束點(縱)
		/// </summary>
		public int PP_eColumn { set; get; }
		#endregion

		#region CC
		/// <summary>
		/// PP(小數點後一位)
		/// </summary>
		public decimal CC { set; get; }
		/// <summary>
		/// 重量的起始點(縱)
		/// </summary>
		public int CC_sColumn { set; get; }
		/// <summary>
		/// 重量的結束點(縱)
		/// </summary>
		public int CC_eColumn { set; get; }
		#endregion

		#region Amount
		/// <summary>
		/// 金額
		/// </summary>
		public decimal Amount { set; get; }
		/// <summary>
		/// 金額的起始點(縱)
		/// </summary>
		public int Amount_sColumn { set; get; }
		/// <summary>
		/// 金額的結束點(縱)
		/// </summary>
		public int Amount_eColumn { set; get; }
		#endregion

	}
}