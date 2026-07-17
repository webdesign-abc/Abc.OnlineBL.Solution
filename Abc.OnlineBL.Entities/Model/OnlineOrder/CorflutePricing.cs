using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
    [DataContract]
    [Serializable]
    public class CorflutePricing
    {
        [DataMember]
        public int MaxQty { get; set; }

        [DataMember]
        public decimal ItemPrice { get; set; }
    }
}
