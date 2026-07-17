using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.Service.Implementation.Utility
{
    class RunsheetDetailSBAdapter
    {
        public static bool IsStockBoard(int orderId)
        {
            return (orderId >= 99000000) || (orderId >= 900000 && orderId <= 950000);
        }


        private RunsheetDetailsSB rsdsb;
        private RunSheetDetail rsd;

        public RunsheetDetailSBAdapter(RunsheetDetailsSB rsdsb)
        {
            this.rsdsb = rsdsb;
        }

        public RunsheetDetailSBAdapter(RunSheetDetail rsd)
	    {
            this.rsd = rsd;
	    }

        public bool IsReinstall
        {
            get
            {
                return (rsd == null) ? rsdsb.SB_Order.ReErectionRequested.HasValue : rsd.DespatchDetail.ReErectionRequested.HasValue;
            }
        }
    }
}
