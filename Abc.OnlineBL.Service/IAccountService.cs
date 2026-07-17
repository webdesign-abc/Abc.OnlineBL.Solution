using System.Collections.Generic;
using System.ServiceModel;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// IAccountService
	/// </summary>
	[ServiceContract]
	public interface IAccountService
	{
		/// <summary>
		/// Says the hello.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		[OperationContract]
        string SayHello(string name);

		/// <summary>
		/// Gets the comm order
		/// </summary>
		/// <param name="cusId">The cus id.</param>
		/// <param name="invID">The inv ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		CommOrder GetCommOrder(int cusId, int invID, List<EntityRelations> loadOptions);              

        /// <summary>
        /// Inserts the online payment.
        /// </summary>
        /// <param name="onlinePayments">The online payments.</param>
        /// <returns></returns>
        [OperationContract]
        OnlinePayment InsertOnlinePayment(OnlinePayment onlinePayments);

		/// <summary>
		/// Determines whether [is payment already made] [the specified online payment ID].
		/// </summary>
		/// <param name="onlinePaymentID">The online payment ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is payment already made] [the specified online payment ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsPaymentAlreadyMade(int onlinePaymentID);

		/// <summary>
		/// Gets the online payment details.
		/// </summary>
		/// <param name="onlinePaymentID">The online payment ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<OnlinePaymentDetail> GetOnlinePaymentDetails(int onlinePaymentID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Process the CommOrder payment.
		/// </summary>
		/// <param name="onlinePayment">The online payment.</param>
		/// <returns></returns>
		[OperationContract]
		string ProcessCommOrderPayment(OnlinePayment onlinePayment);

		/// <summary>
		/// Gets the customer details.
		/// </summary>
		/// <param name="onlinePaymentID">The online payment ID.</param>
		/// <returns></returns>
		[OperationContract]
		CustomerPaymentDetails GetCustomerDetails(int onlinePaymentID);

		/// <summary>
		/// Gets the online payment.
		/// </summary>
		/// <param name="onlinePaymentID">The online payment ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		OnlinePayment GetOnlinePayment(int onlinePaymentID, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the amount due order invoice.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <param name="orderID">The order ID.</param>
        /// <param name="address">The address.</param>
        /// <returns>
        /// Returns A list of the Invoice-Detail Objects.
        /// </returns>
        [OperationContract]
        List<Order> GetAmountDueOrderInvoice(int clientID, int orderID, string address);

        /// <summary>
        /// Gets the amount due order invoice list.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <param name="orderID">The order ID.</param>
        /// <param name="address">The address.</param>
        /// <returns>Returns the list of the InvoiceDetail</returns>
        [OperationContract]
        List<InvoiceDetail> GetAmountDueOrderInvoiceList(int clientID, int orderID, string address);

        /// <summary>
        /// Gets the payment.
        /// </summary>
        /// <param name="onlinePaymentID">The online payment ID.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        [OperationContract]
        Payment GetPayment(int onlinePaymentID, List<EntityRelations> loadOptions);

        /// <summary>
        /// Processes the comm order payment.
        /// </summary>
        /// <param name="onlinePayment">The online payment.</param>
        /// <returns>Returns Payment Object.</returns>
        [OperationContract]
        Payment ProcessOnlinePayments(OnlinePayment onlinePayment);

        /// <summary>
        /// Verifies the invoice numbers.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="orderIdList">The order id list.</param>
        /// <returns>
        /// Returns True for valid, Otherwise False.
        /// </returns>
        [OperationContract]
        bool VerifyInvoiceNumbers(int clientId, List<int> orderIdList);

		/// <summary>
		/// Inserts the comm online payment.
		/// </summary>
		/// <param name="onlinePayment">The online payment.</param>
		/// <returns></returns>
		[OperationContract]
		int InsertCommOnlinePayment(OnlinePayment onlinePayment);

        /// <summary>
        /// Inserts the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <returns>Returns the payment.</returns>
        [OperationContract]
        Payment InsertPayment(Payment payment);


        /// <summary>
        /// Inserts the online express payment.
        /// </summary>
        /// <param name="objPaymentExpress">The payment.</param>
        /// <returns>Returns the Express payment.</returns>
        [OperationContract]
        OnlinePaymentExpressOrder InsertOnlinePaymentExpressOrder(OnlinePaymentExpressOrder objPaymentExpress);

        /// <summary>
        /// Get Online Payment Express Order by Id.
        /// </summary>
        /// <param name="TempOrderDetailsId">The payment Id.</param>
        /// <returns>Returns the payment.</returns>
        [OperationContract]
        OnlinePaymentExpressOrder GetOnlinePaymentExpressOrder(int TempOrderDetailsId);


	}
}
