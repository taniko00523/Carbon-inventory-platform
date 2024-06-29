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
        public DbSet<Analysis> Analyses { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<GHG> GHGs { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<ActivityData> ActivityDatas { get; set; }
        public DbSet<GWP> GWPs { get; set; }
        public DbSet<DeviceData> deviceDatas { get; set; } = null!;
        public DbSet<DefaultDevices> defaultDevices { get; set; } = null!;


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
                entity.HasOne(e => e.Analysis);
                entity.HasMany(e => e.Devices);
            });

            builder.Entity<Analysis>(entity =>
            {
                entity.HasOne(e => e.Area);
            });

            builder.Entity<Device>(entity =>
            {
                entity.HasOne(e => e.Area);
                entity.HasMany(e => e.GHGs);
            });

            builder.Entity<ActivityData>(entity =>
            {
                entity.HasOne(e => e.Device);
            });

            builder.Entity<GHG>(entity =>
            {
                entity.HasOne(e => e.Device);
            });

            DataSeed(builder);
        }
        private void DataSeed(ModelBuilder builder)
        {
            builder.Entity<Material>().HasData(
                    new Material { Id = 1, Name = "自產煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.3328598392M, CO2ULL = 0.077167019M, CO2UUL = 0.067653277M, CH4CEF = 0.0000246603M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000369904M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 2, Name = "原料煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.693284704M, CO2ULL = 0.077167019M, CO2UUL = 0.067653277M, CH4CEF = 0.0000284702M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000427054M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 3, Name = "燃料煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.4081133824M, CO2ULL = 0.077167019M, CO2UUL = 0.067653277M, CH4CEF = 0.0000254557M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000381836M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 4, Name = "無煙煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.922093324M, CO2ULL = 0.0376398779M, CO2UUL = 0.0274669379M, CH4CEF = 0.0000297263M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000445894M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 5, Name = "焦煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.693284704M, CO2ULL = 0.077167019M, CO2UUL = 0.067653277M, CH4CEF = 0.0000284702M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000427054M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 6, Name = "煙煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.4081133824M, CO2ULL = 0.0539112051M, CO2UUL = 0.0539112051M, CH4CEF = 0.0000254557M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000381836M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 7, Name = "亞煙煤(發電)", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.971522252M, CO2ULL = 0.03433923M, CO2UUL = 0.0405827263M, CH4CEF = 0.0000205153M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.000030773M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 8, Name = "亞煙煤(其他)", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.253168288M, CO2ULL = 0.03433923M, CO2UUL = 0.0405827263M, CH4CEF = 0.0000234461M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000351691M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 9, Name = "褐煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.2026331792M, CO2ULL = 0.1M, CO2UUL = 0.1386138614M, CH4CEF = 0.0000119073M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000178609M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 10, Name = "油頁岩", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 0.9528696252M, CO2ULL = 0.1570093458M, CO2UUL = 0.1682242991M, CH4CEF = 0.0000089053M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.000013358M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 11, Name = "泥煤", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.0353872664M, CO2ULL = 0.0566037736M, CO2UUL = 0.0188679245M, CH4CEF = 0.0000097678M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000146517M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 12, Name = "煤球", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.5512094M, CO2ULL = 0.1046153846M, CO2UUL = 0.1179487179M, CH4CEF = 0.0000159098M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000238648M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 13, Name = "焦炭", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 3.1359132M, CO2ULL = 0.1056074766M, CO2UUL = 0.1121495327M, CH4CEF = 0.0000293076M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000439614M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 14, Name = "石油焦", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 3.3473466M, CO2ULL = 0.1497435897M, CO2UUL = 0.1794871795M, CH4CEF = 0.0001029953M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000205991M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 15, Name = "航空汽油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.19807M, CO2ULL = 0.0357142857M, CO2UUL = 0.0428571429M, CH4CEF = 0.000094203M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000188406M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 16, Name = "航空燃油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.3948496M, CO2ULL = 0.0251748252M, CO2UUL = 0.0405594406M, CH4CEF = 0.0001004832M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000200966M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 17, Name = "原油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.76203196M, CO2ULL = 0.0300136426M, CO2UUL = 0.0300136426M, CH4CEF = 0.0001130436M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000226087M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 18, Name = "奧里油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.1190274028M, CO2ULL = 0.1M, CO2UUL = 0.1090909091M, CH4CEF = 0.0000825595M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000165119M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 19, Name = "天然氣凝結油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.8395246038M, CO2ULL = 0.0919003115M, CO2UUL = 0.0965732087M, CH4CEF = 0.0001326881M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000265376M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "M3" },
new Material { Id = 20, Name = "煤油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.55876282M, CO2ULL = 0.0152990264M, CO2UUL = 0.0250347705M, CH4CEF = 0.0001067634M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000213527M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 21, Name = "頁岩油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.7945625586M, CO2ULL = 0.0750341064M, CO2UUL = 0.0804911323M, CH4CEF = 0.0001079943M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000215989M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 22, Name = "柴油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.606031792M, CO2ULL = 0.020242915M, CO2UUL = 0.0094466937M, CH4CEF = 0.0001055074M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000211015M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L", DataULL = -0.01M, DataUUL = -0.01M },
new Material { Id = 23, Name = "車用汽油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.263132872M, CO2ULL = 0.025974026M, CO2UUL = 0.0533910534M, CH4CEF = 0.0000979711M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000195942M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L", DataULL = -0.01M, DataUUL = -0.01M },
new Material { Id = 24, Name = "蒸餘油 (燃料油)", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 3.110959872M, CO2ULL = 0.0245478036M, CO2UUL = 0.0180878553M, CH4CEF = 0.0001205798M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.000024116M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 25, Name = "液化石油氣", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.7528812758M, CO2ULL = 0.0237717908M, CO2UUL = 0.0396196513M, CH4CEF = 0.0000277794M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000027779M, N2OULL = 0.7M, N2OUUL = 2M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 26, Name = "石油腦", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.393761032M, CO2ULL = 0.0545702592M, CO2UUL = 0.0409276944M, CH4CEF = 0.0000979711M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000195942M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 27, Name = "柏油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 3.3787476M, CO2ULL = 0.0954151177M, CO2UUL = 0.1140024783M, CH4CEF = 0.000125604M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000251208M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 28, Name = "潤滑油", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.946167424M, CO2ULL = 0.0190995907M, CO2UUL = 0.0259208731M, CH4CEF = 0.0001205798M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.000024116M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 29, Name = "其他油品", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.76203196M, CO2ULL = 0.0150068213M, CO2UUL = 0.0150068213M, CH4CEF = 0.0001130436M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000226087M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 30, Name = "乙烷", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.8601872992M, CO2ULL = 0.0827922078M, CO2UUL = 0.1136363636M, CH4CEF = 0.0000464316M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000046432M, N2OULL = 0.7M, N2OUUL = 2M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 31, Name = "天然氣", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 1.87903584M, CO2ULL = 0.0320855615M, CO2UUL = 0.0392156863M, CH4CEF = 0.0000334944M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000033494M, N2OULL = 0.7M, N2OUUL = 2M, CEF_Correction = 3, Unit = "M3" },
new Material { Id = 32, Name = "煉油氣", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 2.17043712M, CO2ULL = 0.1631944444M, CO2UUL = 0.1979166667M, CH4CEF = 0.0000376812M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000037681M, N2OULL = 0.7M, N2OUUL = 2M, CEF_Correction = 3, Unit = "M3" },
new Material { Id = 33, Name = "焦爐氣", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 0.780754464M, CO2ULL = 0.1599099099M, CO2UUL = 0.2184684685M, CH4CEF = 0.0000175846M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000017585M, N2OULL = 0.7M, N2OUUL = 2M, CEF_Correction = 3, Unit = "M3" },
new Material { Id = 34, Name = "高爐氣", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 0.845817336M, CO2ULL = 0.1576923077M, CO2UUL = 0.1846153846M, CH4CEF = 0.0000032531M, CH4ULL = 0.7M, CH4UUL = 2M, N2OCEF = 0.0000003253M, N2OULL = 0.7M, N2OUUL = 2M, CEF_Correction = 3, Unit = "M3" },
new Material { Id = 35, Name = "一般廢棄物", Scope = "類別一", EmissionPattern = "固定", CO2CEF = 0.7792272743M, CO2ULL = 0.2006543075M, CO2UUL = 0.3195201745M, CH4CEF = 0.0002549271M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000339903M, N2OULL = 0.625M, N2OUUL = 2.75M, CEF_Correction = 3, Unit = "Kg" },
new Material { Id = 36, Name = "航空汽油", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 2.19807M, CO2ULL = 0.0357142857M, CO2UUL = 0.0428571429M, CH4CEF = 0.000094203M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000188406M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 37, Name = "航空燃油", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 2.3948496M, CO2ULL = 0.0251748252M, CO2UUL = 0.0405594406M, CH4CEF = 0.0001004832M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000200966M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 38, Name = "車用汽油", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 2.263132872M, CO2ULL = 0.025974026M, CO2UUL = 0.0533910534M, CH4CEF = 0.000816426M, CH4ULL = 0.6666666667M, CH4UUL = 2.44M, N2OCEF = 0.0002612563M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L", DataULL = -0.01M, DataUUL = -0.01M },
new Material { Id = 39, Name = "柴油", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 2.606031792M, CO2ULL = 0.020242915M, CO2UUL = 0.0094466937M, CH4CEF = 0.0001371596M, CH4ULL = 0.5897435897M, CH4UUL = 1.4358974359M, N2OCEF = 0.0001371596M, N2OULL = 0.6666666667M, N2OUUL = 2.0769230769M, CEF_Correction = 3, Unit = "L", DataULL = -0.01M, DataUUL = -0.01M },
new Material { Id = 40, Name = "煤油", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 2.55876282M, CO2ULL = 0.0152990264M, CO2UUL = 0.0250347705M, CH4CEF = 0.0001067634M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.0000213527M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 41, Name = "潤滑油", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 2.946167424M, CO2ULL = 0.0190995907M, CO2UUL = 0.0259208731M, CH4CEF = 0.0001205798M, CH4ULL = 0.6666666667M, CH4UUL = 2.3333333333M, N2OCEF = 0.000024116M, N2OULL = 0.6666666667M, N2OUUL = 2.3333333333M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 42, Name = "液化石油氣", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 1.7528812758M, CO2ULL = 0.0237717908M, CO2UUL = 0.0396196513M, CH4CEF = 0.0017223239M, CH4ULL = 0M, CH4UUL = 0M, N2OCEF = 0.0000055559M, N2OULL = 0M, N2OUUL = 0M, CEF_Correction = 3, Unit = "L" },
new Material { Id = 43, Name = "液化天然氣", Scope = "類別一", EmissionPattern = "移動", CO2CEF = 2.11391532M, CO2ULL = 0.0320855615M, CO2UUL = 0.0392156863M, CH4CEF = 0.0034666704M, CH4ULL = 0.4565217391M, CH4UUL = 15.7391304348M, N2OCEF = 0.0001130436M, N2OULL = 0.6666666667M, N2OUUL = 24.6666666667M, CEF_Correction = 3, Unit = "M3" },
new Material { Id = 44, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.555M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 94 },
new Material { Id = 45, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.5625M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 95 },
new Material { Id = 46, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.558M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 96 },
new Material { Id = 47, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.555M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 97 },
new Material { Id = 48, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.543M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 98 },
new Material { Id = 49, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.534M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 99 },
new Material { Id = 50, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.534M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 100 },
new Material { Id = 51, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.529M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 101 },
new Material { Id = 52, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.519M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 102 },
new Material { Id = 53, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.518M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 103 },
new Material { Id = 54, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.525M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 104 },
new Material { Id = 55, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.530M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 105 },
new Material { Id = 56, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.554M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 106 },
new Material { Id = 57, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.533M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 107 },
new Material { Id = 58, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.509M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 108 },
new Material { Id = 59, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.502M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 109 },
new Material { Id = 60, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.509M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 110 },
new Material { Id = 61, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.495M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 111 },
new Material { Id = 62, Name = "外購電力", Scope = "類別二", EmissionPattern = "外購電力", CO2CEF = 0.494M, CO2ULL = -0.07M, CO2UUL = 0.07M, CEF_Correction = 3, DataULL = -0.01M, DataUUL = -0.01M, Year = 112 },
new Material { Id = 63, Name = "廢水處理", Scope = "類別一", EmissionPattern = "逸散", CH4CEF = 0.0038250000M, CEF_Correction = 3 },
new Material { Id = 64, Name = "二氧化碳", Scope = "類別一", EmissionPattern = "逸散", CO2CEF = 1, CEF_Correction = 1 },
new Material { Id = 65, Name = "乙炔", Scope = "類別一", EmissionPattern = "製程", CO2CEF = 3.3841653850M, CEF_Correction = 1 },
new Material { Id = 66, Name = "焊條", Scope = "類別一", EmissionPattern = "製程", CO2CEF = 3.6666666666M, CEF_Correction = 1 },
new Material { Id = 67, Name = "冰箱", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.003000M, CEF_Correction = 3 }, //家用冷凍、冷藏裝備
new Material { Id = 68, Name = "飲水機", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.003000M, CEF_Correction = 3 }, //家用冷凍、冷藏裝備
new Material { Id = 69, Name = "商用冰箱", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.055000M, CEF_Correction = 3 }, //獨立商用冷凍、冷藏裝備	
new Material { Id = 70, Name = "中、大型冰箱", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.200000M, CEF_Correction = 3 }, //中、大型冷凍、冷藏裝備
new Material { Id = 71, Name = "低溫冷凍車", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.330000M, CEF_Correction = 3 }, //交通用冷凍、冷藏裝備
new Material { Id = 72, Name = "乾燥機", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.160000M, CEF_Correction = 3 }, //工業冷凍、冷藏裝備，包括食品加工及冷藏
new Material { Id = 73, Name = "工業冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.160000M, CEF_Correction = 3 }, //工業冷凍、冷藏裝備，包括食品加工及冷藏
new Material { Id = 74, Name = "食品加工冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.160000M, CEF_Correction = 3 }, //工業冷凍、冷藏裝備，包括食品加工及冷藏
new Material { Id = 75, Name = "冰水主機", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.090000M, CEF_Correction = 3 }, //冰水機
new Material { Id = 76, Name = "冷氣機", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.030000M, CEF_Correction = 3 }, //住宅及商業建築冷氣機
new Material { Id = 77, Name = "車用空調", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 0.200000M, CEF_Correction = 3 }, //移動式空氣清靜機
new Material { Id = 78, Name = "丁烷", Scope = "類別一", EmissionPattern = "製程", CO2CEF = 3.0260000000M, CEF_Correction = 1 },
new Material { Id = 79, Name = "WD40", Scope = "類別一", EmissionPattern = "製程", CO2CEF = 0.025M, CEF_Correction = 1 },
new Material { Id = 80, Name = "HFC-236fa", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 1, CEF_Correction = 1 },
new Material { Id = 81, Name = "二氟一氯一溴甲烷", Scope = "類別一", EmissionPattern = "逸散", HFCSCEF = 1, CEF_Correction = 1 }
);
            builder.Entity<GWP>().HasData(
new GWP { Id = 1, Name = "CO2", Num = 1, ARCount = 6 },
new GWP { Id =2, Name = "CH4", Num = 27.9M, ARCount = 6 },
new GWP { Id =3, Name = "N2O", Num = 273, ARCount = 6 },
new GWP { Id =4, Name = "R-12", Num = 12500, ARCount = 6 },
new GWP { Id =5, Name = "R-125", Num = 3740, ARCount = 6 },
new GWP { Id =6, Name = "R-1234yf", Num = 0.501M, ARCount = 6 },
new GWP { Id =7, Name = "R-23", Num = 14600, ARCount = 6 },
new GWP { Id =8, Name = "R-32", Num = 771, ARCount = 6 },
new GWP { Id=9, Name = "R-134A", Num = 1530, ARCount = 6 },
new GWP { Id =10, Name = "FM200", Num = 3600, ARCount = 6 },
new GWP { Id =11, Name = "R-22", Num = 1960, ARCount = 6 },
new GWP { Id =12, Name = "R-410A", Num = 2255.5M, ARCount = 6 },
new GWP { Id =13, Name = "R-600A", Num = 0.006M, ARCount = 6 },
new GWP { Id =14, Name = "R-417A", Num = 2127, ARCount = 6 },
new GWP { Id =15, Name = "R-404A", Num = 4728, ARCount = 6 },
new GWP { Id =16, Name = "R-407C", Num = 1908, ARCount = 6 },
new GWP { Id =17, Name = "R-407F", Num = 1965.3M, ARCount = 6 }, //40% R-134a / 30% R-125 / 30% R-32
new GWP { Id =18, Name = "R-452A", Num = 2291.5603M, ARCount = 6 }, //30 % R-1234yf / 11% R-32 / 59% R-125 
new GWP { Id =19, Name = "R-507A", Num = 4475, ARCount = 6 },
new GWP { Id =20, Name = "NF3", Num = 17400, ARCount = 6 },
new GWP { Id =21, Name = "SF6", Num = 24300, ARCount = 6 },
new GWP { Id =22, Name = "二氟一氯一溴甲烷", Num = 1930, ARCount = 6 },
new GWP { Id =23, Name = "HFC-236fa", Num = 8690, ARCount = 6 }
                );
            builder.Entity<DeviceData>().HasData(
                new DeviceData { Id = 1, Name = "冷氣機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-410A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
        new DeviceData { Id = 2, Name = "冰水主機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
        new DeviceData { Id = 3, Name = "冰箱", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
        new DeviceData { Id = 4, Name = "飲水機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
        new DeviceData { Id = 5, Name = "乾燥機", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
        new DeviceData { Id = 6, Name = "車用空調", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
        new DeviceData { Id = 7, Name = "工業冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
        new DeviceData { Id = 8, Name = "緊急發電機", Scope = "類別一", EmissionPattern = "固定", Material = "柴油", Device_Correction = 3, Data_Correction = 3, unit = "公升" },
                new DeviceData { Id = 9, Name = "廚房", Scope = "類別一", EmissionPattern = "固定", Material = "液化石油氣", Device_Correction = 2, Data_Correction = 2, unit = "公斤" },
                new DeviceData { Id = 10, Name = "公務車", Scope = "類別一", EmissionPattern = "移動", Material = "車用汽油", Device_Correction = 2, Data_Correction = 2, unit = "公升" },
                new DeviceData { Id = 11, Name = "堆高機", Scope = "類別一", EmissionPattern = "移動", Material = "柴油", Device_Correction = 2, Data_Correction = 2, unit = "公升" },
                new DeviceData { Id = 12, Name = "二氧化碳滅火器", Scope = "類別一", EmissionPattern = "逸散", Material = "二氧化碳", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                new DeviceData { Id = 13, Name = "二氧化碳", Scope = "類別一", EmissionPattern = "逸散", Material = "二氧化碳", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                new DeviceData { Id = 14, Name = "WD40", Scope = "類別一", EmissionPattern = "製程", Material = "WD40", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                new DeviceData { Id = 15, Name = "海龍1211", Scope = "類別一", EmissionPattern = "逸散", Material = "二氟一氯一溴甲烷", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                new DeviceData { Id = 16, Name = "FM200", Scope = "類別一", EmissionPattern = "逸散", Material = "FM200", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                new DeviceData { Id = 17, Name = "化糞池", Scope = "類別一", EmissionPattern = "逸散", Material = "廢水處理", Device_Correction = 3, Data_Correction = 3, unit = "人-年" },
                new DeviceData { Id = 18, Name = "電力", Scope = "類別二", EmissionPattern = "外購電力", Material = "外購電力", Device_Correction = 1, Data_Correction = 1, unit = "度" },
                new DeviceData { Id = 19, Name = "乙炔", Scope = "類別一", EmissionPattern = "製程", Material = "乙炔", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                new DeviceData { Id = 20, Name = "焊條", Scope = "類別一", EmissionPattern = "製程", Material = "焊條", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                 new DeviceData { Id = 21, Name = "工業冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                  new DeviceData { Id = 22, Name = "商用冰箱", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                   new DeviceData { Id = 23, Name = "中、大型冰箱", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                    new DeviceData { Id = 24, Name = "低溫冷凍車", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                     new DeviceData { Id = 25, Name = "食品加工冷藏、冷凍", Scope = "類別一", EmissionPattern = "逸散", Material = "R-134A", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                     new DeviceData { Id = 26, Name = "瓦斯罐", Scope = "類別一", EmissionPattern = "製程", Material = "丁烷", Device_Correction = 3, Data_Correction = 3, unit = "公斤" },
                     new DeviceData { Id = 27, Name = "六氟丙烷滅火器", Scope = "類別一", EmissionPattern = "逸散", Material = "HFC-236fa", Device_Correction = 3, Data_Correction = 3, unit = "公斤" }

    );
            builder.Entity<DefaultDevices>().HasData(
new DefaultDevices { Id = 1, Name = "緊急發電機", Material = "柴油", Scope = "類別一", EmissionPattern = "固定" },
new DefaultDevices { Id = 2, Name = "公務車", Material = "柴油", Scope = "類別一", EmissionPattern = "移動" },
new DefaultDevices { Id = 3, Name = "公務車", Material = "車用汽油", Scope = "類別一", EmissionPattern = "移動" },
new DefaultDevices { Id = 4, Name = "冷氣機", Material = "R-410A", Scope = "類別一", EmissionPattern = "逸散" },
new DefaultDevices { Id = 5, Name = "飲水機", Material = "R-134A", Scope = "類別一", EmissionPattern = "逸散" },
new DefaultDevices { Id = 6, Name = "乾燥機", Material = "R-134A", Scope = "類別一", EmissionPattern = "逸散" },
new DefaultDevices { Id = 7, Name = "冰水主機", Material = "R-134A", Scope = "類別一", EmissionPattern = "逸散" },
new DefaultDevices { Id = 8, Name = "車用空調", Material = "R-134A", Scope = "類別一", EmissionPattern = "逸散" },
new DefaultDevices { Id = 9, Name = "化糞池", Material = "廢水處理", Scope = "類別一", EmissionPattern = "逸散" },
new DefaultDevices { Id = 10, Name = "電力", Material = "外購電力", Scope = "類別二", EmissionPattern = "外購電力" }
                );
        }
    }
}