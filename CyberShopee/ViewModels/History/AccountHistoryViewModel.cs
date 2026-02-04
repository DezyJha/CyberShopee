using CyberShopee.Models;

namespace CyberShopee.ViewModels
{
    public class AccountHistoryViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
