using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Service.Implementation;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Abc.OnlineBL.Web.API.Controllers
{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        [Route("ProcessOrder")]
        [HttpPost]
        public OnlineOrderResponse ProcessOrder(HttpRequestMessage req)
        {
            try
            {
                Logger.Warn(req.Content.ReadAsStringAsync().Result);

                string jsonContent = req.Content.ReadAsStringAsync().Result;
                Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrder = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder();
                propertyOrder = JsonConvert.DeserializeObject<Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder>(jsonContent);

                Abc.OnlineBL.Entities.Model.OnlineOrder.OnlineOrderResponse onlineOrderResponse = new OnlineOrderResponse();

                OrderService orderService = new OrderService();
                onlineOrderResponse = orderService.ProcessOrder(propertyOrder);

                //Logger.Warn("after ProcessOrder");

                return onlineOrderResponse;

            }
            catch (Exception e)
            {
                Logger.Exception(e, "Error while creating processing order");
                return new OnlineOrderResponse { OrderHasError = true };
            }
        }
    }
}
