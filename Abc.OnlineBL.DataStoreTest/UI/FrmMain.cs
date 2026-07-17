using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Abc.OnlineBL.DataStoreTest.Commands;
using TemplateManagerCommands = Abc.OnlineBL.DataStoreTest.Commands.TemplateManagerCommands;
using AOPCommands = Abc.OnlineBL.DataStoreTest.Commands.AOPCommands;
using ProductCommands = Abc.OnlineBL.DataStoreTest.Commands.ProductCommands;
using ClientCommands = Abc.OnlineBL.DataStoreTest.Commands.ClientCommands;
using Abc.OnlineBL.Utility;
using Abc.OnlineBL.ServiceProxy;
using Abc.OnlineBL.Entities;
using Abc.OnlineBL.Entities.Model;
using System.Data.SqlClient;
using Abc.OnlineBL.Entities.Model.OnlineOrder;
using Abc.OnlineBL.Utility.WhereIs;

namespace Abc.OnlineBL.DataStoreTest.UI
{
    public partial class FrmMain : Form
    {
        #region Constructor
        public FrmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region ExecuteCommand
        private void ExecuteCommand(BaseCommand cmd)
        {
            try
            {
                object ret = cmd.Execute();
                if (ret == null)
                    richTextBox.Text = "Function returns 'null'";
                else
                    richTextBox.Text = ret.ToString();
            }
            catch (Exception ex)
            {
                richTextBox.Text = ex.ToString();
            }
        }
        #endregion

        #region btnSayHello_Click
        private void btnSayHello_Click(object sender, EventArgs e)
        {
            ClientCommands.SayHelloCommand cmd = new ClientCommands.SayHelloCommand();
            ExecuteCommand(cmd);
        }
        #endregion

        #region btnGetClientOffices_Click
        private void btnGetClientOffices_Click(object sender, EventArgs e)
        {
            TemplateManagerCommands.GetClientsWhoCanOrderProductCommand cmd = new TemplateManagerCommands.GetClientsWhoCanOrderProductCommand();
            ExecuteCommand(cmd);
        }
        #endregion

        #region btnRunGetClientsByNameWithLoadOptionsCommand_Click
        private void btnRunGetClientsByNameWithLoadOptionsCommand_Click(object sender, EventArgs e)
        {
            ClientCommands.GetClientsByNameWithLoadOptionsCommand cmd = new ClientCommands.GetClientsByNameWithLoadOptionsCommand();
            ExecuteCommand(cmd);
        }
        #endregion

        #region btnGetProduct_Click
        private void btnGetProduct_Click(object sender, EventArgs e)
        {
            ProductCommands.GetProductByIdCommand cmd = new ProductCommands.GetProductByIdCommand();
            ExecuteCommand(cmd);
        }
        #endregion

        private void btnUpdateClientsPref_Click(object sender, EventArgs e)
        {
            ClientCommands.UpdateClientsPrefCommand cmd = new ClientCommands.UpdateClientsPrefCommand();
            ExecuteCommand(cmd);
        }

        private void btnGetTemplatesByPaths_Click(object sender, EventArgs e)
        {
            AOPCommands.GetTemplatesByPathsCommand cmd = new AOPCommands.GetTemplatesByPathsCommand();
            ExecuteCommand(cmd);
        }

        private void btnGetProductsByTypeId_Click(object sender, EventArgs e)
        {
            TemplateManagerCommands.GetProductsByTypeIdCommand cmd = new TemplateManagerCommands.GetProductsByTypeIdCommand();
            ExecuteCommand(cmd);
        }

        private void lnkGetProductsByIds_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TemplateManagerCommands.GetProductsByIdsCommand cmd = new TemplateManagerCommands.GetProductsByIdsCommand();
            ExecuteCommand(cmd);
        }

        private void btnUpdateTemplate_Click(object sender, EventArgs e)
        {
            AOPCommands.UpdateTemplateCommand cmd = new AOPCommands.UpdateTemplateCommand();
            ExecuteCommand(cmd);
        }

        private void btnInsertTemplate_Click(object sender, EventArgs e)
        {
            AOPCommands.InsertTemplateCommand cmd = new AOPCommands.InsertTemplateCommand();
            ExecuteCommand(cmd);
        }

        private void btnDeleteTemplate_Click(object sender, EventArgs e)
        {
            AOPCommands.DeleteTemplateCommand cmd = new AOPCommands.DeleteTemplateCommand();
            ExecuteCommand(cmd);
        }

        private void btnClientSayHello1_Click(object sender, EventArgs e)
        {
            richTextBox.AppendText("calling hello 1\r\n");
            BackgroundWorkerHelper.DoWork<string>(() =>
            {
                return ServiceFactory.ClientService.SayHello("My name");
            }, (args) =>
            {
                if (args.Error != null)
                {
                    richTextBox.AppendText("Returned Result from SayHello1: Exception:" + args.Error.ToString() + "\r\n");
                }
                else
                {
                    richTextBox.AppendText("Returned Result from SayHello1: " + args.Result + "\r\n");
                }
            });
        }

        private void btnClientSayHello2_Click(object sender, EventArgs e)
        {
            richTextBox.AppendText("calling hello 2\r\n");
            BackgroundWorkerHelper.DoWork<string>(() =>
            {
                throw new Exception("test");
                //return myProxy.SayHello("My name");
            }, (args) =>
            {
                if (args.Error != null)
                {
                    richTextBox.AppendText("Returned Result from SayHello2: Exception:" + args.Error.ToString() + "\r\n");
                }
                else
                {
                    richTextBox.AppendText("Returned Result from SayHello2: " + args.Result + "\r\n");
                }
            });
            //BackgroundWorker bg = new BackgroundWorker();
            //bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            //bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            //bg.RunWorkerAsync();
            //bg.ProgressChanged += new ProgressChangedEventHandler(bg_ProgressChanged);
        }

        void bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
                MessageBox.Show("Output:" + e.Result);

            if (e.Error != null)
                MessageBox.Show("Output:" + e.Error.ToString());
            else
                MessageBox.Show("Output:" + e.Result);
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorkerExample()
        {
            richTextBox.AppendText("calling hello 2\r\n");
            BackgroundWorkerHelper.DoWork<string, int>("124", new Func<DoWorkArgument<string>, int>(ConvertNumber), new Action<WorkerResult<int>>(Display));

            // Different to do the same thing using lamda function
            BackgroundWorkerHelper.DoWork<string, int>("124", (input) =>
            {
                return ConvertNumber(input);
            }, (result) =>
            {
                MessageBox.Show(result.Result.ToString());
            });
        }

        //public delegate int ConvertNumberDelegate(DoWorkArgument<string> sNumber);
        public int ConvertNumber(DoWorkArgument<string> sNumber)
        {
            return System.Convert.ToInt32(sNumber.Argument);
        }

        //public delegate void DisplayDelegate(WorkerResult<int> result);
        public void Display(WorkerResult<int> result)
        {
            MessageBox.Show(result.Result.ToString());
        }

        private void btnGetTemplateProductsMatchingPriceList_Click(object sender, EventArgs e)
        {
            AOPCommands.GetTemplateProductsMatchingPriceListCommand cmd = new AOPCommands.GetTemplateProductsMatchingPriceListCommand();
            ExecuteCommand(cmd);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Commands.WorkflowCommands.SayHelloCommand cmd = new Abc.OnlineBL.DataStoreTest.Commands.WorkflowCommands.SayHelloCommand();
            ExecuteCommand(cmd);
        }

        private void btnGetAllTrucks_Click(object sender, EventArgs e)
        {
            List<int> objOrderList = new List<int>();

            /*Reverse Order
            objOrderList.Add(332895);
            objOrderList.Add(363576);
            objOrderList.Add(363612);
            objOrderList.Add(363613);
            objOrderList.Add(903884);
            objOrderList.Add(363446);
            objOrderList.Add(11313);
             */


            objOrderList.Add(363446);
            objOrderList.Add(903884);
            objOrderList.Add(363613);
            objOrderList.Add(363612);
            objOrderList.Add(363576);
            objOrderList.Add(332895);
            objOrderList.Add(11313);


            /*ReOrder and job of a runsheet
             ServiceFactory.ManagerService.SetReOrderRunsheetDetailByRunsheetID(11313, objOrderList);
            */

            /* Markoffing a runsheet with its child job
            List<int> objRIDList = new List<int>();
            objRIDList.Add(11313);
            ServiceFactory.ManagerService.SetRunsheetsMarkOffbyRID(objRIDList,2);
            */

            //Markoff and a specific job in a specific runsheet
            //try
            //{
            //    bool result = ServiceFactory.ManagerService.JobCompleteAndMarkOffed(363284, 11296, false);
            //}
            //catch (SqlException sqlex)
            //{
            //    string str1 = sqlex.Message;
            //}
            //catch (Exception ex)
            //{
            //    string str = ex.Message;
            //}
            #region Old Test
			//List<Truck> trucks = ServiceFactory.ManagerService.GetAllTrucks(true);
			//MessageBox.Show("Total truck found: " + trucks.Count.ToString());            
            #endregion

			MessageBox.Show(DateTime.Now.ToString());

			List<Abc.OnlineBL.Entities.Model.RunsheetModel> list = ServiceFactory.ManagerService.GetRunsheetDetailsByTruckIDNDate(49, DateTime.Now);
			MessageBox.Show(DateTime.Now.ToString());
		}

        private void BtnGetOrderSubsetByOrderRange_Click(object sender, EventArgs e)
        {
            richTextBox.AppendText("GetOrderSubsetByOrderRange1\r\n");

            BackgroundWorkerHelper.DoWork<List<Order>>(() =>
            {
                return ServiceFactory.OrderService.GetOrderSubsetByOrderRange(60000, 60999);
            }, (args) =>
            {
                if (args.Error != null)
                {
                    richTextBox.AppendText("Returned Result from GetOrderSubsetByOrderRange: Exception:" + args.Error.ToString() + "\r\n");
                }
                else
                {
                    richTextBox.AppendText("Returned Result from GetOrderSubsetByOrderRange: " + args.Result.Count.ToString() + "\r\n");
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder order = new Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder();
                order.PropertyId = 999;
                order.ClientId = 3728;

                string xml = ObjectUtility.Serialize<Abc.OnlineBL.Entities.Model.OnlineOrder.OnlinePropertyOrder>(order);
                richTextBox.Text = xml;
            }
            catch (Exception ex)
            {
                richTextBox.Text = ex.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string xmlData = @"
<ProductConfig>
	<fields>
		<field>
			<fieldName>Qty</fieldName>
			<caption>Qty</caption>
			<helpText>Enter product quanity</helpText>
			<getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>
			<enabled>true</enabled>
			<value>1</value>
			<fieldType>TextBox</fieldType>
			<validation required=""true"">
				<rangeValidation min=""1"" max=""100""/>
			</validation>
		</field>
		<field>
			<fieldName>Format</fieldName>
			<caption>Format</caption>
			<helpText>Select orientation</helpText>
			<getDefaultFromProductAttribute>true</getDefaultFromProductAttribute>
			<enabled>true</enabled>
			<value></value>
			<fieldType>ComboBox</fieldType>
			<listItems>
				<listItem displayText=""Portrait"" valueText=""Portrait"" />
				<listItem displayText=""Landscape"" valueText=""Landscape"" />
			</listItems>
		</field>
		<field>
			<fieldName>Test</fieldName>
			<caption>Test Check</caption>
			<helpText>Select Test</helpText>
			<getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>
			<enabled>true</enabled>
			<value>false</value>
			<fieldType>CheckBox</fieldType>
			<validation required=""true""></validation>
		</field>
		<field>
			<fieldName>TestRad</fieldName>
			<caption>Test Radio</caption>
			<helpText>Select Radio</helpText>
			<getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>
			<enabled>true</enabled>
			<value></value>
			<fieldType>RadioButton</fieldType>
			<listItems>
				<listItem displayText=""rad 1"" valueText=""rad1"" />
				<listItem displayText=""rad 2"" valueText=""rad2"" />
			</listItems>
		</field>
	</fields>
</ProductConfig>";

            string data = @"<?xml version=""1.0"" encoding=""utf-16""?>
<ProductConfig xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <fields>
    <fieldName>Test</fieldName>
    <caption>Test</caption>
    <helpText>Help</helpText>
    <getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>
    <enabled>true</enabled>
    <value />
    <fieldType>TextBox</fieldType>
    <validation>
      <rangeValidation>
        <min>1</min>
        <max>100</max>
      </rangeValidation>
    </validation>
  </fields>
  <fields>
    <fieldName>Test</fieldName>
    <caption>Test</caption>
    <helpText>Help</helpText>
    <getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>
    <enabled>true</enabled>
    <value />
    <fieldType>TextBox</fieldType>
    <validation>
      <rangeValidation>
        <min>1</min>
        <max>100</max>
      </rangeValidation>
    </validation>
  </fields>
  <fields>
    <fieldName>Test</fieldName>
    <caption>Test</caption>
    <helpText>Help</helpText>
    <getDefaultFromProductAttribute>false</getDefaultFromProductAttribute>
    <enabled>true</enabled>
    <value />
    <fieldType>TextBox</fieldType>
    <validation>
      <rangeValidation>
        <min>1</min>
        <max>100</max>
      </rangeValidation>
    </validation>
  </fields>
</ProductConfig>";
            try
            {
                ProductConfig config = ProductConfig.GetFromString(xmlData);

                richTextBox.Text = "Total Field Counts:" + config.Fields.Field.Count;
            }
            catch (Exception ex)
            {
                richTextBox.Text = ex.ToString();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                ProductConfig config = new ProductConfig();

                config.Fields.Field.Add(new Field()
                {
                    FieldName = "Test",
                    Caption = "Test",
                    Enabled = true,
                    FieldType = FieldType.TextBox,
                    GetDefaultFromProductAttribute = false,
                    HelpText = "Help",
                    Value = "",
                    Validation = new Validation()
                    {
                        RangeValidation = new RangeValidation()
                        {
                            Min = 1,
                            Max = 100
                        }
                    }
                });
                config.Fields.Field.Add(new Field()
                {
                    FieldName = "Test",
                    Caption = "Test",
                    Enabled = true,
                    FieldType = FieldType.TextBox,
                    GetDefaultFromProductAttribute = false,
                    HelpText = "Help",
                    Value = "",
                    Validation = new Validation()
                    {
                        RangeValidation = new RangeValidation()
                        {
                            Min = 1,
                            Max = 100
                        }
                    }
                });
                config.Fields.Field.Add(new Field()
                {
                    FieldName = "Test",
                    Caption = "Test",
                    Enabled = true,
                    FieldType = FieldType.ComboBox,
                    GetDefaultFromProductAttribute = false,
                    HelpText = "Help",
                    Value = "",
                    Validation = new Validation()
                    {
                        RangeValidation = new RangeValidation()
                        {
                            Min = 1,
                            Max = 100
                        }
                    }
                });
                config.Fields.Field[2].ListItems.ListItem.Add(new ListItem()
                {
                    DisplayText = "Hello",
                    ValueText = "1"
                });
                config.Fields.Field[2].ListItems.ListItem.Add(new ListItem()
                {
                    DisplayText = "Hello 2",
                    ValueText = "2"
                });

                richTextBox.Text = ObjectUtility.Serialize<ProductConfig>(config);
            }
            catch (Exception ex)
            {
                richTextBox.Text = ex.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
    <OnlinePropertyOrder xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <ClientId>3728</ClientId>
    <CartCreatedOn>2011-06-16T19:45:37.6811854+06:00</CartCreatedOn>
    <Cart />
    <PackageGroups />
    <IsDIYOrder>false</IsDIYOrder>
    <BoardInstallationType>None</BoardInstallationType>      
    <PreferredBoardRemovalDate xsi:nil=""true"" />      
    <PreferredBoardErectionDate xsi:nil=""true"" />      
    <PreferredBoardRemovalType>On</PreferredBoardRemovalType>      
    <PreferredBoardErectionType>On</PreferredBoardErectionType>      
    <PropertyId>142</PropertyId>     
    </OnlinePropertyOrder>
";
            try
            {
                var obj = ObjectUtility.Deserialize<OnlinePropertyOrder>(xml);
            }
            catch (Exception ex)
            {
                richTextBox.Text = ex.ToString();
            }
        }

        private void GetDocument_Click(object sender, EventArgs e)
        {

            List<EntityRelations> options = new List<EntityRelations>();
            options.Add(EntityRelations.AOP_JobDocument_To_AOP_TemplateProduct);
            options.Add(EntityRelations.AOP_TemplateProduct_To_AOP_Template);
            options.Add(EntityRelations.AOP_JobDocument_To_Order);

            AOP_JobDocument tps = ServiceFactory.AOPService.GetJobDocumentByJobDocumentId(14998);
        }

        private void button6_Click(object sender, EventArgs e)
        {
          //Client client = ServiceFactory.ClientService.GetClientById(3728);
           // List<SmsOrderDetail> test = ServiceFactory.OrderService.GetSMSOrderDetailsByClientID(3728);
            // List<Order> testObj = ServiceFactory.OrderService.GetSmsOrderDetailsForAMonth(2006, 09, 3728);
            //List<Order> testObj = ServiceFactory.OrderService.GetSmsOrderDetailsForAMonth(2009, 09, 3728);

            ///List<InvoiceDetail> obj = ServiceFactory.AccountService.GetInvoiceList(3728, 0, null);

            // List<InvoiceDetail> selectedOrderInvoiceList =new List<InvoiceDetail>();
            //InvoiceDetail obj= new InvoiceDetail();
            //obj.AmountDue=(decimal)5.50;
            //obj.OrderID=363126;
            //selectedOrderInvoiceList.Add(obj);
            //ServiceFactory.AccountService.InsertOnlinePaymentDetails(12, selectedOrderInvoiceList);
            //List<EntityRelations> loadOptions = null;
            //OnlinePayment onlinePayment = ServiceFactory.AccountService.GetOnlinePayment(12, loadOptions);

            //List<EntityRelations> loadOptions = new List<EntityRelations>();
            //loadOptions.Add(EntityRelations.Payment_To_PaymentDetails);
            //Payment payment = ServiceFactory.AccountService.GetPayment(8, loadOptions);

            /*
              List<EntityRelations> loadOptions =  new List<EntityRelations>();
             loadOptions.Add(EntityRelations.OnlinePayment_To_OnlinePaymentDetails);

             /// Call the Service.
             OnlinePayment onlinePaymentObj = ServiceFactory.AccountService.GetOnlinePayment(17, loadOptions);

             if (onlinePaymentObj != null)
             {
                 onlinePaymentObj.TransactionComplete = true;
                 onlinePaymentObj.CBATransactionRef = "transactionSummary";

                 /// Call the Service.
                 Payment paymentObj = ServiceFactory.AccountService.ProcessOnlinePayments(onlinePaymentObj);
                 int paymentId=0;

                 if (paymentObj != null && paymentObj.PaymentID > 0)
                 {  
                     paymentId = paymentObj.PaymentID;
                 }
             }
             */
            /*
             OnlinePayment onlinePaymentObj =new OnlinePayment();
                onlinePaymentObj.ClientId =3728;
                onlinePaymentObj.AccountId =287;
                onlinePaymentObj.CardType = "Visa";
                onlinePaymentObj.Surcharge = (decimal)0.9625;
                onlinePaymentObj.Total = (Decimal)38.50;
                onlinePaymentObj.CreateOn = DateTime.Now;
                onlinePaymentObj.TransactionComplete = false;
                onlinePaymentObj.CBATransactionRef =string.Empty;
                onlinePaymentObj.PaymentId = 0;

               //// onlinePaymentId = Abc.OnlineBL.ServiceProxy.ServiceFactory.AccountService.InsertOnlinePayment(paymentObj);
               // onlinePaymentId = 17;

               // if (this.SelectedInvoiceList == null)
               // {
               //     throw new NullReferenceException(string.Format("onlinePaymentID:{0}, Selected OrderInvoice List is null", onlinePaymentId));
               // }

                /// Insert into OnlinePaymentDetails.
               // Abc.OnlineBL.ServiceProxy.ServiceFactory.AccountService.InsertOnlinePaymentDetails(onlinePaymentId, this.SelectedInvoiceList);
              
                    OnlinePaymentDetail onlinePaymentItem = new OnlinePaymentDetail();
                    onlinePaymentItem.OrderId = 362413;
                    onlinePaymentItem.AmountPaid = (decimal)11.00;
                    onlinePaymentObj.OnlinePaymentDetails.Add(onlinePaymentItem);

                /// Insert into OnlinePayment.
                //OnlinePayment onlinepaymentsObj = Abc.OnlineBL.ServiceProxy.ServiceFactory.AccountService.InsertOnlinePayment(onlinePaymentObj, loadOption);
                OnlinePayment onlinepaymentsObj = Abc.OnlineBL.ServiceProxy.ServiceFactory.AccountService.InsertOnlinePayment(onlinePaymentObj);
                
            */
           // List<InvoiceDetail> objPrev = Abc.OnlineBL.ServiceProxy.ServiceFactory.AccountService.GetInvoiceList(3728, 0, null);

            //List<Order> obj = Abc.OnlineBL.ServiceProxy.ServiceFactory.AccountService.GetOrderInvoice(3728, 0, null);
            //List<FloorplanPackage> obj = Abc.OnlineBL.ServiceProxy.ServiceFactory.FloorPlanService.GetFloorPlanPackageList(3728);
            //  List<EntityRelations> loadOption = new List<EntityRelations>();
            //loadOption.Add(EntityRelations.OnlinePayment_To_OnlinePaymentDetails);
            //OnlinePayment obj = Abc.OnlineBL.ServiceProxy.ServiceFactory.AccountService.GetOnlinePayment(67, loadOption);


            //FloorplanIcon floorPlanIconObj = new FloorplanIcon();
            //floorPlanIconObj.FloorplanId = 1;
            //floorPlanIconObj.X = "102";
            //floorPlanIconObj.Y = "150";
            //floorPlanIconObj.IconType = 1;
            //floorPlanIconObj.FileName = "0781e7be-9b6e-47d4-ab41-1135e8c356ae.jpg";
            //floorPlanIconObj.Caption = "Ditect Insert3";
            //floorPlanIconObj.ArrowDir = "S";
            //Abc.OnlineBL.ServiceProxy.ServiceFactory.FloorPlanService.InsertFloorPlanIcons(floorPlanIconObj);

            //int pgID =1;
            //DateTime dFrom =DateTime.Today;
            //DateTime dateTo = DateTime.Today.AddDays(1).AddSeconds(-1);

           // List<PhotoSysRunsheet> ObjList = Abc.OnlineBL.ServiceProxy.ServiceFactory.PhotoSysRunSheetService.GetPhotoSysRunSheet(pgID, dFrom, dateTo);
            
          //  String confing="";
           // String imei = "000000000000000";

          // confing = Abc.OnlineBL.ServiceProxy.ServiceFactory.PhotoSysRunSheetService.GetPhotographerConfigContent(imei);
          // string firstname="";
          // firstname = Abc.OnlineBL.ServiceProxy.ServiceFactory.PhotoSysRunSheetService.GetPhotographerFirstName(imei);
           /*
            RunsheetDetailCompleted rs=new RunsheetDetailCompleted();
            rs.EmailNote = "Hey Rony";
            rs.IsBoardNotFound = false;
            rs.OrderID = 248036;
            rs.PhotoAttachmentPath = @"/sdcard/JobTrackerPictures/12_248036_NoImageCross.jpg";
            rs.RunID = 12;
            rs.IsRemoval = false;
            bool test = Abc.OnlineBL.ServiceProxy.ServiceFactory.ManagerService.JobCompleteAndMarkOffed(rs);
            */
            //String imei = "000000000000000";
            //string ftpFolderPath = Abc.OnlineBL.ServiceProxy.ServiceFactory.ManagerService.GetFtpFolderPathByImei(imei);

           // int pg =Abc.OnlineBL.ServiceProxy.ServiceFactory.PhotoSysRunSheetService.GetPhotographerID(imei);

          //string test2 = "Test String.";
          //test2 = ftpFolderPath;

        }

		  private void button7_Click(object sender, EventArgs e)
		  {
			  List<AOP_JobQueue> list = ServiceFactory.AOPService.GetQueueItemByStatus(Abc.OnlineBL.Entities.Enums.AOP_QueueStatusList.Waiting_In_Queue);
			  if (list.Count > 0)
			  {
				  var job = list[0];
				  job.SetAsChangeTrackingRoot();
				  job.ConnectorId = 1;
				  job.ConnectorChannelId = Guid.NewGuid().ToString();
				  job.StatusId = 1;

				  var ret = ServiceFactory.AOPService.LockJobQueue(job);

				  if (ret != null)
				  {
					  MessageBox.Show("Lock Job");
				  }
			  }
		  }

		  private void button8_Click(object sender, EventArgs e)
		  {
			  try
			  {
				  var client = ServiceFactory.ClientService.GetClientByUserId("sunshine", EntityRelations.Client_To_UserLogons, EntityRelations.Client_To_ClientContacts);
				  richTextBox.Text = "got client with logon " + client.ClientName + " and user logon count " + client.UserLogons.Count;
			  }
			  catch (Exception ex)
			  {
				  richTextBox.Text = ex.ToString();
			  }
			  
			
		  }

		  private void btnModifyDiyOrder_Click(object sender, EventArgs e)
		  {
			  OnlinePropertyOrder po = new OnlinePropertyOrder();
			  po.ClientId = 3728;
			  po.PropertyId = 826;
			  po.IsDIYOrder = true;

			  CartItem item = new CartItem();
			  item.ItemQty = 1;
			  item.ProductId = 430;
			  item.TypeId = 1;
			  item.SelectedDIYTemplateId = 206;
			  item.WebFriendlyName = "A P - Board_Photo";
			  item.ProductName = "A P - Board_Photo";
			  //add this if want to keep existing template
			  item.IsOldProduct = true;
			  item.OrderDetailsID = 784673;

			  //extra brochure product
			  CartItem item2 = new CartItem();
			  item2.ItemQty = 1;
			  item2.ProductId = 654;
			  item2.TypeId = 2;
			  item2.SelectedDIYTemplateId = 221;
			  item2.WebFriendlyName = "A1 CL";
			  item2.ProductName = "A1 CL";

			  
			  po.Cart.Add(item);
			  po.Cart.Add(item2);

			  ServiceFactory.OrderService.ModifyDIYOrder(1000288, 3728, po, "WORKS", true);
		  }

		  private void btnBoardBrochure_Click(object sender, EventArgs e)
		  {
			  OnlinePropertyOrder po = new OnlinePropertyOrder();
			  po.ClientId = 3728;
			  po.PropertyId = 827;
			  po.IsDIYOrder = true;

			  //Board
			  CartItem item = new CartItem();
			  item.ItemQty = 1;
			  item.ProductId = 430;
			  item.TypeId = 1;
			  item.SelectedDIYTemplateId = 206;
			  item.WebFriendlyName = "A L - Board_Photo";
			  item.ProductName = "A L - Board_Photo";
			  //add this if want to keep existing template
			  //item.IsOldProduct = true;
			  //item.OrderDetailsID = 784685;

			  //extra brochure product
			  CartItem item2 = new CartItem();
			  item2.ItemQty = 1;
			  item2.ProductId = 654;
			  item2.TypeId = 2;
			  item2.SelectedDIYTemplateId = 221;
			  item2.WebFriendlyName = "A1 CL";
			  item2.ProductName = "A1 CL";
			  //add this if want to keep existing template
			  //item2.IsOldProduct = true;
			  //item2.OrderDetailsID = 784686;


			  //extra Window Card product
			  //CartItem itemWindowCard = new CartItem();
			  //itemWindowCard.ItemQty = 1;
			  //itemWindowCard.ProductId = 31;
			  //itemWindowCard.TypeId = 17;
			  //itemWindowCard.SelectedDIYTemplateId = 234; //TemplateProductID
			  //itemWindowCard.WebFriendlyName = "A4 WC";
			  //itemWindowCard.ProductName = "A4 WC";

			  po.Cart.Add(item);
			  po.Cart.Add(item2);
			  //po.Cart.Add(itemWindowCard);

			  ServiceFactory.OrderService.ModifyDIYOrder(1000292, 3728, po, "WORKS", true);
		  }

		  private void button9_Click(object sender, EventArgs e)
		  {
			  var a = ServiceFactory.OrderService.GetDIYOrder(1000284);
		  }

		  private void btnBBWC_Click(object sender, EventArgs e)
		  {
			  OnlinePropertyOrder po = ServiceFactory.OrderService.GetDIYOrder(1000293);

			  //Board
			  //CartItem item = new CartItem();
			  //item.ItemQty = 1;
			  //item.ProductId = 430;
			  //item.TypeId = 1;
			  //item.SelectedDIYTemplateId = 206;
			  //item.WebFriendlyName = "A P - Board_Photo";
			  //item.ProductName = "A P - Board_Photo";
			  //add this if want to keep existing template
			  //item.IsOldProduct = true;
			  //item.OrderDetailsID = 784685;

			  var itemr = (from cc in po.Cart
						  where cc.Id == 1
						  select cc).FirstOrDefault();
			  if (itemr != null)
			  {
				  po.Cart.Remove(itemr);
			  }

			  //extra Window Card product
			  CartItem itemWindowCard = new CartItem();
			  itemWindowCard.ItemQty = 1;
			  itemWindowCard.ProductId = 31;
			  itemWindowCard.TypeId = 17;
			  itemWindowCard.SelectedDIYTemplateId = 234; //TemplateProductID
			  itemWindowCard.WebFriendlyName = "A4 WC";
			  itemWindowCard.ProductName = "A4 WC";
			  

			  //po.Cart.Add(item);
			  //po.Cart.Add(item2);
			  po.Cart.Add(itemWindowCard);

			  ServiceFactory.OrderService.ModifyDIYOrder(1000293, 3728, po, "WORKS", true);
		  }

		  private void btnBPho_Click(object sender, EventArgs e)
		  {
			  OnlinePropertyOrder po = ServiceFactory.OrderService.GetDIYOrder(1000296);

			  //Board
			  //CartItem item = new CartItem();
			  //item.ItemQty = 1;
			  //item.ProductId = 430;
			  //item.TypeId = 1;
			  //item.SelectedDIYTemplateId = 206;
			  //item.WebFriendlyName = "A P - Board_Photo";
			  //item.ProductName = "A P - Board_Photo";
			  //add this if want to keep existing template
			  //item.IsOldProduct = true;
			  //item.OrderDetailsID = 784685;

			  var itemr = (from cc in po.Cart
						   where cc.Id == 1
						   select cc).FirstOrDefault();
			  if (itemr != null)
			  {
				  po.Cart.Remove(itemr);
			  }

			  //var item2 = (from cc in po.Cart
			  //            where cc.Id == 4
			  //            select cc).FirstOrDefault();
			  //if (item2 != null)
			  //{
			  //    po.Cart.Remove(item2);
			  //}

			  //extra Window Card product
			  CartItem itemWindowCard = new CartItem();
			  itemWindowCard.ItemQty = 1;
			  itemWindowCard.ProductId = 31;
			  itemWindowCard.TypeId = 17;
			  itemWindowCard.SelectedDIYTemplateId = 234; //TemplateProductID
			  itemWindowCard.WebFriendlyName = "A4 WC";
			  itemWindowCard.ProductName = "A4 WC";

			  //po.Cart.Add(item);
			  //po.Cart.Add(item2);
			  po.Cart.Add(itemWindowCard);

			  ServiceFactory.OrderService.ModifyDIYOrder(1000296, 3728, po, "WORKS", true);
		  }

		  private void btnBPhFl_Click(object sender, EventArgs e)
		  {
			  OnlinePropertyOrder po = ServiceFactory.OrderService.GetDIYOrder(1000303);

			  //Board
			  CartItem item = new CartItem();
			  item.ItemQty = 1;
			  item.ProductId = 430;
			  item.TypeId = 1;
			  item.SelectedDIYTemplateId = 206;
			  item.WebFriendlyName = "A P - Board_Photo";
			  item.ProductName = "A P - Board_Photo";
			 

			  var itemr = (from cc in po.Cart
						   where cc.Id == 1
						   select cc).FirstOrDefault();
			  if (itemr != null)
			  {
				  po.Cart.Remove(itemr);
			  }

			  //var item2 = (from cc in po.Cart
			  //            where cc.Id == 4
			  //            select cc).FirstOrDefault();
			  //if (item2 != null)
			  //{
			  //    po.Cart.Remove(item2);
			  //}

			  //extra Window Card product
			  CartItem itemWindowCard = new CartItem();
			  itemWindowCard.ItemQty = 1;
			  itemWindowCard.ProductId = 31;
			  itemWindowCard.TypeId = 17;
			  itemWindowCard.SelectedDIYTemplateId = 241; //TemplateProductID
			  itemWindowCard.WebFriendlyName = "A4 WC";
			  itemWindowCard.ProductName = "A4 WC";

			  po.Cart.Add(item);
			  //po.Cart.Add(item2);
			  po.Cart.Add(itemWindowCard);

			  ServiceFactory.OrderService.ModifyDIYOrder(1000303, 3728, po, "WORKS", true);
		  }

		  private void btnBBRPhFl_Click(object sender, EventArgs e)
		  {
			  OnlinePropertyOrder po = ServiceFactory.OrderService.GetDIYOrder(1000305);

			  //Board
			  CartItem item = new CartItem();
			  item.ItemQty = 1;
			  item.ProductId = 430;
			  item.TypeId = 1;
			  item.SelectedDIYTemplateId = 206;
			  item.WebFriendlyName = "A P - Board_Photo";
			  item.ProductName = "A P - Board_Photo";


			  var itemr = (from cc in po.Cart
						   where cc.Id == 2
						   select cc).FirstOrDefault();
			  if (itemr != null)
			  {
				  po.Cart.Remove(itemr);
			  }

			  //var item2 = (from cc in po.Cart
			  //            where cc.Id == 4
			  //            select cc).FirstOrDefault();
			  //if (item2 != null)
			  //{
			  //    po.Cart.Remove(item2);
			  //}

			  //extra Window Card product
			  CartItem itemWindowCard = new CartItem();
			  itemWindowCard.ItemQty = 1;
			  itemWindowCard.ProductId = 31;
			  itemWindowCard.TypeId = 17;
			  itemWindowCard.SelectedDIYTemplateId = 241; //TemplateProductID
			  itemWindowCard.WebFriendlyName = "A4 WC";
			  itemWindowCard.ProductName = "A4 WC";

			  po.Cart.Add(item);
			  //po.Cart.Add(item2);
			  po.Cart.Add(itemWindowCard);
			  ServiceFactory.OrderService.ModifyDIYOrder(1000305, 3728, po, "WORKS", true);
		  }

          private void button10_Click(object sender, EventArgs e)
          {
              DateTime start = DateTime.Now;

              var data = ServiceFactory.ProductService.GetAllOnlineProductsForExpressOrder(3728);

              int a = 0, b = 0, c = 0;

              foreach (var item in data.Products.Take(1))
              {
                  var ret = ServiceFactory.ProductService.PopulateConfigAndMatchingTemplates(3728, item);
                  if (ret.DIYTemplates.Count > 0)
                  {
                      item.DIYTemplates = ret.DIYTemplates;                      
                  }
                  item.XmlConfig = ret.XmlConfig;
                  if (ret.PackageGroups.Count > 0)
                  {
                      item.PackageGroups = ret.PackageGroups;
                  }
                  if (!string.IsNullOrEmpty(item.XmlConfig))
                      a++;
                  if (item.DIYTemplates.Count > 0)
                      b++;
                  if (item.PackageGroups.Count > 0)
                      c++;
              }

              DateTime end = DateTime.Now;

              richTextBox.Text = "GetAllOnlineProductsForExpressOrder:: Rec Found:" + data.Products.Count + ",  time took " + (end - start).TotalMilliseconds + " a:" + a + ", b:" + b + ", c:" + c;
          }

          private void button11_Click(object sender, EventArgs e)
          {
              DateTime start = DateTime.Now;

              var data = ServiceFactory.ProductService.GetOnlineProductsByCategoryId(3728, 3, null, false);

              DateTime end = DateTime.Now;

              richTextBox.Text = "GetOnlineProductsByCategoryId:: Rec Found:" + data.Count + ",  time took " + (end - start).TotalMilliseconds;
          }

          private void button12_Click(object sender, EventArgs e)
          {
              bool dnsError = false;

              WhereisApiClient client = new WhereisApiClient("4213272605751756800", "Password01");
              //WhereisApiClient client = new WhereisApiClient("6165231760435002368", "T_@B)_?_aU5");//old token
              string propertyAddress = string.Format("{0}, {1}, {2}, {3}", "120 Fairbairn road", "Sunshine West", "VIC", "AUS");

              Abc.OnlineBL.Utility.WhereIs.GeocodeResponse resu = client.getUnstructuredGeocode(propertyAddress, ref dnsError);


          }
    }
}
