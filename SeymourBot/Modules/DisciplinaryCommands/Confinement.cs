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
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Config;
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
        private async Task MuteUserAsync(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, timeSpan, reason, user.Username, existing, Context.Message.Author.Username);
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
        private async Task MuteUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
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

                var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
                await user.AddRoleAsync(mutedRole);

                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.MuteEvent,
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
        private async Task PermaMuteUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
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
        private async Task PermaMuteUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
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

                var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
                await user.AddRoleAsync(mutedRole);

                UserDisciplinaryPermanentStorage newEvent = new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.MuteEvent,
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
        private async Task LimitUserAsync(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
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
        private async Task LimitUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
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

                var limitedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(limitedRole);

                UserDisciplinaryEventStorage newEvent = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.LimitedUserEvent,
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
        private async Task PermaLimitUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
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
        private async Task PermaLimitUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
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

                var limitedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                await user.AddRoleAsync(limitedRole);

                UserDisciplinaryPermanentStorage newEvent = new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.LimitedUserEvent,
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

        [Command("unlimit")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task UnlimitUserAsync(ulong userID)
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContext.GetEmoteAyySeymour()}");
                    return;
                }

                var role = user.Roles.FirstOrDefault(x => x.Id == ConfigManager.GetUlongProperty(PropertyItem.Role_LimitedUser));

                if (role != null)
                {
                    await Context.Channel.SendMessageAsync("Cannot see limited role on that user");
                    return;
                }

                await user.RemoveRoleAsync(role);
                var embed = Utilities.BuildRemoveDisciplinaryEmbed($"Successfully unlimited", user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                await StorageManager.RemoveDisciplinaryEventAsync(userID, DisciplinaryEventEnum.LimitedUserEvent);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("unlimit")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task UnlimitUserAsync(SocketGuildUser user)
        {
            try
            {
                var role = user.Roles.FirstOrDefault(x => x.Id == ConfigManager.GetUlongProperty(PropertyItem.Role_LimitedUser));

                if (role == null)
                {
                    await Context.Channel.SendMessageAsync("Cannot see limited role on that user");
                    return;
                }

                await user.RemoveRoleAsync(role);
                var embed = Utilities.BuildRemoveDisciplinaryEmbed($"Successfully unlimited", user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                await StorageManager.RemoveDisciplinaryEventAsync(user.Id, DisciplinaryEventEnum.LimitedUserEvent);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("unmute")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task UnmuteUserAsync(ulong userID)
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContext.GetEmoteAyySeymour()}");
                    return;
                }

                var role = user.Roles.FirstOrDefault(x => x.Id == ConfigManager.GetUlongProperty(PropertyItem.Role_Muted));

                if (role != null)
                {
                    await Context.Channel.SendMessageAsync("Cannot see muted role on that user");
                    return;
                }

                await user.RemoveRoleAsync(role);
                var embed = Utilities.BuildRemoveDisciplinaryEmbed($"Successfully unmuted", user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                await StorageManager.RemoveDisciplinaryEventAsync(userID, DisciplinaryEventEnum.BanEvent);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("unmute")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task UnmuteUserAsync(SocketGuildUser user)
        {
            try
            {
                var role = user.Roles.FirstOrDefault(x => x.Id == ConfigManager.GetUlongProperty(PropertyItem.Role_Muted));

                if (role == null)
                {
                    await Context.Channel.SendMessageAsync("Cannot see muted role on that user");
                    return;
                }

                await user.RemoveRoleAsync(role);
                var embed = Utilities.BuildRemoveDisciplinaryEmbed($"Successfully unmuted", user.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                await StorageManager.RemoveDisciplinaryEventAsync(user.Id, DisciplinaryEventEnum.MuteEvent);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

    }
}
