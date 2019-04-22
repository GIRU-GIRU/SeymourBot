using Microsoft.EntityFrameworkCore;
using SeymourBot.Storage.User.Tables;
using System;
using System.IO;
using System.Reflection;
using Toolbox.Resources;

namespace SeymourBot.Storage.User
{
    class UserContext : DbContext
    {
        public DbSet<BlacklistUserStorage> BlackListedTable { get; set; }
        public DbSet<UserDisciplinaryEventArchive> UserDisciplinaryEventArchiveTable { get; set; }
        public DbSet<UserDisciplinaryEventStorage> UserDisciplinaryEventStorageTable { get; set; }
        public DbSet<UserDisciplinaryPermanentStorage> UserDisciplinaryPermanentStorageTable { get; set; }
        public DbSet<UserRoleStorage> UserRoleStorageTable { get; set; }

        public DbSet<UserStorage> UserStorageTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\Database\\SeymourUserDB.db";
            string databaseLocation = new Uri(path).LocalPath;

            optionsBuilder.UseSqlite($"Data Source={databaseLocation}");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
