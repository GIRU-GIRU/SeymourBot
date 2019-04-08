using Discord.Commands;
using SeymourBot.Config;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Resources;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Text;
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

        public static async Task DeleteInviteLinkWarn(SocketCommandContext context)
        {
            await context.Message.DeleteAsync();
            await context.Channel.SendMessageAsync($"{context.User.Mention}, do not post invite links");

            //todo warn event
        }

        public static async Task DeleteBannedWordWarn(SocketCommandContext context)
        {
            await context.Message.DeleteAsync();
            await context.Channel.SendMessageAsync($"{context.User.Mention}, do not use such foul language in my presence");

            //todo warn event
        }

        public static async Task<int> CheckForWarnThreshold(SocketCommandContext context, int warnCount)
        {
            try
            {
                if (warnCount > 2)//todo
                {
                 // TimedEventManager.CreateEvent()
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static async Task WarnHelper(SocketCommandContext context, string reason)
        {
            try
            {
                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.Now,
                    DateToRemove = DateTime.Now.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)),
                    DiscipinaryEventType = DisciplinaryEventEnum.WarnEvent,
                    DisciplineEventID = (ulong)DateTime.Now.Millisecond,
                    ModeratorID = context.Client.CurrentUser.Id,
                    Reason = "AutoWarn : " + reason,
                    UserID = context.Message.Author.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = context.Message.Author.Id,
                    UserName = context.Message.Author.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);

                int warnCount = await StorageManager.GetRecentWarningsAsync(context.User.Id);

                await context.Channel.SendMessageAsync($"🚫 {context.User.Mention} {BotDialogs.WarnMessageNoReason}🚫\n{warnCount}/{ConfigManager.GetProperty(PropertyItem.MaxWarns)} ");
            }
            catch (Exception ex)
            {
                //todo
                throw;
            }
        }
    }
}
