using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.DiscordUtilities;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Resources;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Modules
{
    public class DisciplinaryCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        private async Task MuteUserAsync(SocketGuildUser user, int timeToMute = 5, string reason = "")
        {
            try
            {
                var role = DiscordContext.GrabRole(MordhauRoleEnum.Muted);

                await user.AddRoleAsync(role);
                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.Now,
                    DateToRemove = DateTime.Now.AddMinutes(timeToMute),
                    DiscipinaryEventType = DisciplinaryEventEnum.MuteEvent,
                    DisciplineEventID = (ulong)DateTime.Now.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = user.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = user.Id,
                    UserName = user.Username
                };
                TimedEventManager.CreateEvent(newEvent, newUser);

                await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention} has muted {user.Mention} for {timeToMute} minute(s)");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Command("warn")]
        [DevOrAdmin]
        private async Task WarnUserAsync(SocketGuildUser user, [Remainder]string reason = "")
        {
            try
            {
                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.Now,
                    DateToRemove = DateTime.Now.AddDays(14), //todo add config item and max warn config
                    DiscipinaryEventType = DisciplinaryEventEnum.WarnEvent,
                    DisciplineEventID = (ulong)DateTime.Now.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = user.Id
                };

                await StorageManager.StoreDisciplinaryEventAsync(obj);

                int warnCount = await StorageManager.GetRecentWarningsAsync(user.Id);

                if (string.IsNullOrEmpty(reason))
                {
                    await Context.Channel.SendMessageAsync($"🚫 {user.Mention} {BotDialogs.WarnMessageNoReason}🚫\n{warnCount}/5 warnings ");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"🚫 {user.Mention} {BotDialogs.WarnMessageReason} {reason}🚫\n{warnCount}/5 warnings");
                }
            }
            catch (Exception ex)
            {
                //todo
                throw;
            }

        }
    }
}
