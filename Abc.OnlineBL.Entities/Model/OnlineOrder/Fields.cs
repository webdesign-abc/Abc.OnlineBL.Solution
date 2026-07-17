using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]	
	public class Fields
	{
		private List<Field> field;

		[XmlElement("field")]
		[DataMember]
		public List<Field> Field
		{
			get { return field; }
			set { field = value; }
		}		

		public Fields()
		{
			this.field = new List<Field>();
		}
	}
}
