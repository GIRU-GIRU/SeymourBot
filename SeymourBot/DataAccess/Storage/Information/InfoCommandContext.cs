using Microsoft.EntityFrameworkCore;
using SeymourBot.Storage.Information;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.DataAccess.Storage.Information
{
    class InfoCommandContext : DbContext
    {
        public DbSet<InfoCommandTable> InfoCommandTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlite("Data Source=SeymourInfoDB.db");

            string dbPath = @"F:\SeymourV2\SeymourBot\SeymourBot\Database\";
            optionsBuilder.UseSqlite($"Data Source={dbPath}SeymourInfoDB.db");

            
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
