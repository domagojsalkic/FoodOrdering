using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.ViewModels;

namespace FoodOrder.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Details),"Order");
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        //GET: OrderItems/Create
        public IActionResult Create(int? id)
        {
            var orderItem = new OrderItem()
            {
                ProductId = _context.Product.FirstOrDefault(o => o.Id == id).Id,
                Product = _context.Product.FirstOrDefault(o => o.Id == id),
                OrderId = _context.Order.OrderByDescending(o => o.Id).FirstOrDefault().Id,
                Order = _context.Order.OrderByDescending(o => o.Id).FirstOrDefault(),
                Amount = 1
            };
            ViewData["OrderId"] = new SelectList(_context.Set<Order>(), "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name");
            return View(orderItem);
        }

        // POST: OrderItems/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,OrderId,Amount")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            ViewData["OrderId"] = new SelectList(_context.Set<Order>(), "Id", "Id", orderItem.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", orderItem.ProductId);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem.Include(o => o.Order).Include(p => p.Product).Where(o=>o.Id == id).FirstAsync();
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Set<Order>(), "Id", "Id", orderItem.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", orderItem.ProductId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,OrderId,Amount")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("LastOrder","Order");
            }
            ViewData["OrderId"] = new SelectList(_context.Set<Order>(), "Id", "Id", orderItem.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Name", orderItem.ProductId);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItem.FindAsync(id);
            _context.OrderItem.Remove(orderItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("LastOrder","Order");
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItem.Any(e => e.Id == id);
        }
    }
}
