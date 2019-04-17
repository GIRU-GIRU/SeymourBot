using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;

namespace OverseerBot.UserMessageLogging
{
    public static class MessageLogger
    {
        public static async Task EditedMessageEvent(Cacheable<IMessage, ulong> msgBefore, SocketMessage msgAfter, ISocketMessageChannel channel)
        {
            try
            {
                if (string.IsNullOrEmpty(msgAfter.Content)) return;
                if (msgBefore.Value.Content == msgAfter.Content) return;

                var logChannel = DiscordContext.GetDeletedMessageLog();
                var message = await msgBefore.GetOrDownloadAsync();

                string time = msgAfter.Timestamp.DateTime.ToLongDateString() + " " + msgAfter.Timestamp.DateTime.ToLongTimeString();

                var embed = new EmbedBuilder();
                embed.WithTitle($"✍️ {msgAfter.Author.Username}#{msgAfter.Author.Discriminator} edited messageID {message.Id} at {time}");
                embed.WithDescription($"in #{channel.Name}, Original: " + msgBefore.Value.Content);
                embed.WithColor(new Color(250, 255, 0));

                await logChannel.SendMessageAsync("", false, embed.Build());

            }
            catch (System.Exception ex)
            {
                throw ex;//todo
            }
        }

        public static async Task DeletedMessageEvent(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            try
            {
                var logChannel = DiscordContext.GetDeletedMessageLog();
                var message = await msg.GetOrDownloadAsync();

                var embed = new EmbedBuilder();
                embed.WithTitle($"🗑 {message.Author.Username}#{message.Author.Discriminator} deleted message at in {message.Channel.Name}. UserID = {message.Author.Id}");
                embed.WithDescription(message.Content);
                embed.WithColor(new Color(255, 0, 0));

                await logChannel.SendMessageAsync("", false, embed.Build());
            }
            catch (System.Exception ex)
            {
                throw ex; //todo
            }

        }
    }
}
