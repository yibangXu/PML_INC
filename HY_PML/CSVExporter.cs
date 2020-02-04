using System.Collections.Generic;
using System.Linq;
using HU.CSVFormatAttributes;
using System.Reflection;

namespace HUSite
{
	/// <summary>
	/// 輸出CSV格式
	/// </summary>
	public static class CSVExporter
	{
		private static string Or(bool allowEmpty,params string[] values)
		{
			foreach (var v in values)
			{
				if (v == null)
					continue;
				if (allowEmpty || v != "")
					return v;
			}
			return "";
		}

		public static string ExportCSVContent<T>(IEnumerable<T> data)
		{
			var sb = new System.Text.StringBuilder();
			//防止亂碼
			sb.Append("\uFEFF");

			var fields = typeof(T).GetProperties().Where(x => x.GetCustomAttributes(typeof(CSVHiddenFieldAttribute), true).Length == 0);
			foreach (var f in fields)
			{
				sb.Append("\"");
				var title = f.GetCustomAttribute<CSVFieldNameAttribute>()?.FieldName;
				if (string.IsNullOrEmpty(title))
					title = f.Name;
				sb.Append(title);
				sb.Append("\"");
				sb.Append(",");
			}
			sb.AppendLine();
			foreach (var row in data)
			{
				foreach (var f in fields)
				{
					var value = f.GetValue(row);
					var formatted_value =Or(true, f.GetCustomAttribute<CSVFormatBaseAttribute>()?.GetFormatValue(value),value?.ToString()).Replace("\"", "\"\"");
					//文字格式輸出 前面加=
					//if (!(formatted_value.Contains(",") || formatted_value.Contains("\n")))
					//	sb.Append("=");

					//只要RFID TAG前加=
					if (f.Name == "Batch_ID" ||f.Name=="Shoe_Barcode")
						sb.Append("=");

					sb.Append("\"");
					sb.Append(formatted_value);
					sb.Append("\"");
					sb.Append(",");
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}
}