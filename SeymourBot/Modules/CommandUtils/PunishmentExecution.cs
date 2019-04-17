using Discord.WebSocket;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;

namespace SeymourBot.Modules.CommandUtils
{
    public static class PunishmentExecution
    {
        public static async Task ReapplyDisciplinaryAction(DisciplinaryEventEnum eventType, SocketGuildUser user)
        {
            try
            {
                switch (eventType)
                {
                    case DisciplinaryEventEnum.MuteEvent:
                        var mutedRole = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
                        await user.AddRoleAsync(mutedRole);
                        break;
                    case DisciplinaryEventEnum.LimitedUserEvent:
                        var limitedRole = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                        await user.AddRoleAsync(limitedRole);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;//todo
            }

        }


    }
}
