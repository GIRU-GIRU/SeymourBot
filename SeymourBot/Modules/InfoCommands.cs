using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using SeymourBot.Attributes;
using System.Linq;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Exceptions;

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
                ExceptionManager.HandleException("", ex);
            }

        }

        [Command("c")]
        [Alias("i", "info")]
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
