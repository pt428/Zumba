using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Zumba.Models;
using Zumba.ViewModels;

namespace Zumba.Controllers
{
	public class MassEmailController : Controller
	{
		private RoleManager<IdentityRole> _roleManager;
		private UserManager<AppUser> _userManager;

		public MassEmailController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager=roleManager;
			_userManager=userManager;
		}

		public async Task<IActionResult> Index()
		{
			// List<IdentityRole> role = await _roleManager.Roles.ToList();
			List<MassEmailRecipient> massEmailUsers = new List<MassEmailRecipient>();
			var users = _userManager.Users;
			foreach (AppUser user in _userManager.Users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				massEmailUsers.Add(new MassEmailRecipient()
				{
					Email=user.Email,
					FirstName=user.FirstName,
					LastName=user.LastName,
					Role=string.Join(", ", roles)
				});
			}
			MassEmailVM massEmailVM = new MassEmailVM()
			{
				Recipients = massEmailUsers
			};
			return View(massEmailVM);
		}
		[HttpPost]
		public async Task<IActionResult> SendEmail( MassEmailVM massEmailVM)
		{


			return View();
		}
	}
}
