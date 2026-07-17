using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
    [DataContract]
    public class DriverReport
    {
        [DataMember]
        public string AppID
        {
            get;
            set;
        }
        [DataMember]
        public int TruckID
        {
            get;
            set;
        }
        [DataMember]
        public string UserID
        {
            get;
            set;
        }
        [DataMember]
        public string VehicleNumber
        {
            get;
            set;
        }

        [DataMember]
        public string Mobile
        {
            get;
            set;
        }
        [DataMember]
        public string Notes
        {
            get;
            set;
        }
        [DataMember]
        public int StartMileage
        {
            get;
            set;
        }
        [DataMember]
        public DateTime StartTime
        {
            get;
            set;
        }
        [DataMember]
        public int FinishMileage
        {
            get;
            set;
        }
        [DataMember]
        public DateTime FinishTime
        {
            get;
            set;
        }

       [DataMember]
        public string IMEI
        {
            get;
            set;
        }

    }
}

