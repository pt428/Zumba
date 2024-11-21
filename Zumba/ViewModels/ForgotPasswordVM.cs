using System.ComponentModel.DataAnnotations;

namespace Zumba.ViewModels
{
		public class ForgotPasswordVM
	{
	 
    
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
 

	}
}
