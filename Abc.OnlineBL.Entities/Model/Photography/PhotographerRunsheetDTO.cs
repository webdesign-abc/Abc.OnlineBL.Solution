using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.Photography
{
    [DataContract]
    [Serializable]
    public class PhotographerRunsheetDTO
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public List<PhotoOrderDTO> Orders { get; set; }

        [DataMember]
        public List<PhotoOrderDetailDTO> OrderDetails { get; set; }

        [DataMember]
        public List<DroneFlightRequestDTO> DroneFlightRequests { get; set; }

        [DataMember]
        public List<DronePilotDTO> DronePilots { get; set; }
    }
}
