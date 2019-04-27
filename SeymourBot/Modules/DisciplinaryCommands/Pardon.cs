using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Config;

namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Pardon : ModuleBase<SocketCommandContext>
    {
        [Command("pardon")]
        [DevOrAdmin]
        private async Task PardonUserAsync(SocketGuildUser user)
        {
            try
            {
                bool existing = await StorageManager.RemoveActiveDisciplinaries(user.Id);

                if (existing)
                {
                    ulong mutedRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_Muted);
                    ulong limitedRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_LimitedUser);

                    var roles = user.Roles.Where(x => x.Id == mutedRoleID && x.Id == limitedRoleID).Cast<IRole>().ToArray();

                    if (roles.Count() > 0) await user.RemoveRolesAsync(roles);                    
                  
                    await Context.Channel.SendMessageAsync($"{user.Mention} has been pardoned for their crimes.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Could not find active punishments for {user.Mention}.");
                }
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        [Command("pardon")]
        [DevOrAdmin]
        private async Task PardonUserAsync(ulong userID)
        {
            try
            {
                var user = Context.Guild.GetUser(userID);
                if (user != null)
                {
                    bool existing = await StorageManager.RemoveActiveDisciplinaries(userID);

                    if (existing)
                    {
                        ulong mutedRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_Muted);
                        ulong limitedRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_LimitedUser);

                        var roles = user.Roles.Where(x => x.Id == mutedRoleID || x.Id == limitedRoleID) as IRole[];

                        if (roles.Count() > 0) await user.RemoveRolesAsync(roles);
                        await Context.Channel.SendMessageAsync($"{user.Mention} has been pardoned for their crimes");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync($"Could not find active punishments for {user.Mention}");
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Unable to locate user");
                }          
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }
    }
}
