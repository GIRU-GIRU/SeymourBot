using Discord.Commands;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Modules
{
    class DisciplinaryCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Mute")]
        private async Task TempMuteCommandTest([Remainder]string UserInput)
        {
            try
            {
                Command command = new Command(UserInput);
                if (!command.Error && command.CommandParameters.Length >= 2)
                {

                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
