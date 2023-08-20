using Carbon_inventory_platform.Data;
using Microsoft.AspNetCore.Mvc;

namespace Carbon_inventory_platform.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DeviceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Devices);
        }
    }
}
