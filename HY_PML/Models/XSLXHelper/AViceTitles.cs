using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HY_PML.Models.XSLXHelper
{
    /// <summary>
    /// 適用於：
    /// 派件明細表-大陸、大陸特貨、香港、澳門、柬埔寨，
    /// 派件明細表-越南，
    /// 清關明細表-大陸、大陸特貨、香港、澳門、柬埔寨，
    /// 清關明細表-越南、河內
    /// </summary>
    public class AViceTitles
    {
        /// <summary>
        /// 副標題
        /// </summary>
        public string ViceTitles { set; get; }
        /// <summary>
        /// 來源，預設"台南總公司"
        /// </summary>
        public string From { set; get; }
        /// <summary>
        /// 出貨日期
        /// </summary>
        public DateTime SheetDate { set; get; }
        /// <summary>
        /// 清關公司，預設"思邦"
        /// </summary>
        public string ClearanceCo { set; get; }
        /// <summary>
        /// 主提單號
        /// </summary>
        public string MasterNo { set; get; }
        /// <summary>
        /// 航班
        /// </summary>
        public string FlightNo { set; get; }
        /// <summary>
        /// 倉單日期
        /// </summary>
        public DateTime FlightDate { set; get; }
    }
}