using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Utils;
namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Raidprotect : ModuleBase<SocketCommandContext>
    {
        [Command("raidprotect on")]
        [DevOrAdmin]
        private async Task RaidprotectEnableAsync()
        {
            try
            {
                var chnl = Context.Channel as ITextChannel;
                var permOverride = new OverwritePermissions();
                permOverride.Modify(sendMessages: PermValue.Deny);

                await chnl.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, permOverride);
                await Context.Channel.SendMessageAsync($"This channel is now silenced. {DiscordContext.GetEmoteReee()}");
            }
            catch (Exception ex)
            {
                throw ex; // todo
            }
        }

        [Command("raidprotect off")]
        [DevOrAdmin]
        private async Task RaidprotectDisableeAsync()
        {
            try
            {
                var chnl = Context.Channel as ITextChannel;
                var permOverride = new OverwritePermissions();
                permOverride.Modify(sendMessages: PermValue.Allow);

                await chnl.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, permOverride);

                await Context.Channel.SendMessageAsync($"The silence has been lifted. {DiscordContext.GetEmoteAyySeymour()}");
            }
            catch (Exception ex)
            {

                throw ex; // todo
            }
        }

    }
}
