using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseBookingSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _db;

        public EventsController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var events = _db.Events.ToList();
            return View(events);
        }



        public IActionResult Create()
        {
            return RedirectToAction("Create", "Booking");
        }

        public IActionResult Edit(int id)
        {
            return RedirectToAction("Edit", "Booking", new { id = id });
        }

        public IActionResult Delete(int id)
        {
            return RedirectToAction("Delete", "Booking", new { id = id });
        }
    }
}
