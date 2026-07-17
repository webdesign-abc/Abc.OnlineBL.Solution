using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public enum FieldType
	{
		[EnumMember]
		TextBox = 0,
		[EnumMember]
		CheckBox = 1,
		[EnumMember]
		ComboBox = 2,
		[EnumMember]
		RadioButton = 3,
		[EnumMember]
		TextArea = 4,
        [EnumMember]
        Default = 5,
	}
}
