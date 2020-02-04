using ClosedXML.Excel;
using HY_PML.helper.XSLSHelper;
using HY_PML.Models.XSLXHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Web;

namespace HY_PML.helper
{
	public class XSLXHelper
	{
		/// <summary>
		/// 產生 excel
		/// </summary>
		/// <typeparam name="T">傳入的物件型別</typeparam>
		/// <param name="data">物件資料集</param>
		/// <returns></returns>
		public void Export<T>(List<T> data, TotalsData totalsData, string ExportName, string Type, dynamic ViceTitles = null)
		{
			var titles = XSLXTitles.GetTitles(Type);

			//建立 excel 物件
			XLWorkbook workbook = new XLWorkbook();
			//加入 excel 工作表名
			var sheet = workbook.Worksheets.Add(titles.TableName ?? ExportName);
			//欄位起啟位置
			int colIdx = 1;
			//使用 reflection 將物件屬性取出當作工作表欄位名稱
			foreach (var item in typeof(T).GetProperties())
			{
				#region - 可以使用 DescriptionAttribute 設定，找不到 DescriptionAttribute 時改用屬性名稱 -

				if (Type == XSLXType.JEMS出貨明細表)
				{
					DescriptionAttribute description = item.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
					sheet.Cell(titles.Header_sIndex, colIdx).Value = description?.Description ?? item.Name;
					if (colIdx == 2)
					{
						sheet.Cell(titles.Header_sIndex, colIdx).AddToNamed("HeadersOth");
						workbook.NamedRanges.NamedRange("HeadersOth").Ranges.Style = XSLXStyle.HeadersOth(XLWorkbook.DefaultStyle, Type);
					}
					else
					{
						sheet.Cell(titles.Header_sIndex, colIdx).AddToNamed("Headers");
						workbook.NamedRanges.NamedRange("Headers").Ranges.Style = XSLXStyle.Headers(XLWorkbook.DefaultStyle, Type);
					}
					colIdx++;
				}
				else
				{
					DescriptionAttribute description = item.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
					sheet.Cell(titles.Header_sIndex, colIdx).Value = description?.Description ?? item.Name;
					sheet.Cell(titles.Header_sIndex, colIdx).AddToNamed("Headers");
					workbook.NamedRanges.NamedRange("Headers").Ranges.Style = XSLXStyle.Headers(XLWorkbook.DefaultStyle, Type);
					colIdx++;
				}
				#endregion
			}

			//Title 位置
			if (titles.Title.IsNotEmpty())
			{
				sheet.Cell(titles.Title_sRow, titles.Title_sColumn).Value = titles.Title;
				sheet.Range(titles.Title_sRow, titles.Title_sColumn, titles.Title_eRow, titles.Title_eColumn).Merge().AddToNamed("Titles");
				workbook.NamedRanges.NamedRange("Titles").Ranges.Style = XSLXStyle.Titles(XLWorkbook.DefaultStyle, Type);
			}

			//副標題
			XSLXViceTitle.GetViceTitle(Type, ViceTitles, titles.TableName, ExportName, ref workbook);

			//Total
			if (totalsData != null)
			{
				var dataCount = data.Count;
				XSLXTotals.GetTotals(Type, totalsData, titles.TableName, ExportName, dataCount, ref workbook);
				if (XSLXType.BType.Contains(Type))
				{
					XSLXOthers.GetOthers(Type, titles.TableName, ExportName, dataCount, totalsData, ViceTitles.MAWBNo, ref workbook);
				}
			}

			//資料起始列位置
			int rowIdx = titles.Data_sIndex;
			foreach (var item in data)
			{
				//每筆資料欄位起始位置
				int conlumnIndex = 1;
				foreach (var jtem in item.GetType().GetProperties())
				{
					//將資料內容加上 "'" 避免受到 excel 預設格式影響，並依 row 及 column 填入
					sheet.Row(rowIdx).Height = titles.Data_Height;
					sheet.Cell(rowIdx, conlumnIndex).Value = string.Concat("'", Convert.ToString(jtem.GetValue(item, null)));
					sheet.Cell(rowIdx, conlumnIndex).AddToNamed("Datas");
					workbook.NamedRanges.NamedRange("Datas").Ranges.Style = XSLXStyle.Datas(XLWorkbook.DefaultStyle, Type);
					conlumnIndex++;
				}
				rowIdx++;
			}
			//xlsx 檔案位置
			//string filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $@"\{ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
			//string filepath = "c:\\downloadExcel\\" + $@"\{ ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
			//workbook.SaveAs(filepath);
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.Buffer = true;
			HttpContext.Current.Response.Charset = "";
			HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			HttpContext.Current.Response.AddHeader("content-disposition", $@"attachment;filename={ ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
			using (MemoryStream MyMemoryStream = new MemoryStream())
			{
				workbook.SaveAs(MyMemoryStream);
				MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
				HttpContext.Current.Response.Flush();
				HttpContext.Current.Response.End();
			}
		}
	}
}