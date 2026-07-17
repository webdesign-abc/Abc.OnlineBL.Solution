
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using System;
namespace Abc.OnlineBL.Orders.Workflow.Model
{
	public class NewOrderEvent
	{
		public int OrderId { get; set; }
        public int ClientId { get; set; }
        public int GroupId { get; set; }
		public string HtmlBody { get; set; }
		public string HtmlBodyUS { get; set; }
		public string FileName { get; set; }
		public string Prop { get; set; }
		
		public string PhotoEmail { get; set; }
        public bool OrderHasOverlay { get; set; }
        public bool OrderHasOverlayExcludeUnitStickerAndNamePlates { get; set; }
        public bool InterstateOrderHasOverlayExcludeUnitStickerAndNamePlates { get; set; }
        public bool OrderHasStockboard { get; set; }
        public string Notes { get; set; }
        public string ManagerID { get; set; }
        public OrderType OrderType { get; set; }
        public bool OrderHasMudMapOrReDrawFloorplan { get; set; }
        public bool OrderHasVirtualWalkThrough { get; set; }
        public bool OrderHasProfessionalSlideshowVideo { get; set; }
        public string InstallFile { get; set; }

        public bool SendPhotoEmailToAdmin { get; set; }
        public bool SendJobsheetToPrintTeam { get; set; }
        public string SendProofToEmail { get; set; }
        public bool OrderHasUnitStickerForStockBoardProduct { get; set; }
        public bool InterstateOrderHasOverlayOrUnitStickerOrNamePlates { get; set; }
    }
}
