using Microsoft.EntityFrameworkCore;
using SeymourBot.Storage.Information.Tables;
using System;
using System.IO;
using System.Reflection;
using Toolbox.Resources;

namespace SeymourBot.DataAccess.Storage.Information
{
    class InfoCommandContext : DbContext
    {
        public DbSet<InfoCommandTable> InfoCommandTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\Database\\SeymourInfoDB.db";
            string databaseLocation = new Uri(path).LocalPath;

            optionsBuilder.UseSqlite($"Data Source={databaseLocation}");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
