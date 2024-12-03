using aspPrakt.Models;
using Microsoft.AspNetCore.Mvc;
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
                var user = await _context.Clients.SingleOrDefaultAsync(u => u.Email == model.Email);
                if (user != null && VerifyPasswordHash(model.Password, user.PasswordHash))
                {
                    return RedirectToAction("Index", "Home");
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
                    RoleId = 1
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
    }
}