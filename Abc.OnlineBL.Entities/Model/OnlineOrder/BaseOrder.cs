using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public abstract class BaseOrder
	{
		protected int clientId;
		protected DateTime cartCreatedOn;
		protected string contactName;
		protected string contactNumber;
		protected string clientRefNumber;
		protected string sendProofBy;
		protected string sendProofTo;
		protected string notes;
		protected List<CartItem> cart;

		public BaseOrder()
		{
			this.cart = new List<CartItem>();
		}

		[DataMember]
		public int ClientId
		{
			get { return clientId; }
			set { clientId = value; }
		}

		[DataMember]
		public DateTime CartCreatedOn
		{
			get { return cartCreatedOn; }
			set { cartCreatedOn = value; }
		}

		[DataMember]
		public string ContactName
		{
			get { return contactName; }
			set { contactName = value; }
		}

		[DataMember]
		public string ContactDetailName { get; set; }

		[DataMember]
		public string ContactNumber
		{
			get { return contactNumber; }
			set { contactNumber = value; }
		}

		[DataMember]
		public string ClientRefNumber
		{
			get { return clientRefNumber; }
			set { clientRefNumber = value; }
		}

		[DataMember]
		public string SendProofBy
		{
			get { return sendProofBy; }
			set { sendProofBy = value; }
		}

		[DataMember]
		public string SendProofTo
		{
			get { return sendProofTo; }
			set { sendProofTo = value; }
		}

		[DataMember]
		public string Notes
		{
			get { return notes; }
			set { notes = value; }
		}

		[DataMember]
		public List<CartItem> Cart
		{
			get { return cart; }
			set { cart = value; }
		}

		public int GetNextCartId()
		{
			if (this.cart.Count == 0)
				return 1;
			else
			{
				var id = (from cc in this.cart
						  select cc.Id).Max() + 1;
				return id;
			}
		}

		public void RemoveItemByProductType(int productType)
		{
			List<CartItem> itemIds = new List<CartItem>();

			foreach (CartItem item in Cart)
			{
				if (item.TypeId == productType)
				{
					itemIds.Add(item);
				}
			}

			foreach (CartItem item in itemIds)
			{
				Cart.Remove(item);
			}
		}
	}
}
