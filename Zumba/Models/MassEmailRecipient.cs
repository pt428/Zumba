using Microsoft.AspNetCore.Identity;

namespace Zumba.Models
{
	public class MassEmailRecipient
	{
		public string? Role { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public bool AllowSendEmail { get; set; }=true;
		
	}
}
