using Discord;
using Discord.WebSocket;
using SeymourBot.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.DiscordUtilities
{
    class BotInitialization
    {
        private DiscordSocketClient _client;

        public BotInitialization(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task BotReadyEvent()

        {
            var guild = _client.GetGuild(ConfigManager.GetUlongUserSetting(PropertyItem.Guild_Mordhau));
            var chnl = _client.GetChannel(ConfigManager.GetUlongUserSetting(PropertyItem.Channel_Main)) as ITextChannel;
            await chnl.SendMessageAsync("Test startup message");
            // the poo
        }
    }
}
