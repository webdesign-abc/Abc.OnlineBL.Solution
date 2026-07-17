using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;

namespace Abc.OnlineBL.Service.Implementation
{
    public class TemplateManagerService : ITemplateManagerService
    {
        #region ITemplateManagerService Members

        #region SayHello
        public string SayHello(string name)
        {
            string sName = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;
            return string.Format("Hello {0}. You are also {1}", name, sName);
        }
        #endregion

        #region GetClients
        /// <summary>
        /// Gets clients with AllowAOP field and Ceased Field.
        /// </summary>
        /// <returns></returns>
        public DataTable GetClients()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from c in ctx.Clients
								join u in ctx.UserLogons on c.ClientID equals u.ClientId
                                from cp in c.ClientsPrefs.Where(i => i.PrefID == ClientsPref.AllowAOP).DefaultIfEmpty()
								where u.ClientContactId == null
                                orderby c.ClientName, c.Office
                                select new
                                {
                                    c.ClientID,
                                    c.GroupId,
                                    c.ClientName,
                                    c.Office,
									u.UserName,
                                    AllowAOP = ((cp.BitValue.HasValue) ? cp.BitValue.Value : false),
                                    c.Ceased
                                };

                    DataTable dt = query.LINQToDataTable();
                    dt.TableName = "Clients";
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetClients'.");
                throw;
            }
        }
        #endregion

        #region GetClientsWhoCanOrderProduct
        /// <summary>
        /// Gets clients with AllowAOP and Ceased fields who can order a specific product.
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public DataTable GetClientsWhoCanOrderProduct(string clientName, int productId)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from c in ctx.Clients
								join u in ctx.UserLogons on c.ClientID equals u.ClientId
                                join pl in ctx.PriceLists on c.PriceListID equals pl.PriceListID
                                join pld in ctx.PriceListDetails on pl.PriceListID equals pld.PriceListID
                                from cp in c.ClientsPrefs.Where(i => i.PrefID == ClientsPref.AllowAOP).DefaultIfEmpty()
                                where c.ClientName == clientName && pld.ProductID == productId && pl.IsComplete
                                orderby c.ClientName, c.Office
                                select new
                                {
                                    c.ClientID,
                                    c.GroupId,
                                    c.ClientName,
                                    c.Office,
                                    u.UserName,
                                    AllowAOP = ((cp.BitValue.HasValue) ? cp.BitValue.Value : false),
                                    c.Ceased
                                };

                    DataTable dt = query.LINQToDataTable();
                    dt.TableName = "Clients";
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetClientsWhoCanOrderProduct'. ClientName:{0}, ProductId:{1}", clientName, productId);
                throw;
            }
        }
        #endregion

        #region GetInActiveClients
        public DataTable GetInActiveClients(int timeInMonth)
        {
            throw new NotImplementedException("");

            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //  Logger.Exception(ex, "Error occured in 'GetInActiveClients'. TimeInMonth:{0}", timeInMonth);
            //  throw;
            //}
        }
        #endregion

        #region UpdateAOPClientPref
        public void UpdateAOPClientPref(int clientId, bool isEnabledForAOP)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from p in ctx.ClientsPrefs
                                where p.ClientId == clientId && p.PrefID == ClientsPref.AllowAOP
                                select p;

                    ClientsPref pref = query.FirstOrDefault();
                    if (pref != null)
                    {
                        pref.BitValue = isEnabledForAOP;
                    }
                    else
                    {
                        pref = new ClientsPref() { ClientId = clientId, PrefID = ClientsPref.AllowAOP, BitValue = isEnabledForAOP };
                        ctx.ClientsPrefs.InsertOnSubmit(pref);
                    }

                    ctx.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'UpdateAOPClientPref'. ClientId:{0}, BitValue:{1}", clientId, isEnabledForAOP);
                throw;
            }
        }
        #endregion

        #region GetProductTypes
        public List<ProductType> GetProductTypes()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from pt in ctx.ProductTypes
                                where pt.TypeID == ProductTypes.BillBoard || pt.TypeID == ProductTypes.Brochure || pt.TypeID == ProductTypes.Stockboard || pt.TypeID == ProductTypes.SignShop || pt.TypeID == ProductTypes.ForPrinting
                                || pt.TypeID == ProductTypes.WindowCard || pt.TypeID == ProductTypes.Corflute || pt.TypeID == ProductTypes.StockboardOverlay || pt.TypeID == ProductTypes.DIYStickers
                                select pt;

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductTypes'.");
                throw;
            }
        }
        #endregion

        #region GetProductById
        public DataTable GetProductById(int productId)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from p in ctx.Products
                                join sc in ctx.ProductSizeCodeDetails on p.SizeCode equals sc.SizeCode
                                join pt in ctx.ProductTypes on p.TypeID equals pt.TypeID
                                where p.ProductID == productId && p.Active && p.SizeCode != null && p.ContentType != null
                                select new
                                {
                                    p.ProductID,
                                    p.Name,
                                    p.TypeID,
                                    p.SizeCode,
                                    p.FrameType,
                                    p.ContentType,
                                    p.Format,
                                    sc.SizeCodeOnWeb,
                                    sc.AvailableInLandscape,
                                    sc.AvailableInPortrait,
                                    sc.ImageRequirements_MinimumMegaPixels,
                                    sc.ImageRequirements_RecommendedMegaPixels,
                                    pt.Type
                                };

                    DataTable dt = query.LINQToDataTable();
                    dt.TableName = "Products";
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductById'. ProductId:{0}.", productId);
                throw;
            }
        }
        #endregion

        #region GetProductsByIds
        public DataTable GetProductsByIds(List<int> productIds)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from p in ctx.Products
                                join sc in ctx.ProductSizeCodeDetails on p.SizeCode equals sc.SizeCode
                                join pt in ctx.ProductTypes on p.TypeID equals pt.TypeID
                                where productIds.Contains(p.ProductID) && p.Active && p.SizeCode != null && p.ContentType != null
                                select new
                                {
                                    p.ProductID,
                                    p.Name,
                                    p.TypeID,
                                    p.SizeCode,
                                    p.FrameType,
                                    p.ContentType,
                                    p.Format,
                                    sc.SizeCodeOnWeb,
                                    sc.AvailableInLandscape,
                                    sc.AvailableInPortrait,
                                    sc.ImageRequirements_MinimumMegaPixels,
                                    sc.ImageRequirements_RecommendedMegaPixels,
                                    pt.Type
                                };

                    DataTable dt = query.LINQToDataTable();
                    dt.TableName = "Products";
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductsByIds'.");
                throw;
            }
        }
        #endregion

        #region GetProductsByTypeId
        public DataTable GetProductsByTypeId(int typeId)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var query = from p in ctx.Products
                                join sc in ctx.ProductSizeCodeDetails.Where(ii => ii.ProductTypeId == typeId) on p.SizeCode equals sc.SizeCode
                                join pt in ctx.ProductTypes on p.TypeID equals pt.TypeID
                                where p.TypeID == typeId && p.Active && p.SizeCode != null && p.ContentType != null
                                select new
                                {
                                    p.ProductID,
                                    p.Name,
                                    p.TypeID,
                                    p.SizeCode,
                                    p.FrameType,
                                    p.ContentType,
                                    p.Format,
                                    sc.SizeCodeOnWeb,
                                    sc.AvailableInLandscape,
                                    sc.AvailableInPortrait,
                                    sc.ImageRequirements_MinimumMegaPixels,
                                    sc.ImageRequirements_RecommendedMegaPixels,
                                    pt.Type
                                };

                    DataTable dt = query.LINQToDataTable();
                    dt.TableName = "Products";
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Error occured in 'GetProductsByTypeId'. TypeId:{0}.", typeId);
                throw;
            }
        }
        #endregion

        #region GetLastOrderId
        public int GetLastOrderId()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    var order = (from o in ctx.Orders
                                 select o.OrderID).Max();
                    return order;
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in 'GetLastOrderId()'");
                throw;
            }
        }
        #endregion

        #region GetClientGroups
        public Dictionary<int, string> GetClientGroups()
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    Dictionary<int, string> grpList = new Dictionary<int, string>();

                    var obj = from grp in ctx.ClientGroups select new { grp.GroupId, grp.GroupName };

                    if (obj != null)
                    {
                        grpList = obj.ToDictionary(key => key.GroupId, val => val.GroupName);
                    }
                    return grpList;
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in 'GetClientGroups'");
                throw;
            }
        }
        #endregion

        #region GetClientOfficeByGroupID
        public List<Entities.Client> GetClientOfficeByGroupID(int submittedGroupID)
        {
            try
            {
                if (submittedGroupID > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        List<Client> clientOfficeList = new List<Client>();

                        var obj = (from client in ctx.Clients
                                   where client.GroupId == submittedGroupID
                                   select client).ToList().Select(e =>
                                      new Client
                                      {
                                          ClientID = e.ClientID,
                                          ClientName = e.ClientName,
                                          Office = e.Office
                                      }).ToList();


                        return obj;
                    }
                }
                return null;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in 'GetClientOfficeByGroupID()'");
                throw;
            }
        }
        #endregion

        #region GetClientNameById
        public string GetClientNameById(int submittedClientID)
        {
            try
            {
                if (submittedClientID > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        return ctx.Clients.SingleOrDefault(client => client.ClientID == submittedClientID).Office.ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in 'GetClientNameById()'");
                throw;
            }
        }
        #endregion

        #region GetTemplateIDByTemplatepath
        public int? GetTemplateIDByTemplatepath(string submittedPath)
        {
            try
            {
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    int? ID = (from tpl in ctx.AOP_Templates where tpl.TemplatePath.Equals(submittedPath.Trim()) select tpl.TemplateId).SingleOrDefault();

                    if (ID != null && ID > 0)
                    {
                        return ID;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in 'GetTemplateIDByTemplatepath()'");
                throw;
            }
        }
        #endregion

        #region InsertDataAnalyzQueue
        public bool InsertDataAnalyzQueue(int? submittedTemplateID)
        {
            try
            {
                if (submittedTemplateID != null && submittedTemplateID > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        var getData = (from analyze in ctx.AOP_AnalyzeQueues where analyze.TemplateId == submittedTemplateID select analyze).SingleOrDefault();

                        if (getData == null)
                        {

                            AOP_AnalyzeQueue queueData = new AOP_AnalyzeQueue()
                            {
                                CreatorId = 0,
                                DateCreated = DateTime.Now,
                                QueueId = 0,
                                TemplateId = int.Parse(submittedTemplateID.ToString()),
                                StatusId = 0,
                                DateFinished = null,
                                StatusMessage = string.Empty
                            };

                            ctx.AOP_AnalyzeQueues.InsertOnSubmit(queueData);
                            ctx.SubmitChanges();
                        }
                        return true;
                    }
                }

                return false;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in 'GetTemplateIDByTemplatepath()'");
                throw;
            }
        }
        #endregion

        public string GetClientNOfficeNameByClientID(int submittedClientId)
        {
            try
            {
                if (submittedClientId > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        var clientInfo = (from client in ctx.Clients where client.ClientID == submittedClientId 
                                          select new { client.ClientName, client.Office }).SingleOrDefault();

                        return clientInfo == null ? string.Empty : clientInfo.ClientName + " " + clientInfo.Office;
                    }
                }

                return string.Empty;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in 'GetClientNOfficeNameByClientID()'");
                throw;
            }
        }

        public List<ClientGroup> GetClientByGrpId(int submittedGrpId)
        {
            try
            {
                if (submittedGrpId > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        var grp = (from clientGrp in ctx.ClientGroups
                                   join client in ctx.Clients on clientGrp.GroupId equals client.GroupId
                                   into tempPets
                                   from Clients in tempPets.DefaultIfEmpty()
                                   where clientGrp.GroupId == submittedGrpId
                                   select clientGrp).Distinct().ToList();

                        List<ClientGroup> lstClientGrp = new List<ClientGroup>();

                        if (grp != null)
                        {
                            foreach (var item in grp)
                            {                                
                                ClientGroup clntGrp = new ClientGroup { GroupId = item.GroupId, GroupName = item.GroupName };

                                if (item.Clients != null)
                                {
                                    foreach (var client in item.Clients)
                                    {
                                        Client clnt = new Client();
                                        clnt.GroupId = client.GroupId;
                                        clnt.ClientID = client.ClientID;
                                        clnt.ClientName = client.ClientName;
                                        clnt.Office = client.Office;
                                        clntGrp.Clients.Add(clnt);
                                    }
                                }
                                lstClientGrp.Add(clntGrp);
                            }
                        }
                        return lstClientGrp;
                    }
                }

                return null;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetClientByGrpId");
                throw oEx;
            }
        }

        public Entities.Client GetGrpNClientByClientId(int submittedClientId)
        {
            try
            {
                if (submittedClientId > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        var grp = (from client in ctx.Clients
                                   join clientGrp in ctx.ClientGroups on client.GroupId equals clientGrp.GroupId
                                   where client.ClientID == submittedClientId
                                   select client).SingleOrDefault();
                        
                        return new Client()
                        {
                            GroupId = grp.GroupId,
                            ClientGroup = grp.ClientGroup == null ? null : new ClientGroup { GroupId = grp.GroupId, GroupName = grp.ClientGroup.GroupName },
                            ClientID = grp.ClientID,
                            ClientName = grp.ClientName,
                            Office = grp.Office
                        };
                    }
                }

                return null;        
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetGrpClientByClientId()");
                throw oEx;
            }
        }

        public List<Entities.ClientGroup> GetClientsByGrpName(string submittedGrpName)
        {
            try
            {
                if (!string.IsNullOrEmpty(submittedGrpName))
                {                   
                        using (AbcDataContext ctx = new AbcDataContext())
                    {
                        var grp = (from clientGrp in ctx.ClientGroups
                                   join client in ctx.Clients on clientGrp.GroupId equals client.GroupId
                                    into tempPets
                                   from Clients in tempPets.DefaultIfEmpty()
                                   where clientGrp.GroupName.Contains(submittedGrpName.ToLower())
                                          orderby clientGrp.GroupId descending
                                   select clientGrp).Distinct().ToList();

                        List<ClientGroup> lstClientGrp = new List<ClientGroup>();

                        if (grp != null)
                        {
                            foreach (var item in grp)
                            {                                
                                ClientGroup clntGrp = new ClientGroup { GroupId = item.GroupId, GroupName = item.GroupName };

                                if (item.Clients != null)
                                {
                                    foreach (var client in item.Clients)
                                    {
                                        Client clnt = new Client();
                                        clnt.GroupId = client.GroupId;
                                        clnt.ClientID = client.ClientID;
                                        clnt.ClientName = client.ClientName;
                                        clnt.Office = client.Office;
                                        clntGrp.Clients.Add(clnt);
                                    }
                                }
                                lstClientGrp.Add(clntGrp);
                            }
                        }
                        return lstClientGrp;
                    }
                }

                return null;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "GetOrdersByPropertyAddress");
                throw oEx;
            }
        }

        public List<Entities.Client> GetClientsByOfficeName(string submittedOfficeName)
        {
            try
            {
                if (!string.IsNullOrEmpty(submittedOfficeName))
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {                        
                        var clientInfo = (from client in ctx.Clients
                                          where client.Office.Contains(submittedOfficeName.ToLower())
                                          orderby client.GroupId descending
                                          select client).ToList().Select(e => new Client
                                          {
                                              GroupId = e.GroupId,
                                              ClientGroup = e.ClientGroup == null ? null : new ClientGroup { GroupId = e.ClientGroup.GroupId, GroupName = e.ClientGroup.GroupName },
                                              ClientID = e.ClientID,
                                              ClientName = e.ClientName,
                                              Office = e.Office
                                          }).ToList();
                        return clientInfo;
                    }
                }

                return null;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetClientsByOfficeName()");
                throw oEx;
            }
        }
        
        public bool? GetAOPClientPrefByClientID(int submittedClientId)
        {
            try
            {
                if (submittedClientId > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        bool? isEnableAOP = (from clientPref in ctx.ClientsPrefs
                                            where clientPref.ClientId == submittedClientId
                                            select clientPref.BitValue).SingleOrDefault();

                        return isEnableAOP;
                    }
                }

                return false;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetAOPClientPref()");
                throw oEx;
            }
        }

        public List<int> GetClientsIDByGroupID(int submittedGrpID)
        {
            try
            {
                if (submittedGrpID > 0)
                {
                    using (AbcDataContext ctx = new AbcDataContext())
                    {
                        List<int> clientIDList = (from client in ctx.Clients where client.GroupId == submittedGrpID select client.ClientID).ToList();

                        return clientIDList;
                    }
                }

                return null;
            }
            catch (Exception oEx)
            {
                Logger.Exception(oEx, "Error Occured in GetClientsIDByGroupID()");
                throw oEx;
            }
        }

        #region ITemplateManagerService Members

        //string ITemplateManagerService.SayHello(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //DataTable ITemplateManagerService.GetClients()
        //{
        //    throw new NotImplementedException();
        //}

        //DataTable ITemplateManagerService.GetClientsWhoCanOrderProduct(string clientName, int productId)
        //{
        //    throw new NotImplementedException();
        //}

        //DataTable ITemplateManagerService.GetInActiveClients(int timeInMonth)
        //{
        //    throw new NotImplementedException();
        //}

        //void ITemplateManagerService.UpdateAOPClientPref(int clientId, bool isEnabledForAOP)
        //{
        //    throw new NotImplementedException();
        //}

        //List<ProductType> ITemplateManagerService.GetProductTypes()
        //{
        //    throw new NotImplementedException();
        //}

        //DataTable ITemplateManagerService.GetProductById(int productId)
        //{
        //    throw new NotImplementedException();
        //}

        //DataTable ITemplateManagerService.GetProductsByIds(List<int> productIds)
        //{
        //    throw new NotImplementedException();
        //}

        //DataTable ITemplateManagerService.GetProductsByTypeId(int typeId)
        //{
        //    throw new NotImplementedException();
        //}

        //int ITemplateManagerService.GetLastOrderId()
        //{
        //    throw new NotImplementedException();
        //}

        //Dictionary<int, string> ITemplateManagerService.GetClientGroups()
        //{
        //    throw new NotImplementedException();
        //}

        //List<Client> ITemplateManagerService.GetClientOfficeByGroupID(int submittedGroupID)
        //{
        //    throw new NotImplementedException();
        //}

        //string ITemplateManagerService.GetClientNameById(int submittedClientID)
        //{
        //    throw new NotImplementedException();
        //}

        //int? ITemplateManagerService.GetTemplateIDByTemplatepath(string submittedPath)
        //{
        //    throw new NotImplementedException();
        //}

        //bool ITemplateManagerService.InsertDataAnalyzQueue(int? submittedTemplateID)
        //{
        //    throw new NotImplementedException();
        //}

        //string ITemplateManagerService.GetClientNOfficeNameByClientID(int submittedClientId)
        //{
        //    throw new NotImplementedException();
        //}

        //List<ClientGroup> ITemplateManagerService.GetClientByGrpId(int submittedGrpId)
        //{
        //    throw new NotImplementedException();
        //}

        //Client ITemplateManagerService.GetGrpNClientByClientId(int submittedClientId)
        //{
        //    throw new NotImplementedException();
        //}

        //List<Client> ITemplateManagerService.GetClientsByGrpName(string submittedGrpName)
        //{
        //    throw new NotImplementedException();
        //}

        //List<Client> ITemplateManagerService.GetClientsByOfficeName(string submittedOfficeName)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
        #endregion
}
