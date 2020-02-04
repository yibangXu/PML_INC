namespace HY_PML.helper
{
    public enum DataModifyResultType
    {
        /// <summary>
		/// 成功但是警告
		/// </summary>
		Warning = 0,
		/// <summary>
		/// 成功
		/// </summary>
		Success = 1,
		/// <summary>
		/// 失敗
		/// </summary>
		Faild = -1,

		#region DataSave 資料儲存 -1xx
		/// <summary>
		/// 資料驗證錯誤
		/// </summary>
		DataInvalid = -101,
		/// <summary>
		/// 資料欄位重複
		/// </summary>
		DataDuplicate = -102,
		/// <summary>
		/// 資料欄位仍有關聯
		/// </summary>
		DataRelated = -103,
		#endregion

		#region DataSearch 資料搜尋 -2xx
		/// <summary>
		/// 找不到對應的資料
		/// </summary>
		DataNotFound = -201,
		#endregion

		#region Permission 權限 -5xx
		/// <summary>
		/// 未登入
		/// </summary>
		Unauthorized = -501,
		/// <summary>
		/// 權限不足
		/// </summary>
		UnauthorizedAccess = -502
		#endregion
    }
}