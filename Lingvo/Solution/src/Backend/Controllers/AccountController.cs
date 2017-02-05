using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Lingvo.Backend.ViewModels;

namespace Lingvo.Backend.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<Editor> _userManager;
		private readonly SignInManager<Editor> _signInManager;

		public AccountController(UserManager<Editor> userManager, SignInManager<Editor> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Login()
		{
			if (User != null && _signInManager.IsSignedIn(User))
				return Redirect("/");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(string userName, string password)
		{
			var editor = await _userManager.FindByNameAsync(userName);
			if (editor == null)
			{
				ModelState.AddModelError(nameof(userName), "Nutzername inkorrekt");
				return View();
			}

			var signinResult = await _signInManager.PasswordSignInAsync(editor, password, false, false);
			if (!signinResult.Succeeded)
			{
				ModelState.AddModelError(nameof(password), "Falsches Passwort");
				return View();
			}

			return Redirect("/");
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return View();
		}

		public IActionResult CreateUser()
		{
			return View(new CreateUserModel());
		}

		[HttpPost]
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
	}
}
