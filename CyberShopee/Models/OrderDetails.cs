using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberShopee.Models
{
    public class OrderDetails
    {
        //[Key]
        //public int OrderDetailsId { get; set; }
        [Key]

        public int OrderId { get; set; }

       
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitCost { get; set; }
    }
}
