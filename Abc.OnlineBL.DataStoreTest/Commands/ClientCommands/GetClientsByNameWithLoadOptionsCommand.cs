using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.ServiceProxy;

namespace Abc.OnlineBL.DataStoreTest.Commands.ClientCommands
{
	public class GetClientsByNameWithLoadOptionsCommand : BaseCommand
	{
		public override object Execute()
		{
			List<EntityRelations> loadOptions = new List<EntityRelations>();
			loadOptions.Add(EntityRelations.Client_To_ClientContacts);
			loadOptions.Add(EntityRelations.Client_To_Orders);
			loadOptions.Add(EntityRelations.Order_To_OrderDetails);

			List<Client> clients = ServiceFactory.ClientService.GetClientsByName("TestClient", loadOptions);

			string objectDump = string.Empty;
			foreach (Client client in clients)
			{
				objectDump += string.Format("-> ClientId:{0} ClientName:{1} Office:{2}\r\n", client.ClientID, client.ClientName, client.Office);
				foreach (Order order in client.Orders)
				{
					objectDump += string.Format("    OrderId:{0} DateReceived:{1}\r\n", order.OrderID, order.DateReceived);
					foreach (OrderDetail orderDetail in order.OrderDetails)
					{
						objectDump += string.Format("       OrderDetailsID:{0} ProductID:{1}\r\n", orderDetail.OrderDetailsID, orderDetail.ProductID);
					}
				}
			}

			return objectDump;
		}
	}
}
