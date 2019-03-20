using SeymourBot.Storage.User;
using SeymourBot.Storage.User.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SeymourBot.Storage
{
    class UserRoleStorage
    {
        [Key]
        public ulong UserID { get; set; }
        public DateTime DateUpdated { get; set; }
        public ICollection<GuildRoleStorage> RoleIDTable { get; set; }
    }
}
