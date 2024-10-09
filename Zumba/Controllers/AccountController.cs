using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zumba.Models;
using Zumba.ViewModels;

namespace Zumba.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private UserManager<AppUser> _userManager;
		private SignInManager<AppUser> _singInManager;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> singInManager)
		{
			_userManager = userManager;
			_singInManager = singInManager;
		}
		//*************************************************************************
		//************ LOGIN START ************************************
		//*************************************************************************
		[AllowAnonymous]
		public IActionResult Login(string returnUrl)
		{
			LoginVM loginVM = new LoginVM();
			loginVM.ReturnUrl = returnUrl;
			return View(loginVM);
		}
		//*************************************************************************
		//************ LOGIN END ************************************
		//*************************************************************************
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginVM loginVM)
		{
			if (ModelState.IsValid)
			{
				AppUser? appUser = await _userManager.FindByEmailAsync(loginVM.Email);
				 
				if (appUser != null && appUser.IsActive)
				{
					await _singInManager.SignOutAsync();
					Microsoft.AspNetCore.Identity.SignInResult result = await _singInManager.PasswordSignInAsync(appUser, loginVM.Password, false, loginVM.Remember);
					if (result.Succeeded)
					{
						if (appUser.MustChangePassword)
						{
                            return RedirectToAction("Edit", "Users", new { id = appUser.Id });
                        }
						return Redirect(loginVM.ReturnUrl ?? "/");
					}
					ModelState.AddModelError(nameof(loginVM.Email), "Nesprávný email nebo heslo");
				}
				else
				{
                    ModelState.AddModelError(nameof(loginVM.Email), "Účet není aktivní");
                }
			}
			return View(loginVM);
		}
		//*************************************************************************
		//************ LOGOUT ************************************
		//*************************************************************************
		public async Task<IActionResult> Logout()
		{
			await _singInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
		//*************************************************************************
		//************ ACCESS DENIED ************************************
		//*************************************************************************
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
