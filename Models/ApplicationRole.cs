﻿using Microsoft.AspNetCore.Identity;

namespace Carbon_inventory_platform.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}