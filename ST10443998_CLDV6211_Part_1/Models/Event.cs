namespace ST10443998_CLDV6211_POE.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }

        public int VenueId { get; set; }
        public Venue Venue { get; set; }

        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
