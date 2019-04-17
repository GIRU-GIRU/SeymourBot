using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage.User;

namespace SeymourBot.AutoModeration
{
    public static class UserJoinedChecker
    {
        internal static async Task SanitizeJoinedUser(SocketGuildUser user)
        {
            try
            {
                var pendingEventType = await StorageManager.CheckPendingDisciplinaries(user);

                if (pendingEventType != DisciplinaryEventEnum.NoEvent)
                {
                    await PunishmentExecution.ReapplyDisciplinaryAction(pendingEventType, user);
                }              
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
