using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ST10443998_CLDV6211_POE.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _db;

        public BookingController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var bookings = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .ToList();

            return View(bookings);
        }


        [HttpGet]
        public IActionResult Customer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Customer(Customer customer)
        {
            _db.Customers.Add(customer);
            _db.SaveChanges();

            return RedirectToAction("Event", new { customerId = customer.CustomerId });
        }

        [HttpGet]
        public IActionResult Event(int customerId)
        {
            ViewBag.CustomerId = customerId;
            ViewBag.Venues = _db.Venues.ToList();
            ViewBag.EventTypes = _db.EventTypes.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Event(Event ev, int customerId)
        {
            var booking = new Booking
            {
                CustomerId = customerId,
                BookingDate = DateTime.Now,
                Event = ev
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();

            // Pass bookingId to Payment view
            return RedirectToAction("Payment", new { bookingId = booking.BookingId });
        }


        [HttpGet]
        public IActionResult Payment(int bookingId)
        {
            ViewBag.BookingId = bookingId;
            return View();
        }

        [HttpPost]
        public IActionResult Payment(Payment payment, int bookingId)
        {
            Console.WriteLine("✅ POST Payment reached");

            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null) return NotFound();

            booking.Payment = payment;
            _db.Payments.Add(payment);
            _db.SaveChanges();

            return RedirectToAction("Confirmation");
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Event).ThenInclude(e => e.Venue)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null) return NotFound();
            return View(booking);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            ViewBag.Venues = _db.Venues.ToList();
            ViewBag.EventTypes = _db.EventTypes.ToList();

            return View(booking);
        }


        [HttpPost]
        public IActionResult Edit(Booking booking)
        {
            var existingBooking = _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == booking.BookingId);

            if (existingBooking == null)
                return NotFound();

            // Update Booking date
            existingBooking.BookingDate = booking.BookingDate;

            // Update Event details
            existingBooking.Event.EventName = Request.Form["Event.EventName"];
            existingBooking.Event.EventDate = DateTime.Parse(Request.Form["Event.EventDate"]);
            existingBooking.Event.Description = Request.Form["Event.Description"];
            existingBooking.Event.EventTypeId = int.Parse(Request.Form["Event.EventTypeId"]);
            existingBooking.Event.VenueId = int.Parse(Request.Form["Event.VenueId"]);

            // Update Payment details
            existingBooking.Payment.Amount = decimal.Parse(Request.Form["Payment.Amount"]);
            existingBooking.Payment.PaymentDate = DateTime.Parse(Request.Form["Payment.PaymentDate"]);

            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Include(b => b.Event.EventType)
                .Include(b => b.Payment)
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            // Delete the associated event
            if (booking.Event != null)
                _db.Events.Remove(booking.Event);

            // Delete the associated payment
            if (booking.Payment != null)
                _db.Payments.Remove(booking.Payment);

            // Delete the associated customer
            if (booking.Customer != null)
                _db.Customers.Remove(booking.Customer);

            // Delete the booking itself
            _db.Bookings.Remove(booking);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

    }
}
