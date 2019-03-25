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
        private async Task TempMuteCommandTest(SocketGuildUser user)
        {
            try
            {
                var role = DiscordObject.GrabRole(MordhauRoleEnum.Muted, Context);

                await user.AddRoleAsync(role);
                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.Now,
                    DateToRemove = DateTime.Now.AddMinutes(5),
                    DiscipinaryEventType = Storage.User.DisciplineEventEnum.MuteEvent,
                    DisciplineEventID = (ulong)DateTime.Now.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = "TEST",
                    UserID = user.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = user.Id,
                    UserName = user.Username
                };
                TimedEventManager.CreateEvent(newEvent, newUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
