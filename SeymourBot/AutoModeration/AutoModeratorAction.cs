using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.AutoModeration
{
    public static class AutoModeratorAction
    {
        public static async Task DeleteInviteLinkWarn(SocketCommandContext context)
        {
            await context.Message.DeleteAsync();
            await context.Channel.SendMessageAsync($"{context.User.Mention}, do not post invite links");

            //todo warn event
        }

        public static async Task DeleteBannedWordWarn(SocketCommandContext context)
        {
            await context.Message.DeleteAsync();
            await context.Channel.SendMessageAsync($"{context.User.Mention}, do not use such foul language in my presence");

            //todo warn event
        }

    }
}
