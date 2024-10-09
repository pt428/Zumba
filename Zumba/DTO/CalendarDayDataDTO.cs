using Zumba.Models;

namespace Zumba.DTO
{
	public class CalendarDayDataDTO
	{
		public int NumberOfReservationOfDay { get; set; }
		public string? FirstDateOfWeek { get; set; }
		public string? DateOfDay { get; set; }
		public string? NameOfDay { get; set; }
		public string? TimeFrom { get; set; }
		public string? TimeTo { get; set; }
		public string? Place { get; set; }
		public string? AmountCredit { get; set; }
		public string? AmountCash { get; set; }
		public bool IsDayActive { get; set; }
		public bool LessonIsOff { get; set; }=false;
		public string? OwnerEmail { get; set; }
	 
		public string? OwnerOfBankAccount { get; set; }
		public string? OwnerBankAccount { get; set; }
		public string? MaxNumberOfPlacesOfDay { get; set; }
		public List<AppUser>? UsersOfReservation { get; set; }
	}
}
