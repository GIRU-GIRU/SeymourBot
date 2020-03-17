using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;

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
                    var rolesToRemove = new List<IRole>();

                    foreach (var role in user.Roles)
                    {
                        if (role.Id == limitedRoleID || role.Id == mutedRoleID)
                        {
                            rolesToRemove.Add(role);
                        }
                    }

                    if (rolesToRemove.Count() > 0) await user.RemoveRolesAsync(rolesToRemove);
                    await DiscordContextOverseer.LogModerationAction(user.Id, "Pardonned", Context.Message.Author.Id, "", "");
                    await Context.Channel.SendMessageAsync($"{user.Mention} has been pardoned for their crimes.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Could not find active punishments for {user.Mention}.");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.PardonException, ex);
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
                        var rolesToRemove = new List<IRole>();

                        foreach (var role in user.Roles)
                        {
                            if (role.Id == limitedRoleID || role.Id == mutedRoleID)
                            {
                                rolesToRemove.Add(role);
                            }
                        }

                        if (rolesToRemove.Count() > 0) await user.RemoveRolesAsync(rolesToRemove);
                        await DiscordContextOverseer.LogModerationAction(userID, "Pardonned", Context.Message.Author.Id, "", "");
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
                ExceptionManager.HandleException(ErrMessages.PardonException, ex);
            }
        }
    }
}
