using System.Collections.Generic;

namespace HY_PML.helper
{
    public class XSLXType
    {
        /// <summary>
        /// 派件明細表-大陸、大陸特貨、香港、澳門、柬埔寨
        /// </summary>
        public static readonly string 派件大陸 = "派件大陸";
        /// <summary>
        /// 派件明細表-越南
        /// </summary>
        public static readonly string 派件越南 = "派件越南";
        /// <summary>
        /// 派件明細表-河內
        /// </summary>
        public static readonly string 派件河內 = "派件河內";
        /// <summary>
        /// 派件明細表-印尼
        /// </summary>
        public static readonly string 派件印尼 = "派件印尼";

        /// <summary>
        /// 清關明細表-大陸、大陸特貨、香港、澳門、柬埔寨
        /// </summary>
        public static readonly string 清關大陸 = "清關大陸";
        /// <summary>
        /// 清關明細表-越南、河內
        /// </summary>
        public static readonly string 清關越南 = "清關越南";
        /// <summary>
        /// 清關明細表-印尼海關
        /// </summary>
        public static readonly string 清關印尼海關 = "清關印尼海關";
        /// <summary>
        /// 清關明細表-印尼
        /// </summary>
        public static readonly string 清關印尼 = "清關印尼";
        /// <summary>
        /// 印尼海關明細表
        /// </summary>
        public static readonly string 印尼海關明細表 = "印尼海關明細表";
        /// <summary>
        /// J.EMS出貨明細表
        /// </summary>
        public static readonly string JEMS出貨明細表 = "JEMS出貨明細表";


        /// <summary>
        /// 適用於：
        /// 派件明細表-大陸、大陸特貨、香港、澳門、柬埔寨，
        /// 派件明細表-越南，
        /// 清關明細表-大陸、大陸特貨、香港、澳門、柬埔寨，
        /// 清關明細表-越南、河內
        /// </summary>
        public static readonly List<string> AType = new List<string>() { XSLXType.派件大陸, XSLXType.派件越南, XSLXType.清關大陸, XSLXType.清關越南 };
        /// <summary>
        /// 適用於：
        /// 派件明細表-河內，
        /// 派件明細表-印尼，
        /// 清關明細表-印尼，
        /// 清關明細表-印尼海關
        /// </summary>
        public static readonly List<string> BType = new List<string>() { XSLXType.派件河內, XSLXType.派件印尼, XSLXType.清關印尼, XSLXType.清關印尼海關 };
        /// <summary>
        /// 適用於：
        /// 印尼海關明細表
        /// </summary>
        public static readonly List<string> CType = new List<string>() { XSLXType.印尼海關明細表 };
        /// <summary>
        /// 適用於：
        /// J.EMS出貨明細表
        /// </summary>
        public static readonly List<string> DType = new List<string>() { XSLXType.JEMS出貨明細表 };

    }
}