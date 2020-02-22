using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodOrder.Data;
using FoodOrder.Models;

namespace FoodOrder.Controllers
{
    public class ChecksController : Controller
    {
        private readonly AppDbContext _context;

        public ChecksController(AppDbContext context)
        {
            _context = context;
        }



        public IActionResult Create(int? id)
        {
            ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "PaymentMethod");
            var check = TotalPrice(id);
            return View(check);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId","PaymentId")]Check check)
        {
            Order order = await _context.Order.FirstOrDefaultAsync(o => o.Id == check.OrderId);
            order.IsPayed = true;
            _context.Update(order);
            if (ModelState.IsValid)
            {
                _context.Add(check);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Order");
            }

            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", check.OrderId);
            ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "Id", check.PaymentId);
            return View(check);
        }

        private Check TotalPrice(int? id)
        {
            var check = new Check()
            {
                Order = _context.Order.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == id),
            };
            check.Order.OrderItems = _context.OrderItem.Include(p => p.Product).Where(o => o.OrderId == id).ToList();
            foreach (var item in check.Order.OrderItems)
            {
                check.Order.TotalPrice += item.Product.Price * item.Amount;
            }

            return check;
        }
    }
}

