using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Entities.Model
{
    public class PickingSlipOrderInfo
    {
        public String Property { get; set; }                
       public List<ProductInfo> Products { get; set; }

    }

    public class ProductInfo {
       
        public int ProductID { get; set; }
        public String ProductName { get; set; }
        public String ProductTypes { get; set; }
        public int OrderID { get; set; }
    }

    public class NewPickSlipInfo
    {
        public int? ClientID { get; set; }
        public int? OrderId { get; set; }
        public String PropertyAddress { get; set; }
        public String ProductXml { get; set; }
    }
}
