using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.DiscordUtilities;

namespace SeymourBot.AutoModeration
{
    public static class MessageContentChecker
    {
        internal static async Task AutoModerateMessage(SocketCommandContext context)
        {
            if (!await DiscordContext.IsUserDevOrAdmin(context.Message.Author as SocketGuildUser))
            {
                _ = Task.Run(() => MessageContainsAsync(context));
            }          
        }

        internal static async Task MessageContainsAsync(SocketCommandContext context)
        {
            await AutoModeratorManager.FilterMessage(context);
        }
    }
}
