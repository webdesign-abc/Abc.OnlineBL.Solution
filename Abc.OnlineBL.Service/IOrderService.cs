using System;
using System.Collections.Generic;
using System.ServiceModel;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OrderFile;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Entities.Model.WebClient;

namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// IOrderService interface
	/// </summary>
	[ServiceContract]
	public interface IOrderService
	{
		/// <summary>
		/// SayHello returns what you pass to it just to let you know that it is listening to you.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[OperationContract]
		string SayHello(string name);

		/// <summary>
		/// Gets the order by id.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <param name="loadOptions">The load options. Pass null to avoid deep loading</param>
		/// <returns></returns>
		[OperationContract]
		Order GetOrderById(int orderId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the order detail by id.
		/// </summary>
		/// <param name="orderDetailId">The order detail id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		OrderDetail GetOrderDetailById(int orderDetailId, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>
        /// Returns A list Of JobSearch Obj.
        /// </returns>
        [OperationContract]
		List<JobSearch> GetJobs(int clientId);

		/// <summary>
		/// Batches the request removal.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="jobs">The jobs.</param>
		/// <param name="reqBy">The req by.</param>
		/// <param name="removalType">Type of the removal.</param>
		/// <param name="removalDate">The removal date.</param>
		/// <returns></returns>
        [OperationContract]
        string BatchRequestRemoval(int clientId, List<int> jobs, string reqBy, int? removalType, DateTime removalDate);

		/// <summary>
		/// Gets the workflow alerts.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <param name="orderDetailsId">The order details id.</param>
		/// <param name="odPackageContentId">The od package content id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<WorkflowAlert> GetWorkflowAlerts(int? orderId, int? orderDetailsId, int? odPackageContentId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Clears the workflow alert.
		/// </summary>
		/// <param name="alertId">The alert id.</param>
		/// <param name="handleBy">The handle by.</param>
		[OperationContract]
		void ClearWorkflowAlert(int alertId, string handleBy);

		/// <summary>
		/// Updates the order status.
		/// </summary>
		/// <param name="orderTrackingEventParameter">The order tracking event parameter.</param>
		/// <param name="nextState">State of the next.</param>
		/// <param name="alsoROnlineBLeAlert">if set to <c>true</c> [also rOnlineBLe alert].</param>
		[OperationContract]
		void UpdateOrderStatus(OrderTrackingEventParameter orderTrackingEventParameter, WorkflowStates nextState, bool alsoROnlineBLeAlert);

		/// <summary>
		/// Updates the order items status.
		/// </summary>
		/// <param name="orderTrackingEventParameter">The order tracking event parameter.</param>
		/// <param name="nextState">State of the next.</param>
		[OperationContract]
		void UpdateOrderItemsStatus(OrderTrackingEventParameter orderTrackingEventParameter, WorkflowStates nextState);

		/// <summary>
		/// Workflows the log history.
		/// </summary>
		/// <param name="trackingParameter">The tracking parameter.</param>
		/// <param name="outgoingStateName">Name of the outgoing state.</param>
		[OperationContract]
		void WorkflowLogHistory(OrderTrackingEventParameter trackingParameter, string outgoingStateName);

		/// <summary>
		/// Workflows the rollback status.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="idType">Type of the id.</param>
		[OperationContract]
		void WorkflowRollbackStatus(int id, string idType);

		/// <summary>
		/// Despatches the products.
		/// </summary>
		/// <param name="orderTrackingEventParameter">The order tracking event parameter.</param>
		/// <param name="productTypeId">The product type id.</param>
		[OperationContract]
		void DespatchProducts(OrderTrackingEventParameter orderTrackingEventParameter, int productTypeId);

		/// <summary>
		/// Despatches the whole order.
		/// </summary>
		/// <param name="orderTrackingEventParameter">The order tracking event parameter.</param>
		[OperationContract]
		void DespatchWholeOrder(OrderTrackingEventParameter orderTrackingEventParameter);

		/// <summary>
		/// Approves the products.
		/// </summary>
		/// <param name="orderTrackingEventParameter">The order tracking event parameter.</param>
		/// <param name="productTypeId">The product type id.</param>
		[OperationContract]
		void ApproveProducts(OrderTrackingEventParameter orderTrackingEventParameter, int productTypeId);

		/// <summary>
		/// Approves the whole order.
		/// </summary>
		/// <param name="orderTrackingEventParameter">The order tracking event parameter.</param>
		[OperationContract]
		void ApproveWholeOrder(OrderTrackingEventParameter orderTrackingEventParameter);

		/// <summary>
		/// Uploads a photo for an Order or agent photo.
		/// </summary>
		/// <remarks>
		/// This function will check the file quality and transfer the file to appropriate destination
		/// folder. The normal rules are:
		/// If a file is for Agent Photo we check for RED/GRAY/GREEN quality and rename the file as in "OrderID_AGENTPHOTO_ContactName_ContactId.ext"
		/// If a file is for Property Photo we check RED/GRAY/GREEN quality and rename the file as in "OrderID_FileName"
		/// If a file is for Imaging Photo we don't check quality and rename the file similar to above and transfer to Image Dir Destination
		/// If a file is for Graphics we don't check quality and rename teh file similar to above and transfer to Graphcis Dir Destination
		/// </remarks>
		/// <param name="requests">The UploadPhotoRequests.</param>
		/// <param name="orderTracking">The orderTracking.</param>
		/// <returns>a response of the processing of the file</returns>
		[OperationContract]
		List<UploadPhotoResponse> UploadPhoto(List<UploadPhotoRequest> requests, OrderTrackingEventParameter orderTracking);

		/// <summary>
		/// Gets the artist name by order ID.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetArtistNameByOrderID(int orderID);

		/// <summary>
		/// Determines whether [is design now applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		bool IsDesignNowApplicable(int orderID);

		/// <summary>
		/// Determines whether [is AOP design in complete] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is AOP design in complete] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsAOPDesignInComplete(int orderID);

		/// <summary>
		/// Determines whether [is approve job applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is approve job applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsApproveJobApplicable(int orderID);

		/// <summary>
		/// Determines whether [is change request reproof and approve applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is change request reproof and approve applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsChangeRequestReproofAndApproveApplicable(int orderID);

		/// <summary>
		/// Determines whether [is request for removal applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is request for removal applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsRequestForRemovalApplicable(int orderID);

		/// <summary>
		/// Determines whether [is board erection details applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is board erection details applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsBoardErectionDetailsApplicable(int orderID);

		/// <summary>
		/// Determines whether [is board erection applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is board erection applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsBoardErectionApplicable(int orderID);

		/// <summary>
		/// Determines whether [is board removal details applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is board removal details applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsBoardRemovalDetailsApplicable(int orderID);

		/// <summary>
		/// Determines whether [is board removal applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is board removal applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsBoardRemovalApplicable(int orderID);

		/// <summary>
		/// Determines whether [is cancel online design applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is cancel online design applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsCancelOnlineDesignApplicable(int orderID);

        /// <summary>
        /// Gets the orders by property address.
        /// </summary>
        /// <param name="submittedPropertyAddress">The submitted property address.</param>
        /// <returns></returns>
        [OperationContract]
        List<Order> GetOrdersSubsetByPropertyAddress(string submittedPropertyAddress);


        /// <summary>
        /// Gets the order data by GRP start ID.
        /// </summary>
        /// <param name="groupStartId">The group start id.</param>
        /// <param name="groupEndOrderId">The group end order id.</param>
        /// <returns></returns>
        [OperationContract]
        List<Order> GetOrderSubsetByOrderRange(int groupStartId, int groupEndOrderId);


		/// <summary>
		/// Adds the board erection details.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="eNotes">The e notes.</param>
		/// <param name="prefType">Type of the pref.</param>
		/// <param name="prefDate">The pref date.</param>
		/// <param name="installationFile">The installation file.</param>
		[OperationContract]
		void AddBoardErectionDetails(int orderID, string eNotes, int prefType, DateTime? prefDate, string installationFile);

		/// <summary>
		/// Adds the board removal details.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="rNotes">The r notes.</param>
		/// <param name="prefType">Type of the pref.</param>
		/// <param name="prefDate">The pref date.</param>
		/// <param name="byWhom">The by whom.</param>
		/// <returns></returns>
		[OperationContract]
		string AddBoardRemovalDetails(int orderID, string rNotes, int prefType, DateTime? prefDate, string byWhom);

		/// <summary>
		/// Sends the other comment queries.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="comment">The comment.</param>
		/// <param name="byWhom">The by whom.</param>
		[OperationContract]
		void SendOtherCommentQueries(int orderID, string comment, string byWhom);

		/// <summary>
		/// Cancels the online design job.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="comment">The comment.</param>
		/// <param name="byWhom">The by whom.</param>
		[OperationContract]
		void CancelOnlineDesignJob(int orderID, string comment, string byWhom);

        /// <summary>
        /// Cancels the online design express job.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="byWhom">The by whom.</param>
        [OperationContract]
        void CancelOnlineDesignExpressJob(int orderID, string comment, string byWhom);

		/// <summary>
		/// Approves the job.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="byWhom">The by whom.</param>
		[OperationContract]
		void ApproveJob(int orderID, string byWhom);

		/// <summary>
		/// Requests for removal.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="rNotes">The r notes.</param>
		/// <param name="byWhom">The by whom.</param>
		/// <param name="prefType">Type of the pref.</param>
		/// <param name="prefDate">The pref date.</param>
		/// <returns></returns>
		[OperationContract]
		string RequestBoardRemoval(int orderID, string rNotes, string byWhom, int prefType, DateTime? prefDate);

		/// <summary>
		/// Changes the request.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="notes">The notes.</param>
		/// <param name="byWhom">The by whom.</param>
		/// <param name="reProof">if set to <c>true</c> [re proof].</param>
		[OperationContract]
		void ChangeRequest(int orderID, string notes, string byWhom, bool reProof);

        /// <summary>
        /// Gets the SMS order details by client ID.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <returns>
        /// Returns List of the SmsOrderDetail object.
        /// </returns>
        [OperationContract]
        List<Order> GetSMSOrderDetailsByClientID(int clientID);

        /// <summary>
        /// Gets the SMS order details for A month.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="clientId">The client id.</param>
        /// <returns>
        /// Returns List of the Order object.
        /// </returns>
        [OperationContract]
        List<Order> GetSmsOrderDetailsForAMonth(int year, int month, int clientId);

		/// <summary>
		/// Updates the SMS order details.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="smsUserText">The SMS user text.</param>
		/// <param name="smsAgentMobileNo">The SMS agent mobile no.</param>
		/// <param name="smsNotifyAgent">if set to <c>true</c> [SMS notify agent].</param>
		/// <param name="smsSendEmail">if set to <c>true</c> [SMS send email].</param>
		/// <param name="smsAgentEmailAddress">The SMS agent email address.</param>
		/// <param name="mmsAllowed">if set to <c>true</c> [MMS allowed].</param>
		/// <param name="smsActive">if set to <c>true</c> [SMS active].</param>
		[OperationContract]
		void UpdateSmsOrderDetails(int orderID, string smsUserText, string smsAgentMobileNo, 
						bool smsNotifyAgent, bool smsSendEmail, string smsAgentEmailAddress,
						bool mmsAllowed, bool smsActive);

		/// <summary>
		/// Gets the SMS queue by order id.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<SMS_Queue> GetSMSQueueByOrderId(int orderID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the MMS photo.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="photoBytes">The photo bytes.</param>
		[OperationContract]
		void UpdateMMSPhoto(int orderID, byte[] photoBytes);

		/// <summary>
		/// Gets the type of all ar property.
		/// </summary>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<AR_PropertyType> GetAllArPropertyType(List<EntityRelations> loadOptions);


		/// <summary>
		/// Gets the location id by state and suburb.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="suburb">The suburb.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		Location GetLocationIdByStateAndSuburb(string state, string suburb, List<EntityRelations> loadOptions);

		/// <summary>
		/// Inserts the property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		[OperationContract]
		Property InsertProperty(Property property);

		/// <summary>
		/// Gets the property by id.
		/// </summary>
		/// <param name="propertyID">The property ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		Property GetPropertyById(int propertyID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Saves the order session.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="data">The data.</param>
		[OperationContract]
		void SaveOrderSession(int clientId, string data);

		/// <summary>
		/// Gets the order from session.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <returns></returns>
		[OperationContract]
		string GetOrderFromSession(int clientId);

		/// <summary>
		/// Saves the property order data.
		/// </summary>
		/// <param name="propertyID">The property ID.</param>
		/// <param name="data">The data.</param>
		[OperationContract]
		void SavePropertyOrderData(int propertyID, string data);


		/// <summary>
		/// Gets the online order category.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="propertyRelatedOrder">if set to <c>true</c> [property related order].</param>
		/// <param name="regularOrder">if set to <c>true</c> [regular order].</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<OnlineOrderCategory> GetOnlineOrderCategory(int clientId, bool propertyRelatedOrder, bool regularOrder, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the order XML file.
		/// </summary>
		/// <param name="model">The model.</param>
		[OperationContract]
		void UpdateOrderXMLFile(UploadText model);

        /// <summary>
        /// Removes the non-paying order.
        /// </summary>
        /// <param name="clientID">Enter a client id.</param>
        /// <param name="orderID">Enter an orderID. </param>
        /// <returns>Returns the online order response.</returns>
        [OperationContract]
        Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse RemoveNonPayingOrder(int clientID, int orderID);

		/// <summary>
		/// Processes the order.
		/// </summary>
		/// <param name="propertyOrder">The property order.</param>
		[OperationContract]
		Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessOrder(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder);

		/// <summary>
		/// Gets the proof file list.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetProofFileList(int orderID);

		/// <summary>
		/// Gets the AOP previews.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetAOPPreviews(int orderID);

		/// <summary>
		/// Orders the has.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="typeID">The type ID.</param>
		/// <returns></returns>
		[OperationContract]
		bool OrderHas(int orderID, int typeID);

		/// <summary>
		/// Gets the SB order by id.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		SB_Order GetSBOrderById(int orderId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Determines whether [is SB erection details applicable] [the specified stockboard order ID].
		/// </summary>
		/// <param name="stockboardOrderID">The stockboard order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is SB erection details applicable] [the specified stockboard order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsSBErectionDetailsApplicable(int stockboardOrderID);


		/// <summary>
		/// Adds the stock board erection details.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="name">The name.</param>
		/// <param name="message">The message.</param>
		/// <param name="prefType">Type of the pref.</param>
		/// <param name="prefDate">The pref date.</param>
		[OperationContract]
		void AddStockBoardErectionDetails(int orderID, string name, string message, int prefType, DateTime? prefDate);

		/// <summary>
		/// Determines whether [is stock board removal details applicable] [the specified stockboard order ID].
		/// </summary>
		/// <param name="stockboardOrderID">The stockboard order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is stock board removal details applicable] [the specified stockboard order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsStockBoardRemovalDetailsApplicable(int stockboardOrderID);

		/// <summary>
		/// Adds the stock board removal details.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="name">The name.</param>
		/// <param name="message">The message.</param>
		/// <param name="prefType">Type of the pref.</param>
		/// <param name="prefDate">The pref date.</param>
		[OperationContract]
		void AddStockBoardRemovalDetails(int orderID, string name, string message, int prefType, DateTime? prefDate);

		/// <summary>
		/// Determines whether [is request for stock board removal applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is request for stock board removal applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsRequestForStockBoardRemovalApplicable(int orderID);

		/// <summary>
		/// Requests the stock board removal.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="name">The name.</param>
		/// <param name="message">The message.</param>
		[OperationContract]
		void RequestStockBoardRemoval(int orderID, string name, string message);

		/// <summary>
		/// Gets all locations.
		/// </summary>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Location> GetAllLocations(List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets all locations.
        /// </summary>
        /// <param name="loadOptions">The load options.</param>
        /// /// <param name="term">The Search term or query.</param>
        /// <returns></returns>
        [OperationContract]
        List<Location> GetAllLocationsBySearchTerm(string term, List<EntityRelations> loadOptions);

		/// <summary>
		/// Properties the orders has board or stock board.
		/// </summary>
		/// <param name="propertyID">The property ID.</param>
		/// <returns></returns>
		[OperationContract]
		bool PropertyOrdersHasBoardOrStockBoard(int propertyID);

		/// <summary>
		/// Gets all notes by order ID.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetAllNotesByOrderID(int orderID);

		/// <summary>
		/// Gets the preferered erection type and date.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetPrefereredErectionTypeAndDate(int orderID);

		/// <summary>
		/// Gets the preferered removal type and date.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetPrefereredRemovalTypeAndDate(int orderID);

		/// <summary>
		/// Notifies the upload images.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="uploadPhotoResponses">The upload photo responses.</param>
		[OperationContract]
		void NotifyUploadImages(int orderID, List<UploadPhotoResponse> uploadPhotoResponses);

		/// <summary>
		/// Gets the AR property type by ID.
		/// </summary>
		/// <param name="pTypeID">The p type ID.</param>
		/// <returns></returns>
		[OperationContract]
		AR_PropertyType GetARPropertyTypeByID(int pTypeID);

		/// <summary>
		/// Uploads the single photo.
		/// </summary>
		/// <param name="requests">The requests.</param>
		/// <param name="orderTracking">The order tracking.</param>
		/// <returns></returns>
		[OperationContract]
		UploadPhotoResponse UploadSinglePhoto(UploadPhotoRequest requests, OrderTrackingEventParameter orderTracking);

		/// <summary>
		/// Gets the orders for picking slip.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <returns></returns>
        [OperationContract]
        List<PickingSlipOrderInfo> GetOrdersForPickingSlip(int clientID);

        /// <summary>
        /// Get Orders For Picking Slip
        /// </summary>
        /// <param name="clientId">clientId.</param>
        /// <param name="property">property.</param>
        /// <param name="email">email.</param>
        ///  <param name="link">link.</param>
        /// <returns></returns>
        [OperationContract]
        String EmailPropertyPhotoLink(int clientId, string property, string email, string link);

		/// <summary>
		/// Adds the new picking slip.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="property">The property.</param>
		/// <param name="orderid">The orderid.</param>
		/// <param name="ps">The ps.</param>
		/// <returns></returns>
        [OperationContract]
		String AddNewPickingSlip(int clientId, string property, int orderid, List<PickingSlipModel> ps);
        /// <summary>
        /// Get Orders For Picking Slip
        /// </summary>
        /// <param name="pickingSlipId">pickingSlipId.</param>
        /// <returns></returns>
        [OperationContract]
        NewPickSlipInfo GetNewPickingSlip(int pickingSlipId);

		/// <summary>
		/// Gets the order image requirement.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <returns></returns>
		[OperationContract]
		GetOrderImageRequirementsResult GetOrderImageRequirement(int orderId);

        /// <summary>
        /// Gets the install file by order id.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>
        /// Returns the full fileName.
        /// </returns>
        [OperationContract]
        string GetInstallFileByOrderId(int orderId);

		/// <summary>
		/// Inserts the in design print queue.
		/// </summary>
		/// <param name="printQueue">The print queue.</param>
		/// <returns></returns>
		[OperationContract]
		RND_InDesignPrintQueue InsertInDesignPrintQueue(RND_InDesignPrintQueue printQueue);

		/// <summary>
		/// Uploads the single artwork.
		/// </summary>
		/// <param name="requests">The requests.</param>
		/// <param name="orderTracking">The order tracking.</param>
		/// <returns></returns>
		[OperationContract]
		UploadPhotoResponse UploadSingleArtwork(UploadPhotoRequest requests, OrderTrackingEventParameter orderTracking);

		/// <summary>
		/// Notifies the upload artworks.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="uploadPhotoResponses">The upload photo responses.</param>
		[OperationContract]
		void NotifyUploadArtworks(int orderID, List<UploadPhotoResponse> uploadPhotoResponses);

		/// <summary>
		/// Gets the stock board preferered erection type and date.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetStockBoardPrefereredErectionTypeAndDate(int orderID);

		/// <summary>
		/// Gets the stock board preferered removal type and date.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetStockBoardPrefereredRemovalTypeAndDate(int orderID);

		/// <summary>
		/// Properties the SB orders has board or stock board.
		/// </summary>
		/// <param name="propertyID">The property ID.</param>
		/// <returns></returns>
		[OperationContract]
		bool PropertySBOrdersHasBoardOrStockBoard(int propertyID);

		/// <summary>
		/// SBs the order has.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="typeID">The type ID.</param>
		/// <returns></returns>
		[OperationContract]
		bool SBOrderHas(int orderID, int typeID);

		/// <summary>
		/// Adds the overlay installation file.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="fileName">Name of the file.</param>
		[OperationContract]
		void AddOverlayInstallationFile(int orderID, string fileName);

		/// <summary>
		/// Gets the products size code from order.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="productType">Type of the product.</param>
		/// <returns></returns>
		[OperationContract]
		List<string> GetProductsSizeCodeFromOrder(int orderID, int productType);

		/// <summary>
		/// Gets the related orders by property id.
		/// </summary>
		/// <param name="propertyId">The property id.</param>
		/// <returns></returns>
		[OperationContract]
		List<int> GetRelatedOrdersByPropertyId(int propertyId);

        /// <summary>
		/// Modifies the DIY Template .
		/// </summary>
		/// <param name="orderID">The order ID.</param>
        /// <param name="propertyOrder">The property order</param>
		/// <returns></returns>
        [OperationContract]
        OnlineOrderResponse ModifyDIYTemplate(int orderID, OnlinePropertyOrder propertyOrder);

		/// <summary>
		/// Modifies the DIY order.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="clientID">The client ID.</param>
		/// <param name="propertyOrder">The property order.</param>
		/// <param name="managerID">The manager ID.</param>
		/// <param name="isWorkshop">if set to <c>true</c> [is workshop].</param>
		/// <returns></returns>
		[OperationContract]
		Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ModifyDIYOrder(int orderID, int clientID, Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, string managerID, bool isWorkshop);

		/// <summary>
		/// Gets the DIY order.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		OnlinePropertyOrder GetDIYOrder(int orderID);

		/// <summary>
		/// Sets the order printed on field in DespatchDetails table
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <param name="printedOn">The printed on.</param>
		[OperationContract]
		void SetOrderPrintedOn(int orderId, DateTime? printedOn);

        /// <summary>
        /// Gets Order Data
        /// </summary>
        /// <param name="orderID">The Order ID.</param>
        [OperationContract]
        string GetOrderData(int orderID);

        /// <summary>
        /// Requests the template changes.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request by.</param>
        /// <param name="requestNumber">The request number.</param>
        /// <param name="reason">The reason.</param>
        [OperationContract]
        void RequestTemplateChanges(int orderID, string requestBy, string requestNumber, string reason);

        /// <summary>
        /// Requests the template changes.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request by.</param>
        /// <param name="requestNumber">The request number.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="requestFile">The requestFile.</param>
        [OperationContract]
        void RequestTemplateChangesUpload(int orderID, string requestBy, string requestNumber, string reason, string requestFile);

        /// <summary>
        /// Approve Job By Artist.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="artistID">The artist ID.</param>
        [OperationContract]
        void ApproveJobByArtist(int orderID, int artistID);

        /// <summary>
        /// Requests the incorrect pack changes.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request by.</param>
        /// <param name="requestNumber">The request number.</param>
        /// <param name="reason">The reason.</param>
        [OperationContract]
        void RequestIncorrectPackChanges(int orderID, string requestBy, string requestNumber, string reason);

        /// <summary>
        /// Update B2B Order.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        void UpdateB2BOrder(int orderID);

        /// <summary>
        /// Update B2B Order Caption.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="caption">The caption.</param>
        [OperationContract]
        void UpdateB2BOrderCaption(int orderID, string caption);

        /// <summary>
        /// Insert B2B Print Queue.
        /// </summary>
        /// <param name="abcB2BPrintQueue">The abcB2BPrintQueue.</param>
        [OperationContract]
        void InsertAbcB2BPrintQueue(AbcB2BPrintQueue abcB2BPrintQueue);

        /// <summary>
        /// Update B2B Print Queue.
        /// </summary>
        /// <param name="abcB2BPrintQueue">The AbcB2BPrintQueue.</param>
        [OperationContract]
        void UpdateAbcB2BPrintQueue(AbcB2BPrintQueue abcB2BPrintQueue);

        /// <summary>
        /// Get B2B Print Queue.
        /// </summary>
        [OperationContract]
        AbcB2BPrintQueue GetAbcB2BPrintQueue();

        /// <summary>
        /// Get B2B Product.
        /// </summary>
        /// <param name="productID">The product ID.</param>
        [OperationContract]
        Product GetB2BProduct(int productID);

        /// <summary>
        /// Approves the express job.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="byWhom">The by whom.</param>
        /// <param name="purchaseOrderNumber">The purchase Order Number.</param>
        [OperationContract]
        void ApproveExpressJob(int orderID, string byWhom, string purchaseOrderNumber);

        /// <summary>
        /// Approves the express DIY job.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="byWhom">The by whom.</param>
        /// <param name="approveAll">The approve all.</param>
        /// <param name="approveBoard">The approve board.</param>
        /// <param name="approveBrochure">The approve brochure.</param>
        /// <param name="approveOther">The approve other.</param>
        /// <param name="purchaseOrderNumber">The purchase Order Number.</param>
        /// <param name="jobDocumentIds">The job Document Ids.</param>
        [OperationContract]
        void ApproveExpressDIYJob(int orderID, string byWhom, DateTime? approveAll, DateTime? approveBoard, DateTime? approveBrochure, DateTime? approveOther, string purchaseOrderNumber, List<int> jobDocumentIds);

        /// <summary>
        /// Make Order As Conjunction Offer.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="comment">The reason</param>
        /// <param name="byWhom">The by whom.</param>
        /// <param name="contactNumber">The contact number of requester.</param>
        /// <param name="filePath">The sample file from Conjuctional agents.</param>
        [OperationContract]
        void MakeOrderAsConjunctionOffer(int orderID, string comment, string byWhom, string contactNumber, string filePath);

        /// <summary>
        /// Approves the express SB DIY job.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="byWhom">The by whom.</param>
        /// <param name="approveBoard">The approve board.</param>
        /// <param name="purchaseOrderNumber">The purchase Order Number.</param>
        /// <param name="jobDocumentIds">The job Document Ids.</param>
        [OperationContract]
        void ApproveExpressBoardDIYJob(int orderID, string byWhom, DateTime? approveBoard, string purchaseOrderNumber, List<int> jobDocumentIds);

        /// <summary>
        /// Order Has Stockboard.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        bool OrderHasStockboard(int orderID);

        /// <summary>
        /// Has All B2B Print Queue Layup Success.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        bool HasAllB2BPrintQueueLayupSuccess(int orderID);

        /// <summary>
        /// Get Service Availability.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="suburb">The suburb.</param>
        [OperationContract]
        ServiceAvailability GetServiceAvailability(string state, string suburb);

        /// <summary>
        /// Send Service Availability Option.
        /// </summary>
        /// <param name="rpRequest">The Regional Property Request.</param>
        [OperationContract]
        void SendServiceAvailabilityOption(RegionalPropertyRequest rpRequest);

        /// <summary>
        /// Add Service Queue.
        /// </summary>
        /// <param name="reportName">The reportName.</param>
        /// <param name="reportParameter">The reportParameter.</param>
        /// <param name="emailAddress">The emailAddress.</param>
        /// <param name="emailCCAddress">The emailCCAddress.</param>
        /// <param name="emailBCCAddress">The emailBCCAddress.</param>
        /// <param name="source">The source.</param>
        [OperationContract]
        void AddServiceQueue(string reportName, string reportParameter, string emailAddress, string emailCCAddress, string emailBCCAddress, string source);

        /// <summary>
        /// Get Proof Image Files.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        List<string> GetProofImageFiles(int orderID);

        /// <summary>
        /// Process Board Extension Order.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request By.</param>
        /// <param name="extraNotes">The extra Notes.</param>
        [OperationContract]
        Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessBoardExtensionOrder(int orderID, string requestBy, string extraNotes);

        /// <summary>
        /// Order Applicable For Twelve Months Removal Or Extension.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        bool OrderApplicableForTwelveMonthsRemovalOrExtension(int orderID);

        /// <summary>
        /// Add B2B Instalation Files.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="installationFiles">The installation Files.</param>
        [OperationContract]
        void AddB2BInstalationFiles(int orderID, string installationFiles);

        /// <summary>
        /// Send DIY Proof By Email.
        /// </summary>
        /// <param name="clientID">The clientID.</param>
        /// <param name="orderID">The orderID.</param>
        /// <param name="pAddress">The pAddress.</param>
        /// <param name="emailAddress">The emailAddress.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="source">The source.</param>
        [OperationContract]
        void SendDIYProofByEmail(int clientID, int orderID, string pAddress, string emailAddress, string attachments, string source);

        /// <summary>
        /// Request One Off Design.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request by.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="requestFile">The requestFile.</param>
        [OperationContract]
        void RequestOneOffDesign(int orderID, string requestBy, string reason, string requestFile);

        /// <summary>
        /// Request Text Overflow.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request by.</param>
        /// <param name="requestFile">The requestFile.</param>
        [OperationContract]
        void RequestTextOverflow(int orderID, string requestBy, string requestFile);

        /// <summary>
        /// Request Image Cropping.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request by.</param>
        /// <param name="requestFile">The requestFile.</param>
        [OperationContract]
        void RequestImageCropping(int orderID, string requestBy, string requestFile);

        /// <summary>
        /// Get Order Status For Approval.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        ResponseModel GetOrderStatusForApproval(int orderID);

        /// <summary>
        /// Processes the pay G order.
        /// </summary>
        /// <param name="propertyOrder">The property order.</param>
        /// <param name="TemporderdetailsId">The paygorderdetailsId.</param>
        /// <param name="ClientId">The ClientId .</param>
        /// <param name="AccountId">The AccountId.</param>
        /// <param name="PaymentResponse">The PaymentResponse.</param>
        /// <param name="CardType">The CardType.</param>
        [OperationContract]
        Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessOnlinePaymentExpressOrder(Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder, int TemporderdetailsId, int ClientId, int AccountId, string PaymentResponse, string CardType);

        /// <summary>
        /// Photo Order Applicable For Ninety Days Hosting Removal Or Extension.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        bool PhotoOrderApplicableForNinetyDaysHostingRemovalOrExtension(int orderID);

        /// <summary>
        /// Process Virtual Walk Through Extension Order.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        /// <param name="requestBy">The request By.</param>
        /// <param name="extraNotes">The extra Notes.</param>
        [OperationContract]
        Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessVirtualWalkThroughExtensionOrder(int orderID, string requestBy, string extraNotes);
    }
}
