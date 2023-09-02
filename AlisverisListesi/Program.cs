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
        // Cookie ayarlarýný burada yapabilirsiniz
        options.LoginPath = "/Account/Login"; // Giriþ sayfasýnýn URL'si
        options.LogoutPath = "/Account/Logout"; // Çýkýþ sayfasnýn URL'si
        options.AccessDeniedPath = "/Account/AccessDenied"; // Eriþim reddedildi sayfasýnýn URL'si
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturum süresi 30 dakika

        // Diðer seçenekleri burada yapýlandýrabilirsiniz
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik doðrulama
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
