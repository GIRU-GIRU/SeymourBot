using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SeymourBot.Storage.User.Tables
{
    class GuildRoleStorage
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
