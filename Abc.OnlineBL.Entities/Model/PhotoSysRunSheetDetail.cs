using System;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
    [DataContract]
    public class PhotoSysRunSheetDetail
    {
        /// <summary>
        /// Gets or sets the orderID.
        /// </summary>
        /// <value>The orderID.</value>
        [DataMember]
        public int OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the clientName.
        /// </summary>
        /// <value>The clientName.</value>
        [DataMember]
        public string ClientName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        /// <value>
        /// The client ID.
        /// </value>
        [DataMember]
        public int ClientID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the office.
        /// </summary>
        /// <value>The office.</value>
        [DataMember]
        public string Office
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        [DataMember]
        public string Phone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the propertyAddress.
        /// </summary>
        /// <value>The propertyAddress.</value>
        [DataMember]
        public string PropertyAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [DataMember]
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        [DataMember]
        public string State
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>The notes.</value>
        [DataMember]
        public string Notes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the photoContact.
        /// </summary>
        /// <value>The photoContact.</value>
        [DataMember]
        public string PhotoContact
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vendorName.
        /// </summary>
        /// <value>The vendorName.</value>
        [DataMember]
        public string VendorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vendorPhone.
        /// </summary>
        /// <value>The vendorPhone.</value>
        [DataMember]
        public string VendorPhone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the appointment.
        /// </summary>
        /// <value>The appointment.</value>
        [DataMember]
        public DateTime? Appointment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [DataMember]
        public string Items
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoSysRunSheet"/> class.
        /// </summary>
        public PhotoSysRunSheetDetail():base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoSysRunSheet"/> class.
        /// </summary>
        /// <param name="photoSysRunSheetObj">The photo sys run sheet obj.</param>
        public PhotoSysRunSheetDetail(PHOTOSYS_RunsheetResult photoSysRunSheetObj)
        {
            this.OrderID = photoSysRunSheetObj.OrderID;
            this.ClientName = photoSysRunSheetObj.ClientName;
            this.ClientID = photoSysRunSheetObj.ClientID;
            this.Office = photoSysRunSheetObj.Office;
            this.Phone = photoSysRunSheetObj.Phone;
            this.PropertyAddress = photoSysRunSheetObj.PropertyAddress;
            this.Location = photoSysRunSheetObj.Location;
            this.State = photoSysRunSheetObj.State;
            this.Notes = photoSysRunSheetObj.Notes;
            this.PhotoContact = photoSysRunSheetObj.PhotoContact;
            this.VendorName = photoSysRunSheetObj.VendorName;
            this.VendorPhone = photoSysRunSheetObj.VendorPhone;
            this.Appointment = photoSysRunSheetObj.Appointment;
            this.Items = photoSysRunSheetObj.Items;
        }
    }
}
