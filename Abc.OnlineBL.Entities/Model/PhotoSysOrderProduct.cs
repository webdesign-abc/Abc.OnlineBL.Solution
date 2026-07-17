using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
    [DataContract]
    [Serializable]
    public class PhotoSysOrderProduct
    {
        [DataMember]
        public int OrderID { get; set; }
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public string ProductName { get; set; }
      
    }


}
