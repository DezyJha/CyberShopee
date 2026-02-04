using System.ComponentModel.DataAnnotations;

namespace CyberShopee.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; }
    }
}
