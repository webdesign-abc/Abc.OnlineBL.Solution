using System.Collections.Generic;
using System.ServiceModel;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;

namespace Abc.OnlineBL.Service
{
    /// <summary>
    /// Floor Plan Service Interface.
    /// </summary>
    [ServiceContract]
    public interface IFloorPlanService
    {
        /// <summary>
        /// Gets the floorplan Package list.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>
        /// Returns the list of the floorplan Package Objects.
        /// </returns>
        [OperationContract]
        List<FloorplanPackage> GetFloorPlanPackageList(int clientId);


        /// <summary>
        /// Gets the floor plan package by id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns>
        /// Returns the floor plan package object.
        /// </returns>
        [OperationContract]
        FloorplanPackage GetFloorPlanPackageById(int floorPlanPackageId, int clientId, List<EntityRelations> loadOptions);
        
        /*
        /// <summary>
        /// Gets the floor plan package by id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="clientId">The client id.</param>
        /// <returns>Returns the floor plan package object.</returns>
        [OperationContract]
        FloorplanPackage GetFloorPlanPackageById(int floorPlanPackageId, int clientId);
        */

        /// <summary>
        /// Gets the floor plan by id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="floorPlanId">The floor plan id.</param>
        /// <returns>
        /// Returns the Floor Plan object.
        /// </returns>
        [OperationContract]
        Floorplan GetFloorPlanById(int floorPlanPackageId, int floorPlanId);

        /// <summary>
        /// Inserts the floor plan image store.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileType">Type of the file.</param>
        [OperationContract]
        void InsertFloorPlanImageStore(int floorPlanPackageId, string fileName, int fileType);

        /// <summary>
        /// Deletes the floor plan by id.
        /// </summary>
        /// <param name="floorPlanId">The floor plan id.</param>
        [OperationContract]
        void DeleteFloorPlanById(int floorPlanId);

        /// <summary>
        /// Updates the status by floor plan package id.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="statusValue">The status value.</param>
         [OperationContract]
        void UpdateStatusByFloorPlanPackageId(int floorPlanPackageId,int statusValue);

        /*
         /// <summary>
         /// Deletes the floor plan icon.
         /// </summary>
         /// <param name="floorPlanIcon">The floor plan icon.</param>
         [OperationContract]
         void DeleteFloorPlanIcon(FloorplanIcon floorPlanIcon);
         */

         /// <summary>
         /// Deletes the floor plan icons.
         /// </summary>
         /// <param name="floorPlanIconsList">The floor plan icons list.</param>
         [OperationContract]
         void DeleteFloorPlanIcons(List<FloorplanIcon> floorPlanIconsList);

         /// <summary>
         /// Inserts the floor plan icons.
         /// </summary>
         /// <param name="floorPlanIcons">The floor plan icons.</param>
         [OperationContract]
         void InsertFloorPlanIcons(List<FloorplanIcon> floorPlanIcons);

         /// <summary>
         /// Inserts the floor plan.
         /// </summary>
         /// <param name="floorPlanObj">The floor plan obj.</param>
         /// <returns></returns>
         [OperationContract]
         int? InsertFloorPlan(Floorplan floorPlanObj);

         /// <summary>
         /// Gets the location match.
         /// </summary>
         /// <param name="state">The state.</param>
         /// <param name="location">The location.</param>
         /// <param name="postCode">The post code.</param>
         /// <returns>
         /// Returns the List of the Location Obj.
         /// </returns>
         [OperationContract]
         List<Location> GetLocationMatch(string state, string location, string postCode);

         /// <summary>
         /// Inserts the floorplan package.
         /// </summary>
         /// <param name="floorplanPackageObj">The floorplan package obj.</param>
         /// <returns>
         /// Returns FloorplanPackage Obj
         /// </returns>
        [OperationContract]
        FloorplanPackage InsertFloorplanPackage(FloorplanPackage floorplanPackageObj);


        /// <summary>
        /// Gets the floor plan price.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="productId">The product id.</param>
        /// <returns>Returns FloorPlanPrice Obj</returns>
        [OperationContract]
        FloorPlanPrice GetFloorPlanPrice(int clientId, int productId);

        /// <summary>
        /// Pres the payment process.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <returns>Pre payment processing task</returns>
        [OperationContract]
        int? PrePaymentProcess(int? productId, int? floorPlanPackageId);

        /// <summary>
        /// Posts the payment process.
        /// </summary>
        /// <param name="floorPlanPackageId">The floor plan package id.</param>
        /// <param name="paymentResponseData">The payment response data.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        [OperationContract]
        int? PostPaymentProcess(int? floorPlanPackageId, string paymentResponseData, int? status);
    }
}
