using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Xml;
using Abc.OnlineBL.Utility.Security;
using System.Collections.Generic;

namespace Abc.OnlineBL.Utility.Configuration
{
	/// <summary>
	/// Reprsents a Abc Config Section
	/// </summary>
	public class AbcSettings
	{
		/// <summary>
		/// Settings
		/// </summary>
		protected Dictionary<string,string> settings;
		/// <summary>
		/// Collection
		/// </summary>
		protected StringCollection col;
		/// <summary>
		/// ActiveProfile
		/// </summary>
		protected string activeProfile;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbcSettings"/> class.
		/// </summary>
		public AbcSettings()
		{
			settings = new Dictionary<string, string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbcSettings"/> class.
		/// </summary>
		/// <param name="node">The node.</param>
		public AbcSettings(XmlNode node)
		{
			settings = new Dictionary<string, string>();
			
			activeProfile = node.Attributes["activeProfile"].Value;
			foreach(XmlNode childNode in node.ChildNodes)
			{
				if (childNode.Name.ToLower()=="profile")
				{
					ParseProfile(childNode);				
				}
			}
		}

		private void ParseProfile(XmlNode node)
		{			
			string name = node.Attributes["name"].Value;
			
			if (name.ToLower()==activeProfile.ToLower() || name.ToLower()=="common")
			{
				foreach(XmlNode childNode in node.ChildNodes)
				{
					switch (childNode.Name.ToLower())
					{
						case "add":
							if (settings.ContainsKey(childNode.Attributes["key"].Value))
								settings.Remove(childNode.Attributes["key"].Value);

							if (childNode.Attributes["encrypted"] != null)
							{
								if (childNode.Attributes["encrypted"].Value.ToLower() == "true")
								{									
									settings.Add(childNode.Attributes["key"].Value, SimpleEncrypt.Decrypt(childNode.Attributes["value"].Value, "abcHaLe"));
								}
								else
								{
									settings.Add(childNode.Attributes["key"].Value, childNode.Attributes["value"].Value);
								}
							}
							else
							{
								settings.Add(childNode.Attributes["key"].Value, childNode.Attributes["value"].Value);
							}
							break;
						case "remove":
							settings.Remove(childNode.Attributes["key"].Value);						
							break;
						case "clear":
							settings.Clear();
							break;
					}
				}
			}
		}

		/// <summary>
		/// Gets the active profile.
		/// </summary>
		/// <value>The active profile.</value>
		public string ActiveProfile
		{
			get { return activeProfile; }			
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> with the specified key.
		/// </summary>
		/// <value></value>
		public string this[string key]
		{
			get
			{
				if (settings.ContainsKey(key))
					return settings[key];
				else
					return string.Empty;
			}
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		public ICollection Keys
		{
			get
			{
				return settings.Keys;
			}
		}

		/// <summary>
		/// Determines whether [is key exists] [the specified key name].
		/// </summary>
		/// <param name="keyName">Name of the key.</param>
		/// <returns>
		/// 	<c>true</c> if [is key exists] [the specified key name]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsKeyExists(string keyName)
		{
			return settings.ContainsKey(keyName);
		}


		/// <summary>
		/// Gets the config.
		/// </summary>
		/// <value>The get config.</value>
		public static AbcSettings GetConfig
		{
			get { return ConfigurationManager.GetSection("abcSettings") as AbcSettings; }
		}

		/// <summary>
		/// Loads the config into object.
		/// </summary>
		/// <param name="config">The config.</param>
		public static void LoadConfigIntoObject(object config)
		{
			AbcSettings sett = AbcSettings.GetConfig;			
			foreach(string key in sett.settings.Keys)
			{
				string val = sett[key];
				SetPropertyValue(config, key, val);
			}
		}

		private static void SetPropertyValue(object obj, string propertyName, string propertyValue)
		{
			PropertyInfo property = obj.GetType().GetProperty(propertyName);
			
			if(property!=null)
			{
				if(property.PropertyType.IsEnum)
				{
					// If the property is an enum, parse the string into a valid enum value:
					if(Enum.IsDefined(property.PropertyType, propertyValue))
					{
						property.SetValue(obj, Enum.Parse(property.PropertyType, propertyValue, true), null);
					}
				}				
				else
				{
					// Set the property, changing the type to the expected type:
					property.SetValue(obj, Convert.ChangeType(propertyValue, property.PropertyType), null);
				}
			}			
		}
	}
}
