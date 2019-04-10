using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace SeymourBot.AutoModeration
{
    public static class MessageContentChecker
    {
        private static Regex _regexInviteLinkDiscord = new Regex(@"(https?:\/\/)?(www\.)?(discord\.(gg|io|me|li)|discordapp\.com\/invite)\/.+[a-z]");

        internal static async Task MessageContainsAsync(SocketCommandContext context)
        {
            await AutoModeratorManager.FilterMessage(context);
        }
    }
}
