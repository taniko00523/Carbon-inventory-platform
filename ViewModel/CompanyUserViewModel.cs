using Carbon_inventory_platform.Models;

namespace Carbon_inventory_platform.ViewModel
{
    public class CompanyUserViewModel 
    {
        public Company? Company { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public string? Password { get; set; }
        public DateTime? UserLimitData { get; set; }
    }
}
