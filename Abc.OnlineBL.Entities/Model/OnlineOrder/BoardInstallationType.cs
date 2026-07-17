using System;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public enum BoardInstallationType
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		Standard = 1,
		/// <summary>
		/// High, but still lower than the 1st level.
		/// </summary>
		[EnumMember]
		High = 2,
		[EnumMember]
		HigherThanFirstLevel = 3,
		[EnumMember]
		BoardToBeROnlineBLed500mmOfTheGround  = 4,
		[EnumMember]
		BoardToBeROnlineBLed1000mmOfTheGround = 5,
		[EnumMember]
		BoardToBeROnlineBLed1250mmOfTheGround = 6,
		[EnumMember]
		BoardToBeROnlineBLed2000mmOfTheGround = 7,
	}

	[Serializable]
	[DataContract]
	public enum OrderType
	{
		[EnumMember]
		None = -1,
		[EnumMember]
		DIY = 0,
		[EnumMember]
		AbcDesign = 1,
		[EnumMember]
		ProvideArtwork = 2,
		[EnumMember]
		B2B = 3
	}

	[Serializable]
	[DataContract]
	public enum MarketingDeliveryType
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		ToOFfice = 1,
		[EnumMember]
		OtherAddress = 2,
        [EnumMember]
        PickUp = 3,
        [EnumMember]
        PickupDropoffAframe = 4,
        [EnumMember]
        DeliverToOfficePickupWhenReady = 5
    }

	[Serializable]
	[DataContract]
	public enum PreferredDateType
	{
		[EnumMember]
		Before = -1,
		[EnumMember]
		On = 0,
		[EnumMember]
		After = 1,
		[EnumMember]
		NotSelected = 2
	}

	[Serializable]
	[DataContract]
	public struct SizeType
	{
		public string Code;
		public string Height;
		public string Width;
		public string Unit;
	}

	public enum OrderDisplayType
	{
		WholeOrder = 1,
		NormalOrderOnly,
		PhotographyOrderOnly,
		StockboardOrderOnly
	}

    public enum DeliveryType
    {
        Office,
        Property,
        Alternate
    }


	public static class EnumExtensions
	{
		public static string Description(this PreferredDateType pdt)
		{
			switch (pdt)
			{
				case PreferredDateType.Before:
					return "Before or On";
				case PreferredDateType.On:
					return "On";
				case PreferredDateType.After:
					return "On or After";
				case PreferredDateType.NotSelected:
					break;
				default:
					break;
			}
			return "Not Selected";
		}

		public static int GetIndex(this PreferredDateType pdt)
		{
			switch (pdt)
			{
				case PreferredDateType.Before:
					return 0;
				case PreferredDateType.On:
					return 1;
				case PreferredDateType.After:
					return 2;
				case PreferredDateType.NotSelected:
					break;
				default:
					break;
			}
			return -1;
		}
	}
}
