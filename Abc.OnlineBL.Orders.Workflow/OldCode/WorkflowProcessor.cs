using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abc.AIS.DataStore;
using Abc.AIS.Entities;
using Abc.AIS.Orders.Workflow.Model;
using Abc.AIS.VirtualFileSystem;
using Abc.Business.Entities;
using Abc.Business.Entities.Enums;

namespace Abc.AIS.Orders.Workflow
{
	public class WorkflowProcessor
	{
		#region CreateNewOrder
		public static int CreateNewOrder(NewOrder nOrder)
		{
			if (nOrder == null)
			{
				throw new ArgumentNullException("nOrder");
			}
			try
			{
				int? orderId = 0;
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//Create new order
					ctx.CDAS_NewOrder(nOrder.ClientId, nOrder.LocId, nOrder.Property,
									nOrder.Caption, nOrder.Notes, nOrder.NoBoards,
									nOrder.ErectionNotes, nOrder.SendBy, nOrder.SendTo,
									ref orderId, nOrder.OrderData, nOrder.RefNo,
									nOrder.TransformListing, nOrder.SendSms,
									nOrder.SmsText, nOrder.SmsAgentMobileNo,
									nOrder.SmsNotifyAgent, nOrder.SmsSendEmail,
									nOrder.SmsAgentEmailAdd, nOrder.InddTemplatesAvail,
									nOrder.HasCommunityBoard, nOrder.MMS_Allowed,
									nOrder.PreferredErectionDate, nOrder.PreferredErectionType,
									nOrder.PreferredRemovalDate, nOrder.PreferredRemovalType);

					
					if (!orderId.HasValue)
						return 0;

					//link property id with order id
					ctx.PropertyOrderInsert(nOrder.PropertyId , orderId.Value);

					InsertProducts(nOrder.CartItems, nOrder.ProductTypesToInclude, nOrder.ClientId, orderId.Value, nOrder.ManagerId, ctx);


					return orderId.HasValue ? orderId.Value : 0;

				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'CreateNewOrder'.");
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion
		
		#region GenerateNewOrderEvent
		public static void GenerateNewOrderEvent(NewOrderEvent nOrderEvent)
		{
			if (nOrderEvent == null)
			{
				throw new ArgumentNullException("nOrderEvent");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//Create new order event
					ctx.CDAS_GenNewOrderEvent(nOrderEvent.OrderId, nOrderEvent.ClientId,
											nOrderEvent.HtmlBody, nOrderEvent.HtmlBodyUS,
											nOrderEvent.FileName, nOrderEvent.Prop);

				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GenerateNewOrderEvent'.");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetTemplateProductById
		public static AOP_TemplateProduct GetTemplateProductById(int templateProductId, List<EntityRelations> loadOptions)
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

		#region UpdateJobDocument
		public static List<AOP_JobDocument> UpdateJobDocument(List<AOP_JobDocument> jobs)
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

						string sourceTemplateFile = AOP_Template.GetTemplatePath(templateDetails.TemplateId);
						sourceTemplateFile = Path.Combine(AISConfig.AOP_TEMPLATE_ROOT_DIR.TrimEnd('\\'), sourceTemplateFile);

						string destTemplatePath = GetJobDocumentPath(AISConfig.AOP_TEMPLATE_ROOT_DIR,
							templateDetails.ClientId, templateDetails.OrderId, newJob.JobDocumentId);
						string destTemplateFile = GetJobDocumentTemplatePath(AISConfig.AOP_TEMPLATE_ROOT_DIR,
							templateDetails.ClientId, templateDetails.OrderId, newJob.JobDocumentId);

						IFile file = VirtualFileSystemFactory.GetFile();

						if (!file.ExistsDir(destTemplatePath))
							file.CreateDir(destTemplatePath);

						file.Copy(sourceTemplateFile, destTemplateFile, true);
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
		#endregion

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

		#region CreateNewPhotoOrder
		public static int CreateNewPhotoOrder(NewPhotoOrder nPhotoOrder)
		{
			if (nPhotoOrder == null)
			{
				throw new ArgumentNullException("nPhotoOrder");
			}
			try
			{
				int? photoOrderId = 0;
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//Create new order event
					ctx.CDAS_NewPhotoOrder(nPhotoOrder.ClientId, nPhotoOrder.LocId,
											nPhotoOrder.Property, nPhotoOrder.Caption,
											nPhotoOrder.Notes, nPhotoOrder.ErectionNotes,
											nPhotoOrder.OrderData, nPhotoOrder.RefNo,
											ref photoOrderId, nPhotoOrder.Instructions,
											nPhotoOrder.VendorName, nPhotoOrder.VendorPhone,
											nPhotoOrder.IsKeySafe, nPhotoOrder.IsPickupKeys,
											nPhotoOrder.PhotoContact, nPhotoOrder.HouseFaces,
											nPhotoOrder.Melway, nPhotoOrder.SendBy,
											nPhotoOrder.SendTo);


					//link property id with photo order id
					if (!photoOrderId.HasValue)
						return 0;
					ctx.PropertyOrderInsert(nPhotoOrder.PropertyId, photoOrderId.Value);


					//// Insert Products into Order Details Table
					InsertProducts(nPhotoOrder.CartItems, nPhotoOrder.ProductTypesToInclude, nPhotoOrder.ClientId, photoOrderId.Value, nPhotoOrder.ManagerId, ctx);

					return photoOrderId.HasValue ? photoOrderId.Value : 0;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'CreateNewPhotoOrder'.");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GenerateNewPhotoOrderEvent
		public static void GenerateNewPhotoOrderEvent(NewOrderEvent nOrderEvent)
		{
			if (nOrderEvent == null)
			{
				throw new ArgumentNullException("nOrderEvent");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//Create new order event
					ctx.CDAS_GenPhotoEvent(nOrderEvent.OrderId, nOrderEvent.ClientId,
											nOrderEvent.HtmlBody, nOrderEvent.HtmlBodyUS,
											nOrderEvent.FileName, nOrderEvent.Prop);

				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GenerateNewPhotoOrderEvent'.");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region CreateNewSBOrder
		public static int CreateNewSBOrder(NewStockBoardOrder nSBOrder)
		{
			if (nSBOrder == null)
			{
				throw new ArgumentNullException("nSBOrder");
			}
			try
			{
				int? sbOrderId = 0;
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//Create new order event
					ctx.CDAS_NewSBOrder(nSBOrder.ClientId, nSBOrder.State,
											nSBOrder.Loc, nSBOrder.Property,
											nSBOrder.Caption, nSBOrder.Notes,
											ref sbOrderId, nSBOrder.PreferredErectionDate,
											nSBOrder.PreferredErectionType);

					//LinkWithSBOrderId();
					nSBOrder.SBOrderId = sbOrderId.HasValue ? sbOrderId.Value : 0;
					if (nSBOrder.PropertyId > 0 && nSBOrder.SBOrderId > 0)
					{
						ctx.PropertySBOrderInsert(nSBOrder.PropertyId, nSBOrder.SBOrderId);
					}

					return nSBOrder.SBOrderId;


				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'CreateNewSBOrder'.");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GenerateNewSBOrderEvent
		public static void GenerateNewSBOrderEvent(NewOrderEvent nOrderEvent)
		{
			if (nOrderEvent == null)
			{
				throw new ArgumentNullException("nOrderEvent");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//Create new order event
					ctx.CDAS_GenNewStockOrderEvent(nOrderEvent.OrderId, nOrderEvent.ClientId,
											nOrderEvent.HtmlBody, nOrderEvent.HtmlBodyUS,
											nOrderEvent.FileName, nOrderEvent.Prop);

				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GenerateNewSBOrderEvent'");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region CreateNewOnlineListing
		public static int CreateNewOnlineListing(NewOnlineListing nOnlineListing)
		{
			if (nOnlineListing == null)
			{
				throw new ArgumentNullException("nOnlineListing");
			}
			try
			{
				int? listingId = 0;
				using (AbcDataContext ctx = new AbcDataContext())
				{
					//Create new order event
					ctx.AR_TransformOrder(nOnlineListing.ListingType, nOnlineListing.ClientId,
											nOnlineListing.Ptype, nOnlineListing.LocationId,
											nOnlineListing.PAddress, nOnlineListing.StreetName,
											nOnlineListing.StreetNo, nOnlineListing.UnitNo,
											nOnlineListing.Heading, nOnlineListing.SubHeading,
											nOnlineListing.BodyText, nOnlineListing.InspectionDetails,
											nOnlineListing.AuctionDate, nOnlineListing.AuctionView,
											nOnlineListing.PriceFrom, nOnlineListing.PriceTo,
											nOnlineListing.HidePrice, nOnlineListing.PriceView,
											nOnlineListing.Rent, nOnlineListing.Lease,
											nOnlineListing.Bed, nOnlineListing.Bath,
											nOnlineListing.Car, nOnlineListing.Land,
											nOnlineListing.AvlFrom, nOnlineListing.Display,
											ref listingId, nOnlineListing.ShowAddress,
											nOnlineListing.Price, nOnlineListing.RentPerMonth,
											nOnlineListing.ShowRent, nOnlineListing.Authority,
											nOnlineListing.LandSizeUnit, nOnlineListing.ListingAddedBy);


					//LinkWithSBOrderId();
					nOnlineListing.ListingId = listingId.HasValue ? listingId.Value : 0;

					// Now Insert Inspection Dates
					foreach (Abc.AIS.Orders.Workflow.Model.InspectionDate insDate in nOnlineListing.InspectionDates)
					{
						ctx.AR_InspectionDateInsert(nOnlineListing.ListingId,
													insDate.Date,
													insDate.TimeFrom,
													insDate.TimeTo);
					}

					return nOnlineListing.ListingId;

				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'CreateNewOnlineListing'.");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region private methods
		private static void InsertProducts(List<CartItem> cartItems, List<ProductTypes> productTypesToInclude, int clientId, int orderId, string managerId, AbcDataContext ctx)
		{
			//Insert Product into Other Detail Table
			foreach (CartItem item in cartItems)
			{
				#region Board
				//if the list contains board then coninue
				// Look for a Board item
				if (productTypesToInclude.Contains(ProductTypes.Board) == true)
				{
					if (item is Board)
					{
						Board board = item as Board;

						if (board != null)
						{
							try
							{
								if (board.ProductId > 0)
								{

									InsertProductIntoOrderDetails(ctx, clientId, board.ProductId, board.Qty,
											board.Format, false, board.UserWillDesignOnline, orderId, null);
								}


								InsertOptionalItem(ctx, orderId, item, clientId);

							}
							catch (Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.BOARD_TYPE_ID, board.ProductId });
							}
						}
					}
				}
				#endregion

				#region StockBoard
				if (productTypesToInclude.Contains(ProductTypes.Stockboard) == true)
				{
					if (item is StockBoard)
					{
						StockBoard stockBoard = item as StockBoard;

						if (stockBoard != null)
						{
							try
							{
								if (stockBoard.ProductId > 0)
								{

									InsertProductIntoOrderDetails(ctx, clientId, stockBoard.ProductId, stockBoard.Qty,
											stockBoard.Format, false, stockBoard.UserWillDesignOnline, orderId, null);
								}


								InsertOptionalItem(ctx, orderId, item, clientId);

							}
							catch (Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.STOCK_BOARD_TYPE_ID, stockBoard.ProductId });
							}
						}
					}
				}
				#endregion

				#region Brochure
				if (productTypesToInclude.Contains(ProductTypes.Brochure) == true)
				{
					if (item is Brochure)
					{
						Brochure brochure = item as Brochure;

						if (brochure != null)
						{
							try
							{
								if (brochure.ProductId > 0)
								{
									int quantity = 1;
									if (AISConfig.IS_NZ)
										quantity = brochure.Qty;

									InsertProductIntoOrderDetails(ctx, clientId, brochure.ProductId, quantity,
											brochure.Format, false, brochure.UserWillDesignOnline, orderId, null);


									InsertOptionalItem(ctx, orderId, item, clientId);
								}

							}
							catch (Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.BROCHURE_TYPE_ID, brochure.ProductId });
							}
						}
					}
				}
				#endregion

				#region WindowCard
				if (productTypesToInclude.Contains(ProductTypes.WindowCard) == true)
				{
					if (item is Window)
					{
						Window window = item as Window;

						if (window != null)
						{
							try
							{
								if (window.ProductId > 0)
								{
									InsertProductIntoOrderDetails(ctx, clientId, window.ProductId, window.Qty,
											window.Format, false, window.UserWillDesignOnline, orderId, null);


									InsertOptionalItem(ctx, orderId, item, clientId);
								}

							}
							catch (Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.WC_TYPE_ID, window.ProductId });
							}
						}
					}
				}
				#endregion

				#region Photography
				if (productTypesToInclude.Contains(ProductTypes.Photography) == true)
				{
					if (item is Photo)
					{
						Photo Photo = item as Photo;

						if (Photo != null)
						{
							try
							{
								if (Photo.ProductId > 0)
								{
									InsertProductIntoOrderDetails(ctx, clientId, Photo.ProductId, 1,
											null, false, false, orderId, null);
								}

							}
							catch (Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.PHOTO_TYPE_ID, Photo.ProductId });
							}
						}
					}
				}
				#endregion

				#region SMS On Demand Service
				// Look for a SMS item
				if (productTypesToInclude.Contains(ProductTypes.SmsService) == true)
				{
					if (item is SmsService)
					{
						SmsService service = item as SmsService;
						int productId = 0;

						if (service != null)
						{
							try
							{
								bool hasStockBoard = OrderHasStockBoard(cartItems);
								bool hasBoard = true; // orderDataExchange.AnOrder.OrderHasBoard();

								if (hasBoard && !hasStockBoard)
								{
									productId = AISConfig.SMS_ON_DEMAND_PRODUCT_ID;

									if (service.AllowMMS)
										productId = AISConfig.MMS_ON_DEMAND_PRODUCT_ID;
								}
								else if (!hasBoard && hasStockBoard)
								{
									productId = AISConfig.STOCK_BOARD_SMS_ON_DEMAND_PRODUCT_ID;

									if (service.AllowMMS)
										productId = AISConfig.STOCK_BOARD_MMS_ON_DEMAND_PRODUCT_ID;
								}

								if (productId > 0)
									InsertProductIntoOrderDetails(ctx, clientId, productId, 1, null, true, false, orderId, null);
							}
							catch (System.Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.SMS_TYPE_ID, productId });
							}
						}
					}
				}
				#endregion

				#region ErectionFee
				if (productTypesToInclude.Contains(ProductTypes.ErectionFee) == true)
				{
					if (item is ErectionFee)
					{
						ErectionFee ef = item as ErectionFee;
						if (ef.ProductId > 0)
						{
							try
							{
								InsertErectionFeeIntoOrderDetails(ctx, ef, orderId, managerId);
							}
							catch (Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.ERECTIONFEE_TYPE_ID, ef.ProductId });
							}
						}
					}
				}
				#endregion

				#region Spotlight
				// Note : Spotlight not being added into the database for NZ yet.
				if (!AISConfig.IS_NZ)
				{
					if (productTypesToInclude.Contains(ProductTypes.Spotlight) == true)
					{
						if (item is SpotLight)
						{
							try
							{
								SpotLight spotlight = item as SpotLight;

								if (item.ProductId > 0)
									InsertProductIntoOrderDetails(ctx, clientId, item.ProductId, spotlight.Quantity, null, false, false, orderId, null);
							}
							catch (System.Exception ex)
							{
								Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderId, OrderData.SPOTLIGHT_TYPE_ID, item.ProductId });
							}
						}
					}
				}
				#endregion
			}
		}

