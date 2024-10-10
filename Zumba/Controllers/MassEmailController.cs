using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zumba.Models;
using Zumba.ViewModels;
using Zumba.Services;
using Microsoft.AspNetCore.Authorization;
namespace Zumba.Controllers
{
	[Authorize(Roles = "Admin")]
	public class MassEmailController : Controller
	{
		private RoleManager<IdentityRole> _roleManager;
		private UserManager<AppUser> _userManager;
		private EmailService _emailService;

		public MassEmailController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, EmailService emailService)
		{
			_roleManager=roleManager;
			_userManager=userManager;
			_emailService=emailService;
		}

		public async Task<IActionResult> Index()
		{

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
				Recipients=massEmailUsers
			};
			return View(massEmailVM);
		}
		[HttpPost]
		public async Task<IActionResult> Index(MassEmailVM massEmailVM)
		{

			List<string> emails = new List<string>();
			foreach (var recipient in massEmailVM.Recipients)
			{
				if (recipient.AllowSendEmail&&recipient.Email!=null)
				{
					emails.Add(recipient.Email);
				}
			}
			var result = await _emailService.SendEmailAsync(emails, massEmailVM.EmailSubject, massEmailVM.EmailBody, "");
			if (result)
			{
				TempData["SuccessMessage"]="Emaily byly odeslány.";
				TempData["ErrorMessage"]=null;
			}
			else
			{
				TempData["SuccessMessage"]=null;
				TempData["ErrorMessage"]="Nastal problém při odesílání emailů.";

				return View(massEmailVM);
			}
			return RedirectToAction("Index", "Calendar");
		}
	}
}
