using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Zumba.Models;
using Zumba.Services;

namespace Zumba.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private UserManager<AppUser> _userManager;
		private SettingsService _settingsService;

		public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SettingsService settingsService)
		{
			_logger = logger;
			_userManager = userManager;
			_settingsService = settingsService;
		}
		//*************************************************************************
		//************  INDEX ************************************
		//*************************************************************************
		[Authorize]
		public async Task<IActionResult> Index()
		{

			string message = await _settingsService.GetNewsAsync();
			return View("Index", message);
		}
		//*************************************************************************
		//************  ABOUT ME  ************************************
		//*************************************************************************
		[AllowAnonymous]
		public IActionResult AboutMe()
		{
			return View();
		}
		//*************************************************************************
		//************  GUIDANCE ************************************
		//*************************************************************************
		[AllowAnonymous]
		public IActionResult Guidance()
		{
			return View();
		}
		//*************************************************************************
		//************  ERROR LIST ************************************
		//*************************************************************************
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
