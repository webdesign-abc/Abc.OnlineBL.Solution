using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities
{
	public partial class ProductRule
	{
		private string _ManagerName;

		[DataMember]
		public string ManagerName
		{
			get
			{
				return _ManagerName;
			}
			set
			{
				_ManagerName = value;
				SendPropertyChanged("ManagerName");
			}
		}

		private string _ClientNameOffice;

		[DataMember]
		public string ClientNameOffice
		{
			get
			{
				return _ClientNameOffice;
			}
			set
			{
				_ClientNameOffice = value;
				SendPropertyChanged("ClientNameOffice");
			}
		}

		private string _PricingType;

		[DataMember]
		public string PricingType
		{
			get
			{
				return _PricingType;
			}
			set
			{
				_PricingType = value;
				SendPropertyChanged("PricingType");
			}
		}

		private string _BusinessRegionName;

		[DataMember]
		public string BusinessRegionName
		{
			get
			{
				return _BusinessRegionName;
			}
			set
			{
				_BusinessRegionName = value;
				SendPropertyChanged("BusinessRegionName");
			}
		}
	}
}
