using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Utility
{
    public static class Formater
    {
        public static string CustomDate(string dateString){
            string result = "";
            if (!string.IsNullOrEmpty(dateString)) { 
                DateTime covertedTime;
                if (DateTime.TryParse(dateString, out covertedTime)) {
                    result = covertedTime.ToString("dd-MMM-yy hh:mm tt");                
                }            
            }
            return result;
        }
    }
}
