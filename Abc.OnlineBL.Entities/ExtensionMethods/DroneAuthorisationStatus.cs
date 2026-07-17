using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abc.OnlineBL.Entities.ExtensionMethods
{
    public static class DroneAuthorisationStatus
    {
        public static DroneAuthorisationStatusEnum GetStatus(this DroneAuthorisation droneAuth)
        {
            switch (droneAuth.Status)
            {
                case 0:
                    return DroneAuthorisationStatusEnum.Pending;
                case 1:
                    return DroneAuthorisationStatusEnum.Approved;
                case 2:
                    return DroneAuthorisationStatusEnum.Rejected;
                default:
                    throw new InvalidOperationException("Status may only be one of the following: {0: Pending, 1: Approved, 2: Rejected }");
            }
        }
    }

    public enum DroneAuthorisationStatusEnum
    {
        Pending,
        Approved,
        Rejected
    }

}
