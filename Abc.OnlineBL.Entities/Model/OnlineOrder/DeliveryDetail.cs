using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
    [Serializable]
    [DataContract]
    public class DeliveryDetail
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int LocationId { get; set; }
        [DataMember]
        public string Suburb { get; set; }
        [DataMember]
        public string PostCode { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string StreetAddress { get; set; }
        [DataMember]
        public string AlternateContactName { get; set; }
        [DataMember]
        public string ContactEmail { get; set; }
        [DataMember]
        public string AlternateContactEmail { get; set; }
    }
}
