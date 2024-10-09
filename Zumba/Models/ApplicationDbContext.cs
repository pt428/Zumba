using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zumba.Migrations;

namespace Zumba.Models
{
	public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)	{		}
	 
		public DbSet<Settings> Settings { get; set; }
		public DbSet<AppUser> AppUsers { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		 public DbSet<History> History { get; set; }
		public DbSet<News> News { get; set; }
		public DbSet<CanceledLesson> CanceledLessons { get; set; }
	}
}
