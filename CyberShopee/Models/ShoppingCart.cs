using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CyberShopee.Models;

public class ShoppingCart
{
    [Key]
    public int RecordId { get; set; }

    [Required]
    public string CartId { get; set; } // Changed to string for Session-based CartId

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}

