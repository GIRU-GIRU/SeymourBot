using Discord;
using Discord.Commands;
using SeymourBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Modules
{
    public class UtilCommands
    {
        [Command("SetProperty")]
        private async Task ListInfoCommandsTest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex);
            }

        }
    }
}
