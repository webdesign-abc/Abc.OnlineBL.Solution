using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using System.ServiceModel;
using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OnlineOrder;

namespace Abc.OnlineBL.Service.Implementation
{
    public class AccountService : IAccountService
    {
        #region IAccountService Members

        public string SayHello(string name)
        {
            string sName = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
            return string.Format("Hello {0}. You are also {1}", name, sName);
        }

        public CommOrder GetCommOrder(int cusId, int invID, List<EntityRelations> loadOptions)
        {
            if (cusId <= 0 || invID <= 0)
            {
                return null;
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var query = from c in ctx.CommOrders
                                where c.CustomerID == cusId
                                && c.OrderID == invID
                                && c.Due > 0
                                select c;

                    return query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetCommOrderInvoice'. CustomerID:{0}. OrderID:{1}", cusId, invID);
                throw;
            }
        }

        /// <summary>
        /// Inserts the online payment.
        /// </summary>
        /// <param name="onlinePayments">The online payments.</param>
        /// <returns></returns>
        public OnlinePayment InsertOnlinePayment(OnlinePayment onlinePayments)
        {
            if (onlinePayments == null)
            {
                throw new ArgumentException("OnlinePayment object is null");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.DeferredLoadingEnabled = false;

                        /// Insert record into OnlinePayment table
                        OnlinePayment onlinePaymentObj = new OnlinePayment();

                        onlinePaymentObj.ClientId = onlinePayments.ClientId;
                        onlinePaymentObj.AccountId = onlinePayments.AccountId;
                        onlinePaymentObj.CardType = onlinePayments.CardType;
                        onlinePaymentObj.Surcharge = onlinePayments.Surcharge;
                        onlinePaymentObj.Total = onlinePayments.Total;
                        onlinePaymentObj.CreateOn = DateTime.Now;
                        onlinePaymentObj.TransactionComplete = false;
                        onlinePaymentObj.CBATransactionRef = string.Empty;
                        onlinePaymentObj.PaymentId = 0;

                        ctx.OnlinePayments.InsertOnSubmit(onlinePaymentObj);
                        ctx.SubmitChanges();

                        int onlinePaymentId = onlinePaymentObj.OnlinePaymentId;

                        if (onlinePaymentId <= 0)
                        {
                            return null;
                        }

                        ///insert PaymentDetails table
                        foreach (OnlinePaymentDetail onlinePaymentDetailItem in onlinePayments.OnlinePaymentDetails)
                        {
                            OnlinePaymentDetail oDObj = new OnlinePaymentDetail();
                            oDObj.OnlinePaymentId = onlinePaymentId;
                            oDObj.OrderId = onlinePaymentDetailItem.OrderId;
                            oDObj.AmountPaid = onlinePaymentDetailItem.AmountPaid;
                            ctx.OnlinePaymentDetails.InsertOnSubmit(oDObj);
                            onlinePaymentObj.OnlinePaymentDetails.Add(oDObj);

                            ctx.SubmitChanges();
                        }

                        return onlinePaymentObj;
                    }
                    catch (Exception)
                    {
                        ctx.Transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'Insert OnlinePayment'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        public bool IsPaymentAlreadyMade(int onlinePaymentID)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from o in ctx.OnlinePayments
                                where o.OnlinePaymentId == onlinePaymentID
                                select o;
                    OnlinePayment onlinePayment = query.FirstOrDefault();
                    if (onlinePayment.PaymentId == null || onlinePayment.PaymentId == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'IsPaymentAlreadyMade'. OnlinePaymentID:{0}", onlinePaymentID);
                throw;
            }
        }

        /// <summary>
        /// Gets the online payment details.
        /// </summary>
        /// <param name="onlinePaymentID">The online payment ID.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        public List<OnlinePaymentDetail> GetOnlinePaymentDetails(int onlinePaymentID, List<EntityRelations> loadOptions)
        {
            if (onlinePaymentID <= 0)
            {
                return null;
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var opds = (from o in ctx.OnlinePaymentDetails
                                where o.OnlinePaymentId == onlinePaymentID
                                select o).ToList();

                    return opds;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetOnlinePaymentDetails'. OnlinePaymentID:{0}", onlinePaymentID);
                throw;
            }
        }

        public string ProcessCommOrderPayment(OnlinePayment onlinePayment)
        {
            if (onlinePayment == null)
            {
                throw new ArgumentException("OnlinePayment object is null");
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();
                        ctx.DeferredLoadingEnabled = false;
                        Decimal totalAmount = 0;

                        //Update CommOrders table
                        foreach (OnlinePaymentDetail invOrder in onlinePayment.OnlinePaymentDetails)
                        {
                            CommOrder comOrder = (from o in ctx.CommOrders
                                                  where o.OrderID == invOrder.OrderId
                                                  select o).SingleOrDefault();

                            comOrder.Due -= invOrder.AmountPaid;
                            comOrder.Paid += invOrder.AmountPaid;

                            ctx.SubmitChanges();

                            totalAmount += invOrder.AmountPaid;
                        }

                        //Insert record into Payment table
                        Payment payment = new Payment();
                        payment.AccountID = 1;
                        payment.PaymentDate = DateTime.Now;
                        payment.PayedBy = "Online";
                        payment.Notes = onlinePayment.CBATransactionRef;
                        payment.Amount = totalAmount;
                        payment.CreationDate = DateTime.Now;
                        payment.AccountCreditId = null;
                        payment.Bank = null;
                        payment.BankBranch = null;
                        payment.Drawer = null;

                        ctx.Payments.InsertOnSubmit(payment);
                        ctx.SubmitChanges();

                        //insert PaymentDetails table
                        foreach (OnlinePaymentDetail invOrder in onlinePayment.OnlinePaymentDetails)
                        {
                            PaymentDetail paymentDetail = new PaymentDetail();
                            paymentDetail.AmountPaid = invOrder.AmountPaid;
                            paymentDetail.OrderID = invOrder.OrderId;
                            paymentDetail.PaymentID = payment.PaymentID;

                            ctx.PaymentDetails.InsertOnSubmit(paymentDetail);
                            ctx.SubmitChanges();
                        }

                        //update OnlinePayment table
                        OnlinePayment onPayment = (from o in ctx.OnlinePayments
                                                   where o.OnlinePaymentId == onlinePayment.OnlinePaymentId
                                                   select o).SingleOrDefault();

                        onPayment.PaymentId = payment.PaymentID;
                        onPayment.TransactionComplete = onlinePayment.TransactionComplete;
                        onPayment.CBATransactionRef = onlinePayment.CBATransactionRef;
                        onPayment.CardType = onlinePayment.CardType;
                        ctx.SubmitChanges();
                        ctx.Transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        throw ex;
                    }
                }
                return "Success";
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateOnlinePayment'. OnlinePaymentID:{0}", onlinePayment.OnlinePaymentId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        public CustomerPaymentDetails GetCustomerDetails(int onlinePaymentID)
        {
            if (onlinePaymentID <= 0)
            {
                return null;
            }
            CustomerPaymentDetails cusPayDetails = new CustomerPaymentDetails();
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    List<EntityRelations> loadOptions = new List<EntityRelations>();
                    loadOptions.Add(EntityRelations.OnlinePayment_To_OnlinePaymentDetails);
                    loadOptions.Add(EntityRelations.CommOrder_To_Customer);

                    ctx.SetDataLoadOptions(loadOptions);

                    OnlinePayment onPayment = (from o in ctx.OnlinePayments
                                               where o.OnlinePaymentId == onlinePaymentID
                                               select o).SingleOrDefault();
                    if (onPayment != null)
                    {
                        cusPayDetails.PaymentID = onPayment.PaymentId.HasValue ? onPayment.PaymentId.Value : 0;
                        cusPayDetails.Surcharge = onPayment.Surcharge;
                        cusPayDetails.TotalAmount = onPayment.Total;

                        if (onPayment.OnlinePaymentDetails.Count > 0)
                        {
                            foreach (OnlinePaymentDetail item in onPayment.OnlinePaymentDetails)
                            {
                                CommOrder comOrder = (from o in ctx.CommOrders
                                                      where o.OrderID == item.OrderId
                                                      select o).SingleOrDefault();

                                if (comOrder != null && comOrder.Customer != null)
                                {
                                    CustomerDetails cusDetails = new CustomerDetails();
                                    cusDetails.Address = comOrder.Customer.Address;
                                    cusDetails.AmountPaid = item.AmountPaid;
                                    cusDetails.Name = comOrder.Customer.Name;
                                    cusDetails.Office = comOrder.Customer.Office;
                                    cusDetails.OrderID = item.OrderId;

                                    cusPayDetails.CustomerDetails.Add(cusDetails);
                                }
                            }
                            return cusPayDetails;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetCustomerDetails'. OnlinePaymentID:{0}", onlinePaymentID);
                throw;
            }
        }

        /// <summary>
        /// Gets the online payment.
        /// </summary>
        /// <param name="onlinePaymentID">The online payment ID.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        public OnlinePayment GetOnlinePayment(int onlinePaymentID, List<EntityRelations> loadOptions)
        {
            if (onlinePaymentID <= 0)
            {
                return null;
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    OnlinePayment op = (from o in ctx.OnlinePayments
                                        where o.OnlinePaymentId == onlinePaymentID
                                        select o).SingleOrDefault();

                    return op;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetOnlinePayment'. OnlinePaymentID:{0}", onlinePaymentID);
                throw;
            }
        }

        /// <summary>
        /// Gets the order invoice.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <param name="orderID">The order ID.</param>
        /// <param name="address">The address.</param>
        /// <returns>
        /// Returns A list of the Invoice-Detail Objects.
        /// </returns>
        public List<Order> GetAmountDueOrderInvoice(int clientID, int orderID, string address)
        {
            if (clientID <= 0)
            {
                return null;
            }

            using (AbcDataContext ctx = new AbcDataContext())
            {

                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.Order_To_Invoice);
                loadOptions.Add(EntityRelations.Order_To_Location);
                loadOptions.Add(EntityRelations.Order_To_Account);


                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                //if clients preference has not to show invoices online, then we return empty set...
                var cpref = ctx.ClientsPrefs.SingleOrDefault(x => x.ClientId == clientID && x.PrefID == ClientsPref.DoNotShowInvoicesOnline);
                if (cpref != null)
                {
                    if (cpref.BitValue.HasValue && cpref.BitValue.Value == true)
                    {
                        return new List<Order>();
                    }
                }

                int? temAccID = ctx.Clients.SingleOrDefault(x => x.ClientID == clientID).PersonalAccount;
                int accId = temAccID.HasValue ? (int)temAccID : 0;

                if (orderID <= 0 && address == null)
                {
                    List<Order> invoiceDetailVarObj = (from o in ctx.Orders
                                                       join i in ctx.Invoices on o.OrderID equals i.OrderID
                                                       join l in ctx.Locations on o.LocationID equals l.LocationID
                                                       join a in ctx.Accounts on o.BillTo equals a.AccountID
                                                       where i.DateInvoiced != null &&
                                                       a.ManagerAcc == false &&
                                                       o.BillTo == a.AccountID &&
                                                       o.BillTo == accId &&
                                                       o.BillTo > 0
                                                       orderby i.DateInvoiced descending
                                                       select o).ToList();

                    /// Filter only Due Invoices
                    return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);
                }
                else if (orderID > 0 && address == null)
                {
                    List<Order> invoiceDetailVarObj = (from o in ctx.Orders
                                                       join i in ctx.Invoices on o.OrderID equals i.OrderID
                                                       join l in ctx.Locations on o.LocationID equals l.LocationID
                                                       join a in ctx.Accounts on o.BillTo equals a.AccountID
                                                       where i.DateInvoiced != null &&
                                                       a.ManagerAcc == false &&
                                                       o.BillTo == a.AccountID &&
                                                       o.BillTo == accId &&
                                                       o.BillTo > 0 &&
                                                       i.OrderID == orderID
                                                       orderby i.DateInvoiced descending
                                                       select o).ToList();
                    /// Filter only Due Invoices
                    return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);
                }
                else if (orderID == 0 && address != null)
                {
                    List<Order> invoiceDetailVarObj = (from o in ctx.Orders
                                                       join i in ctx.Invoices on o.OrderID equals i.OrderID
                                                       join l in ctx.Locations on o.LocationID equals l.LocationID
                                                       join a in ctx.Accounts on o.BillTo equals a.AccountID
                                                       where i.DateInvoiced != null &&
                                                       a.ManagerAcc == false &&
                                                       o.BillTo == a.AccountID &&
                                                       o.BillTo == accId &&
                                                       o.BillTo > 0
                                                       && (o.PropertyAddress.Contains(address) || l.Location1.Contains(address))
                                                       orderby i.DateInvoiced descending
                                                       select o).ToList();
                    /// Filter only Due Invoices
                    return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);
                }
                else if (orderID > 0 && address != null)
                {
                    List<Order> invoiceDetailVarObj = (from o in ctx.Orders
                                                       join i in ctx.Invoices on o.OrderID equals i.OrderID
                                                       join l in ctx.Locations on o.LocationID equals l.LocationID
                                                       join a in ctx.Accounts on o.BillTo equals a.AccountID
                                                       where i.DateInvoiced != null &&
                                                       a.ManagerAcc == false &&
                                                       o.BillTo == a.AccountID &&
                                                       o.BillTo == accId &&
                                                       o.BillTo > 0 &&
                                                       (o.PropertyAddress.Contains(address) || l.Location1.Contains(address))
                                                       orderby i.DateInvoiced descending
                                                       select o).ToList();

                    /// Filter only Due Invoices
                    return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the amount due order invoice list.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        /// <param name="orderID">The order ID.</param>
        /// <param name="address">The address.</param>
        /// <returns>Returns the list of the InvoiceDetail</returns>
        public List<InvoiceDetail> GetAmountDueOrderInvoiceList(int clientID, int orderID, string address)
        {
            if (clientID <= 0)
            {
                return null;
            }

            using (AbcDataContext ctx = new AbcDataContext())
            {
                List<Order> invoiceDetailVarObj = null;

                List<EntityRelations> loadOptions = new List<EntityRelations>();
                loadOptions.Add(EntityRelations.Order_To_Invoice);
                loadOptions.Add(EntityRelations.Order_To_Location);
                loadOptions.Add(EntityRelations.Order_To_Account);
                loadOptions.Add(EntityRelations.Order_To_OrderOtherInfo);

                ctx.DeferredLoadingEnabled = false;
                ctx.SetDataLoadOptions(loadOptions);

                //if clients preference has not to show invoices online, then we return empty set...
                var cpref = ctx.ClientsPrefs.SingleOrDefault(x => x.ClientId == clientID && x.PrefID == ClientsPref.DoNotShowInvoicesOnline);
                if (cpref != null)
                {
                    if (cpref.BitValue.HasValue && cpref.BitValue.Value == true)
                    {
                        return new List<InvoiceDetail>();
                    }
                }
                
                int? temAccID = ctx.Clients.SingleOrDefault(x => x.ClientID == clientID).PersonalAccount;
                int accId = temAccID.HasValue ? (int)temAccID : 0;

                if (orderID <= 0 && address == null)
                {
                    invoiceDetailVarObj = (from o in ctx.Orders
                                           join i in ctx.Invoices on o.OrderID equals i.OrderID
                                           join l in ctx.Locations on o.LocationID equals l.LocationID
                                           join a in ctx.Accounts on o.BillTo equals a.AccountID
                                           where i.DateInvoiced != null &&
                                           a.ManagerAcc == false &&
                                           o.BillTo == a.AccountID &&
                                           o.BillTo == accId &&
                                           o.BillTo > 0
                                           orderby i.DateInvoiced descending
                                           select o).ToList();

                    /// Filter only Due Invoices
                    // return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);
                }
                else if (orderID > 0 && address == null)
                {
                    invoiceDetailVarObj = (from o in ctx.Orders
                                           join i in ctx.Invoices on o.OrderID equals i.OrderID
                                           join l in ctx.Locations on o.LocationID equals l.LocationID
                                           join a in ctx.Accounts on o.BillTo equals a.AccountID
                                           where i.DateInvoiced != null &&
                                           a.ManagerAcc == false &&
                                           o.BillTo == a.AccountID &&
                                           o.BillTo == accId &&
                                           o.BillTo > 0 &&
                                           i.OrderID == orderID
                                           orderby i.DateInvoiced descending
                                           select o).ToList();
                    /// Filter only Due Invoices
                    // return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);
                }
                else if (orderID == 0 && address != null)
                {
                    invoiceDetailVarObj = (from o in ctx.Orders
                                           join i in ctx.Invoices on o.OrderID equals i.OrderID
                                           join l in ctx.Locations on o.LocationID equals l.LocationID
                                           join a in ctx.Accounts on o.BillTo equals a.AccountID
                                           where i.DateInvoiced != null &&
                                           a.ManagerAcc == false &&
                                           o.BillTo == a.AccountID &&
                                           o.BillTo == accId &&
                                           o.BillTo > 0
                                           && (o.PropertyAddress.Contains(address) || l.Location1.Contains(address))
                                           orderby i.DateInvoiced descending
                                           select o).ToList();
                    /// Filter only Due Invoices
                    // return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);
                }
                else if (orderID > 0 && address != null)
                {
                    invoiceDetailVarObj = (from o in ctx.Orders
                                           join i in ctx.Invoices on o.OrderID equals i.OrderID
                                           join l in ctx.Locations on o.LocationID equals l.LocationID
                                           join a in ctx.Accounts on o.BillTo equals a.AccountID
                                           where i.DateInvoiced != null &&
                                           a.ManagerAcc == false &&
                                           o.BillTo == a.AccountID &&
                                           o.BillTo == accId &&
                                           o.BillTo > 0 &&
                                           (o.PropertyAddress.Contains(address) || l.Location1.Contains(address))
                                           orderby i.DateInvoiced descending
                                           select o).ToList();

                    // return invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);

                }
                else
                {
                    return null;
                }

                if (invoiceDetailVarObj == null || invoiceDetailVarObj.Count == 0)
                {
                    return null;
                }

                /// Filter only Due Invoices
                invoiceDetailVarObj.FindAll(x => x.Invoice.AmountDue > 0);

                List<InvoiceDetail> invoiceList = new List<InvoiceDetail>();

                /// change for display model.
                foreach (Order order in invoiceDetailVarObj)
                {
                    InvoiceDetail obj = new InvoiceDetail(order);
                    invoiceList.Add(obj);
                }

                return invoiceList;
            }
        }

        /// Gets the payment.
        /// </summary>
        /// <param name="onlinePaymentID">The online payment ID.</param>
        /// <param name="loadOptions">The load options.</param>
        /// <returns></returns>
        public Payment GetPayment(int onlinePaymentID, List<EntityRelations> loadOptions)
        {
            if (onlinePaymentID <= 0)
            {
                return null;
            }

            try
            {
                int? paymentID;

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    paymentID = ctx.OnlinePayments.SingleOrDefault(x => x.OnlinePaymentId == onlinePaymentID).PaymentId;
                }

                if (!paymentID.HasValue || paymentID <= 0)
                {
                    return null;
                }

                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;

                    if (loadOptions != null && loadOptions.Count > 0)
                        ctx.SetDataLoadOptions(loadOptions);

                    var op = (from p in ctx.Payments
                              where p.PaymentID == paymentID
                              select p).SingleOrDefault();

                    return op;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetOnlinePayment'. OnlinePaymentID:{0}", onlinePaymentID);
                throw;
            }
        }

