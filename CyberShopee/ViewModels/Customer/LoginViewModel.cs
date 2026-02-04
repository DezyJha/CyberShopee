using System.ComponentModel.DataAnnotations;

namespace CyberShopee.ViewModels.Customer
{
    public class LoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

