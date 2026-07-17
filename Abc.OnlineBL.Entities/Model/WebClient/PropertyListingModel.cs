using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.WebClient
{
    [DataContract]
    [Serializable]
    public class PropertyListingModel
    {
        [DataMember]
        public int ClientID { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string Office { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string BulkUploader { get; set; }

        [DataMember]
        public string RequestedBy { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}
