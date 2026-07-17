using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.WebClient
{
    [Serializable]
    [DataContract]
    public class JobSearch
    {
        [DataMember]
        public int OrderID
        {
            set;
            get;
        }

        [DataMember]
        public DateTime DateReceived
        {
            get;
            set;
        }

        [DataMember]
        public int ClientID
        {
            get;
            set;
        }

        [DataMember]
        public string ClientName
        {
            get;
            set;
        }

        [DataMember]
        public string PropertyAddress
        {
            get;
            set;
        }

        [DataMember]
        public string Location1
        {
            get;
            set;
        }

        [DataMember]
        public string State
        {
            get;
            set;
        }

        [DataMember]
        public string Caption
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? DateBoardErected
        {
            get;
            set;
        }

        [DataMember]
        public bool IsSpotorLightBoard
        {
            get;
            set;
        }

    }
}
