using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HY_PML.Models.XSLXHelper
{
    /// <summary>
    /// 適用於：
    /// 派件明細表-河內，
    /// 派件明細表-印尼，
    /// 清關明細表-印尼，
    /// 清關明細表-印尼海關
    /// </summary>
    public class BViceTitles
    {
        /// <summary>
        /// 快遞公司，
        /// 派件河內固定為Smart Cargo Service Co.,Ltd，
        /// 派件印尼及清關印尼海關固定為PT. TATA HARMONI SARANATAMA
        /// </summary>
        public string ConsignTo { set; get; }
        /// <summary>
        /// 航空公司運單
        /// </summary>
        public string MAWBNo { set; get; }
        /// <summary>
        /// 航班
        /// </summary>
        public string FlightNo { set; get; }
        /// <summary>
        /// 發送地，預設TAIWAN
        /// </summary>
        public String From { set; get; }
        /// <summary>
        /// 目的地
        /// </summary>
        public string To { set; get; }
        /// <summary>
        /// 倉單日期
        /// </summary>
        public DateTime FlightDate { set; get; }
    }
}