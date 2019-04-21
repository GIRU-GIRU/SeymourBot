using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.User.Tables;
using System;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Utils;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Blacklist : ModuleBase<SocketCommandContext>
    {
        [Command("blacklist")]
        [DevOrAdmin]
        [Priority(1)]
        public async Task BlacklistUserAsync(SocketGuildUser user, TimeSpan timeSpan)
        {
            try
            {
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

                bool existing = await StorageManager.StoreBlacklistReturnIfExisting(new BlacklistUserStorage()
                {
                    Username = user.Username,
                    DateInserted = DateTime.UtcNow,
                    ModeratorID = Context.Message.Author.Id,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    UserID = user.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });

                var embed = Utilities.BuildBlacklistEmbed(timeSpan, user.Username, existing, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("blacklist")]
        [DevOrAdmin]
        [Priority(1)]
        public async Task BlacklistUserAsync(ulong userID, TimeSpan timeSpan)
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

                bool existing = await StorageManager.StoreBlacklistReturnIfExisting(new BlacklistUserStorage()
                {
                    Username = user.Username,
                    DateInserted = DateTime.UtcNow,
                    ModeratorID = Context.Message.Author.Id,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    UserID = user.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });

                var embed = Utilities.BuildBlacklistEmbed(timeSpan, user.Username, existing);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("blacklist")]
        [DevOrAdmin]
        public async Task MonthBlacklistUserAsync(ulong userID)
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

                bool existing = await StorageManager.StoreBlacklistReturnIfExisting(new BlacklistUserStorage()
                {
                    Username = user.Username,
                    DateInserted = DateTime.UtcNow,
                    ModeratorID = Context.Message.Author.Id,
                    DateToRemove = DateTime.UtcNow.AddMonths(1),
                    UserID = user.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });

                var embed = Utilities.BuildBlacklistEmbed(new TimeSpan(), user.Username, existing);
                await DiscordContext.GetMainChannel().SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("blacklist")]
        [DevOrAdmin]
        public async Task MonthBlacklistUserAsync(SocketGuildUser user)
        {
            try
            {
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

                bool existing = await StorageManager.StoreBlacklistReturnIfExisting(new BlacklistUserStorage()
                {
                    Username = user.Username,
                    DateInserted = DateTime.UtcNow,
                    ModeratorID = Context.Message.Author.Id,
                    DateToRemove = DateTime.UtcNow.AddMonths(1),
                    UserID = user.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });

                var embed = Utilities.BuildBlacklistEmbed(new TimeSpan(), user.Username, existing, Context.Message.Author.Username);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("unblacklist")]
        [DevOrAdmin]
        public async Task UnblacklistUserAsync(SocketGuildUser user)
        {
            try
            {
                bool existing = await StorageManager.RemoveBlacklist(user.Id);
                if (existing)
                {
                    var embed = Utilities.BuildRemoveDisciplinaryEmbed("Successfully unblacklisted", user.Username);
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Could not find that user within the blacklist");
                }

            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("unblacklist")]
        [DevOrAdmin]
        public async Task UnblacklistUserAsync(ulong userID)
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                if (user == null)
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user {DiscordContext.GetEmoteAyySeymour()}");
                    return;
                }

                bool existing = await StorageManager.RemoveBlacklist(user.Id);
                if (existing)
                {
                    var embed = Utilities.BuildRemoveDisciplinaryEmbed("Successfully unblacklisted", user.Username);
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Could not find that user within the blacklist");
                }

            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
