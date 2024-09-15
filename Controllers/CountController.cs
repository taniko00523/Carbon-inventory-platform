using Carbon_inventory_platform.Data;
using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Carbon_inventory_platform.Controllers
{
    public class CountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public static decimal DecimalSqrt(decimal value) //用牛頓法逼近Decimal的平方根
        {
            int iterations = 20;
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
        public decimal CalculateRoundDistance(decimal num1, decimal num2) // 計算兩數平方和的平方根，並四捨五入到小數點後5位
        {
            if (num1 != 0 && num2 != 0)
            {
                decimal distance = DecimalSqrt(num1 * num1 + num2 * num2);
                return Math.Round(distance, 5);
            }
            return 0;
        }
        public decimal Calculate95U(decimal GHG1, decimal GHG2, decimal GHG3, decimal GHG1UUL, decimal GHG2UUL, decimal GHG3UUL) //計算95%信賴區間 
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
        public async Task<bool> CEFAddAsync(Device device, string GHG, decimal? CEF) //自訂排碳係數
        {
            int ARVersion = await _context.Areas.Where(x => x.Id == device.AreaId).Select(x => x.ARVersion).FirstOrDefaultAsync();
            var GWP = await _context.GWPs.ToListAsync();
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
                        hfcsGWP = GWP.Where(x => x.Name == device.Material && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
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
                            ch4GWP = toCreate.GWP = GWP.Where(x => x.Name == "CH4" && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault(); //石化甲烷
                            break;
                    }
                    toCreate.GWP = ch4GWP;
                }
                else //其他GHG
                {
                    toCreate.GWP = GWP.Where(x => x.Name == GHG && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
                }
                toCreate.CreateTime = DateTime.Now;
                device.CEF_Correction = 1;//輸入?
                _context.Add(toCreate);
                await _context.SaveChangesAsync();

            }

            return true;
        }
        public async Task<GHG?> GHGCheckAsync(Device device, string deviceName, string material, string scope, string emisspatern, int year, int ARVersion) //設定資料庫排碳係數及GWP
        {
            var GWP = await _context.GWPs.ToListAsync();
            var MaterialList = await _context.Materials
                                    .Where(x => x.Name == material && x.Scope == scope && x.EmissionPattern == emisspatern && x.Year <= year).ToListAsync();
            var Material = MaterialList.OrderByDescending(x => x.Year).FirstOrDefault();

            var otherMaterial = await _context.Materials.Where(x => x.Name == deviceName && x.Scope == scope && x.EmissionPattern == emisspatern).FirstOrDefaultAsync(); //目前只有冷媒設備，但我包含了PFCS以防萬一
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
                        toCreateCO2.GWP = GWP.Where(x => x.Name == "CO2" && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
                        toCreateCO2.all_UUL = CalculateRoundDistance(Material.CO2UUL, Material.DataUUL);
                        toCreateCO2.all_ULL = CalculateRoundDistance(Material.CO2ULL, Material.DataULL);
                        toCreateCO2.CreateTime = DateTime.Now;
                        _context.AddRange(toCreateCO2);
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
                                    ch4GWP = toCreateCH4.GWP = GWP.Where(x => x.Name == "CH4" && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
                                    break;
                            }
                            toCreateCH4.GWP = ch4GWP;
                        }
                        else // 石化甲烷
                        {
                            toCreateCH4.GWP = GWP.Where(x => x.Name == "CH4" && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
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
                        toCreateN2O.GWP = GWP.Where(x => x.Name == "N2O" && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
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
                        toCreateHFCS.GWP = GWP.Where(x => x.Name == material && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
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
                        toCreatePFCS.GWP = GWP.Where(x => x.Name == material && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
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
                        toCreateSF6.GWP = GWP.Where(x => x.Name == "SF6" && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
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
                        toCreateNF3.GWP = GWP.Where(x => x.Name == "NF3" && x.ARCount == ARVersion).Select(x => x.Num).FirstOrDefault();
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
                            hfcsGWP = GWP.Where(x => x.Name == material && x.ARCount == i).Select(x => x.Num).FirstOrDefault();
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
                        toCreatePFCS.GWP = GWP.Where(x => x.Name == material).Select(x => x.Num).FirstOrDefault();
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
        public GHG CopyGHG(GHG ghg, Guid newDeviceId)
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
            return newGHG;
        }
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
        public async Task<bool> CountEmissionAsync(Guid? id)
        {
            // 合并数据库查询
            var areaData = await _context.Areas
    .Include(a => a.Devices.Where(d => d.isDeleted == 0))  // 只包含 isDeleted == 0 的 Devices
    .ThenInclude(d => d.GHGs)
    .Where(a => a.Id == id && a.isDeleted == 0)  // Area 的 Id 等于 id，且 IsDeleted == 0
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

                if (device.Grade < 10 && device.Grade > 0) no1Grade++;
                else if (device.Grade < 19) no2Grade++;
                else no3Grade++;

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
            decimal NF3 = sum1_NF3 + sum2_SF6;

            sumScope1 = sum1_CO2 + sum1_CH4 + sum1_N2O + sum1_HFCS + sum1_PFCS + sum1_NF3 + sum1_SF6;
            sumScope2 = sum2_CO2 + sum2_CH4 + sum2_N2O + sum2_HFCS + sum2_PFCS + sum2_NF3 + sum2_SF6;
            sumAll = sumScope1 + sumScope2;

            foreach (var device in areaData.Devices)
            {
                avgGrade += (float)Math.Round((float)(Math.Round(device.Emissions / sumAll, 4) * device.Grade), 2);
            }

            if (sumUncertainty != 0)
            {
                all_UUL = DecimalSqrt(allCountUUL) / sumUncertainty;
                all_ULL = DecimalSqrt(allCountULL) / sumUncertainty;
            }

            bool sumMatch = areaData.All != Math.Round(sumAll, 3);
            bool uncertaintyMatch = areaData.ULL != Math.Round(all_ULL, 2);

            if (sumMatch || uncertaintyMatch)
            {
                // 更新資料庫中的資料
                UpdateEmissionData(areaData, sum1_CO2, sum1_CH4, sum1_N2O, sum1_HFCS, sum1_PFCS, sum1_SF6, sum1_NF3,
                    sum2_CO2, sum2_CH4, sum2_N2O, sum2_HFCS, sum2_PFCS, sum2_SF6, sum2_NF3,
                    CO2, CH4, N2O, HFCS, PFCS, SF6, NF3,
                    sumScope1, sumScope2, sumAll,
                    sumHardlyMove, sumMove, sumEscape, sumProcess,
                    all_UUL, all_ULL, sumUncertainty,
                    no1Grade, no2Grade, no3Grade, avgGrade);

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
            areaData.all_Grade = avgGrade < 10 ? "第一級" : (avgGrade < 19 ? "第二級" : "第三級");
            areaData.percentage_CalAll = (sumUncertainty / sumAll * 100);
            areaData.ULL = all_ULL;
            areaData.UUL = all_UUL;
        }
    }
}
