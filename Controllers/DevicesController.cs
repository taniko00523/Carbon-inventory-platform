using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Authorization;
using Carbon_inventory_platform.Filters;

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
            TempData["yearId"] = Id; // 暫存目前所在的廠區年度ID
            TempData["year"] = await _context.Years // 暫存目前所在的公司名稱 顯示在畫面上方
           .Where(a => a.Id == Id)
           .Select(a => a.Num)
           .FirstOrDefaultAsync();
            return _context.Devices != null ? //如果有抓到資料表Null
                         View(await _context.Devices
                         .Where(x => x.isDeleted == 0 && x.YearId == Id) //抓出資料表裡面沒被刪除的
                         .Include(x => x.GHGs)
                         .Include (x => x.Year.Area.Company)
                         .OrderBy(x => x.CreateTime)
                         .ToListAsync()) : //非同步方法
                         Problem("沒有找到資料表"); //否則回報問題 Entity set 'ApplicationDbContext.Companies'  is null.
        }

        //GET: Devices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Year)
                .Include(d => d.Material)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
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
        public async Task<IActionResult> Create([Bind("Id,YearId,AssetNo,Name,Provess,Scope,EmissionPattern,Material")] Device device)
        {
            if (ModelState.IsValid)
            {
                var deviceId = Guid.NewGuid();
                var toCreate = new Device
                {
                    Id = deviceId,
                    YearId = device.YearId,
                    AssetNo = device.AssetNo,
                    Name = device.Name,
                    Provess = device.Provess,
                    Scope = device.Scope,
                    EmissionPattern = device.EmissionPattern,
                    Material = device.Material,
                    isDeleted = 0,
                    CreateTime = DateTime.Now,
                };
                _context.Add(toCreate);
                await _context.SaveChangesAsync();
                await GHGCheckAsync(deviceId, device.Name, device.Material, device.Scope, device.EmissionPattern);
                return RedirectToAction("Index", "Devices", new { id = device.YearId });
            }
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            deviceDatas.Add(new DeviceData { Name = "其他" });
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(await _context.Materials.Select(m => m.Scope).Distinct().ToListAsync());
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            return View(device);
        }

        [HttpGet]
        public JsonResult GetDeviceLevelAndCorrection(string selectedName, Guid id)
        {
            var deviceData = _context.deviceDatas
       .Where(x => x.Name == selectedName)
       .FirstOrDefault();

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
                    num = device.Num
                };
                return Json(result);
            }

            return Json(null);
        }

        [HttpGet]
        public JsonResult GetDeviceData(string selectedName)
        {
            var deviceData = _context.deviceDatas
                .Where(x => x.Name == selectedName)
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
            if (device == null)
            {
                return NotFound();
            }
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            deviceDatas.Add(new DeviceData { Name = "其他" });
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(await _context.Materials.Select(m => m.Scope).Distinct().ToListAsync());
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AreaId,AssetNo,Name,Provess,Scope,EmissionPattern,Material,YearId")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var toUpdate = await _context.Devices.FindAsync(id);

                    if (toUpdate != null)
                    {
                        toUpdate.YearId = device.YearId;
                        toUpdate.AssetNo = device.AssetNo;
                        toUpdate.Name = device.Name;
                        toUpdate.Provess = device.Provess;
                        toUpdate.Scope = device.Scope;
                        toUpdate.EmissionPattern = device.EmissionPattern;
                        toUpdate.Material = device.Material;
                        toUpdate.ModifiedTime = DateTime.Now;
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
                var YearID = device.YearId;
                return RedirectToAction("Index", "Devices", new { id = YearID });
            }
            var deviceDatas = await _context.deviceDatas.ToListAsync();
            deviceDatas.Add(new DeviceData { Name = "其他" });
            ViewData["name"] = new SelectList(deviceDatas, "Name", "Name");
            ViewData["Scope"] = new SelectList(await _context.Materials.Select(m => m.Scope).Distinct().ToListAsync());
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");

            return View(device);
        }

        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Year)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

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
                var year = await _context.Years.FindAsync(toDeleteDevice.YearId);
                if (toDeleteDevice.ModifiedTime == null)
                {
                    year.All -= toDeleteDevice.Emissions;
                    _context.Devices.Remove(toDeleteDevice);
                }
                else
                {
                    year.All -= toDeleteDevice.Emissions;
                    toDeleteDevice.isDeleted = 1;
                    toDeleteDevice.DeleteTime = DateTime.Now;
                }
                await _context.SaveChangesAsync();

            }
            var YearID = toDeleteDevice.YearId;
            return RedirectToAction("Index", "Devices", new { id = YearID });
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

            var activitydata = await _context.Devices.FindAsync(id);
            if (activitydata == null)
            {
                return View();
            }
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
            string[] unit = { "公斤", "公升", "立方公尺", "度", "人", "其他" };
            ViewData["Unit"] = new SelectList(unit);
            string[] source = { "發票", "領用單", "紀錄表", "繳費單" };
            ViewData["Source"] = new SelectList(source);
            return View(activitydata);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivityData(Guid? id, [Bind("Id,Num,Unit,Data_Correction,Device_Correction")] Device activityData)
        {
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

            var GHG = await _context.GHGs.Where(x => x.DeviceId == id).ToListAsync(); //抓出需要算排放量的排放源中的溫室氣體
            var Device = await _context.Devices.FindAsync(id);
            var year = await _context.Years.FindAsync(Device.YearId);

            decimal all_Emission = 0;

            decimal GHG1 = 0;
            decimal GHG2 = 0;
            decimal GHG3 = 0;
            decimal GHG1ULL = 0;
            decimal GHG1UUL = 0;
            decimal GHG2ULL = 0;
            decimal GHG2UUL = 0;
            decimal GHG3ULL = 0;
            decimal GHG3UUL = 0;


            if (GHG != null && Device != null)
            {
                if (ModelState.IsValid)
                {
                    int i = 1;
                    foreach (var item in GHG)
                    {
                        decimal GWP = _context.GWPs.Where(x => x.Name == item.Name).FirstOrDefault().Num;
                        item.Emission = item.CEF * activityData.Num / 1000 * GWP;
                        item.ModifiedTime = DateTime.Now;
                        if (i == 1)
                        {
                            GHG1 += item.Emission;
                            all_Emission += item.Emission;
                            GHG1ULL += item.all_ULL;
                            GHG1UUL += item.all_UUL;
                        }
                        if (i == 2)
                        {
                            GHG2 += item.Emission;
                            all_Emission += item.Emission;
                            GHG2ULL += item.all_ULL;
                            GHG2UUL += item.all_UUL;
                        }
                        if (i == 3)
                        {
                            GHG3 += item.Emission;
                            all_Emission += item.Emission;
                            GHG3ULL += item.all_ULL;
                            GHG3UUL += item.all_UUL;
                        }
                        i++;
                    }
                    decimal Device_allUUL = Calculate95U(GHG1, GHG2, GHG3, GHG1UUL, GHG2UUL, GHG3UUL);
                    decimal Device_allULL = Calculate95U(GHG1, GHG2, GHG3, GHG1ULL, GHG2ULL, GHG3ULL);
                    Device.Num = activityData.Num;
                    Device.Unit = activityData.Unit;
                    Device.Emissions = all_Emission;
                    year.All += all_Emission;
                    Device.Data_Correction = activityData.Data_Correction;
                    Device.Device_Correction = activityData.Device_Correction;
                    Device.Grade = activityData.CEF_Correction * activityData.Data_Correction * activityData.Device_Correction;
                    Device.all_UUL = Device_allUUL;
                    Device.all_ULL = Device_allULL;
                    Device.ModifiedTime = DateTime.Now;
                    Device.count_UUL = Device_allUUL * all_Emission * Device_allUUL * all_Emission;
                    Device.count_ULL = Device_allULL * all_Emission * Device_allULL * all_Emission;
                    string[] unit = { "公斤", "公升", "立方公尺", "度", "人", "其他" };
                    ViewData["Unit"] = new SelectList(unit);
                    string[] source = { "發票", "領用單", "紀錄表", "繳費單" };
                    ViewData["Source"] = new SelectList(source);
                    await _context.SaveChangesAsync();
                }
            }
            
            var yearID = TempData.Peek("yearId");
            return RedirectToAction("Index", "Devices", new { id = yearID });
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

        public IActionResult DownloadFile(string fileName)
        {
            // 設定要下載的檔案路徑
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

            // 檢查檔案是否存在
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // 讀取檔案內容
            var fileContent = System.IO.File.ReadAllBytes(filePath);

            // 指定檔案型別
            var contentType = "application/octet-stream";

            // 建立一個 FileResult 物件
            var fileResult = new FileContentResult(fileContent, contentType)
            {
                FileDownloadName = fileName
            };

            return fileResult;
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
                    YearId = id,
                    Name = Name,
                    Material = Material,
                    Scope = Scope,
                    EmissionPattern = EmissionPattern,
                    CreateTime = DateTime.Now,
                });
                await _context.SaveChangesAsync();
                await GHGCheckAsync(deviceID, Name, Material, Scope, EmissionPattern);
            }

            var areaId = TempData.Peek("SelectedAreaId") as Guid?;


            return RedirectToAction(nameof(Index), new { id = id });
        }

        public async Task<GHG> GHGCheckAsync(Guid id, string name, string material, string scope, string emisspatern) //排放源Id, 排放源名稱, 物料名稱, 類別, 排放型式
        {
            var Device = await _context.Devices.FindAsync(id);
            var GWP = await _context.GWPs.ToListAsync();
            var Material = await _context.Materials.Where(x => x.Name == material && x.Scope == scope && x.EmissionPattern == emisspatern).FirstOrDefaultAsync();
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
    }
}
