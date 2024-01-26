using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using NuGet.ContentModel;
using static System.Formats.Asn1.AsnWriter;
using Xceed.Words.NET;
using Xceed.Document.NET;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;

namespace Carbon_inventory_platform.Controllers
{
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }
        string[] scope = { "類別一", "類別二" };
        string[] emissionPattern = { "固定", "移動", "製程", "逸散" };
        string[] name = {
            "緊急發電機", "廚房", "公務車", "堆高機", "冷氣機", "冰箱","商用冰箱","中、大型冰箱","低溫冷凍車", "乾燥機", "飲水機", "冰水主機", "車用空調", "工業冷藏、冷凍","食品加工冷藏、冷凍", "CO2滅火器", "海龍-1211", "FM200", "WD40", "化糞池", "瓦斯罐","焊條","乙炔","二氧化碳", "電力", "其他" };
        string[] unit = { "公斤", "公升", "立方公尺", "度", "人", "其他" };
        string[] source = { "發票", "領用單", "紀錄表", "繳費單" };
        string[] level = { "連續監測", "定期採樣", "自行評估" };
        string[] correction = { "每年外校一次以上量測", "每年外校不到一次量測", "非量測所得知數據" };
        //GET: Devices
        public async Task<IActionResult> Index(Guid id)
        {
            TempData["SelectedAreaId"] = id; //暫存目前所在的廠區ID
            TempData["Company"] = await _context.Areas //暫存目前所在的公司名稱
            .Where(a => a.Id == id)
            .Include(a => a.Company)
            .Select(a => a.Company.Name)
            .FirstOrDefaultAsync();
            ViewData["Year"] = new SelectList(await _context.Devices.Select(x => x.year).Distinct().ToListAsync()); //抓出排放源資料庫的所有盤查年份
         //   bool yearSelectedFlag = TempData["yearSelectedFlag"] != null && (bool)TempData["yearSelectedFlag"];
         //   if (yearSelectedFlag) //如果有標記(新增、修改、刪除、新增活動數據) 則回到該盤查年份的排放源鑑別
         //   {
         //       var devicesForSelectedYear = await _context.Devices
         //.Where(x => x.isDeleted != 1 && x.AreaId == id && x.year == Convert.ToInt32(TempData.Peek("yearSelected").ToString()))
         //.OrderBy(x => x.Name)
         //.ToListAsync();
         //       TempData["yearSelectedFlag"] = false;
         //       return View(devicesForSelectedYear);
         //   }
            var device = new Device();
            return View(new List<Device> { device }); //將空的device包裝至集合中 返回至Index 
        }

        // 新增用於處理下拉選單變更的 Action
        [HttpPost]
        public async Task<IActionResult> Index(Guid id, int Year)
        {
            ViewData["Year"] = new SelectList(await _context.Devices.Select(x => x.year).Distinct().ToListAsync()); //抓出排放源資料庫的所有盤查年份
            bool abc = true;
            TempData["yearSelected"] = Year;
            //TempData["yearSelectedFlag"] = true; // 新增一個已選擇年份的標記
            // 查詢Device 下拉選單回傳的盤查年度 排放源資料
            var devicesForSelectedYear = await _context.Devices
         .Where(x => x.isDeleted != 1 && x.AreaId == id && x.year == Year)
         .OrderBy(x => x.Name)
         .ToListAsync();

            return View(devicesForSelectedYear);
        }

        //GET: Devices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Areas)
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
            ViewData["name"] = new SelectList(name);
            ViewData["Material"] = new SelectList(await _context.Materials.Where(x => x.EmissionPattern == "固定").ToListAsync(), "Name", "Name");
            ViewData["AreasId"] = new SelectList(await _context.Areas.Where(x => x.isDeleted == 0).ToListAsync(), "Id", "Name");
            ViewData["Scope"] = new SelectList(scope);
            ViewData["EmissionPattern"] = new SelectList(emissionPattern);

            return View();
        }


        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AreaId,AssetNo,Name,Provess,Scope,EmissionPattern,Material,year")] Device device)
        {
            if (ModelState.IsValid)
            {
                var toCreate = new Device();
                {
                    toCreate.Id = Guid.NewGuid();
                    toCreate.AreaId = device.AreaId;
                    toCreate.AssetNo = device.AssetNo;
                    toCreate.Name = device.Name;
                    toCreate.Provess = device.Provess;
                    toCreate.Scope = device.Scope;
                    toCreate.EmissionPattern = device.EmissionPattern;
                    toCreate.Material = device.Material;
                    toCreate.isDeleted = 0;
                    toCreate.CreateTime = DateTime.Now;
                    toCreate.year = device.year;

                    if (_context.Materials
    .Where(x => x.Name == device.Material)
    .Select(x => x.CO2CEF)
    .FirstOrDefault() != 0)
                    {
                        toCreate.CO2_Emission = true;
                    }
                    if (_context.Materials
    .Where(x => x.Name == device.Material)
    .Select(x => x.CH4CEF)
    .FirstOrDefault() != 0)
                    {
                        toCreate.CH4_Emission = true;
                    }
                    if (_context.Materials
    .Where(x => x.Name == device.Material)
    .Select(x => x.N2OCEF)
    .FirstOrDefault() != 0)
                    {
                        toCreate.N2O_Emission = true;
                    }
                    else if (_context.GWPs
    .Where(x => x.Name == device.Material)
    .Select(x => x.Num)
    .FirstOrDefault() != null)
                    {
                        toCreate.HFCS_Emission = true;
                    }
                }
                _context.Add(toCreate);
                var AreaID = device.AreaId;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Devices", new { id = AreaID });

            }
            ViewData["name"] = new SelectList(name);
            ViewData["Material"] = new SelectList(await _context.Materials.Where(x => x.EmissionPattern == "固定").ToListAsync(), "Name", "Name");
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["Scope"] = new SelectList(scope);
            ViewData["EmissionPattern"] = new SelectList(emissionPattern);
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
            var result = new
            {
                level = deviceData?.Level,
                correction = deviceData?.Correction,
                unit = deviceData?.unit,
                num = device.Num
            };
            return Json(result);
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
            var materials = _context.Materials
    .Where(x => x.EmissionPattern == emissionPattern)
    .Select(x => new { name = x.Name })
    .ToList();

            List<object> gwpNames = new List<object>();

            if (emissionPattern == "逸散")
            {
                gwpNames = _context.GWPs
                    .Select(x => new { name = x.Name })
                    .ToList<object>(); // 將型別調整為相同的 List<object>
            }

            return Json(new { materials, gwpNames });

        }


        [HttpGet]
        public JsonResult GetMaterials(string emissionPattern)
        {

            var materials = _context.Materials
                .Where(x => x.EmissionPattern == emissionPattern)
                .Select(x => new { name = x.Name })
                .ToList();

            return Json(materials);
        }
        [HttpGet]
        public JsonResult GetEmissionsByScope(string scope)
        {
            // 根据scope的值生成相应的emission选项，这里假设你已经有了相应的逻辑来获取这些选项
            var emissions = GetEmissionsByScopeFromDatabase(scope);

            return Json(emissions);
        }

        // 示例中的方法，根据scope获取emission选项
        private List<string> GetEmissionsByScopeFromDatabase(string scope)
        {
            // 在这里添加逻辑从数据库获取emission选项
            // 假设你有一个类似的方法，根据scope返回对应的emission选项列表
            // 以下为示例，你需要根据你的实际情况进行修改
            if (scope == "類別一")
            {
                return new List<string> { "固定", "移動", "逸散", "製程" };
            }
            else
            {
                // 其他情况的处理
                return new List<string> { "外購電力" };
            }
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
            ViewData["name"] = new SelectList(name);
            ViewData["Material"] = new SelectList(await _context.Materials.Where(x => x.EmissionPattern == "固定").ToListAsync(), "Name", "Name");
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["Scope"] = new SelectList(scope);
            ViewData["EmissionPattern"] = new SelectList(emissionPattern);
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AreaId,AssetNo,Name,Provess,Scope,EmissionPattern,Material,CO2_Emission,CH4_Emission,N2O_Emission,HFCS_Emission,PFCS_Emission,SF6_Emission,NF3_Emission")] Device device)
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
                        toUpdate.AreaId = device.AreaId;
                        toUpdate.AssetNo = device.AssetNo;
                        toUpdate.Name = device.Name;
                        toUpdate.Provess = device.Provess;
                        toUpdate.Scope = device.Scope;
                        toUpdate.EmissionPattern = device.EmissionPattern;
                        toUpdate.Material = device.Material;
                        toUpdate.CO2_Emission = device.CO2_Emission;
                        toUpdate.CH4_Emission = device.CH4_Emission;
                        toUpdate.N2O_Emission = device.N2O_Emission;
                        toUpdate.HFCS_Emission = device.HFCS_Emission;
                        toUpdate.PFCS_Emission = device.PFCS_Emission;
                        toUpdate.SF6_Emission = device.SF6_Emission;
                        toUpdate.NF3_Emission = device.NF3_Emission;
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
                var AreaID = device.AreaId;
                return RedirectToAction("Index", "Devices", new { id = AreaID });
            }
            ViewData["name"] = new SelectList(name);
            ViewData["Material"] = new SelectList(await _context.Materials.Where(x => x.EmissionPattern == "固定").ToListAsync(), "Name", "Name");
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["Scope"] = new SelectList(scope);
            ViewData["EmissionPattern"] = new SelectList(emissionPattern);
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
                .Include(d => d.Areas)
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
            var toDelete = await _context.Devices.FindAsync(id);
            if (toDelete.Num == 0)
            {
                _context.Devices.Remove(toDelete);
            }
            else if (toDelete != null)
            {
                toDelete.isDeleted = 1;
                toDelete.DeleteTime = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            var AreaID = toDelete.AreaId;
            return RedirectToAction("Index", "Devices", new { id = AreaID });
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
            ViewData["DataCorrection"] = new SelectList(await _context.dataCorrections.ToListAsync(), "id", "name");
            ViewData["DataLevel"] = new SelectList(await _context.dataLevels.ToListAsync(), "id", "name");
            ViewData["Unit"] = new SelectList(unit);
            ViewData["Source"] = new SelectList(source);
            ViewData["Level"] = new SelectList(level);
            ViewData["Correction"] = new SelectList(correction);
            return View(activitydata);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivityData(Guid? id, [Bind("Id,Num,Unit,Source,Dept,Level,Correction,Material,Emissions")] Device activityData)
        {
            ViewData["DataCorrection"] = new SelectList(_context.dataCorrections, "Id", "Name");
            ViewData["DataLevel"] = new SelectList(_context.dataLevels, "Id", "Name");

            var toUpdate = await _context.Devices.FindAsync(id);

            float all = 0;
            float CO2 = 0;
            float CH4 = 0;
            float N2O = 0;
            float HFCS = 0;
            int Grade = 0;
            float CO2ULL = 0;
            float CO2UUL = 0;
            float CH4ULL = 0;
            float CH4UUL = 0;
            float N2OULL = 0;
            float N2OUUL = 0;
            float UUL = 0;
            float ULL = 0;
            float count_ULL = 0;
            float count_UUL = 0;
            if (toUpdate.CO2_Emission == true && toUpdate.CH4_Emission == true && toUpdate.N2O_Emission == true) //固定移動
            {
                var materialsData = await _context.Materials//區分固定與移動
                    .Where(x => x.EmissionPattern == toUpdate.EmissionPattern)
                    .Where(x => x.Name == toUpdate.Material)
                    .FirstOrDefaultAsync();// 抓取對應的Materials 數據

                float CH4_GWP = (float)(await _context.GWPs.Where(x => x.Name == "CH4").FirstOrDefaultAsync()).Num;
                float N2O_GWP = (float)(await _context.GWPs.Where(x => x.Name == "N2O").FirstOrDefaultAsync()).Num;

                // 如果找到 Materials 数据，计算 Emissions
                if (materialsData != null)
                {
                    CO2 = (float)Math.Round(Math.Round(activityData.Num / 1000.0, 4) * materialsData.CO2CEF * 1, 4);
                    CH4 = (float)Math.Round(Math.Round(activityData.Num / 1000.0, 4) * materialsData.CH4CEF * 1, 4);
                    N2O = (float)Math.Round(Math.Round(activityData.Num / 1000.0, 4) * materialsData.N2OCEF * 1, 4);
                    Grade = 3 * activityData.Level * activityData.Correction;
                    all = (float)Math.Round(CO2 + CH4 + N2O, 4);
                }
                CO2ULL = (float)CalculateRoundDistance(-0.01F, materialsData.CO2ULL);//單排放源CO2排放95%信賴區間下限
                CO2UUL = (float)CalculateRoundDistance(0.01F, materialsData.CO2UUL);//單排放源CO2排放95%信賴區間上限
                CH4ULL = (float)CalculateRoundDistance(-0.01F, materialsData.CH4ULL);//單排放源CH4排放95%信賴區間下限
                CH4UUL = (float)CalculateRoundDistance(0.01F, materialsData.CH4UUL);//單排放源CH4排放95%信賴區間上限
                N2OULL = (float)CalculateRoundDistance(-0.01F, materialsData.N2OULL);//單排放源N2O排放95%信賴區間下限
                N2OUUL = (float)CalculateRoundDistance(0.01F, materialsData.N2OUUL);//單排放源N2O排放95%信賴區間上限
                UUL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2UUL, CH4UUL, N2OUUL);//單排放源排放95%信賴區間下限
                ULL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2ULL, CH4ULL, N2OULL);//單排放源排放95%信賴區間上限
                count_UUL = (float)(Math.Pow((UUL * all), 2));
                count_ULL = (float)(Math.Pow((ULL * all), 2));
            }
            else if (toUpdate.HFCS_Emission == true)
            {
                var materialsData = await _context.Materials
                    .Where(x => x.Name == toUpdate.Name)
                    .FirstOrDefaultAsync();

                var GWPData = await _context.GWPs.Where(x => x.Name == toUpdate.Material).FirstOrDefaultAsync();
                if (materialsData != null)
                {
                    HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * materialsData.HFCSCEF), 4);
                }
                else //其他都用工業冷媒計算
                {
                    HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.16), 4);
                }
                Grade = 3 * activityData.Level * activityData.Correction;
                all = (float)Math.Round(HFCS, 4);
            }
            else if (toUpdate.CH4_Emission == true) //化糞池 廢水處理
            {
                var materialsData = await _context.Materials
                    .Where(x => x.Name == toUpdate.Material)
                    .FirstOrDefaultAsync();//
                var GWPData = await _context.GWPs.Where(x => x.Name == "CH4").FirstOrDefaultAsync();

                // 如果找到 Materials 数据，计算 Emissions
                if (GWPData != null)
                {
                    CH4 = (float)(Math.Round(activityData.Num , 4) * materialsData.CH4CEF * GWPData.Num);
                    Grade = 3 * activityData.Level * activityData.Correction;
                    all = (float)Math.Round(CH4, 4);
                }
            }
            else if (toUpdate.CO2_Emission == true) //外購電力及製程
            {
                var AreaData = await _context.Areas.Where(x => x.Id == toUpdate.AreaId).FirstOrDefaultAsync(); //抓取廠區資料來比對基準年
                var materialsData = await _context.Materials.Where(x => x.Name == toUpdate.Material).FirstOrDefaultAsync();// 抓取對應的Materials 數據
                var eletronicData = await _context.Materials.OrderByDescending(x=>x.Year).Where(x => x.Name == toUpdate.Material && x.Year <= toUpdate.year).FirstOrDefaultAsync(); // 以遞減抓取電力排放係數年分，若小於或為該盤查年的排放係數，則採用

                // 如果找到 Materials 数据，计算 Emissions
                if (eletronicData != null)
                {
                    if (toUpdate.Material == "外購電力")
                    {
                        CO2 = CO2 = (float)Math.Round((Math.Round(activityData.Num / 1000.0, 4) * eletronicData.CO2CEF), 4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = CO2;
                        CO2ULL = (float)CalculateRoundDistance(-0.01F, eletronicData.CO2ULL);//單排放源CO2排放95%信賴區間下限
                        CO2UUL = (float)CalculateRoundDistance(0.01F, eletronicData.CO2UUL);//單排放源CO2排放95%信賴區間上限
                        CH4ULL = (float)CalculateRoundDistance(-0.01F, eletronicData.CH4ULL);//單排放源CH4排放95%信賴區間下限
                        CH4UUL = (float)CalculateRoundDistance(0.01F, eletronicData.CH4UUL);//單排放源CH4排放95%信賴區間上限
                        N2OULL = (float)CalculateRoundDistance(-0.01F, eletronicData.N2OULL);//單排放源N2O排放95%信賴區間下限
                        N2OUUL = (float)CalculateRoundDistance(0.01F, eletronicData.N2OUUL);//單排放源N2O排放95%信賴區間上限
                        UUL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2UUL, CH4UUL, N2OUUL);//單排放源排放95%信賴區間下限
                        ULL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2ULL, CH4ULL, N2OULL);//單排放源排放95%信賴區間上限
                        count_UUL = (float)(Math.Pow((UUL * all), 2));
                        count_ULL = (float)(Math.Pow((ULL * all), 2));
                    }
                }
                else if (materialsData != null) //製程
                {
                    CO2 = CO2 = (float)Math.Round((Math.Round(activityData.Num / 1000.0, 4) * materialsData.CO2CEF), 4);
                    Grade = 1 * activityData.Level * activityData.Correction;
                    all = (float)Math.Round(CO2, 4);
                }
            }

            if (toUpdate != null)
            {
                toUpdate.Num = activityData.Num;
                toUpdate.Unit = activityData.Unit;
                toUpdate.Source = activityData.Source;
                toUpdate.Dept = activityData.Dept;
                toUpdate.Level = activityData.Level;
                toUpdate.Correction = activityData.Correction;
                toUpdate.CO2 = CO2;
                toUpdate.CH4 = CH4;
                toUpdate.N2O = N2O;
                toUpdate.HFCS = HFCS;
                toUpdate.Emissions = all;
                toUpdate.Grade = Grade;
                toUpdate.UUL = UUL;
                toUpdate.ULL = ULL;
                toUpdate.count_UUL = count_UUL;
                toUpdate.count_ULL = count_ULL;
                toUpdate.ModifiedTime = DateTime.Now;
            }


            ViewData["Unit"] = new SelectList(unit);
            ViewData["Source"] = new SelectList(source);
            ViewData["Level"] = new SelectList(level);
            ViewData["Correction"] = new SelectList(correction);
            await _context.SaveChangesAsync();
            var AreaID = toUpdate.AreaId;
            return RedirectToAction("Index", "Devices", new { id = AreaID });
        }


        static double CalculateRoundDistance(float num1, float num2) // 計算兩數平方和的平方根，並四捨五入到小數點後5位
        {
            if (num1 != 0 && num2 != 0)
            {
                double distance = Math.Sqrt(Math.Pow(num1, 2) + Math.Pow(num2, 2));
                return Math.Round(distance, 5);
            }
            return 0;
        }
        static double CalculateAHorAG(double J, double R, double Z, double value1, double value2, double value3)
        {
            double numerator = Math.Sqrt(Math.Pow(J * value1, 2) + Math.Pow(R * value2, 2) + Math.Pow(Z * value3, 2));
            double denominator = J + R + Z;

            if (denominator != 0)
            {
                return numerator / denominator;
            }

            return value1;
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

        public async Task<IActionResult> Default(Guid id, int year)
        {
            
            await _context.Devices.AddAsync(new Device()
            {
                Id = Guid.NewGuid(),
                AreaId = id,
                year = year,
                Name = "緊急發電機",
                Material = "柴油",
                Scope = "類別一",
                EmissionPattern = "固定",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "公務車",
                Material = "柴油",
                Scope = "類別一",
                EmissionPattern = "移動",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "公務車",
                Material = "車用汽油",
                Scope = "類別一",
                EmissionPattern = "移動",
                CO2_Emission = true,
                CH4_Emission = true,
                N2O_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "冷氣機",
                Material = "R-410A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "飲水機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "乾燥機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "冰水主機",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "車用空調",
                Material = "R-134A",
                Scope = "類別一",
                EmissionPattern = "逸散",
                HFCS_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "化糞池",
                Material = "廢水處理",
                Scope = "類別一",
                EmissionPattern = "逸散",
                CH4_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.Devices.AddAsync(new Device()
            {
                Id = new Guid(),
                AreaId = id,
                year = year,
                Name = "電力",
                Material = "外購電力",
                Scope = "類別二",
                EmissionPattern = "外購電力",
                CO2_Emission = true,
                CreateTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