        /// <summary>
        /// Processes the comm order payment.
        /// </summary>
        /// <param name="onlinePayment">The online payment.</param>
        /// <returns>
        /// Returns Payment Object.
        /// </returns>
        public Payment ProcessOnlinePayments(OnlinePayment onlinePayment)
        {
            if (onlinePayment == null)
            {
                throw new ArgumentException("OnlinePayment object is null");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();
                        ctx.DeferredLoadingEnabled = false;

                        /// Insert record into Payment table
                        Payment payment = new Payment();
                        payment.AccountID = onlinePayment.AccountId;
                        payment.PaymentDate = DateTime.Now;
                        payment.PayedBy = "Online";
                        payment.Notes = onlinePayment.CBATransactionRef;
                        payment.Amount = onlinePayment.Total;
                        payment.CreationDate = DateTime.Now;
                        payment.AccountCreditId = null;
                        payment.Bank = null;
                        payment.BankBranch = null;
                        payment.Drawer = null;

                        ctx.Payments.InsertOnSubmit(payment);
                        ctx.SubmitChanges();

                        /// insert PaymentDetails table
                        foreach (OnlinePaymentDetail invOrder in onlinePayment.OnlinePaymentDetails)
                        {
                            PaymentDetail paymentDetail = new PaymentDetail();
                            paymentDetail.AmountPaid = invOrder.AmountPaid;
                            paymentDetail.OrderID = invOrder.OrderId;
                            paymentDetail.PaymentID = payment.PaymentID;
                            ctx.PaymentDetails.InsertOnSubmit(paymentDetail);
                            ctx.SubmitChanges();
                            payment.PaymentDetails.Add(paymentDetail);
                        }

                        /// update OnlinePayment table
                        OnlinePayment onPayment = (from o in ctx.OnlinePayments
                                                   where o.OnlinePaymentId == onlinePayment.OnlinePaymentId
                                                   select o).SingleOrDefault();

                        onPayment.PaymentId = payment.PaymentID;
                        onPayment.TransactionComplete = onlinePayment.TransactionComplete;
                        onPayment.CBATransactionRef = onlinePayment.CBATransactionRef;
                        onPayment.CardType = onlinePayment.CardType;
                        ctx.SubmitChanges();
                        ctx.Transaction.Commit();

                        if (onlinePayment.TransactionComplete)
                        {
                            bool isUnblockAccExecuted = false;
                            foreach (OnlinePaymentDetail invOrder in onlinePayment.OnlinePaymentDetails)
                            {
                                if (!isUnblockAccExecuted)
                                {
                                    ctx.AIS_UnblockAccountForOnlinePayment(invOrder.OrderId, onlinePayment.AccountId);
                                    isUnblockAccExecuted = true;
                                }
                            }
                        }

                        return payment;
                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'UpdateOnlinePayment'. OnlinePaymentID:{0}", onlinePayment.OnlinePaymentId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        /// <summary>
        /// Verifies the invoice numbers.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="OrderId">The order id.</param>
        /// <returns>
        /// Returns True for valid, Otherwise False.
        /// </returns>
        public bool VerifyInvoiceNumbers(int clientId, List<int> orderIdList)
        {
            if (clientId <= 0 || orderIdList == null || orderIdList.Count == 0)
            {
                return false;
            }

            bool isValid = false;

            using (AbcDataContext ctx = new AbcDataContext())
            {
                ctx.DeferredLoadingEnabled = false;

                foreach (int orderId in orderIdList)
                {
                    int billTo = ctx.Orders.SingleOrDefault(x => x.OrderID == orderId).BillTo;
                    int? personalAccount = ctx.Clients.SingleOrDefault(x => x.ClientID == clientId).PersonalAccount;

                    if (!personalAccount.HasValue || billTo <= 0 || personalAccount <= 0 || personalAccount != billTo)
                    {
                        isValid = false;
                        break;
                    }

                    isValid = true;
                }

                return isValid;
            }
        }

        public int InsertCommOnlinePayment(OnlinePayment onlinePayment)
        {
            if (onlinePayment == null)
            {
                return 0;
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    ctx.OnlinePayments.InsertOnSubmit(onlinePayment);
                    ctx.SubmitChanges();

                    return onlinePayment.OnlinePaymentId;
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'SaveOnlinePayment'");
                Logger.Exception(ex, message);
                throw;
            }
        }

        /// <summary>
        /// Inserts the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <returns>Returns the payment.</returns>
        public Payment InsertPayment(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentException("Payment object is null");
            }

            int accountId = 0;
            int orderId = 0;
            accountId = payment.AccountID;
            if (payment.PaymentDetails != null && payment.PaymentDetails.Count > 0)
            {
                orderId = payment.PaymentDetails[0].OrderID;
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.Connection.Open();
                        ctx.Transaction = ctx.Connection.BeginTransaction();
                        ctx.DeferredLoadingEnabled = false;

                        // Insert record into Payment table                      
                        ctx.Payments.InsertOnSubmit(payment);
                        ctx.SubmitChanges();

                        // insert PaymentDetails table                        
                        payment.PaymentDetails.AddRange(payment.PaymentDetails);
                        ctx.SubmitChanges();
                        ctx.Transaction.Commit();

                        return payment;
                    }
                    catch (Exception ex)
                    {
                        ctx.Transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'InsertPayment'. AccountID:{0}, OrderID{1}", accountId, orderId);
                Logger.Exception(ex, message);
                throw;
            }
        }

        public OnlinePaymentExpressOrder InsertOnlinePaymentExpressOrder(OnlinePaymentExpressOrder objPaymentExpress)
        {
            if (objPaymentExpress == null)
            {
                throw new ArgumentException("OnlinePaymentExpressOrder object is null");
            }

            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    try
                    {
                        ctx.DeferredLoadingEnabled = false;

                        /// Insert record into OnlinePayment table
                        OnlinePaymentExpressOrder onlinePaymentObj = new OnlinePaymentExpressOrder();

                        //    onlinePaymentObj.TempOrderId = paygOrderDetails.TempOrderId;
                        onlinePaymentObj.PaymentAmount = objPaymentExpress.PaymentAmount;
                        onlinePaymentObj.IsPaymentDone = false;
                        onlinePaymentObj.Surcharge = objPaymentExpress.Surcharge;
                        onlinePaymentObj.PaymentResponse = string.Empty;
                        onlinePaymentObj.OrderData = objPaymentExpress.OrderData;
                        onlinePaymentObj.CardType = objPaymentExpress.CardType;
                        onlinePaymentObj.CreatedDate = DateTime.Now;
                        onlinePaymentObj.ClientId = objPaymentExpress.ClientId;
                        ctx.OnlinePaymentExpressOrders.InsertOnSubmit(onlinePaymentObj);
                        ctx.SubmitChanges();

                        int onlinePaymentId = onlinePaymentObj.Id;

                        if (onlinePaymentId <= 0)
                        {
                            return null;
                        }
                        return onlinePaymentObj;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'InsertOnlinePaymentExpressOrder'");
                Logger.Exception(ex, message);
                throw;
            }
        }
     
        public  OnlinePaymentExpressOrder GetOnlinePaymentExpressOrder(int TempOrderDetailsId)
        {
            if (TempOrderDetailsId <= 0)
            {
                return null;
            }
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    ctx.DeferredLoadingEnabled = false;
                    var opds = (from o in ctx.OnlinePaymentExpressOrders
                                where o.Id == TempOrderDetailsId
                                select o).SingleOrDefault();

                    return opds;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetOnlinePaymentExpressOrder'. OnlinePaymentID:{0}", TempOrderDetailsId);
                throw;
            }
        }

        #endregion
    }
}
