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
using Toolbox.Resources;
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
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var mutedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted);
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
                await DiscordContextOverseer.LogModerationAction(user.Id, "Muted", Context.Message.Author.Id, reason);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.MuteException, ex);
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
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }

                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var mutedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted);
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
                await DiscordContextOverseer.LogModerationAction(userID, "Muted", Context.Message.Author.Id, reason);
                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.MuteEvent, timeSpan, reason, user.Username, existing);
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.MuteException, ex);
            }
        }

        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        private async Task PermaMuteUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var mutedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted);
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
                await DiscordContextOverseer.LogModerationAction(user.Id, "Muted", Context.Message.Author.Id, reason);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.MuteException, ex);
            }
        }

        [Command("Mute")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        private async Task PermaMuteUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }

                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var mutedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted);
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
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);
                await DiscordContextOverseer.LogModerationAction(userID, "Muted", Context.Message.Author.Id, reason);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.MuteException, ex);
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
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var limitedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.LimitedUser);
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
                await DiscordContextOverseer.LogModerationAction(user.Id, "Limited", Context.Message.Author.Id, reason);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.LimitException, ex);
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
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }

                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var limitedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.LimitedUser);
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
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);
                await DiscordContextOverseer.LogModerationAction(userID, "Limited", Context.Message.Author.Id, reason);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.LimitException, ex);
            }
        }

        [Command("limit")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        private async Task PermaLimitUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var limitedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.LimitedUser);
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
                await DiscordContextOverseer.LogModerationAction(user.Id, "Limited", Context.Message.Author.Id, reason);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.LimitException, ex);
            }
        }

        [Command("limit")]
        [DevOrAdmin]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        private async Task PermaLimitUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }

                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                var limitedRole = DiscordContextSeymour.GrabRole(MordhauRoleEnum.LimitedUser);
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
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);
                await DiscordContextOverseer.LogModerationAction(userID, "Limited", Context.Message.Author.Id, reason);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.LimitException, ex);
            }
        }

        [Command("unlimit")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task UnlimitUserAsync(ulong userID)
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
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
                await DiscordContextOverseer.LogModerationAction(userID, "Unlimited", Context.Message.Author.Id, "");
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UnlimitException, ex);
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
                await DiscordContextOverseer.LogModerationAction(user.Id, "Unlimited", Context.Message.Author.Id, "");
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
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
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

                await StorageManager.RemoveDisciplinaryEventAsync(userID, DisciplinaryEventEnum.MuteEvent);
                await DiscordContextOverseer.LogModerationAction(userID, "Unmuted", Context.Message.Author.Id, "");
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UnmuteException, ex);
            }
        }

        [Command("unmute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
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
                await DiscordContextOverseer.LogModerationAction(user.Id, "Unmuted", Context.Message.Author.Id, "");
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UnmuteException, ex);
            }
        }


        [Command("exile")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [DevOrAdmin]
        private async Task ExileUserAsync(SocketGuildUser user)
        {
            try
            {
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                ulong peasantRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_Peasant);
                var roleToApply = Context.Guild.GetRole(peasantRoleID);
                var rolesToRemove = user.Roles.Where(x => x.IsEveryone == false);


                var embed = new EmbedBuilder();

                embed.WithTitle($"{user.Username} has been stripped of their lands and titles, and banished to the gate {DiscordContextSeymour.GetEmoteReee()}");
                embed.WithColor(new Color(255, 0, 0));
                await Context.Channel.SendMessageAsync("", false, embed.Build());

                await user.RemoveRolesAsync(rolesToRemove);
                await user.AddRoleAsync(roleToApply);
                await DiscordContextOverseer.LogModerationAction(user.Id, "Exiled", Context.Message.Author.Id, "");
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.ExileException, ex);
            }
        }

        [Command("exile")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [DevOrAdmin]
        private async Task ExileUserAsync(ulong userID)
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser)) return;

                ulong peasantRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_Peasant);
                var roleToApply = Context.Guild.GetRole(peasantRoleID);
                var rolesToRemove = user.Roles.Where(x => x.IsEveryone == false);


                var embed = new EmbedBuilder();

                embed.WithTitle($"{user.Username} has been stripped of their lands and titles, and banished to the gate {DiscordContextSeymour.GetEmoteReee()}");
                embed.WithColor(new Color(255, 0, 0));
                await Context.Channel.SendMessageAsync("", false, embed.Build());

                await user.RemoveRolesAsync(rolesToRemove);
                await user.AddRoleAsync(roleToApply);
                await DiscordContextOverseer.LogModerationAction(userID, "Exiled", Context.Message.Author.Id, "");
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.ExileException, ex);
            }
        }
    }
}
