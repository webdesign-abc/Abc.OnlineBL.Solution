using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using System.ServiceModel;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service
{
	/// <summary>
	/// IAOPService interface
	/// </summary>
	[ServiceContract]
	public interface IAOPService
	{
		#region Test Function
		/// <summary>
		/// SayHello returns what you pass to it just to let you know that it is listening to you.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[OperationContract]
		string SayHello(string name);
		#endregion

		#region Policy & Templates
		/// <summary>
		/// Gets all the document policies.
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		List<AOP_DocumentPolicy> GetDocumentPolicies(List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the template by id.
		/// </summary>
		/// <param name="templateId">The template id.</param>
		/// <param name="doNotReturnTemplateModel">if set to <c>true</c> [do not return template model].</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_Template GetTemplateById(int templateId, bool doNotReturnTemplateModel, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the template by the patial path.
		/// </summary>
		/// <param name="templatePath">The template path.</param>
		/// <param name="loadOptions">The load options. Pass 'null' to avoid deep loading.</param>
		/// <param name="doNotReturnTemplateModel">if set to <c>true</c> [do not return template model].</param>
		/// <returns></returns>
		[OperationContract]
		AOP_Template GetTemplateByPath(string templatePath, bool doNotReturnTemplateModel, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the templates by paths.
		/// </summary>
		/// <param name="templatePaths">The template paths.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <param name="doNotReturnTemplateModel">if set to <c>true</c> [do not return template model], this is for performance purpose.</param>
		/// <returns></returns>
		[OperationContract]
		List<AOP_Template> GetTemplatesByPaths(List<string> templatePaths, bool doNotReturnTemplateModel, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the template. This operation can be used to insert and delete Template.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_Template UpdateTemplate(AOP_Template template);

		/// <summary>
		/// Updates the template path.
		/// </summary>
		/// <param name="templateId">The template id.</param>
		/// <param name="templatePath">The template path.</param>
		[OperationContract]
		void UpdateTemplatePath(int templateId, string templatePath);

		/// <summary>
		/// Gets the template product by id.
		/// </summary>
		/// <param name="templateProductId">The template product id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_TemplateProduct GetTemplateProductById(int templateProductId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the template products by job id.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<AOP_TemplateProduct> GetTemplateProductsByJobId(int jobId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the template products for a client with specific type.
		/// If client has any custom template then only custom templates are returned
		/// otherwise only corporate templates are returned.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="type">The type.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<AOP_TemplateProduct> GetTemplateProductsForClient(int clientId, string type, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the template products matching with products in the pricelist for a client with specific type.
		/// If client has any custom template then only custom templates are returned
		/// otherwise only corporate templates are returned.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="type">The type.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<AOP_TemplateProduct> GetTemplateProductsMatchingPriceList(int clientId, string type, List<EntityRelations> loadOptions);
		#endregion

		#region AOP Queue Related

		/// <summary>
		/// Gets the analyze queue.
		/// </summary>
		/// <param name="queueId">The queue id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_AnalyzeQueue GetAnalyzeQueue(int queueId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the analyze queue by status.
		/// </summary>
		/// <param name="status">The status.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<AOP_AnalyzeQueue> GetAnalyzeQueueByStatus(AOP_QueueStatusList status, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the analyze queue. This operation can be used to insert and delete Analyze Queue
		/// </summary>
		/// <param name="analyzeQueue">The analyze queue.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_AnalyzeQueue UpdateAnalyzeQueue(AOP_AnalyzeQueue analyzeQueue);

		/// <summary>
		/// Notifies the template files changed. This method is called from AOP Document Analyzer Server
		/// who is watching for physical file changes in Template
		/// </summary>
		/// <param name="watcherServiceId">The watcher service id.</param>
		/// <param name="templatePaths">The list template paths.</param>
		[OperationContract]
		void NotifyTemplateFilesChanged(int watcherServiceId, List<string> templatePaths);

		/// <summary>
		/// Insert a new line in JobQueue for generating a jpeg preview
		/// for a JobDocument and also can save the file as indesign document, depends on what jobType u pass in
		/// </summary>
		/// <param name="jobId">The Job Bo</param>
		/// <param name="jobDocId">The document to generate preview for</param>
		/// <param name="jobType">Type of the job.</param>
		/// <returns>The Queue Id</returns>
		[OperationContract]
		int InsertPreviewRequest(int jobId, int jobDocId, AOP_JobType jobType);

		/// <summary>
		/// Insert a new line in JobQueue for generating a jpeg preview
		/// for a JobDocument and also can save the file as indesign document, depends on what jobType u pass in
		/// </summary>
		/// <param name="jobId">The Job Bo</param>
		/// <param name="jobDocId">The document to generate preview for</param>
		/// <param name="jobType">Type of the job.</param>
		/// <param name="userSessionChannelId">User Browser Uniqueu GUID to Contact via PubNub</param>		
		/// <returns>The Queue Id</returns>
		[OperationContract]
		int InsertPreviewRequestWithPubNubSessionId(int jobId, int jobDocId, AOP_JobType jobType, string userSessionChannelId);

		/// <summary>
		/// Insert a new line in JobQueue for generating a jpeg preview
		/// for a JobDocument and also can save the file as indesign document, depends on what jobType u pass in
		/// </summary>
		/// <param name="jobId">The Job Bo</param>
		/// <param name="jobDocId">The document to generate preview for</param>
		/// <param name="jobType">Type of the job.</param>
		/// <param name="userSessionChannelId">User Browser Uniqueu GUID to Contact via PubNub</param>
		/// <param name="isFormValid">if the document form has valid data</param>
		/// <returns>The Queue Id</returns>
		[OperationContract]
		int InsertPreviewRequestWithPubNubSessionIdV2(int jobId, int jobDocId, AOP_JobType jobType, string userSessionChannelId, bool isFormValid);

		/// <summary>
		/// Get the queue entity from JobQueue table
		/// </summary>
		/// <param name="queueId">The QueueId</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns><see cref="T:AOP_JobQueue"/> entity</returns>
		[OperationContract]
		AOP_JobQueue GetQueue(int queueId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the items in the queue by status id.
		/// </summary>
		/// <returns>List of AOP_JobQueue</returns>
		[OperationContract]
		List<AOP_JobQueue> GetQueueItemByStatus(AOP_QueueStatusList status, List<EntityRelations> loadOptions);

		/// <summary>
		/// Updates the queue.
		/// </summary>
		/// <param name="jobQueue">The job queue.</param>
		/// <returns>AOP_JobQueue Object</returns>
		[OperationContract]
		AOP_JobQueue UpdateJobQueue(AOP_JobQueue jobQueue);

		/// <summary>
		/// Updates the queue status.
		/// </summary>
		/// <param name="queueId">The queue id.</param>
		/// <param name="status">The status.</param>
		/// <param name="msg">The MSG.</param>
		[OperationContract]
		void UpdateQueueStatus(int queueId, AOP_QueueStatusList status, string msg);

		/// <summary>
		/// Locks the job queue with Concurrency Check to make sure no other connector perform the job
		/// </summary>
		/// <param name="jobQueue">The job queue.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_JobQueue LockJobQueue(AOP_JobQueue jobQueue);
		#endregion

		#region AOP Job Related
		/// <summary>
		/// Gets the job documents by job id.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns>List of JobDocument Entities</returns>
		[OperationContract]
		List<AOP_JobDocument> GetJobDocumentsByJobId(int jobId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the job document by job document id.
		/// </summary>
		/// <param name="jobDocumentId">The job document id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_JobDocument GetJobDocumentByJobDocumentId(int jobDocumentId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the job documents by design status.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <param name="statusId">The status id.</param>
		/// <param name="loadOptions">The load options.</param>
		/// <returns></returns>
		[OperationContract]
		List<AOP_JobDocument> GetJobDocumentsByDesignStatus(int jobId, int statusId, List<EntityRelations> loadOptions);

		/// <summary>
		/// Gets the job document ids in queue by job id.
		/// </summary>
		/// <param name="jobId">The job id.</param>
		/// <returns></returns>
		[OperationContract]
		List<int> GetJobDocumentIdsInQueueByJobId(int jobId);

		/// <summary>
		/// Updates/Deletes/Inserts the job document list.
		/// </summary>
		/// <param name="jobs">The jobs.</param>
		/// <returns>the refreshed list</returns>
		[OperationContract]
		List<AOP_JobDocument> UpdateJobDocument(List<AOP_JobDocument> jobs);

		/// <summary>
		/// Saves the working document XML model.
		/// </summary>
		/// <param name="jobDocId">The job doc id.</param>
		/// <param name="workingDocModel">The working doc model.</param>
		/// <param name="isItTempModel">if set to <c>true</c> [is it temp model].</param>
		[OperationContract]
		void SaveDocumentModel(int jobDocId, string workingDocModel, bool isItTempModel);

		/// <summary>
		/// Creates the temporary preview job.
		/// </summary>
		/// <param name="templateId">The template id.</param>
		/// <returns>AOP_JobDocument</returns>
		[OperationContract]
		AOP_JobDocument CreateTemporaryPreviewJob(int templateId);


		/// <summary>
		/// Gets the linked AOP job.
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <returns></returns>
		[OperationContract]
		AOP_JobDocument GetLinkedAOPJob(int orderId);

        /// <summary>
        /// Creates the Express order temporary preview job.
        /// </summary>
        /// <param name="templateProductId">The template product id.</param>
        /// <param name="tempClientID">The temp Client id.</param>
        /// <returns></returns>
        [OperationContract]
        AOP_JobDocument CreateExpressOrderTemporaryPreviewJob(int templateProductId, int tempClientID);

        /// <summary>
        /// Update AOP Job Document.
        /// </summary>
        /// <param name="jobDocId">The job document id.</param>
        /// <param name="status">The status value.</param>
        /// <returns>AOP_JobDocument</returns>
        [OperationContract]
        AOP_JobDocument UpdateAOPJobDocument(int jobDocId, int status);


        /// <summary>
        /// Saves the document model and insert preview request.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <param name="jobDocId">The job doc id.</param>
        /// <param name="workingDocModel">The working doc model.</param>
        /// <param name="jobType">Type of the job.</param>
        /// <param name="userSessionChannelId">The user session channel id.</param>
        /// <param name="isFormValid">if set to <c>true</c> [is form valid].</param>
        /// <returns></returns>
        [OperationContract]
        int SaveDocumentModelAndInsertPreviewRequest(int jobId, int jobDocId, string workingDocModel, AOP_JobType jobType, string userSessionChannelId, bool isFormValid);
		#endregion

        /// <summary>
        /// UpdateSingleJobDocument
        /// </summary>
        /// <param name="job">The job document</param>
        /// <returns></returns>
        [OperationContract]
        void UpdateSingleJobDocument(AOP_JobDocument job);

        /// <summary>
        /// SaveSingleJobDocument
        /// </summary>
        /// <param name="jobDocId">The job document id</param>
        /// <param name="workingDocModel">The workingDocModel</param>
        /// <param name="tempDate">The tempDate</param>
        /// <returns></returns>
        [OperationContract]
        void SaveSingleJobDocument(int jobDocId, string workingDocModel, DateTime tempDate);

        /// <summary>
		/// Updates the template detail. This operation can be used to insert and delete Template Detail.
		/// </summary>
        /// <param name="tempDetail">The template detail.</param>
		/// <returns></returns>
        [OperationContract]
        TemplateDetail UpdateTemplateDetail(TemplateDetail tempDetail);
	}
}
