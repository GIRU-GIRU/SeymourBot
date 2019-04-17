using Microsoft.EntityFrameworkCore;
using SeymourBot.Storage.Information.Tables;
using Toolbox.Resources;

namespace SeymourBot.DataAccess.Storage.Information
{
    class InfoCommandContext : DbContext
    {
        public DbSet<InfoCommandTable> InfoCommandTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=F:\\SeymourV2\\SeymourBot\\SeymourBot\\Database\\SeymourInfoDB.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
