using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lingvo.Backend.ViewModels
{
    public class CreateUserModel
    {
		[Required]
		[Remote(action: "VerifyUniqueUsername", controller: "Account", ErrorMessage = "UserNameTaken")]
		public string Username { get; set; }

		[Required, DataType(DataType.Password)]
		[RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,})", ErrorMessage = "PasswordFormatViolated")]
		public string Password { get; set; }

		[Required, DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "PasswordsDontMatch")]
		public string PasswordRepeat { get; set; }
    }
}
