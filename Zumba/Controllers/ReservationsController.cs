using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Versioning;
using Zumba.DTO;
using Zumba.Models;
using Zumba.Services;

namespace Zumba.Controllers
{
	[Authorize]
	public class ReservationsController : Controller
	{
		private ReservationsService _reservationsService;
		private UserManager<AppUser> _userManager;

		public ReservationsController(ReservationsService reservationsService, UserManager<AppUser> userManager)
		{
			_reservationsService = reservationsService;
			_userManager = userManager;
		}
		//*************************************************************************
		//************  INDEX ************************************
		//*************************************************************************
		public async Task<IActionResult> Index([FromQuery] string parameter = "", string date = "")
		{
			AppUser? actualUser = await _userManager.GetUserAsync(HttpContext.User);
			IEnumerable<ReservationDTO> allReservation;
			if (actualUser != null)
			{
				if (string.IsNullOrWhiteSpace(date))
				{
					allReservation = await _reservationsService.GetAllAsync(actualUser, parameter);

				}
				else
				{
					allReservation = await _reservationsService.GetAllByDateAsync(date, actualUser, parameter);
				}

				ViewBag.Date = date;
				return View(allReservation);
			}
			return View();
		}
		//*************************************************************************
		//************ PAY RESERVATION ************************************
		//*************************************************************************
		[HttpPost]
		public async Task<IActionResult> PayReservation(string id, string date)
		{
			TempData["SuccessMessage"] = "Rezervace byla úspěšně zaplacena.";
			TempData["ErrorMessage"] = null;
			var (resultReservation, resultSaveDb) = await _reservationsService.PayReservationAsync(id);
			if (!resultReservation) { TempData["ErrorMessage"] = "Rezervace nebyla nalezena"; TempData["SuccessMessage"] = null; }
			if (resultSaveDb < 2) { TempData["ErrorMessage"] = "Problem s uložením hodnod do databaze"; TempData["SuccessMessage"] = null; }

			return RedirectToAction("Index", new { date = date });
		}
		//*************************************************************************
		//************ CANCEL RESERVATION ************************************
		//*************************************************************************
		[HttpPost]
		public async Task<IActionResult> CancelReservation(string id, string date)
		{
			TempData["SuccessMessage"] = "Rezervace byla úspěšně zrušena.";
			TempData["ErrorMessage"] = null;
			var (resultReservation, resultUser, resultEmail, resultSave) = await _reservationsService.CancelReservationAsync(id);
			if (!resultReservation) { TempData["SuccessMessage"] = null; TempData["ErrorMessage"] = "Rezervace nebyla nalezena"; }
			if (!resultUser) { TempData["SuccessMessage"] = null; TempData["ErrorMessage"] = "Problém s přičtením kreditu !!!"; }
			if (!resultEmail) { TempData["SuccessMessage"] = null; TempData["ErrorMessage"] = "Problém při posílání potvrzovacího emailu"; }
			if (resultSave < 2) { TempData["SuccessMessage"] = null; TempData["ErrorMessage"] = "Problem s uložením hodnod do databaze"; }

			return RedirectToAction("Index", new { date = date });
		}
	}
}
