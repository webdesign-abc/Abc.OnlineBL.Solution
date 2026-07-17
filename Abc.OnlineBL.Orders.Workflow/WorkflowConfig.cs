using System;

namespace Abc.OnlineBL.Orders.Workflow
{
    public class WorkflowConfig : Abc.OnlineBL.Utility.Configuration.BaseConfig
    {
        /// <summary>
        /// Gets the DUSK_OVERLAY_PRODUCT_ID
        /// </summary>
        /// <value>The DUSK_OVERLAY_PRODUCT_ID</value>
        public static int DUSK_OVERLAY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DUSK_OVERLAY_PRODUCT_ID"]); }
        }

        /// <summary>
        /// Gets the DUSK_OVERLAY_PRODUCT_PRICE
        /// </summary>
        /// <value>The DUSK_OVERLAY_PRODUCT_PRICE</value>
        public static decimal DUSK_OVERLAY_PRODUCT_PRICE
        {
            get { return Convert.ToDecimal(current["DUSK_OVERLAY_PRODUCT_PRICE"]); }
        }


        public static int UPGRADE_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["UPGRADE_PRODUCT_ID"]); }
        }

        public static int DRONE_PHOTOGRAPHY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_PRODUCT_ID"]); }
        }
        public static int DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_ID"]); }
        }
        public static string DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_NAME
        {
            get { return Convert.ToString(current["DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_NAME"]); }
        }
        public static decimal DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_PRICE
        {
            get { return Convert.ToDecimal(current["DRONE_PHOTOGRAPHY_1OVERLAY_PRODUCT_PRICE"]); }
        }
        public static int DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_ID"]); }
        }
        public static string DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_NAME
        {
            get { return Convert.ToString(current["DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_NAME"]); }
        }
        public static decimal DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_PRICE
        {
            get { return Convert.ToDecimal(current["DRONE_PHOTOGRAPHY_2OVERLAY_PRODUCT_PRICE"]); }
        }
        public static int DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_ID"]); }
        }
        public static string DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_NAME
        {
            get { return Convert.ToString(current["DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_NAME"]); }
        }
        public static decimal DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_PRICE
        {
            get { return Convert.ToDecimal(current["DRONE_PHOTOGRAPHY_3OVERLAY_PRODUCT_PRICE"]); }
        }
        public static int DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_ID"]); }
        }
        public static string DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_NAME
        {
            get { return Convert.ToString(current["DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_NAME"]); }
        }
        public static decimal DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_PRICE
        {
            get { return Convert.ToDecimal(current["DRONE_PHOTOGRAPHY_4OVERLAY_PRODUCT_PRICE"]); }
        }
        public static int DRONE_PHOTOGRAPHY_CHIEF_PILOT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_CHIEF_PILOT_ID"]); }
        }
        public static int DRONE_PHOTOGRAPHY_SITEPLAN_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_SITEPLAN_PRODUCT_ID"]); }
        }
        public static decimal DRONE_PHOTOGRAPHY_SITEPLAN_PRODUCT_PRICE
        {
            get { return Convert.ToDecimal(current["DRONE_PHOTOGRAPHY_SITEPLAN_PRODUCT_PRICE"]); }
        }

        public static string PHOTOGRAPHY_UPLOAD_FILE_PATH
        {
            get { return Convert.ToString(current["PHOTOGRAPHY_UPLOAD_FILE_PATH"]); }
        }
        public static int PLATINUM_PHOTOGRAPHER_ID
        {
            get { return Convert.ToInt32(current["PLATINUM_PHOTOGRAPHER_ID"]); }
        }

        public static string INSTRUCTION_UPLOAD_FILE_PATH
        {
            get { return Convert.ToString(current["INSTRUCTION_UPLOAD_FILE_PATH"]); }
        }

        public static int INSTALLATION_EXTENSION_STOCK_BOARD_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["INSTALLATION_EXTENSION_STOCK_BOARD_PRODUCT_ID"]); }
        }

        public static int INSTALLATION_EXTENSION_TEXT_PHOTO_BOARD_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["INSTALLATION_EXTENSION_TEXT_PHOTO_BOARD_PRODUCT_ID"]); }
        }

        public static int DELIVERY_ZONE_1_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DELIVERY_ZONE_1_PRODUCT_ID"]); }
        }

        public static int DELIVERY_ZONE_2_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DELIVERY_ZONE_2_PRODUCT_ID"]); }
        }
    }
}
