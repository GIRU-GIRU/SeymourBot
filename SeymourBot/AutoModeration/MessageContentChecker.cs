using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;

namespace SeymourBot.AutoModeration
{
    public static class MessageContentChecker
    {
        internal static async Task AutoModerateMessage(SocketCommandContext context)
        {
            if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(context.Message.Author as SocketGuildUser))
            {
                _ = Task.Run(() => MessageContainsAsync(context));
            }
        }

        internal static async Task MessageContainsAsync(SocketCommandContext context)
        {
            await AutoModeratorManager.FilterMessage(context);
            await AutoModeratorManager.MassMentionCheck(context);
            if (await DiscordContextSeymour.IsUserRestrictedAsync(context.Message.Author as SocketGuildUser))
            {
                await AutoModeratorManager.EnforceRestricted(context);
            }
        }
    }
}
