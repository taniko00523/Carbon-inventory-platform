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
            "緊急發電機", "廚房", "公務車", "堆高機", "冷氣機", "冰箱","商用冰箱","中、大型冰箱","低溫冷凍車", "乾燥機", "飲水機", "冰水主機", "車用空調", "工業冷藏、冷凍","食品加工冷藏、冷凍", "CO2滅火器", "海龍滅火器", "FM200", "WD40", "化糞池", "瓦斯罐","焊條","乙炔","二氧化碳", "電力", "其他" };
        string[] unit = { "公斤", "公升", "立方公尺", "度", "人", "其他" };
        string[] source = { "發票", "領用單", "紀錄表", "繳費單" };
        string[] level = { "連續監測", "定期採樣", "自行評估" };
        string[] correction = { "每年外校一次以上量測", "每年外校不到一次量測", "非量測所得知數據" };
        //GET: Devices
        public async Task<IActionResult> Index(Guid id)
        {
            TempData["SelectedAreaId"] = id;
            TempData["Company"] = await _context.Areas
            .Where(a => a.Id == id)
            .Include(a=> a.Company)
            .Select(a => a.Company.Name)
            .FirstOrDefaultAsync();

            return _context.Devices != null ?
       View(await _context.Devices
       .Where(x => x.isDeleted != 1)
           .Where(m => m.AreaId == id)
           .ToListAsync()) :
       Problem("沒有找到資料表");
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
        public async Task<IActionResult> Create([Bind("Id,AreaId,AssetNo,Name,Provess,Scope,EmissionPattern,Material")] Device device)
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
                return RedirectToAction("Index","Devices", new { id = AreaID });
            }
            ViewData["name"] = new SelectList(name);
            ViewData["Material"] = new SelectList(await _context.Materials.Where(x => x.EmissionPattern == "固定").ToListAsync(), "Name", "Name");
            ViewData["AreasId"] = new SelectList(_context.Areas.Where(x => x.isDeleted == 0), "Id", "Name");
            ViewData["Scope"] = new SelectList(scope);
            ViewData["EmissionPattern"] = new SelectList(emissionPattern);
            return View(device);
        }

        [HttpGet]
        public JsonResult GetDeviceLevelAndCorrection(string selectedName,Guid id)
        {
            var deviceData = _context.deviceDatas
       .Where(x => x.Name == selectedName)
       .FirstOrDefault();

            var device = _context.Devices
                .Where(x=>x.Id == id)
                .FirstOrDefault();

            // 假設有一個名為 emission 的屬性，將其包含在回應中
            var result = new
            {
                level = deviceData?.Level,
                correction = deviceData?.Correction,
                unit = deviceData?.unit,
                num =  device.Num
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
                return RedirectToAction("Index","Devices", new { id = AreaID });
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
            if(toDelete.Num == 0)
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
                var GWPData = await _context.GWPs.Where(x => x.Name == toUpdate.Material).FirstOrDefaultAsync();

                if (GWPData != null)
                {
                    if (toUpdate.Name == "冰箱"|| toUpdate.Name == "飲水機")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.003), 4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                    if (toUpdate.Name == "商用冰箱")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.055), 4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                    if (toUpdate.Name == "中、大型冰箱")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.2), 4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                    if (toUpdate.Name == "低溫冷凍車")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.33), 4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                    if (toUpdate.Name == "乾燥機"|| toUpdate.Name == "工業冷藏、冷凍"|| toUpdate.Name == "食品加工冷藏、冷凍")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.16),4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                    if (toUpdate.Name == "冰水主機")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.09),4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                    if (toUpdate.Name == "冷氣機")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.03),4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                    if (toUpdate.Name == "車用空調")
                    {
                        HFCS = (float)Math.Round((double)(Math.Round(activityData.Num / 1000.0, 4) * GWPData.Num * 0.2), 4);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = (float)Math.Round(HFCS, 4);
                    }
                }
            }
            else if (toUpdate.CH4_Emission == true)
            {
                var materialsData = await _context.Materials
                    .Where(x => x.Name == toUpdate.Material)
                    .FirstOrDefaultAsync();//
                var GWPData = await _context.GWPs.Where(x => x.Name == "CH4").FirstOrDefaultAsync();

                // 如果找到 Materials 数据，计算 Emissions
                if (GWPData != null)
                {
                    CH4 = (float)(Math.Round(activityData.Num / 1000.0, 4) * materialsData.CH4CEF * GWPData.Num);
                    Grade = 3 * activityData.Level * activityData.Correction;
                    all = (float)Math.Round(CH4, 4);
                }
            }
            else if (toUpdate.CO2_Emission == true) //外購電力及製程
            {
                var AreaData = await _context.Areas.Where(x => x.Id == toUpdate.AreaId).FirstOrDefaultAsync(); //抓取廠區資料來比對基準年
                var materialsData = await _context.Materials.Where(x => x.Name == toUpdate.Material).FirstOrDefaultAsync();// 抓取對應的Materials 數據
                var eletronicData = await _context.Materials.Where(x => x.Name == toUpdate.Material).Where(x => x.Year == AreaData.Year).FirstOrDefaultAsync();// 抓取對應的Materials 數據

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
                else if(materialsData != null) //製程
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
                //toUpdate.ModifiedTime = DateTime.Now;
            }


            ViewData["Unit"] = new SelectList(unit);
            ViewData["Source"] = new SelectList(source);
            ViewData["Level"] = new SelectList(level);
            ViewData["Correction"] = new SelectList(correction);
            await _context.SaveChangesAsync();
            var AreaID = toUpdate.AreaId;
            return RedirectToAction("Index","Devices", new { id = AreaID });
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

        public async Task<IActionResult> HardWordAsync(Guid id)
        {

            // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
            var data = await _context.Areas.Where(x => x.Id == id).Include(x => x.Company).FirstOrDefaultAsync();
            var device = await _context.Devices.Where(x => x.AreaId == id).Where(x => x.isDeleted == 0).ToListAsync();
            var emission = await _context.emissions.Where(x => x.AreaId == id).FirstOrDefaultAsync();
            var nonMove = device.Where(d => d.EmissionPattern == "固定").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var move = device.Where(d => d.EmissionPattern == "移動").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var escape = device.Where(d => d.EmissionPattern == "逸散").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var process = device.Where(d => d.EmissionPattern == "製程").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();

            string Name = data.Company.Name;

            string filePath = "C:\\報告書\\複雜溫盤報告書範本.docx";
            string newFilePath = "C:\\報告書\\" + Name + "-溫室氣體盤查報告書" + ".docx";

            // 複製文件
            using (DocX doc = DocX.Load(filePath))
            {
                // 設定要查找和替換的文本
                foreach (var paragraph in doc.Paragraphs)
                {
                    //foreach (var emissionDevice in device.Select(x => x.EmissionPattern))
                    //{
                    //    if(emission)
                    //}
                    
                    paragraph.ReplaceText("補充公司基本資料", data.Company.Information);
                    paragraph.ReplaceText("補充盤查年度", (data.Year + 1911).ToString() + "年");
                    paragraph.ReplaceText("民國年份補充", (data.Year).ToString());
                    paragraph.ReplaceText("補充公司場所名稱", data.Company.Name);
                    paragraph.ReplaceText("補充公司場所英文名稱", data.Company.EnglishName);
                    paragraph.ReplaceText("補充公司場所簡稱", data.Company.EasyName);
                    paragraph.ReplaceText("補充公司場所英文簡稱", data.Company.EasyEnglishName);
                    paragraph.ReplaceText("補充統一編號", data.UniqueCode.ToString());
                    paragraph.ReplaceText("補充工廠登記編號", data.FactorCode.ToString());
                    paragraph.ReplaceText("補充地址", data.City + data.District + data.Address);
                    if (nonMove.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源。", "組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源，固定排放源包含" + string.Join(", ", nonMove) + "。");
                    }
                    if (move.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源。", "組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源，移動排放源包含" + string.Join(", ", move) + "。");
                    }
                    if (escape.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的人為逸散溫室氣體排放源。", "組織邊界的各據點內所擁有的人為逸散溫室氣體排放源，逸散源包含" + string.Join(", ", escape) + "。");
                    }
                    if (process.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界內在製程中化學反應產生的溫室氣體排放源。", "組織邊界內在製程中化學反應產生的溫室氣體排放源，如" + string.Join(", ", process) + "。");
                    }
                    paragraph.ReplaceText("基準年補充", (data.Year + 1911).ToString() + "年");
                    paragraph.ReplaceText("類別一CO2排放", emission.Scope1_CO2);
                    paragraph.ReplaceText("CO2排放", emission.CO2);
                    paragraph.ReplaceText("CH4排放", emission.CH4);
                    paragraph.ReplaceText("N2O排放", emission.N2O);
                    paragraph.ReplaceText("HFCS排放", emission.HFCS);
                    paragraph.ReplaceText("PFCS排放", emission.PFCS);
                    paragraph.ReplaceText("SF6排放", emission.SF6);
                    paragraph.ReplaceText("NF3排放", emission.NF3);
                    paragraph.ReplaceText("類別一CO2占比", emission.percentage1_CO2);
                    paragraph.ReplaceText("類別一CH4占比", emission.percentage1_CH4);
                    paragraph.ReplaceText("類別一N2O占比", emission.percentage1_N2O);
                    paragraph.ReplaceText("類別一HFCS占比", emission.percentage1_HFCS);
                    paragraph.ReplaceText("類別一PFCS占比", emission.percentage1_PFCS);
                    paragraph.ReplaceText("類別一SF6占比", emission.percentage1_SF6);
                    paragraph.ReplaceText("類別一NF3占比", emission.percentage2_NF3);
                    paragraph.ReplaceText("類別一CO2排放", emission.Scope1);
                    paragraph.ReplaceText("CO2占比", emission.percentage2_CO2);
                    paragraph.ReplaceText("CH4占比", emission.percentage2_CH4);
                    paragraph.ReplaceText("N2O占比", emission.percentage2_N2O);
                    paragraph.ReplaceText("HFCS占比", emission.percentage2_HFCS);
                    paragraph.ReplaceText("PFCS占比", emission.percentage2_PFCS);
                    paragraph.ReplaceText("SF6占比", emission.percentage2_SF6);
                    paragraph.ReplaceText("NF3占比", emission.percentage2_NF3);
                    paragraph.ReplaceText("總排放當量", emission.All);
                    paragraph.ReplaceText("固定排放量", emission.non_move);
                    paragraph.ReplaceText("移動排放量", emission.move);
                    paragraph.ReplaceText("製程排放量", emission.process);
                    paragraph.ReplaceText("逸散排放量", emission.escape);
                    paragraph.ReplaceText("固定排放比例", emission.percentage_nonMove);
                    paragraph.ReplaceText("製程排放比例", emission.percentage_Process);
                    paragraph.ReplaceText("移動排放比例", emission.percentage_Move);
                    paragraph.ReplaceText("逸散排放比例", emission.percentage_Escape);
                    paragraph.ReplaceText("類別一占比", emission.percentage_Scope1);
                    paragraph.ReplaceText("類別二占比", emission.percentage_Scope2);
                    paragraph.ReplaceText("類別一總排放", emission.Scope1);
                    paragraph.ReplaceText("類別二總排放", emission.Scope2);
                    paragraph.ReplaceText("進行評估排放當量", emission.cal_all);
                    paragraph.ReplaceText("不確定性評估占比", emission.percentage_CalAll);
                    paragraph.ReplaceText("第1級評分", emission.no1_Grade);
                    paragraph.ReplaceText("第2級評分", emission.no2_Grade);
                    paragraph.ReplaceText("第3級評分", emission.no3_Grade);
                    paragraph.ReplaceText("清冊等級分數補充", emission.avg_Grade);
                    paragraph.ReplaceText("清冊級別補充", emission.all_Grade);
                    paragraph.ReplaceText("95上", emission.UUL);
                    paragraph.ReplaceText("95下", emission.ULL);
                    paragraph.ReplaceText("補充姓名", data.Company.Owner);
                    paragraph.ReplaceText("補充電話", data.Company.Phone);
                    paragraph.ReplaceText("補充電子信箱", data.Company.Email);



                }

                // 保存新文檔
                doc.SaveAs(newFilePath);
            }
            TempData["Finish"] = "已產生" + data.Company.Name + "報告書";
            // 返回一個視圖或其他操作，根據你的需求
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> WordAsync(Guid id)
        {

            // 這裡要替換成你 MVC 應用程式中正確的檔案路徑
            var data = await _context.Areas.Where(x => x.Id == id).Include(x => x.Company).FirstOrDefaultAsync();
            var device = await _context.Devices.Where(x => x.AreaId == id).Where(x => x.isDeleted == 0).ToListAsync();
            var emission = await _context.emissions.Where(x => x.AreaId == id).FirstOrDefaultAsync();
            var nonMove = device.Where(d => d.EmissionPattern == "固定").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var move = device.Where(d => d.EmissionPattern == "移動").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var escape = device.Where(d => d.EmissionPattern == "逸散").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();
            var process = device.Where(d => d.EmissionPattern == "製程").Where(x => x.isDeleted == 0).Select(d => d.Name).ToList();

            string Name = data.Company.Name;

            string filePath = "C:\\報告書\\溫室氣體盤查報告書範本.docx";
            string newFilePath = "C:\\報告書\\" + Name + "-溫室氣體盤查報告書" + ".docx";

            // 複製文件
            using (DocX doc = DocX.Load(filePath))
            {
                // 設定要查找和替換的文本
                foreach (var paragraph in doc.Paragraphs)
                {

                    paragraph.ReplaceText("補充公司基本資料", data.Company.Information);
                    paragraph.ReplaceText("補充盤查年度", (data.Year + 1911).ToString() + "年");
                    paragraph.ReplaceText("民國年份補充", (data.Year).ToString());
                    paragraph.ReplaceText("補充公司場所名稱", data.Company.Name);
                    paragraph.ReplaceText("補充統一編號", data.UniqueCode.ToString());
                    paragraph.ReplaceText("補充工廠登記編號", data.FactorCode.ToString());
                    paragraph.ReplaceText("補充地址", data.City + data.District + data.Address);
                    if (nonMove.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源。", "組織邊界的各據點內所擁有的固定式化石燃料燃燒排放源，固定排放源包含" + string.Join(", ", nonMove) + "。");
                    }
                    if (move.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源。", "組織邊界的各據點內所擁有的可移動且燃燒化石燃料的排放源，移動排放源包含" + string.Join(", ", move) + "。");
                    }
                    if (escape.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界的各據點內所擁有的人為逸散溫室氣體排放源。", "組織邊界的各據點內所擁有的人為逸散溫室氣體排放源，逸散源包含" + string.Join(", ", escape) + "。");
                    }
                    if (process.Count != 0)
                    {
                        paragraph.ReplaceText("組織邊界內在製程中化學反應產生的溫室氣體排放源。", "組織邊界內在製程中化學反應產生的溫室氣體排放源，如" + string.Join(", ", process) + "。");
                    }
                    paragraph.ReplaceText("基準年補充", (data.Year + 1911).ToString() + "年");
                    paragraph.ReplaceText("類別一CO2排放", emission.Scope1_CO2);
                    paragraph.ReplaceText("CO2排放", emission.CO2);
                    paragraph.ReplaceText("CH4排放", emission.CH4);
                    paragraph.ReplaceText("N2O排放", emission.N2O);
                    paragraph.ReplaceText("HFCS排放", emission.HFCS);
                    paragraph.ReplaceText("PFCS排放", emission.PFCS);
                    paragraph.ReplaceText("SF6排放", emission.SF6);
                    paragraph.ReplaceText("NF3排放", emission.NF3);
                    paragraph.ReplaceText("類別一CO2占比", emission.percentage1_CO2);
                    paragraph.ReplaceText("類別一CH4占比", emission.percentage1_CH4);
                    paragraph.ReplaceText("類別一N2O占比", emission.percentage1_N2O);
                    paragraph.ReplaceText("類別一HFCS占比", emission.percentage1_HFCS);
                    paragraph.ReplaceText("類別一PFCS占比", emission.percentage1_PFCS);
                    paragraph.ReplaceText("類別一SF6占比", emission.percentage1_SF6);
                    paragraph.ReplaceText("類別一NF3占比", emission.percentage2_NF3);
                    paragraph.ReplaceText("類別一CO2排放", emission.Scope1);
                    paragraph.ReplaceText("CO2占比", emission.percentage2_CO2);
                    paragraph.ReplaceText("CH4占比", emission.percentage2_CH4);
                    paragraph.ReplaceText("N2O占比", emission.percentage2_N2O);
                    paragraph.ReplaceText("HFCS占比", emission.percentage2_HFCS);
                    paragraph.ReplaceText("PFCS占比", emission.percentage2_PFCS);
                    paragraph.ReplaceText("SF6占比", emission.percentage2_SF6);
                    paragraph.ReplaceText("NF3占比", emission.percentage2_NF3);
                    paragraph.ReplaceText("總排放當量", emission.All);
                    paragraph.ReplaceText("固定排放量", emission.non_move);
                    paragraph.ReplaceText("移動排放量", emission.move);
                    paragraph.ReplaceText("製程排放量", emission.process);
                    paragraph.ReplaceText("逸散排放量", emission.escape);
                    paragraph.ReplaceText("固定排放比例", emission.percentage_nonMove);
                    paragraph.ReplaceText("製程排放比例", emission.percentage_Process);
                    paragraph.ReplaceText("移動排放比例", emission.percentage_Move);
                    paragraph.ReplaceText("逸散排放比例", emission.percentage_Escape);
                    paragraph.ReplaceText("類別一占比", emission.percentage_Scope1);
                    paragraph.ReplaceText("類別二占比", emission.percentage_Scope2);
                    paragraph.ReplaceText("類別一總排放", emission.Scope1);
                    paragraph.ReplaceText("類別二總排放", emission.Scope2);
                    paragraph.ReplaceText("進行評估排放當量", emission.cal_all);
                    paragraph.ReplaceText("不確定性評估占比", emission.percentage_CalAll);
                    paragraph.ReplaceText("第1級評分", emission.no1_Grade);
                    paragraph.ReplaceText("第2級評分", emission.no2_Grade);
                    paragraph.ReplaceText("第3級評分", emission.no3_Grade);
                    paragraph.ReplaceText("清冊等級分數補充", emission.avg_Grade);
                    paragraph.ReplaceText("清冊級別補充", emission.all_Grade);
                    paragraph.ReplaceText("95上", emission.UUL);
                    paragraph.ReplaceText("95下", emission.ULL);
                    paragraph.ReplaceText("補充姓名", data.Company.Owner);
                    paragraph.ReplaceText("補充電話", data.Company.Phone);
                    paragraph.ReplaceText("補充電子信箱", data.Company.Email);



                }

                // 保存新文檔
                doc.SaveAs(newFilePath);
            }
            TempData["Finish"] = "已產生" + data.Company.Name + "報告書";
            // 返回一個視圖或其他操作，根據你的需求
            return RedirectToAction("Index", "Home");
        }
    }
}
