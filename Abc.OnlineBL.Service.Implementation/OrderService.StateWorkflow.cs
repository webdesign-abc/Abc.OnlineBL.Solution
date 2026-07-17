using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using System.ServiceModel;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Service.Implementation.BusinessLogic;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Service.Implementation.Model;
using Abc.OnlineBL.VirtualFileSystem;
using System.IO;
using Abc.OnlineBL.Entities.Model.OnlineOrder;

namespace Abc.OnlineBL.Service.Implementation
{
	public partial class OrderService
	{
		#region GetWorkflowAlerts
		public List<WorkflowAlert> GetWorkflowAlerts(int? orderId, int? orderDetailsId, int? odPackageContentId, List<EntityRelations> loadOptions)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				ctx.DeferredLoadingEnabled = false;
				ctx.SetDataLoadOptions(loadOptions);

				List<WorkflowAlert> alerts = null;

				if (orderId != null && orderDetailsId == null && odPackageContentId == null)
				{
					alerts = (from wfa in ctx.WorkflowAlerts
										where wfa.OrderId == orderId && wfa.Handled == false
										select wfa).ToList();
				}
				else if (orderId != null && orderDetailsId != null && odPackageContentId == null)
				{
					alerts = (from wfa in ctx.WorkflowAlerts
										where wfa.OrderId == orderId && wfa.OrderDetailsId == orderDetailsId && wfa.Handled == false
										select wfa).ToList();
				}
				else if (orderId != null && orderId != null && odPackageContentId != null)
				{
					alerts = (from wfa in ctx.WorkflowAlerts
										where wfa.OrderId == orderId && wfa.OrderDetailsId == orderDetailsId &&
											wfa.ODPackageContentId == odPackageContentId && wfa.Handled == false
										select wfa).ToList();
				}

