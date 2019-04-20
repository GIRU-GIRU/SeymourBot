using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.User.Tables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Utils;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    class Blacklist : ModuleBase<SocketCommandContext>
    {
        [Command("blacklist")]
        [DevOrAdmin]
        [Priority(1)]
        public async Task BlacklistUser(SocketGuildUser user, TimeSpan timeSpan)
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
                    UserID = Context.Message.Author.Id
                },
                    new UserStorage()
                    {
                        UserID = user.Id,
                        UserName = user.Username,
                    });

                var embed = Utilities.BuildBlacklistEmbed( new TimeSpan(), user.Username, existing, Context.Message.Author.Username);
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
        public async Task BlacklistUser(ulong userID, TimeSpan timeSpan)
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;
                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

                bool existing = await StorageManager.StoreBlacklistReturnIfExisting(new BlacklistUserStorage()
                {
                    Username = user.Username,
                    DateInserted = DateTime.UtcNow,
                    ModeratorID = Context.Message.Author.Id,
                    DateToRemove = (DateTimeOffset.UtcNow + timeSpan).DateTime,
                    UserID = Context.Message.Author.Id
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
        public async Task MonthBlacklistUser(ulong userID)
        {
            try
            {
                SocketGuildUser user = await Context.Channel.GetUserAsync(userID) as SocketGuildUser;

                if (await DiscordContext.IsUserDevOrAdmin(user as SocketGuildUser)) return;

                bool existing = await StorageManager.StoreBlacklistReturnIfExisting(new BlacklistUserStorage()
                {
                    Username = user.Username,
                    DateInserted = DateTime.UtcNow,
                    ModeratorID = Context.Message.Author.Id,
                    DateToRemove = DateTime.UtcNow.AddMonths(1),
                    UserID = Context.Message.Author.Id
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
        public async Task MonthBlacklistUser(SocketGuildUser user)
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
                    UserID = Context.Message.Author.Id
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
    }
}
