using Microsoft.EntityFrameworkCore;
using SeymourBot.DataAccess.Storage.Filter.Tables;
using System;
using System.IO;
using System.Reflection;

namespace SeymourBot.DataAccess.Storage.Filter
{
    class FilterContext : DbContext
    {
        public DbSet<FilterTable> filterTables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\Database\\SeymourFilterDB.db";
            string databaseLocation = new Uri(path).LocalPath;

            optionsBuilder.UseSqlite($"Data Source={databaseLocation}");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
