using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.Web;
using System.ComponentModel;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class ListingOrder
	{
		private DateTime? rentAvailableFrom;
		public ListingOrder()
		{
			DisplayPrice = "Display Price";
			LandUnitMeasure = "SquareMeter";
		}
		[DataMember]
		public bool DisplayAddress { get; set; }

		[DataMember]
		public string Price { get; set; }

		[DataMember]
		public string DisplayPrice { get; set; }

		[DataMember]
		public string DisplayThisPriceInstead { get; set; }

		[DataMember]
		public string RentAmount { get; set; }

		[DataMember]
		public string RentPer { get; set; }

		[DataMember]
		public bool ShowRent { get; set; }

		[DataMember]
		[XmlElement(IsNullable = true)]
		[DataType(DataType.Date)]
		public DateTime? RentAvailableFrom
		{
			get
			{
				return rentAvailableFrom;
			}
			set
			{
				rentAvailableFrom = value;
			}
		}

		//Note LandSize has been used as Building Size
		//Display will be building size
		[DataMember]
		public string LandSize { get; set; }

		[DataMember]
		public string LandUnitMeasure { get; set; }

		[DataMember]
		public string AnnualLease { get; set; }

		[DataMember]
		public int ListingTypeId { get; set; }

		[DataMember]
		public string PropertyTypeName { get; set; }
	}
}
