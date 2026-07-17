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
	public class ListItems
	{
		[DataMember]
		[XmlElement("listItem")]
		public List<ListItem> ListItem { get; set; }

		public ListItems()
		{
			this.ListItem = new List<ListItem>();
		}
	}
}
