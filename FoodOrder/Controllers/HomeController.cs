using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FoodOrder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FoodOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lastRecord = _context.Order.MaxAsync(o => o.Id).Result;

            if (_context.Order.FirstOrDefaultAsync(o => o.Id == lastRecord).Result.IsPayed)
            {
                return RedirectToAction("LastOrder", "Order");
            }
            return View(await _context.Product.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
