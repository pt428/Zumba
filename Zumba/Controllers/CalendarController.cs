using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zumba.Models;
using Zumba.Services;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Zumba.DTO;
using Microsoft.AspNetCore.Http;

namespace Zumba.Controllers
{
	[Authorize]
	public class CalendarController : Controller
	{
		private CalendarService _calendarService;
		private SettingsService _settingsService;
		private UserManager<AppUser> _userManager;
		public static string Info { get; set; } =string.Empty;

		public CalendarController(CalendarService calendarService, SettingsService settingsService, UserManager<AppUser> userManager)
		{
			_calendarService = calendarService;
			_settingsService = settingsService;
			_userManager = userManager;

		}

		//*************************************************************************
		//************ INDEX ************************************
		//*************************************************************************
		public async Task<IActionResult> Index(string date = "")
		{
			// Pass the list of dates to the view
			var data = await _calendarService.GetDatesOfWeek(date);
			 ViewBag.Info = Info;
			return View(data);
		}

		//*************************************************************************
		//************  MOVE CALENDAR TO BACKWARD ************************************
		//*************************************************************************
		public async Task<IActionResult> MoveBackwardWeek(string date)
		{
			var data = await _calendarService.MoveBackwardWeek(date);
			 ViewBag.Info = Info;
			return View("Index",data );
		}
		//*************************************************************************
		//************  MOVE CALENDAR TO FORWARD ************************************
		//*************************************************************************
		public async Task<IActionResult> MoveForwardWeek(string date)
		{
			var data = await _calendarService.MoveForwardWeek(date);
			 ViewBag.Info = Info;
			return View("Index", data);
		}
		//*************************************************************************
		//************  ADD NEW RESERVATION ************************************
		//*************************************************************************
		public async Task<IActionResult> AddReservation(CalendarDayDataDTO calendarDayDataDTO)
		{
			CultureInfo czechCulture = new CultureInfo("cs-CZ");
			DateTime? _startDayOfWeek = DateTime.Parse(calendarDayDataDTO.FirstDateOfWeek);  //DateTime.ParseExact(startDayOfWeek, "dd.MM.yyyy", czechCulture);
			AppUser? actualUser = await _userManager.GetUserAsync(HttpContext.User);

			var (resultFull, resultUserCredit, resultEmail, resultSaveDb) = await _calendarService.AddReservation(actualUser, calendarDayDataDTO);

			TempData["SuccessMessage"] = "Rezervace byla úspěšně vytvořena.";
			TempData["ErrorMessage"] = null;
			if (!resultUserCredit) { TempData["ErrorMessage"] = "Problém s připsáním kreditu !!!"; TempData["SuccessMessage"] = null; }
			if (!resultEmail) { TempData["SuccessMessage"] = null; TempData["ErrorMessage"] = "Problém s odesláním potvrzovacího emailu."; }
			if (resultSaveDb < 2) { TempData["SuccessMessage"] = null; TempData["ErrorMessage"] = "Problém s uložením do databáze."; }
			if (resultFull) { TempData["SuccessMessage"] = null; TempData["ErrorMessage"] = "V daném termínu právě někdo obsadil poslední místo."; }
			 ViewBag.Info = Info;
			return RedirectToAction("Index", new { date = calendarDayDataDTO.DateOfDay });
		}
		//*************************************************************************
		//************  CHANGE LESSON STATUS TO OFF ************************************
		//*************************************************************************
		public async Task<IActionResult> ChangeLessonStatusToOff(CalendarDayDataDTO calendarDayDataDTO)
		{
			await _calendarService.ChangeLessonStatusAsync(calendarDayDataDTO.DateOfDay,true);
				return RedirectToAction("Index", new { date = calendarDayDataDTO.DateOfDay });
		}		
		//*************************************************************************
		//************  CHANGE LESSON STATUS TO ON ************************************
		//*************************************************************************
		public async Task<IActionResult> ChangeLessonStatusToOn(CalendarDayDataDTO calendarDayDataDTO)
		{
			await _calendarService.ChangeLessonStatusAsync(calendarDayDataDTO.DateOfDay,false);
				return RedirectToAction("Index", new { date = calendarDayDataDTO.DateOfDay });
		}

	}
}
