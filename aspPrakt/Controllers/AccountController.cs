using aspPrakt.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Добавленная директива
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace aspPrakt.Controllers
{
    public class AccountController : Controller
    {
        private readonly BpnContext _context;

        public AccountController(BpnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Загружаем пользователя вместе с его ролью
                var user = await _context.Clients
                    .Include(c => c.Role) // Загружаем роль пользователя
                    .SingleOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && VerifyPasswordHash(model.Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Login), // Утверждение для имени пользователя
                        new Claim(ClaimTypes.Email, user.Email), // Утверждение для email
                        new Claim(ClaimTypes.Role, user.Role.RoleName), // Утверждение для роли
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) // Утверждение для идентификатора пользователя
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    // Перенаправляем на разные страницы в зависимости от роли
                    if (user.Role.RoleName == "Admin")
                    {
                        return RedirectToAction("Index", "Admin"); // Перенаправляем на страницу администратора
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Перенаправляем на главную страницу для обычных пользователей
                    }
                }
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка на существование пользователя с таким email
                var existingUserByEmail = await _context.Clients.SingleOrDefaultAsync(u => u.Email == model.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже существует.");
                    return View(model);
                }

                // Проверка на существование пользователя с таким логином
                var existingUserByLogin = await _context.Clients.SingleOrDefaultAsync(u => u.Login == model.Login);
                if (existingUserByLogin != null)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует.");
                    return View(model);
                }

                var client = new Client
                {
                    Email = model.Email,
                    Login = model.Login,
                    RoleId = 1, // По умолчанию роль "Customer"
                    DateJoined = DateTime.UtcNow // Устанавливаем текущую дату
                };
                client.PasswordHash = HashPassword(model.Password);

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}