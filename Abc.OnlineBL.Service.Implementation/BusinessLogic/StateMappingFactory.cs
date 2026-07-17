using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Service.Implementation.Model;

namespace Abc.OnlineBL.Service.Implementation.BusinessLogic
{
	public class StateMappingFactory
	{
		private static StateMappingFactory singleton;

		private OrderStateMapping orderMapping;
		private BoardStateMapping boardMapping;
		private BrochureStateMapping brochureMapping;
		private WebServiceStateMapping webservice;

		private StateMappingFactory()
		{
			orderMapping = new OrderStateMapping();
			boardMapping = new BoardStateMapping();
			brochureMapping = new BrochureStateMapping();
			webservice = new WebServiceStateMapping();
		}

		public static StateMappingFactory Current
		{
			get
			{
				if (singleton == null)
					singleton = new StateMappingFactory();

				return singleton;
			}
		}

		public StateMapping GetStateMapping(int productTypeId)
		{
			switch (productTypeId)
			{
				case 0:
					return orderMapping;
				case 1:
				case 14:
					return boardMapping;
				case 2:
				case 17:
					return brochureMapping;
				case 13:
					return webservice;
				default:
					return null;
			}
		}
	}
}
