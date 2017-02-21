using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.Controllers
{
	/// <summary>
	/// The controller responsible for displaying generic information,
	/// such as the about page or error messages.
	/// </summary>
    public class HomeController : Controller
	{
		/// <summary>
		/// Displays the about view.
		/// </summary>
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
