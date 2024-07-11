using Carbon_inventory_platform.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carbon_inventory_platform.ViewModel
{
    public class DeviceInformationViewModel
    {
        public Device Device { get; set; }
        public List<GHG> GHG { get; set; }
        public List<ActivityData> ActivityDataList { get; set; }
    }
}
