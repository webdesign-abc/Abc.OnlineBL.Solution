using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Abc.OnlineBL.VirtualFileSystem
{
	public class LocalFileProvider : IFile
	{
		private static RemoteProxyWS.FileService fileSvcProxy;

		private static RemoteProxyWS.FileService RemoteSvc
		{
			get
			{
				if (fileSvcProxy == null)
				{
					fileSvcProxy = new Abc.OnlineBL.VirtualFileSystem.RemoteProxyWS.FileService();
					fileSvcProxy.Url = OnlineBL.Utility.Configuration.BaseConfig.VFS_ROOT_URL;
				}
				return fileSvcProxy;
			}
		}

		#region IFile Members

		public bool Exists(string filePath)
		{
			return File.Exists(filePath);
		}

		public void Copy(string srcPath, string destPath, bool overwrite)
		{
			File.Copy(srcPath, destPath, overwrite);
		}
	
		public void Delete(string filePath)
		{
			File.Delete(filePath);			
		}

		public string ReadAllText(string filePath)
		{
			return File.ReadAllText(filePath);
		}

		public byte[] ReadAllBytes(string filePath)
		{
			return File.ReadAllBytes(filePath);
		}

		public void WriteAllText(string filePath, string data)
		{
			File.WriteAllText(filePath, data);
		}

		public void WriteAllBytes(string filePath, byte[] data)
		{
			File.WriteAllBytes(filePath, data);
		}

		#endregion

		#region IFile Members


		public bool ExistsDir(string dirPath)
		{
			return Directory.Exists(dirPath);
		}

		public void CreateDir(string dirPath)
		{
			Directory.CreateDirectory(dirPath);
		}

		public void DeleteDir(string dirPath, bool recursive)
		{
			Directory.Delete(dirPath, recursive);
		}

		public string[] GetFiles(string path, string filter)
		{
			return Directory.GetFiles(path, filter);
		}
		#endregion
	}
}
