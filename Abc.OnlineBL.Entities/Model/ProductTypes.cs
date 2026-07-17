using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
	[DataContract]
	[Serializable]
	public class ProductTypes
	{
		public const int BillBoard = 1;
		public const int Brochure = 2;
		public const int Other = 3;
		public const int Stockboard = 4;
		public const int SignShop = 5;
		public const int Printing = 6;
		public const int Artwork = 7;
		public const int Photography = 8;
		public const int Overlay = 9;
		public const int BoardAccessory = 10;
		public const int FloorPlans = 11;
		public const int WebServices = 13;
		public const int BoardPackages = 14;
		public const int OtherPackages = 15;
		public const int ForPrinting = 16;
		public const int WindowCard = 17;
		public const int Corflute = 18;
        public const int Packages = 25;
        public const int BillboardOverlay = 27;
        public const int StockboardOverlay = 27;
        public const int DIYStickers = 28;
    }

    [DataContract]
    [Serializable]
    public class CategoryTypes
    {
        public const int Billboard = 1;
        public const int Stockboard = 2;
        public const int Brochure = 3;
        public const int Packages = 4;
        public const int WindowCard = 5;
        public const int Overlays = 6;
        public const int Spotlight = 8;
        public const int MiscellaneousProducts = 12;
        public const int Corflute = 15;
    }

    [DataContract]
    [Serializable]
    public class ProductSettings
    {
        public const int Wings1200 = 142;
        public const int FlagHolder = 429;
        public const int UnitSticker = 880;
        public const int Proof = 1339;
        public const int KeyCollectionForPhotographyJobs = 1355;
        public const int Travel = 1381;
        public const int NamePlatesStockBoards = 1566;
        public const int ColourisationOfFloorPlanOrSitePlanStandard = 2652;
        public const int FourByThreeFlatpack = 3162;
        public const int SixByFourFlatpack = 3163;
        public const int RedrawFloorPlan = 4634;
        public const int MudMap = 7878;
        public const int ThreeDFloorPlan = 8045;
        public const int NamePlatesABoard4x3StockBoards = 8191;
        public const int NamePlatesCBoard6x4StockBoards = 8198;
        public const int ThreeDFloorPlanBlackWhiteStandardFloorplan = 8397;
        public const int ThreeDFloorPlanColourStandardFloorplan = 8398;
        public const int PlatinumDayPhotography_UpTo10FinalImages = 9349;
        public const int PlatinumDuskPhotography_UpTo10FinalImages = 9350;
        public const int PlatinumDayPhotographyUpTo5FinalImages = 10020;
        public const int PlatinumDayPhotographyUpTo8FinalImages = 10021;
        public const int PlatinumDayPhotographyUpTo12FinalImages = 10022;
        public const int UnitStickerForStockBoards = 12235;
        public const int NamePlatesDBoard8x4StockBoards = 13741;
        public const int EnduroAframereskinpair = 4615;
        public const int AFrameReskin900x600Pair = 14553;
        public const int AFrameReskin600x600Pair = 16171;
        public const int AFramereskinDeliveryFeePickupDropoff = 15596;
    }

    [DataContract]
    [Serializable]
    public class ClientSettings
    {
        public const int DowlingMedowie = 763;
        public const int LJHookerRaymondTerrace = 3897;
        public const int BuyMyPlaceSouthMelbourne = 10901;
        public const int CraigCurriePakenham = 15757;
        public const int BarryPlantBendigo = 14578;
        public const int McKeanMcGregorBendigo = 15078;
    }

    [DataContract]
    [Serializable]
    public class EventSettings
    {
        public const int ChangeAndReproof = 13;
        public const int ChangeAndApprove = 14;
        public const int OnlineOrderInternal = 16;
        public const int ErectionNotesChanged = 31;
        public const int FileUpload = 33;
        public const int RemovalNotesChanged = 34;
        public const int StockboardProgressToManagers = 38;
        public const int AgentsCommentsRegardingJob = 39;
        public const int WorkshopStockBoardOrderReceived = 48;
        public const int CancelAOPJobs = 54;
        public const int ImageNotAcceptable = 59;
        public const int TextUpload = 61;
        public const int MarkOffRSSummary = 63;
        public const int NoBoardFoundForRemoval = 64;
        public const int InstallationOrRemovalNotesReceivedFromDriver = 65;
        public const int NewPickingSlip = 66;
        public const int SiteInspectionRequired = 67;
        public const int ArtworkUploaded = 68;
        public const int NoInstallationImageTaken = 69;
        public const int UnableToErectRemoveBoard = 70;
        public const int ClientProfileStatus = 74;
        public const int ClientWantsDIYTemplate = 75;
        public const int MarkOffRSSummaryForMainWorkshop = 77;
        public const int ModifyDIYOrder = 78;
        public const int ModifyDIYOrderInternal = 79;
        public const int ABCSpotlightNotFoundWhenRemovingBoard = 80;
        public const int UpdateAgentDetailsRequest = 82;
        public const int HighInstallationProductNeedSetup = 86;
        public const int RequestTemplateChanges = 87;
        public const int OrderHas3DLetteringBoard = 90;
        public const int RequestPackChanges = 91;
        public const int DronePhotographyStatusChange = 93;
        public const int FloorplanDraftImage = 94;
        public const int CommunityBoardRequestTemplateChanges = 96;
        public const int ChangeOrderTemplates = 97;
        public const int ChangeToConjunctionalOrder = 98;
        public const int BoardAndStockboard = 100;
        public const int OrderHasMudMapReDraw = 104;
        public const int B2BAutoApprovalFailed = 105;
        public const int ServiceLocationRequest = 107;
        public const int ABCREPropertyListing = 108;
        public const int PhotographyCompletedStockboardInventory = 111;
        public const int BoardRentalExtension = 113;
        public const int StockboardOrderInAPack = 118;
        public const int RequestOneOffCustomDesign = 119;
        public const int RequestTextOverflow = 120;
        public const int RequestImageCropping = 121;
        public const int ProfessionalSlideshowVideo = 126;
        public const int UnitStickerforStockBoards = 127;
        public const int DIYStockBoardOrderReceived = 141;
        public const int AutomateStockboardOrderReceived = 146;

    }

    [DataContract]
    [Serializable]
    public class ManagerSettings
    {
        public const string WorkshopSouthAustralia = "WSOUT";
        public const string WorkshopVictoria = "WORKS";
        public const string SignshopVictoria = "SIGNSHOP";
        public const string TrishNewcastle = "MTPNEW";
        public const string WorkshopQueensland = "QWORK";
        public const string WorkshopNewSouthWales = "WNEWS";
        public const string WorkshopWesternAustralia = "WORWA";
        public const string ConCostiWollongong = "COCON";
        public const string InHouse = "INHOU";
        public const string GarryPrince = "GAPNEW";

    }

    [DataContract]
    [Serializable]
    public class ClientGroupSettings
    {
        public const int OxbridgePropertyGroup = 258; // 257 for Bells sunshine

    }
}
