using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Carbon_inventory_platform.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Carbon_inventory_platform.Controllers
{
    public class CountController : Controller
    {
        // A3：Excel 匯入排放源時，這裡的計算方法會「每一筆排放源」呼叫一次，原本每次都把
        // 整張 GWP 表載入記憶體。GWP 幾乎不會變動，快取起來即可；GWPController 的
        // Add/Edit/DeleteConfirmed 存檔成功後會清除這個快取鍵，確保後台改了 GWP 值後立即生效。
        public const string GwpCacheKey = "CountController:GWPs";

        private readonly ApplicationDbContext _context;
        public CountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 不改建構子(三個子控制器都繼承本類別)，直接向請求範圍要服務，取不到就跳過／退回查資料庫。
        // protected: 讓 Devices/Emission/Areas 三個子類別也能用。
        [NonAction]
        protected void WarnMissingFactor(string message)
        {
            var logger = HttpContext?.RequestServices?.GetService(typeof(ILogger<CountController>)) as ILogger<CountController>;
            logger?.LogWarning("{Message}", message);
        }

        [NonAction]
        protected async Task<List<GWP>> GetGWPListAsync()
        {
            var cache = HttpContext?.RequestServices?.GetService(typeof(IMemoryCache)) as IMemoryCache;
            if (cache == null)
            {
                return await _context.GWPs.AsNoTracking().ToListAsync();
            }
            if (cache.TryGetValue(GwpCacheKey, out List<GWP>? cached) && cached != null)
            {
                return cached;
            }
            // AsNoTracking 在這裡不只是省追蹤開銷：這份清單會被快取 30 分鐘、跨請求／跨
            // DbContext 執行個體重複使用，若是追蹤中的實體，會被綁在早就處置(Dispose)的
            // DbContext 上，之後任何人碰到都可能出問題。
            var list = await _context.GWPs.AsNoTracking().ToListAsync();
            cache.Set(GwpCacheKey, list, TimeSpan.FromMinutes(30));
            return list;
        }

        // B5：Areas/Devices/Emission 三個子類別共用，判斷這個廠區是否已鎖定。
        // 沿用 WarnMissingFactor／GetGWPListAsync 的作法：不改建構子，直接向請求範圍要服務。
        [NonAction]
        protected async Task<bool> IsAreaLockedAsync(Guid? areaId)
        {
            var lockService = HttpContext?.RequestServices?.GetService(typeof(AreaLockService)) as AreaLockService;
            if (lockService == null)
            {
                return false;
            }
            return await lockService.IsAreaLockedAsync(areaId);
        }

        [NonAction]
        protected async Task<bool> IsAreaLockedByDeviceIdAsync(Guid? deviceId)
        {
            var lockService = HttpContext?.RequestServices?.GetService(typeof(AreaLockService)) as AreaLockService;
            if (lockService == null)
            {
                return false;
            }
            return await lockService.IsAreaLockedByDeviceIdAsync(deviceId);
        }

        // 這三個純數學函式已抽到 Services/EmissionMath.cs（方便直接寫單元測試，
        // 不用透過 ApplicationDbContext），這裡保留同名方法只是委派過去，
        // 檔案內另外 27 處呼叫端完全不用修改。
        public static decimal DecimalSqrt(decimal value) => EmissionMath.DecimalSqrt(value);

        [NonAction]
        public decimal CalculateRoundDistance(decimal num1, decimal num2) => EmissionMath.CalculateRoundDistance(num1, num2);

        [NonAction]
        public decimal Calculate95U(decimal GHG1, decimal GHG2, decimal GHG3, decimal GHG1UUL, decimal GHG2UUL, decimal GHG3UUL)
            => EmissionMath.Calculate95U(GHG1, GHG2, GHG3, GHG1UUL, GHG2UUL, GHG3UUL);
        [NonAction]
        public async Task<bool> CEFAddAsync(Device device, string GHG, decimal? CEF) //自訂排碳係數
        {
            int ARVersion = await _context.Areas.Where(x => x.Id == device.AreaId).Select(x => x.ARVersion).FirstOrDefaultAsync();
            var GWP = await GetGWPListAsync();
            if (GWP == null)
            {
                return false;
            }
            if (ModelState.IsValid)
            {
                var toCreate = new GHG();
                toCreate.Id = Guid.NewGuid();
                toCreate.Name = GHG;
                toCreate.DeviceId = device.Id;
                toCreate.CEF = (decimal)CEF;
                if (GHG == "HFCS")
                {
                    decimal hfcsGWP = 0;
                    for (int i = ARVersion; i > 0; i--) // 找HFCS的GWP，如果沒有找到該版本的GWP則降版本
                    {
                        // 原本條件寫 x.ARVersion == ARVersion，每一圈都查同一個版本，會導致降版本的備援機制完全沒作用(GWP留在0)。
                        hfcsGWP = GWP.Where(x => x.Name == device.Material && x.ARVersion == i).Select(x => x.Num).FirstOrDefault();
                        if (hfcsGWP != 0)
                        {
                            break;
                        }

                    }
                    toCreate.GWP = hfcsGWP;
                }
                else if (GHG == "CH4" && device.Name == "化糞池") //生物甲烷
                {
                    decimal ch4GWP;
                    switch (ARVersion)
                    {
                        case 2:
                            ch4GWP = 21;
                            break;
                        case 3:
                            ch4GWP = 23;
                            break;
                        case 4:
                            ch4GWP = 25;
                            break;
                        case 5:
                            ch4GWP = 28;
                            break;
                        default:
                            ch4GWP = toCreate.GWP = GWP.Where(x => x.Name == "CH4" && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault(); //石化甲烷
                            break;
                    }
                    toCreate.GWP = ch4GWP;
                }
                else //其他GHG
                {
                    toCreate.GWP = GWP.Where(x => x.Name == GHG && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                }
                if (toCreate.GWP == 0) // 查不到GWP時原本就靜靜地存成0，整個氣體的排放量會變成0而且畫面上完全看不出來，至少留下警告紀錄。
                {
                    WarnMissingFactor($"找不到 {GHG} 在 AR{ARVersion} 的 GWP 值(排放源:{device.Name}、物質:{device.Material})，排放量會被算成0。");
                }
                toCreate.CreateTime = DateTime.Now;
                device.CEF_Correction = 1;//輸入?
                _context.Add(toCreate);
                await _context.SaveChangesAsync();

            }

            return true;
        }
        [NonAction]
        public async Task<GHG?> GHGCheckAsync(Device device, string deviceName, string material, string scope, string emisspatern, int year, int ARVersion) //設定資料庫排碳係數及GWP
        {
            var GWP = await GetGWPListAsync();
            var MaterialList = await _context.Materials.AsNoTracking()
                                    .Where(x => x.Name == material && x.Scope == scope && x.EmissionPattern == emisspatern && x.Year <= year).ToListAsync();
            var Material = MaterialList.OrderByDescending(x => x.Year).FirstOrDefault();

            var otherMaterial = await _context.Materials.AsNoTracking().Where(x => x.Name == deviceName && x.Scope == scope && x.EmissionPattern == emisspatern).FirstOrDefaultAsync(); //目前只有冷媒設備，但我包含了PFCS以防萬一
            if (ModelState.IsValid)
            {
                if (Material != null)
                {
                    if (Material.CO2CEF != 0)
                    {
                        var toCreateCO2 = new GHG();
                        toCreateCO2.Id = Guid.NewGuid();
                        toCreateCO2.Name = "CO2";
                        toCreateCO2.DeviceId = device.Id;
                        toCreateCO2.CEF = Material.CO2CEF;
                        toCreateCO2.CEF_UUL = Material.CO2UUL;
                        toCreateCO2.CEF_ULL = Material.CO2ULL;
                        toCreateCO2.GWP = GWP.Where(x => x.Name == "CO2" && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                        toCreateCO2.all_UUL = CalculateRoundDistance(Material.CO2UUL, Material.DataUUL);
                        toCreateCO2.all_ULL = CalculateRoundDistance(Material.CO2ULL, Material.DataULL);
                        toCreateCO2.CreateTime = DateTime.Now;
                        _context.Add(toCreateCO2); // 原本用 AddRange 傳單一實體，和其他氣體的寫法不一致，改回 Add。
                        device.CEF_Correction = Material.CEF_Correction;
                        device.data_UUL = Material.DataUUL;
                        device.data_ULL = Material.DataULL;

                    }
                    if (Material.CH4CEF != 0)
                    {
                        var toCreateCH4 = new GHG();
                        if (deviceName == "化糞池") // 生物甲烷
                        {
                            decimal ch4GWP;
                            switch (ARVersion)
                            {
                                case 2:
                                    ch4GWP = 21;
                                    break;
                                case 3:
                                    ch4GWP = 23;
                                    break;
                                case 4:
                                    ch4GWP = 25;
                                    break;
                                case 5:
                                    ch4GWP = 28;
                                    break;
                                default:
                                    ch4GWP = toCreateCH4.GWP = GWP.Where(x => x.Name == "CH4" && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                                    break;
                            }
                            toCreateCH4.GWP = ch4GWP;
                        }
                        else // 石化甲烷
                        {
                            toCreateCH4.GWP = GWP.Where(x => x.Name == "CH4" && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                        }
                        toCreateCH4.Id = Guid.NewGuid();
                        toCreateCH4.Name = "CH4";
                        toCreateCH4.DeviceId = device.Id;
                        toCreateCH4.CEF = Material.CH4CEF;
                        toCreateCH4.CEF_UUL = Material.CH4UUL;
                        toCreateCH4.CEF_ULL = Material.CH4ULL;
                        toCreateCH4.all_UUL = CalculateRoundDistance(Material.CH4UUL, Material.DataUUL);
                        toCreateCH4.all_ULL = CalculateRoundDistance(Material.CH4ULL, Material.DataULL);
                        toCreateCH4.CreateTime = DateTime.Now;
                        _context.Add(toCreateCH4);
                        device.CEF_Correction = Material.CEF_Correction;
                        device.data_UUL = Material.DataUUL;
                        device.data_ULL = Material.DataULL;
                    }
                    if (Material.N2OCEF != 0)
                    {
                        var toCreateN2O = new GHG();
                        toCreateN2O.Id = Guid.NewGuid();
                        toCreateN2O.Name = "N2O";
                        toCreateN2O.DeviceId = device.Id;
                        toCreateN2O.CEF = Material.N2OCEF;
                        toCreateN2O.CEF_UUL = Material.N2OUUL;
                        toCreateN2O.CEF_ULL = Material.N2OULL;
                        toCreateN2O.GWP = GWP.Where(x => x.Name == "N2O" && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                        toCreateN2O.all_UUL = CalculateRoundDistance(Material.N2OUUL, Material.DataUUL);
                        toCreateN2O.all_ULL = CalculateRoundDistance(Material.N2OULL, Material.DataULL);
                        toCreateN2O.CreateTime = DateTime.Now;
                        _context.Add(toCreateN2O);
                        device.CEF_Correction = Material.CEF_Correction;
                        device.data_UUL = Material.DataUUL;
                        device.data_ULL = Material.DataULL;
                    }
                    if (Material.HFCSCEF != 0)
                    {
                        var toCreateHFCS = new GHG();
                        toCreateHFCS.Id = Guid.NewGuid();
                        toCreateHFCS.Name = "HFCS";
                        toCreateHFCS.DeviceId = device.Id;
                        toCreateHFCS.CEF = Material.HFCSCEF;
                        toCreateHFCS.CEF_UUL = Material.HFCSUUL;
                        toCreateHFCS.CEF_ULL = Material.HFCSULL;
                        toCreateHFCS.GWP = GWP.Where(x => x.Name == material && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                        toCreateHFCS.all_UUL = CalculateRoundDistance(Material.HFCSUUL, Material.DataUUL);
                        toCreateHFCS.all_ULL = CalculateRoundDistance(Material.HFCSULL, Material.DataULL);
                        toCreateHFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreateHFCS);
                        device.CEF_Correction = Material.CEF_Correction;
                        device.data_UUL = Material.DataUUL;
                        device.data_ULL = Material.DataULL;
                    }
                    if (Material.PFCSCEF != 0)
                    {
                        var toCreatePFCS = new GHG();
                        toCreatePFCS.Id = Guid.NewGuid();
                        toCreatePFCS.Name = "PFCS";
                        toCreatePFCS.DeviceId = device.Id;
                        toCreatePFCS.CEF = Material.PFCSCEF;
                        toCreatePFCS.CEF_UUL = Material.PFCSUUL;
                        toCreatePFCS.CEF_ULL = Material.PFCSULL;
                        toCreatePFCS.GWP = GWP.Where(x => x.Name == material && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                        toCreatePFCS.all_UUL = CalculateRoundDistance(Material.PFCSUUL, Material.DataUUL);
                        toCreatePFCS.all_ULL = CalculateRoundDistance(Material.PFCSULL, Material.DataULL);
                        toCreatePFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreatePFCS);
                        device.CEF_Correction = Material.CEF_Correction;
                        device.data_UUL = Material.DataUUL;
                        device.data_ULL = Material.DataULL;
                    }
                    if (Material.SF6CEF != 0)
                    {
                        var toCreateSF6 = new GHG();
                        toCreateSF6.Id = Guid.NewGuid();
                        toCreateSF6.Name = "SF6";
                        toCreateSF6.DeviceId = device.Id;
                        toCreateSF6.CEF = Material.SF6CEF;
                        toCreateSF6.CEF_UUL = Material.SF6UUL;
                        toCreateSF6.CEF_ULL = Material.SF6ULL;
                        toCreateSF6.GWP = GWP.Where(x => x.Name == "SF6" && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                        toCreateSF6.all_UUL = CalculateRoundDistance(Material.SF6UUL, Material.DataUUL);
                        toCreateSF6.all_ULL = CalculateRoundDistance(Material.SF6ULL, Material.DataULL);
                        toCreateSF6.CreateTime = DateTime.Now;
                        _context.Add(toCreateSF6);
                        device.CEF_Correction = Material.CEF_Correction;
                        device.data_UUL = Material.DataUUL;
                        device.data_ULL = Material.DataULL;
                    }
                    if (Material.NF3CEF != 0)
                    {
                        var toCreateNF3 = new GHG();
                        toCreateNF3.Id = Guid.NewGuid();
                        toCreateNF3.Name = "NF3";
                        toCreateNF3.DeviceId = device.Id;
                        toCreateNF3.CEF = Material.NF3CEF;
                        toCreateNF3.CEF_UUL = Material.NF3UUL;
                        toCreateNF3.CEF_ULL = Material.NF3ULL;
                        toCreateNF3.GWP = GWP.Where(x => x.Name == "NF3" && x.ARVersion == ARVersion).Select(x => x.Num).FirstOrDefault();
                        toCreateNF3.all_UUL = CalculateRoundDistance(Material.NF3UUL, Material.DataUUL);
                        toCreateNF3.all_ULL = CalculateRoundDistance(Material.NF3ULL, Material.DataULL);
                        toCreateNF3.CreateTime = DateTime.Now;
                        _context.Add(toCreateNF3);
                        device.CEF_Correction = Material.CEF_Correction;
                        device.data_UUL = Material.DataUUL;
                        device.data_ULL = Material.DataULL;
                    }
                }
                else if (otherMaterial != null) //目前只有冷媒設備，但我包含了PFCS以防萬一
                {
                    if (otherMaterial.HFCSCEF != 0)
                    {
                        var toCreateHFCS = new GHG();
                        toCreateHFCS.Id = Guid.NewGuid();
                        toCreateHFCS.Name = "HFCS";
                        toCreateHFCS.DeviceId = device.Id;
                        toCreateHFCS.CEF = otherMaterial.HFCSCEF;
                        toCreateHFCS.CEF_UUL = otherMaterial.HFCSUUL;
                        toCreateHFCS.CEF_ULL = otherMaterial.HFCSULL;
                        decimal hfcsGWP = 0;
                        for (int i = ARVersion; i > 0; i--) // 找HFCS的GWP，如果沒有找到該版本的GWP則降版本
                        {
                            hfcsGWP = GWP.Where(x => x.Name == material && x.ARVersion == i).Select(x => x.Num).FirstOrDefault();
                            if (hfcsGWP != 0)
                            {
                                break;
                            }

                        }
                        toCreateHFCS.GWP = hfcsGWP;
                        toCreateHFCS.all_UUL = CalculateRoundDistance(otherMaterial.HFCSUUL, otherMaterial.DataUUL);
                        toCreateHFCS.all_ULL = CalculateRoundDistance(otherMaterial.HFCSULL, otherMaterial.DataULL);
                        toCreateHFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreateHFCS);
                        device.CEF_Correction = otherMaterial.CEF_Correction;
                        device.data_UUL = otherMaterial.DataUUL;
                        device.data_ULL = otherMaterial.DataULL;
                    }
                    if (otherMaterial.PFCSCEF != 0)
                    {
                        var toCreatePFCS = new GHG();
                        toCreatePFCS.Id = Guid.NewGuid();
                        toCreatePFCS.Name = "PFCS";
                        toCreatePFCS.DeviceId = device.Id;
                        toCreatePFCS.CEF = otherMaterial.PFCSCEF;
                        toCreatePFCS.CEF_UUL = otherMaterial.PFCSUUL;
                        toCreatePFCS.CEF_ULL = otherMaterial.PFCSULL;
                        // 原本這裡完全沒有過濾AR版本，會拿到資料庫先回傳的任一版本GWP，和其他氣體的查法不一致。
                        decimal pfcsGWP = 0;
                        for (int i = ARVersion; i > 0; i--) // 找PFCS的GWP，如果沒有找到該版本的GWP則降版本
                        {
                            pfcsGWP = GWP.Where(x => x.Name == material && x.ARVersion == i).Select(x => x.Num).FirstOrDefault();
                            if (pfcsGWP != 0)
                            {
                                break;
                            }

                        }
                        if (pfcsGWP == 0) //連舊版本都沒有的話，維持原本「不分版本取一筆」的行為，避免比原本更容易變成0
                        {
                            pfcsGWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
                        }
                        toCreatePFCS.GWP = pfcsGWP;
                        toCreatePFCS.all_UUL = CalculateRoundDistance(otherMaterial.PFCSUUL, otherMaterial.DataUUL);
                        toCreatePFCS.all_ULL = CalculateRoundDistance(otherMaterial.PFCSULL, otherMaterial.DataULL);
                        toCreatePFCS.CreateTime = DateTime.Now;
                        _context.Add(toCreatePFCS);
                        device.CEF_Correction = otherMaterial.CEF_Correction;
                        device.data_UUL = otherMaterial.DataUUL;
                        device.data_ULL = otherMaterial.DataULL;

                    }

                }
                await _context.SaveChangesAsync();

            }

            return null;
        }
        [NonAction]
        public Device CopyDevice(Device device, Guid areaId, Guid NewDeivceId)
        {
            var newDevice = new Device
            {
                Id = NewDeivceId,
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
            return newDevice;
        }
        [NonAction]
        public GHG CopyGHG(GHG ghg, Guid newDeviceId)
        {
            var newGHG = new GHG
            {
                Id = Guid.NewGuid(), // 原本 new Guid() 等於 Guid.Empty(全0)，複製有兩種以上氣體的排放源或複製年度時會違反主鍵而整批存檔失敗。
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
            return newGHG;
        }
        [NonAction]
        public ActivityData CopyActivityData(ActivityData activityData, Guid newDeviceId)
        {
            var newActivityData = new ActivityData
            {
                DeviceId = newDeviceId,
                Num = activityData.Num,
                Time = activityData.Time,
                remark = activityData.remark,
            };
            return newActivityData;
        }
        [NonAction]
        public async Task CountEmissionData(Guid? id)
        {
            var activityDatas = await _context.ActivityDatas.AsNoTracking().Where(x => x.DeviceId == id).ToListAsync();
            decimal Num = activityDatas.Sum(x => x.Num);


            var Device = await _context.Devices.Where(x => x.Id == id).Include(x => x.Area).FirstOrDefaultAsync();
            if (Device == null) // 原本下一行就直接用 Device.AreaId，設備不存在時會丟 NullReferenceException(下面才檢查null已經來不及)。
            {
                return;
            }
            var GHG = await _context.GHGs.Where(x => x.DeviceId == id)
                                    .OrderBy(x => x.CreateTime).ThenBy(x => x.Name).ToListAsync(); //抓出需要算排放量的排放源中的溫室氣體(固定排序，前三種氣體才會配對不確定性)
            var emission = await _context.Areas.FindAsync(Device.AreaId);
            decimal all_Emission = 0,
                GHG1 = 0, GHG2 = 0, GHG3 = 0,
                GHG1ULL = 0, GHG1UUL = 0,
                GHG2ULL = 0, GHG2UUL = 0,
                GHG3ULL = 0, GHG3UUL = 0;
            if (GHG != null && Device != null)
            {

                int i = 1;
                foreach (var item in GHG)
                {
                    if (Device.Material == "廢水處理") // 原本用 item.Device.Material，靠變更追蹤補導覽屬性，沒載到時會丟 NullReferenceException。
                    {
                        item.Emission = item.CEF * Num * item.GWP;
                    }
                    else
                    {
                        item.Emission = item.CEF * Num / 1000 * item.GWP;
                    }

                    if (item.GWP == 0 || item.CEF == 0) // 少了GWP或排放係數，排放量會靜靜算成0，至少留下警告紀錄。
                    {
                        WarnMissingFactor($"排放源 {Device.Name} 的 {item.Name} GWP={item.GWP}、排放係數={item.CEF}，排放量會被算成0。");
                    }

                    item.ModifiedTime = DateTime.Now;
                    all_Emission += item.Emission; // 原本只有 i==1~3 才累加，第4~7種氣體(HFCS/PFCS/SF6/NF3)的排放量算出來卻沒進設備總量。
                    if (i == 1)
                    {
                        GHG1 += item.Emission;
                        GHG1ULL += item.all_ULL * 100;
                        GHG1UUL += item.all_UUL * 100;
                    }
                    if (i == 2)
                    {
                        GHG2 += item.Emission;
                        GHG2ULL += item.all_ULL * 100;
                        GHG2UUL += item.all_UUL * 100;
                    }
                    if (i == 3)
                    {
                        GHG3 += item.Emission;
                        GHG3ULL += item.all_ULL * 100;
                        GHG3UUL += item.all_UUL * 100;
                    }
                    i++;
                }
                decimal Device_allUUL = Calculate95U(GHG1, GHG2, GHG3, GHG1UUL, GHG2UUL, GHG3UUL);
                decimal Device_allULL = Calculate95U(GHG1, GHG2, GHG3, GHG1ULL, GHG2ULL, GHG3ULL);
                Device.Emissions = all_Emission;
                if (emission != null)
                {
                    // 原本是 emission.All += all_Emission;，每次重算都把這台設備的排放量整份再加一次，
                    // 會導致廠區總量(Area.All)每存一次活動數據就往上虛增，改成重新彙總廠區內所有未刪除設備
                    // （isDeleted 現在由全域查詢過濾器處理，不需要重複寫）。
                    decimal otherEmissions = await _context.Devices
                                                    .Where(x => x.AreaId == emission.Id && x.Id != Device.Id)
                                                    .SumAsync(x => (decimal?)x.Emissions) ?? 0;
                    emission.All = otherEmissions + all_Emission;
                }
                Device.all_UUL = Device_allUUL;
                Device.all_ULL = Device_allULL;
                Device.ModifiedTime = DateTime.Now;
                Device.count_UUL = Device_allUUL * all_Emission * Device_allUUL * all_Emission;
                Device.count_ULL = Device_allULL * all_Emission * Device_allULL * all_Emission;

                await _context.SaveChangesAsync();

            }
        }
        [NonAction]
        public async Task<bool> CountEmissionAsync(Guid? id)
        {
            // 合併資料庫查詢。isDeleted 現在由全域查詢過濾器處理，不需要另外過濾。
            var areaData = await _context.Areas
    .Include(a => a.Devices)
    .ThenInclude(d => d.GHGs)
    .Where(a => a.Id == id)
    .FirstOrDefaultAsync();


            if (areaData == null) return false;
            if (areaData.Devices == null) return false;
            // 初始化所有需要的变量
            decimal sumHardlyMove = 0, sumMove = 0, sumEscape = 0, sumProcess = 0, sumElectricity = 0;
            decimal sum1_CO2 = 0, sum2_CO2 = 0, sum1_CH4 = 0, sum2_CH4 = 0, sum1_N2O = 0, sum2_N2O = 0;
            decimal sum1_HFCS = 0, sum2_HFCS = 0, sum1_PFCS = 0, sum2_PFCS = 0, sum1_SF6 = 0, sum2_SF6 = 0, sum1_NF3 = 0, sum2_NF3 = 0;
            decimal sumScope1 = 0, sumScope2 = 0, sumAll = 0;
            int no1Grade = 0, no2Grade = 0, no3Grade = 0;
            float avgGrade = 0;
            decimal all_UUL = 0, all_ULL = 0, allCountUUL = 0, allCountULL = 0, sumUncertainty = 0;

            // 計算排放量及
            foreach (var device in areaData.Devices)
            {
                sumAll += device.Emissions;
                if (device.GHGs != null)
                {
                    foreach (var GHG in device.GHGs)
                    {
                        if (device.Scope == "類別一")
                        {
                            switch (GHG.Name)
                            {
                                case "CO2": sum1_CO2 += GHG.Emission; break;
                                case "CH4": sum1_CH4 += GHG.Emission; break;
                                case "N2O": sum1_N2O += GHG.Emission; break;
                                case "HFCS": sum1_HFCS += GHG.Emission; break;
                                case "PFCS": sum1_PFCS += GHG.Emission; break;
                                case "SF6": sum1_SF6 += GHG.Emission; break;
                                case "NF3": sum1_NF3 += GHG.Emission; break;
                            }
                        }
                        else if (device.Scope == "類別二")
                        {
                            switch (GHG.Name)
                            {
                                case "CO2": sum2_CO2 += GHG.Emission; break;
                                case "CH4": sum2_CH4 += GHG.Emission; break;
                                case "N2O": sum2_N2O += GHG.Emission; break;
                                case "HFCS": sum2_HFCS += GHG.Emission; break;
                                case "PFCS": sum2_PFCS += GHG.Emission; break;
                                case "SF6": sum2_SF6 += GHG.Emission; break;
                                case "NF3": sum2_NF3 += GHG.Emission; break;
                            }
                        }
                    }
                }

                switch (device.EmissionPattern)
                {
                    case "固定": sumHardlyMove += device.Emissions; break;
                    case "移動": sumMove += device.Emissions; break;
                    case "逸散": sumEscape += device.Emissions; break;
                    case "製程": sumProcess += device.Emissions; break;
                    case "外購電力": sumElectricity += device.Emissions; break;
                }

                // 原本第一級多了 device.Grade > 0 的條件，等級為0(修正係數沒填)的設備會被算進第二級，與下方 all_Grade 的分級方式不一致。
                // 分級門檻現在跟 EmissionMath.GradeLabel 共用同一組定義，避免兩處各自維護一份數字。
                switch (EmissionMath.GradeBucket(device.Grade))
                {
                    case 1: no1Grade++; break;
                    case 2: no2Grade++; break;
                    default: no3Grade++; break;
                }

                allCountULL += device.count_ULL;
                allCountUUL += device.count_UUL;

                if (device.all_ULL != 0)
                {
                    sumUncertainty += device.Emissions;
                }
            }

            decimal CO2 = sum1_CO2 + sum2_CO2;
            decimal CH4 = sum1_CH4 + sum2_CH4;
            decimal N2O = sum1_N2O + sum2_N2O;
            decimal HFCS = sum1_HFCS + sum2_HFCS;
            decimal PFCS = sum1_PFCS + sum2_PFCS;
            decimal SF6 = sum1_SF6 + sum2_SF6;
            decimal NF3 = sum1_NF3 + sum2_NF3; // 原本寫 sum1_NF3 + sum2_SF6，會導致廠區NF3總量混入類別二的SF6，且類別二的NF3被漏掉。

            sumScope1 = sum1_CO2 + sum1_CH4 + sum1_N2O + sum1_HFCS + sum1_PFCS + sum1_NF3 + sum1_SF6;
            sumScope2 = sum2_CO2 + sum2_CH4 + sum2_N2O + sum2_HFCS + sum2_PFCS + sum2_NF3 + sum2_SF6;

            // 原本沒有防呆就直接除以 sumAll，廠區內設備排放量全為0(剛建立、還沒填活動數據)時
            // decimal 的 0/0 會丟出 DivideByZeroException，會導致排放量頁面、圖表與所有報告書下載全部500。
            if (sumAll != 0)
            {
                foreach (var device in areaData.Devices)
                {
                    avgGrade += (float)Math.Round((float)(Math.Round(device.Emissions / sumAll, 4) * device.Grade), 2);
                }
            }

            if (sumUncertainty != 0)
            {
                all_UUL = DecimalSqrt(allCountUUL) / sumUncertainty;
                all_ULL = DecimalSqrt(allCountULL) / sumUncertainty;
            }

            // 原本只有「總量」或「ULL」有變才會呼叫 UpdateEmissionData(而且變數名叫 sumMatch 其實存的是不相等)，
            // 設備改類別、改排放型式或一增一減時總量不變，會導致各氣體/範疇/型式/等級等四十幾個分項欄位留著舊資料。
            // 改成每次都重算，再交給 EF 的變更追蹤決定要不要真的寫回資料庫。
            UpdateEmissionData(areaData, sum1_CO2, sum1_CH4, sum1_N2O, sum1_HFCS, sum1_PFCS, sum1_SF6, sum1_NF3,
                sum2_CO2, sum2_CH4, sum2_N2O, sum2_HFCS, sum2_PFCS, sum2_SF6, sum2_NF3,
                CO2, CH4, N2O, HFCS, PFCS, SF6, NF3,
                sumScope1, sumScope2, sumAll,
                sumHardlyMove, sumMove, sumEscape, sumProcess,
                all_UUL, all_ULL, sumUncertainty,
                no1Grade, no2Grade, no3Grade, avgGrade);

            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }

            return true;
        }
        private void UpdateEmissionData(Area areaData,
            decimal sum1_CO2, decimal sum1_CH4, decimal sum1_N2O, decimal sum1_HFCS, decimal sum1_PFCS, decimal sum1_SF6, decimal sum1_NF3,
            decimal sum2_CO2, decimal sum2_CH4, decimal sum2_N2O, decimal sum2_HFCS, decimal sum2_PFCS, decimal sum2_SF6, decimal sum2_NF3,
            decimal CO2, decimal CH4, decimal N2O, decimal HFCS, decimal PFCS, decimal SF6, decimal NF3,
            decimal sumScope1, decimal sumScope2, decimal sumAll,
            decimal sumHardlyMove, decimal sumMove, decimal sumEscape, decimal sumProcess,
            decimal all_UUL, decimal all_ULL, decimal sumUncertainty,
            int no1Grade, int no2Grade, int no3Grade, float avgGrade)
        {
            areaData.Scope1_CO2 = sum1_CO2;
            areaData.Scope1_CH4 = sum1_CH4;
            areaData.Scope1_N2O = sum1_N2O;
            areaData.Scope1_HFCS = sum1_HFCS;
            areaData.Scope1_PFCS = sum1_PFCS;
            areaData.Scope1_SF6 = sum1_SF6;
            areaData.Scope1_NF3 = sum1_NF3;

            areaData.Scope2_CO2 = sum2_CO2;
            areaData.Scope2_CH4 = sum2_CH4;
            areaData.Scope2_N2O = sum2_N2O;
            areaData.Scope2_HFCS = sum2_HFCS;
            areaData.Scope2_PFCS = sum2_PFCS;
            areaData.Scope2_SF6 = sum2_SF6;
            areaData.Scope2_NF3 = sum2_NF3;

            areaData.CO2 = CO2;
            areaData.CH4 = CH4;
            areaData.N2O = N2O;
            areaData.HFCS = HFCS;
            areaData.PFCS = PFCS;
            areaData.SF6 = SF6;
            areaData.NF3 = NF3;

            areaData.Scope1 = sumScope1;
            areaData.Scope2 = sumScope2;
            areaData.All = sumAll;

            areaData.non_move = sumHardlyMove;
            areaData.move = sumMove;
            areaData.escape = sumEscape;
            areaData.process = sumProcess;

            if (sumScope1 != 0)
            {
                areaData.percentage1_CO2 = (sum1_CO2 / sumScope1 * 100);
                areaData.percentage1_CH4 = (sum1_CH4 / sumScope1 * 100);
                areaData.percentage1_N2O = (sum1_N2O / sumScope1 * 100);
                areaData.percentage1_HFCS = (sum1_HFCS / sumScope1 * 100);
                areaData.percentage1_PFCS = (sum1_PFCS / sumScope1 * 100);
                areaData.percentage1_SF6 = (sum1_SF6 / sumScope1 * 100);
                areaData.percentage1_NF3 = (sum1_NF3 / sumScope1 * 100);
            }
            else
            {
                areaData.percentage1_CO2 = 0;
                areaData.percentage1_CH4 = 0;
                areaData.percentage1_N2O = 0;
                areaData.percentage1_HFCS = 0;
                areaData.percentage1_PFCS = 0;
                areaData.percentage1_SF6 = 0;
                areaData.percentage1_NF3 = 0;
            }

            if (sumAll != 0)
            {
                areaData.percentage2_CO2 = (CO2 / sumAll * 100);
                areaData.percentage2_CH4 = (CH4 / sumAll * 100);
                areaData.percentage2_N2O = (N2O / sumAll * 100);
                areaData.percentage2_HFCS = (HFCS / sumAll * 100);
                areaData.percentage2_PFCS = (PFCS / sumAll * 100);
                areaData.percentage2_SF6 = (SF6 / sumAll * 100);
                areaData.percentage2_NF3 = (NF3 / sumAll * 100);

                areaData.percentage_nonMove = (sumHardlyMove / sumAll * 100);
                areaData.percentage_Move = (sumMove / sumAll * 100);
                areaData.percentage_Escape = (sumEscape / sumAll * 100);
                areaData.percentage_Process = (sumProcess / sumAll * 100);
                areaData.percentage_Scope1 = (sumScope1 / sumAll * 100);
                areaData.percentage_Scope2 = (sumScope2 / sumAll * 100);
            }
            else
            {
                areaData.percentage2_CO2 = 0;
                areaData.percentage2_CH4 = 0;
                areaData.percentage2_N2O = 0;
                areaData.percentage2_HFCS = 0;
                areaData.percentage2_PFCS = 0;
                areaData.percentage2_SF6 = 0;
                areaData.percentage2_NF3 = 0;

                areaData.percentage_nonMove = 0;
                areaData.percentage_Move = 0;
                areaData.percentage_Escape = 0;
                areaData.percentage_Process = 0;
                areaData.percentage_Scope1 = 0;
                areaData.percentage_Scope2 = 0;
            }

            areaData.cal_all = sumUncertainty;
            areaData.no1_Grade = no1Grade;
            areaData.no2_Grade = no2Grade;
            areaData.no3_Grade = no3Grade;
            areaData.avg_Grade = avgGrade;
            areaData.all_Grade = EmissionMath.GradeLabel(avgGrade);
            // 原本這行的除法寫在 if (sumAll != 0) 之外，sumAll 為0時會丟出 DivideByZeroException。
            areaData.percentage_CalAll = sumAll != 0 ? (sumUncertainty / sumAll * 100) : 0;
            areaData.ULL = all_ULL;
            areaData.UUL = all_UUL;
        }
    }
}
