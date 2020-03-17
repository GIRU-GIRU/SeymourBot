using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;

namespace SeymourBot.AutoModeration
{
    public static class MessageContentChecker
    {
        internal static async Task AutoModerateMessage(SocketCommandContext context)
        {
            if (!await DiscordContextSeymour.IsUserDevOrAdminAsync(context.Message.Author as SocketGuildUser))
            {
                _ = Task.Run(() => MessageContainsAsync(context));
                _ = Task.Run(() => MassMentionCheck(context));
            }
        }

        internal static async Task MessageContainsAsync(SocketCommandContext context)
        {
            await AutoModeratorManager.FilterMessage(context);

        }

        internal static async Task MassMentionCheck(SocketCommandContext context)
        {
            try
            {
                if (context.Message.MentionedUsers.Count > 8)
                {
                    SocketUser target = context.Message.Author;
                    await context.Message.DeleteAsync();
                    await DiscordContextSeymour.AddRole(DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted), target.Id);
                    await DiscordContextOverseer.LogModerationAction(target.Id, "Muted", "excessive pinging", (new TimeSpan(3, 0, 0, 0)).ToString());
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.MuteEvent,
                                          context.Client.CurrentUser.Id,
                                          "excessive pinging",
                                          target.Id,
                                          target.Username,
                                          (DateTimeOffset.UtcNow + new TimeSpan(0, 30, 0)).DateTime);
                    await TimedEventManager.CreateEvent(DisciplinaryEventEnum.WarnEvent, context.Client.CurrentUser.Id, "AutoWarn : excessive pinging", target.Id, target.Username, DateTime.UtcNow.AddDays(ConfigManager.GetIntegerProperty(PropertyItem.WarnDuration)));
                    if (context.Channel != null)
                    {
                        await DiscordContextOverseer.GetChannel(context.Channel.Id).SendMessageAsync($"{context.Message.Author.Mention}, Thou shall not say thy noble's names in vain. {DiscordContextSeymour.GetEmoteAyySeymour()}");

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }

        }
    }
}
