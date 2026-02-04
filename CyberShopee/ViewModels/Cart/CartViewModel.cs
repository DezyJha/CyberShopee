using CyberShopee.Models;

namespace CyberShopee.ViewModels.Cart
{
    public class CartViewModel
    {
        public IEnumerable<ShoppingCart> CartItems { get; set; }  // Cart items
        public IEnumerable<Product> ProductsInCart { get; set; }
    }
}
