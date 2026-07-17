using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class RangeValidation
	{
		private int min;
		private int max;

		[XmlAttribute("min")]
		[DataMember]
		public int Min
		{
			get { return min; }
			set { min = value; }
		}

		[XmlAttribute("max")]
		[DataMember]
		public int Max
		{
			get { return max; }
			set { max = value; }
		}
	}
}
