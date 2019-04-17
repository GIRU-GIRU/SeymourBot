using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;

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
