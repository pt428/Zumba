
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zumba.DTO;
using Zumba.Models;

namespace Zumba.Services
{
	public class HistoryService
	{
		public ApplicationDbContext _dbContext { get; set; }
		public UserManager<AppUser> _userManager { get; set; }

		public HistoryService(ApplicationDbContext dbContext,UserManager<AppUser> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
		}

		public async Task<IEnumerable<HistoryDTO>> GetAllAsync(AppUser actualUser,string parameter)
		{
			var allHistory = await _dbContext.History.Include(x=>x.AppUser).ToListAsync();
			var allHistoryDtos = new List<HistoryDTO>();
			foreach (var history in allHistory)
			{
				allHistoryDtos.Add(ModelToDto(history));
			}
			 
			if (parameter == "")
			{

				if (await _userManager.IsInRoleAsync(actualUser, "Admin")) { return allHistoryDtos.OrderByDescending(x => x.Id); }
				else { return allHistoryDtos.Where(x => x?.AppUser?.Id == actualUser.Id).ToList().OrderByDescending(x => x.Id); }
			}
			else
			{

				if (await _userManager.IsInRoleAsync(actualUser, "Admin")) { return allHistoryDtos.OrderBy(x => x.Date); }
				else { return allHistoryDtos.Where(x => x?.AppUser?.Id == actualUser.Id).ToList().OrderBy(x => x.Date); }
			}
		}

		public async Task DeleteAllAsync(AppUser user)
		{
			var historyRecords =await _dbContext.History.Include(x=>x.AppUser).Where(x=> x.AppUser.Id == user.Id).ToListAsync();
			_dbContext.History.RemoveRange(historyRecords);
			_dbContext.SaveChanges();
		}
		private HistoryDTO ModelToDto(History history)
		{
			return new HistoryDTO()
			{
				Id = history.Id,
				Date=DateTime.Parse( history.Date ),
				AppUser = history.AppUser,
				Amount = history.Amount,
				CreditAfter = history.CreditAfter,
				CreditBefore = history.CreditBefore,
				Description = history.Description,
				DateOfCreation=DateTime.Parse( history.DateOfCreation )
			};
		}

	}
}
