using AlisverisListesi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class AccountController : Controller
{
    private readonly ShoppingListDbContext _context = new();

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Kullanıcıyı oturum açmış olarak işaretleyin
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Email, model.Email),
            };

            var identity = new ClaimsIdentity(claims, "cookie");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Kayıt işlemi tamamlandığında ana sayfaya yönlendirin
            return RedirectToAction("login");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Attempt to find a user in the database based on the provided email and password
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.UserId.ToString()),
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user using cookie-based authentication
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect to the desired page after successful login
                return RedirectToAction("Index", "ShoppingLists");
            }

            // If user authentication fails, add a model error
            ModelState.AddModelError(string.Empty, "Geçersiz E-Posta ya da Şifre ");
        }

        // If the model state is not valid, return the view with errors
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        // Oturumu kapatma işlemini gerçekleştirin
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Oturumu kapattıktan sonra kullanıcıyı ana sayfaya yönlendirin
        return RedirectToAction("Index", "Home");
    }


}
