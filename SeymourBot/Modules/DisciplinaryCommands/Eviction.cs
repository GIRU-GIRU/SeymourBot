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
using Toolbox.Utils;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Eviction : ModuleBase<SocketCommandContext>
    {
        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(1)]
        public async Task BanUserAsync(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(reason: reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, new TimeSpan(), reason, kickTargetName, false, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = Context.Message.Author.Id,
                    UserName = Context.Message.Author.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(2)]
        public async Task BanUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(reason: reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, new TimeSpan(), reason, kickTargetName, false);
                await Context.Channel.SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = Context.Message.Author.Id,
                    UserName = Context.Message.Author.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }


        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        public async Task PermaBanUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(reason: reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, new TimeSpan(), reason, kickTargetName, false, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });

            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        public async Task PermaBanUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(reason: reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, new TimeSpan(), reason, kickTargetName, false);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });

            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }


        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(1)]
        public async Task BanCleanseUser(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(7, reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, new TimeSpan(), reason, kickTargetName, false, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanCleanseEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = Context.Message.Author.Id,
                    UserName = Context.Message.Author.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(2)]
        public async Task BanCleanseUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(7, reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, new TimeSpan(), reason, kickTargetName, false);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanCleanseEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = Context.Message.Author.Id,
                    UserName = Context.Message.Author.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        public async Task PermaBanCleanseUser(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(7, reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, new TimeSpan(), reason, kickTargetName, false);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanCleanseEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }


        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        public async Task BanCleanseUser(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.BanAsync(7, reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, new TimeSpan(), reason, kickTargetName, false, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanCleanseEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                },
                 new UserStorage()
                 {
                     UserID = user.Id,
                     UserName = user.Username,
                 });
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [DevOrAdmin]
        private async Task KickUserAsync(SocketGuildUser user, [Remainder]string reason = "No reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.KickAsync();
                }
                else
                {
                    return;
                }

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.KickEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                }, new UserStorage()
                {
                    UserID = user.Id,
                    UserName = user.Username,
                });

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.KickEvent, new TimeSpan(), reason, kickTargetName, false, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [DevOrAdmin]
        private async Task KickUserAsync(ulong userID, [Remainder]string reason = "No reason specified")
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;

                string kickTargetName = user.Username;
                if (!await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser))
                {
                    await user.KickAsync();
                }
                else
                {
                    return;
                }

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.KickEvent,
                    DisciplineEventID = (ulong)DateTime.UtcNow.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                }, new UserStorage()
                {
                    UserID = user.Id,
                    UserName = user.Username,
                });

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.KickEvent, new TimeSpan(), reason, kickTargetName, false);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
