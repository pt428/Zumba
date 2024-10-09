using Microsoft.AspNetCore.Identity;

namespace Zumba.Models
{
   
    public class RoleModification
    {
         
        public string? RoleName { get; set; }
        public string? RoleId { get; set; }
        public string[]? AddIds { get; set; }
        public string[]? DeleteIds { get; set; }
    }
}
