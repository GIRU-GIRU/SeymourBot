﻿
using Microsoft.EntityFrameworkCore;
using SeymourBot.Config;
using SeymourBot.Resources;
using SeymourBot.Storage.User.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            optionsBuilder.UseSqlite($"Data Source={DBPaths.UserDB}");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
