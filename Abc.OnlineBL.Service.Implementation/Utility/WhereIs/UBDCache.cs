using System;
using System.Collections.Generic;
using System.Text;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.Utility.WhereIs
{
	public class UBDCache
	{
		#region Private Variables
		private static UBDCache singleton;
		private static object syncLock = new object();

		private Dictionary<int, string> uBDDictionary;
		private DateTime lastRefesh;
		#endregion

		#region Constructor
		public UBDCache()
		{
			uBDDictionary = new Dictionary<int, string>();
			lastRefesh = DateTime.Now;
		}
		#endregion

		#region Current
		public static UBDCache Current
		{
			get
			{
				lock (syncLock)
				{
					if (singleton == null)
						singleton = new UBDCache();

					return singleton;
				}
			}
		}
		#endregion

		#region AddUBDPref
		public void AddUBD(int orderID, string uBDRef)
		{
			if (string.IsNullOrEmpty(uBDRef)) return;

			if (!uBDDictionary.ContainsKey(orderID))
				uBDDictionary.Add(orderID, uBDRef);
		}
		#endregion

		#region GetUBDPref
		public string GetUBDPref(int orderID)
		{
			// Keep the data in the cache for 4 hour
			TimeSpan tspan = DateTime.Now - lastRefesh;
			if (tspan.TotalHours > 4)
			{
				uBDDictionary.Clear();
				lastRefesh = DateTime.Now;
				return string.Empty;
			}

			if (uBDDictionary.ContainsKey(orderID))
				return uBDDictionary[orderID];
			else
				return string.Empty;
		}
		#endregion
	}
}
