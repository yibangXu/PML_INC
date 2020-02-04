using ClosedXML.Excel;
using HY_PML.Models.XSLXHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace HY_PML.helper
{
    public class XSLXTitles
    {
        public static Titles GetTitles(string Type)
        {
            if (Type == XSLXType.派件大陸)
            {
				return new Titles()
				{
					TableName = "派件明細表",
					Title = "遠達(國際空運快遞)有限公司",
					Title_sRow = 1,
					Title_sColumn = 1,
					Title_eRow = 1,
					Title_eColumn = 15,
					Header_sIndex = 6,
					Data_sIndex = 7,
					Data_Height = 60,
                };
            }

			if (Type == XSLXType.派件越南)
			{
				return new Titles()
				{
					TableName = "派件明細表",
					Title = "遠達(國際空運快遞)有限公司",
					Title_sRow = 1,
					Title_sColumn = 1,
					Title_eRow = 1,
					Title_eColumn = 15,
					Header_sIndex = 6,
					Data_sIndex = 7,
					Data_Height = 120,
				};
			}

			if (Type == XSLXType.派件河內 || Type == XSLXType.派件印尼)
            {
                return new Titles()
                {
                    TableName = "派件明細表",
                    Title = "EXPRESS  MANIFEST",
                    Title_sRow = 1,
                    Title_sColumn = 1,
                    Title_eRow = 1,
                    Title_eColumn = 12,
                    Header_sIndex = 7,
                    Data_sIndex = 8,
					Data_Height = 120,
				};
            }

            if (Type == XSLXType.清關大陸 || Type == XSLXType.清關越南)
            {
                return new Titles()
                {
                    TableName = "清關明細表",
                    Title = "遠達(國際空運快遞)有限公司",
                    Title_sRow = 1,
                    Title_sColumn = 1,
                    Title_eRow = 1,
                    Title_eColumn = 7,
                    Header_sIndex = 6,
                    Data_sIndex = 7,
					Data_Height = 35.25,
				};
            }

            if (Type == XSLXType.清關印尼)
            {
                return new Titles()
                {
                    TableName = "清關明細表",
                    Title = "EXPRESS  MANIFEST",
                    Title_sRow = 1,
                    Title_sColumn = 1,
                    Title_eRow = 1,
                    Title_eColumn = 7,
                    Header_sIndex = 7,
                    Data_sIndex = 8,
					Data_Height = 120,
				};
            }

            if (Type == XSLXType.清關印尼海關)
            {
                return new Titles()
                {
                    TableName = "清關明細表",
                    Title = "MANIFEST CLEARANCE",
                    Title_sRow = 1,
                    Title_sColumn = 1,
                    Title_eRow = 1,
                    Title_eColumn = 11,
                    Header_sIndex = 7,
                    Data_sIndex = 8,
					Data_Height = 120,
				};
            }

            if (Type == XSLXType.印尼海關明細表)
            {
                return new Titles()
                {
                    TableName = "其他",
                    Header_sIndex = 1,
                    Data_sIndex = 2,
					Data_Height = 90,
				};
            }

            if (Type == XSLXType.JEMS出貨明細表)
            {
                return new Titles()
                {
                    TableName = "其他",
                    Title = " E M S  國 際 快 捷 郵 件 出 貨 明 細 表",
                    Title_sRow = 1,
                    Title_sColumn = 1,
                    Title_eRow = 1,
                    Title_eColumn = 14,
                    Header_sIndex = 3,
                    Data_sIndex = 4,
					Data_Height = 30,
				};
            }

            else
            {
                return null;
            }
        }
    }
}