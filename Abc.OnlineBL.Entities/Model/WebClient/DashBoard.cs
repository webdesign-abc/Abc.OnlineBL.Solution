using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.WebClient
{
	[Serializable]
	[DataContract]
	public class DashBoard
	{
        public DashBoard()
        {
            CurrentJobs = new List<DashboardJob>();
            AwaitingRemovalJobs = new List<DashboardJob>();
        }

		[DataMember]
		public int TotalCurrentJobs { get; set; }

		[DataMember]
		public int TotalDIYJobToComplete { get; set; }

		[DataMember]
		public int TotalAwaitingInstallOrders { get; set; }

		[DataMember]
		public int TotalAwaitingRemoveOrders { get; set; }

        [DataMember]
        public List<DashboardJob> CurrentJobs { get; set; }

        [DataMember]
        public List<DashboardJob> AwaitingRemovalJobs { get; set; }
	}

    [Serializable]
    [DataContract]
    public class DashboardJob
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public int PropertyId { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public DateTime? BoardErected { get; set; }

        [DataMember]
        public bool HasDiy { get; set; }

        [DataMember]
        public bool HasProof { get; set; }
    }
}
