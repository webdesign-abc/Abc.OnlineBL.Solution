using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.Service.Implementation.Utility
{
    // Provides a common interface between SB_Order and Order
    class OrderSBAdapter
    {
        public static bool IsStockBoard(int orderId)
        {
            return (orderId >= 99000000) || (orderId >= 900000 && orderId <= 950000);
        }

        SB_Order sbOrder;
        Order order;

        public OrderSBAdapter(SB_Order sbOrder)
        {
            this.sbOrder = sbOrder;
        }

        public OrderSBAdapter (Order order)
	    {
            this.order = order;
	    }

        public bool IsNull
        {
            get
            {
                return (sbOrder == null && order == null);
            }
        }

        public string PropertyAddress
        {
            get
            {
                return (order == null) ? sbOrder.PropertyAddress : order.PropertyAddress;
            }
        }

        public string Notes
        {
            get
            {
                return (order == null) ? sbOrder.Notes : order.Notes;
            }
            set
            {
                if (order == null)
                    sbOrder.Notes = value;
                else
                    order.Notes = value;
            }
        }
        

        public string Location1
        {
            get
            {
                return (order == null) ? sbOrder.Location : order.Location.Location1;
            }
        }

        public Client Client
        {
            get
            {
                return (order == null) ? sbOrder.Client : order.Client;
            }
        }

        public int ClientID
        {
            get
            {
                return (order == null) ? sbOrder.ClientID : order.ClientID;
            }
        }

        public string ManagerID
        {
            get
            {
                return (order == null) ? sbOrder.ManagerID : order.ManagerID;
            }
        }
        
    } 
}
