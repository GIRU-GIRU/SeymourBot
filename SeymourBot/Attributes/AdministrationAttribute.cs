using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;

namespace SeymourBot.Attributes

{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DevOrAdmin : PreconditionAttribute
    {
        // Override the CheckPermissions method
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {

            var user = context.Message.Author as SocketGuildUser;

            if (user.Roles.Where(x => x.Name.ToLower() == "dev").Any())
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError("Invalid roles");
        }
    }
}

