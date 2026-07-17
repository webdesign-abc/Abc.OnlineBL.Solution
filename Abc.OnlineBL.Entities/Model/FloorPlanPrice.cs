using System;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
    [DataContract]
    public class FloorPlanPrice
    {
        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        /// <value>
        /// The sub total.
        /// </value>
        [DataMember]
        public Decimal? SubTotal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GST.
        /// </summary>
        /// <value>
        /// The GST.
        /// </value>
        [DataMember]
        public Decimal? Gst
        {
            get;
            set;
        }
    }
}
