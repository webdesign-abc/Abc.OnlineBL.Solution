using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.DataStore;
using System.Threading;
using Abc.OnlineBL.Entities.Enums;

namespace Abc.OnlineBL.Service.Implementation
{
	public partial class AOPService : IAOPService
	{
		#region SayHello
		public string SayHello(string name)
		{
			string sName = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
			Thread.Sleep(3 * 1000);
			return string.Format("Hello {0}. You are also {1}...", name, sName);
		}
		#endregion

		#region GetDocumentPolicies
		public List<AOP_DocumentPolicy> GetDocumentPolicies(List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.SetDataLoadOptions(loadOptions);

					var query = from dp in ctx.AOP_DocumentPolicies
											select dp;
					return query.ToList();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'GetDocumentPolicies'.");
				throw;
			}
		}
		#endregion

		#region GetTemplateById
		public AOP_Template GetTemplateById(int templateId, bool doNotReturnTemplateModel, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var query = from t in ctx.AOP_Templates
											where t.TemplateId == templateId
											select t;
					AOP_Template template = query.FirstOrDefault();

					if (template != null && doNotReturnTemplateModel)
						template.TemplateModel = null;

					return template;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'GetTemplateById'. TemplateId:{0}", templateId);
				throw;
			}
		}
		#endregion

		#region GetTemplateByPath
		public AOP_Template GetTemplateByPath(string templatePath, bool doNotReturnTemplateModel, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var query = (from t in ctx.AOP_Templates
											 where t.TemplatePath == templatePath
											 select t);
					AOP_Template template = query.FirstOrDefault();

					if (doNotReturnTemplateModel && template != null)
						template.TemplateModel = null;

					return template;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'GetTemplateByPath'. TemplatePath:{0}", templatePath);
				throw;
			}
		}
		#endregion

		#region GetTemplatesByPaths
		public List<AOP_Template> GetTemplatesByPaths(List<string> templatePaths, bool doNotReturnTemplateModel, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//string sLog = string.Empty;
					//System.IO.TextWriter log = new System.IO.StringWriter();
					//ctx.Log = log;

					ctx.DeferredLoadingEnabled = false;

					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var query = (from t in ctx.AOP_Templates
											 where templatePaths.Contains(t.TemplatePath)
											 select t);

					List<AOP_Template> templates = query.ToList();
					if (doNotReturnTemplateModel)
						templates.ForEach(delegate(AOP_Template t) { t.TemplateModel = null; });

					//sLog = log.ToString();
					//Logger.Debug(sLog);

					return templates;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'GetTemplatesByPaths'.");
				throw;
			}
		}
		#endregion

		#region UpdateTemplate
		public AOP_Template UpdateTemplate(AOP_Template template)
		{
			//string sLog = string.Empty;
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//System.IO.TextWriter log = new System.IO.StringWriter();
					//ctx.Log = log;

					ctx.DeferredLoadingEnabled = false;
					template.SynchroniseWithDataContext(ctx);
					ctx.SubmitChanges();

					//sLog = log.ToString();
					//Logger.Debug(sLog);
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'UpdateTemplate'.");
				throw;
			}

			return template;
		}
		#endregion

		#region UpdateTemplatePath
		public void UpdateTemplatePath(int templateId, string templatePath)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.AOPService_UpdateTemplatePath(templateId, templatePath);
					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'UpdateTemplatePath'. TemplateId:{0}, TemplatePath:{1}.", templateId, templatePath);
				throw;
			}
		}
		#endregion

		#region GetTemplateProductById
		public AOP_TemplateProduct GetTemplateProductById(int templateProductId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					AOP_TemplateProduct tp = (from t in ctx.AOP_TemplateProducts
																		where t.TemplateProductId == templateProductId && t.Active
																		select t).FirstOrDefault();

					return tp;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'GetTemplateProductById'. templateProductId:{0}", templateProductId);
				throw;
			}
		}
		#endregion

		#region GetTemplateProductsByJobId
		public List<AOP_TemplateProduct> GetTemplateProductsByJobId(int jobId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//System.IO.TextWriter log = new System.IO.StringWriter();
					//ctx.Log = log;

					ctx.DeferredLoadingEnabled = false;
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var tps = (from t in ctx.AOP_TemplateProducts
										 from jp in t.AOP_JobDocuments where jp.JobId == jobId
										 select t).ToList();

					//Logger.Debug(log.ToString());
					return tps;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'GetTemplateProductsByJobId({0})'.", jobId);
				throw;
			}
		}
		#endregion

		#region GetTemplateProductsForClient
		public List<AOP_TemplateProduct> GetTemplateProductsForClient(int clientId, string type, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					// Determine if client has any custom template or use corporate ones.
					// If client has custom templates it is not allowed to use corporate ones.
					var hasClientTemplate = (from t in ctx.AOP_Templates
																	 where t.ClientId == clientId
																	 select t).Count() > 0;

					if (hasClientTemplate)
					{
						var tps = (from t in ctx.AOP_TemplateProducts
											 where t.AOP_Template.ClientId == clientId && t.Type == type && t.Active
											 select t).ToList();

						return tps;
					}
					else
					{
						var tps = (from t in ctx.AOP_TemplateProducts
											 where t.Type == type && t.Active && t.AOP_Template.ClientId == null &&
											 t.AOP_Template.GroupId == (from c in ctx.Clients where c.ClientID == clientId select c.GroupId).FirstOrDefault()
											 select t).ToList();

						return tps;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'GetTemplateProductsForClient({0}, {1}, loadOptions)'.", clientId, type);
				throw;
			}
		}
		#endregion

		#region GetTemplateProductsMatchingPriceList
		public List<AOP_TemplateProduct> GetTemplateProductsMatchingPriceList(int clientId, string type, List<EntityRelations> loadOptions)
		{
			//Business rules only require type of Billboard to match with the price list.
			if (type.ToLower() != "billboard")
				return GetTemplateProductsForClient(clientId, type, loadOptions);

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//System.IO.TextWriter log = new System.IO.StringWriter();
					//ctx.Log = log;

					ctx.DeferredLoadingEnabled = false;
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					// Determine if client has any custom templates or use corporate ones.
					// If client has custom templates it is not allowed to use corporate ones.
					var hasClientTemplate = (from t in ctx.AOP_Templates
																	 where t.ClientId == clientId
																	 select t).Count() > 0;

					// Build query to select all products that a client can have from the pricelist to find
					// the matching TemplateProducts.
					// This query will not be executed until we call functions like .Count(), .Single(), .ToList().....
					var priceListQuery = from p in ctx.Products
															 join pscd in ctx.ProductSizeCodeDetails on new { SizeCode = p.SizeCode, TypeId = p.TypeID } equals new { SizeCode = pscd.SizeCode, TypeId = pscd.ProductTypeId }
															 join pld in ctx.PriceListDetails on p.ProductID equals pld.ProductID
															 join pl in ctx.PriceLists on pld.PriceListID equals pl.PriceListID
															 join c in ctx.Clients on pl.PriceListID equals c.PriceListID
															 join pt in ctx.ProductTypes on p.TypeID equals pt.TypeID
															 where c.ClientID == clientId && p.Active == true && pt.Type == type && p.SizeCode != null && p.ContentType != null
															 select new
															 {
																 Type = pt.Type,
																 SizeCode = pscd.SizeCodeOnWeb,
																 ContentType = p.ContentType,
																 FrameType = p.FrameType,
																 Format = p.Format
															 };

					IQueryable<int> productTemplateIds = null;

					// These queries in if/else block will not be executed
					// Because we haven't enumerated thru it and it is used in another query below.
					if (!hasClientTemplate)
					{
						// Buid a query to get TemplateProductIds of the matching products for corporate templates
						productTemplateIds = (from pq in priceListQuery
																	join t in ctx.AOP_TemplateProducts on new { pq.Type, pq.SizeCode, pq.ContentType, FrameType = (pq.FrameType == null) ? "" : pq.FrameType } equals new { t.Type, t.SizeCode, t.ContentType, FrameType = (t.FrameType == null) ? "" : t.FrameType }
																	where t.Active == true && t.AOP_Template.ClientId == null && t.AOP_Template.GroupId == (from c in ctx.Clients where c.ClientID == clientId select c.GroupId).FirstOrDefault()
																	select t.TemplateProductId).Distinct();
					}
					else
					{
						// Buid a query to get TemplateProductIds of the matching products for client templates
						productTemplateIds = (from pq in priceListQuery
																	join t in ctx.AOP_TemplateProducts on new { pq.Type, pq.SizeCode, pq.ContentType, FrameType = (pq.FrameType == null) ? "" : pq.FrameType } equals new { t.Type, t.SizeCode, t.ContentType, FrameType = (t.FrameType == null) ? "" : t.FrameType }
																	where t.Active == true && t.AOP_Template.ClientId == clientId
																	select t.TemplateProductId).Distinct();
					}

					// This is another way to do. If we can use grouping in the previous statement then we dont have to do this.
					var templateProducts = (from tp in ctx.AOP_TemplateProducts
																	where (from id in productTemplateIds select id).Contains(tp.TemplateProductId)
																	select tp).ToList();

					//Logger.Debug(log.ToString());
					return templateProducts;
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetTemplateProductsMatchingPriceList({0}, {1}, loadOptions)", clientId, type);
				throw;
			}
		}
		#endregion

		#region AnalyzeQueue
		public AOP_AnalyzeQueue GetAnalyzeQueue(int queueId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					return ctx.AOP_AnalyzeQueues.FirstOrDefault(q => q.QueueId == queueId);
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetAnalyzeQueue({0})", queueId);
				throw;
			}
		}

		public List<AOP_AnalyzeQueue> GetAnalyzeQueueByStatus(AOP_QueueStatusList status, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					var queuedItems = from q in ctx.AOP_AnalyzeQueues
														where q.StatusId == (int)status
														select q;

					return queuedItems.ToList();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "GetAnalyzeQueueByStatus");
				throw;
			}
		}

		public AOP_AnalyzeQueue UpdateAnalyzeQueue(AOP_AnalyzeQueue analyzeQueue)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					// If the same template is already in the queue waiting to be processed
					// then don't need to add to the queue again
					if (analyzeQueue.LINQEntityState == EntityState.New)
					{
						List<int> processPendingStatus = new List<int>();
						processPendingStatus.Add((int)AOP_QueueStatusList.Waiting_In_Queue); // Waiting
						processPendingStatus.Add((int)AOP_QueueStatusList.Job_In_Progress); // Being processed

						var queuedItems = from q in ctx.AOP_AnalyzeQueues
															where q.TemplateId == analyzeQueue.TemplateId &&
															processPendingStatus.Contains(q.StatusId)
															select q;

						if (queuedItems.Count() > 0)
							return queuedItems.FirstOrDefault();
					}

					//check if parent entity is also been modified then sync from parent
					//else we don't get the change of parent entity
					if (analyzeQueue.AOP_Template != null && analyzeQueue.AOP_Template.LINQEntityState == EntityState.Modified)
					{
						analyzeQueue.AOP_Template.SynchroniseWithDataContext(ctx);
					}
					else
					{
						analyzeQueue.SynchroniseWithDataContext(ctx);
					}
					
					//analyzeQueue.SetAsUpdateOnSubmit(true);
					//analyzeQueue.AOP_Template.SetAsUpdateOnSubmit();

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error occured in 'UpdateAnalyzeQueue'.");
				throw;
			}

			return analyzeQueue;
		}

		public void NotifyTemplateFilesChanged(int watcherServiceId, List<string> templatePaths)
		{
			if (templatePaths == null)
				return;
			if (templatePaths.Count == 0)
				return;

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<values>");
			foreach (var templatePath in templatePaths)
			{
				sb.AppendFormat("\t<value><![CDATA[{0}]]></value>\r\n", templatePath);
			}
			sb.AppendLine("</values>");

			using (AbcDataContext ctx = new AbcDataContext())
			{
				ctx.AOPService_Insert_AnalyzeQueue(watcherServiceId, sb.ToString());
			}
		}
		#endregion

        #region UpdateTemplateDetail
        public TemplateDetail UpdateTemplateDetail(TemplateDetail tempDetail)
        {
            //string sLog = string.Empty;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    //System.IO.TextWriter log = new System.IO.StringWriter();
                    //ctx.Log = log;

                    ctx.DeferredLoadingEnabled = false;
                    tempDetail.SynchroniseWithDataContext(ctx);
                    ctx.SubmitChanges();

                    //sLog = log.ToString();
                    //Logger.Debug(sLog);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'UpdateTemplateDetail'.");
                throw;
            }

            return tempDetail;
        }
        #endregion
	}
}
