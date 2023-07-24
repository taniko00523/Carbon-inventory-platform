using Microsoft.EntityFrameworkCore;

namespace 碳盤查平台.Models
{
    public class SQLContext : DbContext
    {
        public SQLContext(DbContextOptions<SQLContext> options) : base(options)
        {

        }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
    }
}
