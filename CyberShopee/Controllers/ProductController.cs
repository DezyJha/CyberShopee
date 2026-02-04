//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using CyberShopee.Models;
//using System.Threading.Tasks;
//using CyberShopee.ViewModels.ProducCategory;
//namespace CyberShopee.Controllers
//{
//    public class ProductController : Controller
//    {
//        private readonly CyberShopperDbContext _context;

//        public ProductController(CyberShopperDbContext context)
//        {
//            _context = context;
//        }

//        // GET: /Product
//        public async Task<IActionResult> Index()
//        {
//            var products = await _context.Products
//                                          .Include(p => p.Category) // Include the Category entity
//                                          .ToListAsync();
//            return View(products);
//        }
//        public async Task<IActionResult> ByCategory(int id)
//        {
//            var products = await _context.Products
//                                          .Where(p => p.CategoryId == id)
//                                          .Include(p => p.Category) // Include the Category entity
//                                          .ToListAsync();

//            var category = (from o in _context.Categories
//                            where o.CategoryId == id
//                            select o.CategoryName).FirstOrDefault();

//            var productCategoryModel = new ProductCategoryViewModel
//            {
//                Products = products,
//                Category=category
//            };

//            return View(productCategoryModel);
//        }

//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CyberShopee.Models;
using System.Threading.Tasks;
using CyberShopee.ViewModels.ProducCategory;
namespace CyberShopee.Controllers
{
    public class ProductController : Controller
    {
        private readonly CyberShopperDbContext _context;

        public ProductController(CyberShopperDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                                          .Include(p => p.Category) // Include the Category entity
                                          .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> ByCategory(int id)
        {
            var products = await _context.Products
                                          .Where(p => p.CategoryId == id)
                                          .Include(p => p.Category) // Include the Category entity
                                          .ToListAsync();

            var category = (from o in _context.Categories
                            where o.CategoryId == id
                            select o.CategoryName).FirstOrDefault();

            var productCategoryModel = new ProductCategoryViewModel
            {
                Products = products,
                Category = category
            };

            return View(productCategoryModel);
        }

    }
}