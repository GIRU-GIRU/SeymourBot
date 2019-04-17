using Microsoft.EntityFrameworkCore;
using SeymourBot.DataAccess.Storage.Filter.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Resources;

namespace SeymourBot.DataAccess.Storage.Filter
{
    class FilterContext : DbContext
    {
        public DbSet<FilterTable> filterTables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=F:\\SeymourV2\\SeymourBot\\SeymourBot\\Database\\SeymourFilterDB.db");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
