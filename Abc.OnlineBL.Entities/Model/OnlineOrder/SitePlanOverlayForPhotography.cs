using System;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
    [Serializable]
    [DataContract]
    public class SitePlanOverlayForPhotography
    {
        public SitePlanOverlayForPhotography() { }

        [DataMember]
        public decimal FrontageInMeter { get; set; }

        [DataMember]
        public decimal LeftBorderInMeter { get; set; }


        [DataMember]
        public decimal RightBorderInMeter { get; set; }


        [DataMember]
        public decimal RearBorderInMeter { get; set; }


        [DataMember]
        public decimal ApproxSqMeters { get; set; }
    }
}
