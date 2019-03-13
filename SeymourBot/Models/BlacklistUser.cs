using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.Models
{
    class BlacklistUser
    {
        public static List<SocketUser> BlackListedUser { get; } = new List<SocketUser>();
    }
}
