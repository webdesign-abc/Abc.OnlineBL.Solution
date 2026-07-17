using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using BusinessUtils = Abc.Business.Entities.Utils;
using Abc.Business.Entities;
using System.Collections.Generic;
using Abc.Business.Entities.Enums;
using System.IO;
using Abc.AIS.Service;
using Abc.AIS.Orders.Workflow.Model;
using Abc.AIS.Entities;
using Dom = Abc.OnlinePublication.Common.DOM;
using System.Xml.Linq;

namespace Abc.AIS.Orders.Workflow
{
	public sealed partial class ProcessOrderWF : SequentialWorkflowActivity
	{
		OrderData orderData;
		private string fileName = "", xmlFileName = "", stockFileName = "", photoFileName = "";

		public ProcessOrderWF()
		{
			InitializeComponent();
		}

		private void InitialzeOrder_ExecuteCode(object sender, EventArgs e)
		{
		}

		private void GenerateOrderId_ExecuteCode(object sender, EventArgs e)
		{
			List<ProductTypes> productTypesToInclude = new List<ProductTypes>();
			productTypesToInclude.Add(ProductTypes.Board);
			productTypesToInclude.Add(ProductTypes.Brochure);
			productTypesToInclude.Add(ProductTypes.WindowCard);
			productTypesToInclude.Add(ProductTypes.Spotlight);
			productTypesToInclude.Add(ProductTypes.ModularPackage);
			productTypesToInclude.Add(ProductTypes.SmsService);

			if (BusinessUtils.Utility.HasPhotosignBoard(this.orderData.Property))
				productTypesToInclude.Add(ProductTypes.ErectionFee);

			orderData.OrderId = GenerateOrder(productTypesToInclude);
		}

		private void CreateOrderFile_ExecuteCode(object sender, EventArgs e)
		{
			FormatOrder formatOrder = new FormatOrder(orderData, orderData.OrderId);

			if (orderData.OrderId > 0)
			{
				fileName = formatOrder.FileName;
				try
				{
					xmlFileName = formatOrder.FileNameXml;
					WriteOrderFile(xmlFileName, formatOrder.GetXmlFileContents());
				}
				catch (Exception ex)
				{
					Logger.Exception(ex, string.Format("{0} {1}"), new object[] { xmlFileName, formatOrder.GetXmlFileContents() });
				}
			}
			else
			{
				fileName = formatOrder.FileNameNoJobNo;
			}

			WriteOrderFile(fileName, formatOrder.GetRichTextFileContents(OrderDisplayType.NormalOrderOnly), true);
		}

		private void GenerateB2BOrderEvent_ExecuteCode(object sender, EventArgs e)
		{
			GenerateOrderEventWrapper(false, true, true);
		}

		private void GenerateOrderEvent_ExecuteCode(object sender, EventArgs e)
		{
			GenerateOrderEventWrapper(false, true, false);
		}

		private void ProcessAOPOrder_ExecuteCode(object sender, EventArgs e)
		{
			if (orderData.OrderId == 0 || !orderData.Property.OrderContainsAOPJobs())
				return;

			if (orderData.Property.OnlineDesignDocuments == null ||
				orderData.Property.OnlineDesignDocuments.DesignOptions == null)
				return;

			// The number of pages corresponding to content type for brochures.
			Dictionary<string, int> precedenceMapping = new Dictionary<string, int>();
			precedenceMapping.Add("Colour Front/Back", 2);
			precedenceMapping.Add("Colour Front/Mono Back", 2);
			precedenceMapping.Add("Colour Front", 1);

			List<OnlineDocument> onlineDocuments = new List<OnlineDocument>();
			foreach (DesignOption option in orderData.Property.OnlineDesignDocuments.DesignOptions)
			{
				OnlineDocument od = GetPrecedentedDocument(option.Documents, precedenceMapping);
				onlineDocuments.Add(od);
			}

			Dictionary<int, int> templateProductIdToTemplateIdMap = new Dictionary<int, int>();
			List<AOP_JobDocument> jobDocuments = new List<AOP_JobDocument>();
			foreach (OnlineDocument item in onlineDocuments)
			{
				int templateProductId = item.DocumentId;

				List<EntityRelations> options = new List<EntityRelations>();
				options.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
				options.Add(EntityRelations.AOP_TemplateProduct_To_AOP_DocumentPolicy);

				// If service can't return templateProduct exception will be thrown from the service.
				AOP_TemplateProduct templateProduct = WorkflowProcessor.GetTemplateProductById(templateProductId, options);

				templateProductIdToTemplateIdMap.Add(templateProduct.TemplateProductId, templateProduct.TemplateId);

				string documentModel = templateProduct.AOP_Template.TemplateModel.ToString();
				string policyModel = templateProduct.AOP_DocumentPolicy.PolicyModel.ToString();
				Dom.Document workingDoc = FeedContents(item, policyModel, documentModel);
				workingDoc.ImageRequirement = new Dom.ImageQuality()
				{
					MinimumMegaPixels = Convert.ToDecimal(templateProduct.MinimumMegaPixels),
					RecommendedMegaPixels = Convert.ToDecimal(templateProduct.RecommendedMegaPixels)
				};
				string workingDocumentModel = Dom.Document.Serialize(workingDoc);

				AOP_JobDocument jobDocument = new AOP_JobDocument();
				jobDocument.SetAsChangeTrackingRoot(EntityState.New);
				jobDocument.JobId = orderData.OrderId;
				jobDocument.TemplateProductId = templateProductId;
				jobDocument.StatusId = 0;
				jobDocument.WorkingDocumentModel = XElement.Parse(workingDocumentModel);
				jobDocument.TemplateName = templateProduct.Name;
				if (!string.IsNullOrEmpty(templateProduct.Description))
				{
					jobDocument.TemplateName += " " + templateProduct.Description;
				}

				// Here we include the document suffix like _B/_BR/_O for the AJPS to eventually pick the files for
				// automatic printing
				string originalFileNameWithoutExt = Path.GetFileNameWithoutExtension(templateProduct.AOP_Template.TemplatePath);
				string ext = Path.GetExtension(templateProduct.AOP_Template.TemplatePath);
				jobDocument.TemplateOriginalFileName = string.Format("{0}_{1}{2}", originalFileNameWithoutExt, item.DocTypeSuffix, ext);

				jobDocument.TemplateDateModifiedStamp = templateProduct.AOP_Template.DateModified.HasValue ? templateProduct.AOP_Template.DateModified.Value : templateProduct.AOP_Template.DateCreated;
				jobDocument.SetAsInsertOnSubmit();

				// Add to insert later as a batch. Because we don't handle transaction here.
				jobDocuments.Add(jobDocument);
			}

			jobDocuments = WorkflowProcessor.UpdateJobDocument(jobDocuments);
		}

		private void CreateSpotlightFile_ExecuteCode(object sender, EventArgs e)
		{
			int id = orderData.OrderId;

			System.IO.StreamWriter sw = null;
			string property = orderData.Property.GetPropertyAddress();
			FormatOrder formatOrder = new FormatOrder(orderData, id);
			string txt = formatOrder.GetSpotlightFileContents();

			string fileName = "";
			fileName = property.Replace('\\', '-');
			fileName = fileName.Replace('/', '-');
			fileName = fileName.Replace(':', ' ');
			fileName = fileName.Replace('*', ' ');
			fileName = fileName.Replace('?', ' ');
			fileName = fileName.Replace('"', ' ');
			fileName = fileName.Replace('<', ' ');
			fileName = fileName.Replace('>', ' ');
			fileName = fileName.Replace('|', ' ');
			fileName = Path.Combine(AISConfig.SPOT_ORDER_FILE_DIR, fileName + ".rtf");

			try
			{
				sw = new StreamWriter(fileName, false, System.Text.Encoding.ASCII);
				sw.WriteLine(txt);
				sw.Close();
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, string.Format("{0} {1}"), new object[] { orderData.Property.ClientId, property, txt, id });
			}
		}

		private void GeneratePhotoOrderId_ExecuteCode(object sender, EventArgs e)
		{
			List<ProductTypes> productTypesToInclude = new List<ProductTypes>();
			productTypesToInclude.Add(ProductTypes.Photography);

			orderData.PhotoOrderId = GeneratePhoto(productTypesToInclude);
		}

		private void CreatePhotoFile_ExecuteCode(object sender, EventArgs e)
		{
			FormatOrder formatOrder = new FormatOrder(orderData, orderData.PhotoOrderId);

			if (orderData.PhotoOrderId > 0)
			{
				photoFileName = formatOrder.FileNamePhoto;
			}
			else
			{
				photoFileName = formatOrder.FileNamePhotoNoJobNo;
			}

			WriteOrderFile(photoFileName, formatOrder.GetRichTextFileContents(OrderDisplayType.PhotographyOrderOnly), true);
		}

		private void GeneratePhotoEvent_ExecuteCode(object sender, EventArgs e)
		{
			try
			{
				FormatOrder formatOrder = new FormatOrder(orderData, orderData.PhotoOrderId);
				HtmlFormats html = formatOrder.GetHtmlFileContents();

				NewOrderEvent nOrderEvent = new NewOrderEvent();

				if (orderData.PhotoOrderId > 0)
					nOrderEvent.OrderId = orderData.PhotoOrderId;
				else
					nOrderEvent.OrderId = 0;
				nOrderEvent.ClientId = orderData.Client.ClientID;
				nOrderEvent.HtmlBody = html.ForClient;
				nOrderEvent.HtmlBodyUS = html.ForPhotography;
				nOrderEvent.FileName = (File.Exists(photoFileName) ? photoFileName : "");
				nOrderEvent.Prop = orderData.Property.GetPropertyAddress();
				WorkflowProcessor.GenerateNewPhotoOrderEvent(nOrderEvent);
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, string.Format("{0} {1}"), new Object[] { orderData.Property.ToString() });
			}
		}

		private void GenerateNormalStockID_ExecuteCode(object sender, EventArgs e)
		{
			List<ProductTypes> productTypesToInclude = new List<ProductTypes>();
			productTypesToInclude.Add(ProductTypes.Stockboard);

			// If the order doesn't have the normal board then include the errection fee
			// otherwise the errection fee will be included in the other order.
			// This is to solve problem of the errection fee having its own job#
			if (!orderData.Property.HasPhotosignBoard())
				productTypesToInclude.Add(ProductTypes.ErectionFee);

			orderData.StockId = GenerateOrder(productTypesToInclude);
		}

		private void GenerateManagerStockId_ExecuteCode(object sender, EventArgs e)
		{
			if (orderData.Client.Manager.ManageOwnBoards)
			{
				try
				{
					NewStockBoardOrder nSBOrder = new NewStockBoardOrder();

					nSBOrder.ClientId = orderData.Client.ClientID;
					nSBOrder.State = orderData.Property.State;
					nSBOrder.Loc = orderData.Property.Suburb;
					nSBOrder.Property = orderData.Property.Street;
					//nSBOrder.Caption = (orderData.AnOrder.IncludeImageAndOrTextDetails() ? orderData.AnOrder.Text.Heading : "");
					nSBOrder.Notes = "";

					if (orderData.Property.PreferredErectionDate != null && orderData.Property.PreferredErectionType != PreferredDateType.NotSelected)
					{
						nSBOrder.PreferredErectionDate = orderData.Property.PreferredErectionDate.Value;
						nSBOrder.PreferredErectionType = (int)orderData.Property.PreferredErectionType;
					}

					nSBOrder.PropertyId = orderData.Property.PropertyId;

					orderData.StockId = WorkflowProcessor.CreateNewSBOrder(nSBOrder);


				}
				catch (Exception ex)
				{
					Logger.Exception(ex, string.Format("{0} {1}"), new Object[] { orderData.Property.ToString() });
					orderData.StockId = 0;
				}
			}
		}

		private void CreateStockFile_ExecuteCode(object sender, EventArgs e)
		{
			FormatOrder formatOrder = new FormatOrder(orderData, orderData.StockId);

			if (orderData.StockId > 0)
			{
				stockFileName = formatOrder.FileNameStock;
			}
			else
			{
				stockFileName = formatOrder.FileNameStockNoJobNo;
			}

			WriteOrderFile(stockFileName, formatOrder.GetRichTextFileContents(OrderDisplayType.StockboardOrderOnly), true);
		}

		private void GenerateStockEvent_ExecuteCode(object sender, EventArgs e)
		{
			try
			{
				FormatOrder formatOrder = new FormatOrder(orderData, orderData.StockId);
				HtmlFormats html = formatOrder.GetHtmlFileContents();

				NewOrderEvent nOrderEvent = new NewOrderEvent();
				if (orderData.StockId > 0)
					nOrderEvent.OrderId = orderData.StockId;
				else
					nOrderEvent.OrderId = 0;

				nOrderEvent.ClientId = orderData.Client.ClientID;
				nOrderEvent.HtmlBody = html.ForClient;
				nOrderEvent.HtmlBodyUS = html.ForStockboard;
				nOrderEvent.FileName = (File.Exists(stockFileName) ? stockFileName : "");
				nOrderEvent.Prop = orderData.Property.GetPropertyAddress();

				WorkflowProcessor.GenerateNewSBOrderEvent(nOrderEvent);
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, string.Format("{0} {1}"), new Object[] { orderData.Property.ToString() });
			}
		}

		private void ProcessOnlineListing_ExecuteCode(object sender, EventArgs e)
		{
			orderData.ListingId = GenerateOnlineListing();
		}

		private void NotifyClient_ExecuteCode(object sender, EventArgs e)
		{
			if (orderData.OrderId > 0 || orderData.PhotoOrderId > 0 || orderData.StockId > 0)
				GenerateOrderEventWrapper(true, false, false);
		}

		#region CheckIfOtherProducts
		private void CheckIfOtherProducts(object sender, ConditionalEventArgs e)
		{
			e.Result = BusinessUtils.Utility.IsOtherProductExist(this.orderData.Property);
		}
		#endregion

		private void CheckIfB2BOrder(object sender, ConditionalEventArgs e)
		{
			e.Result = BusinessUtils.Utility.IsItB2BOrder(this.orderData.Property);
		}

		private void CheckIfHasSpotlight(object sender, ConditionalEventArgs e)
		{
			e.Result = BusinessUtils.Utility.IsSpotlightExists(this.orderData.Property);
		}

		private void CheckIfPhotography(object sender, ConditionalEventArgs e)
		{
			e.Result = BusinessUtils.Utility.IsPhotoExists(this.orderData.Property);
		}

		private void CheckIfWorkshopClient(object sender, ConditionalEventArgs e)
		{
			e.Result = orderData.Client.Manager.IsWorkshop;
		}

		private void CheckIfStockBoard(object sender, ConditionalEventArgs e)
		{
			e.Result = BusinessUtils.Utility.IsStockBoardExists(this.orderData.Property);
		}

		private void CheckIfOrderHasOnlineListing(object sender, ConditionalEventArgs e)
		{
			e.Result = BusinessUtils.Utility.IsOnlineListingExists(this.orderData.Property);
		}


		#region GenerateOrder
		private int GenerateOrder(List<ProductTypes> productTypesToInclude)
		{
			int tempOrderId = 0;

			try
			{
				string addNote1 = string.Empty;
				string addNote2 = string.Empty;
				string addNoteAdditional = string.Empty;
				string addNoteAll = string.Empty;

				if (!string.IsNullOrEmpty(orderData.Property.ShopCart.Notes))
					addNoteAll = orderData.Property.ShopCart.Notes + "\r\n";

				//First check to see if the ABCRealestate Link is enabled.
				//If Yes then add that in Notes Section
				if (orderData.Property.ShopCart.ABCReLink)
				{
					addNote1 += "USE ABCRealestate.com.au Link";
				}
				//Then Check to See is there is any Packages Ordered.
				//If Yes then add that Package Name in Notes Section
				foreach (CartItem item in orderData.Property.ShopCart.CartItems)
				{
					if (item.ProductCategory == ProductTypes.ModularPackage)
					{
						addNote2 += item.ItemName + ",";
					}
				}
				if (addNote2.EndsWith(","))
				{
					addNote2 = addNote2.TrimEnd(',') + "\r\n";
				}

				// Not available for stockboard or photography
				if (!productTypesToInclude.Contains(ProductTypes.Stockboard) && !productTypesToInclude.Contains(ProductTypes.Photography))
				{
					if (orderData.Property.PropTextDetails.Bed > 0)
						addNoteAdditional += string.Format("Bed: {0}\r\n", orderData.Property.PropTextDetails.Bed);

					if (orderData.Property.PropTextDetails.Bath > 0)
						addNoteAdditional += string.Format("Bath: {0}\r\n", orderData.Property.PropTextDetails.Bath);

					if (orderData.Property.PropTextDetails.Bed > 0 && orderData.Property.PropTextDetails.Bath > 0)
						addNoteAdditional += string.Format("Display Icons: {0}\r\n", (orderData.Property.PropTextDetails.DisplayIcons) ? "Yes" : "No");

					if (orderData.Property.PropTextDetails.Garage > 0)
						addNoteAdditional += string.Format("Car: {0}\r\n", orderData.Property.PropTextDetails.Garage);

					if (orderData.Property.PropTextDetails.Bed > 0 && orderData.Property.PropTextDetails.Bath > 0)
						addNoteAdditional += string.Format("Pool: {0}\r\n", (orderData.Property.PropTextDetails.Pool) ? "Yes" : "No");
				}

				if (addNote1.Length > 0)
				{
					addNoteAll = addNoteAll + addNote1 + "\r\n";
				}
				if (addNote2.Length > 0)
				{
					addNoteAll = addNoteAll + "\r\nPackages: " + addNote2;
				}
				if (addNoteAdditional.Length > 0)
				{
					addNoteAll = addNoteAll + "\r\n" + addNoteAdditional;
				}

				ErectionDetails ef = orderData.Property.ShopCart.BoardErectionDetails;
				if (ef != null)
				{
					if (ef.ErectionType == BoardInstallationType.High ||
						ef.ErectionType == BoardInstallationType.HigherThanFirstLevel)
					{
						// Because Board and Stockboard share the same errection note so, don't add it again.
						if (orderData.Property.ShopCart.BoardErectionDetails.ErectionNotes.IndexOf(ef.GetDescription()) < 0)
							orderData.Property.ShopCart.BoardErectionDetails.ErectionNotes = string.Format("{0}\r\n{1}", ef.GetDescription(), orderData.Property.ShopCart.BoardErectionDetails.ErectionNotes);
					}
				}

				NewOrder nOrder = new NewOrder();
				nOrder.ClientId = orderData.Property.ClientId;
				nOrder.LocId = orderData.Property.LocationId;
				nOrder.Property = orderData.Property.Street;

				// Not available for stockboard or photography
				if (!productTypesToInclude.Contains(ProductTypes.Stockboard) && !productTypesToInclude.Contains(ProductTypes.Photography))
				{
					nOrder.Caption = orderData.Property.IncludeImageAndOrTextDetails() ? orderData.Property.PropTextDetails.Heading : "";
					//runSp.SpParams[3].Value = (orderData.Property.ShopCart.IncludeImageAndOrTextDetails() ? orderData.Property.PropTextDetails.Heading : "");
				}
				else
					nOrder.Caption = "";

				nOrder.Notes = addNoteAll;
				nOrder.NoBoards = orderData.Property.OrderOnlyHasNonBoardItems();
				nOrder.ErectionNotes = orderData.Property.ShopCart.BoardErectionDetails.ErectionNotes;
				nOrder.SendBy = orderData.Property.SendProofBy;
				nOrder.SendTo = orderData.Property.SendProofTo;
				nOrder.OrderData = orderData.StrOrder;
				nOrder.RefNo = orderData.Property.ClientsReferenceNo;
				nOrder.TransformListing = orderData.Property.TransformListing;
				nOrder.SendSms = orderData.Property.IncludeSmsSticker;

				SmsService smsItem = orderData.Property.GetSmsCartItem() as SmsService;

				if (orderData.Property.IncludeSmsSticker && smsItem != null)
				{
					nOrder.SmsText = smsItem.SmsText;
					nOrder.SmsAgentMobileNo = smsItem.AgentMobileNo;
					nOrder.SmsNotifyAgent = smsItem.NotifyAgent;
					nOrder.SmsSendEmail = smsItem.SendEmail;
					nOrder.SmsAgentEmailAdd = smsItem.AgentEmailAddress;
					nOrder.MMS_Allowed = smsItem.AllowMMS;
				}

				nOrder.InddTemplatesAvail = orderData.Property.InDesignTemplatesAvail;
				if (orderData.Property.PropTextDetails.SaleType != null)
				{
					nOrder.HasCommunityBoard = (orderData.Property.PropTextDetails.SaleType == "Community Board");
				}

				if (productTypesToInclude.Exists(pt => pt == ProductTypes.Board || pt == ProductTypes.Stockboard))
				{
					if (orderData.Property.PreferredErectionDate != null && orderData.Property.PreferredErectionType != PreferredDateType.NotSelected)
					{
						nOrder.PreferredErectionDate = orderData.Property.PreferredErectionDate.Value;
						nOrder.PreferredErectionType = (int)orderData.Property.PreferredErectionType;
					}
					if (orderData.Property.PreferredRemovalDate != null && orderData.Property.PreferredRemovalType != PreferredDateType.NotSelected)
					{
						nOrder.PreferredRemovalDate = orderData.Property.PreferredRemovalDate.Value;
						nOrder.PreferredRemovalType = (int)orderData.Property.PreferredRemovalType;
					}
				}

				nOrder.PropertyId = orderData.Property.PropertyId;
				nOrder.ProductTypesToInclude = productTypesToInclude;
				nOrder.CartItems = orderData.Property.ShopCart.CartItems;
				nOrder.ManagerId = orderData.Client.ManagerID;

				tempOrderId = WorkflowProcessor.CreateNewOrder(nOrder);

			}
			catch (Exception ex)
			{

				Logger.Exception(ex, string.Format("{0} {1}"), new Object[] { orderData.Property.ShopCart.ToString() });
				return 0;
			}
			return tempOrderId;
		}
		#endregion


		private bool WriteOrderFile(string fileNameToUse, string contents)
		{
			bool ret = false;
			System.IO.StreamWriter sw = null;
			int cc = 0;
		RetryCreate:
			try
			{
				sw = new StreamWriter(fileNameToUse);
				sw.WriteLine(contents);
				sw.Close();
				ret = true;
			}
			catch (Exception ex)
			{
				cc++; if (cc < 2) goto RetryCreate;
				Logger.Exception(ex, string.Format("{0} {1}"), new object[] { fileNameToUse, contents });
			}
			return ret;
		}

		private bool WriteOrderFile(string fileNameToUse, string contents, bool useDefaultCoding)
		{
			bool ret = false;
			System.IO.StreamWriter sw = null;
			int cc = 0;
		RetryCreate:
			try
			{
				if (useDefaultCoding)
					sw = new StreamWriter(fileNameToUse, false, System.Text.Encoding.ASCII);
				else
					sw = new StreamWriter(fileNameToUse, false, System.Text.Encoding.UTF8);
				sw.WriteLine(contents);
				sw.Close();
				ret = true;
			}
			catch (Exception ex)
			{
				cc++; if (cc < 2) goto RetryCreate;
				Logger.Exception(ex, string.Format("{0} {1}"), new object[] { fileNameToUse, contents });
			}
			return ret;
		}


		private void GenerateOrderEventWrapper(bool forClient, bool forAbc, bool forB2B)
		{
			try
			{
				FormatOrder formatOrder = new FormatOrder(orderData, orderData.OrderId);
				HtmlFormats html = formatOrder.GetHtmlFileContents();

				string attachments = "";
				if (File.Exists(fileName)) attachments = fileName;

				NewOrderEvent nOrderEvent = new NewOrderEvent();

				if (orderData.OrderId > 0)
					nOrderEvent.OrderId = orderData.OrderId;
				else
					nOrderEvent = null;

				nOrderEvent.ClientId = orderData.Property.ClientId;

				if (forClient)
					nOrderEvent.HtmlBody = html.ForClient;
				else
					nOrderEvent.HtmlBody = null;

				if (forB2B)
				{
					nOrderEvent.HtmlBodyUS = html.ForB2B;
				}
				else if (forAbc)
				{
					nOrderEvent.HtmlBodyUS = html.ForAbc;
				}
				else
					nOrderEvent.HtmlBodyUS = null;


				//if NZ then we send the xml file as well
				if (AISConfig.IS_NZ)
				{
					if (xmlFileName.Length > 0 && File.Exists(xmlFileName))
						attachments += ";" + xmlFileName;
				}

				nOrderEvent.FileName = attachments;
				nOrderEvent.Prop = orderData.Property.GetPropertyAddress();

				WorkflowProcessor.GenerateNewOrderEvent(nOrderEvent);

			}
			catch (Exception ex)
			{
				Logger.Exception(ex, string.Format("{0} {1}"), new Object[] { orderData.Property.ToString() });
			}
		}


		private OnlineDocument GetPrecedentedDocument(List<OnlineDocument> onlineDocuments, Dictionary<string, int> precedenceMapping)
		{
			OnlineDocument document = null;
			int currentNumOfPage = 0;
			foreach (OnlineDocument doc in onlineDocuments)
			{
				if (!string.IsNullOrEmpty(doc.ContentType))
				{
					if (precedenceMapping.ContainsKey(doc.ContentType))
					{
						int numOfPage = precedenceMapping[doc.ContentType];
						if (numOfPage > currentNumOfPage)
						{
							document = doc;
							currentNumOfPage = numOfPage;
						}
					}
				}
				else
				{
					return onlineDocuments[0];
				}
			}

			if (document == null)
				document = onlineDocuments[0];

			return document;
		}

		#region FeedContents
		private Dom.Document FeedContents(OnlineDocument onlineDocument, string policyModel, string documentModel)
		{
			if (string.IsNullOrEmpty(documentModel)) return null;

			Dom.Document doc = Dom.Document.Deserialize(documentModel);
			if (doc == null) return null;

			Abc.Business.Entities.Property property = orderData.Property;
			Abc.AIS.Entities.Client clientRow = orderData.Client;

			try
			{
				if (!string.IsNullOrEmpty(policyModel))
				{
					Dom.DocumentPolicy policy = Dom.DocumentPolicy.Deserialize(policyModel);
					if (policy != null)
						doc.MergeWithPolicy(policy);
				}

				if (string.IsNullOrEmpty(property.PropTextDetails.BroHeading))
					property.PropTextDetails.BroHeading = property.PropTextDetails.Heading;

				if (string.IsNullOrEmpty(property.PropTextDetails.BroSubHeading))
					property.PropTextDetails.BroSubHeading = property.PropTextDetails.SubHeading;

				if (string.IsNullOrEmpty(property.PropTextDetails.BroBodyCopy))
					property.PropTextDetails.BroBodyCopy = property.PropTextDetails.BodyCopy;

				FeedFormElement(doc, "glgr_SaleType", property.SaleInfo.SaleType);

				FeedContent(doc.Root, "OrderId", orderData.OrderId.ToString());
				FeedContent(doc.Root, "ClientId", orderData.Property.ClientId.ToString());
				FeedContent(doc.Root, "ClientName", clientRow.ClientName);
				FeedContent(doc.Root, "ClientOffice", clientRow.Office);

				if (clientRow.Address != null) FeedContent(doc.Root, "ClientAddress", clientRow.Address);
				if (clientRow.Suburb != null) FeedContent(doc.Root, "ClientSuburb", clientRow.Suburb);
				if (clientRow.State != null) FeedContent(doc.Root, "ClientState", clientRow.State);
				if (clientRow.PostCode != null) FeedContent(doc.Root, "ClientPostCode", clientRow.PostCode);
				if (clientRow.Phone != null) FeedContent(doc.Root, "ClientPhone", clientRow.Phone);
				if (clientRow.Fax != null) FeedContent(doc.Root, "ClientFax", clientRow.Fax);
				if (clientRow.Email != null) FeedContent(doc.Root, "ClientEmail", clientRow.Email);
				if (clientRow.LicenceNo != null) FeedContent(doc.Root, "ClientLicNo", clientRow.LicenceNo);

				if (property.PropTextDetails != null)
				{
					FeedContent(doc.Root, "Heading", property.PropTextDetails.Heading);
					FeedContent(doc.Root, "SubHeading", property.PropTextDetails.SubHeading);
					FeedContent(doc.Root, "BrochureHeading", property.PropTextDetails.BroHeading);
					FeedContent(doc.Root, "BrochureSubHeading", property.PropTextDetails.BroSubHeading);
					FeedContent(doc.Root, "BodyCopy", property.PropTextDetails.BodyCopy);
					FeedContent(doc.Root, "BrochureBodyCopy", property.PropTextDetails.BroBodyCopy);
					FeedContent(doc.Root, "AuctionDetails", property.PropTextDetails.AuctionDetails.Text);

					FeedContent(doc.Root, "IconBed", property.PropTextDetails.Bed.ToString());
					FeedContent(doc.Root, "IconBath", property.PropTextDetails.Bath.ToString());
					FeedContent(doc.Root, "IconCar", property.PropTextDetails.Garage.ToString());
					FeedContent(doc.Root, "IconPool", (property.PropTextDetails.Pool) ? "1" : "");
					FeedContent(doc.Root, "IconStudy", property.PropTextDetails.Studyroom.ToString());

					// For backward compatibility
					FeedContent(doc.Root, "Bed", property.PropTextDetails.Bed.ToString());
					FeedContent(doc.Root, "Bath", property.PropTextDetails.Bath.ToString());
					FeedContent(doc.Root, "Car", property.PropTextDetails.Garage.ToString());
					FeedContent(doc.Root, "Pool", (property.PropTextDetails.Pool) ? "1" : "");
					FeedContent(doc.Root, "Study", property.PropTextDetails.Studyroom.ToString());

					FeedContent(doc.Root, "AHDetails", property.PropTextDetails.TextContactDetails);
					FeedContent(doc.Root, "ConjunctionalDetails", property.PropTextDetails.Conjunction);
					FeedContent(doc.Root, "InspectionDetails", property.PropTextDetails.InspectionDetails.Text);
					FeedContent(doc.Root, "TermsConditions", property.PropTextDetails.Terms);
				}

				FeedContent(doc.Root, "UnitNo", property.UnitNo);
				FeedContent(doc.Root, "PropertyAddress", property.GetPropertyAddress());
				FeedContent(doc.Root, "PostCode", property.PostCode);
				FeedContent(doc.Root, "State", property.State);
				FeedContent(doc.Root, "StreetName", property.StreetName);
				FeedContent(doc.Root, "StreetNo", property.StreetNo);
				FeedContent(doc.Root, "Suburb", property.Suburb);


				if (property.SaleInfo != null)
				{
					FeedContent(doc.Root, "SaleType", property.SaleInfo.SaleType);
					FeedContent(doc.Root, "AuctionDate", property.SaleInfo.AuctionDate);
					FeedContent(doc.Root, "AuctionTime", property.SaleInfo.AuctionTime);
				}

				FeedContent(doc.Root, "ClientsReferenceId", property.ClientsReferenceNo);

				int index = 1;
				foreach (AgentContact contact in property.PropTextDetails.AgentContacts)
				{
					FeedContent(doc.Root, string.Format("ContactFirstName{0}", index), contact.FirstName);
					FeedContent(doc.Root, string.Format("ContactLastName{0}", index), contact.LastName);
					FeedContent(doc.Root, string.Format("ContactMobile{0}", index), contact.Mobile);
					FeedContent(doc.Root, string.Format("ContactPhone{0}", index), contact.Phone);
					FeedContent(doc.Root, string.Format("ContactEmail{0}", index), contact.Email);
					index++;
				}

				if (onlineDocument.ContentType == "Colour Front/Back")
				{
					ProcessLayers(doc, "CT_ColourColour", "CT_CC", true);
					ProcessLayers(doc, "CT_ColourMono", "CT_CM", false);
				}
				else if (onlineDocument.ContentType == "Colour Front/Mono Back")
				{
					ProcessLayers(doc, "CT_ColourColour", "CT_CC", false);
					ProcessLayers(doc, "CT_ColourMono", "CT_CM", true);
				}
				else if (onlineDocument.ContentType == "Colour Front")
				{
					ProcessLayers(doc, "CT_ColourColour", "CT_CC", false);
					ProcessLayers(doc, "CT_ColourMono", "CT_CM", false);
					if (doc.Pages != null && doc.Pages.Count == 2)
					{
						// The page number is the page name.
						Dom.Page page = doc.GetPageByName("2");
						if (page != null)
							page.Visible = false;
					}
				}

				doc.Name = onlineDocument.SizeCode;
			}
			catch (Exception ex)
			{
				string mes = string.Format("Policy {0} null, ", string.IsNullOrEmpty(policyModel) ? "is" : "is not");
				mes += string.Format("Document {0} null.", string.IsNullOrEmpty(documentModel) ? "is" : "is not");
				Logger.Exception(ex, mes);
			}

			return doc;
		}

		private void FeedContent(Dom.Tag root, string tagName, string aValue)
		{
			List<Dom.Tag> tags = root.FindAll(delegate(Dom.Tag t) { return (t.TagName == tagName); });
			foreach (Dom.Tag tag in tags)
			{
				if (tag.FormElement == null) continue;

				if (string.IsNullOrEmpty(aValue))
					tag.FormElement.Value = string.Empty;
				else
					tag.FormElement.Value = aValue;
			}
		}

		private void FeedFormElement(Dom.Document doc, string formId, string aValue)
		{
			List<Dom.FormElement> fes = doc.FormElements.FindAll(delegate(Dom.FormElement fe) { return (fe.Id == formId); });
			foreach (Dom.FormElement fe in fes)
			{
				fe.Value = aValue;
			}
		}

		private void ProcessLayers(Dom.Document doc, string layerName, string alternateLayerName, bool visible)
		{
			Dom.Layer layer = doc.GetLayerByNameStartsWith(layerName);
			if (layer == null)
				layer = doc.GetLayerByNameStartsWith(alternateLayerName);

			if (layer != null)
				layer.Visible = visible;
		}
		#endregion


		#region GeneratePhoto
		private int GeneratePhoto(List<ProductTypes> productTypesToInclude)
		{
			int tempPhotoOrderId = 0;
			// This whole method will run in a transaction
			System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(orderData.ConnectionString);
			conn.Open();
			System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

			try
			{
				string addNoteAll = string.Empty;

				if (!string.IsNullOrEmpty(orderData.Property.ShopCart.Notes))
					addNoteAll = orderData.Property.ShopCart.Notes + "\r\n";

				// Try to get the photoOrder from abc.orders.photo items
				Photo photoOrder = null;
				foreach (CartItem item in orderData.Property.ShopCart.CartItems)
				{
					if (item.ProductCategory == ProductTypes.Photography)
					{
						photoOrder = (Photo)item;
						break;
					}
				}

				NewPhotoOrder nPhotoOrder = new NewPhotoOrder();

				nPhotoOrder.ClientId = orderData.Client.ClientID;
				nPhotoOrder.LocId = orderData.Property.LocationId;
				nPhotoOrder.Property = orderData.Property.Street;
				//nPhotoOrder.Caption = (orderData.AnOrder.IncludeImageAndOrTextDetails() ? orderData.AnOrder.Text.Heading : ""); //Captain, type manually
				nPhotoOrder.Notes = addNoteAll;
				nPhotoOrder.ErectionNotes = orderData.Property.ShopCart.BoardErectionDetails.ErectionNotes;
				nPhotoOrder.OrderData = orderData.StrOrder; //Orderdata, can be null
				nPhotoOrder.RefNo = orderData.Property.ClientsReferenceNo; // RefNo, can be null
				nPhotoOrder.Instructions = photoOrder.OtherIns;
				nPhotoOrder.VendorName = photoOrder.ContactName;
				nPhotoOrder.VendorPhone = photoOrder.ContactPhone;
				nPhotoOrder.IsKeySafe = photoOrder.KeySafe;
				nPhotoOrder.IsPickupKeys = photoOrder.PickupKeys;
				nPhotoOrder.PhotoContact = photoOrder.ArrangeContact;

				nPhotoOrder.HouseFaces = photoOrder.HouseFaces;
				nPhotoOrder.Melway = photoOrder.MelWay;

				nPhotoOrder.SendBy = orderData.Property.SendProofBy;
				nPhotoOrder.SendTo = orderData.Property.SendProofTo;

				nPhotoOrder.PropertyId = orderData.Property.PropertyId;

				nPhotoOrder.CartItems = orderData.Property.ShopCart.CartItems;
				nPhotoOrder.ProductTypesToInclude = productTypesToInclude;
				nPhotoOrder.ManagerId = orderData.Client.ManagerID;
				tempPhotoOrderId = WorkflowProcessor.CreateNewPhotoOrder(nPhotoOrder);

			}
			catch (Exception ex)
			{
				Logger.Exception(ex, string.Format("{0} {1}"), new Object[] { orderData.Property.ToString() });
				return 0;
			}

			return tempPhotoOrderId;
		}
		#endregion

		#region GenerateOnlineListing
		private int GenerateOnlineListing()
		{
			int ret = 0;

			try
			{
				OnlineListing obj = (OnlineListing)orderData.Property.ListingToTransform;

				NewOnlineListing nOnlineListing = new NewOnlineListing();

				nOnlineListing.ListingType = (int)obj.ListingType;
				nOnlineListing.ClientId = orderData.Client.ClientID;
				nOnlineListing.Ptype = obj.PType;
				nOnlineListing.LocationId = orderData.Property.LocationId;
				nOnlineListing.PAddress = orderData.Property.Street;
				nOnlineListing.StreetName = orderData.Property.StreetName;
				nOnlineListing.StreetNo = orderData.Property.StreetNo;
				if (orderData.Property.UnitNo != null && orderData.Property.UnitNo.Length > 0)
					nOnlineListing.UnitNo = orderData.Property.UnitNo;
				else
					nOnlineListing.UnitNo = "";
				nOnlineListing.Heading = orderData.Property.PropTextDetails.Heading;
				nOnlineListing.SubHeading = orderData.Property.PropTextDetails.SubHeading;
				nOnlineListing.BodyText = orderData.Property.PropTextDetails.BodyCopy;
				if (orderData.Property.PropTextDetails.InspectionDetails.Text.Length > 0)
					nOnlineListing.InspectionDetails = orderData.Property.PropTextDetails.InspectionDetails.Text;
				else
					nOnlineListing.InspectionDetails = "";
				if (orderData.Property.SaleInfo.SaleType.ToUpper() == "AUCTION" || obj.Authority == "Auction")
				{
					try
					{
						string dd = orderData.Property.SaleInfo.AuctionDate;
						if (orderData.Property.SaleInfo.AuctionTime.Length > 0) dd += " " + orderData.Property.SaleInfo.AuctionTime;
						DateTime dt = Convert.ToDateTime(dd);
						nOnlineListing.AuctionDate = dt;
					}
					catch (Exception ex)
					{
						Logger.Exception(ex, "Error on Convert Auction Date");
						nOnlineListing.AuctionDate = null;
					}
					nOnlineListing.AuctionView = orderData.Property.PropTextDetails.AuctionDetails.Text;
				}
				else
				{
					nOnlineListing.AuctionDate = null;
					nOnlineListing.AuctionView = "";
				}
				nOnlineListing.HidePrice = (!obj.ShowPrice);
				if (obj.PriceView.Trim().Length > 0) nOnlineListing.PriceView = obj.PriceView;

				// If Rent Per Month is Given then Convert to to Rent Per Week as well
				// as Searching is always on Rent Per Week.
				if (obj.RentPerMonth > 0) obj.Rent = (int)(obj.RentPerMonth / 4.3);

				if (obj.Rent > 0) nOnlineListing.Rent = obj.Rent;
				if (obj.Lease > 0) nOnlineListing.Lease = obj.Lease;
				if (obj.Bed > 0) nOnlineListing.Bed = obj.Bed;
				if (obj.Bath > 0) nOnlineListing.Bath = obj.Bath;
				if (obj.Car > 0) nOnlineListing.Car = obj.Car;
				if (obj.Land > 0) nOnlineListing.Land = obj.Land;
				if (obj.AvailFrom != null && obj.AvailFrom.Length > 0)
					nOnlineListing.AvlFrom = Convert.ToDateTime(obj.AvailFrom);
				else
				{
					nOnlineListing.AvlFrom = null;
				}

				nOnlineListing.Display = obj.DisplayListing;

				nOnlineListing.ShowAddress = orderData.Property.ShowAddress;

				if (obj.Price > 0) nOnlineListing.Price = obj.Price;
				if (obj.RentPerMonth > 0) nOnlineListing.RentPerMonth = obj.RentPerMonth;

				nOnlineListing.ShowRent = obj.ShowRent;

				if (obj.Authority.Length > 0)
					nOnlineListing.Authority = obj.Authority;
				else
					nOnlineListing.Authority = null;

				nOnlineListing.LandSizeUnit = obj.LandSizeUnit;

				nOnlineListing.ListingAddedBy = obj.ListingAddedBy;

				ret = WorkflowProcessor.CreateNewOnlineListing(nOnlineListing);

			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Error on GenerateOnlineListing");
				ret = 0;
				throw;
			}
			finally
			{
			}

			return ret;
		}
		#endregion

		private void WFExceptionHandler_ExecuteCode(object sender, EventArgs e)
		{
			if (wfFaultHandlerActivity.Fault != null)
				Logger.Exception(wfFaultHandlerActivity.Fault, "Error on Order Processing Workflow");
		}
	}

}
