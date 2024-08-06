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
        public async Task<bool> CEFAddAsync(Device device, string GHG, decimal? CEF)
        {
            int ARVersion = await _context.Areas.Where(x => x.Id == device.AreaId).Select(x => x.ARVersion).FirstOrDefaultAsync();
            var GWP = await _context.GWPs.Where(x => x.ARCount == ARVersion).ToListAsync();
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

            return true;
        }
        public async Task<GHG?> GHGCheckAsync(Guid id, string name, string material, string scope, string emisspatern, int year) //排放源Id, 排放源名稱, 物料名稱, 類別, 排放型式
        {
            var Device = await _context.Devices.Include(x => x.Area).Where(x => x.Id == id).FirstOrDefaultAsync();
            var GWP = await _context.GWPs.Where(x => x.ARCount == Device.Area.ARVersion).ToListAsync();
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

    }
}
