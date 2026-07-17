using System;
namespace Abc.OnlineBL.Entities.Model
{
    public interface IOnlineBL_GetAllAvailableOnlineOrderProductIds_Products
    {
        bool ApplyGST { get; set; }
        int CategoryId { get; set; }
        string CategoryName { get; set; }
        string ContentType { get; set; }
        string CustomDescription { get; set; }
        string CustomName { get; set; }
        int? FavId { get; set; }
        string Format { get; set; }
        string FrameType { get; set; }
        bool? IsCustomPrice { get; set; }
        bool Myop { get; set; }
        string Name { get; set; }
        string ProductDescription { get; set; }
        int ProductId { get; set; }
        decimal? ProductPrice { get; set; }
        int? Qty { get; set; }
        string SizeCode { get; set; }
        string SizeCodeOnWeb { get; set; }
        int TypeID { get; set; }
        bool UsePackageContentPrice { get; set; }
        string WebFriendlyName { get; set; }
        int SortOrder { get; set; }
    }
}
