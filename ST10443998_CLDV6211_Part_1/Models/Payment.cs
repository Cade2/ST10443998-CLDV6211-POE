namespace ST10443998_CLDV6211_POE.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}
