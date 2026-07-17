namespace Abc.OnlineBL.Entities.Model.AbcVisual
{
    using System.Runtime.Serialization;

    public class RentedAsset
    {
        [DataMember]
        public int RentalID { get; set; }

        [DataMember]
        public int AssetID { get; set; }

        [DataMember]
        public string FriendlyName { get; set; }

        [DataMember]
        public int ListingDisplayOptionID { get; set; }
    }
}
