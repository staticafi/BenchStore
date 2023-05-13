using System.Diagnostics;

using BenchStoreMVC.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace BenchStoreMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/About
        public IActionResult About()
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
