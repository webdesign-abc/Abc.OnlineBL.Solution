using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Utility.Security;

namespace Abc.OnlineBL.Utility.Configuration
{
	/// <summary>
	/// Base Config Class for all ABC Profile based Configuration
	/// </summary>
	public class BaseConfig
	{
		private static object syncLock = new object();
		private static AbcSettings config;
		protected static AbcSettings current
		{
			get
			{
				lock (syncLock)
				{
					if (config == null)
						config = AbcSettings.GetConfig;
				}
				return config;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseConfig"/> class.
		/// </summary>
		protected BaseConfig()
		{
		}

		/// <summary>
		/// Gets the name of the active profile.
		/// </summary>
		/// <value>The name of the active profile.</value>
		public static string ActiveProfileName
		{
			get
			{
				return current.ActiveProfile;
			}
		}

		/// <summary>
		/// IF we are in NZ Mode
		/// </summary>
		/// <value>If NZ</value>
		public static bool IS_NZ
		{
			get 
			{
				if (current.IsKeyExists("IS_NZ"))
				{
					return Convert.ToBoolean(current["IS_NZ"]);
				}
				else
				{
					return false;
				}				
			}
		}

		/// <summary>
		/// Gets the VFS_ROOT_URL.
		/// </summary>
		/// <value>The VFS_ROOT_URL.</value>
		public static string VFS_ROOT_URL
		{
			get { return current["VFS_ROOT_URL"]; }
		}

		/// <summary>
		/// Gets the OnlineBL_ROOT_URL.
		/// </summary>
		/// <value>The AI s_ ROO t_ URL.</value>
		public static string OnlineBL_ROOT_URL
		{
			get { return current["OnlineBL_ROOT_URL"]; }
		}

		/// <summary>
		/// Gets a value indicating whether [AI s_ IMPERSONAT e_ ENABLED].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [AI s_ IMPERSONAT e_ ENABLED]; otherwise, <c>false</c>.
		/// </value>
		public static bool OnlineBL_IMPERSONATE_ENABLED
		{
			get 
			{
				if (current.IsKeyExists("OnlineBL_IMPERSONATE_ENABLED"))
				{
					return Convert.ToBoolean(current["OnlineBL_IMPERSONATE_ENABLED"]);
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the AI s_ IMPERSONAT e_ USERNAME.
		/// </summary>
		/// <value>The AI s_ IMPERSONAT e_ USERNAME.</value>
		public static string OnlineBL_IMPERSONATE_USERNAME
		{
			get { return current["OnlineBL_IMPERSONATE_USERNAME"]; }
		}

		/// <summary>
		/// Gets the AI s_ IMPERSONAT e_ DOMAIN.
		/// </summary>
		/// <value>The AI s_ IMPERSONAT e_ DOMAIN.</value>
		public static string OnlineBL_IMPERSONATE_DOMAIN
		{
			get 
			{
				//User Domain and Machine will be equal if the machine is not 
				//part of an AD
				if (Environment.UserDomainName != Environment.MachineName)
				{
                    if (current.IsKeyExists("OnlineBL_IMPERSONATE_DOMAIN"))
                    {                        
                        return current["OnlineBL_IMPERSONATE_DOMAIN"];
                    }
                    else
                    {
						if (!Environment.UserDomainName.Contains("Authority"))
							return Environment.UserDomainName;
						else
							return System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name;
                    }
				}
				else //incase we are not in a domain
				{
					return current["OnlineBL_IMPERSONATE_DOMAIN"];
				}
			}
		}

		/// <summary>
		/// Gets the AI s_ IMPERSONAT e_ PASSWORD.
		/// </summary>
		/// <value>The AI s_ IMPERSONAT e_ PASSWORD.</value>
		public static string OnlineBL_IMPERSONATE_PASSWORD
		{
			get { return current["OnlineBL_IMPERSONATE_PASSWORD"]; }
		}

		/// <summary>
		/// Gets the AI s_ ENDPOIN t_ PREFIX.
		/// </summary>
		public static string OnlineBL_ENDPOINT_PREFIX
		{
			get
			{
				if (current.IsKeyExists("OnlineBL_ENDPOINT_PREFIX"))
				{
					return current["OnlineBL_ENDPOINT_PREFIX"];
				}
				else
				{
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// Gets the key value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static string GetValue(string key)
		{
			return current[key];
		}
	}
}
