﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;
using Toolbox.Utils;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Eviction : ModuleBase<SocketCommandContext>
    {
        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(1)]
        private async Task BanUserAsync(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
                {
                    await user.BanAsync(reason: reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, timeSpan, reason, kickTargetName, false, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
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
                ExceptionManager.HandleException(ErrMessages.BanException, ex);
            }
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(2)]
        private async Task BanUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }
                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
                {
                    await user.BanAsync(reason: reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, timeSpan, reason, kickTargetName, false);
                await Context.Channel.SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
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
                ExceptionManager.HandleException(ErrMessages.BanException, ex);
            }
        }


        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task PermaBanUserAsync(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
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
                ExceptionManager.HandleException(ErrMessages.BanException, ex);
            }
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task PermaBanUserAsync(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }
                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
                {
                    await user.BanAsync(reason: reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanEvent, new TimeSpan(), reason, kickTargetName, false);
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
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
                ExceptionManager.HandleException(ErrMessages.BanException, ex);
            }
        }


        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(1)]
        private async Task BanCleanseUser(SocketGuildUser user, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
                {
                    await user.BanAsync(7, reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, timeSpan, reason, kickTargetName, false, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanCleanseEvent,
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
                ExceptionManager.HandleException(ErrMessages.BancleanseException, ex);
            }
        }

        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        [Priority(2)]
        private async Task BanCleanseUserAsync(ulong userID, TimeSpan timeSpan, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }
                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
                {
                    await user.BanAsync(7, reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, timeSpan, reason, kickTargetName, false);
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);

                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanCleanseEvent,
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
                ExceptionManager.HandleException(ErrMessages.BancleanseException, ex);
            }
        }

        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task PermaBanCleanseUser(ulong userID, [Remainder]string reason = "no reason specified")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }

                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
                {
                    await user.BanAsync(7, reason);
                }
                else
                {
                    return;
                }

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.BanCleanseEvent, new TimeSpan(), reason, kickTargetName, false);
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);

                await StorageManager.StoreDisciplinaryPermanentEventAsync(new UserDisciplinaryPermanentStorage()
                {
                    DateInserted = DateTime.UtcNow,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanCleanseEvent,
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
                ExceptionManager.HandleException(ErrMessages.BancleanseException, ex);
            }
        }


        [Command("bancleanse")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task PermaBanCleanseUser(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
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
                ExceptionManager.HandleException(ErrMessages.BancleanseException, ex);
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
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
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
                ExceptionManager.HandleException(ErrMessages.KickException, ex);
            }
        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [DevOrAdmin]
        private async Task KickUserAsync(ulong userID, [Remainder]string reason = "No reason specified")
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(userID);
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContextSeymour.GetEmoteAyySeymour()}");
                    return;
                }

                string kickTargetName = user.Username;
                if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(user as SocketGuildUser))
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
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                }, new UserStorage()
                {
                    UserID = user.Id,
                    UserName = user.Username,
                });

                var embed = Utilities.BuildDefaultEmbed(DisciplinaryEventEnum.KickEvent, new TimeSpan(), reason, kickTargetName, false);
                await DiscordContextSeymour.GetMainChannel().SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.KickException, ex);
            }

        }


        [Command("searchban")]
        [DevOrAdmin]
        private async Task SearchBannedUsersAsync([Remainder]string input = null)
        {
            try
            {
                if (input == null) return;

                var bans = await Context.Guild.GetBansAsync();
                List<Discord.Rest.RestBan> matchedBans = new List<Discord.Rest.RestBan>();
                foreach (var ban in bans)
                {
                    if (ban.User.Username.ToLower().Contains(input.ToLower()))
                    {
                        matchedBans.Add(ban);
                    }
                }

                if (bans.Count == 0 || matchedBans.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("Unable to find a banned user of that name");
                    return;
                }

                var bannedUserNames = string.Join("\n", matchedBans.Select(x => x.User.Username).ToArray());
                var bannedUserReasons = matchedBans.Select(x => x.Reason).ToArray();
                string listOfReasons = string.Empty;
                if (bannedUserReasons.Count() > 0)
                {
                    listOfReasons = string.Join("\n", bannedUserReasons);
                }

                var embed = new EmbedBuilder();
                embed.WithTitle($"Banned user names matching \"{input}\" ");
                embed.AddField("Username", bannedUserNames, true);
                embed.AddField("User ID: ", string.Join("\n", matchedBans.Select(x => x.User.Id)), true);
                if (!string.IsNullOrEmpty(listOfReasons)) embed.AddField("Reason for ban: ", listOfReasons, true);
                await Context.Channel.SendMessageAsync("", false, embed.Build());

            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.SearchbanException, ex);
            }
        }
            

        [Command("returnban")]
        [DevOrAdmin]
        private async Task ReturnBannedUserIDAsync([Remainder]string input = null)
        {
            try
            {
                if (input == null) return;

                var bans = await Context.Guild.GetBansAsync();
                List<Discord.Rest.RestBan> matchedBans = new List<Discord.Rest.RestBan>();
                foreach (var ban in bans)
                {
                    if (ban.User.Username.ToLower().Contains(input.ToLower()))
                    {
                        matchedBans.Add(ban);
                    }
                }

                if (bans.Count == 0 || matchedBans.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("Unable to find a banned user of that name");
                    return;
                }

                var bannedUserNames = matchedBans.Select(x => x.User.Username + "#" + x.User.Discriminator).ToArray();
                var bannedUserIDs = matchedBans.Select(x => x.User.Id).ToArray();

                int count = bannedUserNames.Count() > bannedUserIDs.Count() ? bannedUserIDs.Count() : bannedUserNames.Count();

                for (int i = 0; i < count; i++)
                {
                    await Context.Channel.SendMessageAsync(bannedUserNames[i]);
                    await Context.Channel.SendMessageAsync(bannedUserIDs[i].ToString());

                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.ReturnbanException, ex);
            }            
        }


        [Command("unban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task UnbanUserAsync(ulong userID)
        {
            try
            {
                bool existing = false;
                string bannedUserName = "";

                var bans = await Context.Guild.GetBansAsync();
                foreach (var ban in bans)
                {
                    if (ban.User.Id == userID)
                    {
                        existing = true;
                        bannedUserName = ban.User.Username;
                        break;
                    }
                }

                if (!existing)
                {
                    await Context.Channel.SendMessageAsync("Unable to locate a ban correlating to a user of that ID");
                    return;
                }

                await Context.Guild.RemoveBanAsync(userID);
                var embed = Utilities.BuildRemoveDisciplinaryEmbed($"Successfully unbanned", bannedUserName);
                await Context.Channel.SendMessageAsync("", false, embed);

                await StorageManager.RemoveDisciplinaryEventAsync(userID, DisciplinaryEventEnum.BanEvent);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UnbanException, ex);
            }
        }
    }
}
