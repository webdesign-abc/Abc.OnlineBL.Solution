using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// ITemplateManagerService interface provide methods to get custom data.
	/// </summary>
    [ServiceContract]
    public interface ITemplateManagerService
    {
        // Return DataTable because we don't want to create classes for custom set of data that will make
        // the Entities project very messy. Custom classes should only be from the client.

        /// <summary>
        /// SayHello returns what you pass to it just to let you know that it is listening to you.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [OperationContract]
        string SayHello(string name);

        /// <summary>
        /// Gets clients with AllowAOP field and Ceased Field. Only a subset of the data is returned.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataTable GetClients();

        /// <summary>
        /// Gets clients with AllowAOP and Ceased fields who can order a specific product.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataTable GetClientsWhoCanOrderProduct(string clientName, int productId);

        /// <summary>
        /// Gets the in active clients.
        /// </summary>
        /// <param name="timeInMonth">The time in month.</param>
        /// <returns></returns>
        [OperationContract]
        DataTable GetInActiveClients(int timeInMonth);

        /// <summary>
        /// Updates the AOP client pref. PrefID used is 6
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="isEnabledForAOP">if set to <c>true</c> [is enabled for AOP].</param>
        [OperationContract]
        void UpdateAOPClientPref(int clientId, bool isEnabledForAOP);

        /// <summary>
        /// Gets the product types in (1,2,4,5,17,18).
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ProductType> GetProductTypes();

        /// <summary>
        /// Gets the product by id.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns></returns>
        [OperationContract]
        DataTable GetProductById(int productId);

        /// <summary>
        /// Gets the products by a list of product ids.
        /// </summary>
        /// <param name="productIds">The product id list.</param>
        /// <returns></returns>
        [OperationContract]
        DataTable GetProductsByIds(List<int> productIds);

        /// <summary>
        /// Gets the products by type id. This function gets the subset of the data.
        /// </summary>
        /// <param name="typeId">The type id.</param>
        /// <returns></returns>
        [OperationContract]
        DataTable GetProductsByTypeId(int typeId);

        /// <summary>
        /// Gets the last order id.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int GetLastOrderId();

        /// <summary>
        /// Gets the client groups.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Dictionary<int, string> GetClientGroups();

        /// <summary>
        /// Gets the client office by group ID.
        /// </summary>
        /// <param name="submittedGroupID">The submitted group ID.</param>
        /// <returns></returns>
        [OperationContract]
        List<Entities.Client> GetClientOfficeByGroupID(int submittedGroupID);

        /// <summary>
        /// Gets the client name by id.
        /// </summary>
        /// <param name="submittedClientID">The submitted client ID.</param>
        /// <returns></returns>
        [OperationContract]
        string GetClientNameById(int submittedClientID);

        /// <summary>
        /// Gets the template ID by templatepath.
        /// </summary>
        /// <param name="submittedPath">The submitted path.</param>
        /// <returns></returns>
        [OperationContract]
        int? GetTemplateIDByTemplatepath(string submittedPath);

        /// <summary>
        /// Inserts the data analyz queue.
        /// </summary>
        /// <param name="submittedTemplateID">The submitted template ID.</param>
        /// <returns></returns>
        [OperationContract]
        bool InsertDataAnalyzQueue(int? submittedTemplateID);

        /// <summary>
        /// Gets the client N office name by client ID.
        /// </summary>
        /// <param name="submittedClientId">The submitted client id.</param>
        /// <returns></returns>
        [OperationContract]
        string GetClientNOfficeNameByClientID(int submittedClientId);


        /// <summary>
        /// Gets the client by GRP id.
        /// </summary>
        /// <param name="submittedGrpId">The submitted GRP id.</param>
        /// <returns></returns>
        [OperationContract]
        List<Entities.ClientGroup> GetClientByGrpId(int submittedGrpId);

        /// <summary>
        /// Gets the GRP N client by client id.
        /// </summary>
        /// <param name="submittedClientId">The submitted client id.</param>
        /// <returns></returns>
        [OperationContract]
        Entities.Client GetGrpNClientByClientId(int submittedClientId);

        /// <summary>
        /// Gets the name of the clients by GRP.
        /// </summary>
        /// <param name="submittedGrpName">Name of the submitted GRP.</param>
        /// <returns></returns>
        [OperationContract]
        List<Entities.ClientGroup> GetClientsByGrpName(string submittedGrpName);

        /// <summary>
        /// Gets the name of the clients by office.
        /// </summary>
        /// <param name="submittedOfficeName">Name of the submitted office.</param>
        /// <returns></returns>
        [OperationContract]
        List<Entities.Client> GetClientsByOfficeName(string submittedOfficeName);

        /// <summary>
        /// Gets the AOP client pref by client ID.
        /// </summary>
        /// <param name="submittedClientId">The submitted client id.</param>
        /// <returns></returns>
        [OperationContract]
        bool? GetAOPClientPrefByClientID(int submittedClientId);

        /// <summary>
        /// Gets the clients ID by group ID.
        /// </summary>
        /// <param name="submittedGrpID">The submitted GRP ID.</param>
        /// <returns></returns>
        [OperationContract]
        List<int> GetClientsIDByGroupID(int submittedGrpID);
    }
}
