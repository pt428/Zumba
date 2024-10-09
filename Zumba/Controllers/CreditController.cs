using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zumba.Models;
using Zumba.Services;

namespace Zumba.Controllers
{
	public class CreditController : Controller
	{
		private CreditService _creditService;
		private UserManager<AppUser> _userManager;
		public CreditController(CreditService creditService,UserManager<AppUser> userManager)
		{
			_creditService=creditService;
			_userManager=userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task< IActionResult> ShowQrcode(int amount )
		{   AppUser? actualUser = await _userManager.GetUserAsync(HttpContext.User);
			string userName = string.Empty;
			if (actualUser!=null)
			{
				userName=actualUser.FirstName+" "+actualUser.LastName;
			}
			var qrCodeBytes = await _creditService.GenerateQrCode(amount,userName);
			ViewBag.QrCode = qrCodeBytes;
			ViewBag.Amount = amount;
        return View();
		 
		}

	}
}
