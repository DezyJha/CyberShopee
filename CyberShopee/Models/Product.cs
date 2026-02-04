using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberShopee.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [ForeignKey("CategoryId")]

        public int CategoryId { get; set; }

        [MaxLength(50)]
        public string ModelNumber { get; set; }

        [MaxLength(100)]
        public string ModelName { get; set; }

        [Required]
        public decimal UnitCost { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; }
    }
}
