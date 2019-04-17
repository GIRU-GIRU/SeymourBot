﻿using Discord.Commands;
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

        [Command("addcustomfilter")]
        [DevOrAdmin]
        private async Task AddCustomFilter([Remainder]string words)
        {
            try
            {
                if (!words.Contains('|'))
                {
                    string cmdPrefix = ConfigManager.GetProperty(PropertyItem.CommandPrefix);
                    await Context.Channel.SendMessageAsync($"Invalid syntax - must be {cmdPrefix}addcustomfilter word | custom message when automoderated");
                }
                string pattern = words.Substring(0, words.IndexOf('|'));
                string message = words.Substring(words.IndexOf('|'));
                await AutoModeratorManager.AddBannedWord(new ModeratedElement() { Dialog = message, Pattern = pattern.ToLower() });
            }
            catch (Exception ex)
            {
                throw ex;
                //todo
            }
        }
    }
}
