using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]	
	[DataContract]
	public class ProductConfig
	{
		private Fields fields;

		public ProductConfig()
		{
			this.fields = new Fields();
		}

		[XmlElement("fields")]
		[DataMember]
		public Fields Fields
		{
			get { return fields; }
			set { fields = value; }
		}

		public static ProductConfig GetFromString(string xmlConfig)
		{
			return Deserialize<ProductConfig>(xmlConfig);
		}

        public static string GetStringFromObject(ProductConfig pc)
        {
            return Serialize<ProductConfig>(pc);
        }

		#region Serialize
		/// <summary>
		/// Serializes the specified object. Unicode is used
		/// </summary>
		/// <param name="obj">the object to serilize</param>
		/// <returns>xml string copy of the object</returns>
		private static string Serialize<T>(T obj)
		{
			return Serialize(obj, Encoding.Unicode);
		}

		/// <summary>
		/// Serializes the specified tag to xml string.
		/// </summary>
		/// <param name="tag">The object.</param>
		/// <param name="encoding">The encoding.</param>
		/// <returns>xml string copy of the object</returns>
		private static string Serialize<T>(T tag, System.Text.Encoding encoding)
		{
			StringBuilder xml = new StringBuilder();

			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms, encoding);

				XmlSerializer serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(sw, tag);
				StreamReader sr = new StreamReader(ms, encoding);

				sw.Flush();
				ms.Position = 0;

				xml.Append(sr.ReadToEnd());

				sw.Close();
				sr.Close();
				ms.Close();
			}

			return xml.ToString();
		}
		#endregion

		#region Deserialize
		/// <summary>
		/// Deserializes the specified XML data to Generic Type.
		/// </summary>
		/// <param name="xmlData">The XML data.</param>
		/// <returns>The generic object</returns>
		private static T Deserialize<T>(string xmlData)
		{
			StringReader sr = new StringReader(xmlData);
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			T tag = (T)serializer.Deserialize(sr);
			sr.Close();
			return tag;
		}
		#endregion
	}
}
