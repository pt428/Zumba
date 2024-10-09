namespace Zumba.Models
{
	public class History
	{
	 
		public int Id { get; set; }
		public string? Date { get; set; }
		public string? DateOfCreation { get; set; }
		public AppUser? AppUser { get; set; }
		public string? Description { get; set; }
		public string? CreditBefore { get; set; }
		public string? CreditAfter { get; set; }
		public string? Amount { get; set; }

	}
}
