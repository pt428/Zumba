using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zumba.DTO;
using Zumba.Models;
using Zumba.Services;

namespace Zumba.Controllers
{
	[Authorize(Roles = "Admin")]
	public class SettingsController : Controller
	{
		private SettingsService _settingsService;

		public SettingsController(SettingsService settingsService)
		{
			_settingsService = settingsService;
		}
		//*************************************************************************
		//************  INDEX ************************************
		//*************************************************************************
		public async Task<IActionResult> Index()
		{
			var settings = await _settingsService.GetAllAsync();
			return View(settings);
		}
		//*************************************************************************
		//************ SETTING UPDATE CHANGIES ************************************
		//*************************************************************************
		[HttpPost]
		public async Task<IActionResult> Change(IEnumerable<SettingsDTO> settingsDtos)
		{
			foreach (var settingDto in settingsDtos)
			{
				await _settingsService.UpdateSettingsAsync(settingDto);
			}
			CalendarController.Info=settingsDtos.ToList()[0].Info??string.Empty;
			return RedirectToAction("Index", "Calendar"); // Redirect to a relevant action

		}
	}
}
