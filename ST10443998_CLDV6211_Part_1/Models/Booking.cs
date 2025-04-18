﻿using Microsoft.Extensions.Logging;

namespace ST10443998_CLDV6211_POE.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Booking
    {
        public int BookingId { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        // Foreign Key
        public int CustomerId { get; set; }

        [Display(Name = "Customer")]
        public Customer? Customer { get; set; }

        // Owned/linked entities
        public Event? Event { get; set; }
        public int PaymentId { get; set; }  // ❌ <- This is likely the problem
        public Payment? Payment { get; set; }
    }

}
