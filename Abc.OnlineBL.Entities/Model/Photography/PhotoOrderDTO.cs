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
    public class PhotoOrderDTO
    {
        public PhotoOrderDTO(PhotoOrder photoOrder)
        {
            Id = photoOrder.OrderID;
            PhotographerId = photoOrder.PgId;
            DateReceived = photoOrder.Order.DateReceived;
            ClientName = photoOrder.Order.Client.ClientName;
            Office = photoOrder.Order.Client.Office;
            Phone = photoOrder.Order.Client.Phone;
            PropertyAddress = photoOrder.Order.PropertyAddress;
            Location = photoOrder.Order.Location.Location1;
            State = photoOrder.Order.Location.State;
            Notes = photoOrder.Notes;
            PhotoContact = photoOrder.PhotoContact;
            VendorName = photoOrder.VendorName;
            VendorPhone = photoOrder.VendorPhone;
            Appointment = photoOrder.Appointment;
            AppointmentDuration = photoOrder.AppointmentDuration;
            FolderNo = photoOrder.FolderNo;

            // Floorplans
            //HasFloorPlans = photoOrder.Order.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.FloorPlans);

            // Proposed behavior #1 - Ignore redraw products
            //HasFloorPlans = photoOrder.Order.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.FloorPlans
            //    && x.Product.Name.IndexOf("Re-draw", StringComparison.OrdinalIgnoreCase) == 0);

            // Proposed behavior #2 - Ignore products that are not photo item (isPhotoItem)
            HasFloorPlans = photoOrder.Order.OrderDetails.Any(x => x.Product.TypeID == ProductTypes.FloorPlans && x.Product.IsPhotoItem);

            FloorPlanSubmittedOn = photoOrder.Order.FloorPlanOrders.Min(x => x.DateFloorPlanUploadedFromPhotographer);

            FloorPlanNotes = photoOrder.Order.FloorPlanOrders.Aggregate(string.Empty, (x, y) => x += string.IsNullOrEmpty(x) ? y.Notes : "; " + y.Notes);

            // Drone photography
            HasDronePhotos = photoOrder.Order.OrderDetails.Any(x => x.Product.Name.IndexOf("Drone Photography", StringComparison.OrdinalIgnoreCase) >= 0);

            //Details
            HasNightPhotos = photoOrder.Order.OrderDetails.Any(x => {
                return x.Product.Name.IndexOf("night", StringComparison.OrdinalIgnoreCase) >= 0
                    || x.Product.Name.IndexOf("+n", StringComparison.OrdinalIgnoreCase) >= 0
                    || x.Product.Name.IndexOf("twilight", StringComparison.OrdinalIgnoreCase) >= 0;
            });

            HasPlatinumProducts = photoOrder.Order.OrderDetails.Any(x => x.Product.Name.IndexOf("Platinum", StringComparison.OrdinalIgnoreCase) >= 0);

            IsKeySafe = photoOrder.IsKeySafe;
            IsPickupKeys = photoOrder.IsPickupKeys;
            ClientID = photoOrder.Order.ClientID;
            LocationID = photoOrder.Order.LocationID;
            HouseFaces = photoOrder.HouseFaces;
            Melway = photoOrder.Melway;
            Completed = photoOrder.Completed;
            NightWeekendRequest = photoOrder.NightWeekendRequest;
            OrderStatus = photoOrder.Order.OrderStatus;
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int PhotographerId { get; set; }

        [DataMember]
        public DateTime DateReceived { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string Office { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string PropertyAddress { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public bool IsNightPhoto { get; set; }

        [DataMember]
        public DateTime? NightJobCompleted { get; set; }

        [DataMember]
        public string PhotoContact { get; set; }

        [DataMember]
        public string VendorName { get; set; }

        [DataMember]
        public string VendorPhone { get; set; }

        [DataMember]
        public DateTime? Appointment { get; set; }

        [DataMember]
        public string FolderNo { get; set; }

        // Floorplans

        [DataMember]
        public bool HasFloorPlans { get; set; }

        [DataMember]
        public DateTime? FloorPlanSubmittedOn { get; set; }

        [DataMember]
        public string FloorPlanNotes { get; set; }  
        
        // Drone photography

        [DataMember]
        public bool HasDronePhotos { get; set; }

        // Details

        [DataMember]
        public bool HasNightPhotos { get; set; }

        [DataMember]
        public bool HasPlatinumProducts { get; set; }
            
        // Other

        [DataMember]
        public bool? IsKeySafe { get; set; }

        [DataMember]
        public bool? IsPickupKeys { get; set; }

        [DataMember]
        public int ClientID { get; set; }

        [DataMember]
        public int LocationID { get; set; }

        [DataMember]
        public string HouseFaces { get; set; }

        [DataMember]
        public string Melway { get; set; }

        [DataMember]
        public DateTime? Completed { get; set; }

        [DataMember]
        public DateTime? NightWeekendRequest { get; set; }

        [DataMember]
        public bool NotifyAccounts { get; set; }

        [DataMember]
        public int? AppointmentDuration { get; set; }
        
        [DataMember]
        public int? OrderStatus { get; set; }
    }
}
