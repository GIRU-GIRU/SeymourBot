using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Config;
using SeymourBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.DiscordUtilities
{
    /// <summary>
    /// Unify discord api interactions
    /// </summary>
    class DiscordContext
    {
        private static DiscordSocketClient _client;

        public static void InitContext(DiscordSocketClient client)
        {
            _client = client;
        }

        public static async Task BotReadyEvent()
        {
            await GetChannel().SendMessageAsync("Test startup message");
        }

        public static async Task RemoveRole(ulong userId, ulong roleId)
        {
            await GetGuild().GetUser(userId).RemoveRoleAsync(GetGuild().GetRole(roleId));
        }

        public static IRole GrabRole(MordhauRoleEnum role)
        {
            try
            {
                return GetGuild().GetRole(ConfigManager.GetUlongUserSetting(Property.FromMordhauRole(role)));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0701", ex, "Probably role sync issue");
                throw;
            }
        }

        /// <summary>
        /// Unified logging method
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task LogError(string message)
        {
            await GetChannel().SendMessageAsync($" Exception thrown : ```{message}```");
        }

        /// <summary>
        /// Unified logging method for command related errors
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="command">The associated command</param>
        /// <returns></returns>
        public static async Task LogError(string message, string command)
        {
            await GetChannel().SendMessageAsync($" \"{command}\" threw a Exception : ```{message}```");
        }

        private static SocketGuild GetGuild()
        {
            return _client.GetGuild(ConfigManager.GetUlongUserSetting(PropertyItem.Guild_Mordhau));
        }

        private static ITextChannel GetChannel()
        {
            return _client.GetChannel(ConfigManager.GetUlongUserSetting(PropertyItem.Channel_Main)) as ITextChannel;
        }

    }
}
