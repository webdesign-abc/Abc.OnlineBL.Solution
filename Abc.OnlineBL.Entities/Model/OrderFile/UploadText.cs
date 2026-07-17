using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Abc.OnlineBL.Entities.Model.OrderFile
{
	[DataContract]
	[Serializable]
	public class UploadText
	{
		[DataMember]
		public int orderID { get; set; }
		[DataMember]
		public List<ClientContact> Contacts { get; set; }
		[DataMember]
		public string FirstContactID { get; set; }
		[DataMember]
		public string SecondContactID { get; set; }
		[DataMember]
		public string ThirdContactID { get; set; }
		[DataMember]
		public string InspectionDetails { get; set; }
		[DataMember]
		public string TermAndConditions { get; set; }
		[DataMember]
		public string ConjunctionDetails { get; set; }
		[DataMember]
		public string Heading { get; set; }
		[DataMember]
		public string SubHeading { get; set; }
		[DataMember]
		public string BodyCopy { get; set; }
		[DataMember]
		public string BrochureHeading { get; set; }
		[DataMember]
		public string BrochureSubHeading { get; set; }
		[DataMember]
		public string BrochureBodyCopy { get; set; }
		[DataMember]
		public bool IsDisplayIcon { get; set; }
		[DataMember]
		public string Bedrooms { get; set; }
		[DataMember]
		public string Bathrooms { get; set; }
		[DataMember]
		public string CarportsOrGarages { get; set; }
		[DataMember]
		public string Studyrooms { get; set; }
		[DataMember]
		public bool HasPool { get; set; }
		[DataMember]
		public bool UseDifferentTextForBrochureWindowCar { get; set; }

		[DataMember]
		public string Toilet { get; set; }
		[DataMember]
		public int AgentContactId { get; set; }

		[DataMember]
		public string TextSelection { get; set; }
	}
}
