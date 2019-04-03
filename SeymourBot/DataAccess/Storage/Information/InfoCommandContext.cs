using Microsoft.EntityFrameworkCore;
using SeymourBot.Config;
using SeymourBot.Storage.Information.Tables;

namespace SeymourBot.DataAccess.Storage.Information
{
    class InfoCommandContext : DbContext
    {
        public DbSet<InfoCommandTable> InfoCommandTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={ConfigManager.GetSeymourInfoDBPath()}");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
