using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
    public class OnlineCategoryAndProductModel
    {
        public OnlineCategoryAndProductModel()
        {
            RootCategories = new List<OnlineCategoryNode>();
            FavCategories = new List<OnlineCategoryNode>();
        }

        public List<OnlineCategoryNode> RootCategories { get; set; }
        public List<OnlineCategoryNode> FavCategories { get; set; }
        public List<OnlineOrder.OnlineProduct> Products { get; set; }

        public void AddOnlineOrderProduct(OnlineOrder.OnlineProduct op, IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products dbModel)
        {
            ParseProductAndMakeTree(RootCategories, op, dbModel);
            if (dbModel.FavId.HasValue && dbModel.FavId.Value > 0)
            {
                ParseProductAndMakeTree(FavCategories, op, dbModel);
            }
        }

        private void ParseProductAndMakeTree(List<OnlineCategoryNode> root, OnlineOrder.OnlineProduct op, IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products dbModel)
        {
            var cat = (from c in root
                       where c.NodeId == dbModel.CategoryId
                       select c).FirstOrDefault();

            if (cat == null)
            {
                cat = new OnlineCategoryNode() { NodeId = dbModel.CategoryId, NodeName = dbModel.CategoryName, SortOrder = dbModel.SortOrder, NodeType = "Category" };
                root.Add(cat);
            }

            cat.ProductCount += 1;
            cat.ProductIds.Add(dbModel.ProductId);

            if (!cat.ContainsFavouriteProducts)
            {
                if (dbModel.FavId.HasValue && dbModel.FavId.Value > 0)
                {
                    cat.ContainsFavouriteProducts = true;
                }
            }

            if (dbModel.CategoryName.Contains("Billboards") || dbModel.CategoryName.Contains("Stockboards"))
            {
                //first by Content Type
                var cType = ParseContentType(dbModel, cat);
                if (cType != null)
                {
                    //then by Size Code
                    var cSizeCode = ParseSizeCode(dbModel, cType);
                    if (cSizeCode != null)
                    {
                        //then by Frame Type
                        var pFrameType = ParseFrameType(dbModel, cSizeCode);
                        if (pFrameType != null)
                        {
                            //then by Format
                            ParseFormat(dbModel, pFrameType);
                        }
                    }
                }
            }
            else if (dbModel.CategoryName.Contains("Window Cards"))
            {
                //first Content Type
                var cType = ParseContentType(dbModel, cat);
                if (cType != null)
                {
                    //then by Size Code
                    ParseSizeCode(dbModel, cType);
                }
            }
            else if (dbModel.CategoryName.Contains("Brochure"))
            {
                //size code
                var sCode = ParseSizeCode(dbModel, cat);
                if (sCode != null)
                {
                    //then content type
                    var cType = ParseContentType(dbModel, sCode);
                    if (cType != null)
                    {
                        //then Qty
                        var pQty = ParseQty(dbModel, cType);
                        if (pQty != null)
                        {
                            //then format
                            ParseFormat(dbModel, pQty);
                        }
                    }
                }
            }
        }

        private static OnlineCategoryNode ParseSizeCode(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products dbModel, OnlineCategoryNode parent)
        {            
            if (!string.IsNullOrEmpty(dbModel.SizeCodeOnWeb))
            {
                OnlineCategoryNode sCode = (from ct in parent.Childs
                                            where ct.NodeName == dbModel.SizeCodeOnWeb
                                            select ct).FirstOrDefault();

                if (sCode == null)
                {
                    sCode = new OnlineCategoryNode() { NodeId = dbModel.CategoryId, NodeName = dbModel.SizeCodeOnWeb, SortOrder = dbModel.SortOrder, NodeType = "SizeCode" };
                    parent.Childs.Add(sCode);
                    parent.SortChilds();
                }

                sCode.ProductCount += 1;
                sCode.ProductIds.Add(dbModel.ProductId);

                return sCode;
            }
            else
            {
                return null;
            }
        }

        private static OnlineCategoryNode ParseContentType(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products dbModel, OnlineCategoryNode parent)
        {
            if (!string.IsNullOrEmpty(dbModel.ContentType))
            {
                var cType = (from ct in parent.Childs
                             where ct.NodeName == dbModel.ContentType
                             select ct).FirstOrDefault();

                if (cType == null)
                {
                    cType = new OnlineCategoryNode() { NodeId = dbModel.CategoryId, NodeName = dbModel.ContentType, SortOrder = dbModel.SortOrder, NodeType = "ContentType" };
                    parent.Childs.Add(cType);
                    parent.SortChilds();
                }

                cType.ProductCount += 1;
                cType.ProductIds.Add(dbModel.ProductId);
                return cType;
            }
            else
            {
                return null;
            }
        }

        private static OnlineCategoryNode ParseQty(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products dbModel, OnlineCategoryNode parent)
        {
            if (dbModel.Qty.HasValue)
            {
                OnlineCategoryNode pQTy = (from ct in parent.Childs
                                            where ct.NodeName == dbModel.Qty.Value.ToString()
                                            select ct).FirstOrDefault();

                if (pQTy == null)
                {
                    pQTy = new OnlineCategoryNode() { NodeId = dbModel.CategoryId, NodeName = dbModel.Qty.Value.ToString(), SortOrder = dbModel.SortOrder, NodeType = "Qty" };
                    parent.Childs.Add(pQTy);
                    parent.SortChilds();
                }

                pQTy.ProductCount += 1;
                pQTy.ProductIds.Add(dbModel.ProductId);
                return pQTy;
            }
            else
            {
                return null;
            }
        }

        private static OnlineCategoryNode ParseFormat(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products dbModel, OnlineCategoryNode parent)
        {
            if (!string.IsNullOrEmpty(dbModel.Format))
            {
                var pFormat = (from ct in parent.Childs
                             where ct.NodeName == dbModel.Format
                             select ct).FirstOrDefault();

                if (pFormat == null)
                {
                    pFormat = new OnlineCategoryNode() { NodeId = dbModel.CategoryId, NodeName = dbModel.Format, SortOrder = dbModel.SortOrder, NodeType = "Format" };
                    parent.Childs.Add(pFormat);
                    parent.SortChilds();
                }

                pFormat.ProductCount += 1;
                pFormat.ProductIds.Add(dbModel.ProductId);
                return pFormat;
            }
            else
            {
                return null;
            }
        }

        private static OnlineCategoryNode ParseFrameType(IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products dbModel, OnlineCategoryNode parent)
        {
            if (!string.IsNullOrEmpty(dbModel.FrameType))
            {
                var pFrameType = (from ct in parent.Childs
                               where ct.NodeName == dbModel.FrameType
                               select ct).FirstOrDefault();

                if (pFrameType == null)
                {
                    pFrameType = new OnlineCategoryNode() { NodeId = dbModel.CategoryId, NodeName = dbModel.FrameType, SortOrder = dbModel.SortOrder, NodeType = "FrameType" };
                    parent.Childs.Add(pFrameType);
                    parent.SortChilds();
                }

                pFrameType.ProductCount += 1;
                pFrameType.ProductIds.Add(dbModel.ProductId);
                return pFrameType;
            }
            else
            {
                return null;
            }
        }
    }    

    public class OnlineCategoryNode
    {
        public OnlineCategoryNode()
        {
            Childs = new List<OnlineCategoryNode>();
            ProductIds = new List<int>();            
        }

        public int NodeId { get; set; }
        public string NodeType { get; set; }
        public string NodeName { get; set; }
        public int SortOrder { get; set; }
        public bool ContainsFavouriteProducts { get; set; }
        public int ProductCount { get; set; }
        public List<OnlineCategoryNode> Childs { get; set; }
        public List<int> ProductIds { get; set; }        

        public void SortChilds()
        {
            if (Childs.Count > 0)
            {
                Childs.Sort((node1, node2) => { return node1.NodeName.CompareTo(node2.NodeName); });
            }
        }
    }
}
