using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.Photography
{
    [DataContract]
    [Serializable]
    public class FloorPlan
    {
        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public int PhotographerId { get; set; }

        [DataMember]
        public int? FloorPlanId { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string PostCode { get; set; }

        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public DateTime? UpdatedOn { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public bool Urgent { get; set; }

        [DataMember]
        public string Images { get; set; }

    }
}