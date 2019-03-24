using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Config;
using SeymourBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.DiscordUtilities
{
    public static class DiscordObject
    {
        public static IRole GrabRole(MordhauRoleEnum role, SocketCommandContext context)
        {
            try
            {
              return context.Guild.GetRole(ConfigManager.GetUlongProperty(Property.FromString(role.ToString())));
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException("0701", ex, "Probably role sync issue");
                throw;
            }
        }
    }
}
