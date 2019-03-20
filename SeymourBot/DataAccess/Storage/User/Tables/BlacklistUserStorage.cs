using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SeymourBot.Storage.User.Tables
{
    class BlacklistUserStorage
    {
        [Key]
        public ulong UserID { get; set; }
        public string Username { get; set; }
        public ulong ModeratorID { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime DateToRemove { get; set; }
    }
}
