using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[Serializable]
	public class AbcKeyValuePair
	{
		private string _key;
		private string _value;

		[DataMember]
		public string Key 
		{
			get { return _key; }
			set { _key = value; }
		}

		[DataMember]
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}
	}
}
