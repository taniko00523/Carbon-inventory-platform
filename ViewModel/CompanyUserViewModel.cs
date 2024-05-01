using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.ViewModel
{
    public class CompanyUserViewModel 
    {
        public Company Company { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
