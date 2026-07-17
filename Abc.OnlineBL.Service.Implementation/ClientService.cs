using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Enums;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.AbcVisual;
using Abc.OnlineBL.Entities.Model.Marketing;
using Abc.OnlineBL.Entities.Model.WebClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Abc.OnlineBL.Service.Implementation
{
    public partial class ClientService : IClientService
	{
		#region Abc Visual Constants
		private const int ALL_LISTINGS = 1;
		private const int ALL_NEW_LISTINGS = 2;
		#endregion

		#region IClientService Members

		#region SayHello
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
		#endregion

		#region GetClientById
		public Client GetClientById(int clientId, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					Client client = (from c in ctx.Clients
										  where c.ClientID == clientId
										  select c).FirstOrDefault();

					return client;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientByIdWithLoadOptions'. ClientId:{0} LoadOptions:{1}", clientId, options);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region GetClientsByName
		public List<Client> GetClientsByName(string clientName, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var results = from c in ctx.Clients
									  where c.ClientName == clientName
									  select c;

					return results.ToList();
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientsByNameWithLoadOptions'. ClientName:{0} LoadOptions:{1}", clientName, options);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region GetClientsPref
		public ClientsPref GetClientsPref(int clientId, int prefId)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{ 
					ctx.DeferredLoadingEnabled = false;

					ClientsPref pref = (from p in ctx.ClientsPrefs
											  where p.ClientId == clientId && p.PrefID == prefId
											  select p).FirstOrDefault();
					return pref;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetClientsPref'. ClientId:{0}, PrefId:{1}", clientId, prefId);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region UpdateClientPref
		public void UpdateClientsPref(ClientsPref clientsPref)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					clientsPref.SynchroniseWithDataContext(ctx);

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateClientsPref'. PrefId:{0}", clientsPref.PrefID);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region InsertClientsPref
		public ClientsPref InsertClientsPref(ClientsPref clientsPref)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.ClientsPrefs.InsertOnSubmit(clientsPref);

					ctx.SubmitChanges();
					return clientsPref;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'InsertClientsPref'");
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		public Client GetClientByUserId(string userId, List<EntityRelations> loadOptions)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("UserId");
			}

			using (AbcDataContext ctx = new AbcDataContext())
			{
				ctx.SetDataLoadOptions(loadOptions);
				Client client = (from c in ctx.Clients
									  join u in ctx.UserLogons on c.ClientID equals u.ClientId
									  where u.UserName.ToUpper() == userId.ToUpper()
									  select c).FirstOrDefault();
				return client;
			}
		}

		public bool AuthenticateClient(string userId, string password)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("UserId");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}

			using (AbcDataContext ctx = new AbcDataContext())
			{
				UserLogon client = ctx.UserLogons.SingleOrDefault(c => c.UserName.ToLower() == userId.ToLower());
				if (client != null)
				{
					return (client.Password.ToLower() == password.ToLower());
				}
			}

			return false;
		}

		#endregion

		#region IClientService Members

		public bool ValidateUser(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("UserName");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}

			using (AbcDataContext ctx = new AbcDataContext())
			{
				UserLogon userLogon = ctx.UserLogons.SingleOrDefault(
									  c => c.UserName.ToLower() == userName.ToLower()
									  && c.IsActive == true);
				if (userLogon != null)
				{
					return (userLogon.HashPassword.ToUpper() == GetMD5Hash(password.ToUpper()).ToUpper());
				}
			}

			return false;
		}

		private static String GetMD5Hash(String sDataToHash)
		{
			return Abc.Util.Security.MD5.GetMD5Hash(sDataToHash);
		}
		#endregion

		#region GetUserLogonByUserName
		public UserLogon GetUserLogonByUserName(string userName, List<EntityRelations> loadOptions)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}

			using (AbcDataContext ctx = new AbcDataContext())
			{
				ctx.DeferredLoadingEnabled = false;
				ctx.SetDataLoadOptions(loadOptions);
				return ctx.UserLogons.SingleOrDefault(u => u.UserName.ToLower() == userName.ToLower()
															&& u.IsActive == true);
			}
		}

		#endregion

		#region IsUserNameAlreadyExist
		public bool IsUserNameAlreadyExist(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("UserName");
			}

			using (AbcDataContext ctx = new AbcDataContext())
			{
				UserLogon userLogon = ctx.UserLogons.SingleOrDefault(
									  c => c.UserName.ToLower() == userName.ToLower());
				if (userLogon != null)
				{
					return true;
				}
			}

			return false;
		}

		#endregion

		#region InsertClientsRegistration
		public ClientsRegistration InsertClientsRegistration(ClientsRegistration clientsRegistration)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.ClientsRegistrations.InsertOnSubmit(clientsRegistration);

					ctx.SubmitChanges();
					return clientsRegistration;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'InsertClientsRegistration'");
				Logger.Exception(ex, message);
				throw;
			}

		}
		#endregion

		#region GetClientByEmail
		public Client GetClientByEmail(string email, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					Client client = (from c in ctx.Clients
										  where c.Email.ToUpper() == email.ToUpper()
										  select c).FirstOrDefault();

					return client;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientByEmailWithLoadOptions'. Email:{0} LoadOptions:{1}", email, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region UpdateClientRegistration
		public void UpdateClientRegistration(int regID, int clientID)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					ClientsRegistration clientReg = (from cr in ctx.ClientsRegistrations
																where cr.RegID == regID
																select cr).FirstOrDefault();

					clientReg.ClientId = clientID;
					clientReg.RegToken = Abc.Util.Security.MD5.GetMD5Hash(regID.ToString());
					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateClientRegistration'. RegID:{0}", regID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GenerateClientRegistrationEvent
		public void GenerateClientRegistrationEvent(int regID, string source)
		{

			using (AbcDataContext ctx = new AbcDataContext())
			{
				ctx.CDAS_GenRegEvent(regID, source);

				ctx.SubmitChanges();
			}
		}

		#endregion

		#region CreateUser
		public CreateUserStatus CreateUser(ClientsRegistration clientsRegistration)
		{
			if (clientsRegistration == null) throw new ArgumentException("Value cannot be null.", "clientsRegistration");

			CreateUserStatus status = CreateUserStatus.Success;
			try
			{
				//Validate if user name already exist
				if (IsUserNameAlreadyExist(clientsRegistration.UserName))
				{
                    clientsRegistration.UserName = clientsRegistration.UserName + "1";
                    if (IsUserNameAlreadyExist(clientsRegistration.UserName))
                    {
                        status = CreateUserStatus.DuplicateUserName;
                        return status;
                    }
				}

                //validate the credit form file
                if (string.IsNullOrEmpty(clientsRegistration.CreditFormFileByClient))
                {
                    return CreateUserStatus.UserRejected;
                }

				//Insert a new record into client registration table first
				clientsRegistration = InsertClientsRegistration(clientsRegistration);
				if (clientsRegistration == null || clientsRegistration.RegID <= 0)
				{
					status = CreateUserStatus.ProviderError;
					return status;
				}

				//Check if client already register
				int clientID = 0;

				//Client client = GetClientByEmail(clientsRegistration.Email, options);
				UserLogon userLogon = null;
				using (AbcDataContext ctx = new AbcDataContext())
				{
					userLogon = (from c in ctx.Clients
									 join u in ctx.UserLogons on c.ClientID equals u.ClientId
									 where c.Email.ToUpper() == clientsRegistration.Email.ToUpper()
									 select u).FirstOrDefault();

				}

				if (userLogon != null)
				{
					if (!String.IsNullOrEmpty(userLogon.UserName))
					{
						clientID = userLogon.ClientId.Value;
						if (!userLogon.UserName.StartsWith("a"))
						{
							status = CreateUserStatus.DuplicateEmail;
							return status;
						}
					}
				}

				//Update ClientsRegistration table
				UpdateClientRegistration(clientsRegistration.RegID, clientID);

				//Call GenerateClientRegistrationEvent to send email to client or administrator
				GenerateClientRegistrationEvent(clientsRegistration.RegID, "OnlineBL.ClientService::CreateUser");
				return status;

			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'CreateUser'");
				Logger.Exception(ex, message);
				status = CreateUserStatus.ProviderError;
				return status;
			}
		}

		#endregion

		#region EmailPassword
		public string EmailPassword(string userName, string email)
		{
			string newPass = "";
			System.Random rnd = new System.Random();
			newPass = Abc.Util.Security.MD5.GetMD5Hash(rnd.Next().ToString()).Substring(1, 6);
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.CDAS_EmailPass(userName, newPass, Abc.Util.Security.MD5.GetMD5Hash(newPass.ToUpper()), email);

					ctx.SubmitChanges();
					return "A New password will be mailed to your email address	shortly";
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		#endregion

		#region GetClientsRegistrationByTokenKey
		public ClientsRegistration GetClientsRegistrationByTokenKey(string tokenKey, List<EntityRelations> loadOptions)
		{
			if (String.IsNullOrEmpty(tokenKey))
			{
				throw new ArgumentException("Value cannot be null.", "tokenKey");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					ClientsRegistration clientReg = (from cr in ctx.ClientsRegistrations
																where cr.RegToken.ToUpper() == tokenKey.ToUpper()
																select cr).FirstOrDefault();

					return clientReg;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientsRegistrationByEmailWithLoadOptions'. TokenKey:{0} LoadOptions:{1}", tokenKey, options);
				Logger.Exception(ex, message);
				return null;
			}
		}

		#endregion

		#region AuthoriseUserRegistration
		public string AuthoriseUserRegistration(string tokenKey)
		{
			ClientsRegistration clientReg = GetClientsRegistrationByTokenKey(tokenKey, null);
			if (clientReg == null)
			{
				return "Invalid Request...";
			}
			if (clientReg.ClientId == null || clientReg.ClientId == 0)
			{
				return "Invalid Request...";
			}
			if (clientReg.RegComplete.HasValue && clientReg.RegComplete.Value)
			{
				return "This Registration has already been authorised.";
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					Client client = (from c in ctx.Clients
										  where c.ClientID == clientReg.ClientId
										  select c).FirstOrDefault();

					//Update ClientRegistration with RegComplete and RegCompleteOn
					ClientsRegistration clientRe = (from cr in ctx.ClientsRegistrations
															  where cr.RegToken.ToUpper() == tokenKey.ToUpper()
															  && cr.RegID == clientReg.RegID
															  select cr).FirstOrDefault();
					clientRe.RegComplete = true;
					clientRe.RegCompleteOn = DateTime.Now;

					//Might have to sync with UserLogon
					UserLogon userLogon = (from ul in ctx.UserLogons
												  where ul.ClientId == clientReg.ClientId
												  select ul).FirstOrDefault();

					userLogon.UserName = clientReg.UserName;
					userLogon.Password = clientReg.Password;
					userLogon.HashPassword = Abc.Util.Security.MD5.GetMD5Hash(clientReg.Password.ToUpper()).ToUpper();
					userLogon.IsActive = true;

					ctx.SubmitChanges();

					return "Thanks for registering with ABC Photosigns. <BR> Your registration process is now complete." +
							" To login to our site, please click <A href='logon'>here</A>.";
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'AuthoriseUserRegistration'. TokenKey:{0}", tokenKey);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetClientContactsByClientID
		public List<ClientContact> GetClientContactsByClientID(int clientID, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var results = from c in ctx.ClientContacts
									  where c.ClientId == clientID
									  select c;

					return results.ToList();
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientContactsByClientIDWithLoadOptions'. ClientID:{0} LoadOptions:{1}", clientID, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetClientContactByContactID
		public ClientContact GetClientContactByContactID(int contactID, List<EntityRelations> loadOptions)
		{
			if (contactID == 0)
			{
				throw new ArgumentException("Value cannot be null.", "contactID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					ClientContact clientContact = (from cc in ctx.ClientContacts
															 where cc.ContactId == contactID
															 select cc).FirstOrDefault();

					return clientContact;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientContactByContactIDWithLoadOptions'. ContactID:{0} LoadOptions:{1}", contactID, options);
				Logger.Exception(ex, message);
				return null;
			}
		}

		#endregion

		#region UpdateClientContact
		public void UpdateClientContact(ClientContact clientContact, string highResFilePath, string backUpFilePath)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					ClientContact cct = (from cc in ctx.ClientContacts
												where cc.ContactId == clientContact.ContactId
												select cc).FirstOrDefault();
					//save ClientContact
					cct.ContactName = clientContact.ContactName;
					cct.FirstName = clientContact.FirstName;
					cct.LastName = clientContact.LastName;
					cct.JobTitle = clientContact.JobTitle;
					cct.Phone = clientContact.Phone;
					cct.Mobile = clientContact.Mobile;
					cct.Email = clientContact.Email;
					cct.AddedThroughFeedSystem = clientContact.AddedThroughFeedSystem;
					cct.SendProofToEmail = clientContact.SendProofToEmail;
					if (clientContact.Photo != null)
						cct.Photo = clientContact.Photo;
					cct.HighResFilePath = clientContact.HighResFilePath;

					//Generate photo upload event
					ctx.CDAS_GenAgentPhotoUploadEvent(clientContact.ClientId, clientContact.ContactId, highResFilePath, backUpFilePath);

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateClientContact'. highResFilePath:{0}, backUpFilePath:{1}", highResFilePath, backUpFilePath);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region InsertClientContact
		public ClientContact InsertClientContact(ClientContact clientContact)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.ClientContacts.InsertOnSubmit(clientContact);

					ctx.SubmitChanges();
					return clientContact;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'InsertClientContact'");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetClientContactsByClientIDAndStatus
		public List<ClientContact> GetClientContactsByClientIDAndStatus(int clientID, bool status, List<EntityRelations> loadOptions)
		{
			if (clientID <= 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var results = from c in ctx.ClientContacts
									  where c.ClientId == clientID
									  && c.IsActive == status
									  && c.ForABCUseOnly == false
									  && c.AddedThroughFeedSystem == false
									  select c;

					return results.ToList();
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientContactsByClientIDAndStatusWithLoadOptions'. ClientID:{0} Status:{1} LoadOptions:{2}", clientID, status, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region UpdateClientContactStatus
		public void UpdateClientContactStatus(int contactID, bool isActive)
		{
			if (contactID <= 0)
			{
				throw new ArgumentNullException("contactID");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					ClientContact cc = (from c in ctx.ClientContacts
											  where c.ContactId == contactID
											  select c).FirstOrDefault();

					cc.IsActive = isActive;

					UserLogon ul = (from u in ctx.UserLogons
										 where u.ClientContactId == contactID
										 select u).FirstOrDefault();

					if (ul != null)
						ul.IsActive = isActive;

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateClientContactStatus'. ContactID:{0} IsActive:{1}", contactID, isActive);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetUserLogonByContactID
		public UserLogon GetUserLogonByContactID(int contactID, List<EntityRelations> loadOptions)
		{
			if (contactID <= 0)
			{
				throw new ArgumentNullException("contactID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);
					return ctx.UserLogons.SingleOrDefault(u => u.ClientContactId == contactID);
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetUserLogonByContactID'. ContactID:{0}", contactID);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region InsertUserLogon
		public UserLogon InsertUserLogon(UserLogon userLogon)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.UserLogons.InsertOnSubmit(userLogon);

					ctx.SubmitChanges();
					return userLogon;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'InsertUserLogon'");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region UpdateUserLogon
		public void UpdateUserLogon(int contactID, string userName, string password)
		{
			if (contactID <= 0)
			{
				throw new ArgumentNullException("contactID");
			}
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					UserLogon ul = (from c in ctx.UserLogons
										 where c.ClientContactId == contactID
										 select c).FirstOrDefault();

					ul.UserName = userName;
					ul.Password = password;
					ul.HashPassword = GetMD5Hash(password.ToUpper()).ToUpper();
					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateUserLogon'. ContactID:{0} UserName:{1}", contactID, userName);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region UpdateClient
		public void UpdateClient(Client client)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					client.SynchroniseWithDataContext(ctx);

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateClient'. ClientID:{0}", client.ClientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetCurrentOrdersByClientID
		public List<FilterOrders> GetCurrentOrdersByClientID(int clientID, bool isWorkShop, int agentContactID)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetCurrentOrdersResult> results = ctx.AIS_GetCurrentOrders(clientID, isWorkShop, agentContactID).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetCurrentOrdersResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetCurrentOrdersByClientID'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetUnhandleWorkflowAlertByOrderID
		public List<WorkflowAlert> GetUnhandleWorkflowAlertByOrderID(int orderID, List<EntityRelations> loadOptions)
		{
			if (orderID <= 0)
			{
				throw new ArgumentNullException("orderID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					var results = from wfa in ctx.WorkflowAlerts
									  where wfa.OrderId == orderID
									  && wfa.Handled == false
									  select wfa;

					return results.ToList();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetUnhandleWorkflowAlertByOrderID'. orderID:{0}", orderID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetConjunctionalAgentNameAndOfficeByOrderID
		public string GetConjunctionalAgentNameAndOfficeByOrderID(int orderID)
		{
			if (orderID <= 0)
			{
				throw new ArgumentNullException("orderID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{

					var query = from c in ctx.Clients
									join o in ctx.Orders on c.ClientID equals o.ConClientID
									where o.OrderID == orderID
									orderby c.ClientName, c.Office
									select c.ClientName + "/" + c.Office;

					return query.FirstOrDefault();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetConjunctionalAgentNameAndOfficeByOrderID'. orderID:{0}", orderID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetOnlineDesignOrdersByClientID
		public List<FilterOrders> GetOnlineDesignOrdersByClientID(int clientID, bool isWorkShop, int agentContactID)
		{
			if (clientID < 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetOnlineDesignOrdersResult> results = ctx.AIS_GetOnlineDesignOrders(clientID, isWorkShop, agentContactID).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetOnlineDesignOrdersResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetOnlineDesignOrdersByClientID'. clientID:{0}, agentContactID:{1}", clientID, agentContactID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetWaitingForApprovalOrdersByClientID
		public List<FilterOrders> GetWaitingForApprovalOrdersByClientID(int clientID, bool isWorkShop)
		{
			if (clientID < 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetWaitingForApprovalOrdersResult> results = ctx.AIS_GetWaitingForApprovalOrders(clientID, isWorkShop).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetWaitingForApprovalOrdersResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetWaitingForApprovalOrdersByClientID'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region GetWaitingForRemovalOrdersByClientID
		public List<FilterOrders> GetWaitingForRemovalOrdersByClientID(int clientID, bool isWorkShop)
		{
			if (clientID < 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetWaitingForRemovalOrdersResult> results = ctx.AIS_GetWaitingForRemovalOrders(clientID, isWorkShop).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetWaitingForRemovalOrdersResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetWaitingForRemovalOrdersByClientID'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetSMSOnDemandOrdersByClientID
		public List<FilterOrders> GetSMSOnDemandOrdersByClientID(int clientID, bool isWorkShop)
		{
			if (clientID < 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetSMSOnDemandOrdersResult> results = ctx.AIS_GetSMSOnDemandOrders(clientID, isWorkShop).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetSMSOnDemandOrdersResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetSMSOnDemandOrdersByClientID'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetAllClients
		public List<ClientDetails> GetAllClients()
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<ClientDetails> results = (from c in ctx.Clients
															 where c.Ceased == false
															 select new ClientDetails
														  {
															  ClientID = c.ClientID,
															  ClientName = c.ClientName,
															  Office = c.Office,
															  ClientNameOffice = c.ClientName + "/" + c.Office
														  }).OrderBy(c => c.ClientName).ToList();

					return results.ToList();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetAllClients'.");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetClientFavouriteProducts
		public List<Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineProduct> GetClientFavouriteProducts(int clientId, int agentContactId, bool diyOnly)
		{
			if (clientId <= 0)
			{
				throw new ArgumentNullException("clientId");
			}

			List<Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineProduct> ret = new List<Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineProduct>();
			List<EntityRelations> loadOptions = new List<EntityRelations>();
			loadOptions.Add(EntityRelations.ClientFav_To_Product);
			loadOptions.Add(EntityRelations.Product_To_OnlineOrderCategory);
			loadOptions.Add(EntityRelations.Product_To_ProductSizeCode);
			loadOptions.Add(EntityRelations.Product_To_PackageContentGroups);
			loadOptions.Add(EntityRelations.PackageContentGroup_To_PackageContentGroupProducts);
			loadOptions.Add(EntityRelations.Product_To_PriceListDetails);
			loadOptions.Add(EntityRelations.Product_To_ProductType);
			OnlineProductHelper helper = new OnlineProductHelper();

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					Client cl = (from c in ctx.Clients
									 where c.ClientID == clientId
									 select c).First();

					int groupId = cl.GroupId;

					var retFav = from oo in ctx.AIS_GetClientsFavProducts(clientId, agentContactId)
									 select oo.Product;

					Abc.OnlineBL.Service.Implementation.BusinessLogic.ProductComparer comp = new Abc.OnlineBL.Service.Implementation.BusinessLogic.ProductComparer();
					retFav = retFav.Distinct(comp).ToList();

					foreach (Product product in retFav)
					{
						//last minute check, although OnlineBL will never return something that has not CategoryId
						if (!product.CategoryId.HasValue)
							continue;

						//TODO: Turn this on from OnlineBL when DB records are all sorted out
						if (!product.ShowOnWebSite)
							continue;

						//lets create the equivanet model object
                        Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineProduct model = helper.SetupOnlineFavProduct(groupId, clientId, ctx, product, new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineProduct(), diyOnly, cl.PriceListID, false);

						//packages
						if (product.PackageContentGroups.Count > 0)
						{
							helper.SetupPackages(groupId, clientId, model, product, ctx, diyOnly, cl.PriceListID);
						}

						if (product.OnlineOrderCategory.MayContainDIYProducts)
						{
							if (diyOnly)
							{
								//if we are asking for DIY products and product or package content doesn't have any DIY then to return this product, rather
								//cotinue through the rest of the products in the loop
								if (!model.ProductHasAvailableDIYTemplates() && (model.CategoryName.ToLower() != "packages"))
									continue;
								else if ((model.CategoryName.ToLower() == "packages") && !model.ProductHasDIYTemplateOnAllGroup())
									continue;
							}
						}

						ret.Add(model);
					}

					return ret;
				}
			}
			catch (Exception ex)
			{
                string message = string.Format("Error occured in 'GetClientFavouriteProducts'. clientId:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region DeleteClientFavouriteProducts
		public void DeleteClientFavouriteProducts(int clientId, List<int> products, List<EntityRelations> loadOptions)
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
					ctx.SetDataLoadOptions(loadOptions);

					var listToDel = from cf in ctx.ClientFavs
										 where cf.ClientId == clientId && products.Contains(cf.ProductId)
										 select cf;

					if (listToDel.Count() > 0)
					{
						ctx.ClientFavs.DeleteAllOnSubmit(listToDel);
					}

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'DeleteClientFavouriteProducts'. clientId:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region InsertClientFavouriteProducts
		public void InsertClientFavouriteProducts(int clientId, int agentContactId, List<int> products, List<EntityRelations> loadOptions)
		{
			if (clientId <= 0)
			{
				throw new ArgumentNullException("clientId");
			}

			if (products == null)
			{
				throw new ArgumentNullException("products");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					foreach (var prodId in products)
					{
						ClientFav entity = new ClientFav();
						entity.ClientId = clientId;
						entity.ProductId = prodId;
						entity.DateCreated = DateTime.Now;
						if (agentContactId > 0)
							entity.AgentContactId = agentContactId;
						ctx.ClientFavs.InsertOnSubmit(entity);
					}

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'InsertClientFavouriteProducts'. clientId:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region GetPropertiesByClientID
		public List<Property> GetPropertiesByClientID(int clientID, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var results = (from p in ctx.Properties
										where p.ClientId == clientID && p.IsRegularOrder == false
										orderby p.PropertyId descending
										select p).Take(25).ToList().OrderBy(p => p.PropertyAddress).ToList();

					return results;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetPropertiesByClientID'. ClientID:{0} LoadOptions:{1}", clientID, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region SearchByDigit
		public List<FilterOrders> SearchByDigit(int clientID, int searchID, bool isWorkShop)
		{
			if (clientID < 0)
			{
				throw new ArgumentNullException("clientID");
			}
			if (searchID < 0)
			{
				throw new ArgumentNullException("searchID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_SearchByDigitResult> results = ctx.AIS_SearchByDigit(clientID, searchID, isWorkShop).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_SearchByDigitResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'SearchByDigit'. clientID:{0}, searchID:{1}", clientID, searchID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region SearchByAlphaNumeric
		public List<FilterOrders> SearchByAlphaNumeric(int clientID, string searchText, bool isWorkShop)
		{
			if (clientID < 0)
			{
				throw new ArgumentNullException("clientID");
			}
			if (string.IsNullOrEmpty(searchText))
			{
				throw new ArgumentNullException("searchText");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_SearchByAlphaNumericResult> results = ctx.AIS_SearchByAlphaNumeric(clientID, searchText, isWorkShop).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_SearchByAlphaNumericResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'SearchByAlphaNumeric'. clientID:{0}, searchText:{1}", clientID, searchText);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetOrderStatus
		public string GetOrderStatus(int orderID)
		{
			if (orderID <= 0)
			{
				throw new ArgumentNullException("orderID");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<EntityRelations> options = new List<EntityRelations>();
					options.Add(EntityRelations.Order_To_OrderDetails);
					options.Add(EntityRelations.OrderDetail_To_Product);
					options.Add(EntityRelations.Order_To_DespatchDetail);
					options.Add(EntityRelations.Order_To_ProofDetail);

					ctx.DeferredLoadingEnabled = false;
					if (options != null && options.Count > 0)
						ctx.SetDataLoadOptions(options);

					Order or = (from o in ctx.Orders
									where o.OrderID == orderID
									select o).FirstOrDefault();

					if (or == null)
						return "In Progress";
					else
					{
						if (or.OrderDetails != null && or.OrderDetails.Count > 0 && or.OrderDetails.Any(od => od.Product.WorkflowTypeId == 1))
						{
							if (or.DespatchDetail != null && or.DespatchDetail.DateBoardRemoved.HasValue)
							{
								return "Removed";
							}
							else if (or.DespatchDetail != null && or.DespatchDetail.DateRemovalRequested.HasValue)
							{
								return "Removal Requested";
							}
							else if (or.DespatchDetail != null && or.DespatchDetail.DateBoardErected.HasValue)
							{
								return "Installed";
							}
							else if (or.DespatchDetail != null && or.DespatchDetail.InTransit)
							{
                                if (IsOrderOnRegionalRun(orderID))
                                {
                                    return "In Progress";
                                }
                                else
                                {
								    return "Installation Due Today";
                                }
							}
							else
							{
								if (or.DespatchDetail != null && or.DespatchDetail.DateDespatched.HasValue)
								{
									return "Printed";
								}
								else
								{
									if (or.ProofDetail != null && or.ProofDetail.DateApproved.HasValue)
									{
										return "Approved";
									}
									else
										return "In Progress";
								}
							}
						}
						else
						{
							if (or.DespatchDetail != null && or.DespatchDetail.DateDespatched.HasValue)
							{
								return "Printed/Despatched";
							}
							else
							{
								if (or.ProofDetail != null && or.ProofDetail.DateApproved.HasValue)
								{
									return "Approved";
								}
								else
									return "In Progress";
							}
						}
					}

				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetOrderStatus'. orderID:{0}", orderID);
				Logger.Exception(ex, message);
				throw;
			}
		}

        private bool IsOrderOnRegionalRun(int orderID)
        {
            bool isOnRun = false;
            if (orderID <= 0)
            {
                throw new ArgumentNullException("orderID");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {

                    ctx.DeferredLoadingEnabled = false;

                    List<RunSheetDetail> rds = (from rd in ctx.RunSheetDetails
                                                       join r in ctx.RunSheets on rd.RID equals r.RID
                                                       where r.TruckID == 377 &&
                                                       rd.OrderID == orderID
                                                       select rd).ToList();

                    if (rds == null || rds.Count <= 0)
                        isOnRun = false;
                    else
                    {
                        isOnRun = true;
                    }

                }
                return isOnRun;
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'IsOrderOnRegionalRun'. orderID:{0}", orderID);
                Logger.Exception(ex, message);
                //throw;
            }
            return isOnRun;
        }

		#endregion

		#region IsViewLastProofApplicable
		public bool IsViewLastProofApplicable(int orderID)
		{
			#region 
			if (orderID <= 0)
			{
				throw new ArgumentNullException("orderID");
			}
			try
			{
				bool isDisplay = false;

				//check proofing_01 server first
				if (Directory.Exists(ServiceConfig.PROOF_DIR))
				{
					string[] files = Directory.GetFiles(ServiceConfig.PROOF_DIR, orderID + "*.pdf");
					string[] jpgFiles = Directory.GetFiles(ServiceConfig.PROOF_DIR, orderID + "*.jpg");
					List<string> fileList = new List<string>();

					foreach (string file in files)
					{
						if (Path.GetFileName(file).StartsWith(orderID.ToString()))
						{
							fileList.Add(Path.GetFileName(file));
						}
					}

					foreach (string file in jpgFiles)
					{
						if (Path.GetFileName(file).StartsWith(orderID.ToString()))
						{
							fileList.Add(Path.GetFileName(file));
						}
					}

					if (fileList.Count > 0)
					{
						isDisplay = true;
					}
				}
				if (!isDisplay)
				{
					//Check AOP_JobDocument table

					using (AbcDataContext ctx = new AbcDataContext())
					{
						List<EntityRelations> loadOptions = new List<EntityRelations>();
						loadOptions.Add(EntityRelations.Order_To_OrderDetails);
						loadOptions.Add(EntityRelations.Order_To_ProofDetail);

						ctx.DeferredLoadingEnabled = false;
						ctx.SetDataLoadOptions(loadOptions);

						Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

						if (od == null)
							return false;

						if (od.ProofDetail.DateApproved.HasValue || od.OrderDetails.All(odd => (!odd.UserDesignOnline.HasValue || odd.UserDesignOnline.Value == false)))
						{
							return false;
						}

					}

				}

				return isDisplay;
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'IsViewLastProofApplicable'. orderID:{0}", orderID);
				Logger.Exception(ex, message);
				throw;
			}
			#endregion
		}

		#endregion

		#region GetRecentPropertiesByClientID
		public List<FilterOrders> GetRecentPropertiesByClientID(int clientID, bool isWorkShop, int agentContactID)
		{
			if (clientID < 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetRecentPropertyResult> results = ctx.AIS_GetRecentProperty(clientID, isWorkShop, agentContactID).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetRecentPropertyResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetRecentPropertiesByClientID'. clientID:{0}, agentContactID: {1}", clientID, agentContactID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region IsUploadTextApplicable
		public bool IsUploadTextApplicable(int orderID)
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
					loadOptions.Add(EntityRelations.Order_To_OrderDetails);
					loadOptions.Add(EntityRelations.OrderDetail_To_Product);
					loadOptions.Add(EntityRelations.Order_To_MaterialDetail);
					loadOptions.Add(EntityRelations.Order_To_ProofDetail);

					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

					if (od == null)
						return false;

					if (od.OrderDetails != null && od.OrderDetails.Any(d => d.UserDesignOnline != null && d.UserDesignOnline.Value == true))
						return false;

					if (od.OrderDetails != null && od.OrderDetails.Count > 0)
					{
                        List<OrderDetail> odts = od.OrderDetails.Where(odt => (odt.Product.TypeID == ProductTypes.BillBoard || odt.Product.TypeID == ProductTypes.Brochure || odt.Product.TypeID == ProductTypes.WindowCard
                                                        || odt.Product.TypeID == ProductTypes.Corflute)).ToList();
						//TODO: Work with Package typeid
                        if (odts.All(odt => (odt.Product.TypeID == ProductTypes.BillBoard || odt.Product.TypeID == ProductTypes.Brochure || odt.Product.TypeID == ProductTypes.WindowCard
                                                        || odt.Product.TypeID == ProductTypes.Corflute) && odt.UserDesignOnline == true))
						{
							return false;

						}
						else
						{
							//check for DateApproved
							if (od.ProofDetail != null && od.ProofDetail.DateApproved.HasValue)
								return false;
							else
							{
								//Check text receive or not
								if (od.MaterialDetail != null && !od.MaterialDetail.TextReceived.HasValue)
									return true;
								else
									return false;
							}
						}
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'IsUploadTextApplicable'. orderID:{0}", orderID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region IsUploadImageApplicable
		public bool IsUploadImageApplicable(int orderID)
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
					loadOptions.Add(EntityRelations.Order_To_OrderDetails);
					loadOptions.Add(EntityRelations.OrderDetail_To_Product);
					loadOptions.Add(EntityRelations.Order_To_MaterialDetail);
					loadOptions.Add(EntityRelations.Order_To_ProofDetail);

					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					Order od = ctx.Orders.SingleOrDefault(o => o.OrderID == orderID);

					if (od == null)
						return false;

					if (od.OrderDetails != null && od.OrderDetails.Any(d => d.UserDesignOnline != null && d.UserDesignOnline.Value == true))
						return false;

					if (od.OrderDetails != null && od.OrderDetails.Count > 0)
					{
                        List<OrderDetail> odts = od.OrderDetails.Where(odt => (odt.Product.TypeID == ProductTypes.BillBoard || odt.Product.TypeID == ProductTypes.Brochure || odt.Product.TypeID == ProductTypes.WindowCard
                                                        || odt.Product.TypeID == ProductTypes.Corflute)).ToList();

                        if (odts.Any(odt => (odt.Product.TypeID == ProductTypes.BillBoard || odt.Product.TypeID == ProductTypes.Brochure || odt.Product.TypeID == ProductTypes.WindowCard
                                                        || odt.Product.TypeID == ProductTypes.Corflute)))
						{
							//check for DateApproved
							if (od.ProofDetail != null && od.ProofDetail.DateApproved.HasValue)
								return false;
							else
								return true;
						}
						else
						{
							return false;
						}
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'IsUploadTextApplicable'. orderID:{0}", orderID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region DeleteProperty
		public void DeleteProperty(int propertyID, int clientID)
		{
			if (propertyID <= 0)
			{
				throw new ArgumentNullException("propertyID");
			}
			if (clientID <= 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					var pro = (from pd in ctx.Properties
								  where pd.PropertyId == propertyID && pd.ClientId == clientID
								  select pd).FirstOrDefault();


					if (pro != null && !pro.IsRegularOrder)
					{

						ctx.Properties.DeleteOnSubmit(pro);
						ctx.SubmitChanges();
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'DeleteProperty'. propertyID:{0}", propertyID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetClientContactsForTextDetails
		public List<ClientContact> GetClientContactsForTextDetails(int clientID, List<EntityRelations> loadOptions)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var results = from c in ctx.ClientContacts
									  where c.ClientId == clientID && c.AddedThroughFeedSystem == false && c.ForABCUseOnly == false && c.ContactName != "" && c.IsActive == true
									  select c;

					return results.ToList();
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetClientContactsForTextDetails'. ClientID:{0} LoadOptions:{1}", clientID, options);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetSBOrderStatus
		public string GetSBOrderStatus(int orderID)
		{
			if (orderID <= 0)
			{
				throw new ArgumentNullException("orderID");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					SB_Order or = (from o in ctx.SB_Orders
										where o.OrderID == orderID
										select o).FirstOrDefault();

					if (or == null)
						return "In Progress";
					else
					{
						if (or.DateBoardRemoved.HasValue)
						{
							return "Removed";
						}
						else if (or.DateRemovalRequested.HasValue)
						{
							return "Removal Requested";
						}
						else if (or.DateBoardErected.HasValue)
						{
							return "Erected";
						}
						else
						{
							if (or.DateDespatched.HasValue)
							{
								return "Printed";
							}
							else
							{
								if (or.DateApproved.HasValue)
								{
									return "Approved";
								}
								else
									return "In Progress";
							}
						}
					}

				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetSBOrderStatus'. orderID:{0}", orderID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetAwaitingForErectionOrders
		public List<FilterOrders> GetAwaitingForErectionOrders(int clientID, bool isWorkShop)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetAwaitingForErectionOrdersResult> results = ctx.AIS_GetAwaitingForErectionOrders(clientID, isWorkShop).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetAwaitingForErectionOrdersResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetAwaitingForErectionOrders'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetDashBoardInfo
		public DashBoard GetDashBoardInfo(int clientID, int agentContactID)
		{
			if (clientID <= 0)
			{
				throw new ArgumentNullException("clientID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					var currentJob = ctx.AIS_GetStatCJobs(clientID, agentContactID).FirstOrDefault();

					DashBoard dashBoard = new DashBoard();

					if (currentJob != null)
					{
						dashBoard.TotalCurrentJobs = currentJob.TotalCurrent;
						dashBoard.TotalDIYJobToComplete = currentJob.DIYToComplete;
						dashBoard.TotalAwaitingInstallOrders = currentJob.AwaitingErection;
						dashBoard.TotalAwaitingRemoveOrders = currentJob.AwaitingRemoval;
					}

                    var isWorkshop = (from c in ctx.Clients
                                      where c.ClientID == clientID
                                      select c.Manager.IsWorkshop).FirstOrDefault();

                    var cJobs = ctx.AIS_GetCurrentOrders(clientID, isWorkshop, 0).ToList().Take(50);
                    foreach (var job in cJobs)
                    {
                        if (job.OrderID <= 0) continue;

                        DashboardJob jobItem = new DashboardJob();
                        jobItem.OrderId = job.OrderID;
                        jobItem.PropertyId = job.PropertyID;
                        jobItem.Address = job.PAddress;
                        jobItem.HasDiy = job.HasDIY;
                        jobItem.HasProof = job.HasProof;
                        jobItem.Caption = job.Caption;

                        dashBoard.CurrentJobs.Add(jobItem);
                    }

                    var remReqJobs = GetWaitingForRemovalOrdersByClientID(clientID, isWorkshop);

                    foreach (var job in remReqJobs)
                    {
                        if (job.OrderID <= 0) continue;

                        DashboardJob jobItem = new DashboardJob();
                        jobItem.OrderId = job.OrderID;
                        jobItem.Address = job.PAddress;
                        jobItem.Caption = job.Caption;
                        if (job.OrderID < 99000000)
                        {
                            var desd = ctx.DespatchDetails.Where(q => q.OrderID == job.OrderID).FirstOrDefault();
                            if (desd != null && desd.DateBoardErected.HasValue)
                                jobItem.BoardErected = desd.DateBoardErected;
                            else
                                continue;

                        }
                        else
                        {
                            var desd = ctx.SB_Orders.Where(q => q.OrderID == job.OrderID).FirstOrDefault();
                            if (desd != null && desd.DateBoardErected.HasValue)
                                jobItem.BoardErected = desd.DateBoardErected;
                            else
                                continue;
                        }
                        dashBoard.AwaitingRemovalJobs.Add(jobItem);
                    }

					return dashBoard;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetDashBoardInfo'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region UpdateWebSiteClick
		public void UpdateWebSiteClick(bool isNewWebSiteClick)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					WebSiteReport wsReport = (from w in ctx.WebSiteReports
													  select w).FirstOrDefault();

					if (wsReport != null)
					{
						if (isNewWebSiteClick)
							wsReport.NewWebSiteClick = wsReport.NewWebSiteClick + 1;
						else
							wsReport.OldWebSiteClick = wsReport.OldWebSiteClick + 1;
						ctx.SubmitChanges();
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateWebSiteClick");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region InsertMarketingMail
		public void InsertMarketingMail(MarketingMailModel marketingMailModel)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					MarketingMail model = new MarketingMail();
					model.BoardSupplier = marketingMailModel.BoardSupplier;
					model.ClientID = marketingMailModel.ClientID;
					model.Contact = marketingMailModel.ContactName;
					model.OfficeAddress = marketingMailModel.OfficeAddress;
					model.OfficeLocation = marketingMailModel.OfficeLocation;
					model.Phone = marketingMailModel.PhoneNumber;
					model.Boards = marketingMailModel.OrderedBoards;
					model.PhotographySupplier = marketingMailModel.PhotographySupplier;
					model.PhotoTextBoard = marketingMailModel.PhotoTextBoards;
					model.Position = marketingMailModel.ContactPosition;
					model.PostCode = marketingMailModel.PostCode;
					model.PrintingSupplier = marketingMailModel.PrintingSupplier;
					model.StockBoard = marketingMailModel.StockBoards;
					model.Suburb = marketingMailModel.Suburb;
					model.Photography = marketingMailModel.OrderedPhotography;
					model.Printing = marketingMailModel.OrderedPrinting;
					model.Brochures = marketingMailModel.OrderedBrochures;
					model.WindowCards = marketingMailModel.OrderedWindowCards;
					model.DateCreated = DateTime.Now;

					ctx.MarketingMails.InsertOnSubmit(model);

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'InsertUserLogon'");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region ChangePassword
		public bool ChangePassword(int clientId, string userName, string password, string newPassword)
		{
			if (clientId < 0)
			{
				throw new ArgumentNullException("clientId");
			}
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("UserName");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}
			if (string.IsNullOrEmpty(newPassword))
			{
				throw new ArgumentNullException("newPassword");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					UserLogon userLogon = ctx.UserLogons.SingleOrDefault(
										  c => c.UserName.ToLower() == userName.ToLower()
										  && c.IsActive == true && c.ClientId == clientId && c.Password == password);
					if (userLogon != null)
					{
						userLogon.Password = newPassword;
						userLogon.HashPassword = GetMD5Hash(newPassword.ToUpper()).ToUpper();

						ctx.SubmitChanges();

						return true;
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'ChangePassword'");
				Logger.Exception(ex, message);
				throw;
			}

			return false;
		}

		#endregion

		#region GetClientFavs
		public List<ClientFav> GetClientFavs(int clientId, int agenContactId)
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

					var cfs = (from cl in ctx.ClientFavs
								  where ((cl.ClientId == clientId && cl.AgentContactId == null) || (cl.ClientId == clientId && cl.AgentContactId == agenContactId))
								  select cl).ToList();
					return cfs;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetClientFavs'. clientId:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region RemoveMyopProduct
		public void RemoveMyopProduct(int clientId, int productId)
		{
			bool deactivate = false;
			if (clientId <= 0)
			{
				throw new ArgumentNullException("clientId");
			}
			if (productId <= 0)
			{
				throw new ArgumentNullException("productId");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<EntityRelations> options = new List<EntityRelations>();
					options.Add(EntityRelations.Product_To_ProductRules);

					ctx.SetDataLoadOptions(options);
					ctx.DeferredLoadingEnabled = false;

					//Remove package by checking product type is package and myop is true
					var prod = (from p in ctx.Products
                                where p.ProductID == productId && p.RelatedClientId == clientId && p.Myop == true && p.CategoryId == CategoryTypes.Packages
									select p).FirstOrDefault();

					//Then check for product rule has one row matching clientid and product id
					if (prod != null && prod.ProductRules != null && prod.ProductRules.Count > 0)
					{
						if (prod.ProductRules.ToList().Exists(pr => pr.ClientId == clientId))
						{
							try
							{
								ctx.Products.DeleteOnSubmit(prod);
								ctx.SubmitChanges();
							}
							catch (SqlException oSqlex)
							{
								string message = string.Format("Can not delete Myop Product. Try to deactivate product: {0}", productId);
								Logger.Warn(message);
								deactivate = true;
							}
							catch (Exception ex)
							{
								throw ex;
							}
						}
					}
				}

				if (deactivate)
				{
					using (AbcDataContext ctx = new AbcDataContext())
					{
						List<EntityRelations> options = new List<EntityRelations>();
						options.Add(EntityRelations.Product_To_ProductRules);

						ctx.SetDataLoadOptions(options);
						ctx.DeferredLoadingEnabled = false;

						//Remove package by checking product type is package and myop is true
						var prod = (from p in ctx.Products
                                    where p.ProductID == productId && p.RelatedClientId == clientId && p.Myop == true && p.CategoryId == CategoryTypes.Packages
										select p).FirstOrDefault();

						//Then check for product rule has one row matching clientid and product id
						if (prod != null && prod.ProductRules != null && prod.ProductRules.Count > 0)
						{
							if (prod.ProductRules.ToList().Exists(pr => pr.ClientId == clientId))
							{
								//try to deactivate product
								try
								{
									prod.Active = false;

									var favPro = (from p in ctx.ClientFavs
													  where p.ProductId == productId && p.ClientId == clientId
													  select p).FirstOrDefault();
									if (favPro != null)
									{
										ctx.ClientFavs.DeleteOnSubmit(favPro);
									}
									ctx.SubmitChanges();
								}
								catch (Exception ex)
								{
									throw ex;
								}
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'RemoveMyopProduct'. clientId: {0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region ROnlineBLeWatermarkMissingEvent
		public void ROnlineBLeWatermarkMissingEvent(int clientId, string watermarkFile)
		{
			if (clientId <= 0)
			{
				throw new ArgumentNullException("clientID");
			}
			if (string.IsNullOrEmpty(watermarkFile))
			{
				throw new ArgumentNullException("watermarkFile");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					ctx.SP_EventGen_WatermarkMissing(clientId, watermarkFile, "OnlineBL_ClientService_ROnlineBLeWatermarkMissingEvent");

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'ROnlineBLeWatermarkMissingEvent'. clientID:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetRegularPropertyByClientId
		public int GetRegularPropertyByClientId(int clientId)
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

					var pro = (from p in ctx.Properties
								  where p.ClientId == clientId && p.IsRegularOrder == true
								  select p).FirstOrDefault();

					if (pro == null)
					{
						pro = new Property();
						pro.ClientId = clientId;
						pro.IsRegularOrder = true;
						pro.StreetName = "Regular Order";
						pro.StreetNo = "1";
						pro.UnitNo = string.Empty;
						if (ServiceConfig.IS_NZ)
							pro.LocationId = 1745;
						else
							pro.LocationId = 10751;
						pro.CreatedOn = DateTime.Now;

						ctx.Properties.InsertOnSubmit(pro);
						ctx.SubmitChanges();
					}

					return pro.PropertyId;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetRegularPropertyByClientId'. clientID:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetRegularOrdersByClientID
		public List<FilterOrders> GetRegularOrdersByClientID(int clientID, bool isWorkShop)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					List<AIS_GetRegularOrdersResult> results = ctx.AIS_GetRegularOrders(clientID, isWorkShop).ToList();

					List<FilterOrders> lstFilterOrders = new List<FilterOrders>();

					foreach (AIS_GetRegularOrdersResult item in results)
					{
						FilterOrders filterOrders = new FilterOrders(item);
						lstFilterOrders.Add(filterOrders);
					}

					return lstFilterOrders;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetRegularOrdersByClientID'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region NotifyAdminAboutClientInfo
		public void NotifyAdminAboutClientInfo(int clientId)
		{
			if (clientId <= 0)
			{
				throw new ArgumentNullException("clientId");
			}

			bool isWorkshop = false;
			bool isPriceListComplete = false;
			bool showPriceOnWeb = false;
			bool dIYTemplateAvailable = false;

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<EntityRelations> loadOptions = new List<EntityRelations>();
					loadOptions.Add(EntityRelations.Client_To_Manager);
					loadOptions.Add(EntityRelations.Client_To_PriceList);

					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == clientId);

					if (cl != null)
					{
						if (cl.Manager != null)
						{
							isWorkshop = cl.Manager.IsWorkshop;
						}

						if (cl.PriceList != null)
						{
							isPriceListComplete = cl.PriceList.IsComplete;
							if (isWorkshop)
							{
								showPriceOnWeb = cl.PriceList.ShowPricingOnWeb;
							}
						}

						var query = from tm in ctx.AOP_TemplateProducts
										where tm.AOP_Template.ClientId == clientId || tm.AOP_Template.GroupId == cl.GroupId
										select tm;

						if (query.ToList().Count > 0)
						{
							dIYTemplateAvailable = true;
						}

						if (!isPriceListComplete || !dIYTemplateAvailable || (isWorkshop && !showPriceOnWeb))
						{
							//ROnlineBLe an Event to send email notification to Admin
                            int eventID = EventSettings.ClientProfileStatus;
							string sub = "Abc Notification: Client Profile need to amend (Client no: " + clientId + ")";
							string xmlData = @"<EVENT>
								<ClientID>" + clientId + @"</ClientID>
								<ClientName>" + cl.ClientName.Replace("&", "&amp;") + @"</ClientName>
								<Office>" + cl.Office.Replace("&", "&amp;") + @"</Office>
								<Profile>" + isPriceListComplete.ToString() + @"</Profile>
								<Pricing>" + (isWorkshop ? showPriceOnWeb.ToString() : "N/A") + @"</Pricing>
								<Templates>" + dIYTemplateAvailable.ToString() + @"</Templates>
								</EVENT>";

							string textData = "Client Profile need to amend (Client no: " + clientId + ")";
							string source = "OnlineBL_ClientService_NotifyAdminAboutClientInfo";

							ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, null, null, null, null, source, String.Empty);

							ctx.SubmitChanges();
						}
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'NotifyAdminAboutClientInfo'. clientId:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public List<Property> GetPropertiesByClientContactID(int clientContactID, List<EntityRelations> loadOptions)
		{
			if (clientContactID <= 0)
			{
				throw new ArgumentNullException("clientContactID");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (loadOptions != null && loadOptions.Count > 0)
						ctx.SetDataLoadOptions(loadOptions);

					var results = (from p in ctx.Properties
										where p.ClientContactId == clientContactID && p.IsRegularOrder == false
										orderby p.PropertyId descending
										select p).Take(25).ToList().OrderBy(p => p.PropertyAddress).ToList();

					return results;
				}
			}
			catch (Exception ex)
			{
				string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
				string message = string.Format("Error occured in 'GetPropertiesByClientContactID'. clientContactID:{0} LoadOptions:{1}", clientContactID, options);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		#region GetPriceListProductsByPriceListID
		public List<PriceListModel> GetPriceListProductsByPriceListID(int priceListID)
		{
			if (priceListID < 0)
			{
				throw new ArgumentNullException("priceListID");
			}
			try
			{
				List<EntityRelations> loadOptions = new List<EntityRelations>();
				loadOptions.Add(EntityRelations.Product_To_PackageContentGroups);
				loadOptions.Add(EntityRelations.PackageContentGroup_To_PackageContentGroupProducts);

				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					List<AIS_GetAllPriceListProductsResult> results = ctx.AIS_GetAllPriceListProducts(priceListID).ToList();

					List<PriceListModel> lstPriceLists = new List<PriceListModel>();

					foreach (AIS_GetAllPriceListProductsResult item in results)
					{
						PriceListModel pl = new PriceListModel(item);

                        if (item.TypeID == ProductTypes.BoardPackages || item.TypeID == ProductTypes.OtherPackages || item.TypeID == ProductTypes.Packages)
						{
							pl.PackageGroups = new List<Abc.OnlineBL.Entities.Model.OnlineOrder.PackageGroup>();

							var dbProductpk = (from p in ctx.Products
													 where p.ProductID == item.ProductID
													 select p).FirstOrDefault();

							if (dbProductpk != null && dbProductpk.PackageContentGroups != null)
							{
								foreach (var ppk in dbProductpk.PackageContentGroups)
								{
									Abc.OnlineBL.Entities.Model.OnlineOrder.PackageGroup grp = new Abc.OnlineBL.Entities.Model.OnlineOrder.PackageGroup();
									grp.GroupId = ppk.GroupId;
									grp.GroupName = ppk.GroupName;
									foreach (var pkgContentProd in ppk.PackageContentGroupProducts)
									{
										var dbProduct = (from p in ctx.Products
															  where p.ProductID == pkgContentProd.ProductId
															  select p).FirstOrDefault();
										if (dbProduct != null)
										{
											Abc.OnlineBL.Entities.Model.OnlineOrder.PackageContentProduct pkgProduct = new Abc.OnlineBL.Entities.Model.OnlineOrder.PackageContentProduct();
											pkgProduct.UniqueId = pkgContentProd.PackageContentGroupId;
											pkgProduct.PkgQty = pkgContentProd.Qty;
											pkgProduct.ItemNotes = pkgContentProd.ItemNotes;
											pkgProduct.PkgFormat = pkgContentProd.Format;
											pkgProduct.ProductName = dbProduct.Name;
                                            pkgProduct.ProductId = dbProduct.ProductID;
											grp.Products.Add(pkgProduct);
										}
									}
									pl.PackageGroups.Add(grp);
								}
							}
						}


						lstPriceLists.Add(pl);
					}

					return lstPriceLists;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetPriceListProductsByPriceListID'. priceListID:{0}", priceListID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region GetEventSubscriberByClientID
		public List<EventSubscription> GetEventSubscriberByClientID(int clientId)
		{
			if (clientId < 0)
			{
				throw new ArgumentNullException("clientId");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<EntityRelations> loadOptions = new List<EntityRelations>();
					loadOptions.Add(EntityRelations.EventSubscriber_To_EventSubscriptions);
					loadOptions.Add(EntityRelations.EventSubscription_To_Event);

					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					EventSubscriber cl = ctx.EventSubscribers.SingleOrDefault(e => e.ClientId == clientId);

					//if Client not Subscribe yet then subcribe them first
					if (cl == null)
					{
						cl.ClientId = clientId;
						cl.Type = 2;
						cl.Transport = "E-mail";

						List<Event> evs = ctx.Events.Where(e => e.Active && e.ClientsCanSub).ToList();

						cl.EventSubscriptions = new EntitySet<EventSubscription>();

						foreach (Event item in evs)
						{
							EventSubscription eventSubscription = new EventSubscription();
							eventSubscription.Active = false;
							eventSubscription.EventId = item.EventId;
							eventSubscription.SubId = cl.SubId;
							eventSubscription.SubscribedOn = DateTime.Now;

							cl.EventSubscriptions.Add(eventSubscription);
						}

						ctx.SubmitChanges();
					}


					return cl.EventSubscriptions.ToList();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetEventSubscriberByClientID'. clientId:{0}", clientId);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region SaveEventSubscriber
		public void SaveEventSubscriber(List<EventSubscription> es)
		{
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					foreach (EventSubscription item in es)
					{
						EventSubscription ev = ctx.EventSubscriptions.SingleOrDefault(e => e.EventId == item.EventId && e.SubId == item.SubId);
						if (ev != null)
							ev.Active = item.Active;
					}

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'SaveEventSubscriber'. SubId:{0}");
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region IsDIYTemplateAvailable
		public bool IsDIYTemplateAvailable(int clientID)
		{
			if (clientID <= 0)
			{
				throw new ArgumentNullException("clientID");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<EntityRelations> loadOptions = new List<EntityRelations>();
					ctx.DeferredLoadingEnabled = false;

					ClientsPref pref = (from p in ctx.ClientsPrefs
                                        where p.ClientId == clientID && p.PrefID == ClientsPref.CheckDIYTemplateAvailable
											  select p).FirstOrDefault();

					if (pref != null)
					{
						//Logger.Warn("Client has Pref 19: ClientID: {0}", clientID);

						return true;
					}
					else
					{
						//Logger.Warn("Client does not have Pref 19: ClientID: {0}", clientID);

						Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == clientID);

						if (cl != null)
						{

							var query = from tm in ctx.AOP_TemplateProducts
											where tm.AOP_Template.ClientId == clientID || tm.AOP_Template.GroupId == cl.GroupId
											select tm;

							if (query.ToList().Count > 0)
							{
								//insert a row into ClientsPref
								ClientsPref clientsPref = new ClientsPref();
								clientsPref.ClientId = clientID;
                                clientsPref.PrefID = ClientsPref.CheckDIYTemplateAvailable;
								clientsPref.BitValue = true;

								ctx.ClientsPrefs.InsertOnSubmit(clientsPref);
								ctx.SubmitChanges();

								//Logger.Warn("Client has DIY Template: ClientID: {0}", clientID);

								return true;
							}
							else
							{
								//Logger.Warn("Client does not have DIY Template: ClientID: {0}", clientID);

								return false;
							}
						}
						else
							return false;

					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'IsDIYTemplateAvailable'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region SetupDIYForClient
		public void SetupDIYForClient(int clientID, string contactName)
		{
			if (clientID <= 0)
			{
				throw new ArgumentNullException("clientID");
			}
			if (string.IsNullOrEmpty(contactName))
			{
				throw new ArgumentNullException("contactName");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<EntityRelations> loadOptions = new List<EntityRelations>();
					loadOptions.Add(EntityRelations.Client_To_Manager);
					loadOptions.Add(EntityRelations.Client_To_PriceList);

					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					Client cl = ctx.Clients.SingleOrDefault(c => c.ClientID == clientID);

					if (cl != null)
					{
						//ROnlineBLe an Event to send email notification to Admin
                        int eventID = EventSettings.ClientWantsDIYTemplate;
						string sub = "Abc Notification: Client would like to be set up for DIY (Client no: " + clientID + ")";
						string xmlData = @"<EVENT>
							<ClientID>" + clientID + @"</ClientID>
							<ClientName>" + cl.ClientName.Replace("&", "&amp;") + @"</ClientName>
							<Office>" + cl.Office.Replace("&", "&amp;") + @"</Office>
                            <Email>" + cl.Email.Replace("&", "&amp;") + @"</Email>
							<Contact>" + contactName.Replace("&", "&amp;") + @"</Contact>
							<Templates>" + "Not Available" + @"</Templates>
							</EVENT>";

						string textData = "Client would like to be set up for DIY (Client no: " + clientID + ")";
						string source = "OnlineBL_ClientService_SetupDIYForClient";

						ctx.SP_EventQueueAdd(eventID, sub, xmlData, textData, null, null, null, null, source, String.Empty);

						//insert a row into ClientsPref
						ClientsPref clientsPref = new ClientsPref();
						clientsPref.ClientId = clientID;
                        clientsPref.PrefID = ClientsPref.CheckDIYTemplateAvailable;
						clientsPref.BitValue = true;

						ctx.ClientsPrefs.InsertOnSubmit(clientsPref);
						ctx.SubmitChanges();
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'SetupDIYForClient'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region DonotSetupDIYForClient
		public void DonotSetupDIYForClient(int clientID)
		{
			if (clientID <= 0)
			{
				throw new ArgumentNullException("clientID");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.DeferredLoadingEnabled = false;

					//insert a row into ClientsPref
					ClientsPref clientsPref = new ClientsPref();
					clientsPref.ClientId = clientID;
                    clientsPref.PrefID = ClientsPref.CheckDIYTemplateAvailable;
					clientsPref.BitValue = false;

					ctx.ClientsPrefs.InsertOnSubmit(clientsPref);
					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'DonotSetupDIYForClient'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		#endregion

		#region ABC Visual
		//////public List<VisualListing> GetVisualListingsByClientID(int clientID)
		//////{
		//////    if (clientID < 0)
		//////    {
		//////        throw new ArgumentException("Invalid client id.");
		//////    }
		//////    try
		//////    {
		//////        List<EntityRelations> loadOptions = new List<EntityRelations>();
		//////        loadOptions.Add(EntityRelations.ABCVIS_ClientsAssetRental_To_ABCVIS_Listings);
		//////        loadOptions.Add(EntityRelations.ABCVIS_ClientsAssetRental_To_Client);                
		//////        loadOptions.Add(EntityRelations.Client_To_AR_Listings);
		//////        loadOptions.Add(EntityRelations.AR_Listing_To_AR_ListingType);
		//////        loadOptions.Add(EntityRelations.AR_Listing_To_AR_Status);
		//////        loadOptions.Add(EntityRelations.AR_Listing_To_Location);

		//////        loadOptions.Add(EntityRelations.ABCVIS_Listing_To_AR_Listing);

		//////        using (AbcDataContext ctx = new AbcDataContext())
		//////        {
		//////            ABCVIS_ClientsAssetRental result = ctx.ABCVIS_ClientsAssetRentals.Where(assetRental => 
		//////                                                   assetRental.ClientId.Equals(clientID) 
		//////                                                   && assetRental.IsActive.Value
		//////                                                   && (!assetRental.LeaseEnded)
		//////                                               ).FirstOrDefault();
		//////            if (result != null)
		//////            {
		//////                List<VisualListing> visualListings = new List<VisualListing>();
		//////                result.Client.AR_Listings.ToList().ForEach(ar_Listing => 
		//////                {
		//////                    VisualListing visualListing = new VisualListing(ar_Listing);
		//////                    visualListing.IsVisual = result.ABCVIS_Listings.Any(c => 
		//////                                                c.ListingId.Equals(ar_Listing.ListingId) 
		//////                                                && c.RentalId.Equals(result.RentalId)
		//////                                            );
		//////                    visualListings.Add(visualListing);
		//////                });                        

		//////                return visualListings;
		//////            }

		//////            return null;
		//////        }
		//////    }
		//////    catch (Exception ex)
		//////    {
		//////        string message = string.Format("Error occured in 'GetClientsVisualListingByClientID'. clientID:{0}", clientID);
		//////        Logger.Exception(ex, message);
		//////        throw;
		//////    }
		//////}        

		public AbcVisualResponse AddVisualListing(int rentalID, int listingID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}
			if (listingID < 0)
			{
				throw new ArgumentException("Invalid listing id.");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ctx.ABCVIS_Listings.InsertOnSubmit(new ABCVIS_Listing() { RentalId = rentalID, ListingId = listingID });
					ctx.SubmitChanges();
					return new AbcVisualResponse() { OperationHasError = false };
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'AddVisualListing'. rentalID:{0}, listingID:{1}", rentalID, listingID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public AbcVisualResponse RemoveVisualListing(int rentalID, int listingID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}
			if (listingID < 0)
			{
				throw new ArgumentException("Invalid listing id.");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ABCVIS_Listing abcviz_Listing = ctx.ABCVIS_Listings.SingleOrDefault(avl =>
																	avl.RentalId.Equals(rentalID)
																	&& avl.ListingId.Equals(listingID)
															  );
					if (abcviz_Listing != null)
					{
						ctx.ABCVIS_Listings.DeleteOnSubmit(abcviz_Listing);
						ctx.SubmitChanges();

						return new AbcVisualResponse() { OperationHasError = false };
					}

					return new AbcVisualResponse() { OperationHasError = true, ErrorMessage = "Not a valid visual listing." };
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'RemoveVisualListing'. rentalID:{0}, listingID:{1}", rentalID, listingID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public AbcVisualResponse UpdateDeviceVisusalListingsByListingType(int rentalID, int listingType)
		{
			if (rentalID <= 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}

			if (listingType <= 0)
			{
				throw new ArgumentException("Invalid listing type.");
			}

			try
			{
				if (listingType.Equals(ALL_LISTINGS) || listingType.Equals(ALL_NEW_LISTINGS))
				{
					List<EntityRelations> loadOptions = new List<EntityRelations>();
					loadOptions.Add(EntityRelations.ABCVIS_ClientsAssetRental_To_Client);
					loadOptions.Add(EntityRelations.Client_To_AR_Listings);
					loadOptions.Add(EntityRelations.AR_Listing_To_AR_Status);

					using (AbcDataContext ctx = new AbcDataContext())
					{
						List<ABCVIS_Listing> abcvizListings = ctx.ABCVIS_Listings.Where(avl => avl.RentalId.Equals(rentalID)).ToList();
						if (abcvizListings != null && abcvizListings.Count > 0)
						{
							// Remove existing rental-listing mapping data
							ctx.ABCVIS_Listings.DeleteAllOnSubmit(abcvizListings);
						}

						ABCVIS_ClientsAssetRental abcviz_ClientsAssetRental = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																				 assetRental.RentalId.Equals(rentalID)
																				 && assetRental.IsActive.Value
																				 && (!assetRental.LeaseEnded)
																			);
						if (abcviz_ClientsAssetRental != null)
						{
							List<ABCVIS_Listing> abcviz_Listings = new List<ABCVIS_Listing>();
							abcviz_ClientsAssetRental.Client.AR_Listings.ToList().ForEach(ar_Listing =>
							{
								if (ar_Listing.AR_Status.Status.Equals("READY")
									 || ar_Listing.AR_Status.Status.Equals("OFFMARKET")
									 || ar_Listing.AR_Status.Status.Equals("SOLD")
									 || ar_Listing.AR_Status.Status.Equals("LEASED")
									 || ar_Listing.AR_Status.Status.Equals("UNDEROFFER"))
								{
									if (listingType.Equals(ALL_LISTINGS))
									{
										ABCVIS_Listing abcviz_Listing = new ABCVIS_Listing();
										abcviz_Listing.ListingId = ar_Listing.ListingId;
										abcviz_Listing.RentalId = rentalID;
										abcviz_Listings.Add(abcviz_Listing);
									}
									else if ((DateTime.Now.Date - ar_Listing.DateReceived.Date).TotalDays <= 7)
									{
										ABCVIS_Listing abcviz_Listing = new ABCVIS_Listing();
										abcviz_Listing.ListingId = ar_Listing.ListingId;
										abcviz_Listing.RentalId = rentalID;
										abcviz_Listings.Add(abcviz_Listing);
									}
								}
							});
							if (abcviz_Listings.Count > 0)
							{
								ctx.ABCVIS_Listings.InsertAllOnSubmit(abcviz_Listings);
							}
						}
						ctx.SubmitChanges();
					}

					return new AbcVisualResponse() { OperationHasError = false };
				}

				return new AbcVisualResponse() { OperationHasError = true, ErrorMessage = "Invalid listing type." };
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateDeviceVisualListings'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public AbcVisualResponse UpdateDeviceVisualListings(int rentalID, List<int> newListings, List<int> removeListings)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					if (newListings != null && newListings.Count > 0)
					{
						newListings.ForEach(listingID =>
						{
							ctx.ABCVIS_Listings.InsertOnSubmit(new ABCVIS_Listing() { RentalId = rentalID, ListingId = listingID });
						});
					}
					if (removeListings != null && removeListings.Count > 0)
					{
						List<ABCVIS_Listing> abcvizListings = ctx.ABCVIS_Listings.Where(avl => avl.RentalId.Equals(rentalID) && removeListings.Contains(avl.ListingId)).ToList();
						if (abcvizListings != null && abcvizListings.Count > 0)
						{
							ctx.ABCVIS_Listings.DeleteAllOnSubmit(abcvizListings);
						}
					}
					ctx.SubmitChanges();

					return new AbcVisualResponse() { OperationHasError = false };
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateDeviceVisualListings'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public List<RentedAsset> GetRentedAssetByClientID(int clientID)
		{
			if (clientID < 0)
			{
				throw new ArgumentException("Invalid client id.");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<ABCVIS_ClientsAssetRental> clientsAssetRentalList = ctx.ABCVIS_ClientsAssetRentals.Where(a => a.ClientId.Equals(clientID) && a.IsActive.Value).ToList();

					if (clientsAssetRentalList != null && clientsAssetRentalList.Count > 0)
					{
						List<RentedAsset> rentedAssetList = new List<RentedAsset>();
						clientsAssetRentalList.ForEach(ar =>
						{
							rentedAssetList.Add(new RentedAsset()
							{
								RentalID = ar.RentalId,
								AssetID = ar.AssetId,
								FriendlyName = ar.FriendlyName,
								ListingDisplayOptionID = ar.ListingDisplayOptionId
							});
						});

						return rentedAssetList;
					}

					return null;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetRentedAssetByClientID'. clientID:{0}", clientID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public VisualListingOption GetVisualListingOptionByRentalId(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{

					ABCVIS_ClientsAssetRental clientsAssetRental = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(a => a.RentalId.Equals(rentalID));
					if (clientsAssetRental != null)
					{
						VisualListingOption visualListingOption = new VisualListingOption();
						visualListingOption.RentalId = clientsAssetRental.RentalId;
						visualListingOption.FriendlyName = clientsAssetRental.FriendlyName;
						visualListingOption.ListingDisplayOptionId = clientsAssetRental.ListingDisplayOptionId;
						visualListingOption.DisplayOption = new List<DisplayOption>();
						ctx.ABCVIS_ListingDisplayOptions.Where(a => a.ListingDisplayOptionId > 0).ToList().ForEach(ldo =>
						{
							visualListingOption.DisplayOption.Add(new DisplayOption()
							{
								DisplayOptionId = ldo.ListingDisplayOptionId,
								Description = ldo.Description
							});
						});

						return visualListingOption;
					}

					return null;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetVisualListingOptionByRentalId'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public AbcVisualResponse UpdateDisplayOptionForCustomVisualListings(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}
			try
			{
				List<EntityRelations> loadOptions = new List<EntityRelations>();
				loadOptions.Add(EntityRelations.ABCVIS_ClientsAssetRental_To_ABCVIS_Listings);
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ABCVIS_ClientsAssetRental abcviz_ClientsAssetRental = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																								  assetRental.RentalId.Equals(rentalID)
																								  && assetRental.IsActive.Value
																								  && (!assetRental.LeaseEnded)
																							 );
					if (abcviz_ClientsAssetRental != null)
					{
						int listingDisplayOptionId = ctx.ABCVIS_ListingDisplayOptions.Single(a =>
																  a.Description.Trim().ToLower().Equals("let me pick what to display *")
															  ).ListingDisplayOptionId;
						if (!abcviz_ClientsAssetRental.ListingDisplayOptionId.Equals(listingDisplayOptionId))
						{
							// Update listing display option id
							abcviz_ClientsAssetRental.ListingDisplayOptionId = listingDisplayOptionId;
							// Remove existing rental-listing mapping data
							ctx.ABCVIS_Listings.DeleteAllOnSubmit(ctx.ABCVIS_Listings.Where(a => a.RentalId.Equals(rentalID)));
							ctx.SubmitChanges();
						}

						return new AbcVisualResponse() { OperationHasError = false };
					}

					return new AbcVisualResponse() { OperationHasError = true, ErrorMessage = "Not a valid asset rental id." };
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateVisualListingsByRentalID'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public List<VisualListing> GetVisualListingsByClientAssetRentalID(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}
			try
			{
				List<EntityRelations> loadOptions = new List<EntityRelations>();
				loadOptions.Add(EntityRelations.ABCVIS_ClientsAssetRental_To_ABCVIS_Listings);
				loadOptions.Add(EntityRelations.ABCVIS_ClientsAssetRental_To_Client);
				loadOptions.Add(EntityRelations.Client_To_AR_Listings);
				loadOptions.Add(EntityRelations.AR_Listing_To_AR_ListingType);
				loadOptions.Add(EntityRelations.AR_Listing_To_AR_Status);
				loadOptions.Add(EntityRelations.AR_Listing_To_Location);
				loadOptions.Add(EntityRelations.AR_Listing_To_AR_Property);
				loadOptions.Add(EntityRelations.AR_Property_To_AR_PropertyType);
				loadOptions.Add(EntityRelations.ABCVIS_Listing_To_AR_Listing);

				using (AbcDataContext ctx = new AbcDataContext())
				{
					ABCVIS_ClientsAssetRental result = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																		assetRental.RentalId.Equals(rentalID)
																		&& assetRental.IsActive.Value
																		&& (!assetRental.LeaseEnded)
																  );
					if (result != null)
					{
						List<VisualListing> visualListings = new List<VisualListing>();
						int clientId = result.ClientId;
						if (result.ParentClientId.HasValue)
						{
							clientId = result.ParentClientId.Value;
						}
						int[] filterStatus = { 1, 4, 5, 6, 8 };
						var listings = from l in ctx.AR_Listings
											where l.ClientId == clientId && filterStatus.Contains(l.StatusId)
											select l;
						foreach (var ar_Listing in listings)
						{
							//if (ar_Listing.AR_Status.Status.Equals("READY")
							//     || ar_Listing.AR_Status.Status.Equals("OFFMARKET")
							//     || ar_Listing.AR_Status.Status.Equals("SOLD")
							//     || ar_Listing.AR_Status.Status.Equals("LEASED")
							//     || ar_Listing.AR_Status.Status.Equals("UNDEROFFER"))
							//{
							VisualListing visualListing = new VisualListing(ar_Listing);
							visualListing.IsVisual = result.ABCVIS_Listings.Any(c =>
																 c.ListingId.Equals(ar_Listing.ListingId)
																 && c.RentalId.Equals(result.RentalId)
															);
							visualListings.Add(visualListing);
							//} 
						}
						//result.Client.AR_Listings.ToList().ForEach(ar_Listing =>
						//{

						//});

						return visualListings;
					}

					return null;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetVisualListingsByClientAssetRentalID'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public ABCVIS_ClientsAssetRental GetClientsAssetRental(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					return ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(a => a.RentalId.Equals(rentalID) && a.IsActive.Value);
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetClientsAssetRental'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public AbcVisualResponse AddAllListingsAsVisualListing(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ABCVIS_ClientsAssetRental abcviz_ClientsAssetRental = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																								  assetRental.RentalId.Equals(rentalID)
																								  && assetRental.IsActive.Value
																								  && (!assetRental.LeaseEnded)
																							 );
					if (abcviz_ClientsAssetRental != null)
					{
						int listingDisplayOptionId = ctx.ABCVIS_ListingDisplayOptions.Single(a =>
																  a.Description.Trim().ToLower().Equals("automatically display all active listings")
															  ).ListingDisplayOptionId;

						if (!abcviz_ClientsAssetRental.ListingDisplayOptionId.Equals(listingDisplayOptionId))
						{
							// Update listing display option id
							abcviz_ClientsAssetRental.ListingDisplayOptionId = listingDisplayOptionId;
							// Remove existing rental-listing mapping data
							ctx.ABCVIS_Listings.DeleteAllOnSubmit(ctx.ABCVIS_Listings.Where(a => a.RentalId.Equals(rentalID)));
							ctx.SubmitChanges();
						}

						return new AbcVisualResponse() { OperationHasError = false };
					}

					return new AbcVisualResponse() { OperationHasError = true, ErrorMessage = "Not a valid asset rental id." };
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'AddAllListingsAsVisualListing'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public AbcVisualResponse AddAllNewListingsAsVisualListing(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid asset rental id.");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ABCVIS_ClientsAssetRental abcviz_ClientsAssetRental = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																								  assetRental.RentalId.Equals(rentalID)
																								  && assetRental.IsActive.Value
																								  && (!assetRental.LeaseEnded)
																							 );
					if (abcviz_ClientsAssetRental != null)
					{
						int listingDisplayOptionId = ctx.ABCVIS_ListingDisplayOptions.Single(a =>
																  a.Description.Trim().ToLower().Equals("automatically display all new listings")
															  ).ListingDisplayOptionId;

						if (!abcviz_ClientsAssetRental.ListingDisplayOptionId.Equals(listingDisplayOptionId))
						{
							// Update listing display option id
							abcviz_ClientsAssetRental.ListingDisplayOptionId = listingDisplayOptionId;
							// Remove existing rental-listing mapping data
							ctx.ABCVIS_Listings.DeleteAllOnSubmit(ctx.ABCVIS_Listings.Where(a => a.RentalId.Equals(rentalID)));
							ctx.SubmitChanges();
						}

						return new AbcVisualResponse() { OperationHasError = false };
					}

					return new AbcVisualResponse() { OperationHasError = true, ErrorMessage = "Not a valid asset rental id." };
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'AddAllNewListingsAsVisualListing'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public string GetTemplateConfigByRentalID(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid rental id.");
			}
			try
			{
				List<EntityRelations> loadOptions = new List<EntityRelations>();
				loadOptions.Add(EntityRelations.ABCVIS_ClientsAssetRental_To_ABCVIS_Template);

				using (AbcDataContext ctx = new AbcDataContext())
				{
					ABCVIS_ClientsAssetRental result = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																		assetRental.RentalId.Equals(rentalID)
																		&& assetRental.IsActive.Value
																		&& (!assetRental.LeaseEnded)
																  );
					if (result != null)
					{
						return result.ABCVIS_Template.TemplateConfig;
					}

					return null;
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetTemplateConfigByRentalID'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public AbcVisualResponse UpdateTemplateDataByRentalID(int rentalID, string templateData)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid rental id.");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					ABCVIS_ClientsAssetRental result = ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																		assetRental.RentalId.Equals(rentalID)
																		&& assetRental.IsActive.Value
																		&& (!assetRental.LeaseEnded)
																  );
					if (result != null)
					{
						result.TemplateData = templateData;
						ctx.SubmitChanges();
						return new AbcVisualResponse() { OperationHasError = false };
					}

					return new AbcVisualResponse() { OperationHasError = true, ErrorMessage = "Not a valid asset rental id." };
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'UpdateTemplateDataByRentalID'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}

		public ABCVIS_ClientsAssetRental GetVisualClientsAssetRentalByRentalID(int rentalID)
		{
			if (rentalID < 0)
			{
				throw new ArgumentException("Invalid rental id.");
			}
			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					return ctx.ABCVIS_ClientsAssetRentals.SingleOrDefault(assetRental =>
																		assetRental.RentalId.Equals(rentalID)
																		&& assetRental.IsActive.Value
																		&& (!assetRental.LeaseEnded)
																  );
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'GetVisualClientsAssetRentalByRentalID'. rentalID:{0}", rentalID);
				Logger.Exception(ex, message);
				throw;
			}
		}
		#endregion

		public void NotifyAdminAboutClientInfoUpdateRequest(Abc.OnlineBL.Entities.Client client)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}

			try
			{
				using (AbcDataContext ctx = new AbcDataContext())
				{
					List<EntityRelations> loadOptions = new List<EntityRelations>();
					loadOptions.Add(EntityRelations.Client_To_Manager);
					loadOptions.Add(EntityRelations.Client_To_PriceList);

					ctx.DeferredLoadingEnabled = false;
					ctx.SetDataLoadOptions(loadOptions);

					//ROnlineBLe an Event to send email notification to Admin
                    int eventID = EventSettings.UpdateAgentDetailsRequest;
					string sub = "Abc Notification: Client Requested to Change/Update Contact Details (Client no: " + client.ClientID + ")";
					string xmlData = @"<EVENT>
								<ClientID>" + client.ClientID + @"</ClientID>
								<ClientName>" + client.ClientName.Replace("&", "&amp;") + @"</ClientName>
								<Office>" + client.Office.Replace("&", "&amp;") + @"</Office>
								<Phone>" + client.Phone.Replace("&", "&amp;") + @"</Phone>
								<Fax>" + client.Fax.Replace("&", "&amp;") + @"</Fax>
								<Email>" + client.Email.Replace("&", "&amp;") + @"</Email>
								<SendProofBy>" + client.SendProofBy.Replace("&", "&amp;") + @"</SendProofBy>
								<SendProofTo>" + client.SendProofTo.Replace("&", "&amp;") + @"</SendProofTo>
								<ReceivedOn>" + DateTime.Now + @"</ReceivedOn>
								</EVENT>";


					string source = "OnlineBL_ClientService_NotifyAdminAboutClientInfoUpdateRequest";

					ctx.SP_EventQueueAdd(eventID, sub, xmlData, xmlData, null, null, null, null, source, String.Empty);

					ctx.SubmitChanges();
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("Error occured in 'OnlineBL_ClientService_NotifyAdminAboutClientInfoUpdateRequest'");
				Logger.Exception(ex, message);
				throw;
			}
		}

        #region GetPrintReadyTemplatesByPriceListID
        public List<PrintReadyTemplate> GetPrintReadyTemplatesByPriceListID(int priceListID)
        {
            if (priceListID <= 0)
            {
                throw new ArgumentNullException("PriceListID");
            }
            try
            {
                List<EntityRelations> options = new List<EntityRelations>();
                options.Add(EntityRelations.PrintReadyTemplate_To_ProductSizeCode);
                options.Add(EntityRelations.PrintReadyTemplate_To_OnlineOrderCategory);

                using (AbcDataContext ctx = new AbcDataContext())
                {
                
                    ctx.DeferredLoadingEnabled = false;
                    ctx.SetDataLoadOptions(options);

                    List<int> printTemplatesForPriceList = ctx.FrontOffice_GetPrintReadyTemplatesByPriceListID(priceListID).Where(x => x.PrintReadyTemplateID.HasValue).Select(x => x.PrintReadyTemplateID.Value).ToList();

                    var query = from p in ctx.PrintReadyTemplates
                                where printTemplatesForPriceList.Contains(p.PrintReadyTemplateID)
                                select p;
                
                    var result = query.OrderBy(x => x.CategoryID).ThenBy(x => x.SizeCode).ThenBy(x => x.FrameType).ToList();

                    return result;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'GetPrintReadyTemplatesByPriceListID'. priceListID:{0}", priceListID);
                Logger.Exception(ex, message);
                throw;
            }
        }
        #endregion

        #region UpdateOfficeSetting
        public void UpdateOfficeSetting(Client client)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    Client cl = (from cc in ctx.Clients
                                 where cc.ClientID == client.ClientID
                                 select cc).FirstOrDefault();
                    //save ClientContact
                    cl.Phone = client.Phone;
                    cl.Fax = client.Fax;
                    cl.Email = client.Email;
                    cl.SendProofBy = client.SendProofBy;
                    cl.SendProofTo = client.SendProofTo;

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateOfficeSetting'. ");
                Logger.Exception(ex, message);
                throw;
            }
        }

        #endregion
    }
}
