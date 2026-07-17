using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Abc.OnlineBL.Service
{
    /// <summary>
	/// IDesignService interface
	/// </summary>
    [ServiceContract]
    public interface IDesignService
    {
        /// <summary>
		/// Gets the items in the queue by status id.
		/// </summary>
		/// <returns>List of AOP_JobQueue</returns>
        [OperationContract]
        List<Design_JobQueue> GetDesignQueueItemByStatus(AOP_QueueStatusList status, List<EntityRelations> loadOptions);

        /// <summary>
		/// Locks the job queue with Concurrency Check to make sure no other connector perform the job
		/// </summary>
		/// <param name="jobQueue">The job queue.</param>
		/// <returns></returns>
		[OperationContract]
        Design_JobQueue LockDesignJobQueue(Design_JobQueue jobQueue);

        /// <summary>
		/// Gets the design document by job document id.
		/// </summary>
		/// <param name="documentId">The job document id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
        Design_Document GetDesignDocumentByDocumentId(int documentId, List<EntityRelations> loadOptions);

        /// <summary>
		/// Updates the queue status.
		/// </summary>
		/// <param name="queueId">The queue id.</param>
		/// <param name="status">The status.</param>
		/// <param name="msg">The MSG.</param>
		[OperationContract]
        void UpdateDesignQueueStatus(int queueId, AOP_QueueStatusList status, string msg);

        /// <summary>
		/// Saves the working document XML model.
		/// </summary>
		/// <param name="designDocId">The job doc id.</param>
		/// <param name="workingDocModel">The working doc model.</param>
		/// <param name="isItTempModel">if set to <c>true</c> [is it temp model].</param>
		[OperationContract]
        void SaveDesignDocumentModel(int designDocId, string workingDocModel, bool isItTempModel);

        /// <summary>
		/// Gets the job document by job document id.
		/// </summary>
		/// <param name="designDocumentId">The job document id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
        Design_Document GetDesignDocumentByDesignDocumentId(int designDocumentId, List<EntityRelations> loadOptions);

        /// <summary>
		/// Updates/Deletes/Inserts the design document list.
		/// </summary>
		/// <param name="jobs">The jobs.</param>
		/// <returns>the refreshed list</returns>
		[OperationContract]
        List<Design_Document> UpdateDesignDocument(List<Design_Document> jobs);
    }
}
