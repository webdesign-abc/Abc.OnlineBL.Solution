using System;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
    [Serializable]
    [DataContract]
    public class InvoiceDetail
    {
        /// <summary>
        /// Gets or sets the ClientId
        /// </summary>
        /// <value>The ClientId.</value>
        [DataMember]
        public int ClientId
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the InvoiceID
        /// </summary>
        /// <value>The InvoiceID.</value>
        [DataMember]
        public int InvoiceID
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the property address.
        /// </summary>
        /// <value>
        /// The property address.
        /// </value>
        [DataMember]
        public string PropertyAddress
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the OrderID
        /// </summary>
        /// <value>The OrderID.</value>
        [DataMember]
        public int OrderID
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the DateInvoiced
        /// </summary>
        /// <value>The DateInvoiced.</value>
        [DataMember]
        public DateTime? DateInvoiced
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the LocationId
        /// </summary>
        /// <value>The LocationId.</value>
        [DataMember]
        public int LocationId
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [DataMember]
        public string Location
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the AmountDue
        /// </summary>
        /// <value>The AmountDue.</value>
        [DataMember]
        public decimal AmountDue
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the BillTo
        /// </summary>
        /// <value>The BillTo.</value>
        [DataMember]
        public DateTime BillTo
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the ManagerAcc
        /// </summary>
        /// <value>
        ///   <c>true</c> if [manager acc]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool ManagerAcc
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the property address.
        /// </summary>
        /// <value>
        /// The property address.
        /// </value>
        public string PropertyAdd
        {
            get
            {
                return string.Format("{0}/{1}{2}", (PropertyAddress == null || string.IsNullOrEmpty(PropertyAddress)) ? "-" : PropertyAddress, Location, string.IsNullOrEmpty(PurchaseOrder) ? PurchaseOrder : string.Format("({0})", PurchaseOrder));
            }
        }

        [DataMember]
        public string PurchaseOrder
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Qty(this is for PaygClients).
        /// </summary>
        /// <value>
        /// The property address.
        /// </value>
        [DataMember]
        public string Qty
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the ProductName(this is for PaygClients).
        /// </summary>
        /// <value>
        /// The property address.
        /// </value>
        [DataMember]
        public string ProductName
        {
            set;
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetail"/> class.
        /// </summary>
        public InvoiceDetail()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetail"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public InvoiceDetail(Order order)
        {
            var purchaseOrder = order.OrderOtherInfo == null ? string.Empty : order.OrderOtherInfo.PurchaseOrder;
            this.ClientId = order.ClientID;
            this.OrderID = order.OrderID;
            this.PropertyAddress = order.PropertyAddress;
            this.Location = order.Location.Location1;
            this.DateInvoiced = order.Invoice.DateInvoiced;
            this.AmountDue = order.Invoice.AmountDue;
            this.PurchaseOrder = purchaseOrder;
            this.Qty = string.Empty;
            this.ProductName = string.Empty;
           
        }
    }
}
