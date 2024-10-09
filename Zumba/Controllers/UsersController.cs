using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json.Linq;
using Zumba.DTO;
using Zumba.Migrations;
using Zumba.Models;
using Zumba.Services;
using Zumba.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zumba.Controllers
{
	[Authorize]
	public class UsersController : Controller
	{
		private UserManager<AppUser> _userManager;
		private IPasswordHasher<AppUser> _passwordHasher;
		private SettingsService _settingsService;
		private EmailService _emailService;
		private SignInManager<AppUser> _singInManager;
		private ApplicationDbContext _dbContext;
		private IPasswordValidator<AppUser> _passwordValidator;
		public UsersController(
			UserManager<AppUser> userManager,
			IPasswordHasher<AppUser> passwordHasher,
			SettingsService settingsService,
			EmailService emailService,
			SignInManager<AppUser> singInManager,
			ApplicationDbContext dbContext,
			IPasswordValidator<AppUser> passwordValidator)
		{
			_userManager = userManager;
			_passwordHasher = passwordHasher;
			_settingsService = settingsService;
			_emailService = emailService;
			_singInManager = singInManager;
			_dbContext = dbContext;
			_passwordValidator = passwordValidator;
		}

		//*************************************************************************
		//************ INDEX ************************************
		//*************************************************************************
		public async Task<IActionResult> Index()
		{
			AppUser? actualUser = await _userManager.GetUserAsync(HttpContext.User);
			if (actualUser is not null && await _userManager.IsInRoleAsync(actualUser, "Admin"))
			{
				var noActiveUsers = await _userManager.Users.Where(x => x.IsActive).ToListAsync();
				var activeUsers = await _userManager.Users.Where(y => y.IsActive == false).ToListAsync();
				var allUsersGroupByIsActive = activeUsers.Concat(noActiveUsers).ToList();
				return View(allUsersGroupByIsActive);

			}
			else { return View(_userManager.Users.Where(x => x.Id == actualUser.Id)); }

		}
		//**********************************************************************************
		//***********   CREATE START  ***************************************************
		//**********************************************************************************		
		[AllowAnonymous]
		public ViewResult Create() => View();
		//**********************************************************************************
		//***********   CREATE END  ***************************************************
		//**********************************************************************************
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserVM user)
		{
			if (ModelState.IsValid)
			{
				AppUser appUser = new AppUser()
				{
					FirstName = user.FirstName,
					LastName = user.LastName,
					UserName = user.Email,
					Email = user.Email,
					PhoneNumber = user.PhoneNumber,
					IsActive = false,
					State = "Aktivovat",
					Credit = "0",
					MustChangePassword = true
				};
				IdentityResult result = await _userManager.CreateAsync(appUser, "Abcd1234+");
				if (result.Succeeded)
				{
					IEnumerable<SettingsDTO> allSetings = await _settingsService.GetAllAsync();
					string? ownerEmail = allSetings.Select(x => x.Email).FirstOrDefault();
					List<string> toEmails = new List<string>() { user.Email, ownerEmail };

					var subject = "Zumba - Nová registrace";
					var message = $"Dobrý den,<br/>Vaše nová registrace byla zaznamenána, vyčkejte na její schválení.<br/>";
					message += $"Vyplněno:<br/> Jméno: {user.FirstName} {user.LastName},<br/>Email: {user.Email}, <br/>Telefonní číslo: {user.PhoneNumber}";
					await _emailService.SendEmailAsync(toEmails, subject, message, "");


					result = await _userManager.AddToRoleAsync(appUser, "User");
					if (!result.Succeeded) {
						Errors(result);
					}
					else { TempData["SuccessMessage"] = "Účet byl úspěšně vytvořen. Teď musí administrátor účet aktivovat, upozornění o aktivaci Vám přijde do emailu."; }
					return RedirectToAction("Index", "Home");
				}
				else
				{
					foreach (IdentityError error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}

			return View(user);
		}
		//**********************************************************************************
		//***********   EDIT START  ***************************************************
		//**********************************************************************************
		public async Task<IActionResult> Edit(string id)
		{
			AppUser? userToEdit = await _userManager.FindByIdAsync(id);
			if (userToEdit == null)
			{
				return View("NotFound");
			}
			else
			{
				return View(userToEdit);
			}
		}
		//**********************************************************************************
		//***********   EDIT END  ***************************************************
		//**********************************************************************************
		[HttpPost]
		public async Task<IActionResult> Edit(AppUser userVM, string password)
		{
			AppUser? user = await _userManager.FindByIdAsync(userVM.Id);
			if (user != null)
			{
				if (password == "Abcd1234+")
				{
					ModelState.AddModelError("", "Heslo již bylo použito !");
					return View(user);
				}
				IdentityResult? validPass = null;
				validPass = await _passwordValidator.ValidateAsync(_userManager, user, password);
				if (validPass != null && validPass.Succeeded)
				{
				user.FirstName = userVM.FirstName;
				user.LastName = userVM.LastName;
				user.Email = userVM.Email;
				user.UserName = userVM.Email;
				user.PhoneNumber = userVM.PhoneNumber;
				user.MustChangePassword = false;
				user.PasswordHash = _passwordHasher.HashPassword(user, password);
					IdentityResult result = await _userManager.UpdateAsync(user);
					if (result.Succeeded)
					{
						TempData["SuccessMessage"] = $"Heslo pro účet {user.Email} bylo změněno.";
						return RedirectToAction( "Index","Calendar");
					}
					else { Errors(result); }
				}
				else { Errors(validPass); }
				}
			else { ModelState.AddModelError("", "Uživatel nebyl nalezen"); }

			return View(user);
		}
		//**********************************************************************************
		//***********   ACTIVED ACCOUNT  ***************************************************
		//**********************************************************************************
		[HttpPost]
		public async Task<IActionResult> Actived(string id)
		{
			AppUser? user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				List<string> toEmails = new List<string>() { user.Email };

				string subject = string.Empty;
				string message = string.Empty;
				if (user.State == "Aktivovat")
				{
					subject = "Aktivace Vašeho Zumba účtu";
					message = $"Dobrý den,<br/>Váš Zumba účet byl aktivován.<br/>Můžete se přihlásit zde: https://zumba.azurewebsites.net/<br/> Login:{user.Email} <br/>Heslo:Abcd1234+";
					user.State = "Deaktivovat";
					user.IsActive = true;
					TempData["SuccessMessage"] = $"Účet {user.Email} byl AKTIVOVÁN.";
				}
				else
				{
					subject = "Deaktivace Vašeho Zumba účtu";
					message = "Dobrý den,<br/>Váš Zumba účet byl deaktivován.";
					user.State = "Aktivovat";
					user.IsActive = false;
					TempData["SuccessMessage"] = $"Účet {user.Email} byl DEAKTIVOVÁN.";
				}

				IdentityResult result = await _userManager.UpdateAsync(user);
				if (result.Succeeded)
				{

					await _emailService.SendEmailAsync(toEmails, subject, message, "");

					return RedirectToAction("Index");
				}
				else { Errors(result); }
			}
			else { ModelState.AddModelError("", "Uřivatel nebyl nalezen"); }

			return View(user);
		}
		//**********************************************************************************
		//***********   DELETE START ***************************************************
		//**********************************************************************************
		[HttpPost]
		public async Task<IActionResult> AddAccountNumber(string id, string credit)
		{
			return View("AddAccountNumber", (id, credit));
		}
		//**********************************************************************************
		//***********   DELETE END ***************************************************
		//**********************************************************************************
		[HttpPost]
		public async Task<IActionResult> Delete(string id, string userAccountNumber = "")
		{
			AppUser? user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				var setting = await _settingsService.GetAllAsync();
				var oneSetting = setting.First(x => x.Id == 1);
				string adminEmail = oneSetting.Email ?? "";

				AppUser? actualUser = await _userManager.GetUserAsync(HttpContext.User);
				if (actualUser is not null && actualUser.Id == user.Id) { await _singInManager.SignOutAsync(); }
				IdentityResult result = await _userManager.DeleteAsync(user);
				if (actualUser is not null && result.Succeeded)
				{
					List<string> toEmail = new List<string> { adminEmail, user.Email };

					string subject = "Smazání Zumba účtu ";
					string message = $"Váš účet na jméno {user.FirstName} {user.LastName} s loginem {user.Email} byl úspěšně smazán,\n Váš zůstatek kreditu je  {user.Credit}Kč.";

					if (userAccountNumber != "") { message = $"Váš účet na jméno {user.FirstName} {user.LastName} s loginem {user.Email} byl úspěšně smazán,\n kredit ve výši {user.Credit}Kč,\n Vám bude zaslán na číslo účtu: {userAccountNumber}\n"; }
					await _emailService.SendEmailAsync(toEmail, subject, message, "");
					TempData["SuccessMessage"] = $"Účet {user.Email} byl úspěšně smazán.";
					return RedirectToAction("Index");
				}
				else { Errors(result); }
			}
			else { ModelState.AddModelError("", "Uživatel nebyl nalezen"); }
			return View("Index", _userManager.Users);
		}
		//**********************************************************************************
		//***********   ADD CREDIT  ***************************************************
		//**********************************************************************************
		[HttpPost]
		public async Task<IActionResult> AddCredit(string id, string credit)
		{
			AppUser? user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				int creditOld = 0;
				int creditToAdd = 0;
				if (int.TryParse(user.Credit, out creditOld) && int.TryParse(credit, out creditToAdd))
				{

					user.Credit = (creditOld + creditToAdd).ToString();
					IdentityResult result = await _userManager.UpdateAsync(user);
					if (result.Succeeded)
					{
						List<string> toEmail = new List<string> {  user.Email };
						string subject = "Přidání kreditu na Zumba účtu";
						string message = $"Na Váš Zumba kredit bylo přidáno {creditToAdd}Kč,\n Váš aktuální zůstatek kreditu je  {user.Credit}Kč.";

						History history = new History()
						{
							Id = 0,
							Date = DateTime.Now.ToString(),
							AppUser = user,
							CreditAfter = user.Credit,
							CreditBefore = creditOld.ToString(),
							Amount = creditToAdd.ToString(),
							Description = $"Vloženo {creditToAdd}Kč do kreditu",
							DateOfCreation = DateTime.Now.ToString(),

						};


						await _dbContext.History.AddAsync(history);
						await _dbContext.SaveChangesAsync();
						await _emailService.SendEmailAsync(toEmail, subject, message, "");
						TempData["SuccessMessage"] = $"Částka {credit}Kč byla úspěšně přidána.";
						return RedirectToAction("Index");
					}
					else { Errors(result); }


				}
				else { ModelState.AddModelError("", "Problém s přičtení částky"); }

			}
			else { ModelState.AddModelError("", "Uživatel nebyl nalezen"); }
			return RedirectToAction("Index");
		}
		public void Errors(IdentityResult result)
		{
			foreach (IdentityError error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
		}

	}
}
