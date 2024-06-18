using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Carbon_inventory_platform.Filters;
using Carbon_inventory_platform.ViewModel;

namespace Carbon_inventory_platform.Controllers
{
    [CheckSubscriptionData]
    [Authorize]
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }
        //GET: Devices
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
                TempData["companyName"] = AreaData.Company.Name.Trim()!=""? AreaData.Company.Name:"未填寫公司資料";
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

        // GET: Devices/Create
        public async Task<IActionResult> Create()
        {
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            deviceDatas.Add(new DeviceData { Name = "其他" });
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["AreasId"] = new SelectList(await _context.Areas.Where(x => x.isDeleted == 0).ToListAsync(), "Id", "Name");
            ViewData["Scope"] = new SelectList(await _context.Materials.Select(m => m.Scope).Distinct().ToListAsync());
            //ViewData["EmissionPattern"] = new SelectList(emissionPattern);

            return View();
        }


        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            deviceDatas.Add(new DeviceData { Name = "其他" });
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(await _context.Materials.Select(m => m.Scope).Distinct().ToListAsync());
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            return View(device);
        }

        public async Task<IActionResult> CopyDevice(Guid? id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id);
            Guid newDeviceId = Guid.Empty;
            var areaId = new Guid();
            if (device != null)
            {
                areaId = device.AreaId;

                newDeviceId = Guid.NewGuid();
                var newDevice = new Device
                {
                    Id = newDeviceId,
                    AreaId = areaId,
                    Name = device.Name,
                    OtherName = device.OtherName,
                    NameRemark = device.NameRemark,
                    Scope = device.Scope,
                    EmissionPattern = device.EmissionPattern,
                    Material = device.Material,
                    Customize = device.Customize,
                    AssetNo = device.AssetNo,
                    Provess = device.Provess,
                    Source = device.Source,
                    Dept = device.Dept,
                    Unit = device.Unit,
                    Data_Correction = device.Data_Correction,
                    Device_Correction = device.Device_Correction,
                    CEF_Correction = device.CEF_Correction,
                    Grade = device.Grade,
                    Emissions = device.Emissions,
                    data_ULL = device.data_ULL,
                    all_ULL = device.all_ULL,
                    all_UUL = device.all_UUL,
                    count_ULL = device.count_ULL,
                    count_UUL = device.count_UUL,
                    isDeleted = 0,
                    CreateTime = DateTime.Now
                };

                _context.Devices.Add(newDevice);
                await _context.SaveChangesAsync();
            }
            var GHGs = await _context.GHGs.Where(x => x.DeviceId == id).ToListAsync();
            if (GHGs != null)
            {
                foreach (var ghg in GHGs)
                {
                    var newGHG = new GHG
                    {
                        Id = new Guid(),
                        DeviceId = newDeviceId,
                        Name = ghg.Name,
                        GWP = ghg.GWP,
                        CEF = ghg.CEF,
                        all_ULL = ghg.all_ULL,
                        all_UUL = ghg.all_UUL,
                        CEF_ULL = ghg.CEF_ULL,
                        CEF_UUL = ghg.CEF_UUL,
                        Emission = ghg.Emission,
                        isDeleted = 0,
                        CreateTime = DateTime.Now
                    };
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
            emission = _context.Materials.Where(x => x.Scope == scope).Select(m => m.EmissionPattern).Distinct().ToList();
            return Json(emission);
        }

        // GET: Devices/Edit/5
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
            deviceDatas.Add(new DeviceData { Name = "其他" });
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(await _context.Materials.Select(m => m.Scope).Distinct().ToListAsync());
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            return View(deviceViewModel);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            deviceDatas.Add(new DeviceData { Name = "其他" });
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(await _context.Materials.Select(m => m.Scope).Distinct().ToListAsync());
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");

            return View(device);
        }

        //// GET: Devices/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null || _context.Devices == null)
        //    {
        //        return NotFound();
        //    }

        //    var device = await _context.Devices
        //        .Include(d => d.Year)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (device == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(device);
        //}

        // POST: Devices/Delete/5
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
                var toCreate = new ActivityData
                {
                    DeviceId = device.Id,
                    Num = data.Num,
                    Time = data.Time,
                    remark = data.remark,
                };

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
                    // 如果不存在，則添加新的ActivityData
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
        public async Task CountEmissionData(Guid? id)
        {
            var activityDatas = await _context.ActivityDatas.Where(x => x.DeviceId == id).ToListAsync();
            decimal Num = activityDatas.Sum(x => x.Num);


            var Device = await _context.Devices.Where(x => x.Id == id).Include(x => x.Area).FirstOrDefaultAsync();
            var GHG = await _context.GHGs.Where(x => x.DeviceId == id).ToListAsync(); //抓出需要算排放量的排放源中的溫室氣體
            var emission = await _context.Areas.FindAsync(Device.AreaId);
            decimal all_Emission = 0,
                GHG1 = 0, GHG2 = 0, GHG3 = 0,
                GHG1ULL = 0, GHG1UUL = 0,
                GHG2ULL = 0, GHG2UUL = 0,
                GHG3ULL = 0, GHG3UUL = 0;
            if (GHG != null && Device != null)
            {
                if (ModelState.IsValid)
                {
                    int i = 1;
                    foreach (var item in GHG)
                    {
                        if (item.Device.Material == "廢水處理")
                        {
                            item.Emission = item.CEF * Num * item.GWP;
                        }
                        else
                        {
                            item.Emission = item.CEF * Num / 1000 * item.GWP;
                        }

                        item.ModifiedTime = DateTime.Now;
                        if (i == 1)
                        {
                            GHG1 += item.Emission;
                            all_Emission += item.Emission;
                            GHG1ULL += item.all_ULL * 100;
                            GHG1UUL += item.all_UUL * 100;
                        }
                        if (i == 2)
                        {
                            GHG2 += item.Emission;
                            all_Emission += item.Emission;
                            GHG2ULL += item.all_ULL * 100;
                            GHG2UUL += item.all_UUL * 100;
                        }
                        if (i == 3)
                        {
                            GHG3 += item.Emission;
                            all_Emission += item.Emission;
                            GHG3ULL += item.all_ULL * 100;
                            GHG3UUL += item.all_UUL * 100;
                        }
                        i++;
                    }
                    decimal Device_allUUL = Calculate95U(GHG1, GHG2, GHG3, GHG1UUL, GHG2UUL, GHG3UUL);
                    decimal Device_allULL = Calculate95U(GHG1, GHG2, GHG3, GHG1ULL, GHG2ULL, GHG3ULL);
                    Device.Emissions = all_Emission;
                    emission.All += all_Emission;
                    Device.all_UUL = Device_allUUL;
                    Device.all_ULL = Device_allULL;
                    Device.ModifiedTime = DateTime.Now;
                    Device.count_UUL = Device_allUUL * all_Emission * Device_allUUL * all_Emission;
                    Device.count_ULL = Device_allULL * all_Emission * Device_allULL * all_Emission;

                    await _context.SaveChangesAsync();
                }
            }
        }

        static decimal CalculateRoundDistance(decimal num1, decimal num2) // 計算兩數平方和的平方根，並四捨五入到小數點後5位
        {
            if (num1 != 0 && num2 != 0)
            {
                decimal distance = DecimalSqrt(num1 * num1 + num2 * num2);
                return Math.Round(distance, 5);
            }
            return 0;
        }

        static decimal Calculate95U(decimal GHG1, decimal GHG2, decimal GHG3, decimal GHG1UUL, decimal GHG2UUL, decimal GHG3UUL) //計算95%信賴區間 
        {
            decimal count;
            decimal allGHG;

            if (GHG1 != 0 && GHG1UUL != 0)
            {
                if (GHG2 != 0 && GHG2UUL != 0)
                {
                    if (GHG3 != 0 && GHG3UUL != 0)
                    {
                        count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG2 * GHG2UUL * GHG2 * GHG2UUL) + (GHG3 * GHG3UUL * GHG3 * GHG3UUL));
                        allGHG = GHG1 + GHG2 + GHG3;

                        if (allGHG != 0)
                        {
                            return count / allGHG;
                        }

                    }
                    count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG2 * GHG2UUL * GHG2 * GHG2UUL));
                    allGHG = GHG1 + GHG2;

                    if (allGHG != 0)
                    {
                        return count / allGHG;
                    }
                }
                else if (GHG3 != 0 && GHG3UUL != 0)
                {
                    if (GHG2 != 0 && GHG2UUL != 0)
                    {
                        count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG2 * GHG2UUL * GHG2 * GHG2UUL) + (GHG3 * GHG3UUL * GHG3 * GHG3UUL));
                        allGHG = GHG1 + GHG2 + GHG3;

                        if (allGHG != 0)
                        {
                            return count / allGHG;
                        }

                    }
                    count = DecimalSqrt((GHG1 * GHG1UUL * GHG1 * GHG1UUL) + (GHG3 * GHG3UUL * GHG3 * GHG3UUL));
                    allGHG = GHG1 + GHG3;

                    if (allGHG != 0)
                    {
                        return count / allGHG;
                    }
                }

                return GHG1UUL;
            }

            return 0;
        }

        public static decimal DecimalSqrt(decimal value, int iterations = 20) //用牛頓法逼近Decimal的平方根
        {
            if (value < 0)
            {
                throw new ArgumentException("不能計算負數的平方根");
            }

            decimal guess = value / 2;
            for (int i = 0; i < iterations; i++)
            {
                guess = 0.5m * (guess + value / guess);
            }

            return guess;
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

        public async Task<GHG?> GHGCheckAsync(Guid id, string name, string material, string scope, string emisspatern, int year) //排放源Id, 排放源名稱, 物料名稱, 類別, 排放型式
        {
            var Device = await _context.Devices.FindAsync(id);
            //var GWP = await _context.GWPs.OrderBy(x => x.GWP_Year).Where(x => x.GWP_Year <= Device.Year.Num).ToListAsync();
            var GWP = await _context.GWPs.ToListAsync();
            var MaterialList = await _context.Materials
            .Where(x => x.Name == material && x.Scope == scope && x.EmissionPattern == emisspatern && x.Year <= year).ToListAsync();
            var Material = MaterialList.OrderByDescending(x => x.Year).FirstOrDefault();

            var otherMaterial = await _context.Materials.Where(x => x.Name == name && x.Scope == scope && x.EmissionPattern == emisspatern).FirstOrDefaultAsync(); //目前只有冷媒設備，但我包含了PFCS以防萬一
            if (ModelState.IsValid)
            {
                if (Material != null)
                {
                    if (Material.CO2CEF != 0)
                    {
                        var toCreateCO2 = new GHG();
                        toCreateCO2.Id = Guid.NewGuid();
                        toCreateCO2.Name = "CO2";
                        toCreateCO2.DeviceId = id;
                        toCreateCO2.CEF = Material.CO2CEF;
                        toCreateCO2.CEF_UUL = Material.CO2UUL;
                        toCreateCO2.CEF_ULL = Material.CO2ULL;
                        toCreateCO2.GWP = GWP.Where(x => x.Name == "CO2").Select(x => x.Num).FirstOrDefault();
                        toCreateCO2.all_UUL = CalculateRoundDistance(Material.CO2UUL, Material.DataUUL);
                        toCreateCO2.all_ULL = CalculateRoundDistance(Material.CO2ULL, Material.DataULL);
                        toCreateCO2.CreateTime = DateTime.Now;
                        _context.AddRange(toCreateCO2);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;

                    }
                    if (Material.CH4CEF != 0)
                    {
                        var toCreateCH4 = new GHG();
                        toCreateCH4.Id = Guid.NewGuid();
                        toCreateCH4.Name = "CH4";
                        toCreateCH4.DeviceId = id;
                        toCreateCH4.CEF = Material.CH4CEF;
                        toCreateCH4.CEF_UUL = Material.CH4UUL;
                        toCreateCH4.CEF_ULL = Material.CH4ULL;
                        toCreateCH4.GWP = GWP.Where(x => x.Name == "CH4").Select(x => x.Num).FirstOrDefault();
                        toCreateCH4.all_UUL = CalculateRoundDistance(Material.CH4UUL, Material.DataUUL);
                        toCreateCH4.all_ULL = CalculateRoundDistance(Material.CH4ULL, Material.DataULL);
                        toCreateCH4.CreateTime = DateTime.Now;
                        _context.Add(toCreateCH4);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.N2OCEF != 0)
                    {
                        var toCreateN2O = new GHG();
                        toCreateN2O.Id = Guid.NewGuid();
                        toCreateN2O.Name = "N2O";
                        toCreateN2O.DeviceId = id;
                        toCreateN2O.CEF = Material.N2OCEF;
                        toCreateN2O.CEF_UUL = Material.N2OUUL;
                        toCreateN2O.CEF_ULL = Material.N2OULL;
                        toCreateN2O.GWP = GWP.Where(x => x.Name == "N2O").Select(x => x.Num).FirstOrDefault();
                        toCreateN2O.all_UUL = CalculateRoundDistance(Material.N2OUUL, Material.DataUUL);
                        toCreateN2O.all_ULL = CalculateRoundDistance(Material.N2OULL, Material.DataULL);
                        toCreateN2O.CreateTime = DateTime.Now;
                        _context.Add(toCreateN2O);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.HFCSCEF != 0)
                    {
                        var toCreateHFCS = new GHG();
                        toCreateHFCS.Id = Guid.NewGuid();
                        toCreateHFCS.Name = "HFCS";
                        toCreateHFCS.DeviceId = id;
                        toCreateHFCS.CEF = Material.HFCSCEF;
                        toCreateHFCS.CEF_UUL = Material.HFCSUUL;
                        toCreateHFCS.CEF_ULL = Material.HFCSULL;
                        toCreateHFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreateHFCS.all_UUL = CalculateRoundDistance(Material.HFCSUUL, Material.DataUUL);
                        toCreateHFCS.all_ULL = CalculateRoundDistance(Material.HFCSULL, Material.DataULL);
                        toCreateHFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreateHFCS);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.PFCSCEF != 0)
                    {
                        var toCreatePFCS = new GHG();
                        toCreatePFCS.Id = Guid.NewGuid();
                        toCreatePFCS.Name = "PFCS";
                        toCreatePFCS.DeviceId = id;
                        toCreatePFCS.CEF = Material.PFCSCEF;
                        toCreatePFCS.CEF_UUL = Material.PFCSUUL;
                        toCreatePFCS.CEF_ULL = Material.PFCSULL;
                        toCreatePFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreatePFCS.all_UUL = CalculateRoundDistance(Material.PFCSUUL, Material.DataUUL);
                        toCreatePFCS.all_ULL = CalculateRoundDistance(Material.PFCSULL, Material.DataULL);
                        toCreatePFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreatePFCS);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.SF6CEF != 0)
                    {
                        var toCreateSF6 = new GHG();
                        toCreateSF6.Id = Guid.NewGuid();
                        toCreateSF6.Name = "SF6";
                        toCreateSF6.DeviceId = id;
                        toCreateSF6.CEF = Material.SF6CEF;
                        toCreateSF6.CEF_UUL = Material.SF6UUL;
                        toCreateSF6.CEF_ULL = Material.SF6ULL;
                        toCreateSF6.GWP = GWP.Where(x => x.Name == "SF6").Select(x => x.Num).FirstOrDefault();
                        toCreateSF6.all_UUL = CalculateRoundDistance(Material.SF6UUL, Material.DataUUL);
                        toCreateSF6.all_ULL = CalculateRoundDistance(Material.SF6ULL, Material.DataULL);
                        toCreateSF6.CreateTime = DateTime.Now;
                        _context.Add(toCreateSF6);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                    if (Material.NF3CEF != 0)
                    {
                        var toCreateNF3 = new GHG();
                        toCreateNF3.Id = Guid.NewGuid();
                        toCreateNF3.Name = "NF3";
                        toCreateNF3.DeviceId = id;
                        toCreateNF3.CEF = Material.NF3CEF;
                        toCreateNF3.CEF_UUL = Material.NF3UUL;
                        toCreateNF3.CEF_ULL = Material.NF3ULL;
                        toCreateNF3.GWP = GWP.Where(x => x.Name == "NF3").Select(x => x.Num).FirstOrDefault();
                        toCreateNF3.all_UUL = CalculateRoundDistance(Material.NF3UUL, Material.DataUUL);
                        toCreateNF3.all_ULL = CalculateRoundDistance(Material.NF3ULL, Material.DataULL);
                        toCreateNF3.CreateTime = DateTime.Now;
                        _context.Add(toCreateNF3);
                        Device.CEF_Correction = Material.CEF_Correction;
                        Device.data_UUL = Material.DataUUL;
                        Device.data_ULL = Material.DataULL;
                    }
                }
                else if (otherMaterial != null) //目前只有冷媒設備，但我包含了PFCS以防萬一
                {
                    if (otherMaterial.HFCSCEF != 0)
                    {
                        var toCreateHFCS = new GHG();
                        toCreateHFCS.Id = Guid.NewGuid();
                        toCreateHFCS.Name = "HFCS";
                        toCreateHFCS.DeviceId = id;
                        toCreateHFCS.CEF = otherMaterial.HFCSCEF;
                        toCreateHFCS.CEF_UUL = otherMaterial.HFCSUUL;
                        toCreateHFCS.CEF_ULL = otherMaterial.HFCSULL;
                        toCreateHFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreateHFCS.all_UUL = CalculateRoundDistance(otherMaterial.HFCSUUL, otherMaterial.DataUUL);
                        toCreateHFCS.all_ULL = CalculateRoundDistance(otherMaterial.HFCSULL, otherMaterial.DataULL);
                        toCreateHFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreateHFCS);
                        Device.CEF_Correction = otherMaterial.CEF_Correction;
                        Device.data_UUL = otherMaterial.DataUUL;
                        Device.data_ULL = otherMaterial.DataULL;
                    }
                    if (otherMaterial.PFCSCEF != 0)
                    {
                        var toCreatePFCS = new GHG();
                        toCreatePFCS.Id = Guid.NewGuid();
                        toCreatePFCS.Name = "PFCS";
                        toCreatePFCS.DeviceId = id;
                        toCreatePFCS.CEF = otherMaterial.PFCSCEF;
                        toCreatePFCS.CEF_UUL = otherMaterial.PFCSUUL;
                        toCreatePFCS.CEF_ULL = otherMaterial.PFCSULL;
                        toCreatePFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        toCreatePFCS.all_UUL = CalculateRoundDistance(otherMaterial.PFCSUUL, otherMaterial.DataUUL);
                        toCreatePFCS.all_ULL = CalculateRoundDistance(otherMaterial.PFCSULL, otherMaterial.DataULL);
                        toCreatePFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreatePFCS);
                        Device.CEF_Correction = otherMaterial.CEF_Correction;
                        Device.data_UUL = otherMaterial.DataUUL;
                        Device.data_ULL = otherMaterial.DataULL;

                    }

                }
                await _context.SaveChangesAsync();

            }

            return null;
        }

        public async Task<GHG?> CEFAddAsync(Device device, string GHG, decimal? CEF)
        {

            //var GWP = await _context.GWPs.OrderBy(x => x.GWP_Year).Where(x => x.GWP_Year <= device.Year.Num).ToListAsync();
            var GWP = await _context.GWPs.ToListAsync();
            if (ModelState.IsValid)
            {

                var toCreate = new GHG();
                toCreate.Id = Guid.NewGuid();
                toCreate.Name = GHG;
                toCreate.DeviceId = device.Id;
                toCreate.CEF = (decimal)CEF;
                if (GHG == "HFCS")
                {
                    toCreate.GWP = GWP.Where(x => x.Name == device.Material).Select(x => x.Num).FirstOrDefault();
                }
                else
                {
                    toCreate.GWP = GWP.Where(x => x.Name == GHG).Select(x => x.Num).FirstOrDefault();
                }
                toCreate.CreateTime = DateTime.Now;
                device.CEF_Correction = 1;//輸入?
                _context.Add(toCreate);
                await _context.SaveChangesAsync();

            }

            return null;
        }
    }
}
