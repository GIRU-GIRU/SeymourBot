using Discord.Commands;
using SeymourBot.Config;
using SeymourBot.DataAccess.Storage.Filter;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Resources;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                    await context.Channel.SendMessageAsync(context.User.Mention + reason);
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.WarnEvent, context.Client.CurrentUser.Id, "AutoWarn : " + reason, context.Message.Author.Id, context.Message.Author.Username, DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)));
                    await CheckForWarnThreshold(context, await StorageManager.GetRecentWarningsAsync(context.Message.Author.Id));
                }
            }
            foreach (ModeratedElement element in bannedWords)
            {
                if (context.Message.Content.ToLower().Contains(element.Pattern))
                {
                    await context.Message.DeleteAsync();
                    string reason;
                    if (element.Dialog.Length == 0)
                    {
                        reason = BotDialogs.DefaultContainFilterMessage;
                    }
                    else
                    {
                        reason = element.Dialog;
                    }
                    await context.Channel.SendMessageAsync(context.User.Mention + reason);
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.WarnEvent, context.Client.CurrentUser.Id, "AutoWarn : " + reason, context.Message.Author.Id, context.Message.Author.Username, DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)));
                    await CheckForWarnThreshold(context, await StorageManager.GetRecentWarningsAsync(context.Message.Author.Id));
                }
            }
        }

        public static async Task AddBannedWord(ModeratedElement newBannedWord)
        {
            bannedWords.Add(newBannedWord);
            await StorageManager.AddFilterAsync(newBannedWord.Dialog, newBannedWord.Pattern, FilterTypeEnum.ContainFilter);
        }

        public static async Task CheckForWarnThreshold(SocketCommandContext context, int warnCount)
        {
            try
            {
                if (warnCount >= (ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns))) //more or equal the warn thresold
                {
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent, context.Client.CurrentUser.Id, "User has been warned " + warnCount + " times, exceeding the " + ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns) + " warn thresold", context.Message.Author.Id, context.Message.Author.Username, DateTime.UtcNow.AddMinutes(30));
                    await context.Channel.SendMessageAsync("I have had enough of your behaviour"); //todo externalize strings
                }
                else if (warnCount > (ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns) / 2)) //more than half the warn thresold
                {
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent, context.Client.CurrentUser.Id, "User has been warned " + warnCount + " times, exceeding half of the " + ConfigManager.GetIntegerProperty(PropertyItem.MaxWarns) + " warn thresold", context.Message.Author.Id, context.Message.Author.Username, DateTime.UtcNow.AddDays(1));
                    await context.Channel.SendMessageAsync("Your foolish behaviour shant go unpunished !"); //todo externalize strings
                }
            }
            catch (Exception ex)
            {
                throw ex;//todo
            }
        }
    }
}
