using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseBookingSystem.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _db;

        public EventController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var events = _db.Events.Include(e => e.Venue).Include(e => e.EventType).ToList();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Venues = _db.Venues.ToList();
            ViewBag.EventTypes = _db.EventTypes.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Event events)
        {
            _db.Events.Add(events);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var events = _db.Events.Find(id);
            if (events == null) return NotFound();

            ViewBag.Venues = _db.Venues.ToList();
            ViewBag.EventTypes = _db.EventTypes.ToList();
            return View(events);
        }

        [HttpPost]
        public IActionResult Edit(Event events)
        {
                _db.Events.Update(events);
                _db.SaveChanges();
                return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var events = _db.Events.Include(e => e.Venue).Include(e => e.EventType).FirstOrDefault(e => e.EventId == id);
            if (events == null) return NotFound();
            return View(events);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int id)
        {
            var events = _db.Events.Find(id);
            if (events == null) return NotFound();

            _db.Events.Remove(events);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var events = _db.Events
                        .Include(e => e.Venue)
                        .Include(e => e.EventType)
                        .FirstOrDefault(e => e.EventId == id);
            if (events == null) return NotFound();

            return View(events);
        }
    }
}
