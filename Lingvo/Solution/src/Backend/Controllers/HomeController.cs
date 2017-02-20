using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
    public class HomeController : Controller
	{
		public IActionResult About()
		{
			ViewData["Title"] = "Impressum";
			return View();
		}

		public IActionResult Error()
		{
			ViewData["Title"] = "Fehler";
			return View();
		}
	}
}
