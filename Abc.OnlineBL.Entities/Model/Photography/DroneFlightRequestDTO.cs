using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Abc.OnlineBL.Entities;

namespace Abc.OnlineBL.Entities.Model.Photography
{
    [DataContract]
    [Serializable]
    public class DroneFlightRequestDTO
    {
        public DroneFlightRequestDTO(DroneAuthorisation droneAuthorisation)
        {
            OrderID = droneAuthorisation.OrderID;
            Status = droneAuthorisation.Status;
            FlightScheduledOn = droneAuthorisation.FlightScheduledOn;
            FlightAddress = droneAuthorisation.FlightAddress;
            RPASSystem = droneAuthorisation.RPASSystem;
            RemotePilot = droneAuthorisation.RemotePilot;
            ChiefRemotePilot = droneAuthorisation.ChiefRemotePilot;
            Description = droneAuthorisation.Description;
            Observer = droneAuthorisation.Observer;
            LocalFrequencies = droneAuthorisation.LocalFrequencies;
            EmergencyContact = droneAuthorisation.EmergencyContact;
            Notes = droneAuthorisation.Notes;
            LastUpdatedOn = droneAuthorisation.LastUpdatedOn;
            LastUpdatedBy = droneAuthorisation.LastUpdatedBy;
        }

        [DataMember]
        public int OrderID { get; set; }

        [DataMember]
        public byte Status { get; set; }

        [DataMember]
        public DateTime? FlightScheduledOn { get; set; }

        [DataMember]
        public string FlightAddress { get; set; }

        [DataMember]
        public string RPASSystem { get; set; }

        [DataMember]
        public int RemotePilot { get; set; }

        [DataMember]
        public int ChiefRemotePilot { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Observer { get; set; }

        [DataMember]
        public string LocalFrequencies { get; set; }

        [DataMember]
        public string EmergencyContact { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public DateTime LastUpdatedOn { get; set; }

        [DataMember]
        public int LastUpdatedBy { get; set; }
    }
}
