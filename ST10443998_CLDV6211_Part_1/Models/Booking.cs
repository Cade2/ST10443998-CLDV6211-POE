using Microsoft.Extensions.Logging;

namespace ST10443998_CLDV6211_POE.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int VenueId { get; set; }  
        public Venue? Venue { get; set; }

        public Payment Payment { get; set; }
    }
}
