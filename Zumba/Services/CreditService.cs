using System.Text.RegularExpressions;
using System.Text;
using Zumba.DTO;
using Zumba.Migrations;
using Zumba.StaticClass;
using Microsoft.EntityFrameworkCore;
using Zumba.Models;
using System.Drawing;
using Microsoft.AspNetCore.Identity;

namespace Zumba.Services
{
	public class CreditService
	{
		private ApplicationDbContext _dbContext;
		private EmailService _emailService;
	 
		public CreditService(ApplicationDbContext dbContext, EmailService emailService )
		{
			_dbContext=dbContext;
			_emailService=emailService;
			 
		}

		public async Task<byte[]>  GenerateQrCode(int amount, string userName)
		{
			 
			var allSetings = await _dbContext.Settings.ToListAsync();
			var bankAccount = allSetings[0].BankAccount;
			var owner = allSetings[0].OwnerOfBankAccount;

			string message = $"\nZaplaťte prosím částku {amount}Kč, níže naleznete QR kód.";
			string? ownerWithOutDiacritic = new Regex(@"\p{Mn}", RegexOptions.Compiled).Replace(owner.Normalize(NormalizationForm.FormD), string.Empty);
			string? userNameWithOutDiacritic = new Regex(@"\p{Mn}", RegexOptions.Compiled).Replace(userName.Normalize(NormalizationForm.FormD), string.Empty);
			string[]? ucet = bankAccount.Split('/');//SPLIT ACCOUNT DATA
			string IBAN = IbanGenerator.GenerateCzechIban(ucet[0].Replace("-", ""), ucet[1]);
			string qrCodeText = $"SPD*1.0*ACC:{IBAN}*AM:{amount.ToString()}*CC:CZK*MSG:Zumba-dobiti kreditu - {userNameWithOutDiacritic}*RN:{ownerWithOutDiacritic}*X-VS:000000";
			var qrCodeImage = _emailService.GenerateQrCode(qrCodeText);

			 return qrCodeImage;
		}
	}
}
