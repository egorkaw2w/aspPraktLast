using aspPrakt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace aspPrakt.Controllers
{
    public class HomeController : Controller
    {
        private readonly BpnContext _pnContext;

        public HomeController(BpnContext pnContext)
        {
            _pnContext = pnContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}