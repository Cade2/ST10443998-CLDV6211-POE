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
            _db.Events.Add(ev);
            _db.SaveChanges();

            return RedirectToAction("Payment", new { customerId = customerId, eventId = ev.EventId });
        }

        [HttpGet]
        public IActionResult Payment(int customerId, int eventId)
        {
            ViewBag.CustomerId = customerId;
            ViewBag.EventId = eventId;
            return View();
        }

        [HttpPost]
        public IActionResult Payment(Payment payment, int customerId, int eventId)
        {
            // Retrieve the Event
            var ev = _db.Events.FirstOrDefault(e => e.EventId == eventId);
            if (ev == null) return NotFound();

            // Create and save the Booking
            var booking = new Booking
            {
                BookingDate = DateTime.Now,
                CustomerId = customerId,
                Event = ev
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();

            // Link and save Payment
            payment.BookingId = booking.BookingId;
            _db.Payments.Add(payment);
            _db.SaveChanges();

            // ✅ Redirect to Booking Home Page
            return RedirectToAction("Index", "Booking");
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
                .Include(b => b.Customer)
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Include(b => b.Event.EventType)
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
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.BookingId == booking.BookingId);

            if (existingBooking == null)
                return NotFound();

            // Update Booking
            existingBooking.BookingDate = booking.BookingDate;

            // Update Customer
            existingBooking.Customer.FullName = Request.Form["Customer.FullName"];
            existingBooking.Customer.Email = Request.Form["Customer.Email"];
            existingBooking.Customer.PhoneNumber = Request.Form["Customer.PhoneNumber"];

            // Update Event
            existingBooking.Event.EventName = Request.Form["Event.EventName"];
            existingBooking.Event.Description = Request.Form["Event.Description"];
            existingBooking.Event.EventDate = DateTime.Parse(Request.Form["Event.EventDate"]);
            existingBooking.Event.EventTypeId = int.Parse(Request.Form["Event.EventTypeId"]);
            existingBooking.Event.VenueId = int.Parse(Request.Form["Event.VenueId"]);

            // Update Payment
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
