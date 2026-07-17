using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Utility.Configuration;

namespace Abc.OnlineBL.VirtualFileSystem
{
	public class VirtualFileSystemFactory
	{
		private static LocalFileProvider localFileProvider;
		private static RemoteFileProvider remoteFileProvider;

		private static LocalFileProvider LocalProvider
		{
			get
			{
				if (localFileProvider == null)
				{
					localFileProvider = new LocalFileProvider();					
				}
				return localFileProvider;
			}
		}

		private static RemoteFileProvider RemoteProvider
		{
			get
			{
				if (remoteFileProvider == null)
				{
					remoteFileProvider = new RemoteFileProvider();					
				}
				return remoteFileProvider;
			}
		}

		public static IFile GetFile()
		{
			if (BaseConfig.IS_NZ)
			{
				return RemoteProvider;
			}
			else
			{
				return LocalProvider;
			}
		}
	}
}
