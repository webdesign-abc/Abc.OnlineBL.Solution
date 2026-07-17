using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	/// <summary>
	/// Class to represent a event call parameter to OrderTracking Workflow Service
	/// </summary>
	[DataContract]
	[Serializable]
	public class ResponseModel
	{
		/// <summary>
		/// Gets or sets the ResStatus.
		/// </summary>
		/// <value>The order id.</value>
		[DataMember]
        public int ResStatus { get; set; }


		/// <summary>
		/// Gets or sets the Message
		/// </summary>
		[DataMember]
		public string Message { get; set; }
	}
}
