using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
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
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await TimedEventManager.CreateEvent(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent,  timeSpan, reason, user.Username, existing, Context.Message.Author.Username);
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
        [Priority(2)]
        public async Task MuteUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await TimedEventManager.CreateEvent(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, timeSpan, reason, user.Username, existing);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task PermaMuteUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await StorageManager.StoreDisciplinaryPermanentEventAsync(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, new TimeSpan(), reason, user.Username, existing, Context.Message.Author.Username);
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
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await StorageManager.StoreDisciplinaryPermanentEventAsync(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, new TimeSpan(), reason, user.Username, existing);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);
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
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await TimedEventManager.CreateEvent(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, timeSpan, reason, user.Username, existing, Context.Message.Author.Username);
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
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await TimedEventManager.CreateEvent(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, timeSpan, reason, user.Username, existing);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);
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
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await StorageManager.StoreDisciplinaryPermanentEventAsync(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, new TimeSpan(), reason, user.Username, existing, Context.Message.Author.Username);
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
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

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

                bool existing = await StorageManager.StoreDisciplinaryPermanentEventAsync(newEvent, newUser);

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.LimitedUserEvent, new TimeSpan(), reason, user.Username, existing);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("", ex); //todo
            }
        }

    }
}
