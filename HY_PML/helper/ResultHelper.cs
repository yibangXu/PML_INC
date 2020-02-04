using Newtonsoft.Json;

namespace HY_PML.helper
{
    public partial class ResultHelper
    {
        /// <summary>
		/// 執行結果代碼
		/// </summary>
		[JsonProperty("ok")]
        public DataModifyResultType Ok { set; get; }
        /// <summary>
		/// 執行結果訊息，一般而言只有錯誤時才會輸出訊息
		/// </summary>
		[JsonProperty("message")]
        public string Message { set; get; }
        /// <summary>
		/// 總頁數
		/// </summary>
		[JsonProperty("total")]
        public int TotalPage { set; get; }
        /// <summary>
        /// 總筆數
        /// </summary>
        [JsonProperty("records")]
        public int Records { set; get; }
        /// <summary>
        /// 目前頁數
        /// </summary>
        [JsonProperty("page")]
        public int Pages { set; get; }
        /// <summary>
        /// 查訊或執行後的資料返回
        /// </summary>
        [JsonProperty("rows")]
        public object Data { set; get; }
    }
}