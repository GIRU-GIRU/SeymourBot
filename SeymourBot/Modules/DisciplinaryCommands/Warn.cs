using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.Config;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.DiscordUtilities;
using SeymourBot.Exceptions;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Resources;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Warn : ModuleBase<SocketCommandContext>
    {
        [Command("warn")]
        [DevOrAdmin]
        private async Task WarnUserAsync(SocketGuildUser user, [Remainder]string reason = "")
        {
            try
            {
                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.Now,
                    DateToRemove = DateTime.Now.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)),
                    DiscipinaryEventType = DisciplinaryEventEnum.WarnEvent,
                    DisciplineEventID = (ulong)DateTime.Now.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = Context.Message.Author.Id,
                    UserName = Context.Message.Author.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);

                int warnCount = await StorageManager.GetRecentWarningsAsync(user.Id);

                if (string.IsNullOrEmpty(reason))
                {
                    await Context.Channel.SendMessageAsync($"🚫 {user.Mention} {BotDialogs.WarnMessageNoReason}🚫\n{warnCount}/5 warnings ");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResourceUtils.BuildString(BotDialogs.WarnMessageReason, user.Mention, reason, Environment.NewLine, warnCount.ToString(), ConfigManager.GetProperty(PropertyItem.MaxWarns)));
                }
            }
            catch (Exception ex)
            {
                //todo
                ExceptionManager.HandleException("", ex);
            }
        }

    }
}
