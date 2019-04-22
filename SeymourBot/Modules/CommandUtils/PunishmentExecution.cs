using Discord;
using Discord.WebSocket;
using SeymourBot.AutoModeration;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;

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
                throw ex;//todo
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
                        role = DiscordContext.GrabRole(MordhauRoleEnum.Muted);
                        await user.AddRoleAsync(role);
                        break;
                    case DisciplinaryEventEnum.LimitedUserEvent:
                        role = DiscordContext.GrabRole(MordhauRoleEnum.LimitedUser);
                        await user.AddRoleAsync(role);
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
