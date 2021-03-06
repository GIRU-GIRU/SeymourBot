﻿using Discord;
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
using Toolbox.Exceptions;
using Toolbox.Resources;
using Toolbox.Utils;

namespace SeymourBot.Modules
{
    public class Info : ModuleBase<SocketCommandContext>
    {
        ImageFormat png = ImageFormat.Png;
        [Command("userinfo")]
        [Alias("info")]
        [DevOrAdmin]
        private async Task GetInfoAsync(SocketGuildUser user)
        {
            try
            {
                var currentMuteTime = await StorageManager.GetActiveMuteAsync(user);
                string userAvatarURL = user.GetAvatarUrl(png, 1024);
                string userStatus = user.Status.ToString();
                var userCreatedAtString = user.CreatedAt.ToString("yyyy/MM/dd hh:mm");
                string userJoinedAtString = "ERR";
                if (user.JoinedAt.HasValue) //todo attempt to fix the date issue, seems to be similar to this https://stackoverflow.com/questions/1896185/nullable-object-must-have-a-value
                {
                    userJoinedAtString = user.JoinedAt.Value.ToString("yyyy/MM/dd hh:mm");
                }
                var userDiscriminator = user.Discriminator;
                string userActivity = user.Activity == null ? "nothing" : user.Activity.Name;

                var embed = new EmbedBuilder();
                embed.WithTitle($"{user.Username}{userDiscriminator}");
                embed.AddField("Account Created: ", userCreatedAtString, true);
                embed.AddField("Joined Mordhau Guild: ", userJoinedAtString, true);
                embed.AddField("User ID: ", user.Id, false);
                embed.AddField("Currently playing ", userActivity, true);
                embed.AddField("Status: ", userStatus, true);
                embed.AddField("Muted: ", currentMuteTime.TotalMilliseconds == 0 ? "Not Currently Muted" : $"Currently muted for {Utilities.ShortTimeSpanFormatting(currentMuteTime)}", true);
                embed.WithThumbnailUrl(userAvatarURL);
                embed.WithColor(new Color(0, 204, 255));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UserinfoException, ex);
            }
        }

