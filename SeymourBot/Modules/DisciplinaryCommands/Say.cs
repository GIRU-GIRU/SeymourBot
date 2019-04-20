using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.User.Tables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Utils;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Say : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [DevOrAdmin]
        public async Task SeymourSayCustomMessage([Remainder]string input)
        {
            try
            {
                if (Context.Message.MentionedChannels.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("You must mention the channel: (#channel_name)");
                    return;
                }

                var targetChannel = Context.Message.MentionedChannels.FirstOrDefault() as ITextChannel;
                var targetChannelAsString = $"<#{targetChannel.Id}>";
                var sanitizedInput = input.Replace(targetChannelAsString, string.Empty);

                await targetChannel.SendMessageAsync(sanitizedInput);
            }
            catch (Exception ex)
            {

                throw ex; // todo
            }
        }
    }
}