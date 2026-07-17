using Abc.OnlineBL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	public class UploadPhotoRequest
	{
		[DataMember]
		public string UncFilePath { get; set; }
		[DataMember]
		public string FileName { get; set; }
		[DataMember]
		public UploadedFileType FileSelectMode { get; set; }
		[DataMember]
		public string AgentContactName { get; set; }
		[DataMember]
		public string Notes { get; set; }
	}
}
