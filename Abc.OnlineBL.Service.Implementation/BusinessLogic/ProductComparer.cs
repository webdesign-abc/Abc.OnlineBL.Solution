using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abc.OnlineBL.Service.Implementation.BusinessLogic
{
    public class ProductComparer : IEqualityComparer<Abc.OnlineBL.Entities.Product>
    {
        public bool Equals(Abc.OnlineBL.Entities.Product a, Abc.OnlineBL.Entities.Product b)
        {
            return a.ProductID == b.ProductID;
        }


        public int GetHashCode(OnlineBL.Entities.Product obj)
        {
            return obj.ProductID;
        }
    }
}