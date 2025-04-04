using Microsoft.Extensions.Logging;

namespace ST10443998_CLDV6211_POE.Models
{
    public class Venue
    {
        public int? VenueId { get; set; }
        public string? VenueName { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
