using ClosedXML.Excel;
using HY_PML.Models.XSLXHelper;
using System;

namespace HY_PML.helper.XSLSHelper
{
	public class XSLXViceTitle
	{
		public static XLWorkbook GetViceTitle(string ViceType, dynamic data, string TableName, string ExportName, ref XLWorkbook workbook)
		{
			var sheet = workbook.Worksheet(TableName ?? ExportName);
			if (XSLXType.AType.Contains(ViceType))
			{
				AViceTitles viceTitles = data as AViceTitles;

				#region 列高 欄寬
				sheet.Row(1).Height = 17.25;
				sheet.Row(2).Height = 17.25;
				sheet.Row(3).Height = 17.25;
				sheet.Row(4).Height = 17.25;
				sheet.Row(5).Height = 17.25;
				sheet.Row(6).Height = 33.00;
				#endregion

				if (XSLXType.清關大陸 == ViceType)
				{
					sheet.Cell(2, 1).Value = viceTitles.ViceTitles;
					sheet.Range(2, 1, 2, 7).Merge().AddToNamed("ViceTitles");

					sheet.Cell(3, 1).Value = $"FROM：{ viceTitles.From ?? "台南總公司"}";
					sheet.Range(3, 1, 3, 3).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 1).Value = $"出貨日期：{viceTitles.SheetDate.ToString("yyyy.MM.dd")}";
					sheet.Range(4, 1, 4, 3).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 1).Value = $"清關公司：{ viceTitles.ClearanceCo ?? "思邦"}";
					sheet.Range(5, 1, 5, 3).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 4).Value = $"MASTER NO：{viceTitles.MasterNo}";
					sheet.Range(3, 4, 3, 7).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 4).Value = $"FLT NO：{viceTitles.FlightNo}";
					sheet.Range(4, 4, 4, 7).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 4).Value = $"FLT DATE：{viceTitles.FlightDate.ToString("yyyy.MM.dd")}";
					sheet.Range(5, 4, 5, 7).Merge().AddToNamed("ViceHeaders");

					sheet.Column(1).Width = 14.5;
					sheet.Column(2).Width = 14.5;
					sheet.Column(3).Width = 20;
					sheet.Column(4).Width = 7;
					sheet.Column(5).Width = 7;
					sheet.Column(6).Width = 7;
					sheet.Column(7).Width = 30;
				}
				else
				{
					sheet.Cell(2, 1).Value = viceTitles.ViceTitles;
					sheet.Range(2, 1, 2, 15).Merge().AddToNamed("ViceTitles");

					sheet.Cell(3, 1).Value = $"FROM：{ viceTitles.From ?? "台南總公司"}";
					sheet.Range(3, 1, 3, 5).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 1).Value = $"出貨日期：{viceTitles.SheetDate.ToString("yyyy.MM.dd")}";
					sheet.Range(4, 1, 4, 5).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 1).Value = $"清關公司：{ viceTitles.ClearanceCo ?? "思邦"}";
					sheet.Range(5, 1, 5, 5).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 6).Value = $"MASTER NO：{viceTitles.MasterNo}";
					sheet.Range(3, 6, 3, 15).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 6).Value = $"FLT NO：{viceTitles.FlightNo}";
					sheet.Range(4, 6, 4, 15).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 6).Value = $"FLT DATE：{viceTitles.FlightDate.ToString("yyyy.MM.dd")}";
					sheet.Range(5, 6, 5, 15).Merge().AddToNamed("ViceHeaders");

					sheet.Column(1).Width = 14.25;
					sheet.Column(2).Width = 14.25;
					sheet.Column(3).Width = 14.25;
					sheet.Column(4).Width = 14.25;
					sheet.Column(5).Width = 20;
					sheet.Column(6).Width = 5;
					sheet.Column(7).Width = 11.88;
					sheet.Column(8).Width = 7;
					sheet.Column(9).Width = 22.75;
					sheet.Column(10).Width = 14.5;
					sheet.Column(11).Width = 30;
					sheet.Column(12).Width = 5.25;
					sheet.Column(13).Width = 9;
					sheet.Column(14).Width = 5.75;
					sheet.Column(15).Width = 28.75;
				}
				workbook.NamedRanges.NamedRange("ViceTitles").Ranges.Style = XSLXStyle.ViceTitles(XLWorkbook.DefaultStyle, ViceType);
			}

			else if (XSLXType.BType.Contains(ViceType))
			{
				BViceTitles viceTitles = data as BViceTitles;

				#region 列高 欄寬
				sheet.Row(1).Height = 43.5;
				sheet.Row(2).Height = 15;
				sheet.Row(3).Height = 15;
				sheet.Row(4).Height = 20.25;
				sheet.Row(5).Height = 15;
				sheet.Row(6).Height = 20.25;
				sheet.Row(7).Height = 30;

				if (XSLXType.派件河內 == ViceType || XSLXType.派件印尼 == ViceType)
				{
					sheet.Column(1).Width = 10.25;
					sheet.Column(2).Width = 14.5;
					sheet.Column(3).Width = 45.88;
					sheet.Column(4).Width = 45.88;
					sheet.Column(5).Width = 5.1;
					sheet.Column(6).Width = 25.63;
					sheet.Column(7).Width = 7;
					sheet.Column(8).Width = 7;
					sheet.Column(9).Width = 9;
					sheet.Column(10).Width = 8.5;
					sheet.Column(11).Width = 8.5;
					sheet.Column(12).Width = 24.75;
					sheet.Column(13).Width = 15.25;
				}
				else if(XSLXType.清關印尼海關== ViceType)
				{
					sheet.Column(1).Width = 10.25;
					sheet.Column(2).Width = 15;
					sheet.Column(3).Width = 38;
					sheet.Column(4).Width = 38;
					sheet.Column(5).Width = 7;
					sheet.Column(6).Width = 22;
					sheet.Column(7).Width = 7;
					sheet.Column(8).Width = 7;
					sheet.Column(9).Width = 9;
					sheet.Column(10).Width = 26;
					sheet.Column(11).Width = 26;
				}
				else
				{
					sheet.Column(1).Width = 10.25;
					sheet.Column(2).Width = 15;
					sheet.Column(3).Width = 10;
					sheet.Column(4).Width = 28;
					sheet.Column(5).Width = 7;
					sheet.Column(6).Width = 7;
					sheet.Column(7).Width = 15;
				}
				#endregion

				sheet.Cell(2, 1).Value = $"CONSIGN TO：";
				sheet.Range(2, 1, 2, 2).Merge().AddToNamed("ViceHeaders");

				var ConsignTo = viceTitles.ConsignTo;
				var To = viceTitles.To;

				if (XSLXType.派件河內 == ViceType)
				{
					To = "HA NOI";
					ConsignTo = "Smart Cargo Service Co.,Ltd";
				}
				if (XSLXType.派件印尼 == ViceType || XSLXType.清關印尼海關 == ViceType || XSLXType.清關印尼 == ViceType)
				{
					To = "JKT";
					ConsignTo = "PT. TATA HARMONI SARANATAMA";
				}
				if (XSLXType.清關印尼 == ViceType)
				{
					sheet.Cell(2, 3).Value = $"{ ConsignTo }";
					sheet.Range(2, 3, 2, 4).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(2, 5).Value = $"PML EXPRESS LTD";
					sheet.Range(2, 5, 2, 7).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 1).Value = $"MAWB NO：";
					sheet.Range(3, 1, 3, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 3).Value = $"{ viceTitles.MAWBNo }";
					sheet.Range(3, 3, 3, 4).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 5).Value = $"TEL: 4128800";
					sheet.Range(3, 5, 3, 7).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 1).Value = $"FLIGHT NO：";
					sheet.Range(4, 1, 4, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 3).Value = $"{ viceTitles.FlightNo }";
					sheet.Range(4, 3, 4, 4).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 1).Value = $"FROM：";
					sheet.Range(5, 1, 5, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 3).Value = $"{ viceTitles.From ?? "TAIWAN" }";
					sheet.Range(5, 3, 5, 4).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(6, 1).Value = $"TO：";
					sheet.Range(6, 1, 6, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(6, 3).Value = $"{ To }";
					sheet.Range(6, 3, 6, 4).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(6, 5).Value = $"DATE：{viceTitles.FlightDate.ToString("yyyy.MM.dd")}";
					sheet.Range(6, 5, 6, 7).Merge().AddToNamed("ViceHeaders");
				}
				else
				{
					sheet.Cell(2, 3).Value = $"{ ConsignTo }";
					sheet.Range(2, 3, 2, 5).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(2, 6).Value = $"PML EXPRESS LTD";
					sheet.Range(2, 6, 2, 6).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 1).Value = $"MAWB NO：";
					sheet.Range(3, 1, 3, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 3).Value = $"{ viceTitles.MAWBNo }";
					sheet.Range(3, 3, 3, 5).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(3, 6).Value = $"TEL: 4128800";
					sheet.Range(3, 6, 3, 6).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 1).Value = $"FLIGHT NO：";
					sheet.Range(4, 1, 4, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(4, 3).Value = $"{ viceTitles.FlightNo }";
					sheet.Range(4, 3, 4, 4).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 1).Value = $"FROM：";
					sheet.Range(5, 1, 5, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(5, 3).Value = $"{ viceTitles.From ?? "TAIWAN" }";
					sheet.Range(5, 3, 5, 3).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(6, 1).Value = $"TO：";
					sheet.Range(6, 1, 6, 2).Merge().AddToNamed("ViceHeaders");

					sheet.Cell(6, 3).Value = $"{ To }";
					sheet.Cell(6, 3).AddToNamed("ViceHeaders");

					sheet.Cell(6, 6).Value = $"DATE：{viceTitles.FlightDate.ToString("yyyy.MM.dd")}";
					sheet.Range(6, 6, 6, 6).Merge().AddToNamed("ViceHeaders");
				}
			}

			else if (XSLXType.CType.Contains(ViceType))
			{
				sheet.Row(1).Height = 12;

				sheet.Column(1).Width = 13;
				sheet.Column(2).Width = 14;
				sheet.Column(3).Width = 36.5;
				sheet.Column(4).Width = 36.5;
				sheet.Column(5).Width = 36.5;
				sheet.Column(6).Width = 36.5;
				sheet.Column(7).Width = 3.88;
				sheet.Column(8).Width = 23.38;
				sheet.Column(9).Width = 10;
				sheet.Column(10).Width = 7.5;
				sheet.Column(11).Width = 7.5;
				sheet.Column(12).Width = 12.5;
				sheet.Column(13).Width = 12.5;
				sheet.Column(14).Width = 18;
				sheet.Column(15).Width = 18;
				sheet.Column(16).Width = 18;
				sheet.Column(17).Width = 22.5;
				sheet.Column(18).Width = 22.5;
			}

			else if (XSLXType.DType.Contains(ViceType))
			{
				CViceTitles viceTitles = data as CViceTitles;

				#region 列高 欄寬
				sheet.Row(1).Height = 36.75;
				sheet.Row(2).Height = 28.5;
				sheet.Row(3).Height = 21;

				sheet.Column(1).Width = 7.88;
				sheet.Column(2).Width = 19.5;
				sheet.Column(3).Width = 19.5;
				sheet.Column(4).Width = 7.25;
				sheet.Column(5).Width = 7.25;
				sheet.Column(6).Width = 7.25;
				sheet.Column(7).Width = 7.25;
				sheet.Column(8).Width = 7.25;
				sheet.Column(9).Width = 7.25;
				sheet.Column(10).Width = 21.75;
				sheet.Column(11).Width = 7;
				sheet.Column(12).Width = 7;
				sheet.Column(13).Width = 7;
				sheet.Column(14).Width = 17;
				#endregion

				sheet.Cell(2, 1).Value = "合約編號：";
				sheet.Range(2, 1, 2, 2).Merge().AddToNamed("ViceHeaders");

				sheet.Cell(2, 3).Value = $"{viceTitles.ContractNo ?? "遠達快遞"}";
				sheet.Range(2, 3, 2, 4).Merge().AddToNamed("ViceHeaders");

				sheet.Cell(2, 5).Value = $"總重：";
				sheet.Range(2, 5, 2, 7).Merge().AddToNamed("ViceHeaders");

				sheet.Cell(2, 8).Value = $"{Math.Round(viceTitles.Weight, 2)}";
				sheet.Range(2, 8, 2, 9).Merge().AddToNamed("ViceHeaders");

				sheet.Cell(2, 10).Value = $"日期：";
				sheet.Range(2, 10, 2, 14).Merge().AddToNamed("ViceHeaders");

				sheet.Cell(2, 11).Value = $"{viceTitles.FlightDate.Year - 1911}年{viceTitles.FlightDate.Month}月{viceTitles.FlightDate.Day}日";
				sheet.Range(2, 11, 2, 14).Merge().AddToNamed("ViceHeaders");

				sheet.Row(2).Height = 28.5;
			}

			if (ViceType != XSLXType.印尼海關明細表)
				workbook.NamedRanges.NamedRange("ViceHeaders").Ranges.Style = XSLXStyle.ViceHeaders(XLWorkbook.DefaultStyle, ViceType);
			return workbook;
		}
	}
}