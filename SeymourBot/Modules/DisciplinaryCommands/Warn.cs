using Discord;
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
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)),
                    DiscipinaryEventType = DisciplinaryEventEnum.WarnEvent,
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

                string maxWarns = ConfigManager.GetProperty(PropertyItem.MaxWarns);
                if (string.IsNullOrEmpty(reason))
                {
                    await Context.Channel.SendMessageAsync($"{user.Mention} {BotDialogs.WarnMessageNoReason}🚫\n{warnCount}/{maxWarns} warnings.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"{user.Mention} {BotDialogs.WarnMessageReason} 🚫\n{warnCount}/{maxWarns} warnings.\n{reason}");
                }


                await AutoModeratorManager.CheckForWarnThreshold(user, Context, warnCount);


            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.WarnException, ex);
            }
        }

        [Command("warn")]
        [DevOrAdmin]
        public async Task WarnUserAsync(ulong userID, SocketGuildChannel chnl, [Remainder]string reason = "")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                var channel = chnl as ITextChannel;
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)),
                    DiscipinaryEventType = DisciplinaryEventEnum.WarnEvent,
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
                string maxWarns = ConfigManager.GetProperty(PropertyItem.MaxWarns);
                if (string.IsNullOrEmpty(reason))
                {
                    await channel.SendMessageAsync($"{user.Mention} {BotDialogs.WarnMessageNoReason}🚫\n{warnCount}/{maxWarns} warnings.");
                }
                else
                {
                    await channel.SendMessageAsync($"{user.Mention} {BotDialogs.WarnMessageReason} 🚫\n{warnCount}/{maxWarns} warnings.\n{reason}");
                }
                await AutoModeratorManager.CheckForWarnThreshold(user, Context, warnCount, channel);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.WarnException, ex);
            }
        }
    }
}
