using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.AIS.Entities.Model
{
    [DataContract]
	[Serializable]
    public class JobSearch
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
        /// Gets or sets the dateReceived.
        /// </summary>
        /// <value>The dateReceived.</value>
        public DateTime DateReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the clientID.
        /// </summary>
        /// <value>The clientID.</value>
        [DataMember]
        public int ClientID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the clientName.
        /// </summary>
        /// <value>The clientName.</value>
        public string ClientName
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
        /// Gets or sets the location1.
        /// </summary>
        /// <value>The location1.</value>
        [DataMember]
        public string Location1
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
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        [DataMember]
        public string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dateApproved.
        /// </summary>
        /// <value>The dateApproved.</value>
        [DataMember]
        public DateTime? DateApproved
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dateDespatched.
        /// </summary>
        /// <value>The dateDespatched.</value>
        [DataMember]
        public DateTime? DateDespatched
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inTransit.
        /// </summary>
        /// <value>The inTransit.</value>
        [DataMember]
        public bool InTransit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dateBoardErected.
        /// </summary>
        /// <value>The dateBoardErected.</value>
        [DataMember]
        public DateTime? DateBoardErected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dateRemovalRequested.
        /// </summary>
        /// <value>The dateRemovalRequested.</value>
        [DataMember]
        public DateTime? DateRemovalRequested
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dateBoardRemoved.
        /// </summary>
        /// <value>The dateBoardRemoved.</value>
        [DataMember]
        public DateTime? DateBoardRemoved
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the textReceived.
        /// </summary>
        /// <value>The textReceived.</value>
        [DataMember]
        public DateTime? TextReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the photosRecevied.
        /// </summary>
        /// <value>The photosRecevied.</value>
        [DataMember]
        public DateTime? PhotosRecevied
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the floorPlanReceived.
        /// </summary>
        /// <value>The floorPlanReceived.</value>
        [DataMember]
        public DateTime? FloorPlanReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the digitalPhotosReceived.
        /// </summary>
        /// <value>The digitalPhotosReceived.</value>
        [DataMember]
        public DateTime? DigitalPhotosReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the diskReceived.
        /// </summary>
        /// <value>The diskReceived.</value>
        [DataMember]
        public DateTime? DiskReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the artworkReceived.
        /// </summary>
        /// <value>The artworkReceived.</value>
        [DataMember]
        public DateTime? ArtworkReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the managerID.
        /// </summary>
        /// <value>The managerID.</value>
        [DataMember]
        public string ManagerID
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
        /// Gets or sets the urgent.
        /// </summary>
        /// <value>The urgent.</value>
        [DataMember]
        public bool Urgent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the onHold.
        /// </summary>
        /// <value>The onHold.</value>
        [DataMember]
        public DateTime? OnHold
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the erectionNotes.
        /// </summary>
        /// <value>The erectionNotes.</value>
        [DataMember]
        public string ErectionNotes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the clientsRefNo.
        /// </summary>
        /// <value>The clientsRefNo.</value>
        [DataMember]
        public string ClientsRefNo
        {
            get;
            set;
        }
    }
}
