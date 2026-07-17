using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	/// <summary>
	/// Class to represent a event call parameter to OrderTracking Workflow Service
	/// </summary>
	[DataContract]
	[Serializable]
	public class OrderTrackingEventParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrderTrackingEventParameter"/> class.
		/// </summary>
		public OrderTrackingEventParameter()
		{
			this.SelectedOrderDetailIds = new List<int>();
			this.SelectedODPackageContentIds = new List<int>();
		}

		/// <summary>
		/// Gets or sets the order id.
		/// </summary>
		/// <value>The order id.</value>
		[DataMember]
		public int OrderId { get; set; }

		[DataMember]
		public int? OrderDetailsId { get; set; }

		[DataMember]
		public int? ODPackageContentId { get; set; }

		/// <summary>
		/// Gets or sets the Message
		/// </summary>
		[DataMember]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the person or the app making the changes.
		/// </summary>
		[DataMember]
		public string LoggedBy { get; set; }

		/// <summary>
		/// Gets or sets the selected order detail ids.
		/// </summary>
		/// <value>The selected order detail ids.</value>
		[DataMember]
		public List<int> SelectedOrderDetailIds { get; set; }

		/// <summary>
		/// Gets or sets the selected OD package content ids.
		/// </summary>
		/// <value>The selected OD package content ids.</value>
		[DataMember]
		public List<int> SelectedODPackageContentIds { get; set; }

		[DataMember]
		public bool AllItemsSelected { get; set; }
	}
}
