using Microsoft.AspNetCore.Mvc;
using aspPrakt.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace aspPrakt.Controllers
{
    [Authorize] // Убедитесь, что только аутентифицированные пользователи могут работать с корзиной
    public class CartController : Controller
    {
        private readonly BpnContext _context;

        public CartController(BpnContext context)
        {
            _context = context;
        }

        // Отображение корзины текущего пользователя
        public async Task<IActionResult> Index()
        {
            // Проверяем, аутентифицирован ли пользователь
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Перенаправляем на страницу входа
            }

            // Получаем ID текущего пользователя
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Перенаправляем на страницу входа
            }

            var userId = int.Parse(userIdString);

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserID == userId)
                .ToListAsync();

            ViewBag.CartItems = cartItems;
            ViewBag.TotalSum = cartItems.Sum(ci => ci.Price * ci.Quantity);
            return View();
        }

        // Добавление товара в корзину
        public async Task<IActionResult> AddToCart(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Перенаправляем на страницу входа
            }

            var userId = int.Parse(userIdString);

            // Проверяем, существует ли уже такой товар в корзине
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductID == id && ci.UserID == userId);

            if (existingCartItem != null)
            {
                // Если товар уже есть в корзине, увеличиваем количество
                existingCartItem.Quantity++;
            }
            else
            {
                // Если товара нет в корзине, добавляем его
                var cartItem = new CartItem
                {
                    ProductID = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = 1,
                    UserID = userId
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Удаление товара из корзины
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Перенаправляем на страницу входа
            }

            var userId = int.Parse(userIdString);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemID == id && ci.UserID == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Увеличение количества товара в корзине
        public async Task<IActionResult> IncreaseCartItem(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Перенаправляем на страницу входа
            }

            var userId = int.Parse(userIdString);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemID == id && ci.UserID == userId);

            if (cartItem != null)
            {
                cartItem.Quantity++;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Уменьшение количества товара в корзине
        public async Task<IActionResult> DecreaseCartItem(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Перенаправляем на страницу входа
            }

            var userId = int.Parse(userIdString);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemID == id && ci.UserID == userId);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}