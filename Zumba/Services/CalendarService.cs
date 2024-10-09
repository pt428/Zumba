using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using Zumba.DTO;
using Zumba.Migrations;
using Zumba.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Zumba.StaticClass;
using System;
using System.Runtime.Versioning;
using Zumba.Controllers;

namespace Zumba.Services
{
	public class CalendarService
	{
		private ApplicationDbContext _dbContext;
		private UserManager<AppUser> _userManager;
		private readonly EmailService _emailService;
		private SettingsService _settingsService;

		public CalendarService(ApplicationDbContext dbContext, UserManager<AppUser> userManager, EmailService emailService, SettingsService settingsService)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_emailService = emailService;
			_settingsService = settingsService;

		}
		//*************************************************************************
		//************* ADD RESERVATION *************************
		//*************************************************************************
		[SupportedOSPlatform("windows")]
		public async Task<(bool, bool, bool, int)> AddReservation(AppUser actualUser, CalendarDayDataDTO calendarDayDataDTO)
		{
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
			bool resultUserCredit = true;
			bool resultEmail = false;
			bool resultFull = false;
			int resultSaveDb = 0;
			var toEmail = actualUser.Email;
			var subject = "Rezervace na lekci Zumby";
			var message = $"Dobrý den,\nVaše rezervace Zumby na jméno {actualUser.FirstName} {actualUser.LastName} a datum {calendarDayDataDTO.DateOfDay} byla potvrzena.";
			string qrCodeText = string.Empty;


			double price = double.Parse(calendarDayDataDTO.AmountCredit ?? "");
			double userBalance = double.Parse(actualUser.Credit ?? "") - price;
			string? amount = userBalance < 0 ? calendarDayDataDTO.AmountCash : calendarDayDataDTO.AmountCredit;
			string payment = "NEZAPLACENO";
			if (userBalance >= 0) { payment = "ZAPLACENO"; }
			int actualNumberOfReservations = await _dbContext.Reservations.Where(r => r.Date == calendarDayDataDTO.DateOfDay).CountAsync();
			Settings settings = await _dbContext.Settings.FirstAsync(x => x.DayOfWeek == calendarDayDataDTO.NameOfDay);
			var setting = await _settingsService.GetAllAsync();
			var oneSetting = setting.First(x => x.Id == 1);
			string? adminEmail = oneSetting.Email;
			List<string> toEmails = new List<string>() { actualUser.Email, adminEmail };
			int maxNumberOfReservations = int.Parse(settings.NumberOfPlaces);
			if (actualNumberOfReservations < maxNumberOfReservations)
			{
				History history = new History()
				{
					Date = calendarDayDataDTO.DateOfDay,
					AppUser = actualUser,
					CreditBefore = actualUser.Credit,
					CreditAfter = actualUser.Credit,
					Amount = $"{amount}Kč",
					Description = $"Rezervace {calendarDayDataDTO.DateOfDay}, {payment}",
					DateOfCreation = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).ToString(),


				};
				Reservation reservation = new Reservation()
				{
					User = actualUser,
					Number = "1",
					Date = calendarDayDataDTO.DateOfDay,
					Payment = payment,
					Price = amount,
					DateOfCreation = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).ToString(),
					Day = calendarDayDataDTO.NameOfDay,
					TimeFrom =  calendarDayDataDTO.TimeFrom  ,
					TimeTo =  calendarDayDataDTO.TimeTo 

				};
				await _dbContext.Reservations.AddAsync(reservation);
				resultSaveDb = await _dbContext.SaveChangesAsync();
				if (userBalance >= 0)
				{
					actualUser.Credit = userBalance.ToString();
					IdentityResult result = await _userManager.UpdateAsync(actualUser);
					if (result.Succeeded)
					{
						history.CreditAfter = actualUser.Credit;

						message += $"\nZ kreditu bylo strženo {amount}Kč, Váš aktuální kredit je {userBalance}Kč";
					}
					else
					{
						resultUserCredit = false;
					}
				}
				else
				{
					message += $"\nZaplaťte prosím částku {amount}Kč, níže naleznete QR kód.";
					string? ownerWithOutDiacritic = new Regex(@"\p{Mn}", RegexOptions.Compiled).Replace(calendarDayDataDTO.OwnerOfBankAccount.Normalize(NormalizationForm.FormD), string.Empty);
					string[]? ucet = calendarDayDataDTO?.OwnerBankAccount?.Split('/');//SPLIT ACCOUNT DATA
					string IBAN = IbanGenerator.GenerateCzechIban(ucet[0].Replace("-", ""), ucet[1]);
					qrCodeText = $"SPD*1.0*ACC:{IBAN}*AM:{amount}*CC:CZK*MSG:Zumba-{calendarDayDataDTO?.DateOfDay}*RN:{ownerWithOutDiacritic}*X-VS:000000";

				}

				await _dbContext.History.AddAsync(history);
				resultSaveDb += await _dbContext.SaveChangesAsync();
				resultEmail = await _emailService.SendEmailAsync(toEmails, subject, message, qrCodeText);
				resultFull = false;
				return (resultFull, resultUserCredit, resultEmail, resultSaveDb);
			}
			else
			{
				return (true, resultUserCredit, resultEmail, resultSaveDb);
			}
		}
		//*************************************************************************
		//************* GET DATES OF ACTUAL WEEK *************************
		//*************************************************************************
		public async Task<List<CalendarDayDataDTO>> GetDatesOfWeek(string date)
		{
			DateTime today;
			// Get today's date
			if (date != "") {
			  today = DateTime.Parse(date);
			}
			else
			{
				today=DateTime.Now;
			}

			int numberOfDay = (int)today.DayOfWeek == 0 ? 6 : ((int)today.DayOfWeek)-1;
			return await GenerateDaysOfWeeks(today.AddDays(-numberOfDay));
		}
		//*************************************************************************
		//************* MOVE BACKWARD IN CALENDAR *************************
		//*************************************************************************
		public async Task<List<CalendarDayDataDTO>> MoveBackwardWeek(string date)
		{
			return await GenerateDaysOfWeeks(DateTime.Parse(date).AddDays(-7));
		}
		//*************************************************************************
		//************* MOVE FORWARD IN CALENDAR *************************
		//*************************************************************************
		public async Task<List<CalendarDayDataDTO>> MoveForwardWeek(string date)
		{
			return await GenerateDaysOfWeeks(DateTime.Parse(date).AddDays(1));
		}
		//*************************************************************************
		//************* CALCULATE DAYS OF WEEK *************************
		//*************************************************************************
		public async Task<List<CalendarDayDataDTO> > GenerateDaysOfWeeks(DateTime startOfWeek)
		{
			// List to hold formatted dates
			List<CalendarDayDataDTO> calendarDayDatas = new List<CalendarDayDataDTO>();
			List<Settings> settings = await _dbContext.Settings.ToListAsync();
		 
			CalendarController.Info=settings[0].Info;
			// Calculate the dates for each day of the week
			int i = 0;
			foreach (var setting in settings)
			{		
				bool lessonIsOff=false;
				int maxNumberOfreservations = 0;
				int.TryParse(setting.NumberOfPlaces, out maxNumberOfreservations);
				DateTime currentDay = startOfWeek.AddDays(i);
				var allReservations = await _dbContext.Reservations.Include(x=>x.User).Where(r => r.Date == currentDay.ToShortDateString()).ToListAsync();
				int numberOfReservations = allReservations.Count();
				List<AppUser> _usersOfReservation = new List<AppUser>();
				List<CanceledLesson> canceledLessons = new List<CanceledLesson>();
				canceledLessons=await _dbContext.CanceledLessons.ToListAsync();
				foreach (var reservation in allReservations)
				{
					_usersOfReservation.Add(reservation.User);
				}
				foreach (var canceledLesson in canceledLessons) {

					if (canceledLesson.Date==currentDay.ToString("dd.MM.yyyy")) {
						lessonIsOff = true;
					
					}
				}
				numberOfReservations = maxNumberOfreservations - numberOfReservations;
				calendarDayDatas.Add(new CalendarDayDataDTO()
				{
					AmountCash = setting.AmountCash,
					AmountCredit = setting.AmountCredit,
					DateOfDay = currentDay.ToString("dd.MM.yyyy"),
					IsDayActive = setting.IsActive,
					MaxNumberOfPlacesOfDay = setting.NumberOfPlaces,
					NumberOfReservationOfDay = numberOfReservations,
					NameOfDay = setting.DayOfWeek,
					OwnerBankAccount = setting.BankAccount,
					OwnerOfBankAccount = setting.OwnerOfBankAccount,
					OwnerEmail = setting.Email,
					Place = setting.Place,
					TimeFrom = setting.TimeFrom,
					TimeTo = setting.TimeTo,
					FirstDateOfWeek = startOfWeek.ToString(),
					UsersOfReservation= _usersOfReservation,
					LessonIsOff= lessonIsOff,

				});
				
				i++;
			}
			return calendarDayDatas;
		}
		public async Task ChangeLessonStatusAsync(string date,bool status)
		{
			if (status)
			{
				CanceledLesson canceledLesson = new CanceledLesson();
				canceledLesson.Date=date;
				await _dbContext.CanceledLessons.AddAsync(canceledLesson);
				var resultSaveDb = await _dbContext.SaveChangesAsync();
			}
			else
			{
				CanceledLesson canceledLesson =await _dbContext.CanceledLessons.FirstOrDefaultAsync(l=>l.Date==date);
				  _dbContext.Remove(canceledLesson);
				await _dbContext.SaveChangesAsync();
			}
		}
		private ReservationDTO ModelToDto(Reservation reservation)
		{
			return new ReservationDTO()
			{

				User = reservation.User,
				Number = "1",
				Date =  reservation.Date ,
				DateOfCreation = DateTime.Parse(reservation.DateOfCreation),
				Payment = reservation.Payment,
				Price = reservation.Price,
				TimeFrom= reservation.TimeFrom,
				TimeTo= reservation.TimeTo,
				Day = reservation.Day
			};

		}
	}
}