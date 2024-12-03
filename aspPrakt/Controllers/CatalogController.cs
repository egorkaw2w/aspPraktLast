using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspPrakt.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace aspPrakt.Controllers
{
    public class CatalogController : Controller
    {
        private readonly BpnContext _context;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(BpnContext context, ILogger<CatalogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId, string sortOrder)
        {
            var products = from p in _context.Products
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            var categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Categories = categories;

            return View(await products.ToListAsync());
        }
        public IActionResult AddToCart(int id)
        {
            return RedirectToAction("AddToCart", "Cart", new { id });
        }
    }
    }
