using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zumba.Models;
using System.Threading.Tasks;
namespace Zumba.Components
{
	 
	 
		public class UserViewComponent : ViewComponent
		{
			private readonly UserManager<AppUser> _userManager;

			public UserViewComponent(UserManager<AppUser> userManager)
			{
				_userManager = userManager;
			}

			public async Task<IViewComponentResult> InvokeAsync()
			{
				var user = await _userManager.GetUserAsync(HttpContext.User);
				return View(user);
			}
		}
	 

}
