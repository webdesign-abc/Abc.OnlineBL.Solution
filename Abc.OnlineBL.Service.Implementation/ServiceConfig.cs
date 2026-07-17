using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Service.Implementation
{
	public class ServiceConfig : Abc.OnlineBL.Utility.Configuration.BaseConfig
	{
		public static string AOP_TEMPLATE_ROOT_DIR
		{
			get { return current["AOP_TEMPLATE_ROOT_DIR"]; }
		}

		public static string AOP_IMAGE_OUTPUT_DIR_STRUCTURE
		{
			get { return current["AOP_IMAGE_OUTPUT_DIR_STRUCTURE"]; }
		}

		public static string AOP_IMAGE_OUTPUT_DIR
		{
			get { return current["AOP_IMAGE_OUTPUT_DIR"]; }
		}

		public static string XML_XPATH_MAPPING_FILE
		{
			get { return current["XML_XPATH_MAPPING_FILE"]; }
		}

		public static bool XML_ORDER_DIR_IS_STRUCTURED
		{
			get { return Convert.ToBoolean(current["XML_ORDER_DIR_IS_STRUCTURED"]); }
		}

		public static string XML_ORDER_DIR
		{
			get { return current["XML_ORDER_DIR"]; }
		}

		public static string AOP_DOC_OUTPUT_DIR_STRUCTURE
		{
			get { return current["AOP_DOC_OUTPUT_DIR_STRUCTURE"]; }
		}

		public static string AOP_DOC_OUTPUT_DIR
		{
			get { return current["AOP_DOC_OUTPUT_DIR"]; }
		}

		public static string APPROVED_DOC_DIR
		{
			get { return current["APPROVED_DOC_DIR"]; }
		}

		public static string APPROVED_DOC_DIR_STRUCTURE
		{
			get { return current["APPROVED_DOC_DIR_STRUCTURE"]; }
		}

		public static string PROOF_DIR
		{
			get { return current["PROOF_DIR"]; }
		}

		public static string PROOF_DIR_STRUCTURE
		{
			get { return current["PROOF_DIR_STRUCTURE"]; }
		}

		public static string ENCRYPT_KEY
		{
			get { return "longhorn"; }
		}

		public static string PROOF_WEB_SITE_URL
		{
			get { return current["PROOF_WEB_SITE_URL"]; }
		}

		public static string GREEN_PROPERTY_PHOTO_OUTPUT_MESSAGE
		{
			get { return current["GREEN_PROPERTY_PHOTO_OUTPUT_MESSAGE"]; }
		}

		public static string GRAY_PROPERTY_PHOTO_OUTPUT_MESSAGE
		{
			get { return current["GRAY_PROPERTY_PHOTO_OUTPUT_MESSAGE"]; }
		}

		public static string RED_PROPERTY_PHOTO_OUTPUT_MESSAGE
		{
			get { return current["RED_PROPERTY_PHOTO_OUTPUT_MESSAGE"]; }
		}

		public static string PRODUCT_FILES_DIR
		{
			get { return current["PRODUCT_FILES_DIR"]; }
		}

		public static string PRODUCT_FILES_DIR_TEMP
		{
			get { return current["PRODUCT_FILES_DIR_TEMP"]; }
		}

		public static string PRODUCT_GROUP_FILES_DIR
		{
			get { return current["PRODUCT_GROUP_FILES_DIR"]; }
		}

		public static string PRODUCT_GROUP_FILES_DIR_TEMP
		{
			get { return current["PRODUCT_GROUP_FILES_DIR_TEMP"]; }
		}

		public static string AOP_WORKING_DIR
		{
			get { return current["AOP_WORKING_DIR"]; }
		}

		public static string PHOTO_DIR
		{
			get { return current["PHOTO_DIR"]; }
		}

		public static string PICKING_SLIP_SORT_DIR
		{
			get { return current["PICKING_SLIP_SORT_DIR"]; }
		}

		public static double GST
		{
			get { return Convert.ToDouble(current["GST"]); }
		}

		public static string AOP_WORKING_DIR_MODIFY_ORDER
		{
			get { return current["AOP_WORKING_DIR_MODIFY_ORDER"]; }
		}

		public static string AOP_WORKING_DIR_MODIFY_ORDER_COMPLETED
		{
			get { return current["AOP_WORKING_DIR_MODIFY_ORDER_COMPLETED"]; }
		}

        public static int SOLAR_PANEL_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["SOLAR_PANEL_PRODUCT_ID"]); }
        }

        public static int DRONE_PHOTOGRAPHY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_PRODUCT_ID"]); }
        }

        public static int DRONE_PHOTOGRAPHY_CHIEF_PILOT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_CHIEF_PILOT_ID"]); }
        }

        public static string[] NAMEPLATES_PRODUCT_IDS
        {
            get
            {
                var ids = current["NAMEPLATES_PRODUCT_IDS"];
                return ids.Split(',');
            }
        }

        public static string[] UNITSTICKER_PRODUCT_IDS
        {
            get
            {
                var ids = current["UNITSTICKER_PRODUCT_IDS"];
                return ids.Split(',');
            }
        }

        public static string[] OVERLAY_POINTS_PRODUCT_IDS
        {
            get
            {
                var ids = current["OVERLAY_POINTS_PRODUCT_IDS"];
                return ids.Split(',');
            }
        }

        public static string[] OVERLAY_THREE_ICONS_PRODUCT_IDS
        {
            get
            {
                var ids = current["OVERLAY_THREE_ICONS_PRODUCT_IDS"];
                return ids.Split(',');
            }
        }

        public static string[] OVERLAY_INFO_PRODUCT_IDS
        {
            get
            {
                var ids = current["OVERLAY_INFO_PRODUCT_IDS"];
                return ids.Split(',');
            }
        }

        public static string[] OVERLAY_PROPERTY_ADDRESS_IDS
        {
            get
            {
                var ids = current["OVERLAY_PROPERTY_ADDRESS_IDS"];
                return ids.Split(',');
            }
        }

        public static string PROOF_IMAGE_DIR
        {
            get { return current["PROOF_IMAGE_DIR"]; }
        }

        public static int INSTALLATION_EXTENSION_STOCK_BOARD_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["INSTALLATION_EXTENSION_STOCK_BOARD_PRODUCT_ID"]); }
        }

        public static int INSTALLATION_EXTENSION_TEXT_PHOTO_BOARD_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["INSTALLATION_EXTENSION_TEXT_PHOTO_BOARD_PRODUCT_ID"]); }
        }

        public static int VIC_STANDARD_PRODUCT_PRICING_ID
        {
            get { return Convert.ToInt32(current["VIC_STANDARD_PRODUCT_PRICING_ID"]); }
        }

        public static string ABC_LISTING_DIR
        {
            get { return current["ABC_LISTING_DIR"]; }
        }

        public static string DESIGN_TEMPLATE_DOCUMENT_DIR
        {
            get { return current["DESIGN_TEMPLATE_DOCUMENT_DIR"]; }
        }

        public static int VIRTUAL_WALK_THROUGH_RENEWAL_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["VIRTUAL_WALK_THROUGH_RENEWAL_PRODUCT_ID"]); }
        }
    }
}
