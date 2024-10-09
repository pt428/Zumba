namespace Zumba.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public  AppUser? User { get; set; }
        public string? Number { get; set; }
        public string? Day { get; set; }
        public string? Date { get; set; }
        public string? TimeFrom { get; set; }
        public string? TimeTo { get; set; }
        
        public string? DateOfCreation { get; set; }
        public string? Price { get; set; }
        public string? Payment {  get; set; }
    }
}
