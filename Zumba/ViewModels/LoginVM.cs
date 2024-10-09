using System.ComponentModel.DataAnnotations;

namespace Zumba.ViewModels
{
	public class LoginVM
	{
		[Required(ErrorMessage = "Povinné pole.")]
		public string? Email { get; set; }
		[Required(ErrorMessage = "Povinné pole.")]
		public string? Password { get; set; }
		public string? ReturnUrl { get; set; }
		public bool Remember {  get; set; }
	}
}
