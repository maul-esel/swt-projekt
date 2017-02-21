using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.ViewComponents
{
	/// <summary>
	/// A View Component encapsulating the user menu containing options such
	/// as logout or creating a new user.
	/// </summary>
    public class AccountMenuViewComponent : ViewComponent
    {
		private readonly SignInManager<Editor> _signInManager;

		public AccountMenuViewComponent(SignInManager<Editor> signInManager)
		{
			_signInManager = signInManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			if (_signInManager.IsSignedIn(HttpContext.User))
				return View("SignedIn", HttpContext.User.Identity.Name);
			return Content(string.Empty);
		}
    }
}
