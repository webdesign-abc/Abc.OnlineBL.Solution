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
	public class ListItem
	{
		private string displayText;
		private string valueText;

		[XmlAttribute("displayText")]
		[DataMember]
		public string DisplayText
		{
			get { return displayText; }
			set { displayText = value; }
		}

		[XmlAttribute("valueText")]
		[DataMember]
		public string ValueText
		{
			get { return valueText; }
			set { valueText = value; }
		}
	}
}
