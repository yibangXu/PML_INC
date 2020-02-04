using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HY_PML.Models.XSLXHelper
{
    public class Titles
    {
        /// <summary>
        /// 工作表名
        /// </summary>
        public string TableName { set; get; }
        /// <summary>
        /// 大標題
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 大標題的起始點(橫)
        /// </summary>
        public int Title_sRow { set; get; }
        /// <summary>
        /// 大標題的起始點(縱)
        /// </summary>
        public int Title_sColumn { set; get; }
        /// <summary>
        /// 大標題的結束點(橫)
        /// </summary>
        public int Title_eRow { set; get; }
        /// <summary>
        /// 大標題的結束點(縱)
        /// </summary>
        public int Title_eColumn { set; get; }
        /// <summary>
        /// 欄位起始列位置
        /// </summary>
        public int Header_sIndex { set; get; }
        /// <summary>
        /// 資料起始列位置
        /// </summary>
        public int Data_sIndex { set; get; }
		/// <summary>
		/// 資料起始列高度
		/// </summary>
		public double Data_Height { set; get; }
	}
}