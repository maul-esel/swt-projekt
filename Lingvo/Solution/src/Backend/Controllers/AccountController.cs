using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Lingvo.Backend.ViewModels;

namespace Lingvo.Backend.Controllers
{
	/// <summary>
	/// The controller responsible for managing account-related functionality
	/// such as login, logout and creating new users for the backend.
	/// Users are represented by the class <see cref="Editor"/>.
	/// Account management functionality is implemented using Microsoft's
	/// ASP.NET Core Identity framework.
	/// </summary>
#if !DEBUG
	[RequireHttps]
#endif
	public class AccountController : Controller
    {
		private readonly UserManager<Editor> _userManager;
		private readonly SignInManager<Editor> _signInManager;

		/// <summary>
		/// Creates a new instance of the controller. The parameters are supplied
		/// via dependency injection.
		/// </summary>
		public AccountController(UserManager<Editor> userManager, SignInManager<Editor> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		/// <summary>
		/// Displays the login view. The server redirects unrecognized users to this view.
		/// Logged in users are redirected to the index view.
		/// </summary>
		public IActionResult Login()
		{
			if (User != null && _signInManager.IsSignedIn(User))
				return Redirect("/");
			return View();
		}

		/// <summary>
		/// Validetes the given user credentials. If successful, creates a new session for the user
		/// and redirects to the index view.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Login(string userName, string password)
		{
			var editor = await _userManager.FindByNameAsync(userName);
			if (editor == null)
			{
				ModelState.AddModelError(nameof(userName), "Nutzername inkorrekt");
				return View();
			}

			var signinResult = await _signInManager.PasswordSignInAsync(editor, password, isPersistent: true, lockoutOnFailure: false);
			if (!signinResult.Succeeded)
			{
				ModelState.AddModelError(nameof(password), "Falsches Passwort");
				return View();
			}

			return Redirect("/");
		}


		/// <summary>
		/// Deletes a logged in user's session and redirects to <see cref="LoggedOut"/>.
		/// </summary>
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(LoggedOut));
		}

		/// <summary>
		/// Displays a view notifying the user he was logged out.
		/// </summary>
		public IActionResult LoggedOut()
		{
			return View();
		}

		/// <summary>
		/// Displays the view for creating a new user. This functionality is only
		/// available to logged in users.
		/// </summary>
		[Authorize]
		public IActionResult CreateUser()
		{
			return View(new CreateUserModel());
		}

		/// <summary>
		/// Creates a new user (<see cref="Editor"/>) with the given user name and
		/// password. Displays a view notifying the user of this.
		/// </summary>
		[HttpPost, Authorize]
		public async Task<IActionResult> CreateUser(CreateUserModel model)
		{
			if (model.Password != model.PasswordRepeat)
			{
				ModelState.AddModelError(nameof(CreateUserModel.PasswordRepeat), "Passwort stimmt nicht überein");
				return View(model);
			}

			var result = await _userManager.CreateAsync(new Editor { Name = model.Username }, model.Password);
			if (!result.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "Erstellung des Benutzers ist fehlgeschlagen");
				return View(model);
			}

			return View("UserCreated");
		}

		/// <summary>
		/// Remote validation method verifying that user names are unique.
		/// </summary>
		[AcceptVerbs("GET", "POST"), Authorize]
		public async Task<IActionResult> VerifyUniqueUsername(string username)
		{
			var user = await _userManager.FindByNameAsync(username);
			return Json(data: user == null);
		}
	}
}
