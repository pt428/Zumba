using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zumba.Models;
using Zumba.Services;

namespace Zumba.Controllers
{
	[Authorize]
	public class HistoryController : Controller
	{
		public HistoryService _historyService { get; set; }
		public UserManager<AppUser> _userManager { get; set; }

		public HistoryController(HistoryService historyService,UserManager<AppUser> userManager)
		{
			_historyService = historyService;
			_userManager = userManager;
		}
		//*************************************************************************
		//************  INDEX  ************************************
		//*************************************************************************
		public async Task<IActionResult> IndexAsync([FromQuery] string parameter = "")
		{
			AppUser? actualUser = await _userManager.GetUserAsync(HttpContext.User);
			if (actualUser is not null)
			{
				var allHistory = await _historyService.GetAllAsync(actualUser, parameter);
				return View(allHistory);
			}
			return View( );
		}
		//*************************************************************************
		//************  DELETE ************************************
		//*************************************************************************
		public async Task<IActionResult> DeleteAllHistory()
		{
			AppUser? actualUser = await _userManager.GetUserAsync(HttpContext.User);
			if (actualUser is not null)
			{
				await _historyService.DeleteAllAsync(actualUser);
			}
			return RedirectToAction("Index");
		}
	}
}
