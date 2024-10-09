using Zumba.Models;

namespace Zumba.DTO
{
	public class HistoryDTO
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public DateTime DateOfCreation { get; set; }
		public AppUser? AppUser { get; set; }
		public string? Description { get; set; }
		public string? CreditBefore { get; set; }
		public string? CreditAfter { get; set; }
		public string? Amount { get; set; }
	}
}
