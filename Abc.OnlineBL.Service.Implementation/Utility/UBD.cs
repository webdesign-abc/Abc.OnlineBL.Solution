using Abc.OnlineBL.DataStore;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Utility.WhereIs;
using System;
using System.Linq;

namespace Abc.OnlineBL.Utility
{
    public class UBD
    {  

        //public string PopulateUBDMapRef(string rsAddress, string rsLocation, string rsState)//(DS.DSRS ds)
        /// <summary>
        /// This action used for retrive the Longitutude and Latitude according to Address, Location, State, Country 
        /// and assign in RusModel UBD Map Ref
        /// </summary>
        /// <param name="objRSModel"></param>
        /// <returns></returns>
        public static void PopulateUBDMapRef(RunsheetDetail objRSDetailModel, ref bool dnsError)//(DS.DSRS ds)
        {
            //RunsheetDetail obj;
            string strMapRef = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(objRSDetailModel.Address))
                {
                    //AbcSettings objAbcSetting = AbcSettings.GetConfig;
                    string country = "Australia";

					objRSDetailModel.UBD_Map_Ref = UBDCache.Current.GetUBDPref(objRSDetailModel.OrderId);

					if (string.IsNullOrEmpty(objRSDetailModel.UBD_Map_Ref))
					{
						WhereisApiClient client = new WhereisApiClient("4213272605751756800", "Password01");
						//WhereisApiClient client = new WhereisApiClient("6165231760435002368", "T_@B)_?_aU5");//old token
						string propertyAddress = string.Format("{0}, {1}, {2}, {3}", objRSDetailModel.Address, objRSDetailModel.Location, objRSDetailModel.State, country);

						Abc.OnlineBL.Utility.WhereIs.GeocodeResponse resu = client.getUnstructuredGeocode(propertyAddress, ref dnsError);

						if (resu != null && resu.results.Count > 0 && resu.results[0] != null)
						{
							string mapRef = GetPageRef(resu.results[0].centrePoint.lat, resu.results[0].centrePoint.lon);
							if (mapRef != null)
							{
								objRSDetailModel.UBD_Map_Ref = mapRef;
							}
						}

						UBDCache.Current.AddUBD(objRSDetailModel.OrderId, objRSDetailModel.UBD_Map_Ref);
					}
						
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Error occured in 'PopulateUBDMapRef' for . RunsheetDetail ID:{0} RunsheetDetail Address:{1}", objRSDetailModel.OrderId, objRSDetailModel.Address);
                Logger.Exception(ex, message);
                //throw;
            }
            //return strMapRef;
        }

        /// <summary>
        /// This action used for find & Create formated objRSDetailModel.UBD_Map_Ref 
        /// According to latitude, longitude from DB [UBD_MAP_REFs]
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static string GetPageRef(double latitude, double longitude)
        {
            string ret = null;
            try
            {               
                using (AbcDataContext ctx = new AbcDataContext())
                {
                    UBD_MAP_REF objMapRef = (from c in ctx.UBD_MAP_REFs
                                        where c.X1 < longitude && c.X2 > longitude && c.Y1 < latitude && c.Y2 > latitude
                                        select c).FirstOrDefault();


                    if (objMapRef != null)
                    {
                        if (!String.IsNullOrEmpty(objMapRef.Directory)) ret += objMapRef.Directory + " ";
                        if (!(objMapRef.MapNum == null)) ret += objMapRef.MapNum.ToString() + " ";
                        if (!String.IsNullOrEmpty(objMapRef.AlphaGrid)) ret += objMapRef.AlphaGrid;
                        if (!(objMapRef.NumGrid == null)) ret += objMapRef.NumGrid.ToString() + " ";
                        ret = ret.Trim();
                    }
                    return ret;
                }
            }
            catch (Exception ex)
            {
                //string options = DataLoadOptionsUtility.GetOptionsInString(loadOptions);
                string message = string.Format("Error occured in 'GetPageRef' for . Latitude :{0} Longitude:{1}", latitude, longitude);
                Logger.Exception(ex, message);
                throw;
            }
            //
        }
    }
}
