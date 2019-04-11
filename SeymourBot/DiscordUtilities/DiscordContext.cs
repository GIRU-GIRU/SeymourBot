using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Config;
using SeymourBot.Exceptions;
using SeymourBot.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SeymourBot.DiscordUtilities
{
    /// <summary>
    /// Unify discord api interactions
    /// </summary>
    class DiscordContext
    {
        private static DiscordSocketClient _client;
        public static SocketGuild MordhauGuild;

        public static void InitContext(DiscordSocketClient client)
        {
            _client = client;
            MordhauGuild = _client.GetGuild(ConfigManager.GetUlongProperty(PropertyItem.Guild_Mordhau));
        }


        public static async Task RemoveRoleAsync(ulong userId, ulong roleId)
        {
            await MordhauGuild.GetUser(userId).RemoveRoleAsync(MordhauGuild.GetRole(roleId));
        }

        public static IRole GrabRole(MordhauRoleEnum role)
        {
            try
            {
                return MordhauGuild.GetRole(ConfigManager.GetUlongProperty(Property.FromMordhauRole(role)));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0701", ex, "Probably role sync issue");
                throw;
            }
        }

       public static async Task<bool> IsUserDevOrAdmin(ulong userId)
       {
           try
           {
                var userRoles = MordhauGuild.Users.Where(x => x.Id == userId).FirstOrDefault().Roles;

                if (userRoles.Any(x => x.Name.ToLower() == MordhauRoleEnum.Developer.ToString().ToLower() ||
                userRoles.Any(y => y.Name.ToLower() == MordhauRoleEnum.Moderator.ToString().ToLower())))
                {
                    return true;
                }

                return false;
           }
           catch (Exception ex)
           {
               ExceptionManager.HandleException("", ex, ""); // todo
               throw;
           }
       }

        public static async Task<bool> IsUserDevOrAdmin(SocketGuildUser user)
        {
            try
            {

                if (user.Roles.Any(x => x.Name.ToLower() == MordhauRoleEnum.Developer.ToString().ToLower() ||
                user.Roles.Any(y => y.Name.ToLower() == MordhauRoleEnum.Moderator.ToString().ToLower())))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex, ""); // todo
                throw;
            }
        }

        public static async Task AddRole(IRole role, ulong userId)
        {
            await MordhauGuild.GetUser(userId).AddRoleAsync(role);
        }

        public static Emote GetEmote(string emoteName)
        {
            return MordhauGuild.Emotes.Where(x => x.Name.ToLower() == emoteName.ToLower()).FirstOrDefault();
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
