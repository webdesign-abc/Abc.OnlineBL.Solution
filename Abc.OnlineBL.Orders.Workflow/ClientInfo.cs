using System;
using System.Text;
using System.Web;

namespace Abc.OnlineBL.Orders.Workflow
{
	[Serializable]
	public class ClientInfo
	{
		public int ClientID;
		// ClientName and Office can come from the ClientsDisplayInfo table,
		// which is a friendly name for templating.
		public string ClientName;
		public string ActualClientName;
		public string Office;
		public string ActualOffice;
		public string Address;
		public string Suburb;
		public string State;
		public string Country;
		public string PostCode;
		public string Phone;
		public string Fax;
		public string Email;
		public string WebSite;
		public string LicenseNo;

		public string ToXML()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<ClientDetails>\r\n");
			sb.AppendFormat("<ClientId>{0}</ClientId>\r\n", ClientID);
			sb.AppendFormat("<ClientName>{0}</ClientName>\r\n", GetEncodedString(ClientName));
			sb.AppendFormat("<ActualClientName>{0}</ActualClientName>\r\n", GetEncodedString(ActualClientName));
			sb.AppendFormat("<ClientOffice>{0}</ClientOffice>\r\n", GetEncodedString(Office));
			sb.AppendFormat("<ActualClientOffice>{0}</ActualClientOffice>\r\n", GetEncodedString(ActualOffice));
			sb.AppendFormat("<ClientAddress>{0}</ClientAddress>\r\n", GetEncodedString(Address));
			sb.AppendFormat("<ClientSuburb>{0}</ClientSuburb>\r\n", GetEncodedString(Suburb));
			sb.AppendFormat("<ClientState>{0}</ClientState>\r\n", GetEncodedString(State));
			sb.AppendFormat("<ClientPostCode>{0}</ClientPostCode>\r\n", GetEncodedString(PostCode));
			sb.AppendFormat("<ClientCountry>{0}</ClientCountry>\r\n", GetEncodedString(Country));

			sb.AppendFormat("<ClientPhone>{0}</ClientPhone>\r\n", GetEncodedString(Phone));
			sb.AppendFormat("<ClientFax>{0}</ClientFax>\r\n", GetEncodedString(Fax));
			sb.AppendFormat("<ClientEmail>{0}</ClientEmail>\r\n", GetEncodedString(Email));
			sb.AppendFormat("<ClientLicNo>{0}</ClientLicNo>\r\n", GetEncodedString(LicenseNo));
			sb.AppendFormat("<ClientWebSite>{0}</ClientWebSite>\r\n", GetEncodedString(WebSite));
			sb.Append("</ClientDetails>\r\n");

			return sb.ToString();
		}

		public string ToHTML()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<table cellpadding='3' cellspacing='0'>");
			sb.AppendFormat("<tr><td>Agent Name:</td><td>{0}</td></tr>", GetEncodedString(ActualClientName));
			sb.AppendFormat("<tr><td>Office:</td><td>{0}</td></tr>", GetEncodedString(ActualOffice));
			sb.AppendFormat("<tr><td>Address:</td><td>{0} {1} {2} {3}</td></tr>",
				GetEncodedString(Address), GetEncodedString(Suburb), GetEncodedString(State), GetEncodedString(PostCode));

			sb.AppendFormat("<tr><td>Phone:</td><td>{0}</td></tr>", GetEncodedString(Phone));
			sb.AppendFormat("<tr><td>Fax:</td><td>{0}</td></tr>", GetEncodedString(Fax));
			sb.AppendFormat("<tr><td>Email:</td><td>{0}</td></tr>", GetEncodedString(Email));
			sb.AppendFormat("<tr><td>License No:</td><td>{0}</td></tr>", GetEncodedString(LicenseNo));
			sb.AppendFormat("<tr><td>WebSite:</td><td>{0}</td></tr>", GetEncodedString(WebSite));
			sb.Append("</table>");

			return sb.ToString();
		}

		public string ToText()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("ClientId               : {0}\r\n", ClientID);
			sb.AppendFormat("Agent Name/Office      : {0}/{1}\r\n", ActualClientName, ActualOffice);
			sb.AppendFormat("Address                : {0} {1} {2} {3}\r\n", GetString(Address), GetString(Suburb), GetString(State), GetString(PostCode));

			sb.AppendFormat("Phone                  : {0}\r\n", GetString(Phone));
			sb.AppendFormat("Fax                    : {0}\r\n", GetString(Fax));
			sb.AppendFormat("Email                  : {0}\r\n", GetString(Email));
			sb.AppendFormat("License No             : {0}\r\n", GetString(LicenseNo));
			sb.AppendFormat("WebSite                : {0}\r\n", GetString(WebSite));

			return sb.ToString();
		}

		private string GetEncodedString(string input)
		{
			return HttpUtility.HtmlEncode(input != null ? input : "");
		}

		private string GetString(string input)
		{
			return input != null ? input : "";
		}
	}
}
