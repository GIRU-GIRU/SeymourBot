using Discord;
using Discord.WebSocket;
using SeymourBot.AutoModeration;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace SeymourBot.Modules.CommandUtils
{
    public static class PunishmentExecution
    {
        public static async Task ReapplyDisciplinaryAction(List<DisciplinaryEventEnum> eventType, SocketGuildUser user)
        {
            try
            {
                foreach (var eventInstance in eventType)
                {
                    await ReapplyDisciplinaryAction(eventInstance, user);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.AutomaticDisciplinaryReapplication, ex);
            }
        }

        public static async Task ReapplyDisciplinaryAction(DisciplinaryEventEnum eventType, SocketGuildUser user)
        {
            try
            {
                IRole role;
                switch (eventType)
                {
                    case DisciplinaryEventEnum.MuteEvent:
                        role = DiscordContextSeymour.GrabRole(MordhauRoleEnum.Muted);
                        await user.AddRoleAsync(role);
                        break;
                    case DisciplinaryEventEnum.LimitedUserEvent:
                        role = DiscordContextSeymour.GrabRole(MordhauRoleEnum.LimitedUser);
                        await user.AddRoleAsync(role);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.AutomaticDisciplinaryReapplication, ex);
            }
        }
    }
}
