using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Abc.OnlineBL.Entities;
using System.Data.Linq;
using System.Runtime.Serialization;
using System.Data;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.WebClient;
using Abc.OnlineBL.Entities.Model.Marketing;
using Abc.OnlineBL.Entities.Model.AbcVisual;

namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// IClientService defines interfaces to deal with Client related entities/tables
	/// </summary>
	[ServiceContract]
	public interface IClientService
	{
		/// <summary>
		/// SayHello returns what you pass to it just to let you know that it is listening to you.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[OperationContract]
		string SayHello(string name);

		/// <summary>
		/// Gets the client by id.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		Client GetClientById(int clientId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the client by user id.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns>Client Entity</returns>
		[OperationContract]
		Client GetClientByUserId(string userId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the clients by name.
		/// </summary>
		/// <param name="clientName">Name of the client.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Client> GetClientsByName(string clientName, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the clients pref.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="prefId">The pref id.</param>
		/// <returns></returns>
		[OperationContract]
		ClientsPref GetClientsPref(int clientId, int prefId);

		/// <summary>
		/// Updates the client pref.
		/// </summary>
		/// <param name="clientsPref">The clients pref.</param>
		[OperationContract]
		void UpdateClientsPref(ClientsPref clientsPref);

		/// <summary>
		/// Inserts the clients pref.
		/// </summary>
		/// <param name="clientsPref">The clients pref.</param>
		/// <returns></returns>
		[OperationContract]
		ClientsPref InsertClientsPref(ClientsPref clientsPref);

		/// <summary>
		/// Authenticates the client.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="password">The password.</param>
		/// <returns>true if successful</returns>
		[OperationContract]
		bool AuthenticateClient(string userId, string password);

		/// <summary>
		/// Validates the user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		bool ValidateUser(string userName, string password);

		/// <summary>
		/// Gets the name of the user logon by user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		UserLogon GetUserLogonByUserName(string userName, List<EntityRelations> loadOptions);

		/// <summary>
		/// Determines whether [is user name already exist] [the specified user name].
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns>
		/// 	<c>true</c> if [is user name already exist] [the specified user name]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsUserNameAlreadyExist(string userName);

		/// <summary>
		/// Inserts the clients registration.
		/// </summary>
		/// <param name="clientsRegistration">The clients registration.</param>
		/// <returns></returns>
		[OperationContract]
		ClientsRegistration InsertClientsRegistration(ClientsRegistration clientsRegistration);

		/// <summary>
		/// Gets the client by email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		Client GetClientByEmail(string email, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the client registration.
		/// </summary>
		/// <param name="regID">The reg ID.</param>
		/// <param name="clientID">The client ID.</param>
		[OperationContract]
		void UpdateClientRegistration(int regID, int clientID);

		/// <summary>
		/// Generates the client registration event.
		/// </summary>
		/// <param name="regID">The reg ID.</param>
		/// <param name="source">The source.</param>
		[OperationContract]
		void GenerateClientRegistrationEvent(int regID, string source);

		/// <summary>
		/// Creates the user.
		/// </summary>
		/// <param name="clientsRegistration">The clients registration.</param>
		/// <returns></returns>
		[OperationContract]
		CreateUserStatus CreateUser(ClientsRegistration clientsRegistration);

		/// <summary>
		/// Emails the password.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="email">The email.</param>
		/// <returns></returns>
		[OperationContract]
		string EmailPassword(string userName, string email);

		/// <summary>
		/// Gets the clients registration by token key.
		/// </summary>
		/// <param name="tokenKey">The token key.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		ClientsRegistration GetClientsRegistrationByTokenKey(string tokenKey, List<EntityRelations> loadOptions);

		/// <summary>
		/// Authorises the user registration.
		/// </summary>
		/// <param name="tokenKey">The token key.</param>
		/// <returns></returns>
		[OperationContract]
		string AuthoriseUserRegistration(string tokenKey);

		/// <summary>
		/// Gets the client contacts by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<ClientContact> GetClientContactsByClientID(int clientID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the client contacts by client ID and status.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="status">if set to <c>true</c> [status].</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<ClientContact> GetClientContactsByClientIDAndStatus(int clientID, bool status, List<EntityRelations> loadOptions);
		/// <summary>
		/// Gets the client contact by contact ID.
		/// </summary>
		/// <param name="contactID">The contact ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		ClientContact GetClientContactByContactID(int contactID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the client contact.
		/// </summary>
		/// <param name="clientContact">The client contact.</param>
		/// <param name="highResFilePath">The high res file path.</param>
		/// <param name="backUpFilePath">The back up file path.</param>
		[OperationContract]
		void UpdateClientContact(ClientContact clientContact, string highResFilePath, string backUpFilePath);

		/// <summary>
		/// Inserts the client contact.
		/// </summary>
		/// <param name="clientContact">The client contact.</param>
		/// <returns></returns>
		[OperationContract]
		ClientContact InsertClientContact(ClientContact clientContact);

		/// <summary>
		/// Updates the client contact status.
		/// </summary>
		/// <param name="contactID">The contact ID.</param>
		/// <param name="isActive">if set to <c>true</c> [is active].</param>
		[OperationContract]
		void UpdateClientContactStatus(int contactID, bool isActive);

		/// <summary>
		/// Gets the user logon by contact ID.
		/// </summary>
		/// <param name="contactID">The contact ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		UserLogon GetUserLogonByContactID(int contactID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Inserts the user logon.
		/// </summary>
		/// <param name="userLogon">The user logon.</param>
		/// <returns></returns>
		[OperationContract]
		UserLogon InsertUserLogon(UserLogon userLogon);

		/// <summary>
		/// Updates the user logon.
		/// </summary>
		/// <param name="contactID">The contact ID.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		[OperationContract]
		void UpdateUserLogon(int contactID, string userName, string password);

		/// <summary>
		/// Updates the client.
		/// </summary>
		/// <param name="client">The client.</param>
		[OperationContract]
		void UpdateClient(Client client);


		/// <summary>
		/// Gets the current orders by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <param name="agentContactID">The agent contact ID.</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetCurrentOrdersByClientID(int clientID, bool isWorkShop, int agentContactID);

		/// <summary>
		/// Gets the unhandle workflow alert by order ID.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<WorkflowAlert> GetUnhandleWorkflowAlertByOrderID(int orderID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the conjunctional agent name and office by order ID.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetConjunctionalAgentNameAndOfficeByOrderID(int orderID);

		/// <summary>
		/// Gets the online design orders by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <param name="agentContactID">The agent contact ID.</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetOnlineDesignOrdersByClientID(int clientID, bool isWorkShop, int agentContactID);

		/// <summary>
		/// Gets the waiting for approval orders by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetWaitingForApprovalOrdersByClientID(int clientID, bool isWorkShop);

		/// <summary>
		/// Gets the waiting for removal orders by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetWaitingForRemovalOrdersByClientID(int clientID, bool isWorkShop);

		/// <summary>
		/// Gets the SMS on demand orders by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetSMSOnDemandOrdersByClientID(int clientID, bool isWorkShop);

		/// <summary>
		/// Gets all clients.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<ClientDetails> GetAllClients();

		/// <summary>
		/// Gets the client favourite products.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="agentContactId">The agent contact id.</param>
		/// <param name="diyOnly">if set to <c>true</c> [diy only].</param>
		/// <returns></returns>
		[OperationContract]
		List<Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineProduct> GetClientFavouriteProducts(int clientId, int agentContactId, bool diyOnly);

		/// <summary>
		/// Deletes the client favourite products.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="products">The products.</param>
		/// <param name="loadOptions">The load options.</param>
		[OperationContract]
		void DeleteClientFavouriteProducts(int clientId, List<int> products, List<EntityRelations> loadOptions);

		/// <summary>
		/// Inserts the client favourite products.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="agentContactId">The agent contact id.</param>
		/// <param name="products">The products.</param>
		/// <param name="loadOptions">The load options.</param>
		[OperationContract]
		void InsertClientFavouriteProducts(int clientId, int agentContactId, List<int> products, List<EntityRelations> loadOptions);


		/// <summary>
		/// Gets the properties by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Property> GetPropertiesByClientID(int clientID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Searches the by digit.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="searchID">The search ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> SearchByDigit(int clientID, int searchID, bool isWorkShop);

		/// <summary>
		/// Searches the by alpha numeric.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="searchText">The search text.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> SearchByAlphaNumeric(int clientID, string searchText, bool isWorkShop);

		/// <summary>
		/// Gets the order status.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetOrderStatus(int orderID);

		/// <summary>
		/// Determines whether [is view last proof applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is view last proof applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsViewLastProofApplicable(int orderID);

		/// <summary>
		/// Gets the recent properties by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <param name="agentContactID">The agent contact ID.</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetRecentPropertiesByClientID(int clientID, bool isWorkShop, int agentContactID);

		/// <summary>
		/// Determines whether [is upload text applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is upload text applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsUploadTextApplicable(int orderID);

		/// <summary>
		/// Determines whether [is upload image applicable] [the specified order ID].
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is upload image applicable] [the specified order ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsUploadImageApplicable(int orderID);

		/// <summary>
		/// Deletes the property.
		/// </summary>
		/// <param name="propertyID">The property ID.</param>
		/// <param name="clientID">The client ID.</param>
		[OperationContract]
		void DeleteProperty(int propertyID, int clientID);

		/// <summary>
		/// Gets the client contacts for text details.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<ClientContact> GetClientContactsForTextDetails(int clientID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the SB order status.
		/// </summary>
		/// <param name="orderID">The order ID.</param>
		/// <returns></returns>
		[OperationContract]
		string GetSBOrderStatus(int orderID);

		/// <summary>
		/// Gets the awaiting for erection orders.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetAwaitingForErectionOrders(int clientID, bool isWorkShop);


		/// <summary>
		/// Gets the dash board info.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="agentContactID">The agent contact ID.</param>
		/// <returns></returns>
		[OperationContract]
		DashBoard GetDashBoardInfo(int clientID, int agentContactID);

		/// <summary>
		/// Updates the web site click.
		/// </summary>
		/// <param name="isNewWebSiteClick">if set to <c>true</c> [is new web site click].</param>
		[OperationContract]
		void UpdateWebSiteClick(bool isNewWebSiteClick);

		/// <summary>
		/// Inserts the marketing mail.
		/// </summary>
		/// <param name="marketingMailModel">The marketing mail model.</param>
		[OperationContract]
		void InsertMarketingMail(MarketingMailModel marketingMailModel);

		/// <summary>
		/// Changes the password.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		/// <param name="newPassword">The new password.</param>
		/// <returns></returns>
		[OperationContract]
		bool ChangePassword(int clientId, string userName, string password, string newPassword);

		/// <summary>
		/// Gets the client favs.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="agenContactId">The agen contact id.</param>
		/// <returns></returns>
		[OperationContract]
		List<ClientFav> GetClientFavs(int clientId, int agenContactId);

		/// <summary>
		/// Removes the myop product.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="productId">The product id.</param>
		[OperationContract]
		void RemoveMyopProduct(int clientId, int productId);

		/// <summary>
		/// ROnlineBLes the watermark missing event.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="watermarkFile">The watermark file.</param>
		[OperationContract]
		void ROnlineBLeWatermarkMissingEvent(int clientId, string watermarkFile);

		/// <summary>
		/// Gets the regular property by client id.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <returns></returns>
		[OperationContract]
		int GetRegularPropertyByClientId(int clientId);

		/// <summary>
		/// Gets the regular orders by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="isWorkShop">if set to <c>true</c> [is work shop].</param>
		/// <returns></returns>
		[OperationContract]
		List<FilterOrders> GetRegularOrdersByClientID(int clientID, bool isWorkShop);

		/// <summary>
		/// Notifies the admin about client info.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		[OperationContract]
		void NotifyAdminAboutClientInfo(int clientId);

		/// <summary>
		/// Gets the properties by client contact ID.
		/// </summary>
		/// <param name="clientContactID">The client contact ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Property> GetPropertiesByClientContactID(int clientContactID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the price list products by price list ID.
		/// </summary>
		/// <param name="priceListID">The price list ID.</param>
		/// <returns></returns>
		[OperationContract]
		List<PriceListModel> GetPriceListProductsByPriceListID(int priceListID);

		/// <summary>
		/// Gets the event subscription by client ID.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <returns></returns>
		[OperationContract]
		List<EventSubscription> GetEventSubscriberByClientID(int clientId);

		/// <summary>
		/// Saves the event subscriber.
		/// </summary>
		/// <param name="es">The es.</param>
		[OperationContract]
		void SaveEventSubscriber(List<EventSubscription> es);

		/// <summary>
		/// Determines whether [is DIY template available] [the specified client ID].
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <returns>
		/// 	<c>true</c> if [is DIY template available] [the specified client ID]; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsDIYTemplateAvailable(int clientID);

		/// <summary>
		/// Setups the DIY for client.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="contactName">Name of the contact.</param>
		[OperationContract]
		void SetupDIYForClient(int clientID, string contactName);

		/// <summary>
		/// Donots the setup DIY for client.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		[OperationContract]
		void DonotSetupDIYForClient(int clientID);

		///////// <summary>
		///////// Gets the visual listings of client.
		///////// </summary>
		///////// <param name="clientID">The client ID.</param>
		///////// <returns>Returns the visual listings.</returns>
		//////[OperationContract]
		//////List<VisualListing> GetVisualListingsByClientID(int clientID);

		/// <summary>
		/// Gets the rented assets of client.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <returns>Returns the rented assets.</returns>
		[OperationContract]
		List<RentedAsset> GetRentedAssetByClientID(int clientID);

		/// <summary>
		/// Gets the visual listing options.
		/// </summary>
		/// <param name="rentalID">The rental ID.</param>
		/// <returns>Returns the visual listing options.</returns>
		[OperationContract]
		VisualListingOption GetVisualListingOptionByRentalId(int rentalID);

		/// <summary>
		/// Gets the visual listings of client by rental id.
		/// </summary>
		/// <param name="rentalID">The rental ID.</param>
		/// <returns>Returns the visual listings.</returns>
		[OperationContract]
		List<VisualListing> GetVisualListingsByClientAssetRentalID(int rentalID);

		/// <summary>
		/// Updates listing display option for custom  visual listing by rental id.
		/// </summary>
		/// <param name="rentalID">The rental ID.</param>
		/// <returns>Returns the response.</returns>
		[OperationContract]
		AbcVisualResponse UpdateDisplayOptionForCustomVisualListings(int rentalID);

		/// <summary>
		/// Adds a visual listing.
		/// </summary>
		/// <param name="rentalID">The rental ID.</param>
		/// <param name="listingID">The listing ID.</param>
		/// <returns>Returns the response.</returns>
		[OperationContract]
		AbcVisualResponse AddVisualListing(int rentalID, int listingID);

		/// <summary>
		/// Removes visual listing.
		/// </summary>
		/// <param name="rentalID">The rental ID.</param>
		/// <param name="listingID">The listing ID.</param>
		/// <returns>Returns the response.</returns>
		[OperationContract]
		AbcVisualResponse RemoveVisualListing(int rentalID, int listingID);

		/// <summary>
		/// Get the client asset rental informations.
		/// </summary>
		/// <param name="rentalID">The rental ID.</param>
		/// <returns>Returns the client asset rental.</returns>
		[OperationContract]
		ABCVIS_ClientsAssetRental GetClientsAssetRental(int rentalID);

		/// <summary>
		/// Adds all active listings as visual listings.
		/// </summary>
		/// <param name="rentalID">The rental id.</param>
		/// <returns>Returns the response.</returns>
		[OperationContract]
		AbcVisualResponse AddAllListingsAsVisualListing(int rentalID);

		/// <summary>
		/// Adds all new listings as visual listings.
		/// </summary>
		/// <param name="rentalID">The rental id.</param>
		/// <returns>Returns the response.</returns>
		[OperationContract]
		AbcVisualResponse AddAllNewListingsAsVisualListing(int rentalID);

		/// <summary>
		/// Gets the template config.
		/// </summary>
		/// <param name="rentalID">The rental id.</param>
		/// <returns>Returns the template config.</returns>
		[OperationContract]
		string GetTemplateConfigByRentalID(int rentalID);

		/// <summary>
		/// Updates template data.
		/// </summary>
		/// <param name="rentalID">The rental id.</param>
		/// <param name="templateData">The template data.</param>
		/// <returns>Returns the response.</returns>
		[OperationContract]
		AbcVisualResponse UpdateTemplateDataByRentalID(int rentalID, string templateData);

		/// <summary>
		/// Gets the visual clients asset rental.
		/// </summary>
		/// <param name="rentalID">The rental id.</param>
		/// <returns>Returns the visual clients asset rental.</returns>
		[OperationContract]
		ABCVIS_ClientsAssetRental GetVisualClientsAssetRentalByRentalID(int rentalID);

        /// <summary>
        /// Updates device visual listings.
        /// </summary>
        /// <param name="rentalID">The rental id.</param>
        /// <param name="newListings">The new listings.</param>
        /// <param name="removeListings">The remove listings.</param>
        /// <returns>Returns the response.</returns>
        [OperationContract]
        AbcVisualResponse UpdateDeviceVisualListings(int rentalID, List<int> newListings, List<int> removeListings);

        /// <summary>
        /// Updates device visual listings by listing type.
        /// </summary>
        /// <param name="rentalID">The rental id.</param>
        /// <param name="listingType">The listing type.</param>
        /// <returns>Returns the response.</returns>
        [OperationContract]
        AbcVisualResponse UpdateDeviceVisusalListingsByListingType(int rentalID, int listingType);        

		/// <summary>
		/// Notifies the admin about client info update request.
		/// </summary>
		/// <param name="client">The client.</param>
		[OperationContract]
		void NotifyAdminAboutClientInfoUpdateRequest(Abc.OnlineBL.Entities.Client client);

        /// <summary>
        /// Clients the can order DIY only.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        [OperationContract]
        bool ClientCanOrderDIYOnly(int clientId);

        /// <summary>
        /// Clients to supply ready artwork only.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        [OperationContract]
        bool ClientToSupplyReadyArtworkOnly(int clientId);

        /// <summary>
        /// Gets national client post code.
        /// </summary>
        /// <param name="postCode">The post code.</param>
        /// <param name="suburb">The suburb.</param>
        /// <param name="state">the state.</param>
        /// <returns></returns>
        [OperationContract]
        List<ClientNationalDeliveryLocation> GetClientNationalDeliveryLocation(string postCode, string suburb, string state);

        /// <summary>
        /// Get Print Ready Templates By Price ListID
        /// </summary>
        /// <param name="priceListID">the price list ID.</param>
        /// <returns></returns>
        [OperationContract]
        List<PrintReadyTemplate> GetPrintReadyTemplatesByPriceListID(int priceListID);

        /// <summary>
        /// Notifies the admin about ABCRE Property Listing request.
        /// </summary>
        /// <param name="propertyListingModel">The propertyListingModel.</param>
        [OperationContract]
        void NotifyAdminAboutABCREPropertyListingRequest(PropertyListingModel propertyListingModel);

        /// <summary>
        /// Update Office Setting.
        /// </summary>
        /// <param name="client">The client.</param>
        [OperationContract]
        void UpdateOfficeSetting(Client client);
    }
}
