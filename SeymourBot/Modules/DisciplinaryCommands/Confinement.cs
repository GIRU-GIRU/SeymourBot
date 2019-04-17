using Discord;
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
using SeymourBot.Utils;
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
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Priority(1)]
        public async Task MuteUserAsync(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
                await user.AddRoleAsync(mutedRole);

                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, Context, timeSpan, reason, user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
                await user.AddRoleAsync(mutedRole);

                UserDisciplinaryPermanentStorage newEvent = new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, Context, new TimeSpan(), reason, user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

        [Command("Limit")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Priority(1)]
        public async Task LimitUserAsync(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(mutedRole);

                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.LimitedUserEvent,
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, Context, timeSpan, reason, user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

        [Command("limit")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task LimitUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(mutedRole);

                UserDisciplinaryPermanentStorage newEvent = new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.LimitedUserEvent,
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, Context, new TimeSpan(), reason, user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

    }
}
