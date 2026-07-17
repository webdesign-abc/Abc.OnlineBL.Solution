using System;
using System.Configuration;
using System.Xml;

namespace Abc.OnlineBL.Utility.Configuration
{
	/// <summary>
	/// Loads and Parses a Abc Config Section
	/// </summary>
	public class AbcConfigSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbcConfigSectionHandler"/> class.
		/// </summary>
		public AbcConfigSectionHandler()
		{
		}

		#region IConfigurationSectionHandler Members

		/// <summary>
		/// Creates a configuration section handler.
		/// </summary>
		/// <param name="parent">Parent object.</param>
		/// <param name="configContext">Configuration context object.</param>
		/// <param name="section">Section XML node.</param>
		/// <returns>The created section handler object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			try
			{
				AbcSettings settings = new AbcSettings(section);
				return settings;				
			}
			catch (Exception ex)
			{
				throw new AbcConfigParsingException(ex.Message, ex);
			}
		}

		#endregion
	}
}
