namespace Abc.OnlineBL.Entities.Model.AbcVisual
{
    using System.Collections.Generic;

    public class VisualListingOption
    {
        public int RentalId { get; set; }
        public string FriendlyName { get; set; }
        public List<DisplayOption> DisplayOption { get; set; }
        public int ListingDisplayOptionId { get; set; }
    }   

    public class DisplayOption
    {
        public int DisplayOptionId { get; set; }
        public string Description { get; set; }
    }
}
