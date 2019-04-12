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
                await user.BanAsync(reason: reason);
                await Context.Channel.SendMessageAsync("", false, BuildBanEmbed(Context, kickTargetName, timeSpan, reason));


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
                await user.BanAsync(reason: reason);
                await Context.Channel.SendMessageAsync("", false, BuildBanEmbed(Context, kickTargetName, new TimeSpan(), reason));

                await StorageManager.StoreDisciplinaryPermanentEventAsync( new UserDisciplinaryPermanentStorage()
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
                await user.BanAsync(7, reason);
                await Context.Channel.SendMessageAsync("", false, BuildBanEmbed(Context, kickTargetName, timeSpan, reason));


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


        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        public async Task BanCleanseUser(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                await user.BanAsync(7, reason);
                await Context.Channel.SendMessageAsync("", false, BuildBanEmbed(Context, kickTargetName, new TimeSpan(), reason));

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


        private Embed BuildBanEmbed(SocketCommandContext context, string kickTargetName, TimeSpan timeSpan, string reason)
        {
            try
            {
                Emote emote;
                string banLength = "Permanent.";

                if (timeSpan.TotalDays > 0)
                {
                    banLength = $"{Math.Round(timeSpan.TotalDays, 2)} day(s).";
                }
                else if (timeSpan.TotalHours > 0)
                {
                    banLength = $"{Math.Round(timeSpan.TotalMinutes, 2)} hour(s).";
                }
                else if (timeSpan.TotalMinutes > 0)
                {
                    banLength = $"{Math.Round(timeSpan.TotalMinutes, 2)} min(s).";
                }

                emote = banLength == "Permanent" ? DiscordContext.GetEmoteReee() : DiscordContext.GetEmoteAyySeymour();

                var embed = new EmbedBuilder();
                embed.WithTitle($"{Context.User.Username} banned {kickTargetName} {emote.ToString()} ");
                embed.WithDescription($"Reason: {reason}\nDuration: {banLength}");
                embed.WithColor(new Color(255, 0, 0));

                return embed.Build();
            }
            catch (Exception ex)
            {
                throw ex; 
            }           
        }


        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [DevOrAdmin]
        private async Task KickUser(SocketGuildUser user, [Remainder]string reason = "No reason specified")
        {
            try
            {
                await user.KickAsync();
                string kickTargetName = user.Username;

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


                var seymourEmote = DiscordContext.GetEmote("ayyseymour");//todo

                var embed = new EmbedBuilder();
                embed.WithTitle($"{Context.User.Username} booted {kickTargetName} {seymourEmote.ToString()} ");
                embed.WithDescription($"reason: {reason}");
                embed.WithColor(new Color(255, 0, 0));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

    }
}
