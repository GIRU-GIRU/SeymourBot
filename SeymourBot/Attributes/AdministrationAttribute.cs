﻿using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
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

            if (user.Roles.Where(x => x.Name.ToLower() == MordhauRoleEnum.Developer.ToString().ToLower()).Any())
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError("Unauthorized");
        }
    }
}

