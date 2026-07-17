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
    public class DronePilotDTO
    {
        public DronePilotDTO(Photographer photographer)
        {
            Id = photographer.PgId;
            Name = photographer.FName + ' ' + photographer.LName;
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
