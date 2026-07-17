using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Utility;
using Abc.OnlineBL.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Abc.OnlineBL.Service.Implementation
{
    public partial class DesignService : IDesignService
    {
        public List<Design_JobQueue> GetDesignQueueItemByStatus(AOP_QueueStatusList status, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    var items = from jq in ctx.Design_JobQueues
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

        public Design_JobQueue LockDesignJobQueue(Design_JobQueue jobQueue)
        {
            Guard.ArgumentNotNull(jobQueue, "jobQueue");
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    bool? success = false;
                    var jq = ctx.Design_LockJob(jobQueue.QueueId, jobQueue.ConnectorId, jobQueue.TS, ref success, jobQueue.ConnectorChannelId);
                    if (success.HasValue && success.Value == true)
                        return jq.FirstOrDefault();
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "LockDesignJobQueue");
                throw;
            }
        }

        public Design_Document GetDesignDocumentByDocumentId(int documentId, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    var item = (from jd in ctx.Design_Documents
                                where jd.DesignDocumentId == documentId
                                select jd).FirstOrDefault();

                    return item;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GetDesignDocumentByDocumentId({0})", documentId);
                throw;
            }
        }

        public void UpdateDesignQueueStatus(int queueId, AOP_QueueStatusList status, string msg)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    var queue = ctx.Design_JobQueues.SingleOrDefault(q => q.QueueId == queueId);
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
                Logger.Exception(ex, "UpdateDesignQueueStatus");
                throw;
            }
        }

        public void SaveDesignDocumentModel(int designDocId, string workingDocModel, bool isItTempModel)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var jd = ctx.Design_Documents.SingleOrDefault(j => j.DesignDocumentId == designDocId);
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
                Logger.Exception(ex, "SaveDesignDocumentModel ({0})", designDocId);
                throw;
            }
        }

        public Design_Document GetDesignDocumentByDesignDocumentId(int designDocumentId, List<EntityRelations> loadOptions)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    var item = (from jd in ctx.Design_Documents
                                where jd.DesignDocumentId == designDocumentId
                                select jd).FirstOrDefault();

                    return item;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GetDesignDocumentByDesignDocumentId({0})", designDocumentId);
                throw;
            }
        }

        public List<Design_Document> UpdateDesignDocument(List<Design_Document> jobs)
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
                        var templateDetails = (from dd in ctx.Design_Documents
                                               where dd.DesignDocumentId == newJob.DesignDocumentId
                                               select new { PropertyId = dd.PropertyId, ClientId = dd.Property.ClientId, TemplateId = dd.AOP_TemplateProduct.TemplateId }).FirstOrDefault();

                        //Logger.Warn("After getting template");

                        string sourceTemplateFile = AOP_Template.GetTemplatePath(templateDetails.TemplateId);
                        sourceTemplateFile = Path.Combine(ServiceConfig.AOP_TEMPLATE_ROOT_DIR.TrimEnd('\\'), sourceTemplateFile);

                        string destTemplatePath = GetDesignDocumentPath(ServiceConfig.DESIGN_TEMPLATE_DOCUMENT_DIR,
                            templateDetails.ClientId, templateDetails.PropertyId, newJob.DesignDocumentId);
                        string destTemplateFile = GetDesignDocumentTemplatePath(ServiceConfig.DESIGN_TEMPLATE_DOCUMENT_DIR,
                            templateDetails.ClientId, templateDetails.PropertyId, newJob.DesignDocumentId);

                        IFile file = VirtualFileSystemFactory.GetFile();

                        if (!file.ExistsDir(destTemplatePath))
                            file.CreateDir(destTemplatePath);

                        file.Copy(sourceTemplateFile, destTemplateFile, true);
                        //Logger.Warn("After move template");

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

        #region GetDesignDocumentPath
        /// <summary>
        /// Gets the job document path. E.g. RootPath\ClientId\JobId\JobDocumentId\
        /// </summary>
        /// <param name="documentRootPath">The document root path.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="jobId">The job id.</param>
        /// <param name="jobDocumentId">The job document id.</param>
        /// <returns></returns>
        private static string GetDesignDocumentPath(string documentRootPath, int clientId, int propertyId, int jobDocumentId)
        {
            string templateFilePath = string.Format("{0}\\{1}\\{2}\\{3}", documentRootPath.TrimEnd('\\'), clientId, propertyId, jobDocumentId);
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
        private static string GetDesignDocumentTemplatePath(string documentRootPath, int clientId, int propertyId, int jobDocumentId)
        {
            string templateFile = Path.Combine(GetDesignDocumentPath(documentRootPath, clientId, propertyId, jobDocumentId), jobDocumentId + ".indt");
            return templateFile;
        }
        #endregion
    }
}
