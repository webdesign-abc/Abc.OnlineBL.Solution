using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ServiceModel.Web;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Abc.OnlineBL.Service.Implementation.Utility.BingGeoLocation
{
    public class BingMap
    {
        private Response GetResponse(Uri uri)
        {
            WebClient wc = new WebClient();
            Response ret = null;
            using (var sr = wc.OpenRead(uri))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                ret = ser.ReadObject(sr) as Response;
                sr.Close();
            }

            return ret;
        }

		public GeocodeResponse GeocodeAddress(string query, ref bool dnsError)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException("query");
            }

            GeocodeResponse ret = new GeocodeResponse();
            string key = "ArKOiWMDvP_VT6OQDXwe1rQEim8llEKYKOR_ae9JuDKW2PIvj-l66I1ZMgfCUsmO";
			query = query.Replace("#", " ");
            Uri geocodeRequest = new Uri(string.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", query, key));

			try
			{
				var x = GetResponse(geocodeRequest);
				if (x.StatusCode == 200)
				{
					if (x.ResourceSets.Count() > 0 && x.ResourceSets[0].Resources.Count() > 0)
					{
						Location loc = x.ResourceSets[0].Resources[0] as Location;
						if (loc != null)
						{
							ret.FoundMatch = true;
							ret.Latitude = loc.Point.Coordinates[0].ToString();
							ret.Longitude = loc.Point.Coordinates[1].ToString();
							ret.Address = loc.Address.FormattedAddress;
						}
					}
				}            
			}
			catch (Exception ex)
			{
				dnsError = true;
				Logger.Exception(ex, query);
			}
            

            return ret;
        }
    }

    public class GeocodeResponse
    {
        public bool FoundMatch { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
    }
}
