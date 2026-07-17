using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Entities.Model.Myop;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// IProductService
	/// </summary>
	[ServiceContract]
	public interface IProductService
	{
		/// <summary>
		/// SayHello returns what you pass to it just to let you know that it is listening to you.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[OperationContract]
		string SayHello(string name);

		/// <summary>
		/// Gets all the product types.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<ProductType> GetProductTypes();

		/// <summary>
		/// Gets the product by id.
		/// </summary>
		/// <param name="productId">The product id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		Product GetProductById(int productId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the products by a list of product ids.
		/// </summary>
		/// <param name="productIds">The product id list.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetProductsByIds(List<int> productIds, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the products by type id.
		/// </summary>
		/// <param name="typeId">The type id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetProductsByTypeId(int typeId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the product size code details by product type id.
		/// </summary>
		/// <param name="productTypeId">The product type id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<ProductSizeCodeDetail> GetProductSizeCodeDetailsByProductTypeId(int productTypeId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the Product Type.
		/// </summary>
		/// <param name="productType">Type of the product.</param>
		[OperationContract]
		void UpdateProductType(ProductType productType);

		/// <summary>
		/// Gets the products by type ID and price list ID.
		/// </summary>
		/// <param name="typeID">The type ID.</param>
		/// <param name="priceListID">The price list ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetProductsByTypeIDAndPriceListID(int typeID, int priceListID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the products by type ID and client ID.
		/// </summary>
		/// <param name="typeID">The type ID.</param>
		/// <param name="clientID">The client ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetProductsByTypeIDAndClientID(int typeID, int clientID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the window card products by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetWindowCardProductsByClientID(int clientID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the brochure products by client ID.
		/// </summary>
		/// <param name="clientID">The client ID.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetBrochureProductsByClientID(int clientID, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the online order products by type ID and client ID.
		/// </summary>
		/// <param name="typeID">The type ID.</param>
		/// <param name="clientID">The client ID.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetOnlineOrderProductsByTypeIDAndClientID(int typeID, int clientID);

		/// <summary>
		/// Gets the packages content product by package ID.
		/// </summary>
		/// <param name="packageId">The package id.</param>
		/// <returns></returns>
		[OperationContract]
		List<PackageProduct> GetPackagesContentProductByPackageID(int packageId);

		/// <summary>
		/// Gets all products.
		/// </summary>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<Product> GetAllProducts(List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the workflow types.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<WorkflowType> GetWorkflowTypes();

		/// <summary>
		/// Gets the product size code details.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<ProductSizeCodeDetail> GetProductSizeCodeDetails();

		/// <summary>
		/// Gets the product filter data.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		ProductFilterData GetProductFilterData();

		/// <summary>
		/// Updates the price list price.
		/// </summary>
		/// <param name="productID">The product ID.</param>
		/// <returns></returns>
		[OperationContract]
		void UpdatePriceListPrice(int productID);

		/// <summary>
		/// Copies the product.
		/// </summary>
		/// <param name="productID">The product ID.</param>
		[OperationContract]
		void CopyProduct(int productID);

		/// <summary>
		/// Updates the product.
		/// </summary>
		/// <param name="product">The product.</param>
		[OperationContract]
		void UpdateProduct(Product product);

		/// <summary>
		/// Gets the AJPS rule by id.
		/// </summary>
		/// <param name="ruleId">The rule id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		AJPS_Rule GetAJPSRuleById(int ruleId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the RIP locations.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<RIPLocation> GetRIPLocations();

		/// <summary>
		/// Gets all managers.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<ManagerDetails> GetAllManagers();

		/// <summary>
		/// Gets all states.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<State> GetAllStates();

		/// <summary>
		/// Gets all business regions.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<BusinessRegion> GetAllBusinessRegions();

		/// <summary>
		/// Gets all online order categories.
		/// </summary>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<OnlineOrderCategory> GetAllOnlineOrderCategories(List<EntityRelations> loadOptions);


		/// <summary>
		/// Gets the online products by category id.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="categoryId">The category id.</param>
		/// <param name="subCategoryId">The sub category id.</param>
		/// <param name="returnDIYOnly">if set to <c>true</c> [return DIY only].</param>
		/// <returns></returns>
		[OperationContract]
		List<OnlineProduct> GetOnlineProductsByCategoryId(int clientId, int categoryId, int? subCategoryId, bool returnDIYOnly);

        /// <summary>
        /// Return a List of Property Related Products a Client can order. Includes their Fav products as well
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>Online Product object with necessary metadata</returns>
        [OperationContract]
        OnlineCategoryAndProductModel GetAllOnlineProductsForExpressOrder(int clientId);

        /// <summary>
        /// Return a List of Regular Property Related Products a Client can order. Includes their Fav products as well
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>Online Product object with necessary metadata</returns>
        [OperationContract]
        OnlineCategoryAndProductModel GetAllRegularOnlineProductsForExpressOrder(int clientId);

        /// <summary>
        /// Populate DIYTemplate and ProductConfig for a given OnlineProduct Object
        /// </summary>
        /// <param name="clientId">current client id</param>
        /// <param name="product">product whos properties to populate</param>
        /// <returns>returns the same product with the required properties filled in</returns>
        [OperationContract]
        OnlineProduct PopulateConfigAndMatchingTemplates(int clientId, OnlineProduct product);

		/// <summary>
		/// Gets the related online products by category id.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="orderdProductId">The orderd product id which is already in the shopping cart.</param>
		/// <returns></returns>
		[OperationContract]
		List<OnlineProduct> GetRelatedOnlineProductsByCategoryId(int clientId, List<int> orderdProductId);


		/// <summary>
		/// Deletes the product.
		/// </summary>
		/// <param name="productId">The product id.</param>
		[OperationContract]
		string DeleteProduct(int productId);

		/// <summary>
		/// Gets the product file list.
		/// </summary>
		/// <param name="productId">The product id.</param>
		/// <returns></returns>
		[OperationContract]
		List<string> GetProductFileList(int productId);

		/// <summary>
		/// Removes the product file.
		/// </summary>
		/// <param name="productId">The product id.</param>
		/// <param name="fileName">Name of the file.</param>
		[OperationContract]
		void RemoveProductFile(int productId, string fileName);

		/// <summary>
		/// Moves the product file to right folder.
		/// </summary>
		/// <param name="productId">The product id.</param>
		/// <param name="fileName">Name of the file.</param>
		[OperationContract]
		void MoveProductFileToRightFolder(int productId, string fileName);

		/// <summary>
		/// Gets all product group descriptions.
		/// </summary>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<ProductGroupDescription> GetAllProductGroupDescriptions(List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the product group description by id.
		/// </summary>
		/// <param name="productGroupId">The product group id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		ProductGroupDescription GetProductGroupDescriptionById(int productGroupId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the product group file list.
		/// </summary>
		/// <param name="productGroupId">The product group id.</param>
		/// <returns></returns>
		[OperationContract]
		List<string> GetProductGroupFileList(int productGroupId);

		/// <summary>
		/// Moves the product group file to right folder.
		/// </summary>
		/// <param name="productGroupId">The product group id.</param>
		/// <param name="fileName">Name of the file.</param>
		[OperationContract]
		void MoveProductGroupFileToRightFolder(int productGroupId, string fileName);

		/// <summary>
		/// Removes the product group file.
		/// </summary>
		/// <param name="productGroupId">The product group id.</param>
		/// <param name="fileName">Name of the file.</param>
		[OperationContract]
		void RemoveProductGroupFile(int productGroupId, string fileName);

		/// <summary>
		/// Updates the product group description.
		/// </summary>
		/// <param name="productGroupDescription">The product group description.</param>
		[OperationContract]
		void UpdateProductGroupDescription(ProductGroupDescription productGroupDescription);

		/// <summary>
		/// Deletes the product group description.
		/// </summary>
		/// <param name="productGroupId">The product group id.</param>
		[OperationContract]
		void DeleteProductGroupDescription(int productGroupId);

		/// <summary>
		/// Removes all product group file.
		/// </summary>
		/// <param name="productGroupId">The product group id.</param>
		[OperationContract]
		void RemoveAllProductGroupFile(int productGroupId);

		/// <summary>
		/// Removes all product file.
		/// </summary>
		/// <param name="productId">The product id.</param>
		[OperationContract]
		void RemoveAllProductFile(int productId);

		/// <summary>
		/// Gets the name of the product image by file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		[OperationContract]
		Byte[] GetProductImageByFileName(string fileName);

		/// <summary>
		/// Gets the name of the group image by file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		[OperationContract]
		Byte[] GetGroupImageByFileName(string fileName);

		/// <summary>
		/// Gets the product image file list.
		/// </summary>
		/// <param name="productId">The product id.</param>
		/// <returns></returns>
		[OperationContract]
		List<string> GetProductImageFileList(int productId);

		/// <summary>
		/// Gets the product spec sheet file list.
		/// </summary>
		/// <param name="productId">The product id.</param>
		/// <returns></returns>
		[OperationContract]
		List<string> GetProductSpecSheetFileList(int productId);

		/// <summary>
		/// Saves the package.
		/// </summary>
		/// <param name="model">The model.</param>
		[OperationContract]
		void SavePackage(MyOwnPackageModel model);

		/// <summary>
		/// Gets the PDF template.
		/// </summary>
        /// <param name="productID">The product ID.</param>
        /// <param name="sizeCode">The size code.</param>
		/// <param name="frameType">Type of the frame.</param>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="orientation">The orientation.</param>
		/// <returns></returns>
		[OperationContract]
		PDF_Template GetPDFTemplate(int productID, string sizeCode, string frameType, string contentType, string orientation);

		/// <summary>
		/// Gets the optional products.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <returns></returns>
		[OperationContract]
		List<OnlineProduct> GetOptionalProducts(int clientId);

		/// <summary>
		/// Gets the content types.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<string> GetContentTypes();

        /// <summary>
        /// Gets the upgrade product by product id.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        [OperationContract]
        List<UpgradeProduct> GetUpgradeProductByProductId(int productId, int clientId);

        /// <summary>
        /// Gets the Corflute Price by product id.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        [OperationContract]
        List<CorflutePricing> GetCorflutePrice(int productId, int clientId);

        /// <summary>
        /// Gets the Price List Details by price list id and product id.
        /// </summary>
        /// <param name="priceListId">The price list id.</param>
        /// <param name="productId">The product id.</param>
        /// <returns>PriceListDetail</returns>
        [OperationContract]
        decimal GetPriceListDetailsByPriceListIDAndProductID(int priceListId, int productId);

        /// <summary>
        /// Get Drone Availability Status for Location
        /// </summary>
        /// <param name="propertyID">Property Id</param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [OperationContract]
        ProductAvailability GetDronePhotographyAvailability(int propertyID, List<EntityRelations> loadOptions);
	}
}
