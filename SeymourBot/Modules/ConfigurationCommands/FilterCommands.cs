using Discord;
using Discord.Commands;
using SeymourBot.Attributes;
using SeymourBot.AutoModeration;
using SeymourBot.DataAccess.StorageManager;
using SeymourBot.Modules.CommandUtils;
using System;
using System.Collections.Generic;
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
                if (words.Contains(' '))
                {
                    string[] splitWords = words.Split(' ');
                    var list = new List<string>();
                    bool existing;
                    string wordsToAdd = "";

                    foreach (string splitword in splitWords)
                    {
                        wordsToAdd = splitword.ToLower().Trim();
                        existing = await AutoModeratorManager.AddBannedWordAsync(new ModeratedElement() { Dialog = "", Pattern = wordsToAdd });
                        if (existing) list.Add(wordsToAdd);
                    }

                    if (list.Count > 0)
                    {
                        var succesfullyAddedWords = string.Join(", ", list.ToArray());
                        await Context.Channel.SendMessageAsync($"Succesfully added \"{succesfullyAddedWords}\" as a banned word");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync($"Filters already exist for those words");
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Use {ConfigManager.GetProperty(PropertyItem.CommandPrefix)}addfilter for single filters.");
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
                bool existing = await AutoModeratorManager.AddBannedWordAsync(new ModeratedElement() { Dialog = "", Pattern = word.ToLower().Trim() });

                if (existing)
                {
                    await Context.Channel.SendMessageAsync($"Succesfully added \"{word}\" as a banned word");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"\"{word}\" is an already existing banned word");
                }
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
                string message = words.Substring(words.IndexOf('|') + 2).Trim();
                bool existing = await AutoModeratorManager.AddBannedWordAsync(new ModeratedElement() { Dialog = message, Pattern = pattern });

                if (existing)
                {
                    await Context.Channel.SendMessageAsync($"Succesfully added \"{pattern}\" as a banned word");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"\"{pattern}\" is an already existing banned word");
                }
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

        [Command("filterlist")]
        [DevOrAdmin]
        private async Task ListFilterCommandsAsync()
        {
            try
            {
                List<string> filters = StorageManager.GetFilters();
                var embed = new EmbedBuilder();
                embed.WithTitle("Currently Active Filters");
                
                embed.AddField("Banned word", string.Join("\n", filters), false);
                
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.InfoCommandException, ex);
            }

        }
    }
}
