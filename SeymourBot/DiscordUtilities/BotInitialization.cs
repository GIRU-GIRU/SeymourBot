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

            //ulong mordhauGuildID = ConfigManager.GetUlongProperty(PropertyItem.MordhauGuild);

            var guild = _client.GetGuild(390097689750011906);
            var chnl = _client.GetChannel(443742330303021066) as ITextChannel;

            await chnl.SendMessageAsync("test");
            // the poo
        }
    }
}
