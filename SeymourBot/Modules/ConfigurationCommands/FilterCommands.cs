using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
using SeymourBot.AutoModeration;
using SeymourBot.Config;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.DiscordUtilities;
using SeymourBot.Exceptions;
using SeymourBot.Modules.CommandUtils;
using SeymourBot.Resources;
using SeymourBot.Storage;
using SeymourBot.Storage.User;
using SeymourBot.TimedEvent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace SeymourBot.Modules.ConfigurationCommands
{
    public class FilterCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Filter")]
        [DevOrAdmin]
        private async Task AddFilterAsync([Remainder]string words)
        {
            try
            {
                string[] splitWords = words.Split(' ');

                foreach (string splitword in splitWords)
                {
                    await AutoModeratorManager.AddBannedWord(new ModeratedElement() { Dialog = "", Pattern = splitword.ToLower() });
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FilterException, ex);
            }
        }

        [Command("addFilterWithMessage")]
        [DevOrAdmin]
        private async Task AddFilterWithMessageAsync([Remainder]string words)
        {
            try
            {
                string pattern = words.Substring(0, words.IndexOf(' '));
                string message = words.Substring(words.IndexOf(' '));
                await AutoModeratorManager.AddBannedWord(new ModeratedElement() { Dialog = message, Pattern = pattern.ToLower() });
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FilterException, ex);
            }
        }
    }
}
