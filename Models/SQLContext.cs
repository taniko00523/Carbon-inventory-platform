using Microsoft.EntityFrameworkCore;

namespace 碳盤查平台.Models
{
    public class SQLContext : DbContext
    {
        public SQLContext(DbContextOptions<SQLContext> options) : base(options)
        {

        }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Data> Datas { get; set; }
    }
}
