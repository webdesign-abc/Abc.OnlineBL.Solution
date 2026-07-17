using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.WebClient
{
	[Serializable]
	[DataContract]
	public class PickingSlipModel
	{
		[DataMember]
		public int ProductID { get; set; }
		[DataMember]
		public string ProductName { get; set; }
		[DataMember]
		public List<PSImage> Images { get; set; }
	}
}
