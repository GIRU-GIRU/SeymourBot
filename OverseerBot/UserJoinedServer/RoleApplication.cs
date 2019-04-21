using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;

namespace OverseerBot.UserJoinedServer
{
    public static class RoleApplication
    {
        public static async Task ApplyPeasantRoleAsync(SocketGuildUser user)
        {
            try
            {
                await user.AddRoleAsync(DiscordContext.GrabRole(MordhauRoleEnum.Peasant));
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
