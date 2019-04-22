using Discord.Commands;
using SeymourBot.Attributes;
using SeymourBot.AutoModeration;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace SeymourBot.Modules.ConfigurationCommands
{
    public class FilterCommands : ModuleBase<SocketCommandContext>
    {
        [Command("addmultiplefilters")]
        [Alias("addfilters")]
        [DevOrAdmin]
        private async Task AddMultipleFiltersAsync([Remainder]string words)
        {
            try
            {
                string[] splitWords = words.Split(' ');

                foreach (string splitword in splitWords)
                {
                    await AutoModeratorManager.AddBannedWordAsync(new ModeratedElement() { Dialog = "", Pattern = splitword.ToLower().Trim() });
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FilterException, ex);
            }
        }

        [Command("addfilter")]
        [Alias("addfilter")]
        [DevOrAdmin]
        private async Task AddFilterAsync(string word)
        {
            try
            {
                await AutoModeratorManager.AddBannedWordAsync(new ModeratedElement() { Dialog = "", Pattern = word.ToLower().Trim() });
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FilterException, ex);
            }
        }


        [Command("addcustomfilter")]
        [DevOrAdmin]
        private async Task AddCustomFilterAsync([Remainder]string words)
        {
            try
            {
                if (!words.Contains('|'))
                {
                    string cmdPrefix = ConfigManager.GetProperty(PropertyItem.CommandPrefix);
                    await Context.Channel.SendMessageAsync($"Invalid syntax - must be {cmdPrefix}addcustomfilter word | custom message when automoderated");
                }
                string pattern = words.Substring(0, words.IndexOf('|')).Trim().ToLower();
                string message = words.Substring(words.IndexOf('|') + 1).Trim();
                await AutoModeratorManager.AddBannedWordAsync(new ModeratedElement() { Dialog = message, Pattern = pattern });
            }
            catch (Exception ex)
            {
                throw ex;
                //todo
            }
        }

        [Command("removecustomfilter")]
        [Alias("removefilter")]
        [DevOrAdmin]
        private async Task RemoveFilterAsync(string name)
        {
            try
            {

                bool existing = await AutoModeratorManager.RemoveBannedWordAsync(name);

                if (existing)
                {
                    await Context.Channel.SendMessageAsync($"Succesfully removed \"{name}\" as a banned word");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Was unable to locate \"{name}\" as a banned word");
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //todo
            }
        }
    }
}
