using Microsoft.EntityFrameworkCore;
using SeymourBot.Storage.Information.Tables;

namespace SeymourBot.DataAccess.Storage.Information
{
    class InfoCommandContext : DbContext
    {
        public DbSet<InfoCommandTable> InfoCommandTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlite("Data Source=SeymourInfoDB.db");

            string dbPath = @"C:\Users\Monty\Documents\programming\Github Repos\SeymourBot\SeymourBot\Database\";
            optionsBuilder.UseSqlite($"Data Source={dbPath}SeymourInfoDB.db");


            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
