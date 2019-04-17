using Discord.WebSocket;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Threading.Tasks;

namespace SeymourBot.AutoModeration
{
    public static class UserJoinedChecker
    {
        internal static async Task SanitizeJoinedUser(SocketGuildUser user)
        {
            try
            {
                var pendingEventTypeList = await StorageManager.CheckPendingDisciplinaries(user);

                if (pendingEventTypeList.Count > 0)
                {
                    await PunishmentExecution.ReapplyDisciplinaryAction(pendingEventTypeList, user);
                }
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
