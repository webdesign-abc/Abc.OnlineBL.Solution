using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Abc.OnlineBL.VirtualFileSystem
{
	[ServiceContract]
	public interface IFile
	{
		[OperationContract]
		bool Exists(string filePath);
		[OperationContract]
		void Copy(string srcPath, string destPath, bool overwrite);
		[OperationContract]
		void Delete(string filePath);
		[OperationContract]
		string ReadAllText(string filePath);
		[OperationContract]
		byte[] ReadAllBytes(string filePath);
		[OperationContract]
		void WriteAllText(string filePath, string data);
		[OperationContract]
		void WriteAllBytes(string filePath, byte[] data);
		[OperationContract]
		bool ExistsDir(string dirPath);
		[OperationContract]
		void CreateDir(string dirPath);
		[OperationContract]
		void DeleteDir(string dirPath, bool recursive);
		[OperationContract]
		string[] GetFiles(string path, string filter);
	}
}
