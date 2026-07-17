using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Utility.Configuration;

namespace Abc.OnlineBL.Entities.Utility
{
    public class AbcConfig : BaseConfig
    {
        public static int DRONE_PHOTOGRAPHY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["DRONE_PHOTOGRAPHY_PRODUCT_ID"]); }
        }

        public static int SITEPLAN_OVERLAY_PHOTOGRAPHY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["SITEPLAN_OVERLAY_PHOTOGRAPHY_PRODUCT_ID"]); }
        }

        public static string[] SITEPLAN_PRODUCT_IDS
        {
            get
            {
                var ids = current["SITEPLAN_PRODUCT_IDS"];
                return ids.Split(',');
            }
        }

        public static string[] MUDMAP_REDRAW_PRODUCT_IDS
        {
            get
            {
                var ids = current["MUDMAP_REDRAW_PRODUCT_IDS"];
                return ids.Split(',');
            }
        }

        public static int AUGMENT_PHOTOGRAPHY_PRODUCT_ID
        {
            get { return Convert.ToInt32(current["AUGMENT_PHOTOGRAPHY_PRODUCT_ID"]); }
        }
    }
}
