using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
    [Serializable]
    [DataContract]	
    public class BrochureOption
    {
        public BrochureOption()
		{

		}

        [DataMember]
        public bool IsOptionApplicable { get; set; }

        [DataMember]
        public string OptionName { get; set; }

        [DataMember]
        public int OptionQty { get; set; }
    }
}
