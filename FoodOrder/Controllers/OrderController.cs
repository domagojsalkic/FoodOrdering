﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrder.Data;
using FoodOrder.Models;
using FoodOrder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(Order order)
        {
            order = new Order();
            await _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int? id)
        {
            var orderVM = new OrderViewModel()
            {
                Order = await _context.Order.FirstOrDefaultAsync(x => x.Id == id),
                orderItems = await _context.OrderItem.Include(o => o.Product).Where(o => o.OrderId == id).ToListAsync()
            };
            foreach (var item in orderVM.orderItems)
            {
                orderVM.Order.TotalPrice += item.Amount * item.Product.Price;
            }
            return View(orderVM);
        }

        public async Task<IActionResult> LastOrder()
        {
            var lastRecord = await _context.Order.MaxAsync(o => o.Id);

            if (_context.Order.FirstOrDefaultAsync(o => o.Id == lastRecord).Result.IsPayed)
            {
                return RedirectToAction(nameof(Index));
            }
            var orderVM = new OrderViewModel()
            {
                Order = await _context.Order.FirstOrDefaultAsync(x => x.Id == lastRecord),
                orderItems = _context.OrderItem.Include(o => o.Product).Where(o => o.OrderId == lastRecord).ToList()
            };
            foreach (var item in orderVM.orderItems)
            {
                orderVM.Order.TotalPrice += item.Amount * item.Product.Price;
            }
            return View(orderVM);
        }


        public async Task<IActionResult> AllOrders()
        {
            AllOrdersViewModel allOrdersVM = new AllOrdersViewModel()
            {
                Checks = await _context.Check.Include(o => o.Order).Include(o => o.Payment).ToListAsync(),
                Orders = await TotalPrice()
            };
            return View(allOrdersVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Order");
        }

        private async Task<List<Order>> TotalPrice()
        {
            List<Order> orders = await _context.Order.Include(o => o.OrderItems).ToListAsync();
            foreach (var order in orders)
            {
                order.OrderItems = await _context.OrderItem.Include(o => o.Product).Where(o => o.OrderId == order.Id).ToListAsync();
                foreach (var item in order.OrderItems)
                {
                    order.TotalPrice += item.Amount * item.Product.Price;
                }
            }
            return orders;
        }
    }
}