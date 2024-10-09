using System.ComponentModel.DataAnnotations;

namespace Zumba.ViewModels
{
	public class UserVM
	{
		[Required(ErrorMessage = "Povinné pole.")]
		public string? FirstName { get; set; }
		[Required(ErrorMessage = "Povinné pole.")]
		public string? LastName { get; set; }
		[Required(ErrorMessage = "Povinné pole.")]
		[EmailAddress(ErrorMessage = "Neplatný email.")]
		public string? Email { get; set; }
		[Required(ErrorMessage = "Povinné pole.")]
		[Phone(ErrorMessage = "Neplatné telefonní číslo.")]
		public string? PhoneNumber { get; set; }
		public bool IsActive { get; set; }
		public bool MustChangePassword { get; set; }
	}
}
