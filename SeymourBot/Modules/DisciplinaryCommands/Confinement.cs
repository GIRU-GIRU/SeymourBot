using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Utils;

namespace SeymourBot.Modules.DisciplinaryCommands
{
   public class Confinement : ModuleBase<SocketCommandContext>
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, Context, timeSpan, reason, user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

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
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

        //[Command("Mute")]
        //[DevOrAdmin]
        //[RequireBotPermission(GuildPermission.ManageRoles)]
        //[Priority(2)]
        //public async Task MuteUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        //{
        //    try
        //    {
        //        SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
        //        var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
        //        await user.AddRoleAsync(mutedRole);

        //        var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, Context, timeSpan, reason, user.Username);
        //        await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);

        //        UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
        //        {
        //            DateInserted = DateTime.UtcNow,
        //            DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
        //            DiscipinaryEventType = DisciplinaryEventEnum.MuteEvent,
        //            DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
        //            ModeratorID = Context.Message.Author.Id,
        //            Reason = reason,
        //            UserID = user.Id
        //        };
        //        UserStorage newUser = new UserStorage()
        //        {
        //            UserID = user.Id,
        //            UserName = user.Username
        //        };
        //        await TimedEventManager.CreateEvent(newEvent, newUser);

   
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionManager.HandleException("", ex); //todo
        //    }
        //}

        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task PermaMuteUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
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

        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task PermaMuteUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
                await user.AddRoleAsync(mutedRole);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, Context, new TimeSpan(), reason, user.Username);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);

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
        public async Task PermaLimitUserAsync(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                var limitedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(limitedRole);

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

        [Command("Limit")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [Priority(2)]
        public async Task LimitUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;

                var limitedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(limitedRole);


                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, Context, timeSpan, reason, user.Username);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);

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
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

        [Command("limit")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task PermaLimitUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                var limitedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(limitedRole);

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

        [Command("limit")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task PermaLimitUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;

                var limitedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(limitedRole);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, Context, new TimeSpan(), reason, user.Username);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);

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
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

    }
}
