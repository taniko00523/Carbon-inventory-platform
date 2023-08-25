using Carbon_inventory_platform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        }
    }
}