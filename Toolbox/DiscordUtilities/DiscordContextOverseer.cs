﻿using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace Toolbox.DiscordUtilities
{
    /// <summary>
    /// Unify discord api interactions
    /// </summary>
    public static class DiscordContextOverseer
    {
        private static DiscordSocketClient _client;
        private static SocketGuild MordhauGuild;
        private static IUserMessage NoobGateWelcomeMessage;

        public static void InitContext(DiscordSocketClient client)
        {
            _client = client;
            MordhauGuild = _client.GetGuild(ConfigManager.GetUlongProperty(PropertyItem.Guild_Mordhau));
        }

        public static void ReAssignNoobGateWelcome(IUserMessage noobGateWelcomeMessage)
        {
            NoobGateWelcomeMessage = noobGateWelcomeMessage;
        }

        public static IUserMessage GetNoobGateWelcome()
        {
            return NoobGateWelcomeMessage;
        }

        public static SocketGuild GetGuild()
        {
            return MordhauGuild;
        }

        public static async Task LogModerationAction(ulong userId, string moderationAction, string reason, string duration)
        {
            await LogModerationAction(userId, moderationAction, true, 0, reason, duration);
        }

        public static async Task LogModerationAction(ulong userId, string moderationAction, ulong moderatorId, string reason, string duration)
        {
            await LogModerationAction(userId, moderationAction, false, moderatorId, reason, duration);
        }

        private static async Task LogModerationAction(ulong userId, string moderationAction, bool automated, ulong moderatorId, string reason, string duration)
        {
            try
            {
                var embed = new EmbedBuilder();
                var user = MordhauGuild.GetUser(userId);
                var moderator = MordhauGuild.GetUser(moderatorId);
                if (user != null && moderator != null)
                {
                    embed.WithTitle($"User {userId} was {moderationAction} at {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}"); //usage : user unmuted / user banned / user warned
                    embed.WithColor(automated ? new Color(0, 51, 204) : new Color(51, 204, 51));
                    embed.WithDescription($"User {user.Username}#{user.Discriminator} was {moderationAction} {(string.IsNullOrEmpty(duration) ? "" : "for " + duration)} {(automated ? "automatically" : $"by {moderator.Username}")} {(string.IsNullOrEmpty(reason) ? "" : "for " + reason) }");
                }
                else
                {
                    if (moderator != null)
                    {
                        embed.WithTitle($"User {userId} was {moderationAction} at {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                        embed.WithColor(automated ? new Color(0, 51, 204) : new Color(51, 204, 51));
                        embed.WithDescription($"User {userId} was {moderationAction} {(string.IsNullOrEmpty(duration) ? "" : "for " + duration)} {(automated ? "automatically" : $"by {moderator.Username}")} {(string.IsNullOrEmpty(reason) ? "" : "for " + reason) }");
                    }
                    else
                    {
                        embed.WithTitle($"User {userId} was {moderationAction} at {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                        embed.WithColor(automated ? new Color(0, 51, 204) : new Color(51, 204, 51));
                        embed.WithDescription($"User {userId} was {moderationAction} {(string.IsNullOrEmpty(duration) ? "" : "for " + duration)} {(automated ? "automatically" : $"by {moderatorId}")} {(string.IsNullOrEmpty(reason) ? "" : "for " + reason) }");
                    }
                }
                await GetModerationLog().SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(DiscordContextOverseer).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
            }
        }

        public static async Task RemoveRoleAsync(ulong userId, ulong roleId)
        {
            try
            {
                var user = MordhauGuild.GetUser(userId);
                var roleToRemove = MordhauGuild.GetRole(roleId);

                if (user != null && user.Roles.Any(x => x.Id == roleId))
                {
                    await user.RemoveRoleAsync(roleToRemove);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(DiscordContextOverseer).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
            }
        }

        public static ITextChannel GetChannel(ulong id)
        {
            try
            {
                return MordhauGuild.GetChannel(id) as ITextChannel;
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(DiscordContextOverseer).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
                throw;
            }
        }

        public static IRole GrabRole(MordhauRoleEnum role)
        {
            try
            {
                return MordhauGuild.GetRole(ConfigManager.GetUlongProperty(Property.FromMordhauRole(role)));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.DiscordContextException, ex, "Probably role sync issue");
                throw;
            }
        }

        public static ITextChannel GetDeletedMessageLog()
        {
            try
            {
                return MordhauGuild.GetTextChannel(ConfigManager.GetUlongProperty(PropertyItem.Channel_DeletedMessageLog));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(DiscordContextOverseer).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
                throw;
            }
        }

        public static ITextChannel GetModerationLog()
        {
            try
            {
                return MordhauGuild.GetTextChannel(ConfigManager.GetUlongProperty(PropertyItem.Channel_ModerationLog));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(DiscordContextOverseer).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
                throw;
            }
        }

        public static ITextChannel GetNoobGateChannel()
        {
            try
            {
                return MordhauGuild.GetTextChannel(ConfigManager.GetUlongProperty(PropertyItem.Channel_NoobGate));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(DiscordContextOverseer).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
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
                ExceptionManager.HandleException(ErrMessages.DiscordContextException, ex);
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
                ExceptionManager.HandleException(ErrMessages.DiscordContextException, ex);
                throw;
            }
        }

        public static async Task AddRole(IRole role, ulong userId)
        {
            try
            {
                var user = MordhauGuild.GetUser(userId);
                await user.AddRoleAsync(role);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(DiscordContextOverseer).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
            }

        }

        public static Emote GetEmote(string emoteName)
        {
            return MordhauGuild.Emotes.Where(x => x.Name.ToLower() == emoteName.ToLower()).FirstOrDefault();
        }

        public static void AddRole(object discordRoleEnum)
        {
            throw new NotImplementedException();
        }

        public static Emote GetEmoteAyySeymour()
        {
            return MordhauGuild.Emotes.Where(x => x.Name.ToLower() == "ayyseymour").FirstOrDefault(); // todo
        }

        public static Emote GetEmoteReee()
        {
            return MordhauGuild.Emotes.Where(x => x.Name.ToLower() == "reee").FirstOrDefault(); // todo
        }

        public static Emote GetEmotePommel()
        {
            return MordhauGuild.Emotes.Where(x => x.Name.ToLower() == "pommel").FirstOrDefault(); // todo
        }


        /// <summary>
        /// Unified logging method
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task LogErrorAsync(string message)
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
            await GetLoggingChannel().SendMessageAsync($" \"{command}\" failed: ```{message}```");
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
