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
	public class RegexValidation
	{
		private string regexString;
		private string validationMessage;

		[XmlElement("validationMessage")]
		[DataMember]
		public string ValidationMessage
		{
			get { return validationMessage; }
			set { validationMessage = value; }
		}

		[XmlElement("regex")]
		[DataMember]
		public string RegexString
		{
			get { return regexString; }
			set { regexString = value; }
		}
	}
}
