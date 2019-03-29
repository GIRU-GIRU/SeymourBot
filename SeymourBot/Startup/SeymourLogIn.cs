using Discord;
using Discord.WebSocket;
using SeymourBot.Config;
using SeymourBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Startup
{
    class SeymourLogIn
    {
        public async Task RunBotAsync(DiscordSocketClient client)
        {
            try
            {
                await client.LoginAsync(TokenType.Bot, ConfigManager.GetProperty(PropertyItem.BotToken));
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
