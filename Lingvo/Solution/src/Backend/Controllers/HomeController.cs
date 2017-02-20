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

		/// <summary>
		/// Displays the internal error view.
		/// </summary>
		public IActionResult Error()
		{
			ViewData["Title"] = "Fehler";
			return View();
		}
	}
}
