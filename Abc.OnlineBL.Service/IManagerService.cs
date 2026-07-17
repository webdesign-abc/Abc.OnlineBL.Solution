using System.Collections.Generic;
using System.ServiceModel;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using System;
using Abc.OnlineBL.Entities.Model.OnlineOrder;

namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// Manager Related Backend Services
	/// </summary>
    [ServiceContract]
    public interface IManagerService
    {
        /// <summary>
        /// Says the hello.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [OperationContract]
        string SayHello(string name);

        /// <summary>
        /// Get Manager by Id
        /// </summary>
        /// <param name="manId"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [OperationContract]
        Manager GetManagerById(string manId, List<EntityRelations> loadOptions);

        /// <summary>
        /// Get Truck By Id
        /// </summary>
        /// <param name="truckId"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [OperationContract]
        Truck GetTruckById(int truckId, List<EntityRelations> loadOptions);

        /// <summary>
        /// Get Truck By UserID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Truck GetTruckByUserId(string userId);

        /// <summary>
        /// Get All Trucks
        /// </summary>
        /// <param name="active">Active or InActive Trucks</param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [OperationContract]
        List<Truck> GetAllTrucks(bool active, List<EntityRelations> loadOptions);

        /// <summary>
        /// Get Runsheets by truck driver for a specific date
        /// </summary>
        /// <param name="truckId"></param>
        /// <param name="forDate"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [OperationContract]
        List<RunSheet> GetRunsheetByDriver(int truckId, DateTime forDate, List<EntityRelations> loadOptions);

        /// <summary>
        /// Get runsheets by manager
        /// </summary>
        /// <param name="manId"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [OperationContract]
        List<RunSheet> GetRunsheetByManager(string manId, List<EntityRelations> loadOptions);

        /// <summary>
        /// Get runsheet by a manager for a date
        /// </summary>
        /// <param name="manId"></param>
        /// <param name="forDate"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [OperationContract]
        List<RunSheet> GetRunsheetByManagerDate(string manId, DateTime forDate, List<EntityRelations> loadOptions);

        /// <summary>
        /// Gets the runsheet details by truck IDN date.
        /// </summary>
        /// <param name="submittedTruckID">The submitted truck ID.</param>
        /// <param name="submittedDate">The submitted date.</param>
        /// <returns></returns>
        [OperationContract]
        List<Abc.OnlineBL.Entities.Model.RunsheetModel> GetRunsheetDetailsByTruckIDNDate(int submittedTruckID, DateTime submittedDate);

        /// <summary>
        /// This service is used to update an RunsheetID's 
        /// RunsheetDetails SL according to OrderIDList
        /// </summary>
        /// <param name="RunsheetID">Which runsheets Details will be update</param>
        /// <param name="OrderIDList">The SL will be update according to the (index+1) wise of OrderIDList</param>
        /// <returns></returns>
        [OperationContract]
        bool SetReOrderRunsheetDetailByRunsheetID(int RunsheetID, List<int> OrderIDList);

        /// <summary>
        /// This service used for MarkOff runsheet
        /// According to RunsheetID list
        /// </summary>
        /// <param name="submittedRunsheetIDList">The submitted runsheet ID list.</param>
        /// <param name="submittedTruckID">The submitted truck ID.</param>
        /// <param name="localTime">The Driver's device local time</param>
        /// <returns></returns>
        [OperationContract]
        bool SetRunsheetsMarkOffbyRIDLocalTime(List<int> submittedRunsheetIDList, int submittedTruckID, DateTime localTime);

        /// <summary>
        /// This service used for MarkOff runsheet (used by Old driver app)
        /// According to RunsheetID list
        /// </summary>
        /// <param name="submittedRunsheetIDList">The submitted runsheet ID list.</param>
        /// <param name="submittedTruckID">The submitted truck ID.</param>
        /// <returns></returns>
        [OperationContract]
        bool SetRunsheetsMarkOffbyRID(List<int> submittedRunsheetIDList, int submittedTruckID);

        /// <summary>
        /// Jobs the complete and mark offed.
        /// </summary>
        /// <param name="rs">The rs.</param>
        /// <returns></returns>
        [OperationContract]
        bool JobCompleteAndMarkOffed(RunsheetDetailCompleted rs);

         /// <summary>
        /// updated JobCompleteAndMarkOffed
        /// See more JobCompleteAndMarkOffed
        /// </summary>
        /// <param name="rs">The rs.</param>
        /// <returns></returns>
        [OperationContract]
        bool DriverJobCompleteAndMarkOffed(RunsheetDetailCompleted rs);

        
        /// <summary>
        /// Gets the runsheet detail by ID.
        /// </summary>
        /// <param name="runsheetID">The runsheet ID.</param>
        /// <param name="orderID">The order ID.</param>
        /// <returns>
        /// Returns the RunsheetDetails Obj.
        /// </returns>
        [OperationContract]
        RunsheetDetail GetRunsheetDetailByID(int runsheetID, int orderID);

        /// <summary>
        /// Adds a new driver report to the dataReport table using a stored procedure
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateDriverReport(DriverReport report);
    }
}
