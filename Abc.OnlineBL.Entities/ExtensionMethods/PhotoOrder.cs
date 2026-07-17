using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities
{
    public partial class PhotoOrder
    {
        private bool _NotifyAccounts;

        [DataMember]
        public bool NotifyAccounts
        {
            get { return _NotifyAccounts; }
            set
            {
                if (_NotifyAccounts != value)
                {
                    SendPropertyChanging();
                    _NotifyAccounts = value;
                    SendPropertyChanged("NotifyAccounts");                    
                }
            }
        }
    }
}
