using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace SeymourBot.Modules
{
    public class InfoCommands : ModuleBase<SocketCommandContext>
    {

        [Command("addinfo")]
        [DevOrAdmin]
        private async Task StoreInfoCommandTest([Remainder]string UserInput)
        {
            try
            {
                bool existing = false;
                Command command = new Command(UserInput);
                if (!command.Error)
                {
                    existing = await StorageManager.StoreInfoCommandAsync(command);
                }

                if (existing)
                {
                    await Context.Channel.SendMessageAsync($"Updated {command.CommandName} {DiscordContextSeymour.GetEmoteAyySeymour()}");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Added {command.CommandName} {DiscordContextSeymour.GetEmoteAyySeymour()}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Command("infolist")]
        [DevOrAdmin]
        private async Task ListInfoCommandsTest()
        {
            try
            {
                List<string> commands = StorageManager.GetInfoCommands();
                var embed = new EmbedBuilder();
                embed.WithTitle("Avaliable Information Commands");
                foreach (var element in commands)
                {
                    embed.AddField("!info", element, true);
                }
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.InfoCommandException, ex);
            }

        }

        [Command("c")]
        [Alias("i", "info")]
        [Ratelimit(5, 10, Measure.Minutes)]
        private async Task PostInfoCommand(string cmdName)
        {
            try
            {
                if (await StorageManager.UserCheckAndUpdateBlacklist(Context.Message.Author as SocketGuildUser))
                {
                    return;
                }

                Command command = new Command(cmdName);
                if (!command.Error)
                {
                    string commandContent = await StorageManager.GetInfoCommandAsync(command);
                    await Context.Channel.SendMessageAsync(commandContent);
                }
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
