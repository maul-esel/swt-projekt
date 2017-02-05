using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lingvo.Backend.ViewModels
{
    public class CreateUserModel
    {
		[Required]
		public string Username { get; set; }

		[Required, DataType(DataType.Password)] // TODO: password requirements: digit, upper, non-alphanum
		public string Password { get; set; }

		[Required, DataType(DataType.Password), Compare(nameof(Password))]
		public string PasswordRepeat { get; set; }
    }
}
