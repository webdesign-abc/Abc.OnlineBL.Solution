using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class ProductFilterData
	{
		[DataMember]
		public List<AbcKeyValuePair> Names { get; set; }
		[DataMember]
		public List<AbcKeyValuePair> ContentTypes { get; set; }
		[DataMember]
		public List<AbcKeyValuePair> WorkflowTypes { get; set; }
		[DataMember]
		public List<AbcKeyValuePair> SizeCodes { get; set; }
		[DataMember]
		public List<AbcKeyValuePair> Actives { get; set; }
		[DataMember]
		public List<AbcKeyValuePair> HasPricingFors { get; set; }
		[DataMember]
		public List<AbcKeyValuePair> FrameTypes { get; set; }
	}
}
