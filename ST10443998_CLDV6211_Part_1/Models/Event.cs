namespace ST10443998_CLDV6211_POE.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Event
    {
        public int EventId { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Event Type")]
        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }

        [Display(Name = "Venue")]
        public int VenueId { get; set; }
        public Venue Venue { get; set; }
    }

}
