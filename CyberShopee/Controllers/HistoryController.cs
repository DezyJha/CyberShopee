using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CyberShopee.Models;
using CyberShopee.ViewModels;
using System.Linq;

namespace CyberShopee.Controllers
{
    public class HistoryController : Controller
    {
        private readonly CyberShopperDbContext _context;

        public HistoryController(CyberShopperDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult History()
        {
            // Get the customerId from session
            var userId = HttpContext.Session.GetString("CustomerId");

            // Redirect to login if the user is not logged in
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch orders for the logged-in user
            var orders = _context.Orders
                .Where(o => o.CustomerId == int.Parse(userId))  // Fetch orders for the current user
                .ToList();

            if (orders.Count == 0)
            {
                return View("NoOrders"); // Optionally render a view if there are no orders
            }

            // Fetch order details for the fetched orders
            var orderDetails = _context.OrderDetails
                .Where(od => orders.Select(o => o.OrderId).Contains(od.OrderId))
                .ToList();

            // Get the product ids from the order details
            var productIds = orderDetails.Select(od => od.ProductId).Distinct().ToList();

            // Fetch products that are part of the order details
            var products = _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToList();

            // Prepare the ViewModel
            var viewModel = new AccountHistoryViewModel
            {
                Orders = orders,
                OrderDetails = orderDetails,
                Products = products
            };

            // Return the ViewModel to the view
            return View(viewModel);
        }
    }
}
