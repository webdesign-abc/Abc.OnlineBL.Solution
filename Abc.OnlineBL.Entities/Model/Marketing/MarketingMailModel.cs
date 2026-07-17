using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Entities.Model.Marketing
{
	public class MarketingMailModel
    {
		public int ClientID { get; set; }
        public string OfficeLocation { get; set; }
        public string OfficeAddress { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPosition { get; set; }
        public string BoardSupplier { get; set; }
        public bool PhotoTextBoards { get; set; }
        public bool StockBoards { get; set; }
        public string PhotographySupplier { get; set; }
        public string PrintingSupplier { get; set; }
		public bool OrderedBoards { get; set; }
		public bool OrderedPhotography { get; set; }
		public bool OrderedPrinting { get; set; }
		public bool OrderedBrochures { get; set; }
		public bool OrderedWindowCards { get; set; }

    }
}
