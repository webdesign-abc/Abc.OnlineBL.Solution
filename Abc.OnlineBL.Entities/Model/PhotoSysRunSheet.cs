using System;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model
{
    [DataContract]
    public class PhotoSysRunSheet
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
		/// Gets or sets the completed.
		/// </summary>
		/// <value>
		/// The completed.
		/// </value>
		[DataMember]
		public DateTime? Completed
		{
			get;
			set;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoSysRunSheet"/> class.
        /// </summary>
        public PhotoSysRunSheet():base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoSysRunSheet"/> class.
        /// </summary>
        /// <param name="photoSysRunSheetObj">The photo sys run sheet obj.</param>
        public PhotoSysRunSheet(PHOTOSYS_RunsheetResult photoSysRunSheetObj)
        {
            this.OrderID = photoSysRunSheetObj.OrderID;
            this.ClientName = photoSysRunSheetObj.ClientName;
            this.Office = photoSysRunSheetObj.Office;
            this.PropertyAddress = photoSysRunSheetObj.PropertyAddress;
            this.Appointment = photoSysRunSheetObj.Appointment;
            this.Items = photoSysRunSheetObj.Items;
        }
        public PhotoSysRunSheet(PHOTOSYS_Runsheet2Result photoSysRunSheetObj)
        {
            this.OrderID = photoSysRunSheetObj.OrderID;
            this.ClientName = photoSysRunSheetObj.ClientName;
            this.Office = photoSysRunSheetObj.Office;
            this.PropertyAddress = photoSysRunSheetObj.PropertyAddress;
            this.Appointment = photoSysRunSheetObj.Appointment;
            this.Items = photoSysRunSheetObj.Items;
        }
        
    }
}
