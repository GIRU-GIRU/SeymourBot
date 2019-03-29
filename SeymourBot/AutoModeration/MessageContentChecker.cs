using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace SeymourBot.AutoModeration
{
    public static class MessageContentChecker
    {
        private static Regex _regexInviteLinkDiscord = new Regex(@"(https?:\/\/)?(www\.)?(discord\.(gg|io|me|li)|discordapp\.com\/invite)\/.+[a-z]");

        internal static async Task MessageContainsAsync(SocketCommandContext context)
        {
            if (_regexInviteLinkDiscord.Match(context.Message.Content).Success)
            {
                await AutoModeratorAction.DeleteInviteLinkWarn(context);
            }

            foreach (var item in tempBannedWordArray)
            {
                if (context.Message.Content.ToLower().Contains(item.ToLower()))
                {
                    await AutoModeratorAction.DeleteBannedWordWarn(context);
                }
            }

        }


        private static readonly string[] tempBannedWordArray = new string[]
        {
            "N WORD",
            "bastard",
            "judaism",
            "son of bastard",
        };
    }
}
