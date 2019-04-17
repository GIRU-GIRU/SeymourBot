using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;


namespace OverseerBot.Startup
{
    public class OverseerLogIn
    {
        public async Task RunBotAsync(DiscordSocketClient client)
        {
            try
            {
                //ConfigManager.GetProperty(PropertyItem.OverseerBotToken)
                await client.LoginAsync(TokenType.Bot, "NTY4MTMyNDA4Njg0MzE0NjM2.XLdonA.nruqyAT9HAbHfS2orZrB840XaZE");
                await client.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                // ExceptionManager.HandleException("0801", ex);
                throw ex;
            }

            await Task.Delay(-1);
        }
    }
}