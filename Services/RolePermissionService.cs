using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Services
{
    public class RolePermissionService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RolePermissionService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<UserPermission>> GetPermissionAsync(string? userId)
        {
            List<UserPermission> userPermissions = new List<UserPermission>();
            if (userId == null) { return userPermissions; }
            userPermissions = await _context.UserPermissions
                .Where(x => x.UserId == userId)
                .Include(x => x.Permissions.Where(p => p.FunctionAction.Name == "index"))
                .ThenInclude(p => p.Function)
                .ToListAsync();

            return userPermissions;
        }
    }

}
