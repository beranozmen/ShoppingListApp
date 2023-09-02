using AlisverisListesi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// IConfiguration ekleyin
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ShoppingListDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("Server=.;Database=ShoppingListDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Cookie ayarlar�n� burada yapabilirsiniz
        options.LoginPath = "/Account/Login"; // Giri� sayfas�n�n URL'si
        options.LogoutPath = "/Account/Logout"; // ��k�� sayfasn�n URL'si
        options.AccessDeniedPath = "/Account/AccessDenied"; // Eri�im reddedildi sayfas�n�n URL'si
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturum s�resi 30 dakika

        // Di�er se�enekleri burada yap�land�rabilirsiniz
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik do�rulama
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
