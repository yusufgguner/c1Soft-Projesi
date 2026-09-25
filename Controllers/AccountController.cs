using System.Security.Claims;
using c1Soft_Projesi.Data;
using c1Soft_Projesi.Models;
using c1Soft_Projesi.Services;
using c1Soft_Projesi.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace c1Soft_Projesi.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly PasswordService passwordService;

    public AccountController(ApplicationDbContext db, PasswordService passwordService)
    {
        this.db = db;
        this.passwordService = passwordService;
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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await db.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                (x.Username == model.Username || x.Email == model.Username) && x.IsActive);

        if (user == null || !passwordService.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Customer")
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        if (user.Role?.RoleName == "Admin")
        {
            return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Index", "Products");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool usernameUsed = await db.Users.AnyAsync(x => x.Username == model.Username);
        bool emailUsed = await db.Users.AnyAsync(x => x.Email == model.Email);

        if (usernameUsed || emailUsed)
        {
            ModelState.AddModelError("", "Kullanıcı adı veya e-posta zaten kayıtlı.");
            return View(model);
        }

        var customerRole = await db.Roles.FirstAsync(x => x.RoleName == "Customer");

        var user = new User
        {
            RoleId = customerRole.RoleId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Phone = model.Phone,
            Username = model.Username,
            PasswordHash = passwordService.Hash(model.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
