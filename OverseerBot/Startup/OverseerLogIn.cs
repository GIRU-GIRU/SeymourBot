using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Toolbox.Config;

namespace OverseerBot.Startup
{
    public class OverseerLogIn
    {
        public async Task RunBotAsync(DiscordSocketClient client)
        {
            try
            {
                await client.LoginAsync(TokenType.Bot, ConfigManager.GetProperty(PropertyItem.OverseerBotToken));
                await client.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await Task.Delay(-1);
        }
    }
}