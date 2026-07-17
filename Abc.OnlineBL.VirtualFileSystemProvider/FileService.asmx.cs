using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace Abc.OnlineBL.VirtualFileSystemProvider
{
	/// <summary>
	/// Summary description for FileService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class FileService : System.Web.Services.WebService
	{
		#region IFile Members
		[WebMethod]
		public bool Exists(string filePath)
		{
			return File.Exists(filePath);
		}
		[WebMethod]
		public void Copy(string srcPath, string destPath, bool overwrite)
		{
			File.Copy(srcPath, destPath, overwrite);
		}
		[WebMethod]
		public void Delete(string filePath)
		{
			File.Delete(filePath);
		}
		[WebMethod]
		public string ReadAllText(string filePath)
		{
			return File.ReadAllText(filePath);
		}
		[WebMethod]
		public byte[] ReadAllBytes(string filePath)
		{
			return File.ReadAllBytes(filePath);
		}
		[WebMethod]
		public void WriteAllText(string filePath, string data)
		{
			File.WriteAllText(filePath, data);
		}
		[WebMethod]
		public void WriteAllBytes(string filePath, byte[] data)
		{
			File.WriteAllBytes(filePath, data);
		}

		[WebMethod]
		public bool ExistsDir(string dirPath)
		{
			return Directory.Exists(dirPath);
		}
		[WebMethod]
		public void CreateDir(string dirPath)
		{
			Directory.CreateDirectory(dirPath);
		}
		[WebMethod]
		public void DeleteDir(string dirPath, bool recursive)
		{
			Directory.Delete(dirPath, recursive);
		}

		[WebMethod]
		public string[] GetFiles(string path, string filter)
		{
			return Directory.GetFiles(path, filter);
		}
		#endregion
	}
}
