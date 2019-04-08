using Microsoft.EntityFrameworkCore;
using SeymourBot.DataAccess.Storage.Filter.Tables;
using SeymourBot.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.DataAccess.Storage.Filter
{
    class FilterContext : DbContext
    {
        public DbSet<FilterTable> filterTables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DBPaths.FilterDB}");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