		#region InsertErectionFeeIntoOrderDetails
		private static void InsertErectionFeeIntoOrderDetails(AbcDataContext ctx, ErectionFee erectionFee, int tempOrderId, string managerId)
		{

			int? orderDetailId = 0;
			ctx.CDAS_OrderDetailInsertErectionFee(tempOrderId, AISConfig.ERECTION_FEE_PRODUCT_ID,
										erectionFee.Amount, (erectionFee.InstallationType == BoardInstallationType.HigherThanFirstLevel),
										managerId, ref orderDetailId);
		}
		#endregion

		#region OrderHasStockBoard
		// If contains any StockBoard
		public static bool OrderHasStockBoard(List<CartItem> cartItems)
		{
			foreach (CartItem anItem in cartItems)
			{
				if (anItem.ProductCategory == ProductTypes.Stockboard)
					return true;
			}
			return false;
		}
		#endregion

		private static void InsertProductIntoOrderDetails(AbcDataContext ctx, int clientId,
				int productId, int qty, string format, bool isProductSmsService, bool userDesignOnline, int tempOrderId, string itemNote)
		{
			int? orderDetailId = 0;
			ctx.CDAS_OrderDetailInsert(clientId, tempOrderId,
										productId, qty,
										format, ref orderDetailId,
										isProductSmsService, userDesignOnline,
										itemNote);
		}

		private static void InsertOptionalItem(AbcDataContext ctx, int tempOrderId, CartItem item, int clientId)
		{
			//Now Insert Optional Items Inside Products
			if (item.OptionalItems.Count > 0)
			{
				bool isManagerAccount = false;
				isManagerAccount = (from a in ctx.Accounts
									join o in ctx.Orders on a.AccountID equals o.BillTo
									where o.OrderID == tempOrderId
									select a.ManagerAcc).FirstOrDefault();

				foreach (OptionalItem opItem in item.OptionalItems)
				{
					if (opItem.MappedProductId > 0 && // we have a corresponding DB ProductId for this
							opItem.ItemSelected == true)  // and Item has been selected
					{
						try
						{
							// Check if this order is billed to a manager account.
							// If it is then skip the inserting.
							if (opItem.ForNonManagerAccountsOnly)
							{
								if (!isManagerAccount)
									InsertProductIntoOrderDetails(ctx, clientId, opItem.MappedProductId, opItem.GetQtyCount(), null, false, item.UserWillDesignOnline, tempOrderId, null);
							}
							else
							{
								InsertProductIntoOrderDetails(ctx, clientId, opItem.MappedProductId, opItem.GetQtyCount(), null, false, item.UserWillDesignOnline, tempOrderId, null);
							}
						}
						catch (System.Exception ex)
						{
							Logger.Exception(ex, string.Format("{0} {1}"), new object[] { tempOrderId, opItem.MappedProductId });
						}
					}
				}
			}
		}

		#endregion

	}
}
