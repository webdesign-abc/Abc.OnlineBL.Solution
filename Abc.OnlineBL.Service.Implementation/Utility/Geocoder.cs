using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using Abc.OnlineBL.Utility.Configuration;

namespace Abc.OnlineBL.Utility
{
    public class Geocoder
    {

        /// <summary>
        /// Google.com Geocoder
        /// Useful in: USA, Canada, France, Spain, Italy and Germany
        /// </summary>
        /// <remarks>
        /// Url request to
        /// http://maps.google.com/maps/geo?q=1600+Amphitheatre+Parkway,+Mountain+View,+CA&output=xml&key=xxxxxxxxxxxxxxxx
        /// and response in the format:
        /// <![CDATA[
        /// <?xml version="1.0" encoding="UTF-8"?>
        /// <kml xmlns="http://earth.google.com/kml/2.0">
        ///     <Response>
        ///         <name>1 Macquarie Street,Chatswood ,NSW,Australia</name>
        /// 	    <Status>
        ///             <code>200</code>
        ///      		<request>geocode</request>
        /// 	    </Status>
        ///         <Placemark>
        /// 	        <address>1 Macquarie St, Chatswood, NSW 2067, Australia</address>
        /// 	        <AddressDetails Accuracy="8" xmlns="urn:oasis:names:tc:ciq:xsdschema:xAL:2.0">
        ///                 <Country>
        ///                     <CountryNameCode>AU</CountryNameCode>
        ///                     <AdministrativeArea>
        ///                         <AdministrativeAreaName>NSW</AdministrativeAreaName>
        ///                         <Locality>
        ///                             <LocalityName>Chatswood</LocalityName>
        ///                             <Thoroughfare>
        ///                                 <ThoroughfareName>13 Macquarie St</ThoroughfareName>
        ///                             </Thoroughfare>
        ///                             <PostalCode>
        ///                                 <PostalCodeNumber>2067</PostalCodeNumber>
        ///                             </PostalCode>
        ///                         </Locality>
        ///                     </AdministrativeArea>
        ///                 </Country>
        ///             </AddressDetails>
        ///             <Point>
        ///                 <coordinates>151.191666,-33.792181,0</coordinates>
        ///             </Point>
        ///         </Placemark>
        ///     </Response>
        /// </kml>
        /// ]]>
        /// </remarks>
        /// <returns></returns>
        /// address = Address, suburb = Location, state = State, country = Country
        public static Geoloc? LocateGoogle(string address, string suburb, string state, string country, ref bool dnsError)
        {
			  dnsError = false;
			if (!string.IsNullOrEmpty(address))
				address = address.Replace("#", " ");
            string query = string.Format("{0},{1},{2},{3}", address.Trim(), suburb.Trim(), state.Trim(), country.Trim());
            //string url = "http://maps.google.com/maps/geo?q={0}&output=xml&key=" +  AbcDasConfig.GOOGLE_KEY; //ABQIAAAAqk5s3ZgDfx6Fror8PkaM3BS7dMl9tWcZKqHtzShLYcoogiPxgBTt2AL3Uh361Po66T0RC10XiuyUnw";
            Geoloc? gl = null;
            AbcSettings objAbcSetting = AbcSettings.GetConfig;
            if (objAbcSetting.IsKeyExists("GOOGLE_KEY"))
            {
                string url = "http://maps.google.com/maps/geo?q={0}&output=xml&key=" + objAbcSetting["GOOGLE_KEY"];//GOOGLE_KEY; //ABQIAAAAqk5s3ZgDfx6Fror8PkaM3BS7dMl9tWcZKqHtzShLYcoogiPxgBTt2AL3Uh361Po66T0RC10XiuyUnw";

                url = String.Format(url, query);

                XmlNode coords = null;
                XmlNode localityName = null;
                XmlNode administrativeAreaName = null;

                try
                {
                    string xmlString = GetUrl(url);
					if (!string.IsNullOrEmpty(xmlString))
						xmlString = xmlString.Replace("#", " ");
                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(xmlString);
                    XmlNamespaceManager xnm = new XmlNamespaceManager(xd.NameTable);
                    //coords = xd.SelectSingleNode("/").ChildNodes[1].ChildNodes[0].ChildNodes[2].LastChild;
                    coords = xd.GetElementsByTagName("coordinates")[0];
                    localityName = xd.GetElementsByTagName("LocalityName")[0];
                    administrativeAreaName = xd.GetElementsByTagName("AdministrativeAreaName")[0];
                }
                catch (Exception ex)
                {
						 if (ex.Message.StartsWith("The remote name could not be resolved"))
						 {
							 dnsError = true;
						 }
                    //Logger.LogException(ex, query);
                    Logger.Exception(ex, query);
                }

                //Geoloc? gl = null;

                if (coords != null && localityName != null && administrativeAreaName != null)
                {
                    if (string.Compare(suburb, localityName.InnerText, true) == 0 &&
                        string.Compare(state, administrativeAreaName.InnerText, true) == 0)
                    {
                        string[] coordinateArray = coords.InnerText.Split(',');
                        if (coordinateArray.Length >= 2)
                        {
                            gl = new Geoloc(Convert.ToDouble(coordinateArray[1].ToString()), Convert.ToDouble(coordinateArray[0].ToString()));
                        }
                    }

                    if (state == "NSW")
                    {
                        if (string.Compare(suburb, localityName.InnerText, true) == 0 &&
                                    string.Compare("New South Wales", administrativeAreaName.InnerText, true) == 0)
                        {
                            string[] coordinateArray = coords.InnerText.Split(',');
                            if (coordinateArray.Length >= 2)
                            {
                                gl = new Geoloc(Convert.ToDouble(coordinateArray[1].ToString()), Convert.ToDouble(coordinateArray[0].ToString()));
                            }
                        }
                    }
                }
            }// End of IsKeyExist
            return gl;
        }

        /// <summary>
        /// Retrieve a Url via WebClient
        /// </summary>
        /// <param name="url">Url to query (METHOD=GET)</param>
        /// <returns>Result stream (assumed to be Xml)</returns>
        private static string GetUrl(string url)
        {
            string result = string.Empty;
            System.Net.WebClient Client = new WebClient();

            using (Stream strm = Client.OpenRead(url))
            {
                StreamReader sr = new StreamReader(strm);
                result = sr.ReadToEnd();
            }
            return result;
        }
	
    }
}
