using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CyberShopee.Models;
using System.Threading.Tasks;

namespace CyberShopee.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CyberShopperDbContext _context;

        public CategoryController(CyberShopperDbContext context)
        {
            _context = context;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
    }
}

