using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using aspPrakt.Models;
using aspPrakt.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspPrakt.Controllers
{
    public class CartController : Controller
    {
        private readonly BpnContext _context;

        public CartController(BpnContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            var cartItems = new List<CartItem>();

            foreach (var item in cart)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.Key);
                if (product != null)
                {
                    cartItems.Add(new CartItem
                    {
                        ProductID = product.ProductId,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Quantity = item.Value
                    });
                }
            }

            ViewBag.CartItems = cartItems;
            ViewBag.TotalSum = cartItems.Sum(ci => ci.Price * ci.Quantity);
            return View();
        }

        public IActionResult AddToCart(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            if (cart.ContainsKey(id))
            {
                cart[id]++;
            }
            else
            {
                cart[id] = 1;
            }

            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            if (cart.ContainsKey(id))
            {
                cart.Remove(id);
                HttpContext.Session.Set("Cart", cart);
            }

            return RedirectToAction(nameof(Index));
        }

        private Dictionary<int, int> GetCart()
        {
            var cart = HttpContext.Session.Get<Dictionary<int, int>>("Cart");
            if (cart == null)
            {
                cart = new Dictionary<int, int>();
                HttpContext.Session.Set("Cart", cart);
            }
            return cart;
        }
        public IActionResult IncreaseCartItem(int id)
        {
            var cart = GetCart();
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product != null && cart.ContainsKey(id))
            {
                if (cart[id] < product.StockQuantity)
                {
                    cart[id]++;
                    HttpContext.Session.Set("Cart", cart);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DecreaseCartItem(int id)
        {
            var cart = GetCart();
            if (cart.ContainsKey(id))
            {
                if (cart[id] > 1)
                {
                    cart[id]--;
                }
                else
                {
                    cart.Remove(id);
                }
                HttpContext.Session.Set("Cart", cart);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}