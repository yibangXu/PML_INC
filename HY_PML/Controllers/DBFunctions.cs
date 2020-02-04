using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HY_PML.Controllers
{
	public class DBFunctions
	{
		/// <summary>
		///轉入會計系統
		/// </summary>
		public string FinanceInsert(List<SqlParameter> data, string sqlstr)
		{
			var insertResult = "";
			SqlConnection dataConnection = new SqlConnection();
			String sqlConnectionStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ACB2010"].ConnectionString;

			SqlCommand SqlCmd = new SqlCommand(sqlstr, dataConnection);
			try
			{
				dataConnection.ConnectionString = sqlConnectionStr;

				dataConnection.Open();

				SqlCmd.Parameters.AddRange(data.ToArray<SqlParameter>());

				SqlCmd.ExecuteNonQuery();

				dataConnection.Close();

				insertResult = "OK";

			}
			catch (Exception e)
			{
				insertResult = e.Message;
			}
			finally
			{
				SqlCmd.Cancel();
				dataConnection.Close();
				dataConnection.Dispose();
			}
			return insertResult;
		}
	}
}