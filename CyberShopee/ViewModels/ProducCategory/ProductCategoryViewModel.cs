using CyberShopee.Models;
namespace CyberShopee.ViewModels.ProducCategory
{
    public class ProductCategoryViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public string Category { get; set; }
    }
}
