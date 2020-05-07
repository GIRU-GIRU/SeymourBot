using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;

namespace OverseerBot.UserJoinedServer
{
    public static class RoleApplication
    {
        public static async Task ApplyPeasantRoleAsync(SocketGuildUser user)
        {
            try
            {
                await user.AddRoleAsync(DiscordContextOverseer.GrabRole(MordhauRoleEnum.Peasant));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException($"{typeof(RoleApplication).GetType().FullName}: {ExceptionManager.GetAsyncMethodName()}", ex);
            }
        }
    }
}