				return alerts;
			}
		}
		#endregion

		#region ClearWorkflowAlert
		public void ClearWorkflowAlert(int alertId, string handleBy)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				WorkflowAlert alert = (from wfa in ctx.WorkflowAlerts
															 where wfa.AlertId == alertId
															 select wfa).Single();
				alert.Handled = true;
				alert.HandleOn = DateTime.Now;
				alert.HandleBy = handleBy;

				ctx.SubmitChanges();
			}
		}
		#endregion

		#region UpdateOrderStatus
		public void UpdateOrderStatus(Abc.OnlineBL.Entities.Model.OrderTrackingEventParameter orderTrackingEventParameter, Abc.OnlineBL.Entities.Enums.WorkflowStates nextState, bool alsoROnlineBLeAlert)
		{
			string nextStateName = nextState.ToString();
			using (AbcDataContext ctx = new AbcDataContext())
			{
				var order = (from o in ctx.Orders
										 where o.OrderID == orderTrackingEventParameter.OrderId
										 select o).FirstOrDefault();

				bool isTransitionValid = StateMappingFactory.Current.GetStateMapping(0).CheckStateTransition(order.CurrentStatus, nextStateName);
				if (!isTransitionValid)
					throw new Exception(GetInvalidStateMessage(order.CurrentStatus, nextStateName, "Orders.OrderID", order.OrderID, -1));

				order.CurrentStatus = nextStateName;
				ctx.Workflow_LogHistory(order.OrderID, null, null, nextStateName, orderTrackingEventParameter.Message, orderTrackingEventParameter.LoggedBy);

				if (alsoROnlineBLeAlert)
				{
					// number 1 is to notify client
					ctx.Workflow_ROnlineBLeAlert(order.OrderID, null, null, 1, nextStateName, orderTrackingEventParameter.Message, orderTrackingEventParameter.LoggedBy);
				}

				ctx.SubmitChanges();
			}
		}
		#endregion

		#region WorkflowRollbackStatus
		public void WorkflowRollbackStatus(int id, string idType)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				ctx.Workflow_RollbackStatus(id, idType);
				ctx.SubmitChanges();
			}
		}
		#endregion

		#region Despatch
		public void DespatchProducts(OrderTrackingEventParameter orderTrackingEventParameter, int productTypeId)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				var desp = (from dd in ctx.DespatchDetails
							where dd.OrderID == orderTrackingEventParameter.OrderId
							select dd).FirstOrDefault();

                if (desp == null)
                    throw new Exception("Order with Id '" + orderTrackingEventParameter.OrderId.ToString() + "' does not exist.");

				if (productTypeId == ProductTypes.BillBoard)
				{
					if (desp.BoardsDespatched == null)
					{
						ctx.ABCWRKFLOW_DespatchOrder(orderTrackingEventParameter.OrderId, desp.DateDespatched, DateTime.Now, desp.BrochuresDespatched, desp.OthersDespatched);
					}
					else
					{
						throw new Exception("Boards already Despatched");
					}
				}
				else if (productTypeId == ProductTypes.Brochure)
				{
					if (desp.BrochuresDespatched == null)
					{
						ctx.ABCWRKFLOW_DespatchOrder(orderTrackingEventParameter.OrderId, desp.DateDespatched, desp.BoardsDespatched, DateTime.Now, desp.OthersDespatched);
					}
					else
					{
						throw new Exception("Brochure already Despatched");
					}					
				}
				else if (productTypeId == ProductTypes.Other)
				{
					if (desp.OthersDespatched == null)
					{
						ctx.ABCWRKFLOW_DespatchOrder(orderTrackingEventParameter.OrderId, desp.DateDespatched, desp.BoardsDespatched, desp.BrochuresDespatched, DateTime.Now);
					}
					else
					{
						throw new Exception("Other Products already Despatched");
					}						
				}
			}

			UpdateItemStatus(orderTrackingEventParameter, productTypeId, WorkflowStates.Despatched);
		}

		public void DespatchWholeOrder(OrderTrackingEventParameter orderTrackingEventParameter)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				var desp = (from dd in ctx.DespatchDetails
							where dd.OrderID == orderTrackingEventParameter.OrderId
							select dd).FirstOrDefault();

				if (desp.DateDespatched == null)
				{
					ctx.ABCWRKFLOW_DespatchOrder(orderTrackingEventParameter.OrderId, DateTime.Now, desp.BoardsDespatched, desp.BrochuresDespatched, desp.OthersDespatched);
				}
				else
				{
					throw new Exception("Order already Despatched");
				}
			}

			UpdateWholeOrderStatus(orderTrackingEventParameter, WorkflowStates.Despatched);
		}
		#endregion

		#region Approve
		public void ApproveProducts(OrderTrackingEventParameter orderTrackingEventParameter, int productTypeId)
		{
			UpdateItemStatus(orderTrackingEventParameter, productTypeId, WorkflowStates.Approved);
		}

		public void ApproveWholeOrder(OrderTrackingEventParameter orderTrackingEventParameter)
		{
			UpdateWholeOrderStatus(orderTrackingEventParameter, WorkflowStates.Approved);
		}
		#endregion

		#region UpdateOrderItemsStatus
		public void UpdateOrderItemsStatus(OrderTrackingEventParameter orderTrackingEventParameter, WorkflowStates nextState)
		{
			string nextStateName = nextState.ToString();
			using (AbcDataContext ctx = new AbcDataContext())
			{
				foreach (var orderDetailsId in orderTrackingEventParameter.SelectedOrderDetailIds)
				{
					var odEntity = (from od in ctx.OrderDetails
													where od.OrderDetailsID == orderDetailsId
													select od).FirstOrDefault();

					int typeId = odEntity.Product.TypeID;
					StateMapping stateMapping = StateMappingFactory.Current.GetStateMapping(typeId);
					if (stateMapping == null)
						throw new Exception(string.Format("There is no StateMapping for productTypeId:{0}, OrderDetailsId:{1}", typeId, orderDetailsId));

					bool isTransitionValid = stateMapping.CheckStateTransition(odEntity.CurrentStatus, nextStateName);
					if (!isTransitionValid)
						throw new Exception(GetInvalidStateMessage(odEntity.CurrentStatus, nextStateName, "OrderDetails.OrderDetailsID", orderDetailsId, odEntity.Product.TypeID));

					odEntity.CurrentStatus = nextStateName;
					
					ctx.Workflow_LogHistory(orderTrackingEventParameter.OrderId, orderDetailsId, null, nextStateName, orderTrackingEventParameter.Message, orderTrackingEventParameter.LoggedBy);
				}

				foreach (var packageContentId in orderTrackingEventParameter.SelectedODPackageContentIds)
				{
					var odEntity = (from od in ctx.ODPackageContents
													where od.ID == packageContentId
													select od).FirstOrDefault();
					////TODO: check condition here..
					int typeId = odEntity.PackageContent.TypeId;// .OrderDetail.Product.TypeID;
					StateMapping stateMapping = StateMappingFactory.Current.GetStateMapping(typeId);
					if (stateMapping == null)
						throw new Exception(string.Format("There is no StateMapping for productTypeId:{0}, ODPackageContents.ID:{1}", typeId, packageContentId));

					bool isTransitionValid = stateMapping.CheckStateTransition(odEntity.CurrentStatus, nextStateName);
					if (!isTransitionValid)
						throw new Exception(GetInvalidStateMessage(odEntity.CurrentStatus, nextStateName, "ODPackageContents.ID", packageContentId, odEntity.OrderDetail.Product.TypeID));

					odEntity.CurrentStatus = nextStateName;
					
					ctx.Workflow_LogHistory(orderTrackingEventParameter.OrderId, odEntity.OrderDetailsID, packageContentId, nextStateName, orderTrackingEventParameter.Message, orderTrackingEventParameter.LoggedBy);
				}

				ctx.SubmitChanges();
			}
		}
		#endregion

		#region UpdateItemStatus
		private void UpdateItemStatus(OrderTrackingEventParameter evp, int productTypeId, WorkflowStates workflowState)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				List<OrderDetail> products = (from bb in ctx.OrderDetails
																			where bb.OrderID == evp.OrderId
																			select bb).ToList();

				foreach (OrderDetail item in products)
				{
					// If we need to dispatch Boards then also despatch the BoardPackages
					if (productTypeId == ProductTypes.BillBoard)
					{
						if (item.ODPackageContents.Count > 0)
						{
							foreach (var odContent in item.ODPackageContents)
							{
								if (item.Product.TypeID == ProductTypes.BoardPackages)
									evp.SelectedODPackageContentIds.Add(odContent.ID);
							}
						}

						if (item.Product.TypeID == ProductTypes.BillBoard || item.Product.TypeID == ProductTypes.BoardPackages)
						{
							evp.SelectedOrderDetailIds.Add(item.OrderDetailsID);
						}
					}
					else if (productTypeId == ProductTypes.Brochure)
					{
						// If we need to dispatch Brochures then also despatch the OtherPackages
						if (item.ODPackageContents.Count > 0)
						{
							foreach (var odContent in item.ODPackageContents)
							{
								if (item.Product.TypeID == ProductTypes.OtherPackages)
									evp.SelectedODPackageContentIds.Add(odContent.ID);
							}
						}
						else
						{
							if (item.Product.TypeID == ProductTypes.Brochure || item.Product.TypeID == ProductTypes.OtherPackages)
							{
								evp.SelectedOrderDetailIds.Add(item.OrderDetailsID);
							}
						}
					}
					else if (productTypeId == ProductTypes.Other)
					{
						// If we need to dispatch Brochures then also despatch the OtherPackages
						if (item.ODPackageContents.Count > 0)
						{
							foreach (var odContent in item.ODPackageContents)
							{
								if (item.Product.TypeID == ProductTypes.OtherPackages)
									evp.SelectedODPackageContentIds.Add(odContent.ID);
							}
						}
						else
						{
							if (item.Product.TypeID != ProductTypes.BillBoard && item.Product.TypeID != ProductTypes.Brochure)
							{
								evp.SelectedOrderDetailIds.Add(item.OrderDetailsID);
							}
						}
					}
				}
			}

			UpdateOrderItemsStatus(evp, workflowState);
		}
		#endregion

		#region WorkflowLogHistory
		public void WorkflowLogHistory(OrderTrackingEventParameter trackingParameter, string outgoingStateName)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				ctx.Workflow_LogHistory(trackingParameter.OrderId, trackingParameter.OrderDetailsId, trackingParameter.ODPackageContentId,
					outgoingStateName, trackingParameter.Message, trackingParameter.LoggedBy);
				ctx.SubmitChanges();
			}
		}
		#endregion

		#region GetInvalidStateMessage
		private string GetInvalidStateMessage(string currentState, string nextStateName, string itemType, int itemId, int productTypeId)
		{
			string mess = string.Format("Invalid State Transition from '{0}' to '{1}'. ItemType:{2} ItemId:{3}, ProductTypeId:{4}",
				currentState, nextStateName, itemType, itemId, productTypeId);
			return mess;
		}
		#endregion

		#region UpdateWholeOrderStatus
		private void UpdateWholeOrderStatus(OrderTrackingEventParameter evp, WorkflowStates workflowState)
		{
			using (AbcDataContext ctx = new AbcDataContext())
			{
				List<OrderDetail> products = (from bb in ctx.OrderDetails
																			where bb.OrderID == evp.OrderId
																			select bb).ToList();

				// Maybe we need to check to see if all items are despatched
				// then set the order state to despatched instead of setting all item state to despatched.
				foreach (OrderDetail item in products)
				{
					if (item.ODPackageContents.Count > 0)
					{
						foreach (var odContent in item.ODPackageContents)
						{
							evp.SelectedODPackageContentIds.Add(odContent.ID);
						}
					}
					else
					{
						evp.SelectedOrderDetailIds.Add(item.OrderDetailsID);
					}
				}
			}

			UpdateOrderItemsStatus(evp, workflowState);
			UpdateOrderStatus(evp, workflowState, false);
		}
		#endregion

        #region UpdateB2BOrder
        public void UpdateB2BOrder(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            //Regional SB order 
            if (orderID > 90000000)
            {
                return;
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    MaterialDetail od = ctx.MaterialDetails.SingleOrDefault(o => o.OrderID == orderID);

                    od.TextReceived = DateTime.Now;
                    od.ArtworkReceived = DateTime.Now;

                    ctx.SubmitChanges();

                    Order ord = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    var cpref = ctx.ClientsPrefs.SingleOrDefault(x => x.ClientId == ord.ClientID && x.PrefID == ClientsPref.ApproveB2BOrder);
                    if (cpref != null)
                    {
                        if (cpref.BitValue.HasValue && cpref.BitValue.Value == true && !OrderHasStockboard(orderID))
                        {
                            if(string.IsNullOrEmpty(ord.Notes))
                                ord.Notes = "APPROVED";
                            else
                                ord.Notes += " APPROVED";
                        }
                    }

                    ord.Notes = ord.Notes + " B2B Order";

                    ctx.SubmitChanges();

                    try
                    {
                        ProofDetail pr = ctx.ProofDetails.SingleOrDefault(p => p.OrderID == orderID);
                        pr.ArtistID = 73; //test set to 52 //NZ set to 53
                        ctx.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, string.Format("Could not set B2B artist to this order {0}", orderID));
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateB2BOrder'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region UpdateB2BOrderCaption
        public void UpdateB2BOrderCaption(int orderID, string caption)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    if (string.IsNullOrEmpty(od.Caption))
                        od.Caption = caption;
                    else
                    {
                        if (!od.Caption.Contains(caption))
                        {
                            od.Caption = caption + " - " + od.Caption;
                        }
                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateB2BOrderCaption'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region InsertAbcB2BPrintQueue
        public void InsertAbcB2BPrintQueue(AbcB2BPrintQueue abcB2BPrintQueue)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.AbcB2BPrintQueues.InsertOnSubmit(abcB2BPrintQueue);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'InsertAbcB2BPrintQueue'");
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region UpdateAbcB2BPrintQueue
        public void UpdateAbcB2BPrintQueue(AbcB2BPrintQueue abcB2BPrintQueue)
        {
            if (abcB2BPrintQueue == null)
            {
                throw new ArgumentNullException("abcB2BPrintQueue");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    string orientation = abcB2BPrintQueue.StatusMessage.Substring(0, 1);
                    string statusMessage = abcB2BPrintQueue.StatusMessage.Substring(1, abcB2BPrintQueue.StatusMessage.Length - 1);

                    AbcB2BPrintQueue q = ctx.AbcB2BPrintQueues.SingleOrDefault(o => o.QueueId == abcB2BPrintQueue.QueueId);

                    q.StatusId = abcB2BPrintQueue.StatusId;
                    q.DateFinished = abcB2BPrintQueue.DateFinished;
                    q.StatusMessage = statusMessage;

                    ctx.SubmitChanges();

                    Order ord = ctx.Orders.SingleOrDefault(or => or.OrderID == abcB2BPrintQueue.OrderId);
                    ord.Notes = ord.Notes + " -- " + statusMessage;

                    ctx.SubmitChanges();

                    List<OrderDetail> ods = (from o in ctx.OrderDetails
                                             join p in ctx.Products on o.ProductID equals p.ProductID
                                                       where o.OrderID == abcB2BPrintQueue.OrderId &&
                                                       o.ProductID == abcB2BPrintQueue.ProductId &&
                                                       (p.TypeID != ProductTypes.StockboardOverlay && p.TypeID != ProductTypes.Corflute && p.TypeID != ProductTypes.Overlay)  
                                                       select o).ToList();
                    foreach (OrderDetail od in ods)
                    {
                        if (orientation == "P")
                            od.Layout = "Portrait";
                        else
                            od.Layout = "Landscape";

                        ctx.SubmitChanges();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'abcB2BPrintQueue'. orderID:{0}", abcB2BPrintQueue.OrderId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetAbcB2BPrintQueue
        public AbcB2BPrintQueue GetAbcB2BPrintQueue()
        {
            try
            {
                
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var q = (from queue in ctx.AbcB2BPrintQueues
                               orderby queue.QueueId
                               where queue.StatusId == 0
                               select queue).FirstOrDefault();
                    return q;
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetAbcB2BPrintQueue()");
                throw oEx;
            }
        } 
        #endregion

        #region GetB2BProduct
        public Product GetB2BProduct(int productID)
        {
            try
            {
                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.Product_To_ProductType);

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    var p = (from pro in ctx.Products
                             where pro.ProductID == productID
                             select pro).FirstOrDefault();
                    return p;
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetB2BProduct()");
                throw oEx;
            }
        }
        #endregion

        #region HasAllB2BPrintQueueLayupSuccess
        public bool HasAllB2BPrintQueueLayupSuccess(int orderID)
        {
            try
            {

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var q = from queue in ctx.AbcB2BPrintQueues
                             where queue.OrderId == orderID
                             select queue;
                    return q.All(qu => qu.StatusId == 10);
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in HasAllB2BPrintQueueLayupSuccess()");
                throw oEx;
            }
        }
        #endregion

        #region AddB2BInstalationFiles
        public void AddB2BInstalationFiles(int orderID, string installationFiles)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(installationFiles))
            {
                return;
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    OrderOtherDetail odd = ctx.OrderOtherDetails.SingleOrDefault(o => o.OrderId == orderID);

                    if (odd != null)
                    {
                        odd.InstallFile = installationFiles;
                    }
                    else
                    {
                        odd = new OrderOtherDetail();
                        odd.OrderId = orderID;
                        odd.InstallFile = installationFiles;
                        ctx.OrderOtherDetails.InsertOnSubmit(odd);
                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'AddB2BInstalationFiles'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region OrderHasOnlineListing
        public bool OrderHasOnlineListing(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var query = from o in ctx.Orders
                                join ood in ctx.OrderOtherDetails on o.OrderID equals ood.OrderId
                                where o.OrderID == orderID && ood.TransformListing == true
                                select o;

                    return (query.FirstOrDefault() != null);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'OrderHasOnlineListing'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region OrderHasStockboard
        public bool OrderHasStockboard(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var query = from od in ctx.OrderDetails
                                join p in ctx.Products on od.ProductID equals p.ProductID
                                where od.OrderID == orderID && p.TypeID == ProductTypes.Stockboard
                                select od;

                    return (query.FirstOrDefault() != null);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'OrderHasStockboard'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region IsStockBoardApproved
        public bool IsStockBoardApproved(int orderId)
        {
            if (orderId <= 0) throw new ArgumentNullException("orderId");

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from proof in ctx.ProofDetails
                                where proof.OrderID == orderId && proof.DateAppBoards.HasValue
                                select proof;
                    return query.FirstOrDefault() != null;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsStockBoardApproved'. orderID:{0}", orderId);
                Logger.Exception(ex, message);
                throw;
            }
        } 
        #endregion

        #region GetServiceAvailability
        public ServiceAvailability GetServiceAvailability(string state, string suburb)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException("state");
            }
            if (string.IsNullOrEmpty(suburb))
            {
                throw new ArgumentNullException("suburb");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    var dl = ctx.ClientNationalDeliveryLocations.Where(loc => loc.Suburb.ToUpper() == suburb.Trim().ToUpper() && loc.State.ToUpper() == state.Trim().ToUpper()).FirstOrDefault();
                    if (dl != null)
                    {

                        if (dl.DeliveryRegion.ToUpper().Contains("ZONE 1"))
                            return ServiceAvailability.Yes;
                    }
                    else
                    {
                        return ServiceAvailability.Yes;
                    }
                    return ServiceAvailability.No;
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetServiceAvailability'. state:{0}, suburb{1}", suburb);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region SendServiceAvailabilityOption
        public void SendServiceAvailabilityOption(RegionalPropertyRequest rpRequest)
        {
            if(rpRequest == null)
                throw new ArgumentNullException("rpRequest null");
            if (rpRequest.ClientId <= 0) throw new ArgumentNullException("clientId");

            if (string.IsNullOrEmpty(rpRequest.ContactName))
            {
                throw new ArgumentNullException("contactName");
            }
            if (string.IsNullOrEmpty(rpRequest.ContactNumber))
            {
                throw new ArgumentNullException("contactNumber");
            }
            if (string.IsNullOrEmpty(rpRequest.ContactEmail))
            {
                throw new ArgumentNullException("contactEmail");
            } 
            if (string.IsNullOrEmpty(rpRequest.Notes))
            {
                throw new ArgumentNullException("Notes");
            }

            string propertyAddress = string.Empty;

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Client_To_Manager);
                    loadOptions.Add(EntityRelations.Client_To_PriceList);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == rpRequest.ClientId);


                    if (cl != null)
                    {
                        rpRequest.CreatedOn = DateTime.Now;
                        ctx.RegionalPropertyRequests.InsertOnSubmit(rpRequest);
                        ctx.SubmitChanges();

                        if (rpRequest.RPRId > 0)
                        {
                            if (!string.IsNullOrEmpty(rpRequest.UnitNo))
                                propertyAddress = "Unit " + rpRequest.UnitNo + " / " + rpRequest.StreetNo + " " + rpRequest.StreetName + " " + rpRequest.Suburb + ", " + rpRequest.State;
                            else
                                propertyAddress = rpRequest.StreetNo + " " + rpRequest.StreetName + " " + rpRequest.Suburb + ", " + rpRequest.State;

                            //ROnlineBLe an Event to send email notification to Admin
                            int eventID = EventSettings.ServiceLocationRequest;
                            string sub = "Abc Notification: Client would like to be serviced on regional location (Client no: " + rpRequest.ClientId + ") - (Request No: " + rpRequest.RPRId + ")";
                            string xmlData = @"<EVENT>
							<AgentName>" + cl.ClientName.Replace("&", "&amp;") + @"</AgentName>
							<AgentOffice>" + cl.Office.Replace("&", "&amp;") + @"</AgentOffice>
                            <PAddress>" + propertyAddress.Replace("&", "&amp;") + @"</PAddress>
							<ContactName>" + rpRequest.ContactName.Replace("&", "&amp;") + @"</ContactName>
							<ContactNumber>" + rpRequest.ContactNumber.Replace("&", "&amp;") + @"</ContactNumber>
							<ContactEmail>" + rpRequest.ContactEmail.Replace("&", "&amp;") + @"</ContactEmail>
							<ReqNotes>" + rpRequest.Notes.Replace("&", "&amp;") + @"</ReqNotes>
							<ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
							</EVENT>";

                            string textData = "Client would like to be serviced on regional location (Client No: " + rpRequest.ClientId + ") - (Request No: " + rpRequest.RPRId + ")";
                            string source = "OnlineBL_OrderService_SendServiceAvailabilityOption";

                            ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, null, null, null, null, source, String.Empty);
                            ctx.SubmitChanges();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'SendServiceAvailabilityOption'. propertyAddress:{0}, clientId{1}", propertyAddress, rpRequest.ClientId);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region AddServiceQueue
        public void AddServiceQueue(string reportName, string reportParameter, string emailAddress, string emailCCAddress, string emailBCCAddress, string source)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.FrontOffice_ServiceQueueAdd(reportName,
                        reportParameter,
                        0,
                        1,
                        emailAddress,
                        null,
                        emailCCAddress,
                        emailBCCAddress,
                        source,
                        3);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, String.Format("Error occured in 'AddServiceQueue' reportParameter:{0}", reportParameter));
                throw;
            }
        }
        #endregion

        #region GetProofImageFiles
        public List<string> GetProofImageFiles(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                string text = string.Empty;
                List<string> fileList = new List<string>();

                IFile vfile = VirtualFileSystemFactory.GetFile();

                //check proofing_01 server first
                if (vfile.ExistsDir(ServiceConfig.PROOF_IMAGE_DIR))
                {
                    string[] jpgFiles = vfile.GetFiles(ServiceConfig.PROOF_IMAGE_DIR, orderID + "*.jpg");

                    foreach (string file in jpgFiles)
                    {
                        if (Path.GetFileName(file).StartsWith(orderID.ToString()))
                        {
                            fileList.Add(Path.GetFileName(file));
                        }
                    }
                }

                return fileList;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetProofImageFiles'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region OrderApplicableForTwelveMonthsRemovalOrExtension
        public bool OrderApplicableForTwelveMonthsRemovalOrExtension(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var query = from od in ctx.BoardRemovalProcessOrders
                                where od.OrderID == orderID
                                select od;

                    return (query.FirstOrDefault() != null);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'OrderApplicableForTwelveMonthsRemovalOrExtension'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                return false;
            }
        }

        #endregion

        #region ProcessBoardExtensionOrder
        public Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessBoardExtensionOrder(int orderID, string requestBy, string extraNotes)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("OrderID");
            }

            int orderId = 0, photoOrderId = 0, stockId = 0, listingId = 0;
            bool productNotFound = false; 
            string message = string.Empty;
            Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ret = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse();

            OnlinePropertyOrder onlinePropertyOrder = new OnlinePropertyOrder();

            try
            {

                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.Order_To_Client);
                loadOptions.Add(EntityRelations.Order_To_PropertyOrder);
                loadOptions.Add(EntityRelations.PropertyOrder_To_Property);
                loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                loadOptions.Add(EntityRelations.Order_To_OrderOtherInfo);
                loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                Order oldOrder = GetOrderById(orderID, loadOptions);

                //check old order has link order then return
                if (oldOrder != null && oldOrder.DespatchDetail != null)
                {
                    if (oldOrder.DespatchDetail.DateRemovalRequested.HasValue)
                    {
                        message = string.Format("OrderID: {0} - already have Board Removal requested", orderID);
                    }
                    else if (oldOrder.DespatchDetail.BoardExtentOrderID.HasValue)
                    {
                        message = string.Format("OrderID: {0} - already have extra", orderID);
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        Logger.Warn(message);

                        ret.OrderHasError = true;
                        ret.ErrorMessage = message;
                        return ret;
                    }
                }

                //Build up cartitem
                List<CartItem> cartItemList = new List<CartItem>();

                //loop through products and create each cart item
                if (oldOrder.OrderDetails != null)
                {

                    foreach (var item in oldOrder.OrderDetails)
                    {
                        if (item.Product != null && item.Product.TypeID == ProductTypes.Stockboard)
                        {
                            int cartItemId = 1;
                            int itemQty = 1;

                            if (item.ParentID.HasValue)
                            {
                                //pack
                                Logger.Warn("SB pack: " + item.ParentID.Value.ToString());

                                OnlineProduct op = new OnlineProduct();
                                op.ProductId = ServiceConfig.INSTALLATION_EXTENSION_STOCK_BOARD_PRODUCT_ID;

                                op.TypeId = ProductTypes.Other;
                                op.ProductName = "Installation Extension of Stock Board";

                                op.ProductPrice = (decimal)0.00;
                                using (AbcDataContext ctx = new AbcDataContext())
                                {
                                    var priceListDetail = (from pld in ctx.PriceListDetails
                                                     where pld.PriceListID == oldOrder.Client.PriceListID && pld.ProductID == item.ProductID
                                                     select pld).FirstOrDefault();
                                    if (priceListDetail != null)
                                    {
                                        op.ProductPrice = (priceListDetail.ProductPrice * item.Qty * (decimal)1.1);
                                    }

                                    if (op.ProductPrice == 0)
                                    {
                                        var query = (from pp in ctx.ProductPricings
                                                        where pp.PricingID == ServiceConfig.VIC_STANDARD_PRODUCT_PRICING_ID && pp.ProductID == item.ProductID
                                                        select pp).FirstOrDefault();
                                        if (query != null)
                                        {
                                            op.ProductPrice = (query.Price * item.Qty * (decimal)1.1);
                                        }
                                    }
                                }

                                if (op.ProductPrice == 0)
                                {
                                    productNotFound = true;
                                }

                                CartItem cartItem = new CartItem(cartItemId++, op, itemQty);
                                cartItemList.Add(cartItem);
                            }
                            else
                            {
                                Logger.Warn("SB not in pack");

                                OnlineProduct op = new OnlineProduct();
                                op.ProductId = ServiceConfig.INSTALLATION_EXTENSION_STOCK_BOARD_PRODUCT_ID;

                                op.TypeId = ProductTypes.Other;
                                op.ProductName = "Installation Extension of Stock Board";

                                op.ProductPrice = item.Total;

                                if (op.ProductPrice == 0)
                                {
                                    using (AbcDataContext ctx = new AbcDataContext())
                                    {
                                        var priceListDetail = (from pld in ctx.PriceListDetails
                                                     where pld.PriceListID == oldOrder.Client.PriceListID && pld.ProductID == item.ProductID
                                                     select pld).FirstOrDefault();
                                        if (priceListDetail != null)
                                        {
                                            op.ProductPrice = (priceListDetail.ProductPrice * item.Qty * (decimal)1.1);
                                        }

                                        if (op.ProductPrice == 0)
                                        {
                                            var productPricing = (from pp in ctx.ProductPricings
                                                         where pp.PricingID == ServiceConfig.VIC_STANDARD_PRODUCT_PRICING_ID && pp.ProductID == item.ProductID
                                                         select pp).FirstOrDefault();
                                            if (productPricing != null)
                                            {
                                                op.ProductPrice = (productPricing.Price * item.Qty * (decimal)1.1);
                                            }
                                        }
                                    }

                                    if (op.ProductPrice == 0)
                                    {
                                        productNotFound = true;
                                    }
                                }

                                CartItem cartItem = new CartItem(cartItemId++, op, itemQty);
                                cartItemList.Add(cartItem);
                            }
                            
                        }
                        if (item.Product != null && item.Product.TypeID == ProductTypes.BillBoard)
                        {
                            int cartItemId = 1;
                            int itemQty = 1;

                            if (item.ParentID.HasValue)
                            {
                                //pack
                                Logger.Warn("B pack: " + item.ParentID.Value.ToString());

                                OnlineProduct op = new OnlineProduct();
                                op.ProductId = ServiceConfig.INSTALLATION_EXTENSION_TEXT_PHOTO_BOARD_PRODUCT_ID;

                                op.ProductPrice = (decimal)0.00;
                                using (AbcDataContext ctx = new AbcDataContext())
                                {
                                    var priceListDetail = (from pld in ctx.PriceListDetails
                                                           where pld.PriceListID == oldOrder.Client.PriceListID && pld.ProductID == item.ProductID
                                                           select pld).FirstOrDefault();
                                    if (priceListDetail != null)
                                    {
                                        op.ProductPrice = (priceListDetail.ProductPrice * item.Qty * (decimal)1.1) / 2;
                                    }

                                    if (op.ProductPrice == 0)
                                    {
                                        var productPricing = (from pp in ctx.ProductPricings
                                                              where pp.PricingID == ServiceConfig.VIC_STANDARD_PRODUCT_PRICING_ID && pp.ProductID == item.ProductID
                                                              select pp).FirstOrDefault();
                                        if (productPricing != null)
                                        {
                                            op.ProductPrice = (productPricing.Price * item.Qty * (decimal)1.1) / 2;
                                        }
                                    }
                                }

                                if (op.ProductPrice == 0)
                                {
                                    productNotFound = true;
                                }

                                op.TypeId = ProductTypes.Other;
                                op.ProductName = "Installation Extension of Photo/Text Board";

                                CartItem cartItem = new CartItem(cartItemId++, op, itemQty);
                                cartItemList.Add(cartItem);
                            }
                            else
                            {
                                Logger.Warn("B not in pack");

                                OnlineProduct op = new OnlineProduct();
                                op.ProductId = ServiceConfig.INSTALLATION_EXTENSION_TEXT_PHOTO_BOARD_PRODUCT_ID;

                                op.TypeId = ProductTypes.Other;
                                op.ProductName = "Installation Extension of Photo/Text Board";

                                op.ProductPrice = (item.Total) / 2;

                                if (op.ProductPrice == 0)
                                {
                                    using (AbcDataContext ctx = new AbcDataContext())
                                    {
                                        var priceListDetail = (from pld in ctx.PriceListDetails
                                                               where pld.PriceListID == oldOrder.Client.PriceListID && pld.ProductID == item.ProductID
                                                               select pld).FirstOrDefault();
                                        if (priceListDetail != null)
                                        {
                                            op.ProductPrice = (priceListDetail.ProductPrice * item.Qty * (decimal)1.1) / 2;
                                        }

                                        if (op.ProductPrice == 0)
                                        {
                                            var productPricing = (from pp in ctx.ProductPricings
                                                                  where pp.PricingID == ServiceConfig.VIC_STANDARD_PRODUCT_PRICING_ID && pp.ProductID == item.ProductID
                                                                  select pp).FirstOrDefault();
                                            if (productPricing != null)
                                            {
                                                op.ProductPrice = (productPricing.Price * item.Qty * (decimal)1.1) / 2;
                                            }
                                        }
                                    }

                                    if (op.ProductPrice == 0)
                                    {
                                        productNotFound = true;
                                    }
                                }

                                CartItem cartItem = new CartItem(cartItemId++, op, itemQty);
                                cartItemList.Add(cartItem);
                            }
                            
                        }
                    }
                }

                //if no board on old order
                if (cartItemList.Count == 0)
                {
                    Logger.Warn(message);

                    ret.OrderHasError = true;
                    ret.ErrorMessage = message;
                    return ret;
                }

                onlinePropertyOrder.Cart.AddRange(cartItemList);

                //Order Details
                onlinePropertyOrder.PropertyId = oldOrder.PropertyOrder.PropertyId;
                onlinePropertyOrder.ClientId = oldOrder.ClientID;
                onlinePropertyOrder.OrderType = OrderType.AbcDesign;
                onlinePropertyOrder.IsB2BOrder = false;
                onlinePropertyOrder.CartCreatedOn = DateTime.Now;
                onlinePropertyOrder.IsDIYOrder = false;
                onlinePropertyOrder.IsExpressOrder = true;

                onlinePropertyOrder.ContactDetailName = oldOrder.Client.ClientName;

                onlinePropertyOrder.SendProofBy = oldOrder.SendProofBy;
                onlinePropertyOrder.SendProofTo = oldOrder.SendProofTo;

                onlinePropertyOrder.Notes = "Extend Board Rental for Order " + orderID + " Approved By: " + requestBy;
                onlinePropertyOrder.TextDetails.Heading = "Extend Board Rental for Order " + orderID;

                WFRuntimeHelper runtime = new WFRuntimeHelper(onlinePropertyOrder);
                Abc.OnlineBL.Orders.Workflow.OrderDataExchange orderData = runtime.ExecuteWorkflow();

                if (orderData.OrderId > 0)
                {
                    if (productNotFound)
                    {
                        using (AbcDataContext ctx = new AbcDataContext())
                        {
                            //ROnlineBLe an Event to send email notification to Admin
                            int eventID = EventSettings.BoardRentalExtension;
                            string sub = "Charge required for Extended Board Rental Period for Order " + orderData.OrderId;
                            string xmlData = @"<EVENT>
							<OrderID>" + orderData.OrderId + @"</OrderID>
							<AgentName>" + oldOrder.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
							<AgentOffice>" + oldOrder.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
                            <PAddress>" + oldOrder.PropertyOrder.Property.PropertyAddressWithSuburb.Replace("&", "&amp;") + @"</PAddress>
							<Email>" + oldOrder.SendProofTo.Replace("&", "&amp;") + @"</Email>
							<ReceivedOn>" + DateTime.Now.ToString() + @"</ReceivedOn>
							<OriginalOrderNo>" + orderID + @"</OriginalOrderNo>
							</EVENT>";

                            string textData = "Charge required for Extended Board Rental Period for Order " + orderData.OrderId;
                            string source = "OrderService_ProcessBoardExtensionOrder";

                            ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, null, null, null, null, source, String.Empty);
                            ctx.SubmitChanges();

                            var dd = ctx.DespatchDetails.FirstOrDefault(o => o.OrderID == orderID);

                            if (dd != null)
                            {
                                dd.BoardExtentOrderID = orderData.OrderId;
                                ctx.SubmitChanges();
                            }
                        }
                    }
                    else
                    {
                        //Call SP to mark the order approve, despatch and invoice

                        using (AbcDataContext ctx = new AbcDataContext())
                        {
                            ctx.ABCWRKFLOW_UpdateOrder(orderData.OrderId, orderID);

                            ctx.SubmitChanges();
                        }
                    }
                }

                listingId = orderData.ListingId;
                photoOrderId = orderData.PhotoOrderId;
                stockId = orderData.StockId;
                orderId = orderData.OrderId;
            }
            catch (Exception ex)
            {
                ret.OrderHasError = true;
                Logger.Exception(ex, string.Format("ClientID: {0}\r\nPropertyID: {1}\r\n{2}", onlinePropertyOrder.ClientId, onlinePropertyOrder.PropertyId, onlinePropertyOrder.GetHTMLString()));
                SendMail("notifications@photosigns.com.au", OnlineBLConfig.SEND_ERROR_MESSAGE_TO, null, "Error Processing Order", "Order Details:\r\n\r\n" + onlinePropertyOrder.GetHTMLString() + "XML:\r\n" + onlinePropertyOrder.GetXml() + "\r\n");
            }

            ret.PropertyId = onlinePropertyOrder.PropertyId;
            ret.OrderId = orderId;
            ret.PhotoOrderId = photoOrderId;
            ret.StockId = stockId;
            ret.ListingId = listingId;

            return ret;

        }

        #endregion

        #region SendDIYProofByEmail
        public void SendDIYProofByEmail(int clientID, int orderID, string pAddress, string emailAddress, string attachments, string source)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.SP_EventGen_ProofEmail(clientID,
                        orderID,
                        pAddress,
                        emailAddress,
                        attachments,
                        source);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, String.Format("Error occured in 'SendDIYProofByEmail' orderID: {0}, email: {1}", orderID, emailAddress));
                throw;
            }
        }
        #endregion

        #region RequestOneOffDesign
        public void RequestOneOffDesign(int orderID, string requestBy, string reason, string requestFile)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(requestBy))
            {
                throw new ArgumentNullException("requestBy");
            }
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentNullException("reason");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string orderEmail = od.SendProofTo;

                    if (string.IsNullOrEmpty(orderEmail))
                    {
                        orderEmail = od.Client.Email;
                    }

                    requestBy = requestBy.Replace("<", " ");
                    requestBy = requestBy.Replace(">", " ");
                    reason = reason.Replace("<", " ");
                    reason = reason.Replace(">", " ");

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.RequestOneOffCustomDesign;
                    string sub = "Request One-Off Custom Design";
                    string xmlData = @"
								<EVENT>
								<OrderID>" + orderID + @"</OrderID>
								<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
								<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
								<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
								<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
								<ContactName>" + requestBy + @"</ContactName>
								<RequestDetails>" + reason.Replace("&", "&amp;") + @"</RequestDetails>
								</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Request One-Off Custom Design." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Requested By: " + requestBy +
                                            Environment.NewLine +
                                            @"Reason: " + reason;

                    string source = "OrderService_RequestOneOffDesign";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(requestFile) ? requestFile : String.Empty);

                    ctx.SubmitChanges();

                    try
                    {
                        TemplateChangeRequest tempReq = new TemplateChangeRequest();
                        tempReq.OrderID = orderID;
                        tempReq.DateRequest = DateTime.Now;
                        tempReq.RequestDetails = reason;
                        tempReq.RequestedBy = requestBy;
                        tempReq.RequestedContactNumber = "";
                        tempReq.IsCommunityBoard = false;
                        ctx.TemplateChangeRequests.InsertOnSubmit(tempReq);
                        ctx.SubmitChanges();
                    }
                    catch (Exception exc)
                    {
                        Logger.Exception(exc, "Could not save Request Text Overflow changes'. orderID: " + orderID.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RequestOneOffDesign'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region RequestTextOverflow
        public void RequestTextOverflow(int orderID, string requestBy, string requestFile)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(requestBy))
            {
                throw new ArgumentNullException("requestBy");
            }
            
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string orderEmail = od.SendProofTo;

                    if (string.IsNullOrEmpty(orderEmail))
                    {
                        orderEmail = od.Client.Email;
                    }

                    requestBy = requestBy.Replace("<", " ");
                    requestBy = requestBy.Replace(">", " ");

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.RequestTextOverflow;
                    string sub = "Request Text Overflow";
                    string xmlData = @"
								<EVENT>
								<OrderID>" + orderID + @"</OrderID>
								<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
								<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
								<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
								<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
								<ContactName>" + requestBy + @"</ContactName>
								</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Request Request Text Overflow." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Requested By: " + requestBy;

                    string source = "OrderService_RequestTextOverflow";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(requestFile) ? requestFile : String.Empty);

                    ctx.SubmitChanges();

                    try
                    {
                        TemplateChangeRequest tempReq = new TemplateChangeRequest();
                        tempReq.OrderID = orderID;
                        tempReq.DateRequest = DateTime.Now;
                        tempReq.RequestDetails = "Request Text Overflow";
                        tempReq.RequestedBy = requestBy;
                        tempReq.RequestedContactNumber = "";
                        tempReq.IsCommunityBoard = false;
                        ctx.TemplateChangeRequests.InsertOnSubmit(tempReq);
                        ctx.SubmitChanges();
                    }
                    catch (Exception exc)
                    {
                        Logger.Exception(exc, "Could not save Request Text Overflow changes'. orderID: " + orderID.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RequestTextOverflow'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region RequestImageCropping
        public void RequestImageCropping(int orderID, string requestBy, string requestFile)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            if (string.IsNullOrEmpty(requestBy))
            {
                throw new ArgumentNullException("requestBy");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    string orderEmail = od.SendProofTo;

                    if (string.IsNullOrEmpty(orderEmail))
                    {
                        orderEmail = od.Client.Email;
                    }

                    requestBy = requestBy.Replace("<", " ");
                    requestBy = requestBy.Replace(">", " ");

                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.RequestImageCropping;
                    string sub = "Request Image Cropping";
                    string xmlData = @"
								<EVENT>
								<OrderID>" + orderID + @"</OrderID>
								<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + @"</AgentName>
								<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + @"</AgentOffice>
								<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + @"</PAddress>
                                <Email>" + orderEmail.Replace("&", "&amp;") + @"</Email>
								<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
								<ContactName>" + requestBy + @"</ContactName>
								</EVENT>";

                    string textData = @"Notification:" +
                                            Environment.NewLine +
                                            @"Request Request Text Overflow." +
                                            Environment.NewLine +
                                            @"Job Id: " + orderID +
                                            Environment.NewLine +
                                            @"Requested By: " + requestBy;

                    string source = "OrderService_RequestImageCropping";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, od.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(requestFile) ? requestFile : String.Empty);

                    ctx.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'RequestImageCropping'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region GetOrderStatusForApproval
        public ResponseModel GetOrderStatusForApproval(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.Order_To_Client);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

                    ResponseModel ret ;
                    bool payg = false;
                    ClientsPref cpPayg = (from p in ctx.ClientsPrefs
                                        where p.ClientId == od.ClientID && p.PrefID == ClientsPref.PayAsYouGo
                                        select p).FirstOrDefault();
                    if (cpPayg != null && cpPayg.BitValue != null && cpPayg.BitValue == true)
                    {
                        payg = true;
                    }

                    if (od.OrderStatus > 0 && payg)
                    {
                        ctx.BL_OnlineOrderPAYGSendInvoice(orderID);
                        ret = new ResponseModel { ResStatus = 1, Message = "PAYG Client" }; //need to be ResStatus
                    }
                    else
                    {
                        ret = new ResponseModel { ResStatus = 10, Message = "OK" };
                    }

                    return ret;
                    
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetOrderStatusForApproval'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region PhotoOrderApplicableForNinetyDaysHostingRemovalOrExtension
        public bool PhotoOrderApplicableForNinetyDaysHostingRemovalOrExtension(int orderID)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    var query = from od in ctx.VirtualTourHostingOrders
                                where od.PhotoOrderID == orderID
                                && !od.DateDone.HasValue
                                && od.RemoveHosting == false
                                select od;

                    return (query.FirstOrDefault() != null);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'PhotoOrderApplicableForNinetyDaysHostingRemovalOrExtension'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                return false;
            }
        }

        #endregion

        #region ProcessBoardExtensionOrder
        public Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ProcessVirtualWalkThroughExtensionOrder(int orderID, string requestBy, string extraNotes)
        {
            if (orderID <= 0)
            {
                throw new ArgumentNullException("OrderID");
            }

            int orderId = 0, photoOrderId = 0, stockId = 0, listingId = 0;
            string message = string.Empty;

            Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse ret = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse();

            OnlinePropertyOrder onlinePropertyOrder = new OnlinePropertyOrder();

            try
            {

                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.Order_To_Client);
                loadOptions.Add(EntityRelations.Order_To_PropertyOrder);
                loadOptions.Add(EntityRelations.PropertyOrder_To_Property);
                loadOptions.Add(EntityRelations.Order_To_DespatchDetail);
                loadOptions.Add(EntityRelations.Order_To_OrderOtherInfo);
                loadOptions.Add(EntityRelations.Order_To_OrderDetails);
                loadOptions.Add(EntityRelations.OrderDetail_To_Product);
                loadOptions.Add(EntityRelations.Order_To_PhotoOrder);
                Order oldOrder = GetOrderById(orderID, loadOptions);



                //check old order has link order then return
                if (oldOrder != null && oldOrder.PhotoOrder != null)
                {
                    //TODO: check the table for date done, uncomment these two lines to check
                    bool virtualTourLapsed = false;
                    using (AbcDataContext ctx = new AbcDataContext())
                    {

                        var query = (from od in ctx.VirtualTourHostingOrders
                                     where od.PhotoOrderID == orderID
                                     select od).OrderBy(O => O.ID).FirstOrDefault();

                        if (query != null)
                            virtualTourLapsed = query.RemoveHosting;
                    }

                    if (virtualTourLapsed)
                    {
                        message = string.Format("OrderID: {0} - Virtual Walk Through already lapsed", orderID);
                    }
                    else if (oldOrder.PhotoOrder.VirtualExtendOrderID.HasValue)
                    {
                        message = string.Format("OrderID: {0} - Virtual Walk Through already have been extended", orderID);
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        Logger.Warn(message);

                        ret.OrderHasError = true;
                        ret.ErrorMessage = message;
                        return ret;
                    }
                }

                //Build up cartitem
                List<CartItem> cartItemList = new List<CartItem>();

                //loop through products and create each cart item
                if (oldOrder.OrderDetails != null)
                {

                    foreach (var item in oldOrder.OrderDetails)
                    {
                        if (item.Product != null && item.Product.TypeID == ProductTypes.Photography && item.Product.ProductID == 16394)
                        {
                            int cartItemId = 1;
                            int itemQty = 1;

                            OnlineProduct op = new OnlineProduct();
                            op.ProductId = ServiceConfig.VIRTUAL_WALK_THROUGH_RENEWAL_PRODUCT_ID;

                            op.TypeId = ProductTypes.Other;
                            op.ProductName = "3D Virtual Walk Through RENEWAL (1 month)";

                            op.ProductPrice = (decimal)20 / (decimal)1.1;

                            CartItem cartItem = new CartItem(cartItemId++, op, itemQty);
                            cartItemList.Add(cartItem);

                        }
                    }
                }

                //if no Virtual Walk Through on old order
                if (cartItemList.Count == 0)
                {
                    Logger.Warn(message);

                    ret.OrderHasError = true;
                    ret.ErrorMessage = message;
                    return ret;
                }

                onlinePropertyOrder.Cart.AddRange(cartItemList);

                //Order Details
                onlinePropertyOrder.PropertyId = oldOrder.PropertyOrder.PropertyId;
                onlinePropertyOrder.ClientId = oldOrder.ClientID;
                onlinePropertyOrder.OrderType = OrderType.AbcDesign;
                onlinePropertyOrder.IsB2BOrder = false;
                onlinePropertyOrder.CartCreatedOn = DateTime.Now;
                onlinePropertyOrder.IsDIYOrder = false;
                onlinePropertyOrder.IsExpressOrder = true;

                onlinePropertyOrder.ContactDetailName = oldOrder.Client.ClientName;

                onlinePropertyOrder.SendProofBy = oldOrder.SendProofBy;
                onlinePropertyOrder.SendProofTo = oldOrder.SendProofTo;

                onlinePropertyOrder.Notes = "Extend Virtual Walk Through for Order " + orderID + " Approved By: " + requestBy;
                onlinePropertyOrder.TextDetails.Heading = "Extend Virtual Walk Through for Order " + orderID;

                WFRuntimeHelper runtime = new WFRuntimeHelper(onlinePropertyOrder);
                Abc.OnlineBL.Orders.Workflow.OrderDataExchange orderData = runtime.ExecuteWorkflow();

                if (orderData.OrderId > 0)
                {
                    //Call SP to mark the order approve, despatch and invoice

                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        ctx.ABCWRKFLOW_UpdateVirtualOrder(orderData.OrderId, orderID);

                        ctx.SubmitChanges();
                    }
                }

                listingId = orderData.ListingId;
                photoOrderId = orderData.PhotoOrderId;
                stockId = orderData.StockId;
                orderId = orderData.OrderId;
            }
            catch (Exception ex)
            {
                ret.OrderHasError = true;
                Logger.Exception(ex, string.Format("ClientID: {0}\r\nPropertyID: {1}\r\n{2}", onlinePropertyOrder.ClientId, onlinePropertyOrder.PropertyId, onlinePropertyOrder.GetHTMLString()));
                SendMail("notifications@photosigns.com.au", OnlineBLConfig.SEND_ERROR_MESSAGE_TO, null, "Error Processing Order", "Order Details:\r\n\r\n" + onlinePropertyOrder.GetHTMLString() + "XML:\r\n" + onlinePropertyOrder.GetXml() + "\r\n");
            }

            ret.PropertyId = onlinePropertyOrder.PropertyId;
            ret.OrderId = orderId;
            ret.PhotoOrderId = photoOrderId;
            ret.StockId = stockId;
            ret.ListingId = listingId;

            return ret;

        }

        #endregion
    }
}
