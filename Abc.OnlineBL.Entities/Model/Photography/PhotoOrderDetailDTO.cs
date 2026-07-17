using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.Entities.Model.Photography
{
    [DataContract]
    [Serializable]
    public class PhotoOrderDetailDTO
    {
        public PhotoOrderDetailDTO(OrderDetail orderDetail)
        {
            Id = orderDetail.OrderDetailsID;
            OrderId = orderDetail.OrderID;
            Quantity = orderDetail.Qty;
            ProductId = orderDetail.Product.ProductID;
            ProductTypeId = orderDetail.Product.TypeID;
            ProductName = orderDetail.Product.Name;
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public int ProductTypeId { get; set; }

        [DataMember]
        public string ProductName { get; set; }
    }
}
