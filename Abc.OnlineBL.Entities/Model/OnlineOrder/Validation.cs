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
	public class Validation
	{
		private RangeValidation rangeValidation;
		private RegexValidation regExValidation;
		private bool required;

		[XmlAttribute("required")]
		[DataMember]
		public bool Required
		{
			get { return required; }
			set { required = value; }
		}

		[XmlElement("rangeValidation")]
		[DataMember]
		public RangeValidation RangeValidation
		{
			get { return rangeValidation; }
			set { rangeValidation = value; }
		}

		[XmlElement("regExValidation")]
		[DataMember]
		public RegexValidation RegExValidation
		{
			get { return regExValidation; }
			set { regExValidation = value; }
		}


	}
}
