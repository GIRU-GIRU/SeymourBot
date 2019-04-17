using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.Exceptions;

namespace SeymourBot.Startup
{
    class SeymourLogIn
    {
        public async Task RunBotAsync(DiscordSocketClient client)
        {
            try
            {
                await client.LoginAsync(TokenType.Bot, ConfigManager.GetProperty(PropertyItem.SeymourBotToken));
                await client.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0801", ex);
            }

            await Task.Delay(-1);
        }
    }
}
