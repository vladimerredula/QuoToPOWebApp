using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QouToPOWebApp.Models;
using System.Diagnostics;

namespace QouToPOWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Clean()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return RedirectToAction(nameof(Index));
        }
    }
}
