using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HY_PML.Models.XSLXHelper
{
    /// <summary>
    /// 適用於：
    /// J.EMS出貨明細表
    /// </summary>
    public class CViceTitles
    {
        /// <summary>
        /// 合約編號，預設"遠達快遞"
        /// </summary>
        public string ContractNo { set; get; }
        /// <summary>
        /// 總重(小數點後兩位)
        /// </summary>
        public double Weight { set; get; }
        /// <summary>
        /// 倉單日期
        /// </summary>
        public DateTime FlightDate { set; get; }
    }
}