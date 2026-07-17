using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace Abc.OnlineBL.Entities.Model
{
    public class OnlineBL_GetAllAvailableOnlineOrderProductIds_Products : Abc.OnlineBL.Entities.Model.IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string WebFriendlyName { get; set; }
        public string ProductDescription { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TypeID { get; set; }
        public bool Myop { get; set; }
        public bool UsePackageContentPrice { get; set; }
        public string ContentType { get; set; }
        public string FrameType { get; set; }
        public int? Qty { get; set; }
        public string CustomName { get; set; }
        public string Format { get; set; }
        public string SizeCode { get; set; }
        public string SizeCodeOnWeb { get; set; }
        public int? FavId { get; set; }
        public bool ApplyGST { get; set; }
        public decimal? ProductPrice { get; set; }
        public bool? IsCustomPrice { get; set; }
        public string CustomDescription { get; set; }
        public int SortOrder { get; set; }
    }

    public class OnlineBL_GetAllAvailableOnlineOrderProductIds_PackageProducts : Abc.OnlineBL.Entities.Model.IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string WebFriendlyName { get; set; }
        public string ProductDescription { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TypeID { get; set; }
        public bool Myop { get; set; }
        public bool UsePackageContentPrice { get; set; }
        public string ContentType { get; set; }
        public string FrameType { get; set; }
        public int? Qty { get; set; }
        public string CustomName { get; set; }
        public string Format { get; set; }
        public string SizeCode { get; set; }
        public string SizeCodeOnWeb { get; set; }
        public int? FavId { get; set; }
        public bool ApplyGST { get; set; }
        public decimal? ProductPrice { get; set; }
        public bool? IsCustomPrice { get; set; }
        public string CustomDescription { get; set; }

        public string ItemNotes { get; set; }
        public int PackageContentGroupId { get; set; }
        public int GroupId { get; set; }
        public int SortOrder { get; set; }
    }
}
