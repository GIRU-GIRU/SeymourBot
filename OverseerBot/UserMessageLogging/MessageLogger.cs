using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace OverseerBot.UserMessageLogging
{
    public static class MessageLogger
    {
        public static async Task EditedMessageEvent(Cacheable<IMessage, ulong> msgBefore, SocketMessage msgAfter, ISocketMessageChannel channel)
        {
            if (string.IsNullOrEmpty(msgAfter.Content)) return;
            if (msgBefore.Value.Content == msgAfter.Content) return;

            try
            {
                var textChannel = channel as ITextChannel;
                var logChannel = await textChannel.Guild.GetChannelAsync(558072353733738499) as ITextChannel;
                var message = await msgBefore.GetOrDownloadAsync();

                string time = msgAfter.Timestamp.DateTime.ToLongDateString() + " " + msgAfter.Timestamp.DateTime.ToLongTimeString();

                var embed = new EmbedBuilder();
                embed.WithTitle($"✍️ {msgAfter.Author.Username}#{msgAfter.Author.Discriminator} edited messageID {msgBefore.Id} at {time}");
                embed.WithDescription($"in #{channel.Name}, Original: " + msgBefore.Value.Content);

                embed.WithColor(new Color(250, 255, 0));
                await logChannel.SendMessageAsync("", false, embed.Build());

                return;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static async Task DeletedMessageEvent(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            try
            {
                var textChannel = channel as ITextChannel;
                var logChannel = await textChannel.Guild.GetChannelAsync(558072353733738499) as ITextChannel;
                var message = await msg.GetOrDownloadAsync();

                var embed = new EmbedBuilder();
                embed.WithTitle($"🗑 {message.Author.Username}#{message.Author.Discriminator} deleted message at in {message.Channel.Name}. UserID = {message.Author.Id}");
                embed.WithDescription(message.Content);

                embed.WithColor(new Color(255, 0, 0));
                await logChannel.SendMessageAsync("", false, embed.Build());
                return;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
    }
}
