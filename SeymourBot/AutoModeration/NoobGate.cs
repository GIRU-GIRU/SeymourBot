using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;

namespace SeymourBot.AutoModeration
{
    public static class NoobGate
    {
        public static async Task CreateNoobGate()
        {
            try
            {
                var noobGateChannel = DiscordContext.GetNoobGateChannel();

                var msgs = await noobGateChannel.GetMessagesAsync(100).FlattenAsync();
                await noobGateChannel.DeleteMessagesAsync(msgs);

                string welcome = ConfigManager.GetProperty(PropertyItem.NoobGateMessage);
                var noobGateWelcomeMessage = await noobGateChannel.SendMessageAsync(welcome);
                DiscordContext.ReAssignNoobGateWelcome(noobGateWelcomeMessage);

                await noobGateWelcomeMessage.AddReactionAsync(DiscordContext.GetEmotePommel() as IEmote);

            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        public static async Task VerifyUserAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                if (DiscordContext.GetNoobGateChannel().Id == channel.Id)
                {
                    if (message.Id == DiscordContext.GetNoobGateWelcome().Id)
                    {
                        if (reaction.Emote.Name == DiscordContext.GetEmotePommel().Name)
                        {
                            SocketGuildUser user = reaction.User.Value as SocketGuildUser;

                            if (user != null)
                            {
                                var roleToRemove = DiscordContext.GrabRole(MordhauRoleEnum.Peasant);
                                if (user.Roles.Any(x => x == roleToRemove) || roleToRemove != null)
                                {
                                    await user.RemoveRoleAsync(roleToRemove);
                                }
                            }
                        }
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
