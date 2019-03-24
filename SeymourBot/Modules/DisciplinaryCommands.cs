using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.DiscordUtilities;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Modules
{
    class DisciplinaryCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Mute")]
        [RequireBotPermission(Discord.GuildPermission.ManageRoles)]
        private async Task TempMuteCommandTest(SocketGuildUser user)
        {
            try
            {
                var role = DiscordObject.GrabRole(MordhauRoleEnum.Muted, Context);

                await user.AddRoleAsync(role);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
