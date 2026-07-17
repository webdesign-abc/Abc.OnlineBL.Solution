using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Utility.WhereIs
{
    public class GeocodeResponse
    {
		public List<Results> results { get; set; }

		public Pagination pagination { get; set; }
    }
}
