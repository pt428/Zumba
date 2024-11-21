using System.ComponentModel.DataAnnotations;

namespace Zumba.ViewModels
{
	public class ResetPasswordVM
	{	public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Nové heslo")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Potvrďte heslo")]
		[Compare("Password", ErrorMessage = "Hesla se neshodují.")]
		public string ConfirmPassword { get; set; }

		public string Token { get; set; }
	}
}
 