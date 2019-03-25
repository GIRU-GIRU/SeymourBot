using Discord;
using Discord.WebSocket;
using SeymourBot.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.DiscordUtilities
{
    class DiscordContext
    {
        private static DiscordSocketClient _client;

        public static void InitContext(DiscordSocketClient client)
        {
            _client = client;
        }

        public static async Task BotReadyEvent()

        {
            var guild = GetGuild();
            var chnl = _client.GetChannel(ConfigManager.GetUlongUserSetting(PropertyItem.Channel_Main)) as ITextChannel;
            await chnl.SendMessageAsync("Test startup message");
        }

        public static async Task RemoveRole(ulong userId, ulong roleId)
        {
            await GetGuild().GetUser(userId).RemoveRoleAsync(GetGuild().GetRole(roleId));
        }

        private static SocketGuild GetGuild()
        {
            return _client.GetGuild(ConfigManager.GetUlongUserSetting(PropertyItem.Guild_Mordhau));
        }
    }
}
