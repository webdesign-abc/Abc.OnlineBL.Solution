using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.DataStore;
using System.Xml.Linq;
using Abc.OnlineBL.Entities.Enums;
using System.IO;
using Abc.OnlineBL.VirtualFileSystem;
using Abc.OnlineBL.Utility;
using Dom = Abc.OnlinePublication.Common.DOM;

namespace Abc.OnlineBL.Service.Implementation
{
	public partial class AOPService
	{
		public int InsertPreviewRequest(int jobId, int jobDocId, AOP_JobType jobType)
		{
			try
			{
				AOP_JobQueue queue = new AOP_JobQueue();
				queue.JobDocumentId = jobDocId;
				queue.StatusId = (int)AOP_QueueStatusList.Waiting_In_Queue;
				queue.Action = Enum.GetName(typeof(AOP_JobType), jobType);
				queue.DateCreated = DateTime.Now;
				queue.DateFinished = null;
				queue.StatusMessage = string.Empty;

				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					ctx.AOP_JobQueues.InsertOnSubmit(queue);
					ctx.SubmitChanges();
				}

				return queue.QueueId;
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "InsertPreviewRequest(int jobId:{0}, int jobDocId:{1}, AOP_JobType jobType:{2})", jobId, jobDocId, jobType);
				throw;
			}
		}

		public int InsertPreviewRequestWithPubNubSessionId(int jobId, int jobDocId, AOP_JobType jobType, string userSessionChannelId)
		{
			try
			{
				AOP_JobQueue queue = new AOP_JobQueue();
				queue.JobDocumentId = jobDocId;
				queue.StatusId = (int)AOP_QueueStatusList.Waiting_In_Queue;
				queue.Action = Enum.GetName(typeof(AOP_JobType), jobType);
				queue.DateCreated = DateTime.Now;
				queue.DateFinished = null;
				queue.StatusMessage = string.Empty;
				queue.UserChannelId = userSessionChannelId;				

				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					ctx.AOP_JobQueues.InsertOnSubmit(queue);
					ctx.SubmitChanges();
				}

				return queue.QueueId;
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "InsertPreviewRequestWithPubNubSessionId(int jobId:{0}, int jobDocId:{1}, AOP_JobType jobType:{2})", jobId, jobDocId, jobType);
				throw;
			}
		}

		public int InsertPreviewRequestWithPubNubSessionIdV2(int jobId, int jobDocId, AOP_JobType jobType, string userSessionChannelId, bool isFormValid)
		{
			try
			{
				AOP_JobQueue queue = new AOP_JobQueue();
				queue.JobDocumentId = jobDocId;
				queue.StatusId = (int)AOP_QueueStatusList.Waiting_In_Queue;
				queue.Action = Enum.GetName(typeof(AOP_JobType), jobType);
				queue.DateCreated = DateTime.Now;
				queue.DateFinished = null;
				queue.StatusMessage = string.Empty;
				queue.UserChannelId = userSessionChannelId;
				queue.IsFormValid = isFormValid;

				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					ctx.AOP_JobQueues.InsertOnSubmit(queue);
					ctx.SubmitChanges();
				}

				return queue.QueueId;
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "InsertPreviewRequestWithPubNubSessionId(int jobId:{0}, int jobDocId:{1}, AOP_JobType jobType:{2})", jobId, jobDocId, jobType);
				throw;
			}
		}

		public AOP_JobQueue GetQueue(int queueId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					return ctx.AOP_JobQueues.SingleOrDefault(q => q.QueueId == queueId);
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetQueue queueId:{0}", queueId);
				throw;
			}
		}

		public AOP_JobQueue UpdateJobQueue(AOP_JobQueue jobQueue)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					jobQueue.SynchroniseWithDataContext(ctx);
					ctx.SubmitChanges();
				}
				return jobQueue;
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "UpdateJobQueue");
				throw;
			}
		}

		public void UpdateQueueStatus(int queueId, AOP_QueueStatusList status, string msg)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					var queue = ctx.AOP_JobQueues.SingleOrDefault(q => q.QueueId == queueId);
					if (queue != null)
					{
						queue.StatusId = (int)status;
						queue.StatusMessage = msg;
                        queue.DateFinished = DateTime.Now;
						ctx.SubmitChanges();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "UpdateQueueStatus");
				throw;
			}
		}

		public List<AOP_JobQueue> GetQueueItemByStatus(AOP_QueueStatusList status, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					var items = from jq in ctx.AOP_JobQueues
								where jq.StatusId == (int)status
								select jq;

					return items.ToList();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetQueueItemByStatus({0}).", status);
				throw;
			}
		}

		public List<AOP_JobDocument> GetJobDocumentsByJobId(int jobId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					var items = from jd in ctx.AOP_JobDocuments
											where jd.JobId == jobId
											select jd;

					return items.ToList();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetJobDocumentsByJobId({0}).", jobId);
				throw;
			}
		}

		public AOP_JobDocument GetJobDocumentByJobDocumentId(int jobDocumentId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					var item = (from jd in ctx.AOP_JobDocuments
								where jd.JobDocumentId == jobDocumentId
								select jd).FirstOrDefault();

					return item;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetJobDocumentByJobDocumentId({0})", jobDocumentId);
				throw;
			}			
		}

		public List<AOP_JobDocument> GetJobDocumentsByDesignStatus(int jobId, int statusId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					List<AOP_JobDocument> items = (from jd in ctx.AOP_JobDocuments
																				 where jd.JobId == jobId && jd.StatusId == statusId
																				 select jd).ToList();

					return items;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetJobDocumentsByDesignStatus({0}, {1})", jobId, statusId);
				throw;
			}
		}

		public List<int> GetJobDocumentIdsInQueueByJobId(int jobId)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<int> items = (from jd in ctx.AOP_JobDocuments
														 where jd.JobId == jobId
														 select jd.JobDocumentId).ToList();

					return items;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetJobDocumentIdsInQueueByJobId({0})", jobId);
				throw;
			}
		}

		public List<AOP_JobDocument> UpdateJobDocument(List<AOP_JobDocument> jobs)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

                    
					var newJobs = (from j in jobs
								  where j.LINQEntityState == EntityState.New
								  select j).ToList();
					
					jobs.ForEach(jd => jd.SynchroniseWithDataContext(ctx));
					
					ctx.SubmitChanges();

					foreach (var newJob in newJobs)
					{
						var templateDetails = (from jd in ctx.AOP_JobDocuments											   
											   where jd.JobDocumentId == newJob.JobDocumentId
											   select new { OrderId = jd.SafeJobId, ClientId = jd.Order.ClientID, TemplateId = jd.AOP_TemplateProduct.TemplateId }).FirstOrDefault();
                        Logger.Warn("After getting template");

						string sourceTemplateFile = AOP_Template.GetTemplatePath(templateDetails.TemplateId);
						sourceTemplateFile = Path.Combine(ServiceConfig.AOP_TEMPLATE_ROOT_DIR.TrimEnd('\\'), sourceTemplateFile);

						string destTemplatePath = GetJobDocumentPath(ServiceConfig.AOP_TEMPLATE_ROOT_DIR,
							templateDetails.ClientId, templateDetails.OrderId, newJob.JobDocumentId);
						string destTemplateFile = GetJobDocumentTemplatePath(ServiceConfig.AOP_TEMPLATE_ROOT_DIR,
							templateDetails.ClientId, templateDetails.OrderId, newJob.JobDocumentId);

						IFile file = VirtualFileSystemFactory.GetFile();

						if (!file.ExistsDir(destTemplatePath))
							file.CreateDir(destTemplatePath);

						file.Copy(sourceTemplateFile, destTemplateFile, true);
                        Logger.Warn("After move template");

					}

					return jobs;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "UpdateJobDocument");
				throw;
			}			
		}

        public void UpdateSingleJobDocument(AOP_JobDocument job)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    Logger.Warn("Before Sync -- " + job.JobDocumentId + " -- " + job.JobId);

                    job.SynchroniseWithDataContext(ctx);

                    ctx.SubmitChanges();
                    Logger.Warn("After Sync -- " + job.JobDocumentId + " -- " + job.JobId);

                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "UpdateJobDocument");
                throw;
            }
        }

        public void SaveSingleJobDocument(int jobDocId, string workingDocModel, DateTime tempDate)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var jd = ctx.AOP_JobDocuments.SingleOrDefault(j => j.JobDocumentId == jobDocId);
                    if (jd != null)
                    {
                        jd.TemplateDateModifiedStamp = tempDate;
                        jd.WorkingDocumentModel = XElement.Parse(workingDocModel);
                        if (jd.TempDocumentModel != null)
                            jd.TempDocumentModel = jd.WorkingDocumentModel;

                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SaveDocumentModel({0})", jobDocId);
                throw;
            }
        }

		#region GetJobDocumentPath
		/// <summary>
		/// Gets the job document path. E.g. RootPath\ClientId\JobId\JobDocumentId\
		/// </summary>
		/// <param name="documentRootPath">The document root path.</param>
		/// <param name="clientId">The client id.</param>
		/// <param name="jobId">The job id.</param>
		/// <param name="jobDocumentId">The job document id.</param>
		/// <returns></returns>
		private static string GetJobDocumentPath(string documentRootPath, int clientId, int jobId, int jobDocumentId)
		{
			string templateFilePath = string.Format("{0}\\{1}\\{2}\\{3}", documentRootPath.TrimEnd('\\'), clientId, jobId, jobDocumentId);
			return templateFilePath;
		}
		#endregion

		#region GetJobDocumentTemplatePath
		/// <summary>
		/// Gets the job document template path. E.g. RootPath\ClientId\JobId\JobDocumentId\JobDocumentId.indt
		/// </summary>
		/// <param name="documentRootPath">The document root path.</param>
		/// <param name="clientId">The client id.</param>
		/// <param name="jobId">The job id.</param>
		/// <param name="jobDocumentId">The job document id.</param>
		/// <returns></returns>
		private static string GetJobDocumentTemplatePath(string documentRootPath, int clientId, int jobId, int jobDocumentId)
		{
			string templateFile = Path.Combine(GetJobDocumentPath(documentRootPath, clientId, jobId, jobDocumentId), jobDocumentId + ".indt");
			return templateFile;
		}
		#endregion

		public void SaveDocumentModel(int jobDocId, string workingDocModel, bool isItTempModel)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					var jd = ctx.AOP_JobDocuments.SingleOrDefault(j => j.JobDocumentId == jobDocId);
					if (jd != null)
					{
						jd.StatusId = (int)AOP_DocumentStatusList.Design_InComplete;
						if (isItTempModel)
						{
							if (string.IsNullOrEmpty(workingDocModel))
								jd.TempDocumentModel = null;
							else
								jd.TempDocumentModel = XElement.Parse(workingDocModel);
						}
						else
						{
							if (string.IsNullOrEmpty(workingDocModel))
								jd.WorkingDocumentModel = null;
							else
								jd.WorkingDocumentModel = XElement.Parse(workingDocModel);
						}
					}

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "SaveDocumentModel({0})", jobDocId);
				throw;
			}
		}

		public AOP_JobDocument CreateTemporaryPreviewJob(int templateId)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					List<EntityRelations> dataLoad = new List<EntityRelations>();
					dataLoad.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
					ctx.SetDataLoadOptions(dataLoad);

					AOP_TemplateProduct tmp = ctx.AOP_TemplateProducts.FirstOrDefault(tt => tt.TemplateId == templateId);
					if (tmp != null)
					{
						AOP_JobDocument jd = new AOP_JobDocument();
						jd.JobId = null;
						jd.TemplateProductId = tmp.TemplateProductId;
						jd.StatusId = (int)AOP_DocumentStatusList.Design_InComplete;
						jd.WorkingDocumentModel = tmp.AOP_Template.TemplateModel;
						
						jd.TemplateName = tmp.Name;
						jd.TemplateOriginalFileName = Path.GetFileName(tmp.AOP_Template.TemplatePath);
						jd.TemplateDateModifiedStamp = (tmp.AOP_Template.DateModified.HasValue ? tmp.AOP_Template.DateModified : tmp.AOP_Template.DateCreated);

						ctx.AOP_JobDocuments.InsertOnSubmit(jd);

						ctx.SubmitChanges();

						return jd;
					}
				}
				return null;
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "CreateTemporaryPreviewJob({0})", templateId);
				throw;
			}
		}

		public AOP_JobDocument GetLinkedAOPJob(int orderId)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					int propertyId = ctx.PropertyOrders.Single(po => po.OrderId == orderId).PropertyId;

					var oldAOPOrder = (from po in ctx.PropertyOrders
									  join jd in ctx.AOP_JobDocuments on po.OrderId equals jd.JobId
									  where po.PropertyId == propertyId && po.OrderId != orderId
									  orderby jd.JobId
									  select jd).FirstOrDefault();

					//Logger.Warn("Linked Job Id: " + oldAOPOrder.JobId);
					return oldAOPOrder;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetLinkedAOPJob({0})", orderId);
				throw;
			}
		}

		public AOP_JobQueue LockJobQueue(AOP_JobQueue jobQueue)
		{
			Guard.ArgumentNotNull(jobQueue, "jobQueue");
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					bool? success = false;
					var jq = ctx.AOP_LockJob(jobQueue.QueueId, jobQueue.ConnectorId, jobQueue.TS, ref success, jobQueue.ConnectorChannelId);
					if (success.HasValue && success.Value == true)
						return jq.FirstOrDefault();
					else
						return null;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "LockJobQueue");
				throw;
			}
		}

        public AOP_JobDocument CreateExpressOrderTemporaryPreviewJob(int templateProductId, int tempClientID)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    List<EntityRelations> dataLoad = new List<EntityRelations>();
                    dataLoad.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
                    dataLoad.Add(EntityRelations.AOP_TemplateProduct_To_AOP_DocumentPolicy);
                    ctx.SetDataLoadOptions(dataLoad);

                    AOP_TemplateProduct tmp = ctx.AOP_TemplateProducts.FirstOrDefault(tt => tt.TemplateProductId == templateProductId);
                    if (tmp != null)
                    {
                        AOP_JobDocument jd = new AOP_JobDocument();
                        jd.JobId = null;
                        jd.TemplateProductId = tmp.TemplateProductId;
                        jd.StatusId = (int)AOP_DocumentStatusList.Design_InComplete;
                        jd.WorkingDocumentModel = tmp.AOP_Template.TemplateModel;

                        jd.TemplateName = tmp.Name;
                        jd.TemplateOriginalFileName = Path.GetFileName(tmp.AOP_Template.TemplatePath);
                        jd.TemplateDateModifiedStamp = (tmp.AOP_Template.DateModified.HasValue ? tmp.AOP_Template.DateModified : tmp.AOP_Template.DateCreated);

                        //Merge with document policy
                        Dom.Document doc = Dom.Document.Deserialize(jd.WorkingDocumentModel.ToString());
                        Dom.DocumentPolicy pol = Dom.DocumentPolicy.Deserialize(tmp.AOP_DocumentPolicy.PolicyModel.ToString());
                        doc.MergeWithPolicy(pol);
                        doc.ImageRequirement = new Dom.ImageQuality()
                        {
                            MinimumMegaPixels = Convert.ToDecimal(tmp.MinimumMegaPixels),
                            RecommendedMegaPixels = Convert.ToDecimal(tmp.RecommendedMegaPixels)
                        };

                        jd.WorkingDocumentModel = XElement.Parse(Dom.Document.Serialize(doc));

                        //Submit
                        ctx.AOP_JobDocuments.InsertOnSubmit(jd);
                        ctx.SubmitChanges();

                        // Copy template file to the job document folder.
                        string sourceTemplateFile = AOP_Template.GetTemplatePath(tmp.TemplateId);
                        sourceTemplateFile = Path.Combine(ServiceConfig.AOP_TEMPLATE_ROOT_DIR.TrimEnd('\\'), sourceTemplateFile);

                        string destTemplatePath = GetJobDocumentPath(ServiceConfig.AOP_TEMPLATE_ROOT_DIR,
                            tempClientID, 0, jd.JobDocumentId);
                        string destTemplateFile = GetJobDocumentTemplatePath(ServiceConfig.AOP_TEMPLATE_ROOT_DIR,
                            tempClientID, 0, jd.JobDocumentId);

                        IFile file = VirtualFileSystemFactory.GetFile();

                        if (!file.ExistsDir(destTemplatePath))
                            file.CreateDir(destTemplatePath);

                        file.Copy(sourceTemplateFile, destTemplateFile, true);

                        return jd;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "CreateExpressOrderTemporaryPreviewJob({0}, {1})", templateProductId, tempClientID);
                throw;
            }
        }

        public AOP_JobDocument UpdateAOPJobDocument(int jobDocId, int status)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    var queue = ctx.AOP_JobDocuments.SingleOrDefault(q => q.JobDocumentId == jobDocId);
                    if (queue != null)
                    {
                        queue.StatusId = status;
                        ctx.SubmitChanges();
                    }
                    return queue;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "UpdateAOPJobDocument");
                throw;
            }
        }

        public int SaveDocumentModelAndInsertPreviewRequest(int jobId, int jobDocId, string workingDocModel, AOP_JobType jobType, string userSessionChannelId, bool isFormValid)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var jd = ctx.AOP_JobDocuments.SingleOrDefault(j => j.JobDocumentId == jobDocId);
                    if (jd != null)
                    {
                        jd.StatusId = (int)AOP_DocumentStatusList.Design_InComplete;

                        if (string.IsNullOrEmpty(workingDocModel))
                        {
                            jd.TempDocumentModel = null;
                            jd.WorkingDocumentModel = null;
                        }
                        else
                        {
                            jd.TempDocumentModel = XElement.Parse(workingDocModel);
                            jd.WorkingDocumentModel = jd.TempDocumentModel;
                        }
                    }
                    ctx.SubmitChanges();
                }

                AOP_JobQueue queue = new AOP_JobQueue();
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    queue.JobDocumentId = jobDocId;
                    queue.StatusId = (int)AOP_QueueStatusList.Waiting_In_Queue;
                    queue.Action = Enum.GetName(typeof(AOP_JobType), jobType);
                    queue.DateCreated = DateTime.Now;
                    queue.DateFinished = null;
                    queue.StatusMessage = string.Empty;
                    queue.UserChannelId = userSessionChannelId;
                    queue.IsFormValid = isFormValid;

                    ctx.DeferredLoadingEnabled = false;

                    ctx.AOP_JobQueues.InsertOnSubmit(queue);
                    ctx.SubmitChanges();
                }

                return queue.QueueId;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SaveDocumentModelAndInsertPreviewRequest({0})", jobDocId);
                throw;
            }
        }
	}
}
