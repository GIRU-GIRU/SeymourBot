using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.AutoModeration;
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
    class Confinement : ModuleBase<SocketCommandContext>
    {

        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        private async Task MuteUserAsync(SocketGuildUser user, int timeToMute = 5, string reason = "")
        {
            try
            {
                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = DateTime.UtcNow.AddMinutes(timeToMute),
                    DiscipinaryEventType = DisciplinaryEventEnum.MuteEvent,
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
                await TimedEventManager.CreateEvent(newEvent, newUser);

                await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention} has muted {user.Mention} for {timeToMute} minute(s)");
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }
    }
}
