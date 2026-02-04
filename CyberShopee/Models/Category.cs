using System.ComponentModel.DataAnnotations;

namespace CyberShopee.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        public string Description { get; set; }
    }
}
