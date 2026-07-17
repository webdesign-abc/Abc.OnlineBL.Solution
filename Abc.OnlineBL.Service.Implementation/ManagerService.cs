using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using System.ServiceModel;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities.Model;
using System.Threading;
using Abc.OnlineBL.Utility;
using System.Data.Linq;
using System.Data.SqlClient;
using Abc.OnlineBL.Service.Implementation.Utility;

namespace Abc.OnlineBL.Service.Implementation
{
	public class ManagerService : IManagerService
	{
		#region IManagerService Members

		public string SayHello(string name)
		{
			string sName = "Anonymous User";
			if (OperationContext.Current.ServiceSecurityContext != null)
			{
				if (OperationContext.Current.ServiceSecurityContext.WindowsIdentity != null)
				{
					sName = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
				}
				else if (OperationContext.Current.ServiceSecurityContext.IsAnonymous)
				{
					sName = "Anonymous User";
				}
			}
			string dbResult = string.Empty;
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					var userName = ctx.ExecuteQuery<string>("SELECT '' + SYSTEM_USER").FirstOrDefault();

					if (!string.IsNullOrEmpty(userName))
					{
						dbResult = "DB User is " + userName;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Tring to load Clients Count from SayHello");

			}
			return string.Format("Hello {0}. You are also {1}, My Identity Is {2}, DB Test {3}", name, sName, Thread.CurrentPrincipal.Identity.Name, dbResult);
		}

		public Manager GetManagerById(string manId, List<Abc.OnlineBL.Entities.EntityRelations> loadOptions)
		{
			try
			{
				Guard.ArgumentNotNullOrEmptyString(manId, "manId");

				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					Manager manager = (from c in ctx.Managers
											 where c.ManagerID == manId
											 select c).FirstOrDefault();

					return manager;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetManagerById'. manId:{0} LoadOptions:{1}", manId, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public Truck GetTruckById(int truckId, List<Abc.OnlineBL.Entities.EntityRelations> loadOptions)
		{
			try
			{
				Guard.IsPositive(truckId, "truckId");

				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					Truck ret = (from c in ctx.Trucks
									 where c.TruckID == truckId
									 select c).FirstOrDefault();

					return ret;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetTruckById'. truckId:{0} LoadOptions:{1}", truckId, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

        public Truck GetTruckByUserId(string userId)
        {
            try
            {

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Truck ret = (from c in ctx.Trucks
                                 where c.UserID.ToLower() == userId.ToLower()
                                 select c).FirstOrDefault();

                    return ret;
                }
            }
            catch (Exception ex)
            {

                string message = string.Format("Error occured in 'GetTruckByUserId'. userId:{0} LoadOptions:{1}", userId, "");
                Logger.Exception(ex, message);
                throw;
            }
        }

        public List<Abc.OnlineBL.Entities.Truck> GetAllTrucks(bool active, List<Abc.OnlineBL.Entities.EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var ret = (from c in ctx.Trucks
								  where c.Active == active
								  select c).ToList();

					return ret;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetAllTrucks'. active:{0} LoadOptions:{1}", active, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public List<Abc.OnlineBL.Entities.RunSheet> GetRunsheetByDriver(int truckId, DateTime forDate, List<Abc.OnlineBL.Entities.EntityRelations> loadOptions)
		{
			try
			{
				Guard.IsPositive(truckId, "truckId");

				loadOptions.Add(Abc.OnlineBL.Entities.EntityRelations.Order_To_Client);

				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var ret = (from c in ctx.RunSheets
								  where c.TruckID == truckId && c.DateFor.Date == forDate.Date
								  select c).ToList();

					return ret;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetRunsheetByDriver'. truckId:{0}, forDate:{1}, LoadOptions:{2}", truckId, forDate, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public List<Abc.OnlineBL.Entities.RunSheet> GetRunsheetByManager(string manId, List<Abc.OnlineBL.Entities.EntityRelations> loadOptions)
		{
			try
			{
				Guard.ArgumentNotNullOrEmptyString(manId, "manId");

				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var ret = (from c in ctx.RunSheets
								  where c.ManID == manId
								  select c).ToList();

					return ret;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetRunsheetByManager'. manId:{0}, LoadOptions:{1}", manId, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public List<Abc.OnlineBL.Entities.RunSheet> GetRunsheetByManagerDate(string manId, DateTime forDate, List<Abc.OnlineBL.Entities.EntityRelations> loadOptions)
		{
			try
			{
				Guard.ArgumentNotNullOrEmptyString(manId, "manId");

				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var ret = (from c in ctx.RunSheets
								  where c.ManID == manId && c.DateFor == forDate.Date
								  select c).ToList();

					return ret;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetRunsheetByManager'. manId:{0}, forDate:{1}, LoadOptions:{2}", manId, forDate, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

        /// <summary>
        /// Gets the runsheet details by truck IDN date.
        /// </summary>
        /// <param name="submittedTruckID">The submitted truck ID.</param>
        /// <param name="submittedDate">The submitted date.</param>
        /// <returns></returns>
		public List<Abc.OnlineBL.Entities.Model.RunsheetModel> GetRunsheetDetailsByTruckIDNDate(int submittedTruckID, DateTime submittedDate)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (submittedTruckID > 0 && submittedDate != null)
					{
						DataLoadOptions dl = new DataLoadOptions();

						dl.LoadWith<RunSheet>(e => e.RunSheetDetails);
						dl.LoadWith<RunSheet>(e => e.RunsheetDetailsSBs);
						dl.LoadWith<RunSheet>(e => e.RunsheetDetailsNotes);
						dl.LoadWith<RunSheet>(e => e.Truck);
						dl.LoadWith<RunSheetDetail>(e => e.DespatchDetail);
						dl.LoadWith<RunsheetDetailsSB>(e => e.SB_Order);
						dl.LoadWith<DespatchDetail>(e => e.Order);
                        dl.LoadWith<Order>(e => e.Location);
						dl.LoadWith<Order>(e => e.OrderDetails);
						dl.LoadWith<OrderDetail>(e => e.Product);
                        dl.LoadWith<Location>(e => e.DriverRegions);
                        dl.LoadWith<DriverRegions>(e => e.Truck);
						dl.LoadWith<Order>(e => e.OrderOtherDetail);
						dl.LoadWith<Client>(e => e.ClientID);
                        dl.LoadWith<Order>(e => e.ProofDetail);
                        dl.LoadWith<Order>(e => e.OrderOtherInfo);

						ctx.LoadOptions = dl;
						var runsheet = (    from runsht in ctx.RunSheets
											where runsht.TruckID == submittedTruckID && !runsht.MarkedOff.HasValue
											select runsht).ToList();

						bool dnsError = false;
						int dnsErrorCount = 0;

						List<RunsheetModel> lstRunSheet = new List<RunsheetModel>();
						foreach (RunSheet item in runsheet)
						{
							RunsheetModel objRunshet = new RunsheetModel(item);
							foreach (Abc.OnlineBL.Entities.Model.RunsheetDetail objRD in objRunshet.Details)
							{

                                if (RunsheetDetailSBAdapter.IsStockBoard(objRD.OrderId))
                                    objRD.Board = ctx.fnBoardProdListSB(objRD.OrderId, "\n");
                                else
                                    objRD.Board = ctx.fnBoardProdList(objRD.OrderId, "\n");
								
								//Map ref (try to query 1 and also single out Contractor Driver as he has too many job on his runsheet time out issue)
								if (dnsErrorCount < 2 )
								{
									dnsError = false;
									//UBD.PopulateUBDMapRef(objRD, ref dnsError);
									if (dnsError)
										dnsErrorCount++;
								}
							}

							lstRunSheet.Add(objRunshet);
						}
						return lstRunSheet;
					}
					return null;
				}
			}
			catch (Exception oEx)
			{
				Logger.Exception(oEx, "An error Occured in GetRunsheetDetailsByTruckIDNDate()");
				throw;
			}
		}

		/// <summary>
		/// This service will update the SL of RunsheetDetails 
		/// ex(RunSheetDetail,  RunsheetDetailsSB, RunsheetDetailsNote)according to submittedRunsheetID
		/// And the new SL id will be according to submittedOrderIDList
		/// </summary>
		/// <param name="submittedRunsheetID">Details fo which runshee will be update</param>
		/// <param name="submittedOrderIDList">New reOrder state of details</param>
		/// <returns></returns>
		public bool SetReOrderRunsheetDetailByRunsheetID(int submittedRunsheetID, List<int> submittedOrderIDList)
		{
            // TODO: Make sure this supports sign boards. Does it ALREADY support them?
			bool reOrderDone = false;
			try
			{
				if (submittedRunsheetID > 0 && submittedOrderIDList != null)
				{
					using (AbcDataContext ctx = new AbcDataContext())
					{
						int newSL = 1;
						foreach (int OrderId in submittedOrderIDList)
						{

							var objRSD = ctx.RunSheetDetails.SingleOrDefault(rsd => rsd.OrderID == OrderId && rsd.RID == submittedRunsheetID);
                            //var objRSD = (RunsheetDetailSBAdapter.IsSignBoard(OrderId))
                            //    ? new RunsheetDetailSBAdapter(ctx.RunsheetDetailsSBs.SingleOrDefault(rsd => rsd.OrderID == OrderId && rsd.RID == submittedRunsheetID))
                            //    : new RunsheetDetailSBAdapter(ctx.RunSheetDetails.SingleOrDefault(rsd => rsd.OrderID == OrderId && rsd.RID == submittedRunsheetID));

							if (objRSD != null)
							{
								objRSD.Sl = newSL;
								ctx.SubmitChanges();
							}
							else
							{
								RunsheetDetailsSB objRSDsb = ctx.RunsheetDetailsSBs.SingleOrDefault(rsdSB => rsdSB.RID == submittedRunsheetID && rsdSB.OrderID == OrderId);
								if (objRSDsb != null)
								{
									objRSDsb.Sl = newSL;
									ctx.SubmitChanges();
								}
								else
								{
									RunsheetDetailsNote objrsdNote = ctx.RunsheetDetailsNotes.SingleOrDefault(rsdNote => rsdNote.RID == OrderId);
									if (objrsdNote != null)
									{
										objrsdNote.Sl = newSL;
										ctx.SubmitChanges();
									}
								}
							}
							newSL++;
						} // End of for each 
					} //EO Using
					reOrderDone = true;
				}
			}
			catch (Exception oEx)
			{
				Logger.Exception(oEx, "An error Occured in SetReOrderRunsheetDetailByRunsheetID()");
				throw;
			}
			return reOrderDone;
		}
        
		/// <summary>
		/// This service used for MarkOff runsheet (used by old driver app)
		/// According to RunsheetID list
		/// </summary>
		/// <param name="submittedRunsheetIDList">The submitted runsheet ID list.</param>
		/// <param name="submittedTruckID">The submitted truck ID.</param>
        /// <param name="localTime">The submitted driver device datetime.</param>
		/// <returns></returns>
        public bool SetRunsheetsMarkOffbyRID(List<int> submittedRunsheetIDList, int submittedTruckID)
        {
            return SetRunsheetsMarkOffbyRIDLocalTime(submittedRunsheetIDList, submittedTruckID, DateTime.Now);

        }

		/// <summary>
		/// This service used for MarkOff runsheet
		/// According to RunsheetID list
		/// </summary>
		/// <param name="submittedRunsheetIDList">The submitted runsheet ID list.</param>
		/// <param name="submittedTruckID">The submitted truck ID.</param>
        /// <param name="localTime">The submitted driver device datetime.</param>
		/// <returns></returns>
        public bool SetRunsheetsMarkOffbyRIDLocalTime(List<int> submittedRunsheetIDList, int submittedTruckID, DateTime localTime)
		{
			bool allRSMarkOffSuccessfully = false;
			try
			{
				if (submittedRunsheetIDList != null && submittedRunsheetIDList.Count > 0)
				{
					// Retrive all todays RunsheetList
					List<Abc.OnlineBL.Entities.Model.RunsheetModel> todaysRSList = GetRunsheetDetailsByTruckIDNDate(submittedTruckID, DateTime.Now);
					if (todaysRSList != null && todaysRSList.Count > 0)
					{
						// Find the Specific Runshee which order are going to markoff according to subRunsheetID
						foreach (int RunsheetID in submittedRunsheetIDList)
						{
							Abc.OnlineBL.Entities.Model.RunsheetModel rsModelToMarkOff = todaysRSList.Find(rsModel => rsModel.RunsheetId == RunsheetID);

							if (rsModelToMarkOff.Details != null && rsModelToMarkOff.Details.Count > 0)
							{
								using (AbcDataContext ctx = new AbcDataContext())
								{
									#region Mark off each Runsheet Detail on the Runsheet
									foreach (RunsheetDetail rsd in rsModelToMarkOff.Details)
									{
										if (rsd.DetailType != 3) // Notes will not be consider
										{
											if (rsd.OrderId < 900000 || (rsd.OrderId > 950000 && rsd.OrderId < 99000000))// (~900000 -- 950000) find ins SB_Order for logic ref in "DAS_MarkOffRS"
											{
												DespatchDetail objdd = ctx.DespatchDetails.SingleOrDefault(e => e.OrderID == rsd.OrderId);
												if (objdd != null)
												{
													if (rsd.IsRemoval) // Removal job
													{
														if (objdd.DateBoardRemoved == null)
														{
                                                            int? effect = ctx.DRIVER_MarkOffRS(rsd.OrderId, rsd.IsRemoval, rsModelToMarkOff.TruckId, false, true, rsd.Rid, null, DateTime.Now);
														}
													}
													else // Errectional job
													{
														if (objdd.DateBoardErected == null)
														{
                                                            int? effect = ctx.DRIVER_MarkOffRS(rsd.OrderId, rsd.IsRemoval, rsModelToMarkOff.TruckId, false, true, rsd.Rid, null, DateTime.Now);
														}
													}
												}
											}
											else
											{
												SB_Order objSBOrder = ctx.SB_Orders.SingleOrDefault(e => e.OrderID == rsd.OrderId);
												if (objSBOrder != null)
												{
													if (rsd.IsRemoval)
													{
														if (objSBOrder.DateBoardRemoved == null)
														{
                                                            int? effect = ctx.DRIVER_MarkOffRS(rsd.OrderId, rsd.IsRemoval, rsModelToMarkOff.TruckId, false, true, rsd.Rid, null, DateTime.Now);
														}
													}
													else
													{
														if (objSBOrder.DateBoardErected == null)
														{
                                                            int? effect = ctx.DRIVER_MarkOffRS(rsd.OrderId, rsd.IsRemoval, rsModelToMarkOff.TruckId, false, true, rsd.Rid, null,DateTime.Now);
														}
													}
												}
											}

										}
									} //End of Foreach for mark offing each undone job 
									#endregion

									#region Send Notification Email about each Runsheet ID
                                    ctx.DRIVER_RSMarkOff(RunsheetID, localTime);

									allRSMarkOffSuccessfully = true;

									//Create a event queue

                                    int eventID = EventSettings.MarkOffRSSummary;

									Truck truck = ctx.Trucks.SingleOrDefault(e => e.TruckID == rsModelToMarkOff.TruckId);

									string sub = "MarkOff Runsheet Report: RunsheetID " + RunsheetID + " Driver " + rsModelToMarkOff.TruckId + "(" + truck.TruckName + ")";

									StringBuilder sb = new StringBuilder();

									sb.Append("<TABLE style=\"font-family:Tahoma;font-size:9pt;border:1px solid gray;margin:5px;\">");
                                    sb.Append("<tr><td>Order ID</td><td>&nbsp;&nbsp;&nbsp;Property Address</td><td>Suburb</td><td>E/R</td><td>Mark Off</td></tr>");

									foreach (RunsheetDetail rsd in rsModelToMarkOff.Details)
									{
                                        string suburb = string.IsNullOrEmpty(rsd.Location) ? "" : rsd.Location;
										if (!string.IsNullOrEmpty(rsd.DateBoardErected) || !string.IsNullOrEmpty(rsd.DateBoardRemoved))
										{
											string notes = string.Empty;
                                            
											if (rsd.IsRemoval)
											{
												if (!string.IsNullOrEmpty(rsd.DateBoardRemoved))
                                                    notes = Formater.CustomDate(rsd.DateBoardRemoved) + " - Removal";
												else
													notes = "Removal";
											}
											else
											{
												if (!string.IsNullOrEmpty(rsd.DateBoardErected))
													notes =  Formater.CustomDate(rsd.DateBoardErected) + " - Installation";
												else
													notes = "Installation";
											}
                                            sb.AppendFormat("<TR><TD width=\"10%\"><B>{0}</B></TD><TD width=\"50%\">&nbsp;&nbsp;&nbsp;{1}</TD><TD width=\"15%\">{2}</TD><TD width=\"20%\">{3}</TD><TD width=\"5%\">{4}</TD></TR>", rsd.OrderId, rsd.Address, suburb, notes, true);
										}
										else
										{
                                            sb.AppendFormat("<TR><TD width=\"10%\"><B>{0}</B></TD><TD width=\"50%\">&nbsp;&nbsp;&nbsp;{1}</TD><TD width=\"15%\">{2}</TD><TD width=\"20%\">{3}</TD><TD width=\"5%\">{4}</TD></TR>", rsd.OrderId, rsd.Address, suburb, (rsd.IsRemoval == true ? "Removal" : "Installation"), false);
										}
									}

									sb.Append("</TABLE>");


									string xmlEventData = @"<HTML><head></head><body>
											<p>
												Driver has marked off the runsheet.
											</p>
											<p>
												<b><u>Details:</u></b>
											</p>
											<p>
												" + sb.ToString() + @"</p>
											</body>
											</html>";


									string textData = xmlEventData;
									string source = "OnlineBL_ManagerService_SetRunsheetsMarkOffbyRID";

									if (truck != null && !string.IsNullOrEmpty(truck.ManagerID))
									{
                                        Logger.Warn(truck.ManagerID + " -- " + RunsheetID + " -- " + truck.TruckName);
										ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, null, null, truck.ManagerID, null, source, "");
									}
									else
									{
										ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, null, null, null, null, source, "");
									}

                                    if (truck != null && truck.ManagerID == ManagerSettings.WorkshopSouthAustralia)
                                    {
                                        eventID = EventSettings.MarkOffRSSummaryForMainWorkshop;
                                        Logger.Warn(truck.ManagerID + " -- " + RunsheetID + " -- " + truck.TruckName + " -- " + eventID);
                                        ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, null, null, truck.ManagerID, null, source, "");
                                    }
									#endregion

								}//end using ctx database
							}// End of if runsheet found
						}
					}
				}
			}
			catch (SqlException oSqlex)
			{
				Logger.Exception(oSqlex, "An SQL error Occured in SetRunsheetsMarkOffbyRID()");
				throw;
			}
			catch (Exception oEx)
			{
				Logger.Exception(oEx, "An error Occured in SetRunsheetsMarkOffbyRID()");
				throw;
			}
			return allRSMarkOffSuccessfully;
		}
        
        /// <summary>
		/// This service used for markoff an job according to OrderID and RunsheetID
        /// This method was used for old tablets, now redirects to the new updated method DriverJobCompleteAndMarkOffed
		/// </summary>
		/// <param name="subOrderID"></param>
		/// <param name="subRID"></param>
		/// <returns></returns>
		public bool JobCompleteAndMarkOffed(RunsheetDetailCompleted rs)
		{            
            return DriverJobCompleteAndMarkOffed(rs);
		}

        #region Code for NEW TAB
        /// <summary>
        /// This method is used to markoff a job according to OrderID and RunsheetID 
        /// </summary>
        /// <param name="subOrderID"></param>
        /// <param name="subRID"></param>
        /// <returns></returns>
        public bool DriverJobCompleteAndMarkOffed(RunsheetDetailCompleted rs)
        {
            bool jobCompleteAndMarkOff = false;
            string subNotes = string.Empty;
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    //TODO: Make sure sign boards are being included.
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.RunSheet_To_Truck);
                    loadOptions.Add(EntityRelations.RunSheet_To_Truck1);
                    loadOptions.Add(EntityRelations.RunSheet_To_RunSheetDetails);
                    loadOptions.Add(EntityRelations.RunSheet_To_RunsheetDetailsSBs);
                    loadOptions.Add(EntityRelations.RunSheetDetail_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.DespatchDetail_To_Order);
                    loadOptions.Add(EntityRelations.RunsheetDetailsSB_To_SB_Order);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Client_To_ClientsDisplayInfo);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);
                    
                    // Get the Runsheet
                    RunSheet objRS = ctx.RunSheets.SingleOrDefault(e => e.RID == rs.RunID);
                    
                    // Get the Runsheet Details
                     // Wrap Order/SB_Order in adapter.
                    var runsheetDetail = (RunsheetDetailSBAdapter.IsStockBoard(rs.OrderID))
                        ? new RunsheetDetailSBAdapter(objRS.RunsheetDetailsSBs.SingleOrDefault(d => d.OrderID == rs.OrderID))
                        : new RunsheetDetailSBAdapter(objRS.RunSheetDetails.SingleOrDefault(d => d.OrderID == rs.OrderID));

                    var isReinstall = runsheetDetail.IsReinstall;

                    if (objRS != null)
                    {
                        bool hasExcuse = (rs.IsRequireHighInstall || rs.IsRequireCherryPicker || rs.IsRequireTwoMen || rs.IsBoardNotFound || rs.IsDamagedBoard || !string.IsNullOrEmpty(rs.EmailNote));
                        bool UnableToAct = (rs.IsBoardUnableToErect || rs.IsBoardUnableToRemove);

                        if (UnableToAct && !hasExcuse)
                        {
                            throw new Exception("Pls provide reasons why you are unable to do the job - " + rs.OrderID.ToString());
                        }

                        if (rs.IsBoardUnableToErect && hasExcuse || (rs.IsBoardUnableToRemove && (!rs.IsBoardNotFound && !rs.IsDamagedBoard)))
                        {
                            jobCompleteAndMarkOff = DriverUnableToPerformRunsheetJob(ctx, rs);
                            return jobCompleteAndMarkOff;
                        }
                        if (rs.IsRemoval)
                        {
                            if (rs.IsBoardUnableToRemove && (!rs.IsBoardNotFound && !rs.IsDamagedBoard))
                            {
                                jobCompleteAndMarkOff = DriverMarkOffedForUnableToRemoveBoard(rs);
                                return jobCompleteAndMarkOff;
                            }
                            else if (!rs.IsBoardNotFound && !rs.IsDamagedBoard)
                            {
                                int? effect = ctx.DRIVER_MarkOffRS(rs.OrderID, rs.IsRemoval, objRS.TruckID, true, true, rs.RunID, rs.PhotoAttachmentPath, rs.TabLocalDateTime);

                                // Add image to driver photos
                                ctx.DRIVER_Order_Photo_Add(rs.OrderID, true, false, objRS.TruckID, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "", DateTime.Now, rs.RunID);

                                //spotlight gone missing
                                //if a driver says spotlight not found from the job tracker app then we rOnlineBLe this event
                                if (rs.SpotlightNotFound)
                                {
                                    Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID);

                                    //Send new event
                                    int eventID = EventSettings.ABCSpotlightNotFoundWhenRemovingBoard;
                                    string sub = "Abc Notification: Spotlight Not Found While Removing Board - " + rs.OrderID.ToString() + " - " + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;");

                                    string xmlEventData = "<EVENT>" +
                                                            "<OrderID>" + rs.OrderID.ToString() + "</OrderID>" +
                                                            "<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + "</AgentName>" +
                                                            "<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + "</AgentOffice>" +
                                                            "<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location.Location1.Replace("&", "&amp;") + "</PAddress>" +
                                                            "<ReceivedOn>" + rs.TabLocalDateTime.ToString("dd/MM/yyyy") + "</ReceivedOn>" +
                                                            "</EVENT>";

                                    string textData = "Spotlight Not Found While Removing Board - Job No " + rs.OrderID.ToString();
                                    string source = "OnlineBL_ManagerService_DriverJobCompleteAndMarkOffed";

                                    ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, rs.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "");

                                    ctx.SubmitChanges();
                                }
                            }
                            else
                            {
                                int? effect = ctx.DRIVER_MarkOffRS(rs.OrderID, rs.IsRemoval, objRS.TruckID, true, false, rs.RunID, rs.PhotoAttachmentPath, rs.TabLocalDateTime);

                                //Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID);
                                // Wrap Order/SB_Order in adapter.
                                var od = (OrderSBAdapter.IsStockBoard(rs.OrderID))
                                    ? new OrderSBAdapter(ctx.SB_Orders.SingleOrDefault(o => o.OrderID == rs.OrderID))
                                    : new OrderSBAdapter(ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID));

                                OrderOtherInfo orderOtherInfo = ctx.OrderOtherInfos.SingleOrDefault(o => o.OrderID == rs.OrderID);

                                bool newOrderOtherInfo = false;

                                if (orderOtherInfo == null)
                                {
                                    orderOtherInfo = new OrderOtherInfo();
                                    orderOtherInfo.OrderID = rs.OrderID;
                                    newOrderOtherInfo = true;
                                }


                                int eventID = EventSettings.NoBoardFoundForRemoval;
                                string subPrefix = "Abc Notification: No Board Found on Property for Removal - ";
                                string message = "No Board Found on Property For Removal";
                                string textDataPrefix = "No Board Found on Property for Removal - Job No ";
                                string odNoteSuffix = "UNABLE TO REMOVE - BOARD NOT FOUND FOR REMOVAL";

                                if (rs.IsBoardNotFound)
                                {
                                    orderOtherInfo.LostBoard = true;
                                }

                                if (rs.IsDamagedBoard)
                                {
                                    subPrefix = "Abc Notification: Board on Property for Removal is Damaged - ";
                                    message = "Board on Property For Removal is Damaged";
                                    textDataPrefix = "Board on Property for Removal is Damaged - Job No ";
                                    odNoteSuffix = "UNABLE TO REMOVE - BOARD FOR REMOVAL DAMAGED";
                                    orderOtherInfo.DamagedBoard = true;
                                }

                                //Send new event
                                string sub = subPrefix + rs.OrderID.ToString() + " - " + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;");

                                string xmlEventData = "<EVENT>" +
                                                        "<OrderID>" + rs.OrderID.ToString() + "</OrderID>" +
                                                        "<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + "</AgentName>" +
                                                        "<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + "</AgentOffice>" +
                                                        "<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;") + "</PAddress>" +
                                                        "<ReceivedOn>" + rs.TabLocalDateTime.ToString("dd/MM/yyyy") + "</ReceivedOn>" +
                                                        "<Message>" + message + "</Message>" +
                                                        "</EVENT>";

                                string textData = textDataPrefix + rs.OrderID.ToString();
                                string source = "OnlineBL_ManagerService_DriverJobCompleteAndMarkOffed";

                                ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, rs.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "");
                                ctx.DRIVER_Order_Photo_Add(rs.OrderID, true, false, objRS.TruckID, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "", DateTime.Now, rs.RunID);

                                if (newOrderOtherInfo)
                                {
                                    ctx.OrderOtherInfos.InsertOnSubmit(orderOtherInfo);
                                }

                                ctx.SubmitChanges();

                                try
                                {
                                    if (OrderSBAdapter.IsStockBoard(rs.OrderID))
                                    {
                                        Order ord = ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID);
                                        if (ord != null)
                                        {
                                            if (!string.IsNullOrEmpty(ord.Notes))
                                            {
                                                ord.Notes = odNoteSuffix + " -- " + ord.Notes;
                                            }
                                            else
                                            {
                                                ord.Notes = odNoteSuffix;
                                            }
                                        }
                                        ctx.SubmitChanges();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex, "Can not update board removal order: " + rs.OrderID);
                                }
                            }
                            jobCompleteAndMarkOff = true;
                            subNotes = "Removal Notes";
                        }
                        else
                        {
                           

                            int? effect = ctx.DRIVER_MarkOffRS(rs.OrderID, rs.IsRemoval, objRS.TruckID, true, true, rs.RunID, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "", rs.TabLocalDateTime);

                            ctx.DRIVER_Order_Photo_Add(rs.OrderID, false, isReinstall, objRS.TruckID, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "", DateTime.Now, rs.RunID);

                            jobCompleteAndMarkOff = true;
                            subNotes = "Installation Notes";

                            string[] imageFileNameArr = new string[0];
                            if (!string.IsNullOrEmpty(rs.PhotoAttachmentPath))
                            {
                                imageFileNameArr = rs.PhotoAttachmentPath.Split(';');
                            }

                            if (imageFileNameArr.Length > 0)
                            {
                                for (int index = 0; index < imageFileNameArr.Length; index++)
                                {
                                    /// Sent Notification for No-Image.
                                    string imageFileName = System.IO.Path.GetFileName(imageFileNameArr[index]);

                                    if (imageFileName != null && imageFileName.ToLower().Contains("_noimagecross.jpg"))
                                    {
                                        string emailBody = string.Empty;
                                        string emailSubject = string.Empty;

                                        /// get the email-subject and body.
                                        this.GetEmailNotificationForNoImage(objRS, rs.OrderID, out emailSubject, out emailBody);

                                        if (emailSubject != string.Empty && emailBody != string.Empty)
                                        {
                                            string source = "OnlineBL_ManagerService_DriverJobCompleteAndMarkOffed";
                                            int EVENT_ID = EventSettings.NoInstallationImageTaken;
                                            ctx.SP_EventQueueAdd(EVENT_ID, emailSubject, emailBody, emailBody, null, null, objRS.ManID, null, source, "");
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        //Send new event
                        if (!string.IsNullOrEmpty(rs.EmailNote))
                        {
                            // Wrap Order/SB_Order in adapter.
                            var od = (OrderSBAdapter.IsStockBoard(rs.OrderID))
                                ? new OrderSBAdapter(ctx.SB_Orders.SingleOrDefault(o => o.OrderID == rs.OrderID))
                                : new OrderSBAdapter(ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID));

                            int evID = EventSettings.InstallationOrRemovalNotesReceivedFromDriver;
                            string subject = "Abc Notification: " + subNotes + " - " + rs.OrderID.ToString() + " - " + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;");

                            string xmlEventData = "<EVENT>" +
                                                    "<OrderID>" + rs.OrderID.ToString() + "</OrderID>" +
                                                    "<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + "</AgentName>" +
                                                    "<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + "</AgentOffice>" +
                                                    "<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;") + "</PAddress>" +
                                                    "<ReceivedOn>" + rs.TabLocalDateTime.ToString("dd/MM/yyyy") + "</ReceivedOn>" +
                                                    "<TruckName>" + objRS.Truck1.TruckName.Replace("&", "&amp;") + "</TruckName>" +
                                                    "<EmailNote>" + rs.EmailNote.Replace("&", "&amp;") + "</EmailNote>" +
                                                    "</EVENT>";

                            string textData = subNotes + " for " + rs.OrderID.ToString() + ": " + rs.EmailNote;
                            string source = "OnlineBL_ManagerService_DriverJobCompleteAndMarkOffed";

                            ctx.SP_EventQueueAdd(evID, subject, xmlEventData, textData, null, null, null, null, source, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "");
                        }
                    }
                }
            }
            catch (SqlException oSqlEx)
            {
                Logger.Exception(oSqlEx, "An sql error Occured in JobCompleteAndMarkOffed() - " + rs.OrderID.ToString());
                throw;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "An error Occured in JobCompleteAndMarkOffed() - " + rs.OrderID.ToString());
                throw;
            }

            return jobCompleteAndMarkOff;
        }

        /// <summary>
        /// Adds a new driver report to the dataReport table using a stored procedure
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool UpdateDriverReport(DriverReport report)
        {
            var addedReport = false;
            using (AbcDataContext ctx = new AbcDataContext())
            {
               var reportIDApp = new Guid( report.AppID);
               DateTime? starTime = null;
               DateTime? finishTime = null;
               if (report.StartTime > DateTime.MinValue)
                   starTime = report.StartTime;
               if (report.FinishTime > DateTime.MinValue)
                   finishTime = report.FinishTime;
               int? effect = ctx.DRIVER_ReportAdd(report.TruckID, reportIDApp, report.IMEI, report.VehicleNumber, report.Mobile, report.StartMileage, starTime, report.FinishMileage, finishTime, report.Notes);
               if (effect.HasValue && effect.Value > 0)
                   addedReport = true;
            }

            return addedReport;

        }
        private bool DriverUnableToPerformRunsheetJob(AbcDataContext ctx, RunsheetDetailCompleted rs)
        {
            bool jobCompleteAndMarkOff = false;
            string subNotes = string.Empty;

            try
            {
                //update notes

                RunSheet objRS = ctx.RunSheets.SingleOrDefault(e => e.RID == rs.RunID);

                if (objRS != null && objRS.MarkedOff == null)
                {
                    int? effect = ctx.DRIVER_MarkOffRS(rs.OrderID, rs.IsRemoval, objRS.TruckID, false, false, rs.RunID, rs.PhotoAttachmentPath, rs.TabLocalDateTime);

                    // Wrap Order/SB_Order in adapter.
                    var od = (OrderSBAdapter.IsStockBoard(rs.OrderID))
                        ? new OrderSBAdapter(ctx.SB_Orders.SingleOrDefault(o => o.OrderID == rs.OrderID))
                        : new OrderSBAdapter(ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID));

                    DespatchDetail des = ctx.DespatchDetails.SingleOrDefault(o => o.OrderID == rs.OrderID);

                    if (rs.IsRequireTwoMen)
                    {
                        des.RequireTwoMen = true;
                        Truck truck = ctx.Trucks.SingleOrDefault(o => o.TruckID == objRS.TruckID);
                        AuditLog log = new AuditLog() { Computer = Environment.MachineName, LogDateTime = DateTime.Now, ObjectId = rs.OrderID, ObjectType = 1, UserName = truck.TruckName };
                        string xmlData = "<auditSource><![CDATA[JobTracker Tablet]]></auditSource><prop name=\"Require Two Men\"><old><![CDATA[ " + des.RequireTwoMen + " ]]></old><new><![CDATA[ True ]]></new></prop>";
                        log.XmlData = xmlData;
                        ctx.AuditLogs.InsertOnSubmit(log);
                    }

                    
                    //Send new event
                    int eventID = EventSettings.UnableToErectRemoveBoard;
                    
                    string subject = "";
                    string body = "";
                    bool isOtherReaons = !rs.IsRequireHighInstall && !rs.IsRequireCherryPicker && !rs.IsRequireTwoMen && !string.IsNullOrEmpty(rs.EmailNote);
                    if (rs.IsRequireHighInstall || rs.IsRequireCherryPicker || isOtherReaons)
                    {
                        if (rs.IsRequireHighInstall)
                        {
                            subject = "Require High Install";
                            body = subject;
                        }
                        else if (rs.IsRequireCherryPicker)
                        {
                            subject = "Require Cherry Picker";
                            body = subject;
                        }
                        else if (isOtherReaons) {
                            subject = "Unable to Perform Job (Other Reasons)";
                            body = "Notes from "+ objRS.Truck1.TruckName.Replace("&", "&amp;") + ": " + rs.EmailNote.Replace("&", "&amp;");
                        }

                        string sub = "Abc Notification: " + subject + " - " + rs.OrderID.ToString() + " - " + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;");

                        string xmlEventData = "<EVENT>" +
                                                "<OrderID>" + rs.OrderID.ToString() + "</OrderID>" +
                                                "<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + "</AgentName>" +
                                                "<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + "</AgentOffice>" +
                                                "<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;") + "</PAddress>" +
                                                "<ReceivedOn>" + rs.TabLocalDateTime.ToString("dd/MM/yyyy") + "</ReceivedOn>" +
                                                "<Message> " + body + " </Message>" +
                                                "</EVENT>";

                        string textData = subject + " - " + rs.OrderID.ToString();
                        string source = "OnlineBL_ManagerService_DriverUnableToPerformRunsheetJob";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, rs.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "");
                    }

                    ctx.SubmitChanges();

                    jobCompleteAndMarkOff = true;
                }
            }
            catch (SqlException oSqlEx)
            {
                Logger.Exception(oSqlEx, "An sql error Occured in JobCompleteAndMarkOffed()");
                throw;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "An error Occured in JobCompleteAndMarkOffed()");
                throw;
            }

            return jobCompleteAndMarkOff;
        }

        /// <summary>
        /// Marks the offed for unable to remove board.
        /// </summary>
        /// <param name="rs">The rs.</param>
        /// <returns></returns>
        private bool DriverMarkOffedForUnableToRemoveBoard(RunsheetDetailCompleted rs)
        {
            bool jobCompleteAndMarkOff = false;
            string subNotes = string.Empty;

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    //TODO: Make sure sign boards are being included.
                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.RunSheet_To_Truck);
                    loadOptions.Add(EntityRelations.RunSheet_To_Truck1);
                    loadOptions.Add(EntityRelations.RunSheet_To_RunSheetDetails);
                    loadOptions.Add(EntityRelations.RunSheet_To_RunsheetDetailsSBs);
                    loadOptions.Add(EntityRelations.RunSheetDetail_To_DespatchDetail);
                    loadOptions.Add(EntityRelations.DespatchDetail_To_Order);
                    loadOptions.Add(EntityRelations.RunsheetDetailsSB_To_SB_Order);
                    loadOptions.Add(EntityRelations.Order_To_Client);
                    loadOptions.Add(EntityRelations.Order_To_Location);
                    loadOptions.Add(EntityRelations.Client_To_ClientsDisplayInfo);

                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(loadOptions);

                    RunSheet objRS = ctx.RunSheets.SingleOrDefault(e => e.RID == rs.RunID);

                    if (objRS != null)
                    {
                        int? effect = ctx.DRIVER_MarkOffRS(rs.OrderID, rs.IsRemoval, objRS.TruckID, true, false, rs.RunID, rs.PhotoAttachmentPath, rs.TabLocalDateTime);

                        ctx.DRIVER_Order_Photo_Add(rs.OrderID, true, false, objRS.TruckID, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "", DateTime.Now, rs.RunID);

                        var od = (OrderSBAdapter.IsStockBoard(rs.OrderID))
                            ? new OrderSBAdapter(ctx.SB_Orders.SingleOrDefault(o => o.OrderID == rs.OrderID))
                            : new OrderSBAdapter(ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID));

                        if (od != null)
                        {
                            if (!string.IsNullOrEmpty(od.Notes))
                            {
                                od.Notes = od.Notes + " -- Unable To Remove The Board For Removal";
                            }
                            else
                            {
                                od.Notes = " Unable To Remove The Board For Removal";
                            }
                        }
                        
                        //Send new event
                        int eventID = EventSettings.UnableToErectRemoveBoard;

                        string sub = "Abc Notification: Unable To Remove The Board on Property for Removal - " + rs.OrderID.ToString() + " - " + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;");

                        string xmlEventData = "<EVENT>" +
                                                "<OrderID>" + rs.OrderID.ToString() + "</OrderID>" +
                                                "<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + "</AgentName>" +
                                                "<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + "</AgentOffice>" +
                                                "<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;") + "</PAddress>" +
                                                "<ReceivedOn>" + rs.TabLocalDateTime.ToString("dd/MM/yyyy") + "</ReceivedOn>" +
                                                "<Message> Unable To Remove The Board on Property For Removal</Message>" +
                                                "</EVENT>";

                        string textData = " Unable To Remove The Board on Property for Removal - Job No " + rs.OrderID.ToString();
                        string source = "OnlineBL_ManagerService_JobCompleteAndMarkOffed";

                        ctx.SP_EventQueueAdd(eventID, sub, xmlEventData, textData, rs.OrderID, od.ClientID, od.ManagerID, null, source, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "");

                        ctx.SubmitChanges();

                        jobCompleteAndMarkOff = true;
                        subNotes = "Removal Notes";

                        //Send new event
                        if (!string.IsNullOrEmpty(rs.EmailNote))
                        {
                            //od = ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID);
                            od = (OrderSBAdapter.IsStockBoard(rs.OrderID))
                                ? new OrderSBAdapter(ctx.SB_Orders.SingleOrDefault(o => o.OrderID == rs.OrderID))
                                : new OrderSBAdapter(ctx.Orders.SingleOrDefault(o => o.OrderID == rs.OrderID));

                            
                            int evID = EventSettings.InstallationOrRemovalNotesReceivedFromDriver;

                            string subject = "Abc Notification: " + subNotes + " - " + rs.OrderID.ToString() + " - " + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;");

                            xmlEventData = "<EVENT>" +
                                                    "<OrderID>" + rs.OrderID.ToString() + "</OrderID>" +
                                                    "<AgentName>" + od.Client.ClientName.Replace("&", "&amp;") + "</AgentName>" +
                                                    "<AgentOffice>" + od.Client.Office.Replace("&", "&amp;") + "</AgentOffice>" +
                                                    "<PAddress>" + od.PropertyAddress.Replace("&", "&amp;") + ", " + od.Location1.Replace("&", "&amp;") + "</PAddress>" +
                                                    "<ReceivedOn>" + DateTime.Today.ToString("dd/MM/yyyy") + "</ReceivedOn>" +
                                                    "<TruckName>" + objRS.Truck1.TruckName.Replace("&", "&amp;") + "</TruckName>" +
                                                    "<EmailNote>" + rs.EmailNote.Replace("&", "&amp;") + "</EmailNote>" +
                                                    "</EVENT>";

                            textData = subNotes + " for " + rs.OrderID.ToString() + ": " + rs.EmailNote;
                            source = "OnlineBL_ManagerService_DriverMarkOffedForUnableToRemoveBoard";

                            ctx.SP_EventQueueAdd(evID, subject, xmlEventData, textData, null, null, null, null, source, !string.IsNullOrEmpty(rs.PhotoAttachmentPath) ? rs.PhotoAttachmentPath : "");
                        }
                    }
                }
            }
            catch (SqlException oSqlEx)
            {
                Logger.Exception(oSqlEx, "An sql error Occured in JobCompleteAndMarkOffed()");
                throw;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "An error Occured in JobCompleteAndMarkOffed()");
                throw;
            }

            return jobCompleteAndMarkOff;
        } 
        #endregion
        
        /// <summary>
        /// Gets the email notification for no image.
        /// </summary>
        /// <param name="runSheet">The run sheet.</param>
        /// <param name="orderID">The order ID.</param>
        /// <param name="emailSubject">The email subject.</param>
        /// <param name="emailBody">The email body.</param>
        public void GetEmailNotificationForNoImage(RunSheet runSheet, int orderID, out string emailSubject, out string emailBody)
        {
            emailBody = string.Empty;
            emailSubject = string.Empty;

            if (orderID <= 0 || runSheet == null)
            {
                return;
            }

            try
            {
                StringBuilder msgBodyBuilder = new StringBuilder();
               
                /// Get the Track info.
                Truck truckObj = runSheet.Truck1;
                string driverName = string.Empty;
                
                if (truckObj != null && truckObj.TruckID > 0)
                {
                    msgBodyBuilder.AppendFormat("Driver Name: {0} <br/>", truckObj.TruckName);
                    driverName = truckObj.TruckName;
                }
                
				/// Init. Runsheet Model Object.
                RunsheetModel runsheetModel = new RunsheetModel(runSheet);

                if (runsheetModel.Details.Count == 0)
                {
                    return;
                }

                /// Find out the details to get the Property Address.
                RunsheetDetail  runSheetDetailObj = runsheetModel.Details.Single(x => x.OrderId == orderID);

                if (runSheetDetailObj == null)
                {
                    return;
                }

                string propertyAddress = runSheetDetailObj.Address;
                
                msgBodyBuilder.AppendFormat("Property Address: {0}<br/>", propertyAddress);
                msgBodyBuilder.AppendFormat("OrderID: {0} <br/><br/>", orderID);

                /// Add Table Header.
                msgBodyBuilder.Append("<b>No image posted for the board installation.</b>");

                emailSubject = string.Format("[ANDROID Job Tracker] No-Image [{0}][{1}][{2}]", orderID, driverName, propertyAddress);
                emailBody = msgBodyBuilder.ToString();
                
                return;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, ex.Message);
            }
        }

        /// <summary>
        /// Gets the runsheet detail by ID.
        /// </summary>
        /// <param name="runsheetID">The runsheet ID.</param>
        /// <param name="orderID">The order ID.</param>
        /// <returns>
        /// Returns the RunsheetDetails Obj.
        /// </returns>
        public RunsheetDetail GetRunsheetDetailByID(int runsheetID, int orderID)
        {
            if (runsheetID <= 0 || orderID <= 0)
            {
                return null;
            }

            using (AbcDataContext ctx = new AbcDataContext())
            {
                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.RunSheet_To_Truck);
                loadOptions.Add(EntityRelations.RunSheet_To_Truck1);
                loadOptions.Add(EntityRelations.RunSheet_To_RunSheetDetails);
                loadOptions.Add(EntityRelations.RunSheetDetail_To_DespatchDetail);
				loadOptions.Add(EntityRelations.DespatchDetail_To_Order);
				loadOptions.Add(EntityRelations.Order_To_OrderOtherDetail);
                loadOptions.Add(EntityRelations.Order_To_Client);
                loadOptions.Add(EntityRelations.Order_To_Location);
				loadOptions.Add(EntityRelations.Location_To_DriverRegions);
                loadOptions.Add(EntityRelations.Client_To_ClientsDisplayInfo);

                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                RunSheet runSheet = ctx.RunSheets.SingleOrDefault(x => x.RID == runsheetID);

                if (runSheet == null)
                {
                    return null; 
                }

                /// Init. Runsheet Model Object.
                RunsheetModel runsheetModel = new RunsheetModel(runSheet);

                if (runsheetModel.Details.Count == 0)
                {
					Logger.Warn(string.Format("No Details on RunSheetID:{0}, orderID: {1}", runsheetID, orderID));
                    return null;
                }

                /// Find out the details to get the Property Address.
                RunsheetDetail  runSheetDetailObj = runsheetModel.Details.Single(x => x.OrderId == orderID);

                if (runSheetDetailObj == null)
                {
					Logger.Warn(string.Format("Can not locate order on RunSheetID:{0}, orderID: {1}", runsheetID, orderID));
					return null;
                }

				//Logger.Warn(string.Format("RunSheetID:{0}, orderID: {1}, HasInstallationNote: {2}", runsheetID, orderID, runSheetDetailObj.HasInstallationNotes.ToString()));
				return runSheetDetailObj;
            }
        }

        #endregion
    }
}
