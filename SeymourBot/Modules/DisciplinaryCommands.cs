using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.DiscordUtilities;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
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
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        private async Task TempMuteCommandTest(SocketGuildUser user, int timeToMute = 5, string reason = "No reason given")
        {
            try
            {
                var role = DiscordContext.GrabRole(MordhauRoleEnum.Muted);

                await user.AddRoleAsync(role);
                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.Now,
                    DateToRemove = DateTime.Now.AddMinutes(timeToMute),
                    DiscipinaryEventType = Storage.User.DisciplineEventEnum.MuteEvent,
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
    }
}
