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
    public class CustomCommands : ModuleBase<SocketCommandContext>
    {
        [Command("addcommand")]
        [Alias("addcmd")]
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
                ExceptionManager.HandleException(ErrMessages.CustomCommand, ex);
            }
        }

        [Command("deletecommand")]
        [Alias("delcommand", "delcmd")]
        [DevOrAdmin]
        private async Task DeleteInfoCommand([Remainder]string UserInput)
        {
            try
            {
                bool existing = await StorageManager.DeleteInfoCommandAsync(UserInput.ToLower());

                if (existing)
                {
                    await Context.Channel.SendMessageAsync($"Deleted \"{UserInput}\" {DiscordContextSeymour.GetEmoteAyySeymour()}");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Unable to find \"{UserInput}\" {DiscordContextSeymour.GetEmoteAyySeymour()}");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.CustomCommand, ex);
            }
        }

        [Command("commandlist")]
        [Alias("cmdlist")]
        [DevOrAdmin]
        private async Task ListInfoCommandsTest()
        {
            try
            {
                List<string> commands = StorageManager.GetInfoCommands();
                var embed = new EmbedBuilder();
                embed.WithTitle("Avaliable Information Commands");

                embed.AddField("Calling names", string.Join("\n", commands), false);

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.CustomCommand, ex);
            }

        }

        [Command("c")]
        [Alias("command")]
        [DevOrAdmin]
        [Priority(1)]
        private async Task PostInfoCommandDevOrAdminAsync(string cmdName)
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
                    if (!string.IsNullOrEmpty(commandContent))
                    {
                        await Context.Channel.SendMessageAsync(commandContent);
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Unable to find that command");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.CustomCommand, ex);
            }
        }


        [Command("c")]
        [Alias("command")]
        [Ratelimit(5, 10, Measure.Minutes)]
        private async Task PostInfoCommandAsync(string cmdName)
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
                    if (!string.IsNullOrEmpty(commandContent))
                    {
                        await Context.Channel.SendMessageAsync(commandContent);
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Unable to find that command");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.CustomCommand, ex);
            }
        }
    }
}
