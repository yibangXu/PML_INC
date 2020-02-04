using ClosedXML.Excel;
using HY_PML.Models.XSLXHelper;
using System;

namespace HY_PML.helper
{
	public class XSLXTotals
	{
		public static Totals GetTotalsModel(string Type, TotalsData totalsData, int DataCount)
		{
			if (Type == XSLXType.派件大陸)
			{
				var rowIndex = 6 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 2,
					Label_eColumn = 2,

					Total = totalsData.Pcs,
					Total_sColumn = 3,
					Total_eColumn = 3,

					Pcs = totalsData.Pcs,
					Pcs_sColumn = 6,
					Pcs_eColumn = 6,

					Weight = Math.Round(totalsData.Weight, 1),
					Weight_sColumn = 7,
					Weight_eColumn = 7,

					PP_sColumn = -1,
					PP_eColumn = -1,

					CC_sColumn = -1,
					CC_eColumn = -1,

					Amount = totalsData.Amount,
					Amount_sColumn = 13,
					Amount_eColumn = 13,
				};
			}

			if (Type == XSLXType.派件越南)
			{
				var rowIndex = 6 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 2,
					Label_eColumn = 2,

					Total = totalsData.Pcs,
					Total_sColumn = 3,
					Total_eColumn = 3,

					Pcs = totalsData.Pcs,
					Pcs_sColumn = 6,
					Pcs_eColumn = 6,

					Weight = Math.Round(totalsData.Weight, 1),
					Weight_sColumn = 7,
					Weight_eColumn = 7,

					PP_sColumn = -1,
					PP_eColumn = -1,

					CC_sColumn = -1,
					CC_eColumn = -1,

					Amount = totalsData.Amount,
					Amount_sColumn = 13,
					Amount_eColumn = 13,
				};
			}

