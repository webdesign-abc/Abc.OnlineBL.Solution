using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Utility
{
    public struct Geoloc
    {
        public double Lat;
        public double Lon;

        public Geoloc(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public override string ToString()
        {
            return "Latitude: " + Lat.ToString() + " Longitude: " + Lon.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ToQueryString()
        {
            return "+to:" + Lat + "%2B" + Lon;
        }
    }
}
