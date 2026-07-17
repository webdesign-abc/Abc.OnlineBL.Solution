using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.Photography
{
    [DataContract]
    [Serializable]
    public class PhotographerName
    {
        [DataMember]
        public int PhotographerID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

}
