using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.DataAccess.Storage.Filter;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;
using Toolbox.Utils;

namespace SeymourBot.AutoModeration
{
    public static class AutoModeratorManager
    {
        public static List<ModeratedElement> bannedWords;
        public static List<ModeratedElement> bannedRegex;

        static AutoModeratorManager()
        {
            bannedWords = StorageManager.GetModeratedWords();
            bannedRegex = StorageManager.GetModeratedRegex();
        }

        public static async Task MassMentionCheck(SocketCommandContext context)
        {
            try
            {
                if (context.Message.MentionedUsers.Count > 8)
                {
                    SocketUser target = context.Message.Author;
                    await context.Message.DeleteAsync();
                    await DiscordContextSeymour.AddRole(DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted), target.Id);
                    await DiscordContextOverseer.LogModerationAction(target.Id, "Muted", "excessive pinging", Utilities.ShortTimeSpanFormatting(new TimeSpan(3, 0, 0, 0)));
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent,
                                          context.Client.CurrentUser.Id,
                                          "excessive pinging",
                                          target.Id,
                                          target.Username,
                                          (DateTimeOffset.UtcNow + new TimeSpan(0, 30, 0)).DateTime);
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.WarnEvent, context.Client.CurrentUser.Id, "AutoWarn : excessive pinging", target.Id, target.Username, DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)));
                    if (context.Channel != null)
                    {
                        await DiscordContextOverseer.GetChannel(context.Channel.Id).SendMessageAsync($"{context.Message.Author.Mention}, Thou shall not say thy noble's names in vain. {DiscordContextSeymour.GetEmoteAyySeymour()}");

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(AutoModeratorManager).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
            }
        }

        public static async Task EnforceRestricted(SocketCommandContext context)
        {
            try
            {
                if (new Regex(@"(http(s)?:\/\/)?([a-z][-\w]+(?:\.\w+)+(?:\S+)?)", RegexOptions.IgnoreCase | RegexOptions.Compiled).IsMatch(context.Message.Content))
                {
                    await context.Message.DeleteAsync();
                    await DiscordContextOverseer.LogModerationAction(context.Message.Author.Id, "Blocked from posting a link", $"posting links while restricted in message id : {context.Message.Id}", "");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(AutoModeratorManager).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
            }
        }

        public static async Task FilterMessage(SocketCommandContext context)
        {
            var splitContent = context.Message.Content.ToLower().Split();
            if (bannedRegex.Count > 0)
            {
                foreach (ModeratedElement element in bannedRegex)
                {
                    if (new Regex(element.Pattern).Match(context.Message.Content).Success)
                    {
                        await context.Message.DeleteAsync();
                        string reason;
                        if (element.Dialog.Length == 0)
                        {
                            reason = BotDialogs.DefaultRegexFilterMessage;
                        }
                        else
                        {
                            reason = element.Dialog;
                        }
                        await context.Channel.SendMessageAsync(context.User.Mention + ", " + reason);
                        await TimedEventManager.CreateEvent(DisciplinaryEventEnum.WarnEvent, context.Client.CurrentUser.Id, "AutoWarn : " + reason, context.Message.Author.Id, context.Message.Author.Username, DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)));
                        await CheckForWarnThreshold(context.Message.Author as SocketGuildUser, context, await StorageManager.GetRecentWarningsAsync(context.Message.Author.Id));
                        break;
                    }
                }
            }
            foreach (ModeratedElement element in bannedWords)
            {
                if (splitContent.Contains(element.Pattern.Trim()))
                {
                    string reason;
                    if (element.Dialog.Length == 0)
                    {
                        reason = "Do not use such disgusting language in my presence";
                    }
                    else
                    {
                        reason = element.Dialog;
                    }
                    await context.Channel.SendMessageAsync(context.User.Mention + ", " + reason);
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.WarnEvent, context.Client.CurrentUser.Id, "AutoWarn : " + reason, context.Message.Author.Id, context.Message.Author.Username, DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)));
                    await CheckForWarnThreshold(context.Message.Author as SocketGuildUser, context, await StorageManager.GetRecentWarningsAsync(context.Message.Author.Id));


                    try
                    {
                        await context.Message.DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        await ExceptionManager.LogExceptionAsync(ex.Message);
                    }
                    break;
                }
            }
        }

        public static async Task<bool> AddBannedWordAsync(ModeratedElement newBannedWord)
        {
            if (!bannedWords.Any(x => x.Pattern.ToLower().Trim() == newBannedWord.Pattern.ToLower().Trim()))
            {
                bannedWords.Add(newBannedWord);
                await StorageManager.AddFilterAsync(newBannedWord.Dialog, newBannedWord.Pattern, FilterTypeEnum.ContainFilter);
                return true;
            }
            return false;
        }

        public static async Task<bool> RemoveBannedWordAsync(string name)
        {
            var itemToRemove = bannedWords.FirstOrDefault(x => x.Pattern.ToLower().Trim() == name.ToLower().Trim());
            if (itemToRemove != null)
            {
                bannedWords.Remove(itemToRemove);
                await StorageManager.RemoveFilterAsync(name, FilterTypeEnum.ContainFilter);
                return true;
            }
            return false;
        }


        public static async Task CheckForWarnThreshold(SocketGuildUser target, SocketCommandContext context, int warnCount, ITextChannel chnl = null)
        {
            try
            {
                if (warnCount >= (ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns))) //more or equal the warn thresold
                {
                    await DiscordContextSeymour.AddRole(DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted), target.Id);
                    await DiscordContextOverseer.LogModerationAction(target.Id, "Muted", $"User has been warned {warnCount} times, exceeding the {ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns)} warn threshold", Utilities.ShortTimeSpanFormatting(new TimeSpan(1, 0, 0, 0)));
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent,
                                          context.Client.CurrentUser.Id,
                                          $"User has been warned {warnCount} times, exceeding the {ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns)} warn threshold",
                                          target.Id,
                                          target.Username,
                                          (DateTimeOffset.UtcNow + new TimeSpan(1, 0, 0, 0)).DateTime);

                    if (chnl == null)
                    {
                        await DiscordContextOverseer.GetChannel(context.Channel.Id).SendMessageAsync($"Silence. {target.Mention}");
                    }
                    else
                    {
                        await DiscordContextOverseer.GetChannel(chnl.Id).SendMessageAsync($"Silence. {target.Mention}");
                    }

                }
                else if (warnCount > (ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns) / 2)) //more than half the warn thresold
                {
                    await DiscordContextSeymour.AddRole(DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted), target.Id);
                    await DiscordContextOverseer.LogModerationAction(target.Id, "Muted", $"User has been warned {warnCount} times, exceeding half of the {ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns)} warn threshold", Utilities.ShortTimeSpanFormatting(new TimeSpan(0, 30, 0)));
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent,
                                          context.Client.CurrentUser.Id,
                                          $"User has been warned {warnCount} times, exceeding half of the {ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns)} warn threshold",
                                          target.Id,
                                          target.Username,
                                          (DateTimeOffset.UtcNow + new TimeSpan(0, 30, 0)).DateTime);


                    if (chnl == null) //channel specified check
                    {
                        await DiscordContextOverseer.GetChannel(context.Channel.Id).SendMessageAsync($"{target.Mention}, enough.");
                    }
                    else
                    {
                        await DiscordContextOverseer.GetChannel(chnl.Id).SendMessageAsync($"{target.Mention}, enough.");
                    }


                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.CheckForWarnThresholdException, ex);
            }
        }
    }
}
