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
            client = _client;
        }

        internal Func<Task> BotReadyEvent()
        {

            ulong mordhauGuildID = ConfigManager.GetUlongProperty(PropertyItem.MordhauGuild);

            return null;
            // the poo
        }
    }
}
