using System;
using System.Runtime.Serialization;
using System.Text;

namespace Abc.AIS.Entities.Model.OnlineOrder
{
    [DataContract]
    [Serializable]
    public class SmsOrderDetail
    {
        /// <summary>
        /// Gets or sets the order ID.
        /// </summary>
        /// <value>
        /// The order ID.
        /// </value>
        [DataMember]
        public int OrderID
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the property ID.
        /// </summary>
        /// <value>
        /// The property ID.
        /// </value>
        [DataMember]
        public int PropertyID
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the ClientID
        /// </summary>
        /// <value>The ClientID.</value>
        [DataMember]
        public int ClientID
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the UnitNo
        /// </summary>
        /// <value>The UnitNo.</value>
        [DataMember]
        public string UnitNo
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the StreetNo
        /// </summary>
        /// <value>The StreetNo.</value>
        [DataMember]
        public string StreetNo
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the StreetName
        /// </summary>
        /// <value>The StreetName.</value>
        [DataMember]
        public string StreetName
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the Location
        /// </summary>
        /// <value>The Location.</value>
        [DataMember]
        public string Location
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the PropertyAddress
        /// </summary>
        /// <value>
        /// The PropertyAddress.
        /// </value>
        public string PropertyAddress
        {
            get
            {
                StringBuilder propertyAddressBuilder = new StringBuilder();

                if (UnitNo != null && UnitNo != string.Empty)
                {
                    propertyAddressBuilder.AppendFormat("{0}/", UnitNo);
                }

                if (StreetNo != null && StreetNo != string.Empty)
                {
                    propertyAddressBuilder.Append(StreetNo);
                }

                propertyAddressBuilder.AppendFormat(" {0},{1}", StreetName, Location);

                return propertyAddressBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [SM s_ send email].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [SM s_ send email]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool? SMS_SendEmail
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the SMS_AgentEmailAddress
        /// </summary>
        /// <value>The SMS_AgentEmailAddress.</value>
        [DataMember]
        public string SMS_AgentEmailAddress
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [SM s_ notify agent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [SM s_ active]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool? SMS_NotifyAgent
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the NotificationMethod
        /// </summary>
        /// <value>The NotificationMethod.</value>
        public string NotificationMethod
        {
            get
            {
                string notificationmethod = string.Empty;

                if (SMS_NotifyAgent.HasValue)
                {
                    if (SMS_NotifyAgent == true)
                    {
                        notificationmethod = SMS_SendEmail == true ? string.Format("Yes, via email to {0}", SMS_AgentEmailAddress) : "Yes, send an SMS";
                    }

                    else
                    {
                        notificationmethod = "No";
                    }
                }

                return notificationmethod;
            }
        }
       
        /// <summary>
        /// Gets or sets the SMS_UserText
        /// </summary>
        /// <value>The SMS_UserText.</value>
        [DataMember]
        public string SMS_UserText
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the SMS_AgentMobileNo
        /// </summary>
        /// <value>The SMS_AgentMobileNo.</value>
        [DataMember]
        public string SMS_AgentMobileNo
        {
            set;
            get;
        }

        ///// <summary>
        ///// Gets or sets the Sender
        ///// </summary>
        ///// <value>The Sender.</value>
        //[DataMember]
        //public string Sender
        //{
        //    set;
        //    get;
        //}

        /// <summary>
        /// Gets or sets the RcvdOn
        /// </summary>
        /// <value>
        /// The RCVD on.
        /// </value>
        [DataMember]
        public DateTime? RcvdOn
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the MMS_Allowed
        /// </summary>
        /// <value>
        ///   <c>true</c> if [MM s_ allowed]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool MMS_Allowed
        {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets the MMS_Photo
        /// </summary>
        /// <value>
        /// The MMS_Photo.
        /// </value>
        [DataMember]
        public byte[] MMS_Photo
        {
            set;
            get;
        }
        
        /// <summary>
        /// Gets or sets the MMS_Photo_Uploaded
        /// </summary>
        /// <value>
        ///   <c>true</c> if [MM s_ photo_ uploaded]; otherwise, <c>false</c>.
        /// </value>
        public bool MMS_Photo_Uploaded
        {
            get
            {
                return MMS_Photo == null ? false : true;
            }
        }
        
        /// <summary>
        /// Gets or sets the SMS_Active
        /// </summary>
        /// <value>
        ///   <c>true</c> if [SM s_ active]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool SMS_Active
        {
            set;
            get;
        }
    }
}
