using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SeymourBot.Utils;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Eviction : ModuleBase<SocketCommandContext>
    {
        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(1)]
        public async Task BanUser(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, Context, new TimeSpan(), reason, kickTargetName);
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
        public async Task BanUser(SocketGuildUser user, [Remainder]string reason = "no reason specified")
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, Context, new TimeSpan(), reason, kickTargetName);
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, Context, new TimeSpan(), reason, kickTargetName);
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
                        
                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, Context, new TimeSpan(), reason, kickTargetName);
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
        private async Task KickUser(SocketGuildUser user, [Remainder]string reason = "No reason specified")
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

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.KickEvent, Context, new TimeSpan(), reason, kickTargetName);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
