using ClosedXML.Excel;

namespace HY_PML.helper
{
	public class XSLXStyle
	{
		/// <summary>
		/// 主標題樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle Titles(IXLStyle Style, string Type)
		{
			if (XSLXType.AType.Contains(Type))
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			}
			else if (XSLXType.BType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 36;
				Style.Font.FontName = "Arial";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			}
			else if (XSLXType.CType.Contains(Type))
			{
				//No Titles
			}
			else if (XSLXType.DType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 26;
				Style.Font.FontName = "華康細圓體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			}
			else
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			}
			return Style;
		}
		/// <summary>
		/// 副標題樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle ViceTitles(IXLStyle Style, string Type)
		{
			if (XSLXType.AType.Contains(Type))
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			}
			else if (XSLXType.BType.Contains(Type))
			{
				//No ViceTitles
			}
			else if (XSLXType.CType.Contains(Type))
			{
				//No ViceTitles
			}
			else if (XSLXType.DType.Contains(Type))
			{
				//No ViceTitles
			}
			else
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			}
			return Style;
		}
		/// <summary>
		/// 副抬頭樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle ViceHeaders(IXLStyle Style, string Type)
		{
			if (XSLXType.AType.Contains(Type))
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Times New Roman";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
			}
			else if (XSLXType.BType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Arial";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
			}
			else if (XSLXType.CType.Contains(Type))
			{
				//No ViceHeaders
			}
			else if (XSLXType.DType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 18;
				Style.Font.FontName = "華康細圓體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
			}
			else
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Times New Roman";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
			}
			return Style;
		}
		/// <summary>
		/// 資料抬頭樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle Headers(IXLStyle Style, string Type)
		{
			if (XSLXType.AType.Contains(Type))
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.BType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Arial";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.CType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 9;
				Style.Font.FontName = "Calibri";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.DType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 16;
				Style.Font.FontName = "華康細圓體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}

			Style.Border.TopBorder = XLBorderStyleValues.Thin;
			Style.Border.TopBorderColor = XLColor.Black;
			Style.Border.LeftBorder = XLBorderStyleValues.Thin;
			Style.Border.LeftBorderColor = XLColor.Black;
			Style.Border.BottomBorder = XLBorderStyleValues.Thin;
			Style.Border.BottomBorderColor = XLColor.Black;
			Style.Border.RightBorder = XLBorderStyleValues.Thin;
			Style.Border.RightBorderColor = XLColor.Black;
			Style.Alignment.WrapText = true;

			return Style;
		}
		/// <summary>
		/// 資料抬頭額外樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle HeadersOth(IXLStyle Style, string Type)
		{
			if (XSLXType.DType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "華康細圓體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else
			{
				Style.Font.Bold = true;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}

			Style.Border.TopBorder = XLBorderStyleValues.Thin;
			Style.Border.TopBorderColor = XLColor.Black;
			Style.Border.LeftBorder = XLBorderStyleValues.Thin;
			Style.Border.LeftBorderColor = XLColor.Black;
			Style.Border.BottomBorder = XLBorderStyleValues.Thin;
			Style.Border.BottomBorderColor = XLColor.Black;
			Style.Border.RightBorder = XLBorderStyleValues.Thin;
			Style.Border.RightBorderColor = XLColor.Black;

			return Style;
		}
		/// <summary>
		/// 資料樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle Datas(IXLStyle Style, string Type)
		{
			if (XSLXType.AType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.BType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Arial";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.CType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Arial";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.DType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "華康細圓體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}

			Style.Border.TopBorder = XLBorderStyleValues.Thin;
			Style.Border.TopBorderColor = XLColor.Black;
			Style.Border.LeftBorder = XLBorderStyleValues.Thin;
			Style.Border.LeftBorderColor = XLColor.Black;
			Style.Border.BottomBorder = XLBorderStyleValues.Thin;
			Style.Border.BottomBorderColor = XLColor.Black;
			Style.Border.RightBorder = XLBorderStyleValues.Thin;
			Style.Border.RightBorderColor = XLColor.Black;
			Style.Alignment.WrapText = true;
			return Style;
		}
		/// <summary>
		/// Total樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle Totals(IXLStyle Style, string Type)
		{
			if (XSLXType.AType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.BType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Arial";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.CType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "Arial";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else if (XSLXType.DType.Contains(Type))
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "華康細圓體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}
			else
			{
				Style.Font.Bold = false;
				Style.Font.FontSize = 12;
				Style.Font.FontName = "新細明體";
				Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			}

			return Style;
		}
		/// <summary>
		/// Other樣式
		/// </summary>
		/// <param name="Style"></param>
		/// <returns></returns>
		public static IXLStyle Other(IXLStyle Style, string Type)
		{
			Style.Font.Bold = false;
			Style.Font.FontSize = 20;
			Style.Font.FontName = "Arial";
			Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			Style.Alignment.WrapText = true;
			return Style;
		}

	}
}