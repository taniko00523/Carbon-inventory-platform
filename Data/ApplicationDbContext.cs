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

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<CH4> CH4s { get; set; }
        public virtual DbSet<CO2> CO2s { get; set; }
        public virtual DbSet<N2O> N2Os { get; set; }
        public virtual DbSet<ActivityData> ActivityDatas { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Funtion> Funtions { get; set; }
        public virtual DbSet<Authorization> Authorization { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }


            
    }
}