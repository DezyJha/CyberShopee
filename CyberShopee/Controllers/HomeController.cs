using System.Diagnostics;
using CyberShopee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberShopee.Controllers
{
    public class HomeController : Controller
    {
        private readonly CyberShopperDbContext _context;

        public HomeController(CyberShopperDbContext context)
        {
            _context = context;
        }
       

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);

        }

        public IActionResult Privacy()
        {

            return View();

        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        //public async Task<IActionResult> ByCategory(int id)
        //{
        //    var products = await _context.Products
        //                                  .Where(p => p.CategoryId == id)
        //                                  .Include(p => p.Category) // Include the Category entity
        //                                  .ToListAsync();
        //    return View(products);
        //}
    }
}
