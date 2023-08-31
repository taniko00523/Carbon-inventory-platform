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
        public DbSet<CH4> CH4s { get; set; } = null!;
        public DbSet<CO2> CO2s { get; set; } = null!;
        public DbSet<N2O> N2Os { get; set; } = null!;
        public DbSet<ActivityData> ActivityDatas { get; set; } = null!;
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
                entity.HasOne(e => e.ActivityData);
            });

            builder.Entity<Material>(entity =>
                {
                    entity.HasMany(e => e.Devices);
                    entity.HasOne(e => e.CO2);
                    entity.HasOne(e => e.CH4);
                    entity.HasOne(e => e.N2O);
                });

            DataSeed(builder);
        }
        private void DataSeed(ModelBuilder builder)
        {
            builder.Entity<CO2>().HasData(
                new CO2 { Id = 1, CEF = 2.33286F, UncertaintyLowerLimit = 0.077167F, UncertaintyUpperLimit = 0.067653F },
new CO2 { Id = 2, CEF = 2.693285F, UncertaintyLowerLimit = 0.077167F, UncertaintyUpperLimit = 0.067653F },
new CO2 { Id = 3, CEF = 2.408113F, UncertaintyLowerLimit = 0.077167F, UncertaintyUpperLimit = 0.067653F },
new CO2 { Id = 4, CEF = 2.922093F, UncertaintyLowerLimit = 0.03764F, UncertaintyUpperLimit = 0.027467F },
new CO2 { Id = 5, CEF = 2.693285F, UncertaintyLowerLimit = 0.077167F, UncertaintyUpperLimit = 0.067653F },
new CO2 { Id = 6, CEF = 2.408113F, UncertaintyLowerLimit = 0.053911F, UncertaintyUpperLimit = 0.053911F },
new CO2 { Id = 7, CEF = 1.971522F, UncertaintyLowerLimit = 0.034339F, UncertaintyUpperLimit = 0.040583F },
new CO2 { Id = 8, CEF = 2.253168F, UncertaintyLowerLimit = 0.034339F, UncertaintyUpperLimit = 0.040583F },
new CO2 { Id = 9, CEF = 1.202633F, UncertaintyLowerLimit = 0.1F, UncertaintyUpperLimit = 0.138614F },
new CO2 { Id = 10, CEF = 0.95287F, UncertaintyLowerLimit = 0.157009F, UncertaintyUpperLimit = 0.168224F },
new CO2 { Id = 11, CEF = 1.035387F, UncertaintyLowerLimit = 0.056604F, UncertaintyUpperLimit = 0.018868F },
new CO2 { Id = 12, CEF = 1.551209F, UncertaintyLowerLimit = 0.104615F, UncertaintyUpperLimit = 0.117949F },
new CO2 { Id = 13, CEF = 3.135913F, UncertaintyLowerLimit = 0.105607F, UncertaintyUpperLimit = 0.11215F },
new CO2 { Id = 14, CEF = 3.347347F, UncertaintyLowerLimit = 0.149744F, UncertaintyUpperLimit = 0.179487F },
new CO2 { Id = 15, CEF = 2.19807F, UncertaintyLowerLimit = 0.035714F, UncertaintyUpperLimit = 0.042857F },
new CO2 { Id = 16, CEF = 2.39485F, UncertaintyLowerLimit = 0.025175F, UncertaintyUpperLimit = 0.040559F },
new CO2 { Id = 17, CEF = 2.762032F, UncertaintyLowerLimit = 0.030014F, UncertaintyUpperLimit = 0.030014F },
new CO2 { Id = 18, CEF = 2.119027F, UncertaintyLowerLimit = 0.1F, UncertaintyUpperLimit = 0.109091F },
new CO2 { Id = 19, CEF = 2.839525F, UncertaintyLowerLimit = 0.0919F, UncertaintyUpperLimit = 0.096573F },
new CO2 { Id = 20, CEF = 2.558763F, UncertaintyLowerLimit = 0.015299F, UncertaintyUpperLimit = 0.025035F },
new CO2 { Id = 21, CEF = 2.794563F, UncertaintyLowerLimit = 0.075034F, UncertaintyUpperLimit = 0.080491F },
new CO2 { Id = 22, CEF = 2.606032F, UncertaintyLowerLimit = 0.020243F, UncertaintyUpperLimit = 0.009447F },
new CO2 { Id = 23, CEF = 2.263133F, UncertaintyLowerLimit = 0.025974F, UncertaintyUpperLimit = 0.053391F },
new CO2 { Id = 24, CEF = 3.11096F, UncertaintyLowerLimit = 0.024548F, UncertaintyUpperLimit = 0.018088F },
new CO2 { Id = 25, CEF = 1.752881F, UncertaintyLowerLimit = 0.023772F, UncertaintyUpperLimit = 0.03962F },
new CO2 { Id = 26, CEF = 2.393761F, UncertaintyLowerLimit = 0.05457F, UncertaintyUpperLimit = 0.040928F },
new CO2 { Id = 27, CEF = 3.378748F, UncertaintyLowerLimit = 0.095415F, UncertaintyUpperLimit = 0.114002F },
new CO2 { Id = 28, CEF = 2.946167F, UncertaintyLowerLimit = 0.0191F, UncertaintyUpperLimit = 0.025921F },
new CO2 { Id = 29, CEF = 2.762032F, UncertaintyLowerLimit = 0.015007F, UncertaintyUpperLimit = 0.015007F },
new CO2 { Id = 30, CEF = 2.860187F, UncertaintyLowerLimit = 0.082792F, UncertaintyUpperLimit = 0.113636F },
new CO2 { Id = 31, CEF = 1.879036F, UncertaintyLowerLimit = 0.032086F, UncertaintyUpperLimit = 0.039216F },
new CO2 { Id = 32, CEF = 2.170437F, UncertaintyLowerLimit = 0.163194F, UncertaintyUpperLimit = 0.197917F },
new CO2 { Id = 33, CEF = 0.780754F, UncertaintyLowerLimit = 0.15991F, UncertaintyUpperLimit = 0.218468F },
new CO2 { Id = 34, CEF = 0.845817F, UncertaintyLowerLimit = 0.157692F, UncertaintyUpperLimit = 0.184615F },
new CO2 { Id = 35, CEF = 0.779227F, UncertaintyLowerLimit = 0.200654F, UncertaintyUpperLimit = 0.31952F },
new CO2 { Id = 36, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 37, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 38, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 39, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 40, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 41, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 42, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 43, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 44, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 45, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 46, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 47, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CO2 { Id = 48, CEF = 2.19807F, UncertaintyLowerLimit = 0.035714F, UncertaintyUpperLimit = 0.042857F },
new CO2 { Id = 49, CEF = 2.39485F, UncertaintyLowerLimit = 0.025175F, UncertaintyUpperLimit = 0.040559F },
new CO2 { Id = 50, CEF = 2.263133F, UncertaintyLowerLimit = 0.025974F, UncertaintyUpperLimit = 0.053391F },
new CO2 { Id = 51, CEF = 2.606032F, UncertaintyLowerLimit = 0.020243F, UncertaintyUpperLimit = 0.009447F },
new CO2 { Id = 52, CEF = 2.558763F, UncertaintyLowerLimit = 0.015299F, UncertaintyUpperLimit = 0.025035F },
new CO2 { Id = 53, CEF = 2.946167F, UncertaintyLowerLimit = 0.0191F, UncertaintyUpperLimit = 0.025921F },
new CO2 { Id = 54, CEF = 1.752881F, UncertaintyLowerLimit = 0.023772F, UncertaintyUpperLimit = 0.03962F },
new CO2 { Id = 55, CEF = 2.113915F, UncertaintyLowerLimit = 0.032086F, UncertaintyUpperLimit = 0.039216F }
);

            builder.Entity<CH4>().HasData(
                new CH4 { Id = 1, CEF = 0.000025F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 2, CEF = 0.000028F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 3, CEF = 0.000025F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 4, CEF = 0.00003F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 5, CEF = 0.000028F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 6, CEF = 0.000025F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 7, CEF = 0.000021F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 8, CEF = 0.000023F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 9, CEF = 0.000012F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 10, CEF = 0.000009F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 11, CEF = 0.00001F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 12, CEF = 0.000016F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 13, CEF = 0.000029F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 14, CEF = 0.000103F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 15, CEF = 0.000094F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 16, CEF = 0.0001F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 17, CEF = 0.000113F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 18, CEF = 0.000083F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 19, CEF = 0.000133F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 20, CEF = 0.000107F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 21, CEF = 0.000108F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 22, CEF = 0.000106F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 23, CEF = 0.000098F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 24, CEF = 0.000121F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 25, CEF = 0.000028F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 26, CEF = 0.000098F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 27, CEF = 0.000126F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 28, CEF = 0.000121F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 29, CEF = 0.000113F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 30, CEF = 0.000046F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 31, CEF = 0.000033F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 32, CEF = 0.000038F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 33, CEF = 0.000018F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 34, CEF = 0.000003F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new CH4 { Id = 35, CEF = 0.000255F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 36, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 37, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 38, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 39, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 40, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 41, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 42, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 43, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 44, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 45, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 46, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 47, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 48, CEF = 0.000094F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 49, CEF = 0.0001F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 50, CEF = 0.000816F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.44F },
new CH4 { Id = 51, CEF = 0.000137F, UncertaintyLowerLimit = 0.589744F, UncertaintyUpperLimit = 1.435897F },
new CH4 { Id = 52, CEF = 0.000107F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 53, CEF = 0.000121F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new CH4 { Id = 54, CEF = 0.001722F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new CH4 { Id = 55, CEF = 0.003467F, UncertaintyLowerLimit = 0.456522F, UncertaintyUpperLimit = 15.73913F }
                );

            builder.Entity<N2O>().HasData(
                new N2O { Id = 1, CEF = 0.000037F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 2, CEF = 0.000043F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 3, CEF = 0.000038F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 4, CEF = 0.000045F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 5, CEF = 0.000043F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 6, CEF = 0.000038F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 7, CEF = 0.000031F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 8, CEF = 0.000035F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 9, CEF = 0.000018F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 10, CEF = 0.000013F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 11, CEF = 0.000015F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 12, CEF = 0.000024F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 13, CEF = 0.000044F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 14, CEF = 0.000021F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 15, CEF = 0.000019F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 16, CEF = 0.00002F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 17, CEF = 0.000023F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 18, CEF = 0.000017F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 19, CEF = 0.000027F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 20, CEF = 0.000021F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 21, CEF = 0.000022F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 22, CEF = 0.000021F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 23, CEF = 0.00002F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 24, CEF = 0.000024F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 25, CEF = 0.000003F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new N2O { Id = 26, CEF = 0.00002F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 27, CEF = 0.000025F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 28, CEF = 0.000024F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 29, CEF = 0.000023F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 30, CEF = 0.000005F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new N2O { Id = 31, CEF = 0.000003F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new N2O { Id = 32, CEF = 0.000004F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new N2O { Id = 33, CEF = 0.000002F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new N2O { Id = 34, CEF = 0F, UncertaintyLowerLimit = 0.7F, UncertaintyUpperLimit = 2F },
new N2O { Id = 35, CEF = 0.000034F, UncertaintyLowerLimit = 0.625F, UncertaintyUpperLimit = 2.75F },
new N2O { Id = 36, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 37, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 38, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 39, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 40, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 41, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 42, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 43, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 44, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 45, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 46, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 47, CEF = 0F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 48, CEF = 0.000019F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 49, CEF = 0.00002F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 50, CEF = 0.000261F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 51, CEF = 0.000137F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.076923F },
new N2O { Id = 52, CEF = 0.000021F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 53, CEF = 0.000024F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 2.333333F },
new N2O { Id = 54, CEF = 0.000006F, UncertaintyLowerLimit = 0F, UncertaintyUpperLimit = 0F },
new N2O { Id = 55, CEF = 0.000113F, UncertaintyLowerLimit = 0.666667F, UncertaintyUpperLimit = 24.666667F }
                );

            builder.Entity<Material>().HasData(
new Material { Id = 1, Name = "自產煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 1, CO2Id = 1, N2OId = 1, Unit = "Kg" },
new Material { Id = 2, Name = "原料煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 2, CO2Id = 2, N2OId = 2, Unit = "Kg" },
new Material { Id = 3, Name = "燃料煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 3, CO2Id = 3, N2OId = 3, Unit = "Kg" },
new Material { Id = 4, Name = "無煙煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 4, CO2Id = 4, N2OId = 4, Unit = "Kg" },
new Material { Id = 5, Name = "焦煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 5, CO2Id = 5, N2OId = 5, Unit = "Kg" },
new Material { Id = 6, Name = "煙煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 6, CO2Id = 6, N2OId = 6, Unit = "Kg" },
new Material { Id = 7, Name = "亞煙煤(發電)", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 7, CO2Id = 7, N2OId = 7, Unit = "Kg" },
new Material { Id = 8, Name = "亞煙煤(其他)", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 8, CO2Id = 8, N2OId = 8, Unit = "Kg" },
new Material { Id = 9, Name = "褐煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 9, CO2Id = 9, N2OId = 9, Unit = "Kg" },
new Material { Id = 10, Name = "油頁岩", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 10, CO2Id = 10, N2OId = 10, Unit = "Kg" },
new Material { Id = 11, Name = "泥煤", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 11, CO2Id = 11, N2OId = 11, Unit = "Kg" },
new Material { Id = 12, Name = "煤球", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 12, CO2Id = 12, N2OId = 12, Unit = "Kg" },
new Material { Id = 13, Name = "焦炭", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 13, CO2Id = 13, N2OId = 13, Unit = "Kg" },
new Material { Id = 14, Name = "石油焦", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 14, CO2Id = 14, N2OId = 14, Unit = "Kg" },
new Material { Id = 15, Name = "航空汽油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 15, CO2Id = 15, N2OId = 15, Unit = "L" },
new Material { Id = 16, Name = "航空燃油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 16, CO2Id = 16, N2OId = 16, Unit = "L" },
new Material { Id = 17, Name = "原油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 17, CO2Id = 17, N2OId = 17, Unit = "L" },
new Material { Id = 18, Name = "奧里油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 18, CO2Id = 18, N2OId = 18, Unit = "Kg" },
new Material { Id = 19, Name = "天然氣凝結油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 19, CO2Id = 19, N2OId = 19, Unit = "M3" },
new Material { Id = 20, Name = "煤油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 20, CO2Id = 20, N2OId = 20, Unit = "L" },
new Material { Id = 21, Name = "頁岩油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 21, CO2Id = 21, N2OId = 21, Unit = "Kg" },
new Material { Id = 22, Name = "柴油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 22, CO2Id = 22, N2OId = 22, Unit = "L" },
new Material { Id = 23, Name = "車用汽油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 23, CO2Id = 23, N2OId = 23, Unit = "L" },
new Material { Id = 24, Name = "蒸餘油 (燃料油)", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 24, CO2Id = 24, N2OId = 24, Unit = "L" },
new Material { Id = 25, Name = "液化石油氣", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 25, CO2Id = 25, N2OId = 25, Unit = "L" },
new Material { Id = 26, Name = "石油腦", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 26, CO2Id = 26, N2OId = 26, Unit = "L" },
new Material { Id = 27, Name = "柏油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 27, CO2Id = 27, N2OId = 27, Unit = "L" },
new Material { Id = 28, Name = "潤滑油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 28, CO2Id = 28, N2OId = 28, Unit = "L" },
new Material { Id = 29, Name = "其他油品", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 29, CO2Id = 29, N2OId = 29, Unit = "L" },
new Material { Id = 30, Name = "乙烷", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 30, CO2Id = 30, N2OId = 30, Unit = "L" },
new Material { Id = 31, Name = "天然氣", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 31, CO2Id = 31, N2OId = 31, Unit = "M3" },
new Material { Id = 32, Name = "煉油氣", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 32, CO2Id = 32, N2OId = 32, Unit = "M3" },
new Material { Id = 33, Name = "焦爐氣", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 33, CO2Id = 33, N2OId = 33, Unit = "M3" },
new Material { Id = 34, Name = "高爐氣", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 34, CO2Id = 34, N2OId = 34, Unit = "M3" },
new Material { Id = 35, Name = "一般廢棄物", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 35, CO2Id = 35, N2OId = 35, Unit = "Kg" },
new Material { Id = 36, Name = "事業廢棄物", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 36, CO2Id = 36, N2OId = 36, Unit = "" },
new Material { Id = 37, Name = "其他非化石燃料", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 37, CO2Id = 37, N2OId = 37, Unit = "" },
new Material { Id = 38, Name = "木頭－固態", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 38, CO2Id = 38, N2OId = 38, Unit = "" },
new Material { Id = 39, Name = "黑液", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 39, CO2Id = 39, N2OId = 39, Unit = "" },
new Material { Id = 40, Name = "木炭", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 40, CO2Id = 40, N2OId = 40, Unit = "" },
new Material { Id = 41, Name = "其他固體生質燃料", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 41, CO2Id = 41, N2OId = 41, Unit = "" },
new Material { Id = 42, Name = "生質汽油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 42, CO2Id = 42, N2OId = 42, Unit = "" },
new Material { Id = 43, Name = "生質柴油", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 43, CO2Id = 43, N2OId = 43, Unit = "" },
new Material { Id = 44, Name = "其他液態生質燃料", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 44, CO2Id = 44, N2OId = 44, Unit = "" },
new Material { Id = 45, Name = "掩埋場沼氣", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 45, CO2Id = 45, N2OId = 45, Unit = "" },
new Material { Id = 46, Name = "污泥沼氣", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 46, CO2Id = 46, N2OId = 46, Unit = "" },
new Material { Id = 47, Name = "其他氣態生質燃料", Scope = "範疇1", EmissionPattern = "固定", CH4Id = 47, CO2Id = 47, N2OId = 47, Unit = "" },
new Material { Id = 48, Name = "航空汽油", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 48, CO2Id = 48, N2OId = 48, Unit = "L" },
new Material { Id = 49, Name = "航空燃油", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 49, CO2Id = 49, N2OId = 49, Unit = "L" },
new Material { Id = 50, Name = "車用汽油", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 50, CO2Id = 50, N2OId = 50, Unit = "L" },
new Material { Id = 51, Name = "柴油", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 51, CO2Id = 51, N2OId = 51, Unit = "L" },
new Material { Id = 52, Name = "煤油", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 52, CO2Id = 52, N2OId = 52, Unit = "L" },
new Material { Id = 53, Name = "潤滑油", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 53, CO2Id = 53, N2OId = 53, Unit = "L" },
new Material { Id = 54, Name = "液化石油氣", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 54, CO2Id = 54, N2OId = 54, Unit = "L" },
new Material { Id = 55, Name = "液化天然氣", Scope = "範疇1", EmissionPattern = "移動", CH4Id = 55, CO2Id = 55, N2OId = 55, Unit = "M3" }
                );
        }
    }
}