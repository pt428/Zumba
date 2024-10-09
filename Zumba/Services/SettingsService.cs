using Microsoft.EntityFrameworkCore;
using Zumba.DTO;
using Zumba.Models;

namespace Zumba.Services
{
    public class SettingsService
    {
        private ApplicationDbContext _dbContext;

         public SettingsService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<SettingsDTO>> GetAllAsync()
        {
            var allSetings = await _dbContext.Settings.ToListAsync();
            var settingsDtos = new List<SettingsDTO>();
            foreach (var set in allSetings)
            {
                settingsDtos.Add(modetToDto(set));
            }
            return settingsDtos;
        }

		public async Task<SettingsDTO> UpdateSettingsAsync(SettingsDTO settingDtoToUpdate)
		{
            _dbContext.Update(DtoToModel(settingDtoToUpdate));
            await _dbContext.SaveChangesAsync();
            return settingDtoToUpdate;
		}

		private SettingsDTO modetToDto(Settings set)
        {
            return new SettingsDTO()
            {
                Id = set.Id,
                DayOfWeek = set.DayOfWeek,
                IsActive = set.IsActive,
                Place= set.Place,
				Amount = set.Amount,
				AmountCash = set.AmountCash,
				AmountCredit = set.AmountCredit,
                TimeRange   = set.TimeRange,
                TimeFrom = set.TimeFrom,
                TimeTo = set.TimeTo,
                BankAccount = set.BankAccount,
                Email   = set.Email,
                OwnerOfBankAccount= set.OwnerOfBankAccount,
                MaxNumberOfPlaces= set.NumberOfPlaces,
                News= set.News,
                Info= set.Info,
            };
        }
        private Settings DtoToModel(SettingsDTO set)
        {
            return new Settings()
            {
                Id = set.Id,
				DayOfWeek = set.DayOfWeek,
                IsActive = set.IsActive,
                Place= set.Place ?? "",
                Amount= set.Amount ?? "",
                AmountCash= set.AmountCash ?? "",
                AmountCredit= set.AmountCredit ?? "",
				TimeRange   = set.TimeRange ?? "",
                TimeFrom = set.TimeFrom ?? "",
                TimeTo = set.TimeTo ?? "",
				BankAccount = set.BankAccount ?? "",
				Email = set.Email ?? "",
				OwnerOfBankAccount = set.OwnerOfBankAccount ?? "",
                 NumberOfPlaces = set.MaxNumberOfPlaces ?? "",
                News= set.News ?? "",
                Info= set.Info ?? "",

			};
        }

		public async Task<string> GetNewsAsync()
		{
			var settingsFirstRow = await _dbContext.Settings.FirstAsync(x=>x.Id==1);
            string? news = settingsFirstRow.News ?? "";
			return news;
		}
	}
}
