using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Web;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class TextDetails
	{
		private const string TEXTLINE = "____________________________________________________________\r\n\r\n";
		public TextDetails()
        {
        }

		[NonSerialized]
		private ClientContact firstContact;
		[NonSerialized]
		private ClientContact secondContact;
		[NonSerialized]
		private ClientContact thirdContact;
		[NonSerialized]
		private List<ClientContact> contacts;

		[DataMember]
		public int orderID { get; set; }
		[XmlIgnore]
		[DataMember]
		public List<ClientContact> Contacts 
		{
			get { return contacts; }
			set { contacts = value; }
		}

		[DataMember]
		public string FirstContactID { get; set; }
		[XmlIgnore]
		[DataMember]
		public ClientContact FirstContact 
		{
			get { return firstContact; }
			set { firstContact = value; }
		}
		[XmlIgnore]
		[DataMember]
		public ClientContact SecondContact 
		{
			get { return secondContact; }
			set { secondContact = value; }
		}
		[XmlIgnore]
		[DataMember]
		public ClientContact ThirdContact 
		{
			get { return thirdContact; }
			set { thirdContact = value; }
		}
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
		public string Toilet { get; set; }

		[DataMember]
		public string ImageSelection { get; set; }
		[DataMember]
		public string QtyEmailed { get; set; }
		[DataMember]
		public string QtyUploaded { get; set; }

		//Will be decommision soon
		[DataMember]
		public bool HasTextReceived { get; set; }
		[DataMember]
		public string TextSelection { get; set; }

		[DataMember]
		public bool UseDifferentTextForBrochureWindowCar { get; set; }

		public string GetXML(OnlinePropertyOrder parent)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<ImageSupplyDetails>\r\n");

			if (!string.IsNullOrEmpty(this.ImageSelection))
			{
				sb.AppendFormat("<Selection>{0}</Selection>\r\n", ConvertSelection(ImageSelection));

				if (!string.IsNullOrEmpty(QtyEmailed))
					sb.AppendFormat("<QuantityEmailed>{0}</QuantityEmailed>\r\n", QtyEmailed);
				else if (!string.IsNullOrEmpty(QtyUploaded))
					sb.AppendFormat("<QuantityUploaded>{0}</QuantityUploaded>\r\n", QtyUploaded);
			}

			sb.Append("</ImageSupplyDetails>\r\n");


			sb.Append("<TextDetails>\r\n");
			if (!string.IsNullOrEmpty(parent.CommonDetails.SaleType))
			{
				sb.Append("<SaleType>" + parent.CommonDetails.SaleType + "</SaleType>\r\n");
			}

			if(parent.CommonDetails.SaleType == "Auction" && !string.IsNullOrEmpty(parent.CommonDetails.AuctionDetails))
				sb.Append("<AuctionDetails>" + HttpUtility.HtmlEncode(parent.CommonDetails.AuctionDetails) + "</AuctionDetails>\r\n");
			else
				sb.Append("<AuctionDetails></AuctionDetails>\r\n");

			if (this.HasTextReceived || this.TextSelection != "0")
			{
				sb.Append("<InspectionDetails>" + HttpUtility.HtmlEncode(InspectionDetails) + "</InspectionDetails>\r\n");
				sb.Append("<TermsConditions>" + HttpUtility.HtmlEncode(TermAndConditions) + "</TermsConditions>\r\n");

				sb.Append("<Heading>" + HttpUtility.HtmlEncode(Heading) + "</Heading>\r\n");
				if (!string.IsNullOrEmpty(BrochureHeading))
					sb.AppendFormat("<BrochureHeading>{0}</BrochureHeading>\r\n", HttpUtility.HtmlEncode(BrochureHeading));

				sb.Append("<SubHeading>" + HttpUtility.HtmlEncode(SubHeading) + "</SubHeading>\r\n");
				if (!string.IsNullOrEmpty(BrochureSubHeading))
					sb.AppendFormat("<BrochureSubHeading>{0}</BrochureSubHeading>\r\n", HttpUtility.HtmlEncode(BrochureSubHeading));

				sb.Append("<BodyCopy>\r\n<![CDATA[\r\n" + BodyCopy + "\r\n]]></BodyCopy>\r\n");
				if (!string.IsNullOrEmpty(BrochureBodyCopy))
					sb.AppendFormat("<BrochureBodyCopy>\r\n<![CDATA[\r\n{0}\r\n]]></BrochureBodyCopy>\r\n", BrochureBodyCopy.Trim());

				sb.AppendFormat("<AHDetails>\r\n<![CDATA[\r\n{0}\r\n]]></AHDetails>\r\n", TextContactDetails);

				if (FirstContact != null || SecondContact != null || ThirdContact != null)
				{
					sb.Append("<AgentContacts>\r\n");

					if (FirstContact != null)
					{
						sb.Append("<Contact>\r\n");

						sb.AppendFormat("<Id>{0}</Id>\r\n", "1");
						sb.AppendFormat("<ContactId>{0}</ContactId>\r\n", FirstContact.ContactId);
						sb.AppendFormat("<FirstName>{0}</FirstName>\r\n", HttpUtility.HtmlEncode(FirstContact.FirstName));
						if (!string.IsNullOrEmpty(FirstContact.LastName)) sb.AppendFormat("<LastName>{0}</LastName>\r\n", HttpUtility.HtmlEncode(FirstContact.LastName));
						if (!string.IsNullOrEmpty(FirstContact.Phone)) sb.AppendFormat("<Phone>{0}</Phone>\r\n", HttpUtility.HtmlEncode(FirstContact.Phone));
						if (!string.IsNullOrEmpty(FirstContact.Mobile)) sb.AppendFormat("<Mobile>{0}</Mobile>\r\n", HttpUtility.HtmlEncode(FirstContact.Mobile));
						if (!string.IsNullOrEmpty(FirstContact.AhDetails)) sb.AppendFormat("<AhDetails>{0}</AhDetails>\r\n", HttpUtility.HtmlEncode(FirstContact.AhDetails));
						if (!string.IsNullOrEmpty(FirstContact.Email)) sb.AppendFormat("<Email>{0}</Email>\r\n", HttpUtility.HtmlEncode(FirstContact.Email));

						sb.Append("</Contact>\r\n");
					}
					if (SecondContact != null)
					{
						sb.Append("<Contact>\r\n");

						sb.AppendFormat("<Id>{0}</Id>\r\n", "2");
						sb.AppendFormat("<ContactId>{0}</ContactId>\r\n", SecondContact.ContactId);
						sb.AppendFormat("<FirstName>{0}</FirstName>\r\n", HttpUtility.HtmlEncode(SecondContact.FirstName));
						if (!string.IsNullOrEmpty(SecondContact.LastName)) sb.AppendFormat("<LastName>{0}</LastName>\r\n", HttpUtility.HtmlEncode(SecondContact.LastName));
						if (!string.IsNullOrEmpty(SecondContact.Phone)) sb.AppendFormat("<Phone>{0}</Phone>\r\n", HttpUtility.HtmlEncode(SecondContact.Phone));
						if (!string.IsNullOrEmpty(SecondContact.Mobile)) sb.AppendFormat("<Mobile>{0}</Mobile>\r\n", HttpUtility.HtmlEncode(SecondContact.Mobile));
						if (!string.IsNullOrEmpty(SecondContact.AhDetails)) sb.AppendFormat("<AhDetails>{0}</AhDetails>\r\n", HttpUtility.HtmlEncode(SecondContact.AhDetails));
						if (!string.IsNullOrEmpty(SecondContact.Email)) sb.AppendFormat("<Email>{0}</Email>\r\n", HttpUtility.HtmlEncode(SecondContact.Email));

						sb.Append("</Contact>\r\n");
					}
					if (ThirdContact != null)
					{
						sb.Append("<Contact>\r\n");

						sb.AppendFormat("<Id>{0}</Id>\r\n", "3");
						sb.AppendFormat("<ContactId>{0}</ContactId>\r\n", ThirdContact.ContactId);
						sb.AppendFormat("<FirstName>{0}</FirstName>\r\n", HttpUtility.HtmlEncode(ThirdContact.FirstName));
						if (!string.IsNullOrEmpty(ThirdContact.LastName)) sb.AppendFormat("<LastName>{0}</LastName>\r\n", HttpUtility.HtmlEncode(ThirdContact.LastName));
						if (!string.IsNullOrEmpty(ThirdContact.Phone)) sb.AppendFormat("<Phone>{0}</Phone>\r\n", HttpUtility.HtmlEncode(ThirdContact.Phone));
						if (!string.IsNullOrEmpty(ThirdContact.Mobile)) sb.AppendFormat("<Mobile>{0}</Mobile>\r\n", HttpUtility.HtmlEncode(ThirdContact.Mobile));
						if (!string.IsNullOrEmpty(ThirdContact.AhDetails)) sb.AppendFormat("<AhDetails>{0}</AhDetails>\r\n", HttpUtility.HtmlEncode(ThirdContact.AhDetails));
						if (!string.IsNullOrEmpty(ThirdContact.Email)) sb.AppendFormat("<Email>{0}</Email>\r\n", HttpUtility.HtmlEncode(ThirdContact.Email));

						sb.Append("</Contact>\r\n");
					}

					sb.Append("</AgentContacts>\r\n");
				}

				sb.Append("<ConjunctionalDetails>" + HttpUtility.HtmlEncode(ConjunctionDetails) + "</ConjunctionalDetails>\r\n");
				sb.AppendFormat("<Bed>{0}</Bed>\r\n", Bedrooms);
				sb.AppendFormat("<Bath>{0}</Bath>\r\n", Bathrooms);
				sb.AppendFormat("<Car>{0}</Car>\r\n", CarportsOrGarages);
				sb.AppendFormat("<Study>{0}</Study>\r\n", Studyrooms);
				sb.AppendFormat("<Toilet>{0}</Toilet>\r\n", Toilet);
				sb.AppendFormat("<Pool>{0}</Pool>\r\n", (HasPool) ? "1" : "");
				sb.AppendFormat("<DisplayIcons>{0}</DisplayIcons>\r\n", (IsDisplayIcon) ? "Yes" : "No");
			}
			sb.Append("</TextDetails>\r\n");
			return sb.ToString();
		}

		public string GetText(string property, OnlinePropertyOrder parent)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Image You will be Supplying to ABC\r\n");
			sb.Append("----------------------------------\r\n");

			if (!string.IsNullOrEmpty(this.ImageSelection))
			{
				sb.AppendFormat("Selection: {0}\r\n", ConvertSelection(ImageSelection));

				if (!string.IsNullOrEmpty(QtyEmailed))
					sb.AppendFormat("Qty to be emailed: {0}\r\n", QtyEmailed);
				else if (!string.IsNullOrEmpty(QtyUploaded))
					sb.AppendFormat("Qty to be uploaded: {0}\r\n", QtyUploaded);
			}
			sb.Append(TEXTLINE);

			sb.Append("**TEXT**\r\n");
			if (!string.IsNullOrEmpty(parent.CommonDetails.SaleType))
			{
				sb.Append("[Sale Type]" + parent.CommonDetails.SaleType + "\r\n");
			}

			if (parent.CommonDetails.SaleType == "Auction" && !string.IsNullOrEmpty(parent.CommonDetails.AuctionDetails))
				sb.Append("[Auction Details]" + parent.CommonDetails.AuctionDetails + "\r\n");
			else
				sb.Append("[Auction Details]\r\n");
			

			if (property != null && property.Length > 0)
				sb.Append("[Property Address]" + property + "\r\n");

			if (HasTextReceived || this.TextSelection != "0")
			{
				sb.Append("[Inspection Details]" + InspectionDetails + "\r\n");
				sb.Append("[Terms & Conditions]" + TermAndConditions + "\r\n");

				sb.Append("[Heading]" + Heading + "\r\n");
				if (!string.IsNullOrEmpty(BrochureHeading))
					sb.AppendFormat("***BROCHURE HEADING***\r\n{0}\r\n", BrochureHeading);

				sb.Append("[Sub-Heading]" + SubHeading + "\r\n");
				if (!string.IsNullOrEmpty(BrochureSubHeading))
					sb.AppendFormat("***BROCHURE SUB HEADING***\r\n{0}\r\n", BrochureSubHeading);

				sb.Append("[Body Copy]" + BodyCopy + "\r\n");
				if (!string.IsNullOrEmpty(BrochureBodyCopy))
					sb.AppendFormat("***BROCHURE BODY COPY***\r\n{0}\r\n", BrochureBodyCopy);

				sb.Append("[A/H Details]" + TextContactDetails + "\r\n");
				sb.Append("[Conjunctional Details]" + ConjunctionDetails + "\r\n");
				
			}
			else
			{
				sb.Append("Text will be provided at a later time \r\n");
			}
			sb.Append("[$NZTAGS$]\r\n");

			return sb.ToString();

		}

		private string ConvertSelection(string ImageSelection)
		{
			switch (ImageSelection)
			{
				case "1":
					return "I will upload/email my images after I place this order";
				case "2":
					return "I will be uploading my images immediately after this order";
				case "3":
					return "I will upload my images at a later date";
				case "4":
					return "I have already ordered Photography for this property";
				default:
					return "";
			}
		}

		public string TextContactDetails
		{
			get
			{
				if (FirstContact != null || SecondContact != null || ThirdContact != null)
				{
					StringBuilder sb = new StringBuilder();

					if (FirstContact != null)
					{
						sb.AppendFormat("{0}{1}", GetContactDetails(FirstContact), "\r\n");
					}
					if (SecondContact != null)
					{
						sb.AppendFormat("{0}{1}", GetContactDetails(SecondContact), "\r\n");
					}
					if (ThirdContact != null)
					{
						sb.AppendFormat("{0}{1}", GetContactDetails(ThirdContact), "\r\n");
					}

					return sb.ToString();
				}
				else
					return null;
			}
		}

		public string GetContactDetails(ClientContact cc)
		{
			if (!OnlineBLConfig.IS_NZ)
				return string.Format("{0} {1} {2}", cc.FirstName, cc.LastName, (!string.IsNullOrEmpty(cc.AhDetails)) ? cc.AhDetails :
																  (!string.IsNullOrEmpty(cc.Mobile)) ? cc.Mobile : cc.Phone);
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("{0} {1}", cc.FirstName, cc.LastName);

				if (!string.IsNullOrEmpty(cc.Mobile))
				{
					if (string.IsNullOrEmpty(cc.AhDetails) && string.IsNullOrEmpty(cc.Phone))
						sb.AppendFormat(" {0}", cc.Mobile);
					else
						sb.AppendFormat(" Mob: {0}", cc.Mobile);
				}

				if (!string.IsNullOrEmpty(cc.AhDetails))
				{
					if (string.IsNullOrEmpty(cc.Mobile) && string.IsNullOrEmpty(cc.Phone))
						sb.AppendFormat(" {0}", cc.AhDetails);
					else
						sb.AppendFormat(" A/h: {0}", cc.AhDetails);
				}

				if (!string.IsNullOrEmpty(cc.Phone))
				{
					if (string.IsNullOrEmpty(cc.Mobile) && string.IsNullOrEmpty(cc.AhDetails))
						sb.AppendFormat(" {0}", cc.Phone);
					else
						sb.AppendFormat(" Ph: {0}", cc.Phone);
				}

				return sb.ToString();
			}
		}

	}
}
