﻿using System.ComponentModel.DataAnnotations;

namespace Zumba.DTO
{
    public class SettingsDTO
    {
        public int Id { get; set; }
        public string? DayOfWeek { get; set; }
        public bool IsActive { get; set; }
        public string? TimeRange { get; set; }
		public string? TimeFrom { get; set; }
		public string? TimeTo { get; set; }
		public string? Place { get; set; }
		public string? AmountCredit { get; set; }
		public string? AmountCash { get; set; }
		public string? Amount { get; set; }
		public string? Email { get; set; }
        public string? OwnerOfBankAccount { get; set; }
        public string? BankAccount { get; set; }
		[Required]
		public string? MaxNumberOfPlaces { get; set; }
		public string? News { get; set; }
		public string? Info { get; set; }
	}
}