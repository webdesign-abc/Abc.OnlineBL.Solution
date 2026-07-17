using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Orders.Workflow.Model;

namespace Abc.OnlineBL.Service.Implementation.Utility.OnlineOder
{
	#region HtmlFormats
	[Serializable]
	public class HtmlFormats
	{
		public string ForClient = "";
		public string ForAbc = "";
	}
	#endregion

	#region FormatOrder
	[Serializable]
	public class FormatOrder
	{
		private Abc.OnlineBL.Entities.Client client;
		private Abc.OnlineBL.Orders.Workflow.ClientInfo clientInfo;
		protected int propertyId;
		private int orderId;
		private Abc.OnlineBL.Orders.Workflow.OrderDataExchange orderDataExchange;

		#region Constructor
		public FormatOrder(Abc.OnlineBL.Orders.Workflow.OrderDataExchange orderDataExchange, int orderIdToUse)
		{
			this.orderDataExchange = orderDataExchange;
			this.propertyId = orderDataExchange.PropertyOrder.PropertyId;
			this.client = orderDataExchange.Client;
			this.clientInfo = orderDataExchange.ClientInfo;
			this.orderId = orderIdToUse;
		}
		#endregion

		#region GetHtmlFileContents
		public HtmlFormats GetHtmlFileContents()
		{
			HtmlFormats htmlContents = new HtmlFormats();
			htmlContents.ForAbc = GetHtmlFileContents4NormalOrder();
			htmlContents.ForClient = GetHtmlFileContents4Client();

			return htmlContents;
		}

		private string GetHtmlFileContents4Client()
		{
			string html = GetClientHtmContents();
			html = html + orderDataExchange.PropertyOrder.GetModifyDIYOrderHTMLString();

			List<string> ids = new List<string>();
			if (orderDataExchange.OrderId > 0)
				ids.Add(string.Format("Order Id: {0}", orderDataExchange.OrderId));

			html = FormatHtmlBody(html, ids, true);

			return html;
		}

		private string GetHtmlFileContents4NormalOrder()
		{
			string html = GetClientHtmContents();

			html = html + orderDataExchange.PropertyOrder.GetModifyDIYOrderHTMLString();

			List<string> ids = new List<string>();
			if (orderDataExchange.OrderId > 0)
				ids.Add(string.Format("Order Id: {0}", orderDataExchange.OrderId));

			html = FormatHtmlBody(html, ids, false);

			return html;
		}

		private string GetClientHtmContents()
		{
			StringBuilder sb = new StringBuilder();

			// Header Section
			sb.Append("ABC Online Order<BR>");
			sb.Append("Date: " + OnlineBLConfig.Now.ToString("dd-MMM-yyyy hh:mm tt") + "<BR>");
			sb.Append("<HR><BR>");
			sb.Append("Client Details<BR>");
			sb.Append("--------------<BR>");

			if (clientInfo != null)
			{
				sb.Append(clientInfo.ToHTML());
			}
			else
			{
				sb.Append("Agent Name   : " + client.ClientName + "<BR>");
				sb.Append("Office       : " + client.Office + "<BR>");
				sb.Append("Address                : " + (!string.IsNullOrEmpty(client.Address) ? client.Address : "") + " / ");
				sb.Append("Phone                  : " + (!string.IsNullOrEmpty(client.Phone) ? client.Phone : "") + "\r\n");
				sb.Append("Fax                    : " + (!string.IsNullOrEmpty(client.Fax) ? client.Fax : "") + "\r\n");
				sb.Append("Email                  : " + (!string.IsNullOrEmpty(client.Email) ? client.Email : ""));
			}

			sb.Append("<HR><BR>");

			return sb.ToString();
		}
		#endregion

		#region Formatting Utility
		private string FormatHtmlBody(string htmlBody, List<string> idsToDisplay, bool includeConditions)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(@"
						<html>
							<head>
								<title>ABC Photosigns - Modify Order Request</title>
								<style type=""text/css""> .Texts { FONT-WEIGHT: bold; FONT-SIZE: 9pt; BORDER-BOTTOM: #66ff66 1px solid; FONT-FAMILY: Tahoma, Verdana, Arial }
								</style>
							</head>
							<body bgColor=""#EEFFEE"">
								<div align=""center"">
										<table cellSpacing=""0"" cellPadding=""5"" width=""100%"" bgColor=""#ffffff"" border=""0"">
											<tr>
												<td width=""20%"" bgColor=""#ccff66""><font face=""Tahoma, Verdana, Arial"" color=""#ff0000"" size=""2""><strong>
																Modify Order Request</strong></font></td>
												<td align=""right"" width=""80%"" bgColor=""#ccff66""><strong><font color=""#000000"" size=""5"">ABC
																Photosigns</font></strong></td>
											</tr>
											<tr>
												<td colSpan=""2"">
														<div align=""center""><font face=""Tahoma, Verdana, Arial"" size=""2"">
														Thank you for modifying DIY order. A proof will be sent out by the next working day, unless the files supplied are finished artwork where proofs will not be supplied.
														</font></div>
												</td>
											</tr>
											<tr>
												<td width=""100%"" colspan=""2""><font face=""Courier New, Courier, mono"">
						");

			foreach (string id in idsToDisplay)
			{
				sb.AppendFormat("<b>{0}</b><br />", id);
			}
			sb.Append("<hr />");

			if (propertyId > 0)
			{
				sb.Append("<B>Property Id:" + propertyId + "</B><HR>");
			}

			string conditions = @"</FONT></TD></TR><TR><TD width=""100% colspan=""2"">";

			string includeFile = OnlineBLConfig.ONLINE_ORDER_EMAIL_INCLUDE;

			if (!string.IsNullOrEmpty(includeFile) && File.Exists(includeFile))
			{
				StreamReader sr = new StreamReader(includeFile);
				conditions += sr.ReadToEnd();
			}
			else
			{
				if (!OnlineBLConfig.IS_NZ)
				{
					conditions += @"<P><B>CONDITIONS</B></P>

										1. A fee of $45.00 will be incurred for the cancellation of an order prior
										to printing.<BR>
										2. Prices stated allow for ground floor installation only. Installation
										and dismantling of boards no more than 900mm above ground level.<BR>
										3. Please check proofs carefully as we do not accept any responsibility
										for any undetected errors after we receive your approval.<BR>
										4. Delivery: 3-5 days after approval by you.<BR>
										5. Payment must accompany order.<BR>
										6. This board remains the property of ABC Photosigns.<BR>
										7. Hire is for a six month maximum period and thereafter negotiable.<BR>
										</P>";
				}
				else
				{
					conditions += @"<P><B>***</B></P></P>";
				}
			}

			string endTags = @"</TD></TR></TABLE></BODY></HTML>";

			if (includeConditions)
				return sb.ToString() + htmlBody + conditions + endTags;
			else
				return sb.ToString() + htmlBody + endTags;

		}

		#endregion
	}
	#endregion
}
