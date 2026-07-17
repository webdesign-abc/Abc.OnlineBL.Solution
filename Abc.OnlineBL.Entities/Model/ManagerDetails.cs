using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class ManagerDetails
	{
		[DataMember]
		public string ManagerID { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public bool IsActive { get; set; }
	}
}
