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
	public class Field
	{
		private string id = Guid.NewGuid().ToString("N");
		private string fieldName;
		private string caption;
		private string helpText;
		private bool getDefaultFromProductAttribute;
		private bool getDefaultFromItemOrPkgQty;

		private bool enabled;
		private string value;
		private FieldType fieldType;
		private ListItems listItems;
		private Validation validation;

		public Field()
		{
			this.listItems = new ListItems();
		}

		[XmlIgnore]
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		[XmlElement("fieldName")]
		[DataMember]
		public string FieldName
		{
			get { return fieldName; }
			set { fieldName = value; }
		}

		[XmlElement("caption")]
		[DataMember]
		public string Caption
		{
			get { return caption; }
			set { caption = value; }
		}

		[XmlElement("helpText")]
		[DataMember]
		public string HelpText
		{
			get { return helpText; }
			set { helpText = value; }
		}

		[XmlElement("getDefaultFromProductAttribute")]
		[DataMember]
		public bool GetDefaultFromProductAttribute
		{
			get { return getDefaultFromProductAttribute; }
			set { getDefaultFromProductAttribute = value; }
		}

		[XmlElement("getDefaultFromItemOrPkgQty")]
		[DataMember]
		public bool GetDefaultFromItemOrPkgQty
		{
			get { return getDefaultFromItemOrPkgQty; }
			set { getDefaultFromItemOrPkgQty = value; }
		}

		[XmlElement("enabled")]
		[DataMember]
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		[XmlElement("value")]
		[DataMember]
		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		[XmlElement("fieldType")]
		[DataMember]		
		public FieldType FieldType
		{
			get { return fieldType; }
			set { fieldType = value; }
		}

		[XmlElement("listItems")]
		[DataMember]
		public ListItems ListItems
		{
			get { return listItems; }
			set { listItems = value; }
		}

		[XmlElement("validation")]
		[DataMember]
		public Validation Validation
		{
			get { return validation; }
			set { validation = value; }
		}
	}
}
