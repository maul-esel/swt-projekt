using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.ViewModels
{
	/// <summary>
	/// A View Model used when creating a new user.
	/// </summary>
    public class CreateUserModel
    {
		/* While it would be preferrable to store error messages in a resource file instead of code,
		 * ASP.NET Core's data annotation localization support failed to work in release mode. */

		[Required(ErrorMessage = "Es muss ein Nutzername angegeben werden.")]
		[Remote(action: "VerifyUniqueUsername", controller: "Account",
			ErrorMessage = "Es existiert bereits ein Benutzer mit diesem Namen.")]
		public string Username { get; set; }

		[Required, DataType(DataType.Password)]
		[RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,})",
			ErrorMessage = "Das Passwort muss mindestens 6 Zeichen lang sein und Ziffern, Sonderzeichen, Groß- und Kleinbuchstaben enthalten.")]
		public string Password { get; set; }

		[Required, DataType(DataType.Password)]
		[Compare(nameof(Password),
			ErrorMessage = "Die eingegebenen Passwörter stimmen nicht überein.")]
		public string PasswordRepeat { get; set; }
    }
}
