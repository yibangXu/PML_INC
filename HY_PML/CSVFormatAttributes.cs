using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace HU.CSVFormatAttributes
{
	/// <summary>
	/// 輸出CSV格式檔案所需
	/// Ex. 欄位名稱 : [CSVFieldName(fieldName)]
	/// Ex. 欄位格式 : [CSVDateTimeOnly] / [CSVDateOnly]
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	sealed class CSVHiddenFieldAttribute : Attribute { }

	[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	sealed class CSVFieldNameAttribute : Attribute
	{
		public string FieldName { set; get; }
		public CSVFieldNameAttribute(string fieldName)
		{
			FieldName = fieldName;
		}
	}

	[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	abstract class CSVFormatBaseAttribute : Attribute
	{
		internal string PropertyName;
		public CSVFormatBaseAttribute([CallerMemberName] string propertyName = null) { PropertyName = propertyName; }
		public abstract string GetFormatValue<T>(T value);
	}

	sealed class CSVDateOnly : CSVFormatBaseAttribute
	{
		public override string GetFormatValue<T>(T value)
		{
			if (value != null && value.GetType() == typeof(DateTime))
				return ((DateTime)(object)value).ToString("yyyy/MM/dd");
			else if (value != null && value.GetType() == typeof(DateTime?))
				return ((DateTime?)(object)value)?.ToString("yyyy/MM/dd");
			else if (value == null)
				return "";
			else
				throw new ArgumentException("此格式化屬性僅可用於DateTime，請檢查" + PropertyName + "欄位的屬性");
		}
	}

	sealed class CSVDateTimeOnly : CSVFormatBaseAttribute
	{
		public override string GetFormatValue<T>(T value)
		{
			if (value != null && value.GetType() == typeof(DateTime))
				return ((DateTime)(object)value).ToString("yyyy-MM-dd HH:mm:ss");
			else if (value != null && value.GetType() == typeof(DateTime?))
				return ((DateTime?)(object)value)?.ToString("yyyy-MM-dd HH:mm:ss");
			else if (value == null)
				return "";
			else
				throw new ArgumentException("此格式化屬性僅可用於DateTime，請檢查" + PropertyName + "欄位的屬性");
		}
	}
}