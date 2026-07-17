using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Utility.Configuration;

namespace Abc.OnlineBL
{
	/// <summary>
	/// Summary description for Config. This class is not in Abc.OnlineBL.Configuration namespace
	/// for convenience. We can use it any where without importing the name space.
	/// </summary>
	public class OnlineBLConfig : BaseConfig
	{		
		#region Public Settings

        /// <summary>
        /// Gets the SEND MAIL FROM.
        /// </summary>
        public static string SEND_MAIL_FROM
        {
            get
            {
                return current["SEND_MAIL_FROM"];
            }
        }
		/// <summary>
		/// Gets the Smtp Server.
		/// </summary>
		/// <value>The Smtp Server name or IP address.</value>
		public static string SMTP_SERVER
		{
			get { return current["SMTP_SERVER"]; }
		}

		/// <summary>
		/// Gets the email address to the the error message to.
		/// </summary>
		/// <value>The email address</value>
		public static string SEND_ERROR_MESSAGE_TO
		{
			get { return current["SEND_ERROR_MESSAGE_TO"]; }
		}

		/// <summary>
		/// Gets the database connection string.
		/// </summary>
		/// <value>The connection string.</value>
		public static string DB_CONN
		{
			get { return current["DB_CONN"]; }
		}

		/// <summary>
		/// Where will the Green Quality Photos go
		/// </summary>
		/// <value>Green Quality Photo Output Folder</value>
		public static string GREEN_PROPERTY_PHOTO_OUTPUT_FOLDER
		{
			get { return current["GREEN_PROPERTY_PHOTO_OUTPUT_FOLDER"]; }
		}
		/// <summary>
		/// Where will the Gray Quality Photos go
		/// </summary>
		/// <value>Gray Quality Photo Output Folder</value>
		public static string GRAY_PROPERTY_PHOTO_OUTPUT_FOLDER
		{
			get { return current["GRAY_PROPERTY_PHOTO_OUTPUT_FOLDER"]; }
		}
		/// <summary>
		/// Where will the RED Quality Photos go
		/// </summary>
		/// <value>RED Quality Photo Output Folder</value>
		public static string RED_PROPERTY_PHOTO_OUTPUT_FOLDER
		{
			get { return current["RED_PROPERTY_PHOTO_OUTPUT_FOLDER"]; }
		}
		/// <summary>
		/// Where will the GRAPHICS Quality Photos go
		/// </summary>
		/// <value>GRAPHICS Quality Photo Output Folder</value>
		public static string GRAPHICS_PROPERTY_PHOTO_OUTPUT_FOLDER
		{
			get { return current["GRAPHICS_PROPERTY_PHOTO_OUTPUT_FOLDER"]; }
		}
		/// <summary>
		/// Where will the IMAGING Quality Photos go
		/// </summary>
		/// <value>IMAGING Quality Photo Output Folder</value>
		public static string IMAGING_PROPERTY_PHOTO_OUTPUT_FOLDER
		{
			get { return current["IMAGING_PROPERTY_PHOTO_OUTPUT_FOLDER"]; }
		}

		/// <summary>
		/// Gets the OTHER s_ DOCUMEN t_ OUTPU t_ FOLDER.
		/// </summary>
		/// <value>The OTHER s_ DOCUMEN t_ OUTPU t_ FOLDER.</value>
		public static string OTHERS_DOCUMENT_OUTPUT_FOLDER
		{
			get { return current["OTHERS_DOCUMENT_OUTPUT_FOLDER"]; }
		}
		/// <summary>
		/// Gets the now.
		/// </summary>
		/// <value>The now.</value>
		public static DateTime Now
		{
			get
			{
				if (IS_NZ)
					return DateTime.Now.AddHours(2);
				else
					return DateTime.Now;
			}
		}

		/// <summary>
		/// Gets the Online Order Email Include
		/// </summary>
		/// <value>The ONLINE_ORDER_EMAIL_INCLUDE.</value>
		public static string ONLINE_ORDER_EMAIL_INCLUDE
		{
			get { return current["ONLINE_ORDER_EMAIL_INCLUDE"]; }
		}

		/// <summary>
		/// Gets the AOP_TEMPLATE_ROOT_DIR.
		/// </summary>
		/// <value>The AOP_TEMPLATE_ROOT_DIR.</value>
		public static string AOP_TEMPLATE_ROOT_DIR
		{
			get { return current["AOP_TEMPLATE_ROOT_DIR"]; }
		}

		/// <summary>
		/// Gets the SPOT_ORDER_FILE_DIR.
		/// </summary>
		/// <value>The SPOT_ORDER_FILE_DIR.</value>
		public static string SPOT_ORDER_FILE_DIR
		{
			get { return current["SPOT_ORDER_FILE_DIR"]; }
		}

		/// <summary>
		/// Gets the SMS_ON_DEMAND_PRODUCT_ID.
		/// </summary>
		/// <value>The SMS_ON_DEMAND_PRODUCT_ID.</value>
		public static int SMS_ON_DEMAND_PRODUCT_ID
		{
			get { return Convert.ToInt32(current["SMS_ON_DEMAND_PRODUCT_ID"]); }
		}

		/// <summary>
		/// Gets the MMS_ON_DEMAND_PRODUCT_ID.
		/// </summary>
		/// <value>The MMS_ON_DEMAND_PRODUCT_ID.</value>
		public static int MMS_ON_DEMAND_PRODUCT_ID
		{
			get { return Convert.ToInt32(current["MMS_ON_DEMAND_PRODUCT_ID"]); }
		}

		/// <summary>
		/// Gets the STOCK_BOARD_SMS_ON_DEMAND_PRODUCT_ID.
		/// </summary>
		/// <value>The STOCK_BOARD_SMS_ON_DEMAND_PRODUCT_ID.</value>
		public static int STOCK_BOARD_SMS_ON_DEMAND_PRODUCT_ID
		{
			get { return Convert.ToInt32(current["STOCK_BOARD_SMS_ON_DEMAND_PRODUCT_ID"]); }
		}

		/// <summary>
		/// Gets the STOCK_BOARD_MMS_ON_DEMAND_PRODUCT_ID.
		/// </summary>
		/// <value>The STOCK_BOARD_MMS_ON_DEMAND_PRODUCT_ID.</value>
		public static int STOCK_BOARD_MMS_ON_DEMAND_PRODUCT_ID
		{
			get { return Convert.ToInt32(current["STOCK_BOARD_MMS_ON_DEMAND_PRODUCT_ID"]); }
		}

		/// <summary>
		/// Gets the ERECTION_FEE_PRODUCT_ID.
		/// </summary>
		/// <value>The ERECTION_FEE_PRODUCT_ID.</value>
		public static int ERECTION_FEE_PRODUCT_ID
		{
			get { return Convert.ToInt32(current["ERECTION_FEE_PRODUCT_ID"]); }
		}

		/// <summary>
		/// Gets the ERECTIO n_ FE e_ AMOUNT.
		/// </summary>
		/// <value>The ERECTIO n_ FE e_ AMOUNT.</value>
		public static decimal ERECTION_FEE_AMOUNT
		{
			get { return Convert.ToDecimal(current["ERECTION_FEE_AMOUNT"]); }
		}
		/// <summary>
		/// Gets the IND d_ TEMPLATE s_ DIR.
		/// </summary>
		/// <value>The IND d_ TEMPLATE s_ DIR.</value>
		public static string INDD_TEMPLATES_DIR
		{
			get { return current["INDD_TEMPLATES_DIR"]; }
		}

		/// <summary>
		/// Gets the ORDE r_ FIL e_ DIR.
		/// </summary>
		/// <value>The ORDE r_ FIL e_ DIR.</value>
		public static string ORDER_FILE_DIR
		{
			get { return current["ORDER_FILE_DIR"]; }
		}

		/// <summary>
		/// Gets the UPLOA d_ TEX t_ DETAIL s_ DIR.
		/// </summary>
		/// <value>The UPLOA d_ TEX t_ DETAIL s_ DIR.</value>
		public static string UPLOAD_TEXT_DETAILS_DIR
		{
			get { return current["UPLOAD_TEXT_DETAILS_DIR"]; }
		}

		/// <summary>
		/// Gets the SEN d_ PHOT o_ ORDE r_ TO.
		/// </summary>
		/// <value>The SEN d_ PHOT o_ ORDE r_ TO.</value>
		public static string SEND_PHOTO_ORDER_TO
		{
			get { return current["SEND_PHOTO_ORDER_TO"]; }
		}

		/// <summary>
		/// Gets the SEN d_ STOCKBOAR d_ ORDE r_ TO.
		/// </summary>
		/// <value>The SEN d_ STOCKBOAR d_ ORDE r_ TO.</value>
		public static string SEND_STOCKBOARD_ORDER_TO
		{
			get { return current["SEND_STOCKBOARD_ORDER_TO"]; }
		}

		/// <summary>
		/// Gets the INSTALLATIO n_ FIL e_ PATH.
		/// </summary>
		/// <value>The INSTALLATIO n_ FIL e_ PATH.</value>
		public static string INSTALLATION_FILE_PATH
		{
			get { return current["INSTALLATION_FILE_PATH"]; }
		}

		/// <summary>
		/// Gets the ARTWOR k_ PD f_ UPLOA d_ FOLDER.
		/// </summary>
		/// <value>The ARTWOR k_ PD f_ UPLOA d_ FOLDER.</value>
		public static string ARTWORK_PDF_UPLOAD_FOLDER
		{
			get { return current["ARTWORK_PDF_UPLOAD_FOLDER"]; }
		}

		/// <summary>
		/// Gets the EMAI l_ NOTIFICATIO n_ FROM.
		/// </summary>
		/// <value>The EMAI l_ NOTIFICATIO n_ FROM.</value>
		public static string EMAIL_NOTIFICATION_FROM
		{
			get { return current["EMAIL_NOTIFICATION_FROM"]; }
		}

        /// <summary>
        /// Gets the SOLAR_PANEL_PRODUCT_ID
        /// </summary>
        /// <value>The SOLAR_PANEL_PRODUCT_ID</value>
        public static int SOLAR_PANEL_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["SOLAR_PANEL_PRODUCT_ID"]); }
        }
		#endregion
		
	}
}
