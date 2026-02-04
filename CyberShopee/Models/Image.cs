using System.ComponentModel.DataAnnotations;

namespace CyberShopee.Models
{
    public class Image
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }
    }
}
