using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.Modules.CommandUtils;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Cleanse : ModuleBase<SocketCommandContext>
    {
        [Command("cleanse")]
        [Alias("purge", "prune")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [DevOrAdmin]
        private async Task CleanChatAmount(int amount)
        {
            try
            {
                if (amount > 0)
                {
                    var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
                    var chnl = Context.Channel as ITextChannel;
                 
                    await chnl.DeleteMessagesAsync(messages);

                    var embed = new EmbedBuilder();
                    embed.WithTitle($"Cleansed {amount} messages {DiscordContextSeymour.GetEmoteAyySeymour().ToString()}");
                    embed.WithColor(new Color(0, 255, 0));

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            catch (System.Exception ex)
            {
                ExceptionManager.HandleException("", ex);
            }
        }

        [Command("cleanse")]
        [Alias("purge", "prune")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [DevOrAdmin]
        private async Task CleanChat()
        {
            try
            {
                int amount = 2;

                var messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
                var chnl = Context.Channel as ITextChannel;
                await chnl.DeleteMessagesAsync(messages);

                await Context.Channel.SendMessageAsync($"{DiscordContextSeymour.GetEmoteAyySeymour().ToString()}");
            }
            catch (System.Exception ex)
            {
                ExceptionManager.HandleException("", ex);
            }
        }

        [Command("cleanse")]
        [Alias("purge", "prune")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [DevOrAdmin]
        private async Task CleanChatUser(SocketGuildUser user)
        {
            try
            {
                var usersocket = user as SocketGuildUser;
                int amount = 300;
                var msgsCollection = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
                var result = from m in msgsCollection
                             where m.Author == user
                             select m.Id;

                var chnl = Context.Channel as ITextChannel;
                int amountOfMessages = result.Count();
                await chnl.DeleteMessagesAsync(result);

                var embed = new EmbedBuilder();
                embed.WithTitle($"Cleansed {amountOfMessages} messages from {user.Username} {DiscordContextSeymour.GetEmoteAyySeymour().ToString()}");
                embed.WithColor(new Color(0, 255, 0));

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (System.Exception ex)
            {
                ExceptionManager.HandleException("", ex);
            }
        }

        [Command("cleanse")]
        [Alias("purge", "prune")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [DevOrAdmin]
        private async Task CleanChatUserAmount(SocketGuildUser user, int amountToDelete)
        {
            try
            {
                if (amountToDelete > 0)
                {
                    var usersocket = user as SocketGuildUser;
                    int amount = 900;
                    var msgsCollection = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
                    var result = from m in msgsCollection
                                 where m.Author == user
                                 select m.Id;

                    var chnl = Context.Channel as ITextChannel;

                    var totalToDelete = result.Take(amountToDelete);
                    await chnl.DeleteMessagesAsync(totalToDelete);
                    var embed = new EmbedBuilder();
                    embed.WithTitle($"Cleansed {totalToDelete.Count()} messages from {user.Username} {DiscordContextSeymour.GetEmoteAyySeymour().ToString()}");
                    embed.WithColor(new Color(0, 255, 0));

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
            catch (System.Exception ex)
            {
                ExceptionManager.HandleException("", ex);
            }
        }
    }
}
