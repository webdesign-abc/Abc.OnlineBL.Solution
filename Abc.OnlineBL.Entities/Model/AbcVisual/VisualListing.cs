namespace Abc.OnlineBL.Entities.Model.AbcVisual
{
    using System.Runtime.Serialization;
    using System;

    [DataContract]
    public class VisualListing
    {
        [DataMember]
        public int ListingID { get; set; }

		  [DataMember]
		  public int ClientId { get; set; }

        [DataMember]
        public int ListingTypeID { get; set; }

        [DataMember]
        public string ListingTypeName { get; set; }

        [DataMember]
        public int PropertyTypeID { get; set; }

        [DataMember]
        public string PropertyTypeName { get; set; }

        [DataMember]
        public int StatusID { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Heading { get; set; }

        [DataMember]
        public string PropertyAddress { get; set; }

        [DataMember]
        public string MainImage { get; set; }

        [DataMember]
        public bool IsVisual { get; set; }

        [DataMember]
        public DateTime  DateReceived { get; set; }

        public VisualListing(Abc.OnlineBL.Entities.AR_Listing ar_Listing)
		{
            this.ListingID = ar_Listing.ListingId;
				this.ClientId = ar_Listing.ClientId;
            this.ListingTypeID = ar_Listing.AR_ListingType.ListingType;
            this.ListingTypeName = ar_Listing.AR_ListingType.ListingTypeName;
            this.PropertyTypeID = ar_Listing.AR_Property.PType;
            this.PropertyTypeName = ar_Listing.AR_Property.AR_PropertyType.PTypeName;
            this.StatusID = ar_Listing.AR_Status.StatusId;
            this.Status = ar_Listing.AR_Status.Status;
            this.Location = ar_Listing.Location.Location1;
            this.Heading = ar_Listing.Heading;
            this.PropertyAddress = ar_Listing.PropertyAddress;
            this.MainImage = ar_Listing.MainImage != null ? ar_Listing.MainImage.Substring(0,ar_Listing.MainImage.LastIndexOf('.')) : null;
            this.DateReceived = ar_Listing.DateReceived;
		}        
    }
}
