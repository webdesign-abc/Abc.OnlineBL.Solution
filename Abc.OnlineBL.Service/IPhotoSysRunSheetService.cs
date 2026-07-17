using System;
using System.Collections.Generic;
using Abc.OnlineBL.Entities;
using System.ServiceModel;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.Photography;

namespace Abc.OnlineBL.Service
{
    /// <summary>
    /// Photosys Runsheet Service Interface Class.
    /// </summary>
    [ServiceContract]
    public interface IPhotoSysRunSheetService
    {
        /// <summary>
        /// Gets the photo order.
        /// </summary>
        /// <param name="IMEI">The IMEI.</param>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        [OperationContract]
        List<PhotoOrder> GetPhotoOrder(string IMEI, DateTime dateFrom, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the photo order.
        /// </summary>
        /// <param name="IMEI">IMEI</param>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        [OperationContract]
        List<PhotoOrder> GetPhotoOrderVersion2(string IMEI, int photographerId, string username, DateTime dateFrom, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the photo order by id.
        /// </summary>
        /// <param name="IMEI">The IMEI.</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        [OperationContract]
        PhotoOrder GetPhotoOrderById(string IMEI, int orderId, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the photo order by id.
        /// </summary>
        /// <param name="IMEI">IMEI</param>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        [OperationContract]
        PhotoOrder GetPhotoOrderByIdVersion2(string IMEI, int photographerId, string username, int orderId, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the products of an Order
        /// </summary>
        /// <param name="IMEI">The IMEI.</param>
        /// <param name="orderId">The order id.</param>
        /// <returns></returns>
        [OperationContract]
        List<PhotoSysOrderProduct> GetProductsByOrderID(string IMEI, int orderId);

        /// <summary>
        /// Gets the products of an Order
        /// </summary>
        /// <param name="IMEI">IMEI</param>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <param name="orderId">The order id.</param>
        /// <returns></returns>
        [OperationContract]
        List<PhotoSysOrderProduct> GetProductsByOrderIDVersion2(string IMEI, int photographerId, string username, int orderId);

        /// <summary>
        /// Updates the photo order.
        /// </summary>
        /// <param name="IMEI">The IMEI.</param>
        /// <param name="order">The order.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        [OperationContract]
        PhotoOrder UpdatePhotoOrderSubset(string IMEI, PhotoOrder order, List<EntityRelations> loadOptions);


        /// <summary>
        /// Updates the photo order.
        /// </summary>
        /// <param name="IMEI">IMEI</param>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <param name="order">The order.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        [OperationContract]
        PhotoOrder UpdatePhotoOrderSubsetVersion2(string IMEI, int photographerId, string username, PhotoOrder order, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the photo sys run sheet.
        /// </summary>
        /// <param name="photographerId">The photographer id.</param>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        /// <returns>
        /// Returns the list of the PHOTOSYS_RunsheetResult obj.
        /// </returns>
        [OperationContract]
        List<PhotoSysRunSheet> GetPhotoSysRunSheet(int photographerId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Gets the photo sys run sheet.
        /// </summary>
        /// <param name="photographerId">The photographer id.</param>
        [OperationContract]
        PhotographerRunsheetDTO GetRunsheet(int photographerId);

        /// <summary>
        /// Gets the photo sys run sheet details.
        /// </summary>
        /// <param name="photographerId">The photographer id.</param>
        /// <param name="dateFrom">The date from.</param>
        /// <param name="dateTo">The date to.</param>
        /// <param name="orderID">The order ID.</param>
        /// <returns>
        /// Returns the PHOTOSYS_RunsheetResult obj.
        /// </returns>
        [OperationContract]
        PhotoSysRunSheetDetail GetPhotoSysRunSheetDetails(int photographerId, DateTime dateFrom, DateTime dateTo, int orderID);

        /*
        /// <summary>
        /// Gets the photographers IMEI.
        /// </summary>
        /// <param name="photographerID">The photographer ID.</param>
        /// <returns>
        /// Rentuns the IMEI number.
        /// </returns>
        //[OperationContract]
        //string GetPhotographerIMEI(int photographerID);
        */

        /// <summary>
        /// Gets the photographer.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns>
        /// Returns the Photographer.
        /// </returns>
        [OperationContract]
        Photographer GetPhotographer(string imei);

        /// <summary>
        /// Gets the photographer.
        /// </summary>
        /// <param name="IMEI">The imei.</param>
        /// <param name="photographerId">photographerId</param>
        /// <param name="username">username</param>
        /// <returns>
        /// Returns the Photographer.
        /// </returns>
        [OperationContract]
        Photographer GetPhotographerVersion2(string IMEI, int photographerId, string username);


        /// <summary>
        /// Gets the photographer's ID.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns>
        /// Returns the Photographer ID
        /// </returns>
        [OperationContract]
        int GetPhotographerID(string imei);


        /// <summary>
        /// Returns Photographer ID if a matching entry exists in the database. 
        /// </summary>
        /// <param name="IMEI">IMEI</param>
        /// <param name="username">username</param>
        /// <param name="password">PhotoSync password</param>
        /// <returns>Returns Photographer ID if exists, otherwise -1.</returns>
        [OperationContract]
        int GetPhotographerIdByLogin(string IMEI, string username, string password);


        /// <summary>
        /// Gets the content of the photographer config.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns></returns>
        [OperationContract]
        string GetPhotographerConfigContent(string imei);

        /// <summary>
        /// Gets the content of the photographer config.
        /// </summary>
        /// <param name="IMEI">IMEI</param>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <returns></returns>
        [OperationContract]
        string GetPhotographerConfigContentVersion2(string IMEI, int photographerId, string username);

        /// <summary>
        /// Updates the run sheet photo order Table.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="photographerId">The photographer id.</param>
        [OperationContract]
        void UpdateRunSheetPhotoOrder(int orderId, int photographerId);

        /// <summary>
        /// Gets the FTP path by imei.
        /// </summary>
        /// <param name="imei">The imei.</param>
        /// <returns>
        /// Returns the ftp Folder Path.
        /// </returns>
        [OperationContract]
        string GetFtpFolderPathByImei(string imei);

        /// <summary>
        /// Gets the FTP path by imei.
        /// </summary>
        /// <param name="IMEI">IMEI</param>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <returns>
        /// Returns the ftp Folder Path.
        /// </returns>
        [OperationContract]
        string GetFtpFolderPathByImeiVersion2(string IMEI, int photographerId, string username);

        /// <summary>
        /// Gets a list of Drone Authorisations
        /// </summary>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <returns>
        /// List of Drone Authorisations
        /// </returns>
        [OperationContract]
        List<DroneAuthorisation> GetDroneAuthorisationRequests(int photographerId, string username);

        /// <summary>
        /// Saves a Drone Authorisation request
        /// </summary>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <param name="droneAuthorisation">Drone Authorisation</param>
        [OperationContract]
        void SaveDroneAuthorisationRequests(int photographerId, string username, DroneAuthorisation droneAuthorisation);

        /// <summary>
        /// Gets a list of Photographers (for Drone Authorisation)
        /// </summary>
        /// <param name="photographerId">Photographer ID</param>
        /// <param name="username">username</param>
        /// <returns>
        /// List of Photographers
        /// </returns>
        [OperationContract]
        List<PhotographerName> GetPhotographers(int photographerId, string username);

        /// <summary>
        /// Get Photographer Login information
        /// </summary>
        /// <param name="IMEI">The phone IMEI</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>
        /// PhotographerLogin object
        /// </returns>
        [OperationContract]
        PhotographerLogin GetPhotographerLogin(string IMEI, string username, string password);

        /// <summary>
        /// Generates Drone Photography Status Change Event.
        /// </summary>
        /// <param name="orderID">The order ID.</param>
        [OperationContract]
        void GenerateDronePhotographyStatusChangeEvent(int orderID);

        /// <summary>
        /// Gets all the outstanding floor plans for a particular photographer.
        /// </summary>
        /// <param name="photographerId">photographerId</param>
        [OperationContract]
        List<FloorPlan> GetFloorPlans(int photographerId);

        /// <summary>
        /// Applies an image file to any FloorPlanOrders, given the orderId.
        /// </summary>
        [OperationContract]
        void AddFloorPlanImage(int photographerId, int orderId, string filePath);

        /// <summary>
        /// Applies a note to any FloorPlanOrders, given the orderId.
        /// </summary>
        [OperationContract]
        void SetFloorPlanNote(int photographerId, int orderId, string notes);

        /// <summary>
        /// Submits a new floor plan.
        /// </summary>
        /// <param name="floorPlan">floorPlan</param>
        [OperationContract]
        void SaveFloorPlan(FloorPlan floorPlan);
    }
}
