using System;
using Abc.Business.Entities;

namespace Abc.AIS.Orders.Workflow
{
	[Serializable]
	public class OrderData
	{
		#region Product Type ID's
		public const int BOARD_TYPE_ID = 1;
		public const int STOCK_BOARD_TYPE_ID = 4;
		public const int BROCHURE_TYPE_ID = 2;
		public const int WC_TYPE_ID = 17;
		public const int PHOTO_TYPE_ID = 8;
		public const int SMS_TYPE_ID = 16;
		public const int ERECTIONFEE_TYPE_ID = 19;
		public const int SPOTLIGHT_TYPE_ID = 19;
		#endregion

		public Abc.Business.Entities.Property Property { get; set; }
		public int OrderId { get; set; }
		public int StockId { get; set; }
		public int PhotoOrderId { get; set; }
		public int ListingId { get; set; }

		public string ConnectionString { get; set; }
		public string OrderDir { get; set; }
		public string MappedPath { get; set; }
		public Abc.AIS.Entities.Client Client { get; set; }
		public ClientInfo ClientInfo { get; set; }

		public string StrOrder
		{
			get
			{
				return Abc.Util.Runtime.Serializer.BinarySerialize(Property);

			}
		}

		#region StringToObject
		public static Property StringToObject(string strObj)
		{
			return (Property)Abc.Util.Runtime.Serializer.BinaryDeserialize(strObj);
		}
		#endregion
	}
}
