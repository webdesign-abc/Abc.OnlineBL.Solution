using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Abc.OnlineBL.Entities
{
	public partial class Property
	{
		public string PropertyAddress
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(this.UnitNo))
					sb.Append(this.UnitNo).Append("/");
				if (!string.IsNullOrEmpty(this.StreetNo))
					sb.Append(this.StreetNo).Append(" ");
				if (!string.IsNullOrEmpty(this.StreetName))
					sb.Append(this.StreetName).Append(" ");

				if (this.Location != null && !string.IsNullOrEmpty(this.Location.Location1))
					sb.Append(this.Location.Location1).Append(" ");

				if (this.Location != null && !string.IsNullOrEmpty(this.Location.State))
					sb.Append(this.Location.State);

				return sb.ToString().Trim().Replace("  ", " ");
			}
		}

		public string PropertyAddressWithoutLocation
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(this.UnitNo))
					sb.Append(this.UnitNo).Append("/");
				if (!string.IsNullOrEmpty(this.StreetNo))
					sb.Append(this.StreetNo).Append(" ");
				if (!string.IsNullOrEmpty(this.StreetName))
					sb.Append(this.StreetName);

				return sb.ToString().Trim().Replace("  ", " ");
			}
		}

		public string PropertyAddressWithSuburb
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(this.UnitNo))
					sb.Append(this.UnitNo).Append("/");
				if (!string.IsNullOrEmpty(this.StreetNo))
					sb.Append(this.StreetNo).Append(" ");
				if (!string.IsNullOrEmpty(this.StreetName))
					sb.Append(this.StreetName).Append(", ");

				if (this.Location != null && !string.IsNullOrEmpty(this.Location.Location1))
					sb.Append(this.Location.Location1);

				return sb.ToString().Trim().Replace("  ", " ");
			}
		}

		public string GetHTMLString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<P><B>Property Details</B></P>");
			sb.Append("Address      : " + PropertyAddressWithoutLocation + "<BR>");
			sb.Append("Suburb       : " + this.Location.Location1 + "<BR>");
			sb.Append("State        : " + this.Location.State + "<BR>");
			sb.Append("Post Code    : " + this.Location.PostCode + "<BR>");
			sb.Append("Display Address : true<BR>");
			sb.Append("Property Type: " + this.PropertyType + "<BR>");

			return sb.ToString();
		}

		public string GetXml()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<PropertyDetails>\r\n");
			sb.AppendFormat("<UnitNo>{0}</UnitNo>\r\n", HttpUtility.HtmlEncode(UnitNo));
			sb.AppendFormat("<StreetNo>{0}</StreetNo>\r\n", HttpUtility.HtmlEncode(StreetNo));
			sb.AppendFormat("<StreetName>{0}</StreetName>\r\n", HttpUtility.HtmlEncode(StreetName));
			sb.AppendFormat("<Suburb>{0}</Suburb>\r\n", HttpUtility.HtmlEncode(Location.Location1));
			sb.AppendFormat("<State>{0}</State>\r\n", HttpUtility.HtmlEncode(Location.State));
			sb.AppendFormat("<PostCode>{0}</PostCode>\r\n", HttpUtility.HtmlEncode(Location.PostCode));
			sb.Append("<PropertyType>" + HttpUtility.HtmlEncode(PropertyType) + "</PropertyType>\r\n");
			sb.Append("<PropertyTypeId>" + PropertyTypeId + "</PropertyTypeId>\r\n");
			sb.Append("<ShowAddress>true</ShowAddress>\r\n");

			sb.Append("</PropertyDetails>\r\n");
			return sb.ToString();
		}

		public string GetText()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Property Details\r\n");
			sb.Append("----------------\r\n");
			sb.Append("Address: " + PropertyAddressWithoutLocation + " / ");
			sb.Append(this.Location.Location1 + " / ");
			sb.Append(this.Location.State + " / ");
			sb.Append(this.Location.PostCode + "\r\n");
			sb.Append("Property Type: " + this.PropertyType + "\r\n");
			return sb.ToString();
		}
	}
}
