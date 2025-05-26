using CreepStat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CreepStat.Controllers
{
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

        public IActionResult Matches()
        {
            return View();
        }

        public IActionResult Heroes()
        {
            return View();
        }

        public IActionResult Teams()
        {
            return View();
        }

        public IActionResult Distributions()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}