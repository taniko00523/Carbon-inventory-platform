using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;

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
                entity.HasMany(e => e.Areas);
                entity.HasOne(e => e.Material);
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
                new Material { Id = 1, Name = "自產煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.33286F, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.000025F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000037F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 2, Name = "原料煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.693285F, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.000028F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000043F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 3, Name = "燃料煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.408113F, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.000025F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000038F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 4, Name = "無煙煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.922093F, CO2ULL = 0.03764F, CO2UUL = 0.027467F, CH4CEF = 0.00003F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000045F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 5, Name = "焦煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.693285F, CO2ULL = 0.077167F, CO2UUL = 0.067653F, CH4CEF = 0.000028F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000043F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 6, Name = "煙煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.408113F, CO2ULL = 0.053911F, CO2UUL = 0.053911F, CH4CEF = 0.000025F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000038F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 7, Name = "亞煙煤(發電)", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 1.971522F, CO2ULL = 0.034339F, CO2UUL = 0.040583F, CH4CEF = 0.000021F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000031F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 8, Name = "亞煙煤(其他)", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.253168F, CO2ULL = 0.034339F, CO2UUL = 0.040583F, CH4CEF = 0.000023F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000035F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 9, Name = "褐煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 1.202633F, CO2ULL = 0.1F, CO2UUL = 0.138614F, CH4CEF = 0.000012F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000018F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 10, Name = "油頁岩", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 0.95287F, CO2ULL = 0.157009F, CO2UUL = 0.168224F, CH4CEF = 0.000009F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000013F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 11, Name = "泥煤", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 1.035387F, CO2ULL = 0.056604F, CO2UUL = 0.018868F, CH4CEF = 0.00001F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000015F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 12, Name = "煤球", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 1.551209F, CO2ULL = 0.104615F, CO2UUL = 0.117949F, CH4CEF = 0.000016F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000024F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 13, Name = "焦炭", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 3.135913F, CO2ULL = 0.105607F, CO2UUL = 0.11215F, CH4CEF = 0.000029F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000044F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 14, Name = "石油焦", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 3.347347F, CO2ULL = 0.149744F, CO2UUL = 0.179487F, CH4CEF = 0.000103F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000021F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 15, Name = "航空汽油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.19807F, CO2ULL = 0.035714F, CO2UUL = 0.042857F, CH4CEF = 0.000094F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000019F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 16, Name = "航空燃油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.39485F, CO2ULL = 0.025175F, CO2UUL = 0.040559F, CH4CEF = 0.0001F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 17, Name = "原油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.762032F, CO2ULL = 0.030014F, CO2UUL = 0.030014F, CH4CEF = 0.000113F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000023F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 18, Name = "奧里油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.119027F, CO2ULL = 0.1F, CO2UUL = 0.109091F, CH4CEF = 0.000083F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000017F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 19, Name = "天然氣凝結油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.839525F, CO2ULL = 0.0919F, CO2UUL = 0.096573F, CH4CEF = 0.000133F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000027F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "M3" },
new Material { Id = 20, Name = "煤油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.558763F, CO2ULL = 0.015299F, CO2UUL = 0.025035F, CH4CEF = 0.000107F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000021F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 21, Name = "頁岩油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.794563F, CO2ULL = 0.075034F, CO2UUL = 0.080491F, CH4CEF = 0.000108F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000022F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "Kg" },
new Material { Id = 22, Name = "柴油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.606032F, CO2ULL = 0.020243F, CO2UUL = 0.009447F, CH4CEF = 0.000106F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000021F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 23, Name = "車用汽油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.263133F, CO2ULL = 0.025974F, CO2UUL = 0.053391F, CH4CEF = 0.000098F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 24, Name = "蒸餘油 (燃料油)", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 3.11096F, CO2ULL = 0.024548F, CO2UUL = 0.018088F, CH4CEF = 0.000121F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000024F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 25, Name = "液化石油氣", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 1.752881F, CO2ULL = 0.023772F, CO2UUL = 0.03962F, CH4CEF = 0.000028F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000003F, N2OULL = 0.7F, N2OUUL = 2F, Unit = "L" },
new Material { Id = 26, Name = "石油腦", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.393761F, CO2ULL = 0.05457F, CO2UUL = 0.040928F, CH4CEF = 0.000098F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 27, Name = "柏油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 3.378748F, CO2ULL = 0.095415F, CO2UUL = 0.114002F, CH4CEF = 0.000126F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000025F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 28, Name = "潤滑油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.946167F, CO2ULL = 0.0191F, CO2UUL = 0.025921F, CH4CEF = 0.000121F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000024F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 29, Name = "其他油品", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.762032F, CO2ULL = 0.015007F, CO2UUL = 0.015007F, CH4CEF = 0.000113F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000023F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 30, Name = "乙烷", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.860187F, CO2ULL = 0.082792F, CO2UUL = 0.113636F, CH4CEF = 0.000046F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000005F, N2OULL = 0.7F, N2OUUL = 2F, Unit = "L" },
new Material { Id = 31, Name = "天然氣", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 1.879036F, CO2ULL = 0.032086F, CO2UUL = 0.039216F, CH4CEF = 0.000033F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000003F, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 32, Name = "煉油氣", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 2.170437F, CO2ULL = 0.163194F, CO2UUL = 0.197917F, CH4CEF = 0.000038F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000004F, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 33, Name = "焦爐氣", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 0.780754F, CO2ULL = 0.15991F, CO2UUL = 0.218468F, CH4CEF = 0.000018F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0.000002F, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 34, Name = "高爐氣", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 0.845817F, CO2ULL = 0.157692F, CO2UUL = 0.184615F, CH4CEF = 0.000003F, CH4ULL = 0.7F, CH4UUL = 2F, N2OCEF = 0F, N2OULL = 0.7F, N2OUUL = 2F, Unit = "M3" },
new Material { Id = 35, Name = "一般廢棄物", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = 0.779227F, CO2ULL = 0.200654F, CO2UUL = 0.31952F, CH4CEF = 0.000255F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000034F, N2OULL = 0.625F, N2OUUL = 2.75F, Unit = "Kg" },
new Material { Id = 36, Name = "事業廢棄物", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 37, Name = "其他非化石燃料", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 38, Name = "木頭－固態", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 39, Name = "黑液", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 40, Name = "木炭", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 41, Name = "其他固體生質燃料", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 42, Name = "生質汽油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 43, Name = "生質柴油", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 44, Name = "其他液態生質燃料", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 45, Name = "掩埋場沼氣", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 46, Name = "污泥沼氣", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 47, Name = "其他氣態生質燃料", Scope = "範疇1", EmissionPattern = "固定", CO2CEF = null, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 48, Name = "航空汽油", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 2.19807F, CO2ULL = 0.035714F, CO2UUL = 0.042857F, CH4CEF = 0.000094F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000019F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 49, Name = "航空燃油", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 2.39485F, CO2ULL = 0.025175F, CO2UUL = 0.040559F, CH4CEF = 0.0001F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.00002F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 50, Name = "車用汽油", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 2.263133F, CO2ULL = 0.025974F, CO2UUL = 0.053391F, CH4CEF = 0.000816F, CH4ULL = 0.666667F, CH4UUL = 2.44F, N2OCEF = 0.000261F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 51, Name = "柴油", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 2.606032F, CO2ULL = 0.020243F, CO2UUL = 0.009447F, CH4CEF = 0.000137F, CH4ULL = 0.589744F, CH4UUL = 1.435897F, N2OCEF = 0.000137F, N2OULL = 0.666667F, N2OUUL = 2.076923F, Unit = "L" },
new Material { Id = 52, Name = "煤油", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 2.558763F, CO2ULL = 0.015299F, CO2UUL = 0.025035F, CH4CEF = 0.000107F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000021F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 53, Name = "潤滑油", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 2.946167F, CO2ULL = 0.0191F, CO2UUL = 0.025921F, CH4CEF = 0.000121F, CH4ULL = 0.666667F, CH4UUL = 2.333333F, N2OCEF = 0.000024F, N2OULL = 0.666667F, N2OUUL = 2.333333F, Unit = "L" },
new Material { Id = 54, Name = "液化石油氣", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 1.752881F, CO2ULL = 0.023772F, CO2UUL = 0.03962F, CH4CEF = 0.001722F, CH4ULL = 0F, CH4UUL = 0F, N2OCEF = 0.000006F, N2OULL = 0F, N2OUUL = 0F, Unit = "L" },
new Material { Id = 55, Name = "液化天然氣", Scope = "範疇1", EmissionPattern = "移動", CO2CEF = 2.113915F, CO2ULL = 0.032086F, CO2UUL = 0.039216F, CH4CEF = 0.003467F, CH4ULL = 0.456522F, CH4UUL = 15.73913F, N2OCEF = 0.000113F, N2OULL = 0.666667F, N2OUUL = 24.666667F, Unit = "M3" },
new Material { Id = 56, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.559F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 57, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.559F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 58, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.559F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 59, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.559F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 60, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.559F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 61, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.559F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 62, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.564F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 63, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.559F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 64, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.557F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 65, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.543F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 66, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.535F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 67, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.536F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 68, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.532F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 69, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.522F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 70, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.521F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 71, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.528F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 72, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.529F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 73, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.529F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" },
new Material { Id = 74, Name = "外購電力", Scope = "範疇2", EmissionPattern = "其他電力", CO2CEF = 0.533F, CO2ULL = null, CO2UUL = null, CH4CEF = null, CH4ULL = null, CH4UUL = null, N2OCEF = null, N2OULL = null, N2OUUL = null, Unit = "" }
                );
        }
    }
}