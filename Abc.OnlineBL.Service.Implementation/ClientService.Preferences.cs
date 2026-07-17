using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model.WebClient;
using Abc.OnlineBL.Entities.Model;

namespace Abc.OnlineBL.Service.Implementation
{
    public partial class ClientService
    {
        #region ClientCanOrderDIYOnly
        public bool ClientCanOrderDIYOnly(int clientId)
        {
            if (clientId <= 0)
            {
                throw new ArgumentNullException("clientId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    ClientsPref pref = (from p in ctx.ClientsPrefs
                                        where p.ClientId == clientId && p.PrefID == ClientsPref.ClientCanOrderDIYOnly
                                        select p).FirstOrDefault();

                    if (pref == null)
                        return false;

                    if (pref.BitValue.HasValue && pref.BitValue.Value)
                        return true;
                    else
                        return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ClientCanOrderDIYOnly'. clientId:{0}", clientId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region ClientToSupplyReadyArtworkOnly
        public bool ClientToSupplyReadyArtworkOnly(int clientId)
        {
            if (clientId <= 0)
            {
                throw new ArgumentNullException("clientId");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    ClientsPref pref = (from p in ctx.ClientsPrefs
                                        where p.ClientId == clientId && p.PrefID == ClientsPref.ClientToSupplyReadyArtworkOnly
                                        select p).FirstOrDefault();

                    if (pref == null)
                        return false;

                    if (pref.BitValue.HasValue && pref.BitValue.Value)
                        return true;
                    else
                        return false;

                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'ClientToSupplyReadyArtworkOnly'. clientId:{0}", clientId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion

        #region GetNationalClientPostCode
        public List<ClientNationalDeliveryLocation> GetClientNationalDeliveryLocation(string postCode, string suburb, string state)
        {
            if (string.IsNullOrEmpty(postCode) && string.IsNullOrEmpty(suburb) && string.IsNullOrEmpty(state))
            {
                state = "VIC";
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    List<ClientNationalDeliveryLocation> cnl = (from c in ctx.ClientNationalDeliveryLocations
                                                         where c.State.ToUpper() == state.ToUpper()
                                                         select c).ToList();


                    if (!string.IsNullOrEmpty(postCode))
                    {
                        cnl = cnl.FindAll(c => c.PostCode.ToUpper().Contains(postCode.ToUpper()));
                    }
                    if (!string.IsNullOrEmpty(suburb))
                    {
                        cnl = cnl.FindAll(c => c.Suburb.ToUpper().Contains(suburb.ToUpper()));
                    }

                    return cnl;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetClientNationalDeliveryLocation'. postCode: {0}, suburb: {1}, state: {2}", postCode, suburb, state);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region NotifyAdminAboutABCREPropertyListingRequest
        public void NotifyAdminAboutABCREPropertyListingRequest(PropertyListingModel propertyListingModel)
        {
            if (propertyListingModel == null)
            {
                throw new ArgumentNullException("propertyListingModel");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    //ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.ABCREPropertyListing;
                    string sub = "Abc Notification: ABCRE Property Listing Request";
                    string xmlData = @"<EVENT>
								<ClientID>" + propertyListingModel.ClientID + @"</ClientID>
								<AgentName>" + propertyListingModel.ClientName.Replace("&", "&amp;") + @"</AgentName>
								<AgentOffice>" + propertyListingModel.Office.Replace("&", "&amp;") + @"</AgentOffice>
								<ContactName>" + propertyListingModel.RequestedBy.Replace("&", "&amp;") + @"</ContactName>
								<Title>" + propertyListingModel.Title.Replace("&", "&amp;") + @"</Title>
								<ContactNumber>" + propertyListingModel.PhoneNumber.ToString() + @"</ContactNumber>
								<ContactEmail>" + propertyListingModel.Email.ToString() + @"</ContactEmail>
								<BulkUploader>" + propertyListingModel.BulkUploader.ToString() + @"</BulkUploader>
								</EVENT>";

                    string textData = "ABCRE Property Listing Request";
                    string source = "OnlineBL_ClientService_NotifyAdminAboutABCREPropertyListingRequest";

                    ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, null, null, null, null, source, String.Empty);

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'NotifyAdminAboutABCREPropertyListingRequest'. ClientName: {0}, Office: {1}, RequestedBy: {2}", propertyListingModel.ClientName, propertyListingModel.Office, propertyListingModel.RequestedBy);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion
    }
}
