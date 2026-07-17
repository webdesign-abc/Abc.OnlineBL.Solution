using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Abc.OnlineBL.Entities.Model;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Orders.Workflow.Model;
using Abc.OnlineBL.VirtualFileSystem;

namespace Abc.OnlineBL.Orders.Workflow
{
	#region HtmlFormats
	[Serializable]
	public class HtmlFormats
	{
        public string ForClient = "";
        public string ForClientNewEmail = "";
		public string ForB2B = "";
		public string ForAbc = "";
		public string ForStockboard = "";
		public string ForPhotography = "";
	}
	#endregion

	#region FormatOrder
	[Serializable]
	public class FormatOrder
	{
		public static int NoLocation = 10751;
		public const string LineBreak = "\r\n____________________________________________________________\r\n\r\n";

		private OnlinePropertyOrder propertyOrder;
		private Abc.OnlineBL.Entities.Client client;
		private Abc.OnlineBL.Entities.Property property;

		private ClientInfo clientInfo;
		private int locationId;
		protected int propertyId;

		private int orderId;
		private string fileDir = "";

		private OrderDataExchange orderDataExchange;

		#region Constructor
		public FormatOrder(OrderDataExchange orderDataExchange, int orderIdToUse)
		{
			this.orderDataExchange = orderDataExchange;
			this.propertyId = orderDataExchange.PropertyOrder.PropertyId;
			this.propertyOrder = orderDataExchange.PropertyOrder;
			this.locationId = orderDataExchange.Property.LocationId;
			this.fileDir = orderDataExchange.OrderDir;
			this.client = orderDataExchange.Client;
			this.property = orderDataExchange.Property;

			this.clientInfo = orderDataExchange.ClientInfo;

			this.orderId = orderIdToUse;
		}
		#endregion

		#region GetTextFileContents
		/// <summary>
		/// This method returns the Text body for Creating the Text file which describes the Order.
		/// This file is used for Internal Use.
		/// </summary>
		/// <returns>String Text Body</returns>
		public string GetTextFileContents(OrderDisplayType orderDisplayType)
		{
			StringBuilder sb = new StringBuilder();
			if (orderId > 0)
			{
				sb.Append("JOB ID: " + orderId + "\r\n");
			}
			else
			{
				sb.Append("JOB ID: TO BE ADVISED\r\n");
			}
			if (propertyId > 0)
			{
				sb.Append("Property ID: " + propertyId + "\r\n");
			}

            if (client != null && !string.IsNullOrEmpty(client.ManagerID))
            {
                sb.Append("Manager ID: " + client.ManagerID + "\r\n");
            }
            sb.Append(FormatOrder.LineBreak);

			// Check to See if Location is N/A which 10751. If yes then Show a Warning Message
			if (locationId == FormatOrder.NoLocation)
			{
				sb.Append("+--------------------------------------+\r\n");
				sb.Append("¦ For Office Use Only:                 ¦\r\n");
				sb.Append("¦ Check Location, Currently N/A        ¦\r\n");
				sb.Append("+--------------------------------------+\r\n");
			}

			// Header Section
			sb.Append("ABC Online Order     -     ");
			sb.Append("Date: " + OnlineBLConfig.Now.ToString("dd-MMM-yyyy hh:mm tt") + "\r\n");

			sb.Append(FormatOrder.LineBreak);

			sb.Append("Client Details\r\n");
			sb.Append("--------------\r\n");

			if (clientInfo != null)
			{
				sb.Append(clientInfo.ToText());
			}
			else
			{
				sb.Append("Agent Name/Office      : " + client.ClientName + " / ");
				sb.Append(client.Office + "\r\n");
				sb.Append("Address                : " + (!string.IsNullOrEmpty(client.Address) ? client.Address : "") + " / ");
				sb.Append((!string.IsNullOrEmpty(client.Suburb) ? client.Suburb : "") + "\r\n");
				sb.Append("Phone                  : " + (!string.IsNullOrEmpty(client.Phone) ? client.Phone : "") + "\r\n");
				sb.Append("Fax                    : " + (!string.IsNullOrEmpty(client.Fax) ? client.Fax : "") + "\r\n");
				sb.Append("Email                  : " + (!string.IsNullOrEmpty(client.Email) ? client.Email : ""));
			}

			sb.Append(FormatOrder.LineBreak);

			string orderContent = propertyOrder.GetText(orderDisplayType);
			
			orderContent = orderContent.Replace("[$NZTAGS$]", "");
			
			// Add the Actual Order Contents
			sb.Append(orderContent);

			return sb.ToString();

		}
		#endregion

		#region GetRichTextFileContents
		public string GetRichTextFileContents(OrderDisplayType orderDisplayType)
		{
			string text = GetTextFileContents(orderDisplayType).Replace("\r\n", "\\par\r\n");
			StringBuilder sb = new StringBuilder();
			sb.Append("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang3081");
			sb.Append("{\\fonttbl{\\f0\\fnil\\fprq2\\fcharset0 Free 3 of 9 Extended;}");
			sb.Append("{\\f1\\fmodern\\fprq1\\fcharset0 Lucida Console;}}\r\n");
			sb.Append("{\\*\\generator Msftedit 5.41.15.1507;}\\viewkind4\\uc1\\pard\\f0\\fs96 *").Append(orderId.ToString()).Append("*\\f1\\fs20\\par\\par\r\n");
			sb.Append(text);
			sb.Append("}");
			return sb.ToString();
		}
		#endregion

		#region GetHtmlFileContents
		public HtmlFormats GetHtmlFileContents()
		{
			HtmlFormats htmlContents = new HtmlFormats();
			htmlContents.ForAbc = GetHtmlFileContents4NormalOrder();

			//P3: Uncomment this for Phase 3
			htmlContents.ForB2B = GetHtmlFileContents4B2BOrder();
            htmlContents.ForClient = GetHtmlFileContents4Client();
            htmlContents.ForClientNewEmail = GetXMLContents4Client();
			htmlContents.ForStockboard = GetHtmlFileContents4StockboardOrder();
			htmlContents.ForPhotography = GetHtmlFileContents4PhotographyOrder();

			return htmlContents;
		}

		private string GetHtmlFileContents4Client()
		{
			string html = GetClientHtmContents();
            //html = html + orderDataExchange.PropertyOrder.GetHTMLString();
            html = html + orderDataExchange.PropertyOrder.GetClientHTMLString();

			List<string> ids = new List<string>();
			if (orderDataExchange.OrderId > 0)
				ids.Add(string.Format("Order Id: {0}", orderDataExchange.OrderId));
			if (orderDataExchange.PhotoOrderId > 0)
				ids.Add(string.Format("Photography Order Id: {0}", orderDataExchange.PhotoOrderId));
			if (orderDataExchange.StockId > 0)
				ids.Add(string.Format("Stockboard Order Id: {0}", orderDataExchange.StockId));
			if (orderDataExchange.ListingId > 0)
				ids.Add(string.Format("ListingId Id: {0}", orderDataExchange.ListingId));

			html = FormatHtmlBody(html, ids, true);

			return html;
		}

        private string GetXMLContents4Client()
        {
            StringBuilder sb = new StringBuilder();

            // Header Section
            sb.Append("<EVENT>");
            sb.Append("<AgentName>");
            sb.Append(client.ClientName.Replace("&", " "));
            sb.Append("</AgentName>");
            sb.Append("<AgentOffice>");
            sb.Append(client.Office.Replace("&", " "));
            sb.Append("</AgentOffice>");
            sb.Append("<PAddress>");
            sb.Append(orderDataExchange.PropertyOrder.Property.PropertyAddressWithSuburb.Replace("&", " "));
            sb.Append("</PAddress>");
            sb.Append("<OrderID>");

            List<string> ids = new List<string>();
            if (orderDataExchange.OrderId > 0)
                ids.Add(string.Format("{0}", orderDataExchange.OrderId));
            if (orderDataExchange.PhotoOrderId > 0)
                ids.Add(string.Format("{0}", orderDataExchange.PhotoOrderId));
            if (orderDataExchange.StockId > 0)
                ids.Add(string.Format("{0}", orderDataExchange.StockId));

            string sbOrders = string.Empty;
            foreach (string id in ids)
            {
                sbOrders = sbOrders + id + " - ";
            }

            if (sbOrders.EndsWith(" - "))
                sbOrders = sbOrders.Substring(0, sbOrders.Length - 3);

            sb.Append(sbOrders);
            sb.Append("</OrderID>");
            sb.Append("<DateReceived>");
            sb.Append(OnlineBLConfig.Now.ToString("dd MMMM yyyy") + " at " + OnlineBLConfig.Now.ToString("h:mmtt"));
            
            sb.Append("</DateReceived>");
            sb.Append("<ContactName>");
            sb.Append(orderDataExchange.PropertyOrder.ContactDetailName.Replace("&", " "));
            sb.Append("</ContactName>");
            sb.Append("<ContactNumber>");
            sb.Append(orderDataExchange.PropertyOrder.ContactNumber);
            sb.Append("</ContactNumber>");
            sb.Append("<SendProofTo>");
            sb.Append(orderDataExchange.PropertyOrder.SendProofTo);
            sb.Append("</SendProofTo>");

            sb.Append("<Products>");

            foreach (CartItem anItem in orderDataExchange.PropertyOrder.Cart)
            {

                if (anItem.TypeId == ProductTypes.Packages || anItem.TypeId == ProductTypes.BoardPackages || anItem.TypeId == ProductTypes.OtherPackages)
                {
                    if (!string.IsNullOrEmpty(anItem.ProductName))
                    {
                        sb.Append("- 1x " + anItem.ProductName.Replace("&", "&amp;") + "&lt;br&gt;");
                    }
                }
                else
                {
                    sb.Append("- " + anItem.ItemQty + "x ");

                    if (!string.IsNullOrEmpty(anItem.WebFriendlyName))
                    {
                        sb.Append(anItem.WebFriendlyName.Replace("&", "&amp;") + "&lt;br&gt;");
                    }
                    else if (!string.IsNullOrEmpty(anItem.ProductName))
                    {
                        sb.Append(anItem.ProductName.Replace("&", "&amp;") + "&lt;br&gt;");
                    }

                }
                
            }

            sb.Append("</Products>");

            sb.Append("</EVENT>");

            return sb.ToString();
        }

		/// <summary>
		/// A normal order should not contain Stockboard and Photography
		/// </summary>
		/// <returns></returns>
		private string GetHtmlFileContents4NormalOrder()
		{
			string html = GetClientHtmContents();

			// Create new PropertyOrder object so it doesn't interfere with the existing one.
			Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrderClone = OrderDataExchange.StringToObject(orderDataExchange.StrOrder);
			List<Abc.OnlineBL.Entities.EntityRelations> options = new List<Abc.OnlineBL.Entities.EntityRelations>();
			options.Add(Abc.OnlineBL.Entities.EntityRelations.Property_To_Location);
			propertyOrderClone.Property = OrderProcessor.GetPropertyById(propertyOrderClone.PropertyId, options);

			if (OnlineOrder.IsPhotoExists(propertyOrderClone))
				propertyOrderClone.RemoveItemByProductType((int)ProductTypes.Photography);
			if (OnlineOrder.IsStockBoardExists(propertyOrderClone))
				propertyOrderClone.RemoveItemByProductType((int)ProductTypes.Stockboard);

			html = html + propertyOrderClone.GetHTMLString();

			string sWarning = "";
			// Check to See if Location is N/A which 10751. If yes then Show a Warning Message
			if (locationId == FormatOrder.NoLocation)
			{
				sWarning += "+--------------------------------------+<BR>";
				sWarning += "¦ <FONT COLOR='red'>Please Check Location, Currently N/A</FONT> ¦<BR>";
				sWarning += "+--------------------------------------+<BR>";
			}

			List<string> ids = new List<string>();
			if (orderDataExchange.OrderId > 0)
				ids.Add(string.Format("Order Id: {0}", orderDataExchange.OrderId));

			html = FormatHtmlBody(sWarning + html, ids, false);

			return html;
		}

		private string GetHtmlFileContents4B2BOrder()
		{
			string html = GetClientHtmContents();

			// Create new AnOrder object so it doesn't interfere with the existing one.
			Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder propertyOrderClone = OrderDataExchange.StringToObject(orderDataExchange.StrOrder);

			if (OnlineOrder.IsPhotoExists(propertyOrder))
				propertyOrderClone.RemoveItemByProductType((int)ProductTypes.Photography);
			if (OnlineOrder.IsStockBoardExists(propertyOrder))
				propertyOrderClone.RemoveItemByProductType((int)ProductTypes.Stockboard);

			string sWarning = "";
			// Check to See if Location is N/A which 10751. If yes then Show a Warning Message
			if (locationId == FormatOrder.NoLocation)
			{
				sWarning += "+--------------------------------------+<BR>";
				sWarning += "¦ <FONT COLOR='red'>Please Check Location, Currently N/A</FONT> ¦<BR>";
				sWarning += "+--------------------------------------+<BR>";
			}

			List<string> ids = new List<string>();
			if (orderDataExchange.OrderId > 0)
				ids.Add(string.Format("Order Id: {0}", orderDataExchange.OrderId));

			html = FormatHtmlBody4B2BOrder(sWarning + html, ids, false);

			return html;
		}

		private string GetHtmlFileContents4PhotographyOrder()
		{
			string html = GetClientHtmContents();

			// Create new AnOrder object so it doesn't interfere with the existing one.
			OnlinePropertyOrder propertyOrderClone = OrderDataExchange.StringToObject(orderDataExchange.StrOrder);
			List<Abc.OnlineBL.Entities.EntityRelations> options = new List<Abc.OnlineBL.Entities.EntityRelations>();
			options.Add(Abc.OnlineBL.Entities.EntityRelations.Property_To_Location);
			propertyOrderClone.Property = OrderProcessor.GetPropertyById(propertyOrderClone.PropertyId, options);

			// A Photography order should only contain Photography
			List<CartItem> itemIds = new List<CartItem>();
			foreach (CartItem item in propertyOrderClone.Cart)
			{
				if (item.TypeId != ProductTypes.Photography)
				{
					itemIds.Add(item);
				}
			}

			foreach (CartItem itemId in itemIds)
			{
				propertyOrderClone.Cart.Remove(itemId);
			}

			html = html + propertyOrderClone.GetHTMLString();

			List<string> ids = new List<string>();
			if (orderDataExchange.PhotoOrderId > 0)
				ids.Add(string.Format("Photography Order Id: {0}", orderDataExchange.PhotoOrderId));

			html = FormatHtmlBody(html, ids, false);
			return html;
		}

		private string GetHtmlFileContents4StockboardOrder()
		{
			string html = GetClientHtmContents();

			// Create new AnOrder object so it doesn't interfere with the existing one.
			OnlinePropertyOrder propertyOrderClone = OrderDataExchange.StringToObject(orderDataExchange.StrOrder);
			List<Abc.OnlineBL.Entities.EntityRelations> options = new List<Abc.OnlineBL.Entities.EntityRelations>();
			options.Add(Abc.OnlineBL.Entities.EntityRelations.Property_To_Location);
			propertyOrderClone.Property = OrderProcessor.GetPropertyById(propertyOrderClone.PropertyId, options);

			// A Stockboard order should only contain Stockboard
			List<CartItem> itemIds = new List<CartItem>();
			foreach (CartItem item in propertyOrderClone.Cart)
			{
				if (item.TypeId != ProductTypes.Stockboard)
				{
					itemIds.Add(item);
				}
			}

			foreach (CartItem itemId in itemIds)
			{
				propertyOrderClone.Cart.Remove(itemId);
			}

			html = html + propertyOrderClone.GetHTMLString(); 

			List<string> ids = new List<string>();
			if (orderDataExchange.StockId > 0)
				ids.Add(string.Format("Stockboard Order Id: {0}", orderDataExchange.StockId));

			html = FormatHtmlBody(html, ids, false);
			return html;
		}

		/// <summary>
		/// This method returns the Html body for Creating the Text file which describes the Order.
		/// </summary>
		/// <returns>Two Types of String, One for ABC and another one for Clients in a Class called
		/// HtmlFormats</returns>
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

		#region GetSpotlightFileContents
		/// <summary>
		/// This method will get the Contents for Spotlight Orders for generating a Text File
		/// </summary>
		/// <returns>String Text Body for File</returns>
		public string GetSpotlightFileContents()
		{

			string txtBody = "";
			StringBuilder sb = new StringBuilder();
			sb.Append("Spotlight Order\r\n");
			sb.Append("Date: " + OnlineBLConfig.Now.ToString("dd-MMM-yyyy hh:mm tt") + "\r\n");
			sb.Append("____________________________________________________________\r\n\r\n");
			sb.Append("Client Details\r\n");
			sb.Append("--------------\r\n");
			sb.Append("Agent Name   : " + client.ClientName + "\r\n");
			sb.Append("Office       : " + client.Office + "\r\n");
			sb.Append("Address                : " + (!string.IsNullOrEmpty(client.Address) ? client.Address : "") + " / ");
			sb.Append("Phone                  : " + (!string.IsNullOrEmpty(client.Phone) ? client.Phone : "") + "\r\n");
			sb.Append("Fax                    : " + (!string.IsNullOrEmpty(client.Fax) ? client.Fax : "") + "\r\n");
			sb.Append("Email                  : " + (!string.IsNullOrEmpty(client.Email) ? client.Email : ""));
			sb.Append("\r\n____________________________________________________________\r\n\r\n");
			sb.Append(orderDataExchange.Property.GetText());
			sb.Append("\r\n____________________________________________________________\r\n\r\n");
			sb.Append("Contact Details\r\n");
			sb.Append("---------------\r\n\r\n");
			sb.Append("Contact Name: " + orderDataExchange.PropertyOrder.ContactNumber + "\r\n");
			sb.Append("Contact No  : " + orderDataExchange.PropertyOrder.ContactName + "\r\n");
			sb.Append("\r\n____________________________________________________________\r\n\r\n");
			sb.Append("Product Details\r\n");
			sb.Append("---------------\r\n\r\n");
			txtBody = sb.ToString();
			List<CartItem> spotJobs = orderDataExchange.PropertyOrder.GetSpotlightOrders();

			foreach (CartItem item in spotJobs)
			{
				txtBody += item.GetText();
				txtBody += "\r\n____________________________________________________________\r\n\r\n";
			}

			return txtBody;
		}
		#endregion

		#region GetXmlFileContents
		/// <summary>
		/// This method returns the XML body for Creating the XML file which describes the Order.
		/// This file is used for Internal Use.
		/// </summary>
		/// <returns>String XML Body</returns>
		public string GetXmlFileContents()
		{
			StringBuilder sb = new StringBuilder();

			try
			{
				sb.Append("<OnlineOrder>\r\n");
				// Job Number Section
				sb.Append("<OrderId>" + orderId + "</OrderId>\r\n");

				if (propertyId > 0)
				{
					sb.Append("<PropertyId>" + propertyId + "</PropertyId>");
				}

				// Header Section			
				sb.Append("<DateReceived>" + OnlineBLConfig.Now.ToString("dd-MMM-yyyy hh:mm tt") + "</DateReceived>\r\n");

				if (clientInfo != null)
				{
					sb.Append(clientInfo.ToXML());
				}
				else
				{
					// ActualClientOffice tag is for InDesignHelper to navigate to the office folder.
					// ClientOffice tag was used before but this tag's content might be overwritten by content
					// coming from the ClientsDisplayInfo table for friendly naming purpose.
					sb.Append("<ClientDetails>\r\n");
					sb.Append("<ClientId>" + client.ClientID + "</ClientId>\r\n");
					sb.Append("<ClientName>" + HttpUtility.HtmlEncode(client.ClientName) + "</ClientName>\r\n");
					sb.Append("<ClientOffice>" + HttpUtility.HtmlEncode(client.Office) + "</ClientOffice>\r\n");
					sb.Append("<ActualClientOffice>" + HttpUtility.HtmlEncode(client.Office) + "</ActualClientOffice>\r\n");
					sb.Append("<ClientAddress>" + HttpUtility.HtmlEncode(!string.IsNullOrEmpty(client.Address) ? client.Address : "") + "</ClientAddress>\r\n");
					sb.Append("<ClientSuburb>" + HttpUtility.HtmlEncode(!string.IsNullOrEmpty(client.Suburb) ? client.Suburb : "") + "</ClientSuburb>\r\n");
					sb.Append("<ClientPhone>" + HttpUtility.HtmlEncode(!string.IsNullOrEmpty(client.Phone) ? client.Phone : "") + "</ClientPhone>\r\n");
					sb.Append("<ClientFax>" + HttpUtility.HtmlEncode(!string.IsNullOrEmpty(client.Fax) ? client.Fax : "") + "</ClientFax>\r\n");
					sb.Append("<ClientEmail>" + HttpUtility.HtmlEncode(!string.IsNullOrEmpty(client.Email) ? client.Email : "") + "</ClientEmail>\r\n");
					sb.Append("</ClientDetails>\r\n");
				}


				// Add the Actual Order Contents
				sb.Append(propertyOrder.GetXml());
				sb.Append("</OnlineOrder>\r\n");
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error Occurred in GetXmlFileContents()");
			}

			
			return sb.ToString();
		}
		#endregion

		#region FileName, FileNameNoJobNo, FileNamePhoto, FileNameStock Properties
		public string FileName
		{
			get
			{
				return fileDir + "\\" + orderId + ".rtf";
			}
		}

		public string FileNameXml
		{
			get
			{
				string dir = fileDir + "\\xml\\";
				IFile file = VirtualFileSystemFactory.GetFile();

				if (!file.ExistsDir(dir))
					file.CreateDir(dir);

				return dir + orderId + ".xml";

			}
		}

		public string FileNameNoJobNo
		{
			get
			{
				return fileDir + "\\Order-" + orderDataExchange.Property.PropertyAddressWithSuburb.Replace("/", "_") + "-" + OnlineBLConfig.Now.ToString("dd-MMM-yyyy-hh-mm-tt") + ".rtf";
			}
		}

		public string FileNamePhotoNoJobNo
		{
			get
			{
				return fileDir + "\\Photography-" + orderDataExchange.Property.PropertyAddressWithSuburb.Replace("/", "_") + "-" + OnlineBLConfig.Now.ToString("dd-MMM-yyyy-hh-mm-tt") + ".rtf";
			}
		}

		public string FileNamePhoto
		{
			get
			{
				return Path.Combine(fileDir, string.Format("{0}_Photography.rtf", orderId));
			}
		}

		public string FileNameStockNoJobNo
		{
			get
			{
				return fileDir + "\\StockBoard-" + orderDataExchange.Property.PropertyAddressWithSuburb.Replace("/", "_") + "-" + OnlineBLConfig.Now.ToString("dd-MMM-yyyy-hh-mm-tt") + ".rtf";
			}
		}

		public string FileNameStock
		{
			get
			{
				return Path.Combine(fileDir, string.Format("{0}_StockBoard.rtf", orderId));
			}
		}
		#endregion

		#region Formatting Utility
		private string FormatHtmlBody(string htmlBody, List<string> idsToDisplay, bool includeConditions)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(@"
						<html>
							<head>
								<title>ABC Photosigns - New Order Request</title>
								<style type=""text/css""> .Texts { FONT-WEIGHT: bold; FONT-SIZE: 9pt; BORDER-BOTTOM: #66ff66 1px solid; FONT-FAMILY: Tahoma, Verdana, Arial }
								</style>
							</head>
							<body bgColor=""#EEFFEE"">
								<div align=""center"">
										<table cellSpacing=""0"" cellPadding=""5"" width=""100%"" bgColor=""#ffffff"" border=""0"">
											<tr>
												<td width=""20%"" bgColor=""#ccff66""><font face=""Tahoma, Verdana, Arial"" color=""#ff0000"" size=""2""><strong>
																New Order Request</strong></font></td>
												<td align=""right"" width=""80%"" bgColor=""#ccff66""><strong><font color=""#000000"" size=""5"">ABC
																Photosigns</font></strong></td>
											</tr>
											<tr>
												<td colSpan=""2"">
														<div align=""center""><font face=""Tahoma, Verdana, Arial"" size=""2"">
														Thank you for placing an order. The following order details were received. A proof will be sent out by the next working day, unless the files supplied are finished artwork where proofs will not be supplied.
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

			string endTags = @"</TD></TR></TABLE></BODY></HTML>";

			if (includeConditions)
				return sb.ToString() + htmlBody + conditions + endTags;
			else
				return sb.ToString() + endTags;

		}

		private string FormatHtmlBody4B2BOrder(string htmlBody, List<string> idsToDisplay, bool includeConditions)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(@"
						<html>
							<head>
								<title>ABC Photosigns - New Order Request</title>
                                <style type=""text/css""> .Texts { FONT-WEIGHT: bold; FONT-SIZE: 12pt; background-color:#668d1b; color:#fff; padding:4px; FONT-FAMILY: Tahoma, Verdana, Arial }
                                </style>								
							</head>
							<body bgColor=""#EEFFEE"">
								<div align=""center"">
										<table cellSpacing=""0"" cellPadding=""5"" width=""100%"" bgColor=""#ffffff"" border=""0"">
											<tr>
												<td width=""20%"" bgColor=""#ccff66""><font face=""Tahoma, Verdana, Arial"" color=""#ff0000"" size=""2""><strong>
																New B2B Order Request</strong></font></td>
												<td align=""right"" width=""80%"" bgColor=""#ccff66""><strong><font color=""#000000"" size=""5"">ABC
																Photosigns</font></strong></td>
											</tr>
											<tr>
												<td colSpan=""2"">
														<div align=""center""><font face=""Tahoma, Verdana, Arial"" size=""2"">
														Thank you for placing an order. The following order details were received. A proof will be sent out by the next working day, unless the files supplied are finished artwork where proofs will not be supplied.
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

			string endTags = @"</TD></TR></TABLE></BODY></HTML>";

			if (includeConditions)
				return sb.ToString() + htmlBody + conditions + endTags;
			else
				return sb.ToString() + endTags;

		}
		private string FormatStockboardHtmlBody(string jobNo)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(@"
						<html>
							<head>
								<title>ABC Photosigns - New Order Request</title>
								<style type=""text/css""> .Texts { FONT-WEIGHT: bold; FONT-SIZE: 9pt; BORDER-BOTTOM: #66ff66 1px solid; FONT-FAMILY: Tahoma, Verdana, Arial }
								</style>
							</head>
							<body bgColor=""#EEFFEE"">
								<div align=""center"">
										<table cellSpacing=""0"" cellPadding=""5"" width=""100%"" bgColor=""#ffffff"" border=""0"">
											<tr>
												<td width=""20%"" bgColor=""#ccff66""><font face=""Tahoma, Verdana, Arial"" color=""#ff0000"" size=""2""><strong>
																New Stockboard Order Request</strong></font></td>
												<td align=""right"" width=""80%"" bgColor=""#ccff66""><strong><font color=""#000000"" size=""5"">ABC
																Photosigns</font></strong></td>
											</tr>
											<tr>
												<td colSpan=""2"">
														<div align=""center""><font face=""Tahoma, Verdana, Arial"" size=""2"">
														Thank you for placing an order. The following order details were received. A proof will be sent out by the next working day, unless the files supplied are finished artwork where proofs will not be supplied.
														</font>
														<font face=""Tahoma, Verdana, Arial"" size=""2"" color=""red"">
														<BR><B>Please Add Products and Edit Order By Clicking
														<A href=""http://www.photosigns.com.au/myabc/BStockboard.aspx"">Here</A>
														</B></font>
														</div>
												</td>
											</tr>
											<tr>
												<td width=""100%"" colspan=""2""><font face=""Courier New, Courier, mono"">
						");
			sb.Append("<B>JOB NO:" + jobNo + "</B><HR>");

			if (propertyId > 0)
			{
				sb.Append("<B>Property Id:" + propertyId + "</B><HR>");
			}

			return sb.ToString();
		}

		#endregion
	}
	#endregion
}
