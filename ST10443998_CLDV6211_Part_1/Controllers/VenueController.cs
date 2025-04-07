using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Models;
using Microsoft.AspNetCore.Mvc;

namespace ST10443998_CLDV6211_POE.Controllers
{
    public class VenueController : Controller
    {
        private readonly AppDbContext _db;

        public VenueController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Venue> venues = _db.Venues.ToList();
            return View(venues);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Venue venue)
        {
                _db.Venues.Add(venue);
                _db.SaveChanges();
                return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var venue = _db.Venues.Find(id);
            return View(venue);
        }

        [HttpPost]
        public IActionResult Edit(Venue venue)
        {
                _db.Venues.Update(venue);
                _db.SaveChanges();
                return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var venue = _db.Venues.FirstOrDefault(v => v.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: /Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var venue = _db.Venues.FirstOrDefault(v => v.VenueId == id);
            if (venue == null) return NotFound();

            _db.Venues.Remove(venue);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var venue = _db.Venues.Find(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

    }
}
