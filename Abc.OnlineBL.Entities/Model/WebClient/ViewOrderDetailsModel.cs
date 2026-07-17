using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Abc.OnlineBL.Entities.Model.WebClient
{
    [DataContract]
    [Serializable]
    public class ViewOrderDetailsModel
    {
        [DataMember]
        public int ClientID { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string Office { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public int PropertyID { get; set; }


        [DataMember]
        public bool IsRegularOrder { get; set; }

         [DataMember]
        public string Caption { get; set; }

         [DataMember]
         [Required]
         public string SendProofTo { get; set; }
         [DataMember]
         public string Location1 { get; set; }

        
        
        [DataMember]
        public int OrderID { get; set; }

        [DataMember]
        public string PropertyAddress { get; set; }

        [DataMember]
      
        public string ClientsRefNo { get; set; }

       

        [DataMember]
       
        public string RefNo { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        [Required]
        [DisplayName("Requested By")]
        public string RequestedBy { get; set; }

        [DataMember]
        [Required]
        [DisplayName("Contact Name")]
        public string ContactName { get; set; }

        [DataMember]
        [Required]
        [DisplayName("Contact No")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Wrong Contact No")]
        public string ContactNumber { get; set; }

        [DataMember]
        public string ExtraNotes { get; set; }
    }
}

