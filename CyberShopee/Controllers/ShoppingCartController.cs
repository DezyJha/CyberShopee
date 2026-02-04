using Microsoft.AspNetCore.Mvc;

using CyberShopee.Models;

using Microsoft.AspNetCore.Http;

using CyberShopee.ViewModels.Checkout;

using Microsoft.EntityFrameworkCore;

using CyberShopee.ViewModels.Cart;

using System.Net.Http;

using System.Text.Json;

using System.Text;

using Newtonsoft.Json;


namespace CyberShopee.Controllers

{

    public class ShoppingCartController : Controller

    {

        private readonly CyberShopperDbContext _context;

        private readonly IHttpClientFactory _httpClientFactory;

        public ShoppingCartController(CyberShopperDbContext context, IHttpClientFactory httpClientFactory)

        {

            _context = context;

            _httpClientFactory = httpClientFactory;

        }

        [HttpPost]

        public IActionResult AddToCart(int productId)

        {

            if (HttpContext.Session.GetString("CartId") != null)

            {

                var cartItem = new ShoppingCart

                {

                    CartId = HttpContext.Session.GetString("CartId"),

                    ProductId = productId,

                    Quantity = 1, // Default quantity

                    DateCreated = DateTime.Now

                };

                _context.ShoppingCarts.Add(cartItem);

            }

            else

            {

                var cartId = HttpContext.Session.GetString("CartId") ?? Guid.NewGuid().ToString();

                HttpContext.Session.SetString("CartId", cartId);

                var cartItem = new ShoppingCart

                {

                    CartId = cartId,

                    ProductId = productId,

                    Quantity = 1, // Default quantity

                    DateCreated = DateTime.Now

                };

                _context.ShoppingCarts.Add(cartItem);

            }




            _context.SaveChanges();

            return RedirectToAction("Cart");

        }

        [HttpGet]

        public IActionResult Cart()

        {

            var cartId = HttpContext.Session.GetString("CartId");

            if (string.IsNullOrEmpty(cartId))

            {

                return View(new CartViewModel { CartItems = Enumerable.Empty<ShoppingCart>(), ProductsInCart = Enumerable.Empty<Product>() });

            }

            var cartItems = _context.ShoppingCarts

                .Where(c => c.CartId == cartId)

                .ToList();

            var productIds = cartItems.Select(c => c.ProductId).ToList();

            var productsInCart = _context.Products

                .Where(p => productIds.Contains(p.ProductId))

                .ToList();

            var viewModel = new CartViewModel

            {

                CartItems = cartItems,

                ProductsInCart = productsInCart

            };

            return View(viewModel);

        }

        [HttpPost]

        public IActionResult UpdateCart(int cartItemId, string operation)

        {

            var cartItem = _context.ShoppingCarts.FirstOrDefault(c => c.RecordId == cartItemId);

            if (cartItem != null)

            {

                if (operation == "increase")

                {

                    cartItem.Quantity += 1;

                }

                else if (operation == "decrease" && cartItem.Quantity > 1)

                {

                    cartItem.Quantity -= 1;

                }

                _context.SaveChanges();

            }

            return RedirectToAction("Cart");

        }

        [HttpPost]

        public IActionResult RemoveFromCart(int cartItemId)

        {

            var cartItem = _context.ShoppingCarts.FirstOrDefault(c => c.RecordId == cartItemId);

            if (cartItem != null)

            {

                _context.ShoppingCarts.Remove(cartItem);

                _context.SaveChanges();

            }

            return RedirectToAction("Cart");

        }

        public async Task<IActionResult> Checkout()  // Change from async IActionResult to async Task<IActionResult>

        {

            var cartId = HttpContext.Session.GetString("CartId");

            if (string.IsNullOrEmpty(cartId))

            {

                return RedirectToAction("Cart");

            }

            var cartItems = await _context.ShoppingCarts.Where(c => c.CartId == cartId).ToListAsync(); // Ensure async call

            if (!cartItems.Any())

            {

                return RedirectToAction("Cart");

            }

            var viewModel = new CheckoutViewModel

            {

                CartItems = cartItems,

                ProductsInCart = await _context.Products

                    .Where(p => cartItems.Select(c => c.ProductId).Contains(p.ProductId))

                    .ToListAsync(), // Ensure async call

            };

            var authJSON = new

            {

                OrderDate = DateTime.Now.ToString("ddMMyyyy").ToString(),

                ShippingDate = DateTime.Now.AddDays(7).ToString("ddMMyyyy").ToString()

            };

            string json = System.Text.Json.JsonSerializer.Serialize(authJSON);

            using (var client = new HttpClient())

            {

                client.Timeout = TimeSpan.FromSeconds(30);

                var validateUrl = "https://prod-14.northcentralus.logic.azure.com:443/workflows/18f736bbc28c4c95a0291c5cc2c1ca3e/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=6WfGkdMQwfChve9tnXZ1VPTdqy0ZJh0oyNZwTYkwN1Q";

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(validateUrl, content); // Await the async call

            }

            return View(viewModel);

        }



        [HttpPost]

        public async Task<IActionResult> FinalizeOrder(int customerId, string paymentMethod, string cart)

        {

            var cartId = HttpContext.Session.GetString("CartId");


            if (string.IsNullOrEmpty(cartId))

            {

                return RedirectToAction("Cart");

            }

            var cartItems = _context.ShoppingCarts.Where(c => c.CartId == cartId).ToList();

            if (!cartItems.Any())

            {

                return RedirectToAction("Cart");

            }

            var order = new Order

            {

                CustomerId = int.Parse(HttpContext.Session.GetString("CustomerId")),

                OrderDate = DateTime.Now,

                ShipDate = DateTime.Now.AddDays(7)

            };

            _context.Orders.Add(order);

            _context.SaveChanges();

            foreach (var cartItem in cartItems)

            {

                // Fetch the product details

                var productItem = _context.Products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);

                if (productItem != null)

                {

                    var orderItem = new OrderDetails

                    {

                        /*OrderId = order.OrderId*/

                        ProductId = cartItem.ProductId,

                        Quantity = cartItem.Quantity,

                        UnitCost = productItem.UnitCost

                    };

                    _context.OrderDetails.Add(orderItem);

                }

            }

            _context.ShoppingCarts.RemoveRange(cartItems);

            _context.SaveChanges();

            var model = JsonConvert.DeserializeObject<CheckoutViewModel>(cart);

            // Calculate total amount

            decimal totalAmount = model.CartItems.Sum(item =>

                item.Quantity * model.ProductsInCart.FirstOrDefault(p => p.ProductId == item.ProductId)?.UnitCost ?? 0);


            var oDate = (from o in _context.Orders

                         where o.CustomerId == customerId

                         select order.OrderDate).FirstOrDefault();

            Console.WriteLine(oDate);

            var orderData = new

            {

                CustomerEmailId = HttpContext.Session.GetString("EmailId"),

                CustomerName = HttpContext.Session.GetString("FullName"),

                PaymentMethod = paymentMethod,

                CartItems = model.CartItems.Select(cartItem => new

                {

                    ProductId = cartItem.ProductId,

                    ProductName = model.ProductsInCart.FirstOrDefault(p => p.ProductId == cartItem.ProductId)?.ModelName,

                    Quantity = cartItem.Quantity,

                    UnitCost = model.ProductsInCart.FirstOrDefault(p => p.ProductId == cartItem.ProductId)?.UnitCost,

                    Subtotal = cartItem.Quantity * (model.ProductsInCart.FirstOrDefault(p => p.ProductId == cartItem.ProductId)?.UnitCost ?? 0)

                }),

                TotalAmount = totalAmount,

                OrderDate = oDate.ToString("yyyy-MM-ddTHH:mm:ss"),

                ShippedDate = oDate.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss"),

            };

            // Serialize order data to JSON

            var jsonContent = new StringContent(JsonConvert.SerializeObject(orderData), Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();

            string logicAppUrl = "https://prod-01.eastus.logic.azure.com:443/workflows/2cfab74b80a54110ba61f578ae9b05d3/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=13mqTe7Toavy-TX00jA4wojfwUZkyvdouTMd0nSP7uA"; // Your Logic App URL here

            var response = await client.PostAsync(logicAppUrl, jsonContent);

            if (!response.IsSuccessStatusCode)

            {

                return RedirectToAction("OrderConfirmation", "ShoppingCart");

            }

            else

            {

                ModelState.AddModelError("", "There was an issue finalizing the order.");

                return View(model);

            }

        }

        [HttpGet]

        public IActionResult OrderConfirmation()

        {


            return View();

        }

    }

}