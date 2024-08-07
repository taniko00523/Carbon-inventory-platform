using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.ViewModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class DevicesController : CountController
    {
        private readonly ApplicationDbContext _context;
        public DevicesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Guid Id)
        {
            TempData["areaId"] = Id; // 暫存目前所在的廠區ID
            var AreaData = await _context.Areas.Where(x => x.Id == Id && x.isDeleted == 0).Include(x => x.Company).FirstOrDefaultAsync();
            if (AreaData == null)
            {
                return View();
            }
            TempData["areaName"] = AreaData.Name;
            if (AreaData.Company != null)
            {
                TempData["companyName"] = AreaData.Company.Name.Trim() != "" ? AreaData.Company.Name : "未填寫公司資料";
            }

            TempData["year"] = AreaData.Year;
            return _context.Devices != null ? //如果有抓到資料表Null
                         View(await _context.Devices
                         .Where(x => x.isDeleted == 0 && x.AreaId == Id) //抓出資料表裡面沒被刪除的
                         .Include(x => x.GHGs)
                         .Include(x => x.ActivityDatas)
                         .Include(x => x.Area.Company)
                         .OrderBy(x => x.Name)
                         .ThenBy(x => x.CreateTime)
                         .ToListAsync()) : //非同步方法
                         Problem("沒有找到資料表"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }
        public async Task<IActionResult> Create()
        {
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["AreasId"] = new SelectList(await _context.Areas.Where(x => x.isDeleted == 0).ToListAsync(), "Id", "Name");
            ViewData["Scope"] = new SelectList(new List<string> { "類別一", "類別二", "類別三", "類別四", "類別五", "類別六" });

            //ViewData["EmissionPattern"] = new SelectList(emissionPattern);

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AreaId,AssetNo,Name,OtherName,Provess,Scope,EmissionPattern,Material,Customize,CO2CEF,CH4CEF,N2OCEF,HFCSCEF,PFCSCEF,NF3CEF,SF6CEF,NameRemark")] DeviceViewModel device)
        {
            if (ModelState.IsValid)
            {
                var deviceId = Guid.NewGuid();
                var toCreate = new Device
                {
                    Id = deviceId,
                    AreaId = device.AreaId,
                    AssetNo = device.AssetNo,
                    Name = device.Name,
                    OtherName = device.OtherName,
                    NameRemark = device.NameRemark,
                    Provess = device.Provess,
                    Scope = device.Scope,
                    EmissionPattern = device.EmissionPattern,
                    Material = device.Material,
                    Customize = device.Customize,
                    isDeleted = 0,
                    CreateTime = DateTime.Now,
                };
                _context.Add(toCreate);
                await _context.SaveChangesAsync();

                if (device.Customize == true) // 自訂排放係數被勾選
                {
                    if (device.CO2CEF != null || // 至少有一個欄位不為 null，執行原有的程式碼
                        device.CH4CEF != null ||
                        device.N2OCEF != null ||
                        device.HFCSCEF != null ||
                        device.PFCSCEF != null ||
                        device.SF6CEF != null ||
                        device.NF3CEF != null)
                    {
                        int ARVersion = await _context.Areas.Where(x => x.Id == device.AreaId).Select(x => x.ARVersion).FirstOrDefaultAsync();

                        if (device.CO2CEF != null && device.CO2CEF != 0)
                        {
                            await CEFAddAsync(toCreate, "CO2", device.CO2CEF);
                        }
                        if (device.CH4CEF != null && device.CH4CEF != 0)
                        {
                            await CEFAddAsync(toCreate, "CH4", device.CH4CEF);
                        }
                        if (device.N2OCEF != null && device.N2OCEF != 0)
                        {
                            await CEFAddAsync(toCreate, "N2O", device.N2OCEF);
                        }
                        if (device.HFCSCEF != null && device.HFCSCEF != 0)
                        {
                            await CEFAddAsync(toCreate, "HFCS", device.HFCSCEF);
                        }
                        if (device.PFCSCEF != null && device.PFCSCEF != 0)
                        {
                            await CEFAddAsync(toCreate, "PFCS", device.PFCSCEF);
                        }
                        if (device.NF3CEF != null && device.NF3CEF != 0)
                        {
                            await CEFAddAsync(toCreate, "NF3", device.NF3CEF);
                        }
                        if (device.SF6CEF != null && device.SF6CEF != 0)
                        {
                            await CEFAddAsync(toCreate, "SF6", device.SF6CEF);
                        }
                    }
                }
                else
                {

                    await GHGCheckAsync(deviceId, device.Name, device.Material, device.Scope, device.EmissionPattern, _context.Areas.FirstOrDefault(x => x.Id == device.AreaId).Year);
                }

                return RedirectToAction("Index", "Devices", new { id = device.AreaId });
            }
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(new List<string> { "類別一", "類別二", "類別三", "類別四", "類別五", "類別六" });

            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            return View(device);
        }
        public async Task<IActionResult> Copy(Guid? id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id);
            if (device == null)
            {
                return NotFound();
            }
            Guid newDeviceId = Guid.NewGuid();
            Guid areaId = device.AreaId;
            Device newDevice = CopyDevice(device,areaId,newDeviceId);
           
            _context.Devices.Add(newDevice);
            await _context.SaveChangesAsync();

            var GHGs = await _context.GHGs.Where(x => x.DeviceId == id).ToListAsync();
            if (GHGs != null)
            {
                foreach (var ghg in GHGs)
                {
                    var newGHG = CopyGHG(ghg, newDeviceId);
                    _context.GHGs.Add(newGHG);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Devices", new { id = areaId });
        }
        [HttpGet]
        public JsonResult GetDeviceLevelAndCorrection(string selectedName, Guid id)
        {
            var deviceData = _context.deviceDatas
       .Where(x => x.Name == selectedName)
       .FirstOrDefault();

            var activitydata = _context.ActivityDatas.Where(x => x.DeviceId == id).ToList();
            decimal all_activitydata = 0;
            foreach (var item in activitydata)
            {
                all_activitydata += item.Num;
            }

            var device = _context.Devices
                .Where(x => x.Id == id)
                .FirstOrDefault();

            // 假設有一個名為 emission 的屬性，將其包含在回應中
            if (device != null)
            {
                var result = new
                {
                    level = deviceData?.Data_Correction,
                    correction = deviceData?.Device_Correction,
                    unit = deviceData?.unit,
                    num = all_activitydata
                };
                return Json(result);
            }

            return Json(null);
        }
        [HttpGet]
        public JsonResult GetDefaultDeviceData(string selectedName)
        {
            var deviceData = _context.deviceDatas
                .Where(x => x.Name == selectedName)
                .FirstOrDefault();

            return Json(deviceData);
        }
        [HttpGet]
        public JsonResult GetDeviceData(Guid id)
        {
            var deviceData = _context.Devices
               .Where(x => x.Id == id)
               .FirstOrDefault();
            return Json(deviceData);
        }
        [HttpGet]
        public JsonResult GetMaterialsAndGWPNames(string emissionPattern)
        {
            List<object> gwpNames = new List<object>();
            List<object> materials = new List<object>();
            List<string> excludedOptions = new List<string>();
            Guid areaId = (Guid)TempData.Peek("areaId");
            var arVersion = _context.Areas.Where(x => x.Id == areaId).Select(x => x.ARVersion).FirstOrDefault();
            if (emissionPattern == "逸散")
            {
                excludedOptions = new List<string> {
                    "中、大型冰箱",
                    "乾燥機",
                    "低溫冷凍車",
                    "冰水主機",
                    "冰箱",
                    "冷氣機",
                    "商用冰箱",
                    "工業冷藏、冷凍",
                    "車用空調",
                    "食品加工冷藏、冷凍",
                    "飲水機"
                };

                gwpNames = _context.GWPs
                    .Where(x => x.ARCount == arVersion)
                    .Select(x => new { name = x.Name })
                    .Distinct()
                    .ToList<object>();
            }

            materials = _context.Materials
                .Where(x => x.EmissionPattern == emissionPattern)
                .Select(x => new { name = x.Name })
                .Distinct()
                .Where(x => !excludedOptions.Contains(x.name))
                .ToList<object>();

            return Json(new { gwpNames, materials });
        }
        [HttpGet]
        public JsonResult GetEmissions(string scope)
        {
            var emission = new List<string> { };
            // 根據scope的值抓取相對應的emission選項


            if (scope == "類別三")
            {
                emission.AddRange(new List<string> { "上游運輸和配送貨物", "下游運輸和配送貨物", "員工通勤", "客戶和訪客運輸", "商務旅行" });

            }
            else if (scope == "類別四")
            {
                emission.AddRange(new List<string> { "商品_輸入電力", "商品_輸入能源", "商品_燃料", "商品_資源", "商品_主要原料", "商品_輔助原料", "商品_包裝材料", "服務_不可回收廢棄物", "商品_資本貨物", "服務_租賃資產", "服務_其他" });
            }
            else if (scope == "類別五")
            {
                emission.AddRange(new List<string> { "產品加工", "產品使用", "下游租賃", "產品壽命終止處置", "投資" });
            }
            else if (scope == "類別六")
            {
                emission.AddRange(new List<string> { "維修與施工" });
            }
            else
            {
                emission = _context.Materials.Where(x => x.Scope == scope).Select(m => m.EmissionPattern).Distinct().ToList();
            }
            return Json(emission);
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            var GHGs = await _context.GHGs.Where(x => x.DeviceId == id).ToListAsync();
            var deviceViewModel = new DeviceViewModel
            {
                Id = device.Id,
                Name = device.Name,
                OtherName = device.OtherName,
                NameRemark = device.NameRemark,
                AreaId = device.AreaId,
                AssetNo = device.AssetNo,
                Provess = device.Provess,
                Scope = device.Scope,
                EmissionPattern = device.EmissionPattern,
                Material = device.Material,
                Customize = device.Customize
            };

            var CO2 = GHGs.Where(x => x.Name == "CO2").FirstOrDefault();
            if (CO2 != null)
            {
                deviceViewModel.CO2CEF = CO2.CEF;
            }

            var CH4 = GHGs.Where(x => x.Name == "CH4").FirstOrDefault();
            if (CH4 != null)
            {
                deviceViewModel.CH4CEF = CH4.CEF;
            }


            var N2O = GHGs.Where(x => x.Name == "N2O").FirstOrDefault();
            if (N2O != null)
            {
                deviceViewModel.N2OCEF = N2O.CEF;
            }


            var HFCS = GHGs.Where(x => x.Name == "HFCS").FirstOrDefault();
            if (HFCS != null)
            {
                deviceViewModel.HFCSCEF = HFCS.CEF;
            }

            var PFCS = GHGs.Where(x => x.Name == "PFCS").FirstOrDefault();
            if (PFCS != null)
            {
                deviceViewModel.PFCSCEF = PFCS.CEF;
            }

            var NF3 = GHGs.Where(x => x.Name == "NF3").FirstOrDefault();
            if (NF3 != null)
            {
                deviceViewModel.NF3CEF = NF3.CEF;
            }

            var SF6 = GHGs.Where(x => x.Name == "SF6").FirstOrDefault();
            if (SF6 != null)
            {
                deviceViewModel.SF6CEF = SF6.CEF;
            }



            if (device == null)
            {
                return NotFound();
            }
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(new List<string> { "類別一", "類別二", "類別三", "類別四", "類別五", "類別六" });
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            return View(deviceViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AreaId,AssetNo,Name,OtherName,Provess,Scope,EmissionPattern,Material,Customize,CO2CEF,CH4CEF,N2OCEF,HFCSCEF,PFCSCEF,NF3CEF,SF6CEF,NameRemark")] DeviceViewModel device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var deviceUpdate = await _context.Devices.FindAsync(id);
                    var ghgUpdate = await _context.GHGs.Where(x => x.DeviceId == device.Id).ToListAsync();

                    bool changeName = deviceUpdate.Name != device.Name ||
                                        device.OtherName != device.OtherName; //換排放源
                    bool changeMaterial = deviceUpdate.Material != device.Material ||  //換物料
                                            deviceUpdate.EmissionPattern != device.EmissionPattern;
                    bool changeIsComstomCEF = deviceUpdate.Customize != device.Customize; //換排放係數


                    if (deviceUpdate != null)
                    {
                        deviceUpdate.AreaId = device.AreaId;
                        deviceUpdate.AssetNo = device.AssetNo;
                        deviceUpdate.Name = device.Name;
                        deviceUpdate.OtherName = device.OtherName;
                        deviceUpdate.NameRemark = device.NameRemark;
                        deviceUpdate.Provess = device.Provess;
                        deviceUpdate.Scope = device.Scope;
                        deviceUpdate.EmissionPattern = device.EmissionPattern;
                        deviceUpdate.Material = device.Material;
                        deviceUpdate.ModifiedTime = DateTime.Now;
                        deviceUpdate.Customize = device.Customize;

                        if (changeIsComstomCEF || changeName || changeMaterial || deviceUpdate.Customize) // 換排放源 或 換物料 或 換排放係數是否自訂勾選選項 或 換排放係數
                        {
                            if (device.Customize == true) // 自訂排放係數
                            {
                                if (device.CO2CEF != null ||
                                    device.CH4CEF != null ||
                                    device.N2OCEF != null ||
                                    device.HFCSCEF != null ||
                                    device.PFCSCEF != null ||
                                    device.SF6CEF != null ||
                                    device.NF3CEF != null)
                                {
                                    if (ghgUpdate != null)
                                    {
                                        foreach (var item in ghgUpdate)
                                        {
                                            _context.GHGs.Remove(item);
                                        }
                                    }
                                    if (device.CO2CEF != null && device.CO2CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "CO2", device.CO2CEF);
                                    }
                                    if (device.CH4CEF != null && device.CH4CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "CH4", device.CH4CEF);
                                    }
                                    if (device.N2OCEF != null && device.N2OCEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "N2O", device.N2OCEF);
                                    }
                                    if (device.HFCSCEF != null && device.HFCSCEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "HFCS", device.HFCSCEF);
                                    }
                                    if (device.PFCSCEF != null && device.PFCSCEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "PFCS", device.PFCSCEF);
                                    }
                                    if (device.NF3CEF != null && device.NF3CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "NF3", device.NF3CEF);
                                    }
                                    if (device.SF6CEF != null && device.SF6CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "SF6", device.SF6CEF);
                                    }
                                }
                            }
                            else
                            {
                                if (ghgUpdate != null)
                                {
                                    foreach (var item in ghgUpdate)
                                    {
                                        _context.GHGs.Remove(item);
                                    }
                                }
                                await GHGCheckAsync(id, device.Name, device.Material, device.Scope, device.EmissionPattern, _context.Areas.FirstOrDefault(x => x.Id == device.AreaId).Year);
                            }
                        }
                        else
                        {
                            if (device.Customize == true) // 自訂排放係數
                            {
                                if (device.CO2CEF != null || // 至少有一個欄位不為 null，執行原有的程式碼
                                    device.CH4CEF != null ||
                                    device.N2OCEF != null ||
                                    device.HFCSCEF != null ||
                                    device.PFCSCEF != null ||
                                    device.SF6CEF != null ||
                                    device.NF3CEF != null)
                                {
                                    if (ghgUpdate != null)
                                    {
                                        foreach (var item in ghgUpdate)
                                        {
                                            _context.GHGs.Remove(item);
                                        }
                                    }
                                    int ARVersion = await _context.Areas.Where(x => x.Id == device.AreaId).Select(x => x.ARVersion).FirstOrDefaultAsync();
                                    if (device.CO2CEF != null && device.CO2CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "CO2", device.CO2CEF);
                                    }
                                    if (device.CH4CEF != null && device.CH4CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "CH4", device.CH4CEF);
                                    }
                                    if (device.N2OCEF != null && device.N2OCEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "N2O", device.N2OCEF);
                                    }
                                    if (device.HFCSCEF != null && device.HFCSCEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "HFCS", device.HFCSCEF);
                                    }
                                    if (device.PFCSCEF != null && device.PFCSCEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "PFCS", device.PFCSCEF);
                                    }
                                    if (device.NF3CEF != null && device.NF3CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "NF3", device.NF3CEF);
                                    }
                                    if (device.SF6CEF != null && device.SF6CEF != 0)
                                    {
                                        await CEFAddAsync(deviceUpdate, "SF6", device.SF6CEF);
                                    }
                                }
                            }
                        }

                    }
                    if (_context.ActivityDatas.Where(x => x.DeviceId == id) != null)
                    {
                        await CountEmissionData(id);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var areaId = device.AreaId;
                return RedirectToAction("Index", "Devices", new { id = areaId });
            }
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(new List<string> { "類別一", "類別二", "類別三", "類別四", "類別五", "類別六" });
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");

            return View(device);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Devices == null)
            {
                return Problem("沒有找到資料");
            }
            var toDeleteDevice = await _context.Devices.FindAsync(id);
            if (toDeleteDevice != null)
            {
                var emission = await _context.Areas.FindAsync(toDeleteDevice.AreaId);
                if (toDeleteDevice.ModifiedTime == null)
                {
                    if (emission != null)
                    {
                        emission.All -= toDeleteDevice.Emissions;
                        _context.Devices.Remove(toDeleteDevice);
                    }
                }
                else
                {
                    if (emission != null)
                    {
                        emission.All -= toDeleteDevice.Emissions;
                        toDeleteDevice.isDeleted = 1;
                        toDeleteDevice.DeleteTime = DateTime.Now;
                    }
                }
                await _context.SaveChangesAsync();

            }
            var areaId = toDeleteDevice.AreaId;
            return RedirectToAction("Index", "Devices", new { id = areaId });
        }
        private bool DeviceExists(Guid id)
        {
            return (_context.Devices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> AddActivityData(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return View();
            }
            var activityData = await _context.ActivityDatas.Where(x => x.DeviceId == id).ToListAsync();
            if (activityData == null || activityData.Count == 0)
            {
                activityData = new List<ActivityData> { new ActivityData() };
            }
            var viewModel = new ActivityDataViewModel
            {
                Device = device,
                ActivityDataList = activityData
            };
            var deviceCorrection = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "有外部校正或多組數據佐證者" },
                new SelectListItem { Value = "2", Text = "有內部校正或經過會計簽證等證明者" },
                new SelectListItem { Value = "3", Text = "未進行儀器校正或未進行紀錄彙整者" }
            };
            ViewData["DeviceCorrection"] = new SelectList(deviceCorrection, "Value", "Text");

            var dataCorrections = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "連續監測" },
                new SelectListItem { Value = "2", Text = "定期/間歇量測" },
                new SelectListItem { Value = "3", Text = "自行/財務推估" }
            };

            ViewData["DataCorrections"] = new SelectList(dataCorrections, "Value", "Text");
            string[] unit = { "公斤", "公升", "立方公尺", "度", "人-年", "其他" };
            ViewData["Unit"] = new SelectList(unit);
            string[] source = { "發票", "領用單", "紀錄表", "繳費單" };
            ViewData["Source"] = new SelectList(source);

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivityData(Guid? id, ActivityDataViewModel activityData)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = activityData.Device;
            if (device == null)
            {
                return View();
            }
            var Device = await _context.Devices.FindAsync(id);
            if (Device == null)
            {
                return View();
            }

            foreach (var data in activityData.ActivityDataList)
            {


                // 查找資料庫中是否已經存在該ActivityData
                var existingData = await _context.ActivityDatas.FirstOrDefaultAsync(x => x.id == data.id);

                if (existingData != null)
                {
                    // 如果已經存在，則將其更新
                    existingData.Num = data.Num;
                    existingData.Time = data.Time;
                    existingData.remark = data.remark;
                    _context.ActivityDatas.Update(existingData);
                }
                else
                {
                    //var maxId = await _context.ActivityDatas.MaxAsync(x => (int?)x.id) ?? 0;
                    // 如果不存在，則添加新的ActivityData
                    var toCreate = new ActivityData
                    {
                        //id = maxId + 1,
                        DeviceId = device.Id,
                        Num = data.Num,
                        Time = data.Time,
                        remark = data.remark,
                    };
                    _context.ActivityDatas.Add(toCreate);
                }
            }
            Device.Unit = device.Unit;
            Device.Data_Correction = device.Data_Correction;
            Device.Device_Correction = device.Device_Correction;
            Device.Grade = device.CEF_Correction * device.Data_Correction * device.Device_Correction;
            Device.Source = device.Source;
            await _context.SaveChangesAsync();

            await CountEmissionData(id);

            string[] unit = { "公斤", "公升", "立方公尺", "度", "人-年", "其他" };
            ViewData["Unit"] = new SelectList(unit);
            string[] source = { "發票", "領用單", "紀錄表", "繳費單" };
            ViewData["Source"] = new SelectList(source);
            var dataCorrections = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "有外部校正或多組數據佐證者" },
                new SelectListItem { Value = "2", Text = "有內部校正或經過會計簽證等證明者" },
                new SelectListItem { Value = "3", Text = "未進行儀器校正或未進行紀錄彙整者" }
            };

            ViewData["DataCorrection"] = new SelectList(dataCorrections, "Value", "Text");

            var dataLevel = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "連續監測" },
                new SelectListItem { Value = "2", Text = "定期/間歇量測" },
                new SelectListItem { Value = "3", Text = "自行/財務推估" }
            };

            ViewData["DataLevel"] = new SelectList(dataLevel, "Value", "Text");
            var areaId = TempData.Peek("areaId");
            return RedirectToAction("Index", "Devices", new { id = areaId });
        }
       
        public async Task<IActionResult> Default(Guid id)
        {
            var defaultDevice = await _context.defaultDevices.ToListAsync();
            foreach (var device in defaultDevice)
            {
                var deviceID = Guid.NewGuid();
                string Name = device.Name;
                string Material = device.Material;
                string Scope = device.Scope;
                string EmissionPattern = device.EmissionPattern;
                await _context.Devices.AddAsync(new Device()
                {
                    Id = deviceID,
                    AreaId = id,
                    Name = Name,
                    Material = Material,
                    Scope = Scope,
                    EmissionPattern = EmissionPattern,
                    CreateTime = DateTime.Now,
                });
                await _context.SaveChangesAsync();
                await GHGCheckAsync(deviceID, Name, Material, Scope, EmissionPattern, _context.Areas.FirstOrDefault(x => x.Id == id).Year);
            }

            var areaId = TempData.Peek("SelectedAreaId") as Guid?;


            return RedirectToAction(nameof(Index), new { id = id });
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailAsync(Guid Id)
        {
            var deviceInformation = new DeviceInformationViewModel();
            deviceInformation.Device = await _context.Devices.FindAsync(Id);
            deviceInformation.ActivityDataList = await _context.ActivityDatas.Where(x => x.DeviceId == Id).ToListAsync();
            deviceInformation.GHG = await _context.GHGs.Where(x => x.DeviceId == Id).ToListAsync();
            return View(deviceInformation);

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeviceExcelAsync(Guid id)
        {
            // 读取现有的Excel文件
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "test.xlsx");
            IWorkbook workbook;
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }

            // 抓取工作表
            ISheet basicDataSheet = workbook.GetSheetAt(workbook.GetSheetIndex("邊界資料"));
            ISheet deviesDataSheet = workbook.GetSheetAt(workbook.GetSheetIndex("排放源資料"));
            ISheet countingDataSheet = workbook.GetSheetAt(workbook.GetSheetIndex("排放量計算資料"));
            ISheet emissionDataSheet = workbook.GetSheetAt(workbook.GetSheetIndex("溫室氣體排放量彙總"));
            

            // 抓取列索引
            var basicDataIndexes = GetColumnIndexes(basicDataSheet.GetRow(0));
            var devicesDataIndexs = GetColumnIndexes(deviesDataSheet.GetRow(0));
            var coutingDataIndexs = GetColumnIndexes(countingDataSheet.GetRow(0));

            var devices = await _context.Devices.Where(x => x.AreaId == id && x.isDeleted == 0)
                .OrderBy(x => x.Scope)
                .ThenBy(x => x.EmissionPattern)
                .ToListAsync();
            if (devices == null) { return NotFound(); }
            var allGHGs = await _context.GHGs.Include(ghg => ghg.Device)
                .Where(ghg => ghg.Device.AreaId == id && ghg.Device.isDeleted == 0)
                .OrderBy(x => x.Device.Scope)
                .ThenBy(x => x.Device.EmissionPattern)
                .ToListAsync();
            var area = await _context.Areas
               .Where(x => x.Id == devices[0].AreaId)
               .Include(a => a.Devices)
               .ThenInclude(d => d.GHGs)
               .FirstOrDefaultAsync();
            var company = await _context.Companies
              .Where(x => x.Id == area.CompanyId)
              .FirstOrDefaultAsync();
            var activityDatas = _context.ActivityDatas.ToList();



            FillBasicData(basicDataSheet, company, area, basicDataIndexes);
            FillDeviceData(deviesDataSheet, devices,allGHGs, activityDatas, devicesDataIndexs); ;
            FillCountingData(countingDataSheet, allGHGs, coutingDataIndexs);
            FillEmissionData(emissionDataSheet, area);


            for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetIndex);
                sheet.ForceFormulaRecalculation = true;
            }
            // 将工作簿写入MemoryStream
            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                var bytes = exportData.ToArray();

                // 返回Excel文件
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", company.Name + "溫室氣體盤查清冊.xlsx");
            }
        }

        private Dictionary<string, int> GetColumnIndexes(IRow headerRow)
        {
            var columnIndexes = new Dictionary<string, int>();
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                string cellValue = headerRow.GetCell(i)?.ToString();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    columnIndexes[cellValue] = i;
                }
            }
            return columnIndexes;
        }

        private Dictionary<string, int> GetColumnIndexesSheet2(IRow headerRow)
        {
            var columnIndexes = new Dictionary<string, int>();
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                string cellValue = headerRow.GetCell(i)?.ToString();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    columnIndexes[cellValue] = i;
                }
            }
            return columnIndexes;
        }

        private void FillBasicData(ISheet sheet,Company company,Area area, Dictionary<string, int> columnIndexes)
        {
            IRow row = sheet.GetRow(1) ?? sheet.CreateRow(1);
            row.CreateCell(columnIndexes["公司名稱"]).SetCellValue(company.Name);
            row.CreateCell(columnIndexes["英文公司名稱"]).SetCellValue(company.EnglishName);
            row.CreateCell(columnIndexes["姓名"]).SetCellValue(company.ContactName); //聯絡人姓名
            row.CreateCell(columnIndexes["電話號碼"]).SetCellValue(company.Phone);
            row.CreateCell(columnIndexes["電子信箱"]).SetCellValue(company.Email);
            row.CreateCell(columnIndexes["地址"]).SetCellValue(area.Address);
            row.CreateCell(columnIndexes["工廠登記編號"]).SetCellValue(area.FactorCode);
            row.CreateCell(columnIndexes["統一編號"]).SetCellValue(area.UniqueCode);
            row.CreateCell(columnIndexes["盤查年度"]).SetCellValue(area.Year);
        }
        private void FillDeviceData(ISheet sheet, List<Device> devices, List<GHG> ghgs, List<ActivityData> activityDatas, Dictionary<string, int> columnIndexes)
        {
            for (int i = 0; i < devices.Count; i++)
            {
                IRow row = sheet.GetRow(i + 1) ?? sheet.CreateRow(i + 1);
                var device = devices[i];
                List<GHG> selectGHG = ghgs.Where(x => x.DeviceId == devices[i].Id).ToList();
                row.CreateCell(columnIndexes["排放源名稱"]).SetCellValue(device.Name);
                row.CreateCell(columnIndexes["類別"]).SetCellValue(device.Scope);
                row.CreateCell(columnIndexes["排放型式"]).SetCellValue(device.EmissionPattern);
                row.CreateCell(columnIndexes["原燃物料"]).SetCellValue(device.Material);
                row.CreateCell(columnIndexes["CO2排放當量"]).SetCellValue(device.Emissions.ToString());
                row.CreateCell(columnIndexes["數據來源名稱"]).SetCellValue(device.Source);

                FillCorrectionData(row, device, columnIndexes);
                FillActivityData(row, device, activityDatas, columnIndexes);
                FillGHGData(row, selectGHG, columnIndexes);
            }
        }

        private void FillCountingData(ISheet sheet, List<GHG> ghgs, Dictionary<string, int> columnIndexes)
        {
            for (int i = 0; i < ghgs.Count; i++)
            {
                IRow row = sheet.GetRow(i + 1) ?? sheet.CreateRow(i + 1);
                var ghg = ghgs[i];
                row.CreateCell(columnIndexes["類別"]).SetCellValue(ghg.Device.Scope);
                row.CreateCell(columnIndexes["排放型式"]).SetCellValue(ghg.Device.EmissionPattern);
                row.CreateCell(columnIndexes["排放源名稱"]).SetCellValue(ghg.Device.Name);
                row.CreateCell(columnIndexes["原燃物料"]).SetCellValue(ghg.Device.Material);
                row.CreateCell(columnIndexes["溫室氣體"]).SetCellValue(ghg.Name);
                row.CreateCell(columnIndexes["GWP值"]).SetCellValue(ghg.GWP.ToString());
                row.CreateCell(columnIndexes["排放係數"]).SetCellValue(ghg.CEF.ToString());
                row.CreateCell(columnIndexes["排放係數不確性上限"]).SetCellValue(ghg.CEF_UUL.ToString());
                row.CreateCell(columnIndexes["排放係數不確性下限"]).SetCellValue(ghg.CEF_ULL.ToString());
            }
        }
        private void FillEmissionData(ISheet sheet, Area data)
        {
            IRow allScopeRow = sheet.GetRow(3) ?? sheet.CreateRow(3);
            ReplaceTextInSheet(sheet, "[類別一CO2排放]", data.Scope1_CO2.ToString("N4"));
            ReplaceTextInSheet(sheet, "[類別一CH4排放]", data.Scope1_CH4.ToString("N4"));
            ReplaceTextInSheet(sheet, "[類別一N2O排放]", data.Scope1_N2O.ToString("N4"));
            ReplaceTextInSheet(sheet, "[類別一HFCS排放]", data.Scope1_HFCS.ToString("N4"));
            ReplaceTextInSheet(sheet, "[類別一PFCS排放]", data.Scope1_PFCS.ToString("N4"));
            ReplaceTextInSheet(sheet, "[類別一SF6排放]", data.Scope1_SF6.ToString("N4"));
            ReplaceTextInSheet(sheet, "[類別一NF3排放]", data.Scope1_NF3.ToString("N4"));

            ReplaceTextInSheet(sheet, "[CO2排放]", data.CO2.ToString("N4"));
            ReplaceTextInSheet(sheet, "[CH4排放]", data.CH4.ToString("N4"));
            ReplaceTextInSheet(sheet, "[N2O排放]", data.N2O.ToString("N4"));
            ReplaceTextInSheet(sheet, "[HFCS排放]", data.HFCS.ToString("N4"));
            ReplaceTextInSheet(sheet, "[PFCS排放]", data.PFCS.ToString("N4"));
            ReplaceTextInSheet(sheet, "[SF6排放]", data.SF6.ToString("N4"));
            ReplaceTextInSheet(sheet, "[NF3排放]", data.NF3.ToString("N4"));

           

            //ReplaceTextInSheet(sheet, "[範疇二CO2排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別二").Sum(x => x.GHGs.Where(g => g.Name == "CO2").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇二CH4排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別二").Sum(x => x.GHGs.Where(g => g.Name == "CH4").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇二N2O排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別二").Sum(x => x.GHGs.Where(g => g.Name == "N2O").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇二HFCS排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別二").Sum(x => x.GHGs.Where(g => g.Name == "HFCS").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇二PFCS排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別二").Sum(x => x.GHGs.Where(g => g.Name == "PFCS").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇二SF6排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別二").Sum(x => x.GHGs.Where(g => g.Name == "SF6").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇二NF3排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別二").Sum(x => x.GHGs.Where(g => g.Name == "NF3").FirstOrDefault()?.Emission ?? 0).ToString("N4")));

            //ReplaceTextInSheet(sheet, "[範疇三CO2排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope != "類別一" && x.Scope != "類別二").Sum(x => x.GHGs.Where(g => g.Name == "CO2").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇三CH4排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別一" && x.Scope != "類別二").Sum(x => x.GHGs.Where(g => g.Name == "CH4").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇三N2O排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別一" && x.Scope != "類別二").Sum(x => x.GHGs.Where(g => g.Name == "N2O").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇三HFCS排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別一" && x.Scope != "類別二").Sum(x => x.GHGs.Where(g => g.Name == "HFCS").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇三PFCS排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別一" && x.Scope != "類別二").Sum(x => x.GHGs.Where(g => g.Name == "PFCS").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇三SF6排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別一" && x.Scope != "類別二").Sum(x => x.GHGs.Where(g => g.Name == "SF6").FirstOrDefault()?.Emission ?? 0).ToString("N4")));
            //ReplaceTextInSheet(sheet, "[範疇三NF3排放]", (data.Devices.Where(x => x.isDeleted == 0 && x.Scope == "類別一" && x.Scope != "類別二").Sum(x => x.GHGs.Where(g => g.Name == "NF3").FirstOrDefault()?.Emission ?? 0).ToString("N4")));

            ReplaceTextInSheet(sheet, "[總排放當量]", data.All.ToString("N3"));
            ReplaceTextInSheet(sheet, "[固定排放量]", data.non_move.ToString("N4"));
            ReplaceTextInSheet(sheet, "[移動排放量]", data.move.ToString("N4"));
            ReplaceTextInSheet(sheet, "[製程排放量]", data.process.ToString("N4"));
            ReplaceTextInSheet(sheet, "[逸散排放量]", data.escape.ToString("N4"));
           
            ReplaceTextInSheet(sheet, "[類別一總排放]", data.Scope1.ToString("N4"));
            ReplaceTextInSheet(sheet, "[類別二總排放]", data.Devices.Where(x => x.Scope == "類別二").Sum(x => x.Emissions).ToString("N4"));
            ReplaceTextInSheet(sheet, "[進行評估排放當量]", data.cal_all.ToString("N4"));

            ReplaceTextInSheet(sheet, "[第1級個數]", data.no1_Grade.ToString());
            ReplaceTextInSheet(sheet, "[第2級個數]", data.no2_Grade.ToString());
            ReplaceTextInSheet(sheet, "[第3級個數]", data.no3_Grade.ToString());
            ReplaceTextInSheet(sheet, "[清冊等級分數]", data.avg_Grade.ToString());
            ReplaceTextInSheet(sheet, "[清冊級別]", data.all_Grade.ToString());
            ReplaceTextInSheet(sheet, "[95上]", "-" + data.UUL.ToString("N2") + "%");
            ReplaceTextInSheet(sheet, "[95下]", "+" + data.ULL.ToString("N2") + "%");
        }

        private void FillCorrectionData(IRow row, Device device, Dictionary<string, int> columnIndexes)
        {
            switch (device.CEF_Correction)
            {
                case 1:
                    row.CreateCell(columnIndexes["排放係數誤差等級"]).SetCellValue("自廠發展係數/質量平衡所得係數/同製程/設備經驗係數");
                    break;
                case 2:
                    row.CreateCell(columnIndexes["排放係數誤差等級"]).SetCellValue("製造商提供係數/區域排放係數");
                    break;
                case 3:
                    row.CreateCell(columnIndexes["排放係數誤差等級"]).SetCellValue("國家/國際排放係數");
                    break;
            }

            switch (device.Data_Correction)
            {
                case 1:
                    row.CreateCell(columnIndexes["活動數據誤差等級"]).SetCellValue("連續監測");
                    break;
                case 2:
                    row.CreateCell(columnIndexes["活動數據誤差等級"]).SetCellValue("定期/間歇量測");
                    break;
                case 3:
                    row.CreateCell(columnIndexes["活動數據誤差等級"]).SetCellValue("自行/財務推估");
                    break;
            }

            switch (device.Device_Correction)
            {
                case 1:
                    row.CreateCell(columnIndexes["儀器校正等級"]).SetCellValue("有外部校正或多組數據佐證者");
                    break;
                case 2:
                    row.CreateCell(columnIndexes["儀器校正等級"]).SetCellValue("有內部校正或經過會計簽證等證明者");
                    break;
                case 3:
                    row.CreateCell(columnIndexes["儀器校正等級"]).SetCellValue("為進行儀器校正或未進行紀錄彙整者");
                    break;
            }

            row.CreateCell(columnIndexes["數據等級評分"]).SetCellValue(device.Grade);
            row.CreateCell(columnIndexes["活動數據信賴區間上限"]).SetCellValue(device.data_UUL.ToString());
            row.CreateCell(columnIndexes["活動數據信賴區間下限"]).SetCellValue(device.data_ULL.ToString());
            row.CreateCell(columnIndexes["CO2排放當量"]).SetCellValue(device.Emissions.ToString());
        }

        private void FillActivityData(IRow row, Device device ,List<ActivityData> activityData, Dictionary<string, int> columnIndexes)
        {
            activityData = activityData.Where(x => x.DeviceId == device.Id).ToList();
            string displayUnit = GetDisplayUnit(device.Unit);

            if (displayUnit != "人-年")
            {
                row.CreateCell(columnIndexes["活動數據"]).SetCellValue($"{activityData.Sum(ad => ad.Num) / 1000:F4}");
            }
            else
            {
                row.CreateCell(columnIndexes["活動數據"]).SetCellValue($"{activityData.Sum(ad => ad.Num):F4}");
            }

            row.CreateCell(columnIndexes["單位"]).SetCellValue(displayUnit);
        }
        private void FillGHGData(IRow row, List<GHG> ghgs, Dictionary<string, int> columnIndexes)
        {
            foreach (var ghg in ghgs)
            {
                switch (ghg.Name)
                {
                    case "CO2":
                        row.CreateCell(columnIndexes["CO2"]).SetCellValue("V");
                        break;
                    case "CH4":
                        row.CreateCell(columnIndexes["CH4"]).SetCellValue("V");
                        break;
                    case "N2O":
                        row.CreateCell(columnIndexes["N2O"]).SetCellValue("V");
                        break;
                    case "HFCS":
                        row.CreateCell(columnIndexes["HFCS"]).SetCellValue("V");
                        break;
                    case "PFCS":
                        row.CreateCell(columnIndexes["PFCS"]).SetCellValue("V");
                        break;
                    case "SF6":
                        row.CreateCell(columnIndexes["SF6"]).SetCellValue("V");
                        break;
                    case "NF3":
                        row.CreateCell(columnIndexes["NF3"]).SetCellValue("V");
                        break;
                }
            }
        }

        private string GetDisplayUnit(string unit)
        {
            return unit switch
            {
                "公斤" => "公噸",
                "公升" => "公秉",
                "立方公尺" => "千立方公尺",
                "度" => "千度",
                _ => unit
            };
        }

        public void ReplaceTextInSheet(ISheet sheet, string oldText, string newText)
        {
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    for (int colIndex = 0; colIndex < row.LastCellNum; colIndex++)
                    {
                        ICell cell = row.GetCell(colIndex);
                        if (cell != null && cell.CellType == CellType.String)
                        {
                            string cellValue = cell.StringCellValue;
                            if (cellValue.Contains(oldText))
                            {
                                cell.SetCellType(CellType.Numeric);
                                cell.SetCellValue(cellValue.Replace(oldText, newText));
                            }
                        }
                    }
                }
            }
        }
    }
}
