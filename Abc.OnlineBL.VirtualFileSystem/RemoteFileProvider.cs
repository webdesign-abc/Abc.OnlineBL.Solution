using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Utility.Configuration;

namespace Abc.OnlineBL.VirtualFileSystem
{
	public class RemoteFileProvider : RemoteProxyWS.FileService, IFile
	{
		public RemoteFileProvider()
		{
			this.Url = BaseConfig.VFS_ROOT_URL;
		}
	}
}
