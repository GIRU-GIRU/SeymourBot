using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;

namespace SeymourBot.Attributes

{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DevOrAdmin : PreconditionAttribute
    {
        // Override the CheckPermissions method
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {

            var user = context.Message.Author as SocketGuildUser;

            ulong modRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_Moderator);
            ulong devRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_Developer);

            if (user.Roles.Any(x => x.Id == devRoleID || x.Id == modRoleID))
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError("Unauthorized");
        }
    }
}

