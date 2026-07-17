using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
	[Serializable]
	[DataContract]
	public class SmsOrder
	{
		private string smsText;
		private string agentMobileNo;
		private bool notifyAgent;
		private bool sendEmail;
		private string agentEmailAddress;
		private bool allowMMS;

		[DataMember]
		public string SmsText
		{
			get { return smsText; }
			set { smsText = value; }
		}

		[DataMember]
		public string AgentMobileNo
		{
			get { return agentMobileNo; }
			set { agentMobileNo = value; }
		}

		[DataMember]
		public bool NotifyAgent
		{
			get { return notifyAgent; }
			set { notifyAgent = value; }
		}

		[DataMember]
		public bool SendEmail
		{
			get { return sendEmail; }
			set { sendEmail = value; }
		}

		[DataMember]
		public string AgentEmailAddress
		{
			get { return agentEmailAddress; }
			set { agentEmailAddress = value; }
		}

		[DataMember]
		public bool AllowMMS
		{
			get { return allowMMS; }
			set { allowMMS = value; }
		}
	}
}
