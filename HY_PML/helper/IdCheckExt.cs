using System.Text.RegularExpressions;

namespace HY_PML.helper
{
    public static class IdCheckExt
	{
		/// <summary>
		/// 身分證驗證
		/// </summary>
		/// <param name="id">身分證字號</param>
		/// <returns>驗證結果</returns>
		public static bool CheckIdCardNumber(string id)
		{
			var d = false;
			if (id.Length == 10)
			{
				id = id.ToUpper();
				if (id[0] >= 0x41 && id[0] <= 0x5A)
				{
					var a = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
					var b = new int[11];
					b[1] = a[(id[0]) - 65] % 10;
					var c = b[0] = a[(id[0]) - 65] / 10;
					for (var i = 1; i <= 9; i++)
					{
						b[i + 1] = id[i] - 48;
						c += b[i] * (10 - i);
					}
					if (((c % 10) + b[10]) % 10 == 0)
					{
						d = true;
					}
				}
			}

			return d;
		}

		/// <summary>
		/// 外來人口統一證號驗證
		/// </summary>
		/// <param name="id">證號</param>
		/// <returns>驗證結果</returns>
		public static bool CheckForeignIdNumber(this string id)
		{
			// 基礎檢查 「任意1個字母」+「A~D其中一個字母」+「8個數字」
			if (!Regex.IsMatch(id, @"^[A-Za-z][A-Da-d]\d{8}$")) return false;
			id = id.ToUpper();

			// 縣市區域碼
			var cityCode = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
			// 計算時使用的容器，最後一個位置拿來放檢查碼，所以有11個位置(縣市區碼佔2個位置)
			var valueContainer = new int[11];
			valueContainer[0] = cityCode[id[0] - 65] / 10; // 區域碼十位數
			valueContainer[1] = cityCode[id[0] - 65] % 10; // 區域碼個位數
			valueContainer[2] = cityCode[id[1] - 65] % 10; // 性別碼個位數
																	  // 證號執行特定數規則所產生的結果值的加總，這裡把初始值訂為區域碼的十位數數字(特定數為1，所以不用乘)
			var sumVal = valueContainer[0];

			// 迴圈執行特定數規則
			for (var i = 1; i <= 9; i++)
			{
				// 跳過性別碼，如果是一般身分證字號則不用跳過
				if (i > 1)
					// 將當前證號於索引位置的數字放到容器的下一個索引的位置
					valueContainer[i + 1] = id[i] - 48;

				// 特定數為: 1987654321 ，因為首個數字1已經在sumVal初始值算過了，所以這裡從9開始
				sumVal += valueContainer[i] * (10 - i);
			}

			// 此為「檢查碼 = 10 - 總和值的個位數數字 ; 若個位數為0則取0為檢查碼」的反推
			if ((sumVal + valueContainer[10]) % 10 == 0) return true;
			return false;
		}


		/// <summary>
		/// 統一編號驗證
		/// </summary>
		/// <param name="id">統一編號</param>
		/// <returns>驗證結果</returns>
		public static bool CheckInvNumber(this string InvNo)
		{
			int[] cx = new int[8];
			cx[0] = 1;
			cx[1] = 2;
			cx[2] = 1;
			cx[3] = 2;
			cx[4] = 1;
			cx[5] = 2;
			cx[6] = 4;
			cx[7] = 1;
			if (InvNo.Length != 8)
				return false;
			int[] cnum = new int[8];
			cnum[0] = int.Parse(InvNo.Substring(0, 1));
			cnum[1] = int.Parse(InvNo.Substring(1, 1));
			cnum[2] = int.Parse(InvNo.Substring(2, 1));
			cnum[3] = int.Parse(InvNo.Substring(3, 1));
			cnum[4] = int.Parse(InvNo.Substring(4, 1));
			cnum[5] = int.Parse(InvNo.Substring(5, 1));
			cnum[6] = int.Parse(InvNo.Substring(6, 1));
			cnum[7] = int.Parse(InvNo.Substring(7, 1));

			int SUM = 0;
			for (int i = 0; i <= 7; i++)
			{
				SUM += cc(cnum[i] * cx[i]);
			}

			if (SUM % 10 == 0)
				return true;
			else if (cnum[6] == 7 && (SUM + 1) % 10 == 0)
				return true;
			else
				return false;
		}
		public static int cc(int n)
		{
			if (n > 9)
			{
				string n1 = n.ToString().Substring(0, 1);
				string n2 = n.ToString().Substring(1, 1);
				n = int.Parse(n1) + int.Parse(n2);
				return n;
			}
			return n;
		}
	}
}
