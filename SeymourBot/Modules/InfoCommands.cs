using Discord;
using Discord.Commands;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace SeymourBot.Modules
{
    public class InfoCommands : ModuleBase<SocketCommandContext>
    {

        [Command("InfoNew")]
        private async Task StoreInfoCommandTest([Remainder]string UserInput)
        {
            try
            {
                Command command = new Command(UserInput);
                if (!command.Error)
                {
                    await StorageManager.StoreInfoCommandAsync(command);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        [Command("InfoList")]
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
        private async Task<RuntimeResult> PostInfoCommandTest(string cmdName)
        {
            try
            {
                Command command = new Command(cmdName);
                if (!command.Error)
                {
                    string commandContent = await StorageManager.GetInfoCommandAsync(command);
                    await Context.Channel.SendMessageAsync(commandContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                return new CommandErrorResult(CommandError.Unsuccessful, ex.Message);
            }
        }
    }
}
