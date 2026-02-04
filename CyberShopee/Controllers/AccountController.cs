using CyberShopee.Models;
using CyberShopee.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace CyberShopee.Controllers
{
    public class AccountController : Controller
    {
        private readonly CyberShopperDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(CyberShopperDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user already exists
                var existingUser = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == model.EmailAddress);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email address already in use.");
                    return View(model);
                }

                // Create a new customer
                var customer = new Customer
                {
                    FullName = model.FullName,
                    EmailAddress = model.EmailAddress,
                    Password = model.Password, // Consider hashing the password
                    DeliveryAddress = model.DeliveryAddress
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                // Redirect to login after successful registration
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user exists and the password matches
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.EmailAddress == model.EmailAddress && c.Password == model.Password);

                if (customer != null)
                {
                    // If customer is found, set session values
                    HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                    HttpContext.Session.SetString("FullName", customer.FullName);
                    HttpContext.Session.SetString("EmailId", customer.EmailAddress);
                    // Redirect to home page or desired page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
