using Microsoft.EntityFrameworkCore;

namespace NBAFantasy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
    }
}
