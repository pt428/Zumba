using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using Zumba.Services;

namespace Zumba.Controllers
{
	[Authorize]
	public class QrCodeController : Controller
	{
		private readonly QrCodeService _qrCodeService;

		public QrCodeController(QrCodeService qrCodeService)
		{
			_qrCodeService = qrCodeService;
		}
		//*************************************************************************
		//************  GENERATE QR CODE ************************************
		//*************************************************************************
		[SupportedOSPlatform("windows")]//PRIDANO !!!
		[HttpGet]
		public IActionResult GenerateQrCode(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return BadRequest("Text cannot be null or empty.");
			}

			var qrCodeImage = _qrCodeService.GenerateQrCode(text);

			using var stream = new MemoryStream();
			qrCodeImage.Save(stream, ImageFormat.Png);
			stream.Position = 0;

			return File(stream.ToArray(), "image/png");
		}
	}
}
