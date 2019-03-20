using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SeymourBot.Storage
{
    class UserStorage
    {
        [Key]
        public ulong UserID { get; set; }
        public string UserName { get; set; }
        
    }
}
