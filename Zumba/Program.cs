using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using Zumba.Models;
using Zumba.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<IdentityOptions>(opts =>
{
	opts.Password.RequiredLength = 8;
	opts.Password.RequireLowercase = true;
});




// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var supportedCultures = new List<CultureInfo>
	{
		new CultureInfo("cs-CZ"),
	};
	options.DefaultRequestCulture = new RequestCulture("cs-CZ");
	options.SupportedCultures = supportedCultures;
	options.SupportedUICultures = supportedCultures;
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	//options.UseSqlServer(builder.Configuration.GetConnectionString("MonsterZumbaTestDb"));
	options.UseSqlServer(builder.Configuration.GetConnectionString("MonsterZumbaDb"));
});
 
builder.Services.AddScoped<CalendarService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<ReservationsService>();
builder.Services.AddScoped<QrCodeService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<HistoryService>();
builder.Services.AddScoped<CreditService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddErrorDescriber<CzechIdentityErrorDescriber>();	 


builder.Services.ConfigureApplicationCookie(opt=>opt.LoginPath = "/Account/Login");
builder.Services.AddRazorPages();
var app = builder.Build();


var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
if (locOptions is not null) { app.UseRequestLocalization(locOptions.Value); }
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
