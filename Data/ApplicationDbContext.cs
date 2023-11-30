using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Reflection.Emit;

namespace Carbon_inventory_platform.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<GWP> GWPs { get; set; }
        public DbSet<Refrigerant> refrigerants { get; set; } = null!;
        public DbSet<DataLevel> dataLevels { get; set; } = null!;
        public DbSet<Emission> emissions { get; set; } = null!;
        public DbSet<DataCorrection> dataCorrections { get; set; } = null!;
        public DbSet<DeviceData> deviceDatas { get; set; } = null!;

        //public virtual DbSet<User> Users { get; set; }
        //public virtual DbSet<Staff> Staffs { get; set; }
        //public virtual DbSet<Groups> Groups { get; set; }
        //public virtual DbSet<Funtion> Funtions { get; set; }
        //public virtual DbSet<Authorization> Authorization { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Company>(entity =>
                {
                    entity.HasMany(e => e.Areas);
                });

            builder.Entity<Area>(entity =>
            {
                entity.HasOne(e => e.Company);
            });

            builder.Entity<Device>(entity =>
            {
                //entity.HasMany(e => e.Areas);// 廠區名稱bug修改成以下
                entity.HasOne(e => e.Areas);
                //entity.HasOne(e => e.Material);
            });
            builder.Entity<Emission>(entity =>
            {
                entity.HasOne(e => e.Areas);
            });

            builder.Entity<Material>(entity =>
                {
                    entity.HasMany(e => e.Devices);
                });

            DataSeed(builder);
        }
        private void DataSeed(ModelBuilder builder)
        {
            builder.Entity<Material>().HasData(
                new Material { Id = 1, Name = "自產煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.3328598392, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.000024660252, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000036990378, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 2, Name = "原料煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.693284704, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.00002847024, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00004270536, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 3, Name = "燃料煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.4081133824, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.000025455744, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000038183616, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 4, Name = "無煙煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.922093324, CO2ULL = 0.03764F, CO2UUL = 0.027467F, CH4CEF = 0.00002972628, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00004458942, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 5, Name = "焦煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.693284704, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.00002847024, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00004270536, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 6, Name = "煙煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.4081133824, CO2ULL = 0.053911F, CO2UUL = 0.053911F, CH4CEF = 0.000025455744, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000038183616, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 7, Name = "亞煙煤(發電)", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 1.971522252, CO2ULL = 0.034339F, CO2UUL = 0.040583F, CH4CEF = 0.00002051532, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00003077298, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 8, Name = "亞煙煤(其他)", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.253168288, CO2ULL = 0.034339F, CO2UUL = 0.040583F, CH4CEF = 0.00002344608, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00003516912, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 9, Name = "褐煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 1.2026331792, CO2ULL = 0.1F, CO2UUL = 0.138614F, CH4CEF = 0.0000119072592, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.0000178608888, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 10, Name = "油頁岩", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 0.9528696252, CO2ULL = 0.157009F, CO2UUL = 0.168224F, CH4CEF = 0.0000089053236, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.0000133579854, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 11, Name = "泥煤", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 1.0353872664, CO2ULL = 0.056604F, CO2UUL = 0.018868F, CH4CEF = 0.0000097678044, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.0000146517066, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 12, Name = "煤球", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 1.5512094, CO2ULL = 0.104615F, CO2UUL = 0.117949F, CH4CEF = 0.00001590984, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00002386476, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 13, Name = "焦炭", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 3.1359132, CO2ULL = 0.105607F, CO2UUL = 0.11215F, CH4CEF = 0.0000293076, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.0000439614, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 14, Name = "石油焦", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 3.3473466, CO2ULL = 0.149744F, CO2UUL = 0.179487F, CH4CEF = 0.00010299528, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000020599056, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 15, Name = "航空汽油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.19807, CO2ULL = 0.035714F, CO2UUL = 0.042857F, CH4CEF = 0.000094203, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.0000188406, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 16, Name = "航空燃油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.3948496, CO2ULL = 0.025175F, CO2UUL = 0.040559F, CH4CEF = 0.0001004832, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002009664, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 17, Name = "原油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.76203196, CO2ULL = 0.030014F, CO2UUL = 0.030014F, CH4CEF = 0.0001130436, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002260872, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 18, Name = "奧里油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.1190274028, CO2ULL = 0.1F, CO2UUL = 0.109091F, CH4CEF = 0.0000825595092, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00001651190184, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 19, Name = "天然氣凝結油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.83952460384, CO2ULL = 0.0919F, CO2UUL = 0.096573F, CH4CEF = 0.0001326880656, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002653761312, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "M3" },
new Material { Id = 20, Name = "煤油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.55876282, CO2ULL = 0.015299F, CO2UUL = 0.025035F, CH4CEF = 0.0001067634, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002135268, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 21, Name = "頁岩油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.79456255864, CO2ULL = 0.075034F, CO2UUL = 0.080491F, CH4CEF = 0.0001079943192, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002159886384, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 22, Name = "柴油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.606031792, CO2ULL = 0.020243F, CO2UUL = 0.009447F, CH4CEF = 0.00010550736, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000021101472, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 23, Name = "車用汽油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.263132872, CO2ULL = 0.025974F, CO2UUL = 0.053391F, CH4CEF = 0.00009797112, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000019594224, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 24, Name = "蒸餘油 (燃料油)", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 3.110959872, CO2ULL = 0.024548F, CO2UUL = 0.018088F, CH4CEF = 0.00012057984, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000024115968, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 25, Name = "液化石油氣", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 1.7528812758, CO2ULL = 0.023772F, CO2UUL = 0.03962F, CH4CEF = 0.000027779418, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.0000027779418, N2OULL = 0.7F, N2OUUL = 2F, Unit = "L" },
new Material { Id = 26, Name = "石油腦", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.393761032, CO2ULL = 0.05457F, CO2UUL = 0.040928F, CH4CEF = 0.00009797112, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000019594224, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 27, Name = "柏油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 3.3787476, CO2ULL = 0.095415F, CO2UUL = 0.114002F, CH4CEF = 0.000125604, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.0000251208, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 28, Name = "潤滑油", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.946167424, CO2ULL = 0.0191F, CO2UUL = 0.025921F, CH4CEF = 0.00012057984, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000024115968, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 29, Name = "其他油品", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.76203196, CO2ULL = 0.015007F, CO2UUL = 0.015007F, CH4CEF = 0.0001130436, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002260872, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 30, Name = "乙烷", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.8601872992, CO2ULL = 0.082792F, CO2UUL = 0.113636F, CH4CEF = 0.000046431612, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.0000046431612, N2OULL = 0.7F, N2OUUL = 2F, Unit = "L" },
new Material { Id = 31, Name = "天然氣", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 1.87903584, CO2ULL = 0.032086F, CO2UUL = 0.039216F, CH4CEF = 0.0000334944, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00000334944, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 32, Name = "煉油氣", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 2.17043712, CO2ULL = 0.163194F, CO2UUL = 0.197917F, CH4CEF = 0.0000376812, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00000376812, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 33, Name = "焦爐氣", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 0.780754464, CO2ULL = 0.15991F, CO2UUL = 0.218468F, CH4CEF = 0.00001758456, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000001758456, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 34, Name = "高爐氣", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 0.845817336, CO2ULL = 0.157692F, CO2UUL = 0.184615F, CH4CEF = 0.0000032531436, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.00000032531436, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 35, Name = "一般廢棄物", Scope = "類別1", EmissionPattern = "固定", CO2CEF = 0.7792272742716, CO2ULL = 0.200654F, CO2UUL = 0.31952F, CH4CEF = 0.00025492713444, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000033990284592, N2OULL = 0.625F, N2OUUL = 2.75F, Unit = "Kg" },
new Material { Id = 36, Name = "航空汽油", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 2.19807, CO2ULL = 0.035714F, CO2UUL = 0.042857F, CH4CEF = 0.000094203, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.0000188406, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 37, Name = "航空燃油", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 2.3948496, CO2ULL = 0.025175F, CO2UUL = 0.040559F, CH4CEF = 0.0001004832, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002009664, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 38, Name = "車用汽油", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 2.263132872, CO2ULL = 0.025974F, CO2UUL = 0.053391F, CH4CEF = 0.000816426, CH4ULL = 0.666667F, CH4UUL = 2.44F, N2OCEF = 0.00026125632, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 39, Name = "柴油", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 2.606031792, CO2ULL = 0.020243F, CO2UUL = 0.009447F, CH4CEF = 0.000137159568, CH4ULL = 0.589744F, CH4UUL = 1.435897F, N2OCEF = 0.000137159568, N2OULL = 0.666667F, N2OUUL = 2.076923F, Unit = "L" },
new Material { Id = 40, Name = "煤油", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 2.55876282, CO2ULL = 0.015299F, CO2UUL = 0.025035F, CH4CEF = 0.0001067634, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002135268, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 41, Name = "潤滑油", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 2.946167424, CO2ULL = 0.0191F, CO2UUL = 0.025921F, CH4CEF = 0.00012057984, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000024115968, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 42, Name = "液化石油氣", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 1.7528812758, CO2ULL = 0.023772F, CO2UUL = 0.03962F, CH4CEF = 0.001722323916, CH4ULL = 0F, CH4UUL = 0F, N2OCEF = 0.0000055558836, N2OULL = 0F, N2OUUL = 0F, Unit = "L" },
new Material { Id = 43, Name = "液化天然氣", Scope = "類別1", EmissionPattern = "移動", CO2CEF = 2.11391532, CO2ULL = 0.032086F, CO2UUL = 0.039216F, CH4CEF = 0.0034666704, CH4ULL = 0.456522F, CH4UUL = 15.73913F, N2OCEF = 0.0001130436, N2OULL = 0.666667F, N2OUUL = 24.666667F, Unit = "M3" },
new Material { Id = 56, Name = "外購電力", Scope = "類別2", EmissionPattern = "外購電力", CO2CEF = 0.495, CO2ULL = -0.07F, CO2UUL = 0.07F, Unit = "", Year = 111 },
new Material { Id = 57, Name = "廢水處理", Scope = "類別1", EmissionPattern = "逸散", CH4CEF = 0.002546062 },
new Material { Id = 58, Name = "二氧化碳", Scope = "類別1", EmissionPattern = "逸散", CO2CEF = 1 },
new Material { Id = 59, Name = "乙炔", Scope = "類別1", EmissionPattern = "製程", CO2CEF = 3.3841653850 },
new Material { Id = 60, Name = "焊條", Scope = "類別1", EmissionPattern = "製程", CO2CEF = 3.6666666666 }
);
            builder.Entity<GWP>().HasData(
new GWP { Name = "CO2", Num = 1, GWP_Year = 2022 },
new GWP { Name = "CH4", Num = 27.9F, GWP_Year = 2022 },
new GWP { Name = "N2O", Num = 273, GWP_Year = 2022 },
new GWP { Name = "R-23", Num = 14600, GWP_Year = 2022 },
new GWP { Name = "R-32", Num = 771, GWP_Year = 2022 },
new GWP { Name = "R-134A", Num = 1530, GWP_Year = 2022 },
new GWP { Name = "七氟丙烷", Num = 3600, GWP_Year = 2022 },
new GWP { Name = "R-22", Num = 1960, GWP_Year = 2022 },
new GWP { Name = "R-410A", Num = 2256, GWP_Year = 2022 },
new GWP { Name = "R-600A", Num = 0.006F, GWP_Year = 2022 },
new GWP { Name = "R-417A", Num = 2127, GWP_Year = 2022 },
new GWP { Name = "R-407C", Num = 1908, GWP_Year = 2022 },
new GWP { Name = "R-507A", Num = 4475, GWP_Year = 2022 },
new GWP { Name = "NF3", Num = 17400, GWP_Year = 2022 },
new GWP { Name = "SF6", Num = 24300, GWP_Year = 2022 }
                );
            builder.Entity<DeviceData>().HasData(
                new DeviceData { Id = 1, Name = "冷氣機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-410A" , Correction=3,Level=3 ,unit="公斤"},
        new DeviceData { Id = 2, Name = "冰水主機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
        new DeviceData { Id = 3, Name = "冰箱", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
        new DeviceData { Id = 4, Name = "飲水機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
        new DeviceData { Id = 5, Name = "乾燥機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3 , unit = "公斤" },
        new DeviceData { Id = 6, Name = "車用空調", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3 , unit = "公斤" },
        new DeviceData { Id = 7, Name = "工業冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3 , unit = "公斤" },
        new DeviceData { Id = 8, Name = "緊急發電機", Scope = "類別一", EmissionPattern = "固定", Material = "柴油", Correction = 3, Level = 3, unit = "公升" },
                new DeviceData { Id = 9, Name = "廚房", Scope = "類別一", EmissionPattern = "固定", Material = "液化石油氣", Correction = 2, Level = 2, unit = "公斤" },
                new DeviceData { Id = 10, Name = "公務車", Scope = "類別一", EmissionPattern = "移動", Material = "車用汽油", Correction = 2, Level = 2, unit = "公升" },
                new DeviceData { Id = 11, Name = "堆高機", Scope = "類別一", EmissionPattern = "移動", Material = "柴油", Correction = 2, Level = 2, unit = "公升" },
                new DeviceData { Id = 12, Name = "CO2滅火器", Scope = "類別一", EmissionPattern = "逸散", Material = "二氧化碳", Correction = 3, Level = 3 , unit = "公斤" },
                new DeviceData { Id = 13, Name = "二氧化碳", Scope = "類別一", EmissionPattern = "逸散", Material = "二氧化碳", Correction = 3, Level = 3 , unit = "公斤" },
                new DeviceData { Id = 14, Name = "WD40", Scope = "類別一", EmissionPattern = "逸散", Material = "二氧化碳", Correction = 3, Level = 3 , unit = "公斤" },
                new DeviceData { Id = 15, Name = "海龍滅火器", Scope = "類別一", EmissionPattern = "逸散", Material = "海龍1211", Correction = 3, Level = 3 , unit = "公斤" },
                new DeviceData { Id = 16, Name = "FM200", Scope = "類別一", EmissionPattern = "逸散", Material = "FM200", Correction = 3, Level = 3 , unit = "公斤" },
                new DeviceData { Id = 17, Name = "化糞池", Scope = "類別一", EmissionPattern = "逸散", Material = "廢水處理", Correction = 3, Level = 3 , unit = "人" },
                new DeviceData { Id = 18, Name = "電力", Scope = "類別二", EmissionPattern = "外購電力", Material = "外購電力", Correction = 1, Level = 1, unit = "度" },
                new DeviceData { Id = 19, Name = "乙炔", Scope = "類別一", EmissionPattern = "製程", Material = "乙炔", Correction = 3, Level = 3, unit = "公斤" },
                new DeviceData { Id = 20, Name = "焊條", Scope = "類別一", EmissionPattern = "製程", Material = "焊條", Correction = 3, Level = 3, unit = "公斤" },
                 new DeviceData { Id = 21, Name = "工業冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
                  new DeviceData { Id = 22, Name = "商用冰箱", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
                   new DeviceData { Id = 23, Name = "中、大型冰箱", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
                    new DeviceData { Id = 24, Name = "低溫冷凍車", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
                     new DeviceData { Id = 25, Name = "食品加工冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Correction = 3, Level = 3, unit = "公斤" },
                     new DeviceData { Id = 26, Name = "瓦斯罐", Scope = "類別一", EmissionPattern = "製程", Material = "丁烷", Correction = 3, Level = 3, unit = "公斤" }

    );

            builder.Entity<Refrigerant>().HasData(
new Refrigerant { Name = "家用冷凍、冷藏裝備", Num = 0.003000F },
new Refrigerant { Name = "獨立商用冷凍、冷藏裝備", Num = 0.055000F },
new Refrigerant { Name = "中、大型冷凍、冷藏裝備", Num = 0.200000F },
new Refrigerant { Name = "交通用冷凍、冷藏裝備", Num = 0.33F },
new Refrigerant { Name = "工業冷凍、冷藏裝備，包括食品加工及冷藏", Num = 0.16F },
new Refrigerant { Name = "冰水機", Num = 0.09F },
new Refrigerant { Name = "住宅及商業建築冷氣機", Num = 0.03F },
new Refrigerant { Name = "移動式空氣清靜機", Num = 0.2F }
                );
            builder.Entity<DataLevel>().HasData(
new DataLevel { id = 1, name = "連續監測" },
new DataLevel { id = 2, name = "定期/間歇量測" },
new DataLevel { id = 3, name = "自行/財務推估" }
                );
            builder.Entity<DataCorrection>().HasData(
new DataCorrection { id = 1, name = "有外部校正或多組數據佐證者" },
new DataCorrection { id = 2, name = "有內部校正或經過會計簽證等證明者" },
new DataCorrection { id = 3, name = "未進行儀器校正或未進行紀錄彙整者" }
                );
        }
    }
}