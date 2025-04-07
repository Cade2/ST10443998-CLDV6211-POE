using Microsoft.AspNetCore.Mvc;
using ST10443998_CLDV6211_POE.Data;
using ST10443998_CLDV6211_POE.Controllers;

namespace ST10443998_CLDV6211_POE.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _db;

        public PaymentsController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var payments = _db.Payments.ToList();
            return View(payments);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Customer", "Booking");
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
