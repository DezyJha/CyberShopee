// CheckoutViewModel.cs
using System.Collections.Generic;
using CyberShopee.Models;

namespace CyberShopee.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        public IEnumerable<ShoppingCart> CartItems { get; set; }
        public IEnumerable<Product> ProductsInCart { get; set; }
    }
}

