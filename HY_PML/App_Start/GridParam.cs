using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
	public class GridParam
	{
        private int _page=1;
        /// <summary>
        /// 取得或設定頁數
        /// </summary>
		public int page { set { _page = Math.Max(value, 1); } get { return _page; } }

        /// <summary>
        /// 取得或設定每頁資料筆數
        /// </summary>
        public int rows { set; get; }

        /// <summary>
        /// 取得或設定排序欄位(可使用"@@"兩個@號分隔欄位做多欄排序)
        /// </summary>
        public string sidx { set; get; }

        /// <summary>
        /// 正排序true / 反排序false
        /// </summary>
        private bool _sord = true;
        /// <summary>
        /// 取得或設定正或逆排序
        /// </summary>
        public string sord { set { _sord = value != "desc"; } get { return _sord ? "asc" : "desc"; } }

        public bool _search { set; get; }

		private Int64 _recordAmout;
		public Int64 getRecordAmout()
		{
			return _recordAmout;
		}
		public void setRecordAmout(Int64 value)
		{
			_recordAmout = value;
		}


    }
}