        [Command("userinfo")]
        [Alias("info")]
        [DevOrAdmin]
        private async Task GetInfoAsync(ulong userID)
        {
            try
            {
                var user = Context.Guild.GetUser(userID);

                if (user == null)
                {
                    await Context.Channel.SendMessageAsync("Unable to locate that user");
                    return;
                }
                var currentMuteTime = await StorageManager.GetActiveMuteAsync(user);
                string userAvatarURL = user.GetAvatarUrl(png, 1024);
                string userStatus = user.Status.ToString();
                var userCreatedAtString = user.CreatedAt.ToString("yyyy/MM/dd hh:mm");
                string userJoinedAtString = "ERR";
                if (user.JoinedAt.HasValue) //todo attempt to fix the date issue, seems to be similar to this https://stackoverflow.com/questions/1896185/nullable-object-must-have-a-value
                {
                    userJoinedAtString = user.JoinedAt.Value.ToString("yyyy/MM/dd hh:mm");
                }
                var userDiscriminator = user.Discriminator;
                string userActivity = user.Activity == null ? "nothing" : user.Activity.Name;

                var embed = new EmbedBuilder();
                embed.WithTitle($"{user.Username}{userDiscriminator}");
                embed.AddField("Account Created: ", userCreatedAtString, true);
                embed.AddField("Joined Mordhau Guild: ", userJoinedAtString, true);
                embed.AddField("User ID: ", user.Id, false);
                embed.AddField("Currently playing ", userActivity, true);
                embed.AddField("Status: ", userStatus, true);
                embed.AddField("Muted: ", currentMuteTime.TotalMilliseconds == 0 ? "Not Currently Muted" : $"Currently muted for {Utilities.ShortTimeSpanFormatting(currentMuteTime)}", true);
                embed.WithThumbnailUrl(userAvatarURL);
                embed.WithColor(new Color(0, 204, 255));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UserinfoException, ex);
            }
        }

        [Command("userinfo")]
        [Alias("info")]
        [DevOrAdmin]
        private async Task GetInfoAsync()
        {
            try
            {
                var user = Context.Message.Author as SocketGuildUser;

                string userAvatarURL = user.GetAvatarUrl(png, 1024);
                string userStatus = user.Status.ToString();
                var currentMuteTime = await StorageManager.GetActiveMuteAsync(user);
                var userCreatedAtString = user.CreatedAt.ToString("yyyy/MM/dd hh:mm");
                string userJoinedAtString = "ERR";
                if (user.JoinedAt.HasValue) //todo attempt to fix the date issue, seems to be similar to this https://stackoverflow.com/questions/1896185/nullable-object-must-have-a-value
                {
                    userJoinedAtString = user.JoinedAt.Value.ToString("yyyy/MM/dd hh:mm");
                }
                var userDiscriminator = user.Discriminator;
                string userActivity = user.Activity == null ? "nothing" : user.Activity.Name;

                var embed = new EmbedBuilder();
                embed.WithTitle($"{user.Username}{userDiscriminator}");
                embed.AddField("Account Created: ", userCreatedAtString, true);
                embed.AddField("Joined Mordhau Guild: ", userJoinedAtString, true);
                embed.AddField("User ID: ", user.Id, false);
                embed.AddField("Currently playing: ", userActivity, true);
                embed.AddField("Status: ", userStatus, true);
                embed.AddField("Muted: ", currentMuteTime.TotalMilliseconds == 0 ? "Not Currently Muted" : $"Currently muted for {Utilities.ShortTimeSpanFormatting(currentMuteTime)}", true);
                embed.WithThumbnailUrl(userAvatarURL);
                embed.WithColor(new Color(0, 204, 255));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UserinfoException, ex);
            }
        }

        [Command("avatar")]
        [DevOrAdmin]
        private async Task PullAvatarAsync(IGuildUser user)
        {
            try
            {
                string avatarURL = user.GetAvatarUrl(ImageFormat.Auto, 1024);

                if (avatarURL is null)
                {
                    await Context.Message.Channel.SendMessageAsync($"{user.Mention} does not have a profile picture");
                    return;
                }
                var embed = new EmbedBuilder();
                embed.WithColor(new Color(0, 204, 255));
                embed.WithTitle($"{user.Username}'s avatar");
                embed.WithUrl(avatarURL);
                embed.WithImageUrl(avatarURL);

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.UserinfoException, ex);
            }
        }

        [Command("disciplinaries")]
        [DevOrAdmin]
        private async Task GetUserDisciplinariesAsync(SocketGuildUser user)
        {
            try
            {
                var disciplinaries = await StorageManager.GetDisciplinariesAsync(user);
                var linesPerEmbed = 10;
                EmbedBuilder embed;

                if (disciplinaries.Keys.Count > linesPerEmbed)
                {
                    List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                    int i = 0;
                    while (i < disciplinaries.Keys.Count)
                    {
                        result.Add(new Dictionary<string, string>());
                        for (int n = 0; n < linesPerEmbed && i < disciplinaries.Keys.Count; n++, i++)
                        {
                            result[i / linesPerEmbed].Add(disciplinaries.Keys.ElementAt(i), disciplinaries[disciplinaries.Keys.ElementAt(i)]);
                        }
                    }
                    i = 1;
                    foreach (Dictionary<string, string> temp in result)
                    {
                        embed = new EmbedBuilder();
                        embed.WithTitle($"Disciplinaries for \"{user.Username}#{user.Discriminator}\" page {i}/{result.Count}");
                        embed.AddField("Date", String.Join("\n", temp.Keys), true);
                        embed.AddField("Type and Reason: ", String.Join("\n", temp.Values), true);

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                        i++;
                    }
                }
                else if (disciplinaries.Keys.Count > 0)
                {
                    embed = new EmbedBuilder();
                    embed.WithTitle($"Disciplinaries for \"{user.Username}#{user.Discriminator}\" ");
                    embed.AddField("Date", String.Join("\n", disciplinaries.Keys), true);
                    embed.AddField("Type and Reason: ", String.Join("\n", disciplinaries.Values), true);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Could not find any disciplinaries for this user");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.DisciplinariesException, ex);
            }
        }

        [Command("disciplinaries")]
        [DevOrAdmin]
        private async Task GetUserDisciplinariesAsync(SocketGuildUser user, TimeSpan time)
        {
            try
            {
                var disciplinaries = await StorageManager.GetDisciplinariesAsync(user, time);
                var linesPerEmbed = 10;
                EmbedBuilder embed;

                if (disciplinaries.Keys.Count > linesPerEmbed)
                {
                    List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                    int i = 0;
                    while (i < disciplinaries.Keys.Count)
                    {
                        result.Add(new Dictionary<string, string>());
                        for (int n = 0; n < linesPerEmbed && i < disciplinaries.Keys.Count; n++, i++)
                        {
                            result[i / linesPerEmbed].Add(disciplinaries.Keys.ElementAt(i), disciplinaries[disciplinaries.Keys.ElementAt(i)]);
                        }
                    }
                    i = 1;
                    foreach (Dictionary<string, string> temp in result)
                    {
                        embed = new EmbedBuilder();
                        embed.WithTitle($"Disciplinaries for \"{user.Username}#{user.Discriminator}\" page {i}/{result.Count}");
                        embed.AddField("Date", String.Join("\n", temp.Keys), true);
                        embed.AddField("Type and Reason: ", String.Join("\n", temp.Values), true);

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                        i++;
                    }
                }
                else if (disciplinaries.Keys.Count > 0)
                {
                    embed = new EmbedBuilder();
                    embed.WithTitle($"Disciplinaries for \"{user.Username}#{user.Discriminator}\" ");
                    embed.AddField("Date", String.Join("\n", disciplinaries.Keys), true);
                    embed.AddField("Type and Reason: ", String.Join("\n", disciplinaries.Values), true);

                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Could not find any disciplinaries for this user");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.DisciplinariesException, ex);
            }
        }

        [Command("peasantry")]
        [Ratelimit(3, 10, Measure.Minutes)]
        private async Task GetUsersWithPeasantRoleAsync()
        {
            try
            {
                var gateChannelUsers = Context.Guild.GetTextChannel(ConfigManager.GetUlongProperty(PropertyItem.Channel_NoobGate)).Users;
                var peasantRoleID = ConfigManager.GetUlongProperty(PropertyItem.Role_Peasant);

                int count = 0;
                foreach (var user in gateChannelUsers)
                {
                    if (user.Roles.Any(x => x.Id == peasantRoleID))
                    {
                        count++;
                    }
                }

                await Context.Channel.SendMessageAsync($"There are currently {count} members of the local peasantry");

            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.DisciplinariesException, ex);
            }
        }
    }
}
