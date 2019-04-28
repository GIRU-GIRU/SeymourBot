using Discord;
using Discord.Commands;
using SeymourBot.Attributes;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;
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
                var permOverride = new OverwritePermissions(sendMessages: PermValue.Deny);

                await Context.Channel.SendMessageAsync($"This channel is now silenced. {DiscordContextSeymour.GetEmoteReee()}");
                await chnl.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, permOverride);
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.RaidprotectException, ex);
            }
        }

        [Command("raidprotect off")]
        [DevOrAdmin]
        private async Task RaidprotectDisableeAsync()
        {
            try
            {
                var chnl = Context.Channel as ITextChannel;
                var permOverride = new OverwritePermissions(sendMessages: PermValue.Allow);
                await chnl.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, permOverride);

                await Context.Channel.SendMessageAsync($"The silence has been lifted. {DiscordContextSeymour.GetEmoteAyySeymour()}");
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.RaidprotectException, ex);
            }
        }

    }
}