			if (Type == XSLXType.派件印尼 || Type == XSLXType.派件河內)
			{
				var rowIndex = 7 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 1,
					Label_eColumn = 2,

					Total_sColumn = -1,
					Total_eColumn = -1,

					Pcs_sColumn = -1,
					Pcs_eColumn = -1,

					Weight = Math.Round(totalsData.Weight, 1),
					Weight_sColumn = 8,
					Weight_eColumn = 8,

					PP = totalsData.PP,
					PP_sColumn = 10,
					PP_eColumn = 10,

					CC = totalsData.CC,
					CC_sColumn = 11,
					CC_eColumn = 11,

					Amount_sColumn = -1,
					Amount_eColumn = -1,
				};
			}

			if (Type == XSLXType.清關大陸 || Type == XSLXType.清關越南)
			{
				var rowIndex = 6 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 2,
					Label_eColumn = 2,

					Total = totalsData.Pcs,
					Total_sColumn = 3,
					Total_eColumn = 3,

					Pcs_sColumn = 4,
					Pcs_eColumn = 4,

					Weight = Math.Round(totalsData.Weight, 1),
					Weight_sColumn = 5,
					Weight_eColumn = 5,

					PP_sColumn = -1,
					PP_eColumn = -1,

					CC_sColumn = -1,
					CC_eColumn = -1,

					Amount_sColumn = -1,
					Amount_eColumn = -1,
				};
			}

			if (Type == XSLXType.清關印尼)
			{
				var rowIndex = 7 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 2,
					Label_eColumn = 2,

					Total_sColumn = -1,
					Total_eColumn = -1,

					Pcs = totalsData.Pcs,
					Pcs_sColumn = 3,
					Pcs_eColumn = 3,

					Weight = Math.Round(totalsData.Weight, 1),
					Weight_sColumn = 6,
					Weight_eColumn = 6,

					PP_sColumn = -1,
					PP_eColumn = -1,

					CC_sColumn = -1,
					CC_eColumn = -1,

					Amount_sColumn = -1,
					Amount_eColumn = -1,
				};
			}

			if (Type == XSLXType.清關印尼海關)
			{
				var rowIndex = 7 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 1,
					Label_eColumn = 2,

					Total_sColumn = -1,
					Total_eColumn = -1,

					Pcs = totalsData.Pcs,
					Pcs_sColumn = 5,
					Pcs_eColumn = 5,

					Weight = Math.Round(totalsData.Weight, 1),
					Weight_sColumn = 8,
					Weight_eColumn = 8,

					PP_sColumn = -1,
					PP_eColumn = -1,

					CC_sColumn = -1,
					CC_eColumn = -1,

					Amount_sColumn = -1,
					Amount_eColumn = -1,
				};
			}

			if (Type == XSLXType.印尼海關明細表)
			{
				var rowIndex = 1 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 1,
					Label_eColumn = 2,

					Total_sColumn = -1,
					Total_eColumn = -1,

					Pcs = totalsData.Pcs,
					Pcs_sColumn = 7,
					Pcs_eColumn = 7,

					Weight = Math.Round(totalsData.Weight, 1),
					Weight_sColumn = 11,
					Weight_eColumn = 11,

					PP_sColumn = -1,
					PP_eColumn = -1,

					CC_sColumn = -1,
					CC_eColumn = -1,

					Amount_sColumn = -1,
					Amount_eColumn = -1,
				};
			}

			if (Type == XSLXType.JEMS出貨明細表)
			{
				var rowIndex = 3 + DataCount + 1;
				return new Totals()
				{
					Row = rowIndex,
					Label_sColumn = 1,
					Label_eColumn = 2,

					Total_sColumn = -1,
					Total_eColumn = -1,

					Pcs = totalsData.Pcs,
					Pcs_sColumn = 13,
					Pcs_eColumn = 13,

					Weight = totalsData.Weight,
					Weight_sColumn = 4,
					Weight_eColumn = 4,

					PP_sColumn = -1,
					PP_eColumn = -1,

					CC_sColumn = -1,
					CC_eColumn = -1,

					Amount_sColumn = -1,
					Amount_eColumn = -1,
				};
			}
			else
			{
				return null;
			}
		}

		public static XLWorkbook GetTotals(string Type, TotalsData totalsData, string TableName, string ExportName, int DataCount, ref XLWorkbook workbook)
		{
			var sheet = workbook.Worksheet(TableName ?? ExportName);
			var totals = GetTotalsModel(Type, totalsData, DataCount);

			sheet.Row(totals.Row).Height = 30;

			if (totals.Label_sColumn != -1)
			{
				sheet.Cell(totals.Row, totals.Label_sColumn).Value = $"TOTAL：";
				sheet.Range(totals.Row, totals.Label_sColumn, totals.Row, totals.Label_eColumn).Merge().AddToNamed("Totals");
			}
			if (totals.Total_sColumn != -1)
			{
				sheet.Cell(totals.Row, totals.Total_sColumn).Value = $"共{totals.Total}袋";
				sheet.Range(totals.Row, totals.Total_sColumn, totals.Row, totals.Total_eColumn).Merge().AddToNamed("Totals");
			}
			if (totals.Pcs_sColumn != -1)
			{
				sheet.Cell(totals.Row, totals.Pcs_sColumn).Value = $"{totals.Pcs}";
				sheet.Range(totals.Row, totals.Pcs_sColumn, totals.Row, totals.Pcs_eColumn).Merge().AddToNamed("Totals");
			}
			if (totals.Weight_sColumn != -1)
			{
				sheet.Cell(totals.Row, totals.Weight_sColumn).Value = $"{totals.Weight}";
				sheet.Range(totals.Row, totals.Weight_sColumn, totals.Row, totals.Weight_eColumn).Merge().AddToNamed("Totals");
			}
			if (totals.PP_sColumn != -1)
			{
				sheet.Cell(totals.Row, totals.PP_sColumn).Value = $"{totals.PP}";
				sheet.Range(totals.Row, totals.PP_sColumn, totals.Row, totals.PP_eColumn).Merge().AddToNamed("Totals");
			}
			if (totals.CC_sColumn != -1)
			{
				sheet.Cell(totals.Row, totals.CC_sColumn).Value = $"{totals.CC}";
				sheet.Range(totals.Row, totals.CC_sColumn, totals.Row, totals.CC_eColumn).Merge().AddToNamed("Totals");
			}
			if (totals.Amount_sColumn != -1)
			{
				sheet.Cell(totals.Row, totals.Amount_sColumn).Value = $"{totals.Amount}";
				sheet.Range(totals.Row, totals.Amount_sColumn, totals.Row, totals.Amount_eColumn).Merge().AddToNamed("Totals");
			}

			workbook.NamedRanges.NamedRange("Totals").Ranges.Style = XSLXStyle.Totals(XLWorkbook.DefaultStyle, Type);

			return workbook;
		}
	}

	public class XSLXOthers
	{
		public static XLWorkbook GetOthers(string Type, string TableName, string ExportName, int DataCount, TotalsData totalsData, string MAWBNo, ref XLWorkbook workbook)
		{
			var sheet = workbook.Worksheet(TableName ?? ExportName);

			if (Type == XSLXType.清關印尼海關)
			{
				var rowIndex = 7 + DataCount + 2;
				sheet.Cell(rowIndex, 1).Value = "JKT-P:" + $"{totalsData.pPcs}" + " A:" + $"{totalsData.aPcs}" + " X:" + $"{totalsData.xPcs}" + " T:" + $"{totalsData.tPcs}" + " K:" + $"{totalsData.kPcs}" + "\n" + $"共{totalsData.Pcs}件  {totalsData.Weight}KG" + "\n" + $"MAWB:{MAWBNo}";
				sheet.Row(rowIndex).Height = 139.5;
				sheet.Range(rowIndex, 1, rowIndex, 11).Merge().AddToNamed("Other"); ;
				workbook.NamedRanges.NamedRange("Other").Ranges.Style = XSLXStyle.Other(XLWorkbook.DefaultStyle, Type);
				return workbook;
			}
			else
			{
				return null;
			}
		}
	}
}