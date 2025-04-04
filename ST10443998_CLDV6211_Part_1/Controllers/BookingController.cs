using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using ST10443998_CLDV6211_POE.Models;
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
                .Include(b => b.Venue)
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

            // Redirect to payment step, passing both customerId and eventId
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
            // Get Event to retrieve VenueId
            var ev = _db.Events.FirstOrDefault(e => e.EventId == eventId);
            if (ev == null) return NotFound();

            var booking = new Booking
            {
                CustomerId = customerId,
                EventId = eventId,
                VenueId = ev.VenueId,
                BookingDate = DateTime.Now
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges(); 

            payment.BookingId = booking.BookingId;

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
                .Include(b => b.Venue)
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
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .Include(b => b.Payment) 
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null) return NotFound();

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

            if (existingBooking == null) return NotFound();

            // Update Booking info
            existingBooking.BookingDate = booking.BookingDate;
            existingBooking.VenueId = booking.VenueId;

            // Update Customer info
            existingBooking.Customer.FullName = Request.Form["Customer.FullName"];
            existingBooking.Customer.Email = Request.Form["Customer.Email"];
            existingBooking.Customer.PhoneNumber = Request.Form["Customer.PhoneNumber"];

            // Update Event info
            existingBooking.Event.EventName = Request.Form["Event.EventName"];
            existingBooking.Event.EventDate = DateTime.Parse(Request.Form["Event.EventDate"]);
            existingBooking.Event.Description = Request.Form["Event.Description"];
            existingBooking.Event.EventTypeId = int.Parse(Request.Form["Event.EventTypeId"]);

            // Update Payment info
            existingBooking.Payment.Amount = decimal.Parse(Request.Form["Payment.Amount"]);
            existingBooking.Payment.PaymentDate = DateTime.Parse(Request.Form["Payment.PaymentDate"]);

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _db.Bookings.FirstOrDefault(b => b.BookingId == id);
            if (booking == null) return NotFound();

            // Get IDs for related entities
            int customerId = booking.CustomerId;
            int eventId = booking.EventId;

            // Find and delete the Payment based on BookingId
            var payment = _db.Payments.FirstOrDefault(p => p.BookingId == booking.BookingId);
            if (payment != null)
                _db.Payments.Remove(payment);

            // Delete the Event
            var ev = _db.Events.FirstOrDefault(e => e.EventId == eventId);
            if (ev != null)
                _db.Events.Remove(ev);

            // Delete the Customer
            var customer = _db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
                _db.Customers.Remove(customer);

            // Delete the Booking
            _db.Bookings.Remove(booking);

            _db.SaveChanges();

            return RedirectToAction("Index");
        }



        public IActionResult Confirmation()
        {
            return View();
        }

    }
}
