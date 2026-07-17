using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abc.OnlineBL.Entities.Utility;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class OnlinePropertyOrder :BaseOrder
	{
		private int propertyId;
		private PreferredDateType preferredBoardErectionType;
		private PreferredDateType preferredBoardRemovalType;
		private DateTime? preferredBoardErectionDate;
		private DateTime? preferredBoardRemovalDate;
		private bool isPreferredBoardErectionDateSelected;
		private bool isPreferredBoardRemovalDateSelected;
		private string erectionNotes;
		private string removalNotes;
		private BoardInstallationType boardInstallationType;
		private bool isDIYOrder;
		private SmsOrder smsOrder;
		private bool includeSMSOnDemand;
		private ListingOrder listingOrder;
		private bool isListingonAbcRe;
		private OrderCommonDetailsModel commonDetails;
		private TextDetails textDetails;
		[NonSerialized]
		private Property property;
		private bool isB2BOrder;
        private int businessID;
        private int b2bListingID;
		private string installationFile;
		private string originalInsFileName;
		private bool isSiteInspectionRequired;
		private string siteInspectionNotes;
		private OrderType orderType;
		private string officeDeliveryAddress;
		private MarketingDeliveryType marketingDeliveryType;
		private MarketingDeliveryAddress marketingDeliveryAddress;
		private int agentContactId;
        private DeliveryType deliveryPref;
        private DeliveryDetail deliveryDetail;

		private const string line = "<HR>";
		private const string TEXTLINE = "____________________________________________________________\r\n\r\n";


		public OnlinePropertyOrder()
			: base()
		{
			commonDetails = new OrderCommonDetailsModel();
			textDetails = new TextDetails();
			smsOrder = new SmsOrder();
			listingOrder = new ListingOrder();
            marketingDeliveryAddress = new MarketingDeliveryAddress();
            deliveryDetail = new DeliveryDetail();
            //SitePlanOverlayForPhotography = new SitePlanOverlayForPhotography();
		}
		
		/// <summary>
		/// No need to have this in XML or Data Contract as it will create a circular loop thus breaking Xml Serilize
		/// </summary>
		[XmlIgnore]
		[DataMember]
		public Property Property
		{
			get { return property; }
			set { property = value; }
		}

		[DataMember]
		public OrderCommonDetailsModel CommonDetails
		{
			get { return commonDetails; }
			set { commonDetails = value; }
		}

		[DataMember]
		public TextDetails TextDetails
		{
			get { return textDetails; }
			set { textDetails = value; }
		}

		[DataMember]
		public bool IncludeSMSOnDemand
		{
			get { return includeSMSOnDemand; }
			set { includeSMSOnDemand = value; }
		}


		[DataMember]
		public SmsOrder SmsOrder
		{
			get { return smsOrder; }
			set { smsOrder = value; }
		}

		[DataMember]
		public bool IsListingonAbcRe
		{
			get { return isListingonAbcRe; }
			set { isListingonAbcRe = value; }
		}

		[DataMember]
		public ListingOrder ListingOrder
		{
			get { return listingOrder; }
			set { listingOrder = value; }
		}

		[DataMember]
		public bool IsDIYOrder
		{
			get { return isDIYOrder; }
			set { isDIYOrder = value; }
		}

		[DataMember]
		public BoardInstallationType BoardInstallationType
		{
			get { return boardInstallationType; }
			set { boardInstallationType = value; }
		}

		[DataMember]
		public string RemovalNotes
		{
			get { return removalNotes; }
			set { removalNotes = value; }
		}

		[DataMember]
		public string ErectionNotes
		{
			get { return erectionNotes; }
			set { erectionNotes = value; }
		}

		[DataMember]
		public bool IsPreferredBoardErectionDateSelected
		{
			get { return isPreferredBoardErectionDateSelected; }
			set { isPreferredBoardErectionDateSelected = value; }
		}

		[DataMember]
		public bool IsPreferredBoardRemovalDateSelected
		{
			get { return isPreferredBoardRemovalDateSelected; }
			set { isPreferredBoardRemovalDateSelected = value; }
		}

		[DataMember]
		[XmlElement(IsNullable = true)]
		[DataType(DataType.Date)]
		public DateTime? PreferredBoardRemovalDate
		{
			get { return preferredBoardRemovalDate; }
			set { preferredBoardRemovalDate = value; }
		}

		[DataMember]
		[XmlElement(IsNullable = true)]
		[DataType(DataType.Date)]
		public DateTime? PreferredBoardErectionDate
		{
			get { return preferredBoardErectionDate; }
			set { preferredBoardErectionDate = value; }
		}

		[DataMember]
		public PreferredDateType PreferredBoardRemovalType
		{
			get { return preferredBoardRemovalType; }
			set { preferredBoardRemovalType = value; }
		}

		[DataMember]
		public PreferredDateType PreferredBoardErectionType
		{
			get { return preferredBoardErectionType; }
			set { preferredBoardErectionType = value; }
		}

		[DataMember]
		public int PropertyId
		{
			get { return propertyId; }
			set { propertyId = value; }
		}

		[DataMember]
		public bool IsB2BOrder
		{
			get { return isB2BOrder; }
			set { isB2BOrder = value; }
		}

        [DataMember]
        public int BusinessID
        {
            get { return businessID; }
            set { businessID = value; }
        }

        [DataMember]
        public int B2bListingID
        {
            get { return b2bListingID; }
            set { b2bListingID = value; }
        }

		[DataMember]
		public string InstallationFile
		{
			get { return installationFile; }
			set { installationFile = value; }
		}

		[DataMember]
		public string OriginalInsFileName
		{
			get { return originalInsFileName; }
			set { originalInsFileName = value; }
		}

		[DataMember]
		public bool IsSiteInspectionRequired
		{
			get { return isSiteInspectionRequired; }
			set { isSiteInspectionRequired = value; }
		}

		[DataMember]
		public string SiteInspectionNotes
		{
			get { return siteInspectionNotes; }
			set { siteInspectionNotes = value; }
		}

		[DataMember]
		public OrderType OrderType
		{
			get { return orderType; }
			set { orderType = value; }
		}

		[DataMember]
		public string OfficeDeliveryAddress
		{
			get { return officeDeliveryAddress; }
			set { officeDeliveryAddress = value; }
		}

		[DataMember]
		public MarketingDeliveryType MarketingDeliveryType
		{
			get { return marketingDeliveryType; }
			set { marketingDeliveryType = value; }
		}

		[DataMember]
		public MarketingDeliveryAddress MarketingDeliveryAddress
		{
			get { return marketingDeliveryAddress; }
			set { marketingDeliveryAddress = value; }
		}

		[DataMember]
		public int AgentContactId
		{
			get { return agentContactId; }
			set { agentContactId = value; }
		}

        [DataMember]
        public bool? IsExpressOrder { get; set; }

        [DataMember]
        public bool IsArtworkUpload { get; set; }

        [DataMember]
        public bool ArtworkUploadFile { get; set; }

        [DataMember]
        public DeliveryType DeliveryPref
        {
            get { return deliveryPref; }
            set { deliveryPref = value; }
        }

        [DataMember]
        public DeliveryDetail DeliveryInfo
        {
            get { return deliveryDetail; }
            set { deliveryDetail = value; }
        }

        [DataMember]
        public string LandOverlayDescription { get; set; }

        [DataMember]
        public string OriginalPhotographyFileName { get; set; }

        [DataMember]
        public string PhotographyFile { get; set; }

        [DataMember]
        public string OriginalSitePlanInstructionFileName { get; set; }

        [DataMember]
        public string SitePlanInstructionFile { get; set; }

        [DataMember]
        public bool ClientHasStockboardDIYPreference { get; set; }

        [DataMember]
        public bool ApplySurcharge { get; set; }

        [DataMember]
        public bool HasInstallFile { get; set; }

        [DataMember]
        public string OrderDescription { get; set; }

        #region GetHTMLString
        public string GetHTMLString()
		{
			StringBuilder sb = new StringBuilder();
            if (!IsArtworkUpload)
            {
                if (isDIYOrder)
                {
                    sb.Append("<p>Design Option: User Designed</p>");
                }
                else if (OrderType == OrderType.AbcDesign)
                {
                    sb.Append("<p>Design Option: ABC Design</p>");
                }
                else if (OrderType == OrderType.B2B)
                {
                    sb.Append("<p>Design Option: This is B2B Order</p>");
                }
                else
                {
                    sb.Append("<p>Design Option: Client Will Upload Artwork</p>");
                }
            }
            else
            {
                sb.Append("<p>Design Option: Client Will Upload Artwork</p>");
            }

			if (!this.IsOnlyListingProduct())
			{
				sb.Append("<P><B>Your Contact Details</B></P>");
				sb.Append("Contact Name   : " + ContactDetailName + "<BR>");
				sb.Append("Contact Number : " + ContactNumber + "<BR>");
				sb.Append("Send Proof By  : " + SendProofBy + "<BR>");
				sb.Append("Send Proof To  : " + SendProofTo + "<BR>");
				if (ClientRefNumber != null && ClientRefNumber.Length > 0)
				{
					sb.Append("Clients Reference Id  : " + ClientRefNumber + "<BR>");
				}
                if (DeliveryInfo != null)
                {
                    if (!string.IsNullOrEmpty(DeliveryInfo.Name))
                    {
                        sb.Append("Delivery Name  : " + DeliveryInfo.Name + "<BR>");
                    }
                    if (!string.IsNullOrEmpty(DeliveryInfo.ContactEmail))
                    {
                        sb.Append("Delivery Email : " + DeliveryInfo.ContactEmail + "<BR>");
                    }
                    sb.Append("Deliver To     : " + DeliveryPref.ToString() + " Address<BR>");
                    sb.Append("                 " + DeliveryInfo.StreetAddress + "<BR>");
                    sb.Append("                 " + DeliveryInfo.Suburb + " - " + DeliveryInfo.PostCode + ", " + DeliveryInfo.State + "<BR>");
                    
                }
				sb.Append("<br />");
				sb.Append(line);
			}
			// Show Products Details
			sb.Append("<P><B>Products Ordered</B></P>");
			foreach (CartItem anItem in Cart)
			{
				sb.Append(anItem.GetHTMLString());
				sb.Append("<BR>");
			}

			sb.Append(line);

			if (Property != null)
			{
				sb.Append(Property.GetHTMLString());
				sb.Append("<br />");
				sb.Append(line);
			}

			if (ShouldBindSaleInfo())
			{
				sb.Append(GetSaleInfoHTMLString());
				sb.Append("<br />");
				sb.Append(line);
			}

			// Show the Notes
			if (!IsOnlyListingProduct())
			{
				sb.Append("<P><B>Notes</B></P>");
				sb.Append(Notes + "<BR>");
				sb.Append(line);
			}

			//Show the ABCrealestate.com.au Link if it exists
			if (this.commonDetails.IsInsertAbcRealEstateLink)
			{
				sb.Append("<B>USE ABCrealestate.com.au Link</B><BR>");
				sb.Append(line);
			}

			// Show the Erection Notes if there is any
			if (this.erectionNotes != "" && (!OrderOnlyHasNonBoardItems()))
			{
				sb.Append("<P><B>Erection Notes</B></P>");
				sb.Append(erectionNotes + "<BR>");
			}

			if (this.removalNotes != "" && (!OrderOnlyHasNonBoardItems()))
			{
				sb.Append("<P><B>Removal Notes</B></P>");
				sb.Append(removalNotes + "<BR>");
			}

			//if (this.IsPreferredBoardRemovalDateSelected && (!OrderOnlyHasNonBoardItems()) && this.PreferredBoardRemovalDate.HasValue)
			//{
			//    sb.Append("<P><B>Removal </B></P>");
			//    sb.Append(removalNotes + "<BR>");
			//}

			if (this.MarketingDeliveryType == MarketingDeliveryType.ToOFfice)
			{
				sb.Append("<P><B>Delivery Address</B></P>");
				sb.Append(this.OfficeDeliveryAddress + "<BR>");
			}
			else if (this.MarketingDeliveryType == MarketingDeliveryType.OtherAddress)
			{
				sb.Append("<P><B>Delivery Address</B></P>");
				sb.Append(this.MarketingDeliveryAddress.StreetNo + " " + this.MarketingDeliveryAddress.StreetName + " " +
							this.MarketingDeliveryAddress.Suburb + " " +
							this.MarketingDeliveryAddress.State + " " + this.MarketingDeliveryAddress.PostCode + "<BR>");
			}

			if (this.isPreferredBoardErectionDateSelected==true && this.PreferredBoardErectionType != PreferredDateType.NotSelected && this.PreferredBoardErectionDate.HasValue == true)
			{
				sb.AppendFormat("<p>Preferred Erection Date: {0} {1:dd-MMM-yyyy}</P>", this.PreferredBoardErectionType.Description(), this.PreferredBoardErectionDate.Value);
			}
			//Logger.Warn("Client has DIY Template: ClientID: {0}, test{1}", this.isPreferredBoardRemovalDateSelected, this.PreferredBoardRemovalDate.Value);
			if (this.PreferredBoardRemovalType != PreferredDateType.NotSelected && this.PreferredBoardRemovalDate.HasValue == true)
			{
				//Logger.Warn("Client has DIY Template: ClientID: {0}, test{1}", this.isPreferredBoardRemovalDateSelected, this.PreferredBoardRemovalDate.Value);

				sb.AppendFormat("<p>Preferred Removal Date: {0} {1:dd-MMM-yyyy}</P>", this.PreferredBoardRemovalType.Description(), this.PreferredBoardRemovalDate.Value);
			}

			sb.Append(line);

			return sb.ToString();
		}
		#endregion

        #region GetClientHTMLString
        public string GetClientHTMLString()
        {
            StringBuilder sb = new StringBuilder();
            if (!IsArtworkUpload)
            {
                if (isDIYOrder)
                {
                    sb.Append("<p>Design Option: User Designed</p>");
                }
                else if (OrderType == OrderType.AbcDesign)
                {
                    sb.Append("<p>Design Option: ABC Design</p>");
                }
                else if (OrderType == OrderType.B2B)
                {
                    sb.Append("<p>Design Option: This is B2B Order</p>");
                }
                else
                {
                    sb.Append("<p>Design Option: Client Will Upload Artwork</p>");
                }
            }
            else
            {
                sb.Append("<p>Design Option: Client Will Upload Artwork</p>");
            }

            if (!this.IsOnlyListingProduct())
            {
                sb.Append("<P><B>Your Contact Details</B></P>");
                sb.Append("Contact Name   : " + ContactDetailName + "<BR>");
                sb.Append("Contact Number : " + ContactNumber + "<BR>");
                sb.Append("Send Proof By  : " + SendProofBy + "<BR>");
                sb.Append("Send Proof To  : " + SendProofTo + "<BR>");
                if (ClientRefNumber != null && ClientRefNumber.Length > 0)
                {
                    sb.Append("Clients Reference Id  : " + ClientRefNumber + "<BR>");
                }
                if (DeliveryInfo != null)
                {
                    if (!string.IsNullOrEmpty(DeliveryInfo.Name))
                    {
                        sb.Append("Delivery Name  : " + DeliveryInfo.Name + "<BR>");
                    }
                    if (!string.IsNullOrEmpty(DeliveryInfo.ContactEmail))
                    {
                        sb.Append("Delivery Email : " + DeliveryInfo.ContactEmail + "<BR>");
                    }
                    sb.Append("Deliver To     : " + DeliveryPref.ToString() + " Address<BR>");
                    sb.Append("                 " + DeliveryInfo.StreetAddress + "<BR>");
                    sb.Append("                 " + DeliveryInfo.Suburb + " - " + DeliveryInfo.PostCode + ", " + DeliveryInfo.State + "<BR>");
                    
                }
                sb.Append("<br />");
                sb.Append(line);
            }
            // Show Products Details
            sb.Append("<P><B>Products Ordered</B></P>");
            foreach (CartItem anItem in Cart)
            {
                //sb.Append(anItem.GetHTMLString());
                //sb.Append("<BR>");

                if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {

                    sb.Append(anItem.GetPackageHTMLString());
                }
                else
                {
                    sb.Append(anItem.GetHTMLString());
                }
                sb.Append("<BR>");
            }

            sb.Append(line);

            if (Property != null)
            {
                sb.Append(Property.GetHTMLString());
                sb.Append("<br />");
                sb.Append(line);
            }

            if (ShouldBindSaleInfo())
            {
                sb.Append(GetSaleInfoHTMLString());
                sb.Append("<br />");
                sb.Append(line);
            }

            // Show the Notes
            if (!IsOnlyListingProduct())
            {
                sb.Append("<P><B>Notes</B></P>");
                sb.Append(Notes + "<BR>");
                sb.Append(line);
            }

            //Show the ABCrealestate.com.au Link if it exists
            if (this.commonDetails.IsInsertAbcRealEstateLink)
            {
                sb.Append("<B>USE ABCrealestate.com.au Link</B><BR>");
                sb.Append(line);
            }

            // Show the Erection Notes if there is any
            if (this.erectionNotes != "" && (!OrderOnlyHasNonBoardItems()))
            {
                sb.Append("<P><B>Erection Notes</B></P>");
                sb.Append(erectionNotes + "<BR>");
            }

            if (this.removalNotes != "" && (!OrderOnlyHasNonBoardItems()))
            {
                sb.Append("<P><B>Removal Notes</B></P>");
                sb.Append(removalNotes + "<BR>");
            }

            //if (this.IsPreferredBoardRemovalDateSelected && (!OrderOnlyHasNonBoardItems()) && this.PreferredBoardRemovalDate.HasValue)
            //{
            //    sb.Append("<P><B>Removal </B></P>");
            //    sb.Append(removalNotes + "<BR>");
            //}

            if (this.MarketingDeliveryType == MarketingDeliveryType.ToOFfice)
            {
                sb.Append("<P><B>Delivery Address</B></P>");
                sb.Append(this.OfficeDeliveryAddress + "<BR>");
            }
            else if (this.MarketingDeliveryType == MarketingDeliveryType.OtherAddress)
            {
                sb.Append("<P><B>Delivery Address</B></P>");
                sb.Append(this.MarketingDeliveryAddress.StreetNo + " " + this.MarketingDeliveryAddress.StreetName + " " +
                            this.MarketingDeliveryAddress.Suburb + " " +
                            this.MarketingDeliveryAddress.State + " " + this.MarketingDeliveryAddress.PostCode + "<BR>");
            }

            if (this.isPreferredBoardErectionDateSelected == true && this.PreferredBoardErectionType != PreferredDateType.NotSelected && this.PreferredBoardErectionDate.HasValue == true)
            {
                sb.AppendFormat("<p>Preferred Erection Date: {0} {1:dd-MMM-yyyy}</P>", this.PreferredBoardErectionType.Description(), this.PreferredBoardErectionDate.Value);
            }
            //Logger.Warn("Client has DIY Template: ClientID: {0}, test{1}", this.isPreferredBoardRemovalDateSelected, this.PreferredBoardRemovalDate.Value);
            if (this.PreferredBoardRemovalType != PreferredDateType.NotSelected && this.PreferredBoardRemovalDate.HasValue == true)
            {
                //Logger.Warn("Client has DIY Template: ClientID: {0}, test{1}", this.isPreferredBoardRemovalDateSelected, this.PreferredBoardRemovalDate.Value);

                sb.AppendFormat("<p>Preferred Removal Date: {0} {1:dd-MMM-yyyy}</P>", this.PreferredBoardRemovalType.Description(), this.PreferredBoardRemovalDate.Value);
            }

            sb.Append(line);

            return sb.ToString();
        }
        #endregion

		#region GetHTMLString
		public string GetModifyDIYOrderHTMLString()
		{
			StringBuilder sb = new StringBuilder();
			if (isDIYOrder)
			{
				sb.Append("<p>Design Option: User Designed</p>");
			}

			// Show Products Details
			sb.Append("<P><B>DIY Products Ordered</B></P>");
			foreach (CartItem anItem in Cart)
			{
				sb.Append(anItem.GetHTMLString());
				sb.Append("<BR>");
			}

			sb.Append(line);

			if (Property != null)
			{
				sb.Append(Property.GetHTMLString());
				sb.Append("<br />");
				sb.Append(line);
			}

			sb.Append(line);

			return sb.ToString();
		}
		#endregion

		#region GetXml
		public string GetXml()
		{
			StringBuilder sb = new StringBuilder();
			if (isDIYOrder)
			{
				sb.Append("<DesignOption>DIY</DesignOption>\r\n");
			}
			else if (OrderType == OrderType.AbcDesign)
			{
				sb.Append("<DesignOption>ABCDesign</DesignOption>\r\n");
			}
			else if (OrderType == OrderType.B2B)
			{
				sb.Append("<DesignOption>This is B2B Order</DesignOption>\r\n");
			}
			else
			{
				sb.Append("<DesignOption>Client Will Upload Artwork</DesignOption>\r\n");
			}


			if (!IsOnlyListingProduct())
			{
				sb.Append("<ContactDetails>\r\n");

				sb.Append("<ContactName>" + HttpUtility.HtmlEncode(ContactDetailName) + "</ContactName>\r\n");
				sb.Append("<ContactNumber>" + HttpUtility.HtmlEncode(ContactNumber) + "</ContactNumber>\r\n");
				sb.Append("<SendProofBy>" + HttpUtility.HtmlEncode(SendProofBy) + "</SendProofBy>\r\n");
				sb.Append("<SendProofTo>" + HttpUtility.HtmlEncode(SendProofTo) + "</SendProofTo>\r\n");
				if (ClientRefNumber != null && ClientRefNumber.Length > 0)
				{
					sb.Append("<ClientsReferenceId>" + HttpUtility.HtmlEncode(ClientRefNumber) + "</ClientsReferenceId>\r\n");
				}
				sb.Append("</ContactDetails>\r\n");
			}
			sb.Append(property.GetXml());

			if (ShouldBindSaleInfo())
			{
				sb.Append(GetSaleInfoXml());
			}

			if (this.IsListingonAbcRe == true)
			{
				sb.Append("<TransformListing>");

				sb.Append("<OnlineListing>\r\n");

				switch (this.listingOrder.ListingTypeId)
				{
					case 1:
						sb.AppendFormat("<ListingType>For Sale - Residential</ListingType>\r\n");
						break;
					case 2:
						sb.AppendFormat("<ListingType>For Sale - Commercial</ListingType>\r\n");
						break;
					case 3:
						sb.AppendFormat("<ListingType>For Rent - Residential</ListingType>\r\n");
						break;
					case 4:
						sb.AppendFormat("<ListingType>For Lease - Commercial</ListingType>\r\n");
						break;
					default:
						break;
				}

				sb.AppendFormat("<ListingTypeId>{0}</ListingTypeId>\r\n", this.listingOrder.ListingTypeId);
				if (!string.IsNullOrEmpty(this.ListingOrder.PropertyTypeName))
				{
					sb.AppendFormat("<PropertyType>{0}</PropertyType>\r\n", this.ListingOrder.PropertyTypeName);
				}

				if (!string.IsNullOrEmpty(this.ListingOrder.Price))
					sb.AppendFormat("<Price>{0}</Price>\r\n", HttpUtility.HtmlEncode(this.ListingOrder.Price));

				if (this.ListingOrder.DisplayPrice != "Do not display any price") 
					sb.AppendFormat("<ShowPrice>true</ShowPrice>\r\n");
				else
					sb.AppendFormat("<ShowPrice>false</ShowPrice>\r\n");

				if (!string.IsNullOrEmpty(this.ListingOrder.DisplayThisPriceInstead))
					sb.AppendFormat("<PriceView>{0}</PriceView>\r\n", HttpUtility.HtmlEncode(this.ListingOrder.DisplayThisPriceInstead));

				if (!string.IsNullOrEmpty(this.ListingOrder.RentAmount))
				{
					sb.AppendFormat("<Rent>{0}</Rent>\r\n", this.ListingOrder.RentAmount);
					sb.AppendFormat("<ShowRent>{0}</ShowRent>\r\n", this.ListingOrder.ShowRent.ToString().ToLower());
					if (this.ListingOrder.RentAvailableFrom.HasValue)
					{
						sb.AppendFormat("<DateAvailable>{0}</DateAvailable>\r\n", HttpUtility.HtmlEncode(this.ListingOrder.RentAvailableFrom.Value.ToShortDateString()));
					}
				}

				if (!string.IsNullOrEmpty(this.ListingOrder.AnnualLease))
					sb.AppendFormat("<Lease>{0}</Lease>\r\n", HttpUtility.HtmlEncode(this.ListingOrder.AnnualLease));
				if (!string.IsNullOrEmpty(this.TextDetails.Bedrooms))
					sb.AppendFormat("<Bed>{0}</Bed>\r\n", HttpUtility.HtmlEncode(this.TextDetails.Bedrooms));
				if (!string.IsNullOrEmpty(this.TextDetails.Bathrooms))
					sb.AppendFormat("<Bath>{0}</Bath>\r\n", HttpUtility.HtmlEncode(this.TextDetails.Bathrooms));
				if (!string.IsNullOrEmpty(this.TextDetails.CarportsOrGarages))
					sb.AppendFormat("<Car>{0}</Car>\r\n", HttpUtility.HtmlEncode(this.TextDetails.CarportsOrGarages));
				if (!string.IsNullOrEmpty(this.ListingOrder.LandSize))
					sb.AppendFormat("<Building><size>{0}</size><unit>{1}</unit></Building>\r\n", HttpUtility.HtmlEncode(this.ListingOrder.LandSize), HttpUtility.HtmlEncode(this.ListingOrder.LandUnitMeasure));
				if (this.CommonDetails.SaleType.ToUpper() == "AUCTION")
					sb.AppendFormat("<Authority>Auction</Authority>\r\n");
				else if (this.CommonDetails.SaleType.ToUpper() == "FOR SALE")
					sb.AppendFormat("<Authority>Exclusive \"For Sale\"</Authority>\r\n");

				sb.Append("</OnlineListing>\r\n");

				sb.Append("</TransformListing>");
			}

			if (this.IncludeSMSOnDemand==true && this.SmsOrder != null)
				sb.AppendFormat("<ShowSmsSticker>{0}</ShowSmsSticker>", "true");

			if (CommonDetails.IsInsertAbcRealEstateLink)
				sb.AppendFormat("<ShowAbcReSticker>{0}</ShowAbcReSticker>", "true");

			// Show the Notes
			if (!IsOnlyListingProduct())
			{
				sb.Append("<Notes><![CDATA[\r\n");
				if (!string.IsNullOrEmpty(Notes) && Notes.Trim().Length > 0) sb.Append(Notes + "\r\n");
				if (CommonDetails.IsInsertAbcRealEstateLink)
				{
					sb.Append("USE ABCrealestate.com.au Link" + "\r\n");
				}

				if (this.TextDetails.HasTextReceived || this.TextDetails.TextSelection != "0")
				{
					if (!string.IsNullOrEmpty(this.TextDetails.Bedrooms))
						sb.AppendFormat("Bed: {0}\r\n", this.TextDetails.Bedrooms);

					if (!string.IsNullOrEmpty(this.TextDetails.Bathrooms))
						sb.AppendFormat("Bath: {0}\r\n", this.TextDetails.Bathrooms);

					if (!string.IsNullOrEmpty(this.TextDetails.Studyrooms))
						sb.AppendFormat("Study: {0}\r\n", this.TextDetails.Studyrooms);

					if (!string.IsNullOrEmpty(this.TextDetails.CarportsOrGarages))
						sb.AppendFormat("Car: {0}\r\n", this.TextDetails.CarportsOrGarages);

					if (!string.IsNullOrEmpty(this.TextDetails.Bedrooms) && !string.IsNullOrEmpty(this.TextDetails.Bathrooms) && this.TextDetails.HasPool)
						sb.Append("Pool: 1\r\n");

					if (!string.IsNullOrEmpty(this.TextDetails.Bedrooms) && !string.IsNullOrEmpty(this.TextDetails.Bathrooms))
						sb.AppendFormat("Display Icons: {0}\r\n", (this.TextDetails.IsDisplayIcon) ? "Yes" : "No");
				}

				sb.Append("]]></Notes>" + "\r\n");
			}

			if (this.ErectionNotes != "" && (!OrderOnlyHasNonBoardItems()))
			{
				sb.Append("<ErectionNotes><![CDATA[\r\n");
				sb.Append(this.ErectionNotes + "\r\n");
				sb.Append("]]></ErectionNotes>\r\n");
			}
			if (this.PreferredBoardErectionType != PreferredDateType.NotSelected && this.PreferredBoardErectionDate.HasValue == true)
			{
				sb.AppendFormat("<PreferredErectionDate>{0:dd-MMM-yyyy}</PreferredErectionDate>", this.PreferredBoardErectionDate.Value);
				sb.AppendFormat("<PreferredErectionDateType>{0}</PreferredErectionDateType>", this.PreferredBoardErectionType.Description());
			}
			if (this.PreferredBoardRemovalType != PreferredDateType.NotSelected && this.PreferredBoardRemovalDate.HasValue == true)
			{
				sb.AppendFormat("<PreferredRemovalDate>{0:dd-MMM-yyyy}</PreferredRemovalDate>", this.PreferredBoardRemovalDate.Value);
				sb.AppendFormat("<PreferredRemovalDateType>{0}</PreferredRemovalDateType>", this.PreferredBoardRemovalType.Description());
			}

			// Show Products Details
			sb.Append("<Products>" + "\r\n");
			foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem anItem in Cart)
			{
				sb.Append(anItem.GetXml());
			}

			#region SMS On Demand Service
			if (this.IncludeSMSOnDemand == true && this.SmsOrder != null)
			{
				sb.Append("<SmsOnDemand>\r\n");

				sb.AppendFormat("<TextMessage><![CDATA[\r\n{0}\r\n]]></TextMessage>\r\n", this.smsOrder.SmsText);
				sb.AppendFormat("<AgentMobileNo>{0}</AgentMobileNo>\r\n", HttpUtility.HtmlEncode(this.smsOrder.AgentMobileNo));
				sb.AppendFormat("<NotifyAgent>{0}</NotifyAgent>\r\n", HttpUtility.HtmlEncode(this.SmsOrder.NotifyAgent.ToString().ToLower()));
				if (this.SmsOrder.NotifyAgent) sb.AppendFormat("<SendEmail>{0}</SendEmail>\r\n", HttpUtility.HtmlEncode(this.SmsOrder.NotifyAgent.ToString().ToLower()));
				if (this.SmsOrder.NotifyAgent) sb.AppendFormat("<AgentEmailAddress>{0}</AgentEmailAddress>\r\n", HttpUtility.HtmlEncode(this.SmsOrder.AgentEmailAddress));

				sb.AppendFormat("<AllowMMS>{0}</AllowMMS>\r\n", HttpUtility.HtmlEncode(this.SmsOrder.AllowMMS.ToString().ToLower()));

				sb.Append("</SmsOnDemand>\r\n");
			}
			#endregion


			sb.Append("</Products>" + "\r\n");

			//Include Text details
			if (ShouldIncludeTextDetail())
			{
				sb.Append(TextDetails.GetXML(this));
			}

			return sb.ToString(); ;
		}
		#endregion

		#region GetText
		public string GetText(OrderDisplayType orderDisplayType)
		{
			StringBuilder sb = new StringBuilder();

            if (IsExpressOrder.HasValue && IsExpressOrder.Value)
            {
                sb.AppendFormat("*************************************************\r\n");
                sb.AppendFormat("***  Order came through New Express Order Web Site  ***\r\n");
                sb.AppendFormat("*************************************************\r\n\r\n");
            }

			if (IsDIYOrder)
			{
				sb.AppendFormat("*************************************************\r\n");
				sb.AppendFormat("******   Design Option: User Designed  **********\r\n");
				sb.AppendFormat("*************************************************\r\n\r\n");

			}
			else if (OrderType == OrderType.AbcDesign)
			{
				sb.AppendFormat("Design Option: ABC Design\r\n\r\n");
			}
			else if (OrderType == OrderType.B2B)
			{
				sb.AppendFormat("******************************************************\r\n");
				sb.AppendFormat("******  This is B2B Order  ***************************\r\n");
				sb.AppendFormat("******************************************************\r\n\r\n");
			}
			else
			{
				sb.AppendFormat("******************************************************\r\n");
				sb.AppendFormat("******  Design Option: Client Will Upload Artwork  *\r\n");
				sb.AppendFormat("******************************************************\r\n\r\n");
			}

			if (!IsOnlyListingProduct())
			{
				sb.Append("Your Contact Details\r\n");
				sb.Append("---------------\r\n");
				sb.Append("Contact Name          : " + ContactDetailName + "\r\n");
				sb.Append("Contact Number        : " + ContactNumber + "\r\n");
				sb.Append("Send Proof By         : " + SendProofBy + "\r\n");
				sb.Append("Send Proof To         : " + SendProofTo + "\r\n");
				if (ClientRefNumber != null && ClientRefNumber.Length > 0)
				{
					sb.Append("Clients Reference Id  : " + ClientRefNumber + "\r\n");
				}
                if (DeliveryInfo != null)
                {
                    if (!string.IsNullOrEmpty(DeliveryInfo.Name))
                    {
                        sb.Append("Delivery Name         : " + DeliveryInfo.Name + "\r\n");
                    }
                    if (!string.IsNullOrEmpty(DeliveryInfo.ContactEmail))
                    {
                        sb.Append("Delivery Email        : " + DeliveryInfo.ContactEmail + "\r\n");
                    }
                    sb.Append("Deliver To            : " + DeliveryPref.ToString() + " Address \r\n");
                    sb.Append("                        " + DeliveryInfo.StreetAddress + "\r\n");
                    sb.Append("                        " + DeliveryInfo.Suburb + " - " + DeliveryInfo.PostCode + ", " + DeliveryInfo.State + "\r\n");
                    
                }
				sb.Append(TEXTLINE);
			}

			if (Property != null)
			{
				sb.Append("Property Details\r\n");
				sb.Append("----------------\r\n");
				sb.Append("Address: " + Property.PropertyAddress + " / ");
				sb.Append(Property.Location.PostCode + "\r\n");
				sb.Append("Property Type: " + Property.PropertyType + "\r\n");
				sb.Append(TEXTLINE);
			}


			if (ShouldBindSaleInfo())
			{
				sb.Append(GetSaleInfoText());
				sb.Append(TEXTLINE);
			}

			// Show the Notes
			if (!IsOnlyListingProduct())
			{
				sb.Append("\r\nNotes" + "\r\n");
				sb.Append("-----" + "\r\n");
				if (!string.IsNullOrEmpty(Notes) && Notes.Trim().Length > 0) sb.Append(Notes + "\r\n");
				if (CommonDetails.IsInsertAbcRealEstateLink)
				{
					sb.Append("USE ABCrealestate.com.au Link" + "\r\n");
				}

				if (this.TextDetails.HasTextReceived || this.TextDetails.TextSelection != "0")
				{
					if (!string.IsNullOrEmpty(this.TextDetails.Bedrooms))
						sb.AppendFormat("Bed: {0}\r\n", this.TextDetails.Bedrooms);

					if (!string.IsNullOrEmpty(this.TextDetails.Bathrooms))
						sb.AppendFormat("Bath: {0}\r\n", this.TextDetails.Bathrooms);

					if (!string.IsNullOrEmpty(this.TextDetails.Studyrooms))
						sb.AppendFormat("Study: {0}\r\n", this.TextDetails.Studyrooms);

					if (!string.IsNullOrEmpty(this.TextDetails.CarportsOrGarages))
						sb.AppendFormat("Car: {0}\r\n", this.TextDetails.CarportsOrGarages);

					if (!string.IsNullOrEmpty(this.TextDetails.Bedrooms) && !string.IsNullOrEmpty(this.TextDetails.Bathrooms) && this.TextDetails.HasPool)
						sb.Append("Pool: 1\r\n");

					if (!string.IsNullOrEmpty(this.TextDetails.Bedrooms) && !string.IsNullOrEmpty(this.TextDetails.Bathrooms))
						sb.AppendFormat("Display Icons: {0}\r\n", (this.TextDetails.IsDisplayIcon) ? "Yes" : "No");
				}
				sb.Append(TEXTLINE);
			}

			// Show the Erection Notes if it Exists
			if (this.ErectionNotes != "" && (!OrderOnlyHasNonBoardItems()))
			{
				sb.Append("Erection Notes" + "\r\n");
				sb.Append("--------------" + "\r\n");
				sb.Append(this.ErectionNotes + "\r\n");
				sb.Append(TEXTLINE);
			}

			if (!string.IsNullOrEmpty(this.InstallationFile))
			{
				sb.Append("Installation Diagram/Map has been uploaded" + "\r\n");
				sb.Append("------------------------------------------" + "\r\n");
				sb.Append(TEXTLINE);

			}

			if (this.PreferredBoardErectionType != PreferredDateType.NotSelected && this.PreferredBoardErectionDate.HasValue == true)
			{
				sb.AppendFormat("Preferred Erection Date: {0} {1:dd-MMM-yyyy}\r\n", this.PreferredBoardErectionType.Description(), this.PreferredBoardErectionDate.Value);
			}
			if (this.PreferredBoardRemovalType != PreferredDateType.NotSelected && this.PreferredBoardRemovalDate.HasValue == true)
			{
				sb.AppendFormat("Preferred Removal Date: {0} {1:dd-MMM-yyyy}\r\n", this.PreferredBoardRemovalType.Description(), this.PreferredBoardRemovalDate.Value);
			}

			if (this.IsSiteInspectionRequired)
			{
				sb.Append("Site Inspection Required" + "\r\n");
				sb.Append("------------------------" + "\r\n");
				if (!string.IsNullOrEmpty(this.SiteInspectionNotes))
				{
					sb.Append(this.SiteInspectionNotes + "\r\n");
				}
			}

			sb.Append(TEXTLINE);

			if (this.MarketingDeliveryType == MarketingDeliveryType.ToOFfice)
			{
				sb.Append("Delivery Address" + "\r\n");
				sb.Append("----------------" + "\r\n");
				sb.Append(this.OfficeDeliveryAddress + "\r\n");
				sb.Append(TEXTLINE);
			}
			else if (this.MarketingDeliveryType == MarketingDeliveryType.OtherAddress)
			{
				sb.Append("Delivery Address" + "\r\n");
				sb.Append("----------------" + "\r\n");
				sb.Append(this.MarketingDeliveryAddress.StreetNo + " " + this.MarketingDeliveryAddress.StreetName + " " +
							this.MarketingDeliveryAddress.Suburb + " " +
							this.MarketingDeliveryAddress.State + " " + this.MarketingDeliveryAddress.PostCode + "\r\n");
				sb.Append(TEXTLINE);
			}

			// Show Products Details
			sb.Append("Products Ordered" + "\r\n");
			sb.Append("----------------" + "\r\n");
			foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem anItem in Cart)
			{
				if (orderDisplayType == OrderDisplayType.NormalOrderOnly)
				{
					if (anItem.TypeId == (int)ProductTypes.Photography)
						continue;
                    else if (!ClientHasStockboardDIYPreference && anItem.TypeId == (int)ProductTypes.Stockboard)
                        continue;

					// Don't display errection fee if the order does not have board.
					if (anItem.ProductName.Contains("Erection Fee") && OrderHasPhotosignBoard())
						continue;

				}
				else if (orderDisplayType == OrderDisplayType.PhotographyOrderOnly)
				{
					if (!(anItem.TypeId == (int)ProductTypes.Photography || anItem.TypeId == (int)ProductTypes.FloorPlans ||
							anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages ||
							anItem.TypeId == ProductTypes.OtherPackages))
						continue;
				}
				else if (orderDisplayType == OrderDisplayType.StockboardOrderOnly)
				{
					if (anItem.TypeId != (int)ProductTypes.Stockboard && anItem.TypeId != ProductTypes.BoardAccessory
                        && anItem.TypeId != ProductTypes.Overlay && anItem.TypeId != ProductTypes.StockboardOverlay && !(anItem.ProductName.Contains("Erection Fee")))
						continue;

                    if ((anItem.TypeId == ProductTypes.BoardAccessory || anItem.TypeId == ProductTypes.Overlay || anItem.TypeId == ProductTypes.StockboardOverlay) && Cart.Any(i => i.TypeId == ProductTypes.BillBoard))
						continue;
					// Don't display errection fee if the order has board, because the errection fee
					// will be included in the board order.
					if (OrderHasPhotosignBoard() && anItem.ProductName.Contains("Erection Fee"))
						continue;
				}
				if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					if (orderDisplayType == OrderDisplayType.PhotographyOrderOnly)
					{
						foreach (PackageGroup itemGroup in anItem.PackageGroups)
						{
							foreach (PackageContentProduct contentProductItem in itemGroup.Products)
							{
								if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
								{
									if (contentProductItem.TypeId == ProductTypes.Photography || contentProductItem.TypeId == ProductTypes.FloorPlans)
									{
										if (contentProductItem.ProductId > 0)
										{
											sb.Append("***** Product Id: " + contentProductItem.ProductId + Environment.NewLine);
										}

										if (!string.IsNullOrEmpty(contentProductItem.ProductName))
										{
											sb.Append("***** Product Name: " + contentProductItem.ProductName + Environment.NewLine);
										}

										if (contentProductItem.ProductConfig != null)
										{
											foreach (var field in contentProductItem.ProductConfig.Fields.Field)
											{
												sb.Append("***** " + field.Caption + " - " + field.Value + "\r\n");
											}
										}
									}
								}
							}
						}
					}
					else
					{
						sb.Append(anItem.GetPackageText());
					}
				}
				else
				{
					sb.Append(anItem.GetText());
				}
				sb.Append("\r\n");
			}

			if (orderDisplayType == OrderDisplayType.NormalOrderOnly)
			{
				#region SMS On Demand Service
				if (this.IncludeSMSOnDemand == true && this.SmsOrder != null)
				{
					sb.Append(TEXTLINE);

					sb.Append("'Details On Demand via SMS' on Board\r\n");
					sb.Append("------------------------------------\r\n");

					sb.AppendFormat("Text Message : {0}\r\n", this.SmsOrder.SmsText);
					sb.AppendFormat("Agent Mobile No. : {0}\r\n", this.SmsOrder.AgentMobileNo);
					sb.AppendFormat("Notify Agent : {0}{1}", (this.SmsOrder.NotifyAgent) ? "Yes" : "No",
								 (this.SmsOrder.NotifyAgent ? string.Format(", via email to {0}", this.SmsOrder.AgentEmailAddress) : ""));

					if (this.SmsOrder.AllowMMS)
						sb.Append("\r\nReply Using MMS : yes\r\n");
				}
				#endregion

				sb.Append(TEXTLINE);

				// Should include Text Details
				if (ShouldIncludeTextDetail())
				{
					sb.Append(TextDetails.GetText(property.PropertyAddressWithSuburb, this));
				}
			}

			return sb.ToString();
		}
		#endregion

		#region IsOnlyListingProduct
		public bool IsOnlyListingProduct()
		{
			return (this.Cart.Count == 1 && IsListingExists());
		}
		#endregion

		#region IsListingExists
		public bool IsListingExists()
		{
			//This value indicate that we display all info on xml
			return false;
		}
		#endregion

		#region ShouldBindSaleInfo
		public bool ShouldBindSaleInfo()
		{
			bool ret = false;
            if (IsDIYOrder)
            {
                return false;
            }
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.BillBoard || anItem.TypeId == ProductTypes.Brochure 
					|| anItem.TypeId == ProductTypes.WindowCard)
				{
					ret = true;
					break;
				}
				else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
                        if (item.IsUpgradeProductApplicable)
                        {
                            if (item.UpgradedProduct.TypeId == ProductTypes.BillBoard || anItem.TypeId == ProductTypes.Brochure || anItem.TypeId == ProductTypes.WindowCard)
                            {
                                ret = true;
                            }
                        }
                        else
                        {
                            foreach (PackageContentProduct contentProductItem in item.Products)
                            {
                                if (contentProductItem.UniqueId == item.SelectedUniqueId && (contentProductItem.TypeId == ProductTypes.BillBoard || contentProductItem.TypeId == ProductTypes.Brochure
                                    || contentProductItem.TypeId == ProductTypes.Stockboard || contentProductItem.TypeId == ProductTypes.WindowCard))
                                {
                                    ret = true;
                                    break;
                                }

                            }
                        }
						if (ret == true)
							break;
					}
				}
				else if (anItem.TypeId == ProductTypes.Stockboard)
				{
					if (Cart.Any(i => i.TypeId == ProductTypes.BillBoard || i.TypeId == ProductTypes.Brochure || i.TypeId == ProductTypes.WindowCard))
					{
						ret = true;
						break;
					}
				}
			}
			return ret;

		}
        #endregion

        public string GetSaleInfoHTMLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<P><B>Sale Details</B></P>");
            try
            {
                sb.Append("Sale Type: " + this.commonDetails.SaleType + "<BR>");
                if (this.commonDetails.SaleType.ToUpper() == "AUCTION" && !string.IsNullOrEmpty(this.commonDetails.AuctionDate) && this.commonDetails.AuctionDate.Length > 0 && !string.IsNullOrEmpty(CommonDetails.AuctionTime))
                {
                    sb.Append("Auction Date: " + this.commonDetails.AuctionDate + " at " + this.CommonDetails.AuctionTime);
                }
            }
            catch (Exception)
            {

            }

            return sb.ToString();
        }

        public string GetSaleInfoXml()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<SaleDetails>\r\n");

            try
            {
                sb.Append("<SaleType>").Append(CommonDetails.SaleType).Append("</SaleType>\r\n");
                if (CommonDetails.SaleType.ToUpper() == "AUCTION" && !string.IsNullOrEmpty(CommonDetails.AuctionDate) && CommonDetails.AuctionDate.Length > 0 && !string.IsNullOrEmpty(CommonDetails.AuctionTime))
                {
                    sb.Append("<AuctionDate>").Append(HttpUtility.HtmlEncode(CommonDetails.AuctionDate + " at " + CommonDetails.AuctionTime)).Append("</AuctionDate>\r\n");
                }
                if (CommonDetails.SaleType.ToUpper() == "AUCTION" && !string.IsNullOrEmpty(CommonDetails.StandardAuctionDateTime) && !string.IsNullOrEmpty(CommonDetails.AuctionTime))
                {
                    sb.Append("<StandardAuctionDateTime>").Append(HttpUtility.HtmlEncode(CommonDetails.StandardAuctionDateTime + "T" + CommonDetails.AuctionTime)).Append("</StandardAuctionDateTime>\r\n");
                }
            }
            catch (Exception)
            {

            }

            sb.Append("</SaleDetails>\r\n");
            return sb.ToString();

        }

        public string GetSaleInfoText()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Sale Details\r\n");
			sb.Append("----------------\r\n");
			sb.Append("Sale Type: " + CommonDetails.SaleType + "\r\n");
			if (!string.IsNullOrEmpty(commonDetails.SaleType) && !string.IsNullOrEmpty(CommonDetails.AuctionDate) && !string.IsNullOrEmpty(CommonDetails.AuctionTime))
			{
				if (CommonDetails.SaleType.ToUpper() == "AUCTION" && CommonDetails.AuctionDate.Length > 0 && !string.IsNullOrEmpty(CommonDetails.AuctionTime))
				{
					sb.Append("Auction Date: " + CommonDetails.AuctionDate + " at " + CommonDetails.AuctionTime + "\r\n");
				}
			}

			return sb.ToString();
		}

		public bool OrderOnlyHasNonBoardItems()
		{
			bool ret = true;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.BillBoard || anItem.TypeId == ProductTypes.Stockboard 
					|| anItem.TypeId == ProductTypes.Other || anItem.TypeId == ProductTypes.Overlay)
				{
					ret = false;
					break;
				}
				else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
						foreach (PackageContentProduct contentProductItem in item.Products)
						{
							if (contentProductItem.TypeId == ProductTypes.BillBoard || contentProductItem.TypeId == ProductTypes.Stockboard
								|| contentProductItem.TypeId == ProductTypes.Other || contentProductItem.TypeId == ProductTypes.Overlay)
							{
								ret = false;
								break;
							}

						}
						if (ret == false)
							break;
					}

				}
			}
			return ret;
		}

        public bool OrderOnlyHasNonBoardExcludeFlatPackItems()
        {
            bool ret = true;
            foreach (CartItem anItem in Cart)
            {
                if ((anItem.TypeId == ProductTypes.BillBoard && !string.IsNullOrEmpty(anItem.ProductName) && !anItem.ProductName.ToUpper().Contains("FLATPACK")) || anItem.TypeId == ProductTypes.Stockboard
                    || anItem.TypeId == ProductTypes.Other || (anItem.TypeId == ProductTypes.Overlay && !anItem.ProductName.ToUpper().Contains("DELIVER")) || anItem.TypeId == ProductTypes.BoardAccessory || anItem.TypeId == ProductTypes.StockboardOverlay || anItem.TypeId == ProductTypes.DIYStickers)
                {
                    ret = false;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if ((contentProductItem.TypeId == ProductTypes.BillBoard && !string.IsNullOrEmpty(contentProductItem.ProductName) && !contentProductItem.ProductName.ToUpper().Contains("FLATPACK")) || contentProductItem.TypeId == ProductTypes.Stockboard
                                || contentProductItem.TypeId == ProductTypes.Other || (contentProductItem.TypeId == ProductTypes.Overlay && !contentProductItem.ProductName.ToUpper().Contains("DELIVER")) || contentProductItem.TypeId == ProductTypes.BoardAccessory || contentProductItem.TypeId == ProductTypes.StockboardOverlay
                                || contentProductItem.TypeId == ProductTypes.DIYStickers)
                            {
                                ret = false;
                                break;
                            }

                        }
                        if (ret == false)
                            break;
                    }

                }
            }
            return ret;
        }

		public bool OrderHasBoard()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.BillBoard)
				{
					ret = true;
					break;
				}
				else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
						foreach (PackageContentProduct contentProductItem in item.Products)
						{
							if (contentProductItem.TypeId == ProductTypes.BillBoard)
							{
								ret = true;
								break;
							}

						}
						if (ret == true)
							break;
					}

				}
			}
			return ret;
		}

        public bool OrderHasBoardOverlay()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.StockboardOverlay)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.StockboardOverlay)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
        }

        public bool OrderHasCustomOverlayInstalled()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.ProductId == 112 || anItem.ProductId == 113 || anItem.ProductId == 114 || anItem.ProductId == 621
                    || anItem.ProductId == 1536 || anItem.ProductId == 2016 || anItem.ProductId == 2319 || anItem.ProductId == 2320
                    || anItem.ProductId == 3297 || anItem.ProductId == 4610 || anItem.ProductId == 7205 || anItem.ProductId == 8103
                    || anItem.ProductId == 11782 || anItem.ProductId == 11855 || anItem.ProductId == 12367 || anItem.ProductId == 12550
                    || anItem.ProductId == 13113 || anItem.ProductId == 16284 || anItem.ProductId == 16298 || anItem.ProductId == 16299
                    || anItem.ProductId == 16334 || anItem.ProductId == 16382 || anItem.ProductId == 16726 || anItem.ProductId == 16727
                    || anItem.ProductId == 18243 || anItem.ProductId == 19633 || anItem.ProductId == 19797 || anItem.ProductId == 19798
                    || anItem.ProductId == 19936 || anItem.ProductId == 20017 || anItem.ProductId == 20301 || anItem.ProductId == 20400
                    || anItem.ProductId == 20428 || anItem.ProductId == 20647 || anItem.ProductId == 21044 || anItem.ProductId == 20976 
                    || anItem.ProductId == 21035 || anItem.ProductId == 20896 || anItem.ProductId == 20897 || anItem.ProductId == 20898)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.ProductId == 112 || contentProductItem.ProductId == 113 || contentProductItem.ProductId == 114 || contentProductItem.ProductId == 621
                                || contentProductItem.ProductId == 1536 || contentProductItem.ProductId == 2016 || contentProductItem.ProductId == 2319 || contentProductItem.ProductId == 2320
                                || contentProductItem.ProductId == 3297 || contentProductItem.ProductId == 4610 || contentProductItem.ProductId == 7205 || contentProductItem.ProductId == 8103
                                || contentProductItem.ProductId == 11782 || contentProductItem.ProductId == 11855 || contentProductItem.ProductId == 12367 || contentProductItem.ProductId == 12550
                                || contentProductItem.ProductId == 13113 || contentProductItem.ProductId == 16284 || contentProductItem.ProductId == 16298 || contentProductItem.ProductId == 16299
                                || contentProductItem.ProductId == 16334 || contentProductItem.ProductId == 16382 || contentProductItem.ProductId == 16726 || contentProductItem.ProductId == 16727
                                || contentProductItem.ProductId == 18243 || contentProductItem.ProductId == 19633 || contentProductItem.ProductId == 19797 || contentProductItem.ProductId == 19798
                                || contentProductItem.ProductId == 19936 || contentProductItem.ProductId == 20017 || contentProductItem.ProductId == 20301 || contentProductItem.ProductId == 20400
                                || contentProductItem.ProductId == 20428 || contentProductItem.ProductId == 20647 || contentProductItem.ProductId == 21044 || contentProductItem.ProductId == 20976
                                || contentProductItem.ProductId == 21035 || contentProductItem.ProductId == 20896 || contentProductItem.ProductId == 20897 || contentProductItem.ProductId == 20898)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
        }

        public bool OrderHasBoardNotIncludingCommunityBoard()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.BillBoard && anItem.Attributes.Any(a => a.Key == "ContentType" && !a.Value.Contains("Community Board")))
				{
					ret = true;
					break;
				}
				else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
						foreach (PackageContentProduct contentProductItem in item.Products)
						{
                            if (contentProductItem.TypeId == ProductTypes.BillBoard && contentProductItem.Attributes.Any(a => a.Key == "ContentType" && !a.Value.Contains("Community Board")))
							{
								ret = true;
								break;
							}

						}
						if (ret == true)
							break;
					}

				}
			}
			return ret;
		}

        public bool OrderHasBoardNotIncludingFlatPack()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if ((anItem.TypeId == ProductTypes.BillBoard || anItem.TypeId == ProductTypes.Stockboard) && !anItem.FrameType.Contains("Corflute"))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.UniqueId == item.SelectedUniqueId && (contentProductItem.TypeId == ProductTypes.BillBoard || contentProductItem.TypeId == ProductTypes.Stockboard) && !contentProductItem.FrameType.Contains("Corflute"))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
        }

        public bool OrderHasCommunityBoardOnly()
        {
            bool ret = false;

            if (Cart != null && Cart.Count == 1 && Cart[0].ItemQty == 1)
            {
                CartItem anItem = Cart[0];
                if (anItem != null && anItem.TypeId == ProductTypes.BillBoard)
                {
                    foreach (AbcKeyValuePair item in anItem.Attributes)
                    {
                        if (item.Key == "ContentType" && item.Value.Contains("Community Board"))
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }
            return ret;
        }

		public bool OrderHasBrochure()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.Brochure)
				{
					ret = true;
					break;
				}
				else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
						foreach (PackageContentProduct contentProductItem in item.Products)
						{
							if (contentProductItem.TypeId == ProductTypes.Brochure)
							{
								ret = true;
								break;
							}

						}
						if (ret == true)
							break;
					}
				}
			}
			return ret;
		}

        public bool OrderHasDIYSticker()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.DIYStickers)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.DIYStickers)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasSoldSticker()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.ForPrinting)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.ForPrinting)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasCorflute()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Corflute)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Corflute)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

		public bool OrderHasWindowCard()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.WindowCard)
				{
					ret = true;
					break;
				}
				else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
						foreach (PackageContentProduct contentProductItem in item.Products)
						{
							if (contentProductItem.TypeId == ProductTypes.WindowCard)
							{
								ret = true;
								break;
							}

						}
						if (ret == true)
							break;
					}
				}
			}
			return ret;
		}

		public bool OrderHasPhotosignBoard()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.BillBoard)
				{
					foreach (AbcKeyValuePair item in anItem.Attributes)
					{
						if (item.Key == "ContentType" && item.Value == "Photo Board")
						{
							ret = true;
							break;
						}
					}
				}
			}
			return ret;
		}

		public bool OrderHasTextBoard()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.BillBoard)
				{
					foreach (AbcKeyValuePair item in anItem.Attributes)
					{
						if (item.Key == "ContentType" && item.Value == "Text Board")
						{
							ret = true;
							break;
						}
					}
				}
			}
			return ret;
		}

		public bool OrderHasCommunityBoard()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.BillBoard)
				{
					foreach (AbcKeyValuePair item in anItem.Attributes)
					{
						if (item.Key == "ContentType" && item.Value.Contains("Community Board"))
						{
							ret = true;
							break;
						}
					}
				}
			}
			return ret;
		}

        public bool OrderHasStockboardOverlaysOnly()
        {
            bool ret = true;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId != ProductTypes.StockboardOverlay && anItem.TypeId != ProductTypes.Overlay)
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }

        public bool OrderHasSpotlight()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.Other && anItem.ProductName.Contains("Spotlight"))
				{
					ret = true;
					break;
				}
			}
			return ret;
		}

		public bool OrderHasPhotographyorFloorPlan()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.Photography || anItem.TypeId == ProductTypes.FloorPlans)
				{
					ret = true;
					break;
				}
				else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
						foreach (PackageContentProduct contentProductItem in item.Products)
						{
							if (contentProductItem.TypeId == ProductTypes.Photography || contentProductItem.TypeId == ProductTypes.FloorPlans)
							{
								ret = true;
								break;
							}

						}
						if (ret == true)
							break;
					}
				}
			}
			return ret;
		}

        public bool OrderHasOtherProductNotJustPhotographyorFloorPlan()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId != ProductTypes.Photography && contentProductItem.TypeId != ProductTypes.FloorPlans)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
                else if (anItem.TypeId != ProductTypes.Photography && anItem.TypeId != ProductTypes.FloorPlans)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public bool OrderHasStockboard()
		{
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Stockboard)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
		}

        public bool OrderHasOverlay()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Overlay)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public bool OrderHasOverlayExcludeUnitStickerAndNamePlates()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if ((anItem.TypeId == ProductTypes.Overlay) || (anItem.TypeId == ProductTypes.StockboardOverlay && anItem.ProductId != ProductSettings.NamePlatesABoard4x3StockBoards && anItem.ProductId != ProductSettings.NamePlatesCBoard6x4StockBoards
                                                                                            && anItem.ProductId != ProductSettings.NamePlatesDBoard8x4StockBoards && anItem.ProductId != ProductSettings.UnitStickerForStockBoards && anItem.ProductId != 14030 && anItem.ProductId != 9311
                                                                                            && anItem.ProductId != 13677 && anItem.ProductId != 14031 && anItem.ProductId != 14103 && anItem.ProductId != 14703))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }


        public bool InterstateOrderHasOverlayExcludeUnitStickerAndNamePlates()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if ((anItem.TypeId == ProductTypes.Overlay) || (anItem.TypeId == ProductTypes.StockboardOverlay && anItem.ProductId != ProductSettings.NamePlatesABoard4x3StockBoards && anItem.ProductId != ProductSettings.NamePlatesCBoard6x4StockBoards
                                                                                           && anItem.ProductId != ProductSettings.NamePlatesDBoard8x4StockBoards && anItem.ProductId != ProductSettings.UnitStickerForStockBoards && anItem.ProductId != 14030 && anItem.ProductId != 9311
                                                                                           && anItem.ProductId != 13677))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public bool InterstateOrderHasOverlayOrUnitStickerOrNamePlates()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if ((anItem.TypeId == ProductTypes.Printing || anItem.TypeId == ProductTypes.Overlay || anItem.TypeId == ProductTypes.ForPrinting || anItem.TypeId == ProductTypes.StockboardOverlay || anItem.TypeId == ProductTypes.DIYStickers)
                    && !string.IsNullOrEmpty(anItem.ProductName) && (anItem.ProductName.ToLower().Contains("overlay") || anItem.ProductName.ToLower().Contains("sticker") || anItem.ProductName.ToLower().Contains("plate")))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public bool OrderHasUnitStickerForStockBoardProduct()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.StockboardOverlay && anItem.ProductId == ProductSettings.UnitStickerForStockBoards)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public bool OrderHasStockboardIncludePackageCheck()
		{
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Stockboard)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Stockboard)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
		}

        public bool OrderHasPackAndStockboardInsideThePack()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Stockboard)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
        }

        public bool OrderHasPackAndPhotographyOrFloorplanInsideThePack()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Photography || contentProductItem.TypeId == ProductTypes.FloorPlans)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public string GetStockboardName()
        {
            string name = string.Empty;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Stockboard)
                {
                    name = anItem.ProductName;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Stockboard)
                            {
                                name = contentProductItem.ProductName;
                                break;
                            }

                        }
                        if (!string.IsNullOrEmpty(name))
                            break;
                    }

                }
            }
            return name;
        }


		public bool OrderHasOtherProductsOtherThanStockboardAndPhotography()
		{
			bool ret = false;
			foreach (CartItem anItem in Cart)
			{
                if (!(anItem.TypeId == ProductTypes.Stockboard || anItem.TypeId == ProductTypes.Photography
                    || anItem.TypeId == ProductTypes.FloorPlans || ((anItem.TypeId == ProductTypes.BoardAccessory || anItem.TypeId == ProductTypes.Overlay || anItem.TypeId == ProductTypes.StockboardOverlay) && Cart.Any(i => i.TypeId == ProductTypes.Stockboard))))
                {
					ret = true;
					break;
				}
			}

			
			return ret;
		}

        public bool DIYOrderHasOtherProductsOtherThanPhotographyAndFloorplan()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (!(anItem.TypeId == ProductTypes.Photography
                    || anItem.TypeId == ProductTypes.FloorPlans))
                {
                    ret = true;
                    break;
                }
            }


            return ret;
        }

		public bool OrderHasDIYProduct()
		{
			bool ret = false;
			if(IsDIYOrder)
			{
				return true;
			}
			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.Packages)
				{
					foreach (PackageGroup item in anItem.PackageGroups)
					{
						foreach (PackageContentProduct contentProductItem in item.Products)
						{
							if (contentProductItem.ProductHasDIY())
							{
								ret = true;
								break;
							}

						}
					}
				}
			}
			return ret;
		}

		#region GetSpotlightOrders
		public List<CartItem> GetSpotlightOrders()
		{
			List<CartItem> aList = new List<CartItem>();

			foreach (CartItem anItem in Cart)
			{
				if (anItem.TypeId == ProductTypes.Other && anItem.ProductName.Contains("Spotlight"))
				{
					aList.Add(anItem);
				}
			}
			return aList;
		}
		#endregion

		public bool ShouldAskSaleInfo()
		{
			return this.ShouldBindSaleInfo();
		}

		public bool ShouldAskSMS()
		{
			return this.OrderHasBoard() || this.OrderHasStockboard();
		}

		public bool  ShouldAskListing()
		{
			if (!OnlineBLConfig.IS_NZ)
			{
				return this.OrderHasPhotosignBoard() || this.OrderHasTextBoard() || this.OrderHasBrochure() || this.OrderHasWindowCard();
			}
			return false;
		}

		public bool ShouldAskBoardPref()
		{
			return this.OrderHasBoard() || this.OrderHasStockboardIncludePackageCheck();
		}

		public bool ShouldAskBrochureText()
		{
			return (this.OrderHasBrochure() || this.OrderHasWindowCard());
		}

		public bool ShouldIncludeTextDetail()
		{
			return !this.IsDIYOrder;
		}

		public bool ShouldAskImagesDetails()
		{
			return (!this.IsDIYOrder && (this.OrderHasBrochure() || OrderHasPhotosignBoard())); 
		}


		public void SyncItemQty()
		{
			foreach (Abc.OnlineBL.Entities.Model.OnlineOrder.CartItem item in this.Cart)
			{
				#region ModularPackage
				if (item.TypeId == ProductTypes.Packages || item.TypeId == ProductTypes.BoardPackages || item.TypeId == ProductTypes.OtherPackages)
				{
                    //foreach (PackageGroup itemGroup in item.PackageGroups)
                    //{
                    //    foreach (PackageContentProduct contentProductItem in itemGroup.Products)
                    //    {
                    //        if (contentProductItem.UniqueId == itemGroup.SelectedUniqueId)
                    //        {
                    //            contentProductItem.SyncItemQty();
                    //        }
                    //    }
                    //}
				}
				#endregion

				#region Other Product Types
				else 
				{
					try
					{
						if (item.ProductId > 0)
						{
							item.SyncItemQty();
						}
					}
					catch (Exception ex)
					{
						Logger.Exception(ex, string.Format("ProductID{0}", item.ProductId));
					}
				}
				#endregion

			}
		}

        public int TotalBoard()
        {
            int total = 0;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.BillBoard)
                {
                    if(anItem.ItemQty == 0)
                    {
                        total = total + 1;
                    }
                    else
                    {
                        total = total + anItem.ItemQty;
                    }
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        if (item.IsUpgradeProductApplicable)
                        {
                            if (item.UpgradedProduct.TypeId == ProductTypes.BillBoard)
                            {
                                total = total + 1;
                            }
                        }
                        else
                        {
                            foreach (PackageContentProduct contentProductItem in item.Products)
                            {
                                if (contentProductItem.TypeId == ProductTypes.BillBoard)
                                {
                                    if (contentProductItem.UniqueId == item.SelectedUniqueId)
                                    {
                                        total = total + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return total;
        }

        public int TotalStockBoard()
        {
            int total = 0;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Stockboard)
                {
                    if (anItem.ItemQty == 0)
                    {
                        total = total + 1;
                    }
                    else
                    {
                        total = total + anItem.ItemQty;
                    }
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        if (item.IsUpgradeProductApplicable)
                        {
                            if (item.UpgradedProduct.TypeId == ProductTypes.Stockboard)
                            {
                                total = total + 1;
                            }
                        }
                        else
                        {
                            foreach (PackageContentProduct contentProductItem in item.Products)
                            {
                                if (contentProductItem.TypeId == ProductTypes.Stockboard)
                                {
                                    if (contentProductItem.UniqueId == item.SelectedUniqueId)
                                    {
                                        total = total + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return total;
        }

        public bool OrderHas3DLetteringBoard()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.BillBoard && anItem.ProductName.Contains("3D"))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.BillBoard && anItem.ProductName.Contains("3D"))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
        }

        public bool OrderHasLargeBoard()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.BillBoard && (anItem.SizeCode == "AUSSIE16x8" || anItem.SizeCode == "16 x 8 Board" || anItem.SizeCode == "8 x 10 Board"
                    || anItem.SizeCode == "8 x 12 Board" || anItem.SizeCode == "18 x 8 Board"
                    || anItem.SizeCode == "10 x 10 Board" || anItem.SizeCode == "10 x 12 Board"
                    || anItem.SizeCode == "12 x 12 Board" || anItem.SizeCode == "12 x 4 Board"
                    || anItem.SizeCode == "G" || anItem.SizeCode == "G Board - 2400mm x 2400mm (8x8)" || anItem.SizeCode == "H" || anItem.SizeCode == "H Board - 2400mm x 3000mm (8x10)"
                    || anItem.SizeCode == "ABISign8x12" || anItem.SizeCode == "ABISign8x16"
                    || anItem.SizeCode == "AUSSIE8x20" || anItem.SizeCode == "AUSSIE18x8"
                    || anItem.SizeCode == "AUSSIE8x10" || anItem.SizeCode == "AUSSIE8x12"))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.BillBoard && (contentProductItem.SizeCode == "AUSSIE16x8" || contentProductItem.SizeCode == "16 x 8 Board" || contentProductItem.SizeCode == "8 x 10 Board"
                                || contentProductItem.SizeCode == "8 x 12 Board" || contentProductItem.SizeCode == "18 x 8 Board"
                                || contentProductItem.SizeCode == "10 x 10 Board" || contentProductItem.SizeCode == "10 x 12 Board"
                                || contentProductItem.SizeCode == "12 x 12 Board" || contentProductItem.SizeCode == "12 x 4 Board"
                                || contentProductItem.SizeCode == "G" || contentProductItem.SizeCode == "G Board - 2400mm x 2400mm (8x8)" || contentProductItem.SizeCode == "H" || contentProductItem.SizeCode == "H Board - 2400mm x 3000mm (8x10)"
                                || contentProductItem.SizeCode == "ABISign8x12" || contentProductItem.SizeCode == "ABISign8x16"
                                || contentProductItem.SizeCode == "AUSSIE8x20" || contentProductItem.SizeCode == "AUSSIE18x8"
                                || contentProductItem.SizeCode == "AUSSIE8x10" || contentProductItem.SizeCode == "AUSSIE8x12"))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
        }

        public bool OrderHasHighInstallation()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.ProductId == 1534)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.ProductId == 1534)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }

                }
            }
            return ret;
        }

        public bool OrderHasDronePhotography()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Photography && anItem.ProductName.Contains("Drone"))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Photography && contentProductItem.ProductName.Contains("Drone"))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasSitePlanOverlayForPhotography()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.FloorPlans && anItem.ProductId == AbcConfig.SITEPLAN_OVERLAY_PHOTOGRAPHY_PRODUCT_ID)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.FloorPlans && contentProductItem.ProductId == AbcConfig.SITEPLAN_OVERLAY_PHOTOGRAPHY_PRODUCT_ID)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasSitePlan()
        {
            var ret = false;
            foreach (var anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.FloorPlans && AbcConfig.SITEPLAN_PRODUCT_IDS.Contains(anItem.ProductId.ToString()))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.FloorPlans && AbcConfig.SITEPLAN_PRODUCT_IDS.Contains(anItem.ProductId.ToString()))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasMudMapOrReDrawFloorplan()
        {
            var ret = false;
            foreach (var anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.FloorPlans && AbcConfig.MUDMAP_REDRAW_PRODUCT_IDS.Contains(anItem.ProductId.ToString()))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.FloorPlans && AbcConfig.MUDMAP_REDRAW_PRODUCT_IDS.Contains(contentProductItem.ProductId.ToString()))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasVirtualWalkThrough()
        {
            var ret = false;
            foreach (var anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Photography && anItem.ProductId == 16394)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Photography && contentProductItem.ProductId == 16394)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasProfessionalSlideshowVideo()
        {
            var ret = false;
            foreach (var anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Photography && (anItem.ProductId == 16393 || anItem.ProductId == 16526))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Photography && (contentProductItem.ProductId == 16393 || contentProductItem.ProductId == 16526))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasAugmentPhotographyProduct()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.Photography && anItem.ProductId == AbcConfig.AUGMENT_PHOTOGRAPHY_PRODUCT_ID)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.Photography && contentProductItem.ProductId == AbcConfig.AUGMENT_PHOTOGRAPHY_PRODUCT_ID)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasA3WindowCard()
        {
            bool ret = false;
            foreach (CartItem anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.WindowCard && (anItem.ProductId == 1208 || anItem.ProductId == 1207))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public bool OrderHasBoardInstallationExtension()
        {
            var ret = false;
            foreach (var anItem in Cart)
            {
                if (anItem.ProductId == 12175 || anItem.ProductId == 12176)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.ProductId == 12175 || contentProductItem.ProductId == 12176)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasVirtualTourExtension()
        {
            var ret = false;
            foreach (var anItem in Cart)
            {
                if (anItem.ProductId == 16405)
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.ProductId == 16405)
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }

        public bool OrderHasEnduroReSkinProduct()
        {
            var ret = false;
            foreach (var anItem in Cart)
            {
                if (anItem.TypeId == ProductTypes.ForPrinting && (anItem.ProductId == ProductSettings.EnduroAframereskinpair || anItem.ProductId == ProductSettings.AFrameReskin600x600Pair || anItem.ProductId == ProductSettings.AFrameReskin900x600Pair))
                {
                    ret = true;
                    break;
                }
                else if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    foreach (PackageGroup item in anItem.PackageGroups)
                    {
                        foreach (PackageContentProduct contentProductItem in item.Products)
                        {
                            if (contentProductItem.TypeId == ProductTypes.ForPrinting && (contentProductItem.ProductId == ProductSettings.EnduroAframereskinpair || contentProductItem.ProductId == ProductSettings.AFrameReskin600x600Pair || contentProductItem.ProductId == ProductSettings.AFrameReskin900x600Pair))
                            {
                                ret = true;
                                break;
                            }

                        }
                        if (ret == true)
                            break;
                    }
                }
            }
            return ret;
        }
    }
}
