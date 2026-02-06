using CyberShopee.Models;
using CyberShopee.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CyberShopee.Controllers
{
    public class AccountController : Controller
    {
        private readonly CyberShopperDbContext _context;
        private readonly PasswordHasher<Customer> _passwordHasher;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            CyberShopperDbContext context,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _passwordHasher = new PasswordHasher<Customer>();
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
            if (!ModelState.IsValid)
                return View(model);

            // Check if email already exists
            if (await _context.Customers.AnyAsync(c => c.EmailAddress == model.EmailAddress))
            {
                ModelState.AddModelError("", "Email address already in use.");
                return View(model);
            }

            // Create Customer
            var customer = new Customer
            {
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                DeliveryAddress = model.DeliveryAddress
            };

            // Hash password
            customer.Password = _passwordHasher.HashPassword(customer, model.Password);

            // Save to DB
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
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
            if (!ModelState.IsValid)
                return View(model);

            // Find customer by email
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.EmailAddress == model.EmailAddress);

            if (customer == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Try hashed, fallback to plain-text auto migration
            PasswordVerificationResult verify;

            try
            {
                verify = _passwordHasher.VerifyHashedPassword(customer, customer.Password, model.Password);
            }
            catch
            {
                if (customer.Password == model.Password)
                {
                    // Auto-migrate legacy password
                    customer.Password = _passwordHasher.HashPassword(customer, model.Password);
                    await _context.SaveChangesAsync();
                    verify = PasswordVerificationResult.Success;
                }
                else
                {
                    verify = PasswordVerificationResult.Failed;
                }
            }

            if (verify == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Store session values
            HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
            HttpContext.Session.SetString("FullName", customer.FullName);
            HttpContext.Session.SetString("EmailId", customer.EmailAddress);

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            // Logout Identity (if used anywhere)
            await _signInManager.SignOutAsync();

            // Clear session
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}