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
            "緊急發電機", "廚房", "公務車", "堆高機", "冷氣機", "冰箱", "乾燥機", "飲水機", "冰水主機", "車用空調", "工業冷媒", "CO2滅火器", "海龍滅火器", "FM200", "WD40", "化糞池", "瓦斯罐", "電力", "其他" };
        string[] unit = { "公斤", "公升", "立方公尺", "度", "人", "其他" };
        string[] source = { "發票", "領用單", "紀錄表", "繳費單" };
        string[] level = { "連續監測", "定期採樣", "自行評估" };
        string[] correction = { "每年外校一次以上量測", "每年外校不到一次量測", "非量測所得知數據" };
        //GET: Devices
        public async Task<IActionResult> Index(Guid id)
        {
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
    .FirstOrDefault() != null)
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
                    if (_context.GWPs
    .Where(x => x.Name == device.Material)
    .Select(x => x.Name)
    .FirstOrDefault() == "CH4")
                    {
                        toCreate.CH4_Emission = true;
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
            if (toDelete != null)
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
                    CO2 = (float)(activityData.Num / 1000 * materialsData.CO2CEF * 1);
                    CH4 = (float)(activityData.Num / 1000 * materialsData.CH4CEF * CH4_GWP);
                    N2O = (float)(activityData.Num / 1000 * materialsData.N2OCEF * N2O_GWP);
                    Grade = 3 * activityData.Level * activityData.Correction;
                    all = CO2 + CH4 + N2O;
                }
                CO2ULL = (float)CalculateRoundDistance(-0.01F, materialsData.CO2ULL);//單排放源CO2排放95%信賴區間下限
                CO2UUL = (float)CalculateRoundDistance(0.01F, materialsData.CO2UUL);//單排放源CO2排放95%信賴區間上限
                CH4ULL = (float)CalculateRoundDistance(-0.01F, materialsData.CH4ULL);//單排放源CH4排放95%信賴區間下限
                CH4UUL = (float)CalculateRoundDistance(0.01F, materialsData.CH4UUL);//單排放源CH4排放95%信賴區間上限
                N2OULL = (float)CalculateRoundDistance(-0.01F, materialsData.N2OULL);//單排放源N2O排放95%信賴區間下限
                N2OUUL = (float)CalculateRoundDistance(0.01F, materialsData.N2OUUL);//單排放源N2O排放95%信賴區間上限
                UUL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2UUL, CH4UUL, N2OUUL);//單排放源排放95%信賴區間下限
                ULL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2ULL, CH4ULL, N2OULL);//單排放源排放95%信賴區間上限
                count_UUL = (float)(Math.Pow((UUL * activityData.Emissions), 2));
                count_ULL = (float)(Math.Pow((ULL * activityData.Emissions), 2));
            }
            else if (toUpdate.HFCS_Emission == true)
            {
                var GWPData = await _context.GWPs.Where(x => x.Name == toUpdate.Material).FirstOrDefaultAsync();

                if (GWPData != null)
                {
                    if (toUpdate.Name == "冰箱")
                    {
                        HFCS = (float)(activityData.Num / 1000 * GWPData.Num * 0.003);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = HFCS;
                    }
                    if (toUpdate.Name == "乾燥機")
                    {
                        HFCS = (float)(activityData.Num / 1000 * GWPData.Num * 0.16);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = HFCS;
                    }
                    if (toUpdate.Name == "工業冷媒")
                    {
                        HFCS = (float)(activityData.Num / 1000 * GWPData.Num * 0.16);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = HFCS;
                    }
                    if (toUpdate.Name == "冰水主機")
                    {
                        HFCS = (float)(activityData.Num / 1000 * GWPData.Num * 0.09);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = HFCS;
                    }
                    if (toUpdate.Name == "冷氣機")
                    {
                        HFCS = (float)(activityData.Num / 1000 * GWPData.Num * 0.03);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = HFCS;
                    }
                    if (toUpdate.Name == "飲水機")
                    {
                        HFCS = (float)(activityData.Num / 1000 * GWPData.Num * 0.003);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = HFCS;
                    }
                    if (toUpdate.Name == "車用空調")
                    {
                        HFCS = (float)(activityData.Num / 1000 * GWPData.Num * 0.2);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = HFCS;
                    }
                }
            }
            else if (toUpdate.CH4_Emission == true)
            {
                var GWPData = await _context.GWPs.Where(x => x.Name == "CH4").FirstOrDefaultAsync();

                // 如果找到 Materials 数据，计算 Emissions
                if (GWPData != null)
                {
                    CH4 = (float)(activityData.Num / 1000 * 0.002546062 * GWPData.Num);
                    Grade = 3 * activityData.Level * activityData.Correction;
                    all = CH4;
                }
            }
            else if (toUpdate.CO2_Emission == true) //外購電力及製程
            {
                var AreaData = await _context.Areas.Where(x => x.Id == toUpdate.AreaId).FirstOrDefaultAsync(); //抓取廠區資料來比對基準年
                var materialsData = await _context.Materials.Where(x => x.Name == toUpdate.Material).Where(x => x.Year == AreaData.Year).FirstOrDefaultAsync();// 抓取對應的Materials 數據

                // 如果找到 Materials 数据，计算 Emissions
                if (materialsData != null)
                {
                    if (toUpdate.Material == "外購電力")
                    {
                        CO2 = (float)(activityData.Num / 1000 * materialsData.CO2CEF);
                        Grade = 3 * activityData.Level * activityData.Correction;
                        all = CO2;
                        CO2ULL = (float)CalculateRoundDistance(-0.01F, materialsData.CO2ULL);//單排放源CO2排放95%信賴區間下限
                        CO2UUL = (float)CalculateRoundDistance(0.01F, materialsData.CO2UUL);//單排放源CO2排放95%信賴區間上限
                        CH4ULL = (float)CalculateRoundDistance(-0.01F, materialsData.CH4ULL);//單排放源CH4排放95%信賴區間下限
                        CH4UUL = (float)CalculateRoundDistance(0.01F, materialsData.CH4UUL);//單排放源CH4排放95%信賴區間上限
                        N2OULL = (float)CalculateRoundDistance(-0.01F, materialsData.N2OULL);//單排放源N2O排放95%信賴區間下限
                        N2OUUL = (float)CalculateRoundDistance(0.01F, materialsData.N2OUUL);//單排放源N2O排放95%信賴區間上限
                        UUL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2UUL, CH4UUL, N2OUUL);//單排放源排放95%信賴區間下限
                        ULL = (float)CalculateAHorAG(CO2, CH4, N2O, CO2ULL, CH4ULL, N2OULL);//單排放源排放95%信賴區間上限
                        count_UUL = (float)(Math.Pow((UUL * activityData.Emissions), 2));
                        count_ULL = (float)(Math.Pow((ULL * activityData.Emissions), 2));
                    }
                    else //製程
                    {
                        CO2 = (float)(activityData.Num / 1000 * materialsData.CO2CEF);
                        Grade = 1 * activityData.Level * activityData.Correction;
                        all = CO2;
                    }
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
                //toUpdate.ModifiedTime = DateTime.Now;
            }


            ViewData["Unit"] = new SelectList(unit);
            ViewData["Source"] = new SelectList(source);
            ViewData["Level"] = new SelectList(level);
            ViewData["Correction"] = new SelectList(correction);
            await _context.SaveChangesAsync();
            var AreaID = activityData.AreaId;
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
            double numerator = Math.Sqrt(Math.Pow(J * value1, 2) + Math.Pow(R * value2, 2));

            if (Z != 0)
            {
                numerator += Math.Pow(Z * value3, 2);
            }

            double denominator = J + R + Z;

            if (denominator != 0)
            {
                return numerator / denominator;
            }

            return value1;
        }

        static double CalculateAI(double num, double J, double R, double Z)
        {
            if (num != 0)
            {
                double sum = J + R + Z;
                return Math.Pow(num * sum, 2);
            }
            return 0;
        }

        //static double CalculateFinalResult(double[] values)
        //{
        //    double sum = values.Sum();
        //    return Math.Sqrt(sum) / (AK + AL + AM);
        //}
    }
}
