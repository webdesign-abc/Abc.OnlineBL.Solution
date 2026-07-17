using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.WebClient
{
	[Serializable]
	[DataContract]
	public class PSImage
	{
		[DataMember]
		public int OrderNo { get; set; }
		[DataMember]
		public String Path { get; set; }
		[DataMember]
		public String Note { get; set; }
	}
}
