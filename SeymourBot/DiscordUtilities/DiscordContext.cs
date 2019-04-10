using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Config;
using SeymourBot.Exceptions;
using SeymourBot.Resources;
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


        public static async Task RemoveRoleAsync(ulong userId, ulong roleId)
        {
            await GetGuild().GetUser(userId).RemoveRoleAsync(GetGuild().GetRole(roleId));
        }

        public static IRole GrabRole(MordhauRoleEnum role)
        {
            try
            {
                return GetGuild().GetRole(ConfigManager.GetUlongProperty(Property.FromMordhauRole(role)));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0701", ex, "Probably role sync issue");
                throw;
            }
        }

        public static async Task AddRole(IRole role, ulong userId)
        {
            await GetGuild().GetUser(userId).AddRoleAsync(role);
        }

        public static async Task GetEmoji(string emoji)
        {
            //todo
        }
        /// <summary>
        /// Unified logging method
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task LogError(string message)
        {
            await GetLoggingChannel().SendMessageAsync($" Exception thrown : ```{message}```");
        }

        /// <summary>
        /// Unified logging method for command related errors
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="command">The associated command</param>
        /// <returns></returns>
        public static async Task LogErrorAsync(string message, string command)
        {
            await GetLoggingChannel().SendMessageAsync($" \"{command}\" threw a Exception : ```{message}```");
        }

        public static SocketGuild GetGuild()
        {
            return _client.GetGuild(ConfigManager.GetUlongProperty(PropertyItem.Guild_Mordhau));
        }

        public static ITextChannel GetMainChannel()
        {
            return _client.GetChannel(ConfigManager.GetUlongProperty(PropertyItem.Channel_Main)) as ITextChannel;
        }

        public static ITextChannel GetLoggingChannel()
        {
            return _client.GetChannel(ConfigManager.GetUlongProperty(PropertyItem.Channel_Logging)) as ITextChannel;
        }

    }
}
