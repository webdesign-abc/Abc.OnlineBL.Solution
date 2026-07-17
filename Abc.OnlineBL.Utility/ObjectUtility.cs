/*
 * Object Serilizer Class
 * */

namespace Abc.OnlineBL.Utility
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.IO;
	using System.Xml.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Utility Methods Placeholders
    /// </summary>
    public class ObjectUtility
    {
        /// <summary>
        /// get a duplicate copy using XML serialzation
        /// </summary>
        /// <param name="obj">The object to duplicate</param>
        /// <returns>a copy of obj</returns>
        public static T Duplicate<T>(T obj)
        {
			string xml = ObjectUtility.Serialize<T>(obj);
			T newCopy = ObjectUtility.Deserialize<T>(xml);
            return newCopy;
        }

        #region Serialize
        /// <summary>
        /// Serializes the specified object. Unicode is used
        /// </summary>
        /// <param name="obj">the object to serilize</param>
        /// <returns>xml string copy of the object</returns>
        public static string Serialize<T>(T obj)
        {
            return Serialize(obj, Encoding.Unicode);
        }

        /// <summary>
        /// Serializes the specified tag to xml string.
        /// </summary>
        /// <param name="tag">The object.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>xml string copy of the object</returns>
        public static string Serialize<T>(T tag, System.Text.Encoding encoding)
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
        public static T Deserialize<T>(string xmlData)
        {
            StringReader sr = new StringReader(xmlData);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T tag = (T)serializer.Deserialize(sr);            
            sr.Close();
            return tag;
        }
        #endregion

		#region BinarySerialize
		/// <summary>
		/// Binary serialize.
		/// </summary>
		/// <typeparam name="T">Generic Type</typeparam>
		/// <param name="obj">The obj.</param>
		/// <returns>byte array of the object</returns>
		public static byte[] BinarySerialize<T>(T obj)
		{
			byte[] data = null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				data = ms.ToArray();
				ms.Close();
			}
			return data;
		}
		#endregion

		#region BinaryDeserialize
		/// <summary>
		/// Binary Deserialize.
		/// </summary>
		/// <typeparam name="T">Generic Type</typeparam>
		/// <param name="data">The data.</param>
		/// <returns>Returns the generic typed object</returns>
		public static T BinaryDeserialize<T>(byte[] data)
		{
			T obj = default(T);
			BinaryFormatter bf = new BinaryFormatter();
			
			using (MemoryStream ms = new MemoryStream(data))
			{
				obj = (T)bf.Deserialize(ms);
				ms.Close();
			}
			return obj;
		}
		#endregion
	}
}
