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

        public static async Task FilterMessage(SocketCommandContext context)
        {
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
                if (context.Message.Content.ToLower().Contains(element.Pattern.Trim()))
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
                    await DiscordContextOverseer.LogModerationAction(target.Id, "Muted");
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent,
                                          target.Id,
                                          $"User has been warned {warnCount} times, exceeding the {ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns)} warn threshold",
                                          context.Message.Author.Id,
                                          context.Message.Author.Username,
                                          DateTime.UtcNow.AddMinutes(1)); //todo change back to 30 minutes (maybe add to config)

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
                    await DiscordContextOverseer.LogModerationAction(target.Id, "Muted");
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent,
                                          target.Id,
                                          $"User has been warned {warnCount} times, exceeding half of the {ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns)} warn threshold",
                                          context.Message.Author.Id,
                                          context.Message.Author.Username,
                                          DateTime.UtcNow.AddDays(1));


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
                throw ex;//todo
            }
        }
    }
}
