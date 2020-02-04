using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HY_PML.helper
{
	static public class FtpHelper
	{
		/// <summary>
		/// 從ftp伺服器上獲得資料夾列表
		/// </summary>
		/// <param name="RequedstPath">伺服器下的相對路徑</param>
		/// <returns></returns>//基本設定
		static private string path = "ftp://waws-prod-dm1-147.ftp.azurewebsites.windows.net//";    //目標路徑
		static private string ftpip = "ftp://waws-prod-dm1-147.ftp.azurewebsites.windows.net";    //ftp IP地址
		static private string username = "yb\\$yb";  //ftp使用者名稱
		static private string password = "P6EPa7PF4sftkPuayacDwj0C4BjY430Afn2ZlA2rWQCrCbJMEMfA2CtB4Cdk";   //ftp密碼

		//獲取ftp上面的檔案和資料夾
		public static string[] GetFileList(string dir)
		{
			string[] downloadFiles;
			StringBuilder result = new StringBuilder();
			FtpWebRequest request;
			try
			{
				request = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
				request.UseBinary = true;
				request.Credentials = new NetworkCredential(username, password);//設定使用者名稱和密碼
				request.Method = WebRequestMethods.Ftp.ListDirectory;
				request.UseBinary = true;

				WebResponse response = request.GetResponse();
				StreamReader reader = new StreamReader(response.GetResponseStream());

				string line = reader.ReadLine();
				while (line != null)
				{
					result.Append(line);
					result.Append("\n");
					Console.WriteLine(line);
					line = reader.ReadLine();
				}
				// to remove the trailing '\n'
				result.Remove(result.ToString().LastIndexOf('\n'), 1);
				reader.Close();
				response.Close();
				return result.ToString().Split('\n');
			}
			catch (Exception ex)
			{
				Console.WriteLine("獲取ftp上面的檔案和資料夾：" + ex.Message);
				downloadFiles = null;
				return downloadFiles;
			}
		}

		/// <summary>
		/// 獲取檔案大小
		/// </summary>
		/// <param name="file">ip伺服器下的相對路徑</param>
		/// <returns>檔案大小</returns>
		public static int GetFileSize(string file)
		{
			StringBuilder result = new StringBuilder();
			FtpWebRequest request;
			try
			{
				request = (FtpWebRequest)FtpWebRequest.Create(new Uri(path + file));
				request.UseBinary = true;
				request.Credentials = new NetworkCredential(username, password);//設定使用者名稱和密碼
				request.Method = WebRequestMethods.Ftp.GetFileSize;

				int dataLength = (int)request.GetResponse().ContentLength;

				return dataLength;
			}
			catch (Exception ex)
			{
				Console.WriteLine("獲取檔案大小出錯：" + ex.Message);
				return -1;
			}
		}

		/// <summary>
		/// 檔案上傳
		/// </summary>
		/// <param name="filePath">原路徑（絕對路徑）包括檔名</param>
		/// <param name="objPath">目標資料夾：伺服器下的相對路徑 不填為根目錄</param>
		public static void FileUpLoad(string filePath, string objPath = "")
		{
			try
			{
				string url = path;
				if (objPath != "")
					url += objPath + "/";
				try
				{

					FtpWebRequest reqFTP = null;
					//待上傳的檔案 （全路徑）
					try
					{
						FileInfo fileInfo = new FileInfo(filePath);
						using (FileStream fs = fileInfo.OpenRead())
						{
							long length = fs.Length;
							reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + fileInfo.Name));

							//設定連線到FTP的帳號密碼
							reqFTP.Credentials = new NetworkCredential(username, password);
							//設定請求完成後是否保持連線
							reqFTP.KeepAlive = false;
							//指定執行命令
							reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
							//指定資料傳輸型別
							reqFTP.UseBinary = true;

							using (Stream stream = reqFTP.GetRequestStream())
							{
								//設定緩衝大小
								int BufferLength = 5120;
								byte[] b = new byte[BufferLength];
								int i;
								while ((i = fs.Read(b, 0, BufferLength)) > 0)
								{
									stream.Write(b, 0, i);
								}
								Console.WriteLine("上傳檔案成功");
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("上傳檔案失敗錯誤為" + ex.Message);
					}
					finally
					{

					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("上傳檔案失敗錯誤為" + ex.Message);
				}
				finally
				{

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("上傳檔案失敗錯誤為" + ex.Message);
			}
		}

		/// <summary>
		/// 刪除檔案
		/// </summary>
		/// <param name="fileName">伺服器下的相對路徑 包括檔名</param>
		public static void DeleteFileName(string fileName)
		{
			try
			{
				//FileInfo fileInf = new FileInfo(ftpip + "" + fileName);
				string uri = path +"LadingNo123\\"+ fileName;
				FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
				// 指定資料傳輸型別
				reqFTP.UseBinary = true;
				// ftp使用者名稱和密碼
				reqFTP.Credentials = new NetworkCredential(username, password);
				// 預設為true，連線不會被關閉
				// 在一個命令之後被執行
				reqFTP.KeepAlive = false;
				// 指定執行什麼命令
				reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
				FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
				response.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("刪除檔案出錯：" + ex.Message);
			}
		}

		/// <summary>
		/// 新建目錄 上一級必須先存在
		/// </summary>
		/// <param name="dirName">伺服器下的相對路徑</param>
		public static void MakeDir(string dirName)
		{
			try
			{
				string uri = path + dirName;
				FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
				// 指定資料傳輸型別
				reqFTP.UseBinary = true;
				// ftp使用者名稱和密碼
				reqFTP.Credentials = new NetworkCredential(username, password);
				reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
				FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
				response.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("建立目錄出錯：" + ex.Message);
			}
		}

		/// <summary>
		/// 刪除目錄 上一級必須先存在
		/// </summary>
		/// <param name="dirName">伺服器下的相對路徑</param>
		public static void DelDir(string dirName)
		{
			try
			{
				string uri = path + dirName;
				FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
				// ftp使用者名稱和密碼
				reqFTP.Credentials = new NetworkCredential(username, password);
				reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
				FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
				response.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("刪除目錄出錯：" + ex.Message);
			}
		}

		/// <summary>
		/// 從ftp伺服器上獲得資料夾列表
		/// </summary>
		/// <param name="RequedstPath">伺服器下的相對路徑</param>
		/// <returns></returns>
		public static List<string> GetDirctory(string RequedstPath)
		{
			List<string> strs = new List<string>();
			try
			{
				string uri = path + RequedstPath;   //目標路徑 path為伺服器地址
				FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
				// ftp使用者名稱和密碼
				reqFTP.Credentials = new NetworkCredential(username, password);
				reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
				WebResponse response = reqFTP.GetResponse();
				StreamReader reader = new StreamReader(response.GetResponseStream());//中文檔名

				string line = reader.ReadLine();
				while (line != null)
				{
					if (line.Contains("<DIR>"))
					{
						string msg = line.Substring(line.LastIndexOf("<DIR>") + 5).Trim();
						strs.Add(msg);
					}
					line = reader.ReadLine();
				}
				reader.Close();
				response.Close();
				return strs;
			}
			catch (Exception ex)
			{
				Console.WriteLine("獲取目錄出錯：" + ex.Message);
			}
			return strs;
		}

		/// <summary>
		/// 從ftp伺服器上獲得檔案列表
		/// </summary>
		/// <param name="RequedstPath">伺服器下的相對路徑</param>
		/// <returns></returns>
		public static List<string> GetFile(string RequedstPath)
		{
			List<string> strs = new List<string>();
			try
			{
				string uri = path + RequedstPath;   //目標路徑 path為伺服器地址
				FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
				// ftp使用者名稱和密碼
				reqFTP.Credentials = new NetworkCredential(username, password);
				reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
				WebResponse response = reqFTP.GetResponse();
				StreamReader reader = new StreamReader(response.GetResponseStream());//中文檔名

				string line = reader.ReadLine();
				while (line != null)
				{
					if (!line.Contains("<DIR>"))
					{
						string msg = line.Substring(39).Trim();
						strs.Add(msg);
					}
					line = reader.ReadLine();
				}
				reader.Close();
				response.Close();
				return strs;
			}
			catch (Exception ex)
			{
				Console.WriteLine("獲取檔案出錯：" + ex.Message);
			}
			return strs;
		}
	}
}
