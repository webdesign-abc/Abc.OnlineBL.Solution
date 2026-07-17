using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using Abc.OnlineBL.Entities.Model;
using System.Data.Linq;
using System.Reflection;

namespace Abc.OnlineBL.DataStore
{
    public partial class AbcDataContext
    {
        [Function(Name = "dbo.AIS_GetAllAvailableOnlineOrderProductIds")]
        [ResultType(typeof(OnlineBL_GetAllAvailableOnlineOrderProductIds_Products))]
        [ResultType(typeof(Abc.OnlineBL.Entities.PackageContentGroup))]
        [ResultType(typeof(OnlineBL_GetAllAvailableOnlineOrderProductIds_PackageProducts))]
        public IMultipleResults OnlineBL_GetAllAvailableOnlineOrderProductIds(int ClientId)
        {
            IExecuteResult result =
                this.ExecuteMethodCall(this,
                      ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                      ClientId);

            return (IMultipleResults)(result.ReturnValue);
        }

        [Function(Name = "dbo.AIS_GetAllAvailableRegularOnlineOrderProductIds")]
        [ResultType(typeof(OnlineBL_GetAllAvailableOnlineOrderProductIds_Products))]
        [ResultType(typeof(Abc.OnlineBL.Entities.PackageContentGroup))]
        [ResultType(typeof(OnlineBL_GetAllAvailableOnlineOrderProductIds_PackageProducts))]
        public IMultipleResults AIS_GetAllAvailableRegularOnlineOrderProductIds(int ClientId)
        {
            IExecuteResult result =
                this.ExecuteMethodCall(this,
                      ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                      ClientId);

            return (IMultipleResults)(result.ReturnValue);
        }
    }
}
