using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using Zumba.DTO;
using Zumba.Migrations;
using Zumba.Models;

namespace Zumba.Services
{
	public class ReservationsService
	{
		private ApplicationDbContext _dbContext;
		private UserManager<AppUser> _userManager;
		private EmailService _emailService;
		private SettingsService _settingsService;

		public ReservationsService(ApplicationDbContext dbContext, UserManager<AppUser> userManager, EmailService emailService, SettingsService settingsService)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_emailService = emailService;
			_settingsService = settingsService;
		}
		 //******************************************************************
        //***************** GET ALL RESERVATIONS  *****************************
        //******************************************************************
		public async Task<IEnumerable<ReservationDTO>> GetAllAsync(AppUser actualUser, string parameter)
		{
			var allReservations = await _dbContext.Reservations.Include(x => x.User).ToListAsync();

			var reservationDtos = new List<ReservationDTO>();
			foreach (var reservation in allReservations)
			{
				reservationDtos.Add(ModelToDto(reservation));
			}
			if (parameter == "old")
			{
				if (await _userManager.IsInRoleAsync(actualUser, "Admin")) { return reservationDtos.OrderBy(x => x.Date); }
				else { return reservationDtos.Where(x => x.User.Id == actualUser.Id).ToList().OrderBy(x => x.Date); }

			}
			else
			{
				if (await _userManager.IsInRoleAsync(actualUser, "Admin")) { return reservationDtos.OrderByDescending(x => x.Date); }
				else { return reservationDtos.Where(x => x.User.Id == actualUser.Id).ToList().OrderByDescending(x => x.Date); }

			}
		}
		 //******************************************************************
        //***************** GET ALL RESERVATION BY DATE   *****************************
        //******************************************************************
		public async Task<IEnumerable<ReservationDTO>> GetAllByDateAsync(string date, AppUser actualUser, string parameter)
		{
			var allReservations = await _dbContext.Reservations.Where(x => x.Date == date).Include(x => x.User).ToListAsync();
			var reservationDtos = new List<ReservationDTO>();
			foreach (var reservation in allReservations)
			{
				reservationDtos.Add(ModelToDto(reservation));
			}
			if (parameter == "old")
			{

				if (await _userManager.IsInRoleAsync(actualUser, "Admin")) { return reservationDtos.OrderBy(x => x.Date); }
				else { return reservationDtos.Where(x => x.User.Id == actualUser.Id).ToList().OrderBy(x => x.Date); }
			}
			else
			{
				if (await _userManager.IsInRoleAsync(actualUser, "Admin")) { return reservationDtos.OrderByDescending(x => x.Date); }
				else { return reservationDtos.Where(x => x.User.Id == actualUser.Id).ToList().OrderByDescending(x => x.Date); }

			}
		}
		 //******************************************************************
        //***************** PAY RESERVATION  *****************************
        //******************************************************************
		public async Task<(bool, int)> PayReservationAsync(string idOfReservation)
		{
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

			int resultSaveDb = 0;
			bool resultReservation = false;
			int id = int.Parse(idOfReservation);
			Reservation reservation = await _dbContext.Reservations.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (reservation != null)
			{
				resultReservation = true;
				History history = new History()
				{
					Id = 0,
					Date = reservation.Date,
					AppUser = reservation.User,
					CreditAfter = reservation.User.Credit,
					CreditBefore = reservation.User.Credit,
					Amount = reservation.Price,
					Description = $"Rezervace {reservation.Date} zaplacena hotově",
					DateOfCreation = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).ToString(),

				};
				reservation.Payment = "ZAPLACENO";
				_dbContext.Reservations.Update(reservation);
				await _dbContext.History.AddAsync(history);
				resultSaveDb = await _dbContext.SaveChangesAsync();

			}
			return (resultReservation, resultSaveDb);
		}
		 //******************************************************************
        //***************** CANCEL RESERVATION   *****************************
        //******************************************************************
		public async Task<(bool, bool, bool, int)> CancelReservationAsync(string idOfReservation)
		{
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
			bool resultReservation = false;
			bool resultUser = true;
			bool resultEmail = false;
			int resultSave = 0;
			DateTime dateTimeNow = DateTime.Now;
			int id = int.Parse(idOfReservation);
			Reservation? reservation = await _dbContext.Reservations.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (reservation != null)
			{
				resultReservation = true;
				AppUser? user = reservation.User;
				string? date = reservation.Date;
				string? payment = reservation.Payment;
				TimeSpan hours = (DateTime.Parse(date) - dateTimeNow);
				int hoursBefor = (int)hours.TotalHours;
				double price = double.Parse(reservation.Price);

				_dbContext.Reservations.Remove(reservation);
				resultSave = await _dbContext.SaveChangesAsync();
				var setting = await _settingsService.GetAllAsync();

				var oneSetting = setting.First(x => x.Id == 1);
				string adminEmail = oneSetting.Email;
				List<string> toEmails = new List<string>() { user.Email, adminEmail };

				var subject = "Zrušení rezervace Zumby";
				var message = $"Dobrý den,\nVaše rezervace Zumby na jméno {user.FirstName} {user.LastName} a datum {date} byla zrušena.";
				History history = new History()
				{
					Id = 0,
					Date = date,
					AppUser = user,
					CreditBefore = user.Credit,
					CreditAfter = user.Credit,
					Amount = $"{price.ToString()} Kč",
					Description =  "Zrušení rezervace, bylo vráceno  " + (payment == "ZAPLACENO" ? price.ToString() : "0") +  " Kč",
					DateOfCreation = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).ToString(),
				};

				if (payment == "ZAPLACENO")
				{
					double creditAfter = double.Parse(user.Credit) + price;
					user.Credit = creditAfter.ToString();
					IdentityResult userResult = await _userManager.UpdateAsync(user);
					if (userResult.Succeeded)
					{
						history.CreditAfter = user.Credit;
						message += $"\nDo kreditu Vám byla vrácena částka {price.ToString()} Kč, Váš aktuální kredit je {user.Credit} Kč";
					}
					else
					{
						resultUser = false;
					}
				}
				else
				{

				}

				await _dbContext.History.AddAsync(history);

				resultSave += await _dbContext.SaveChangesAsync();
				resultEmail = await _emailService.SendEmailAsync(toEmails, subject, message, "");
			}

			return (resultReservation, resultUser, resultEmail, resultSave);
		}
		 //******************************************************************
        //***************** MODEL TO DTO   *****************************
        //******************************************************************
		private ReservationDTO ModelToDto(Reservation reservation)
		{
			return new ReservationDTO()
			{
				Id = reservation.Id,
				Date = reservation.Date,
				Number = reservation.Number,
				Payment = reservation.Payment,
				User = reservation.User,
				Price = reservation.Price,
				DateOfCreation = DateTime.Parse(reservation.DateOfCreation),
				Day = reservation.Day,
				TimeFrom = reservation.TimeFrom,
				TimeTo = reservation.TimeTo
			};
		}
		 //******************************************************************
        //***************** DTO TO MODEL  *****************************
        //******************************************************************
		private Reservation DtoToModel(ReservationDTO reservationDto)
		{
			return new Reservation()
			{
				Id = reservationDto.Id,
				Date = reservationDto.Date.ToString(),
				Number = reservationDto.Number,
				Payment = reservationDto.Payment,
				User = reservationDto.User,
				Price = reservationDto.Price,
				DateOfCreation = reservationDto.DateOfCreation.ToString(),
				Day = reservationDto.Day,
				TimeTo = reservationDto.TimeTo,
				TimeFrom = reservationDto.TimeFrom

			};
		}
	}
}
