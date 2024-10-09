using Microsoft.AspNetCore.Identity;

namespace Zumba.Models
{
    public class AppUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string? State { get; set; }
        public string? Credit {  get; set; }
        public bool MustChangePassword { get; set; }

	}
}
