using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Web;

namespace Abc.OnlineBL.Utility.WhereIs
{
    public class WhereisApiClient
    {
        private string TOKEN;
        private string PASSWORD;

        /**
         * Create an instance of the Whereis API Client 
         * @param TOKEN a valid Whereis API token
         * @param PASSWORD a valid Whereis API password
         */
        public WhereisApiClient(string TOKEN, string PASSWORD) {
            this.TOKEN = TOKEN;
            this.PASSWORD = PASSWORD;
        }

		public GeocodeResponse getUnstructuredGeocode(string query, ref bool dnsError)
        {
            string result = string.Empty;
			GeocodeResponse ret = new GeocodeResponse();

            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new ArgumentNullException("Property Address is Empty");
                }

                string endPoint = "http://api.ems.sensis.com.au/v2/service/geocode/unstructured";
                var populatedEndPoint = "{\"query\": \"" + query.Trim().Replace("\t", " ") + "\", \"pagination\": {\"size\": 1, \"start\": 0}}";
                byte[] bytes = Encoding.UTF8.GetBytes(populatedEndPoint);

                HttpWebRequest request = CreateWebRequest(endPoint, bytes.Length);

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }
                    else
                    {
						using (Stream sr = response.GetResponseStream())
						{
							DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GeocodeResponse));
							ret = ser.ReadObject(sr) as GeocodeResponse;
						}
                    }
                }  
            }
            catch (Exception ex)
            {
                throw ex;
            }

			return ret;
        }

        private HttpWebRequest CreateWebRequest( string endPoint, Int32 contentLength )  
        {  
            var request = (HttpWebRequest)WebRequest.Create( endPoint );  
  
            request.Method = "POST";  
            request.ContentLength = contentLength;
            request.ContentType = "application/json";
			request.Headers.Add("X-Auth-Token", TOKEN);
			request.Headers.Add("X-Auth-Password", PASSWORD);
  
            return request;  
        }  
    }
}
