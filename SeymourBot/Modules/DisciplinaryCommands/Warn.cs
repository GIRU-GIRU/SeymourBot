using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.AutoModeration;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Warn : ModuleBase<SocketCommandContext>
    {
        [Command("warn")]
        [DevOrAdmin]
        public async Task WarnUserAsync(SocketGuildUser user, [Remainder]string reason = "")
        {
            try
            {
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)),
                    DiscipinaryEventType = DisciplinaryEventEnum.WarnEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = user.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = user.Id,
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
                await AutoModeratorManager.CheckForWarnThreshold(user, Context, warnCount);
            }
            catch (Exception ex)
            {
                //todo
                ExceptionManager.HandleException("", ex);
            }
        }

        [Command("warn")]
        [DevOrAdmin]
        public async Task WarnUserAsync(ulong userID, [Remainder]string reason = "")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContext.GetEmoteAyySeymour()}");
                    return;
                }
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)),
                    DiscipinaryEventType = DisciplinaryEventEnum.WarnEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = user.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = user.Id,
                    UserName = user.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);

                int warnCount = await StorageManager.GetRecentWarningsAsync(user.Id);

                if (string.IsNullOrEmpty(reason))
                {
                    await DiscordContext.GetMainChannel().SendMessageAsync($"🚫 {user.Mention} {BotDialogs.WarnMessageNoReason}🚫\n{warnCount}/5 warnings ");
                }
                else
                {
                    await DiscordContext.GetMainChannel().SendMessageAsync(ResourceUtils.BuildString(BotDialogs.WarnMessageReason, user.Mention, reason, Environment.NewLine, warnCount.ToString(), ConfigManager.GetProperty(PropertyItem.MaxWarns)));
                }
                await AutoModeratorManager.CheckForWarnThreshold(user, Context, warnCount);
            }
            catch (Exception ex)
            {
                //todo
                ExceptionManager.HandleException("", ex);
            }
        }
    }
}
