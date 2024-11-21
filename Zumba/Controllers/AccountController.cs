using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using Zumba.Models;
using Zumba.ViewModels;
using Zumba.Services;
namespace Zumba.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private UserManager<AppUser> _userManager;
		private SignInManager<AppUser> _singInManager;
				private EmailService _emailService;
		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> singInManager, EmailService emailService)
		{
			_userManager=userManager;
			_singInManager=singInManager;
			_emailService=emailService;
		}
		//*************************************************************************
		//************ LOGIN START ************************************
		//*************************************************************************
		[AllowAnonymous]
		public IActionResult Login(string returnUrl)
		{
			LoginVM loginVM = new LoginVM();
			loginVM.ReturnUrl=returnUrl;
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

				if (appUser!=null&&appUser.IsActive)
				{
					await _singInManager.SignOutAsync();
					Microsoft.AspNetCore.Identity.SignInResult result = await _singInManager.PasswordSignInAsync(appUser, loginVM.Password, false, loginVM.Remember);
					if (result.Succeeded)
					{
						if (appUser.MustChangePassword)
						{
							return RedirectToAction("Edit", "Users", new { id = appUser.Id });
						}
						return Redirect(loginVM.ReturnUrl??"/");
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
		//************ FORGET PASSWORD ************************************
		//*************************************************************************
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{

			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
		{


			AppUser user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

			if (user==null)
			{
				// Zobrazit zprávu o tom, že uživatel nebyl nalezen
				ModelState.AddModelError("", "Uživatel nebyl nalezen");

				return View(forgotPasswordVM);
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			// Sestavte odkaz pro reset hesla

			//var resetLink = $"{Request.Scheme}://{Request.Host}/Account/ResetPassword?token={token}&email={HttpUtility.UrlEncode(email)}";
			var resetLink = Url.Action("ResetPassword", "Account", new { token = WebUtility.UrlEncode(token), email = user.Email }, protocol: HttpContext.Request.Scheme);
			List<string> toEmails = new List<string>() { user.Email };
			bool result = await _emailService.SendEmailAsync(toEmails, "Zumba - Reset hesla", $"Kliknutím na <a href='{resetLink}'>odkaz</a> resetujete heslo.", "");


			if (result)
			{
				return RedirectToAction("EmailResult", new { message = $"Odkaz pro resetování hesla byl odeslán na email {forgotPasswordVM.Email}" });
			}
			return RedirectToAction("EmailResult", new { message = $"Chyba při odesílání na email {forgotPasswordVM.Email}" });


		}
		[AllowAnonymous]
		public IActionResult EmailResult(string message)
		{
			return View("EmailResult",message);
		}
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword(string token, string email)
		{
			if (token==null||email==null)
			{
				// Zobrazit chybovou stránku, pokud token nebo e-mail chybí
				return View("Error");
			}
		
			var model = new ResetPasswordVM { Token=token, Email=email };
			return View(model);

		}
				[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user==null)
			{
				// Ochrana proti odhalení, zda uživatel existuje nebo ne
				ModelState.AddModelError("", "Uživatel neexistuje.");
				return View( );
			}
				string decodedToken = WebUtility.UrlDecode(model.Token);
			var result = await _userManager.ResetPasswordAsync(user,decodedToken , model.Password);
			if (result.Succeeded)
			{
				TempData["SuccessMessage"] = $"Heslo bylo změněno.\nMůžete se přihlásit.";
				return RedirectToAction("Login",new { returnUrl = "/" });
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}

		// GET: /Account/ResetPasswordConfirmation
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
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
