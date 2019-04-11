using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SeymourBot.Attributes;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SeymourBot.Modules.DisciplinaryCommands
{
    public class Eviction : ModuleBase<SocketCommandContext>
    {
        private DateTime _disciplineDuration;
        private string _reason;
        private static Regex durationDaysMeasurementRegex = new Regex("([1-9])d+");
        private static Regex durationHoursMeasurementRegex = new Regex("([1-9])h+");

        //needs big refactor dont touch, working though

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [DevOrAdmin]
        private async Task BanUser(SocketGuildUser user, [Remainder]string reason = "no reason specified")
        {
            try
            {
                if (!TryProcessBanParams(reason))
                {
                    await Context.Channel.SendMessageAsync("niga wat THEJ FAK THAT SUPPSOED TO BE");
                    return;
                }

                string kickTargetName = user.Username;
                await Context.Channel.SendMessageAsync("", false, BuildBanEmbed(Context, kickTargetName));


                UserDisciplinaryEventStorage obj = new UserDisciplinaryEventStorage()
                {
                    DateInserted = DateTime.Now,
                    DateToRemove = _disciplineDuration,
                    DiscipinaryEventType = DisciplinaryEventEnum.BanEvent,
                    DisciplineEventID = (ulong)DateTime.Now.Millisecond,
                    ModeratorID = Context.Message.Author.Id,
                    Reason = reason,
                    UserID = Context.Message.Author.Id
                };
                UserStorage newUser = new UserStorage()
                {
                    UserID = Context.Message.Author.Id,
                    UserName = Context.Message.Author.Username
                };

                await TimedEventManager.CreateEvent(obj, newUser);
            }
            catch (Exception ex)
            {

                throw ex; //todo
            }
        }

        private Embed BuildBanEmbed(SocketCommandContext context, string kickTargetName)
        {
            var calcTime = _disciplineDuration - DateTime.Now;
            var seymourEmote = DiscordContext.GetEmote("ayyseymour");//todo
            string banLength = "";

            if (_disciplineDuration == DateTime.MinValue)
            {
                banLength = "Permanent.";
            }
            else if (calcTime.TotalDays >= 1)
            {
                banLength = $"for {calcTime.Duration().TotalDays} days.";
            }
            else if (calcTime.TotalHours >= 1)
            {
                banLength = $"for {calcTime.Duration().TotalHours} hours.";
            }
            else
            {
                banLength = $"for {calcTime.Duration().TotalMinutes} minutes.";
            }

            var embed = new EmbedBuilder();
            embed.WithTitle($"{Context.User.Username} banned {kickTargetName} {seymourEmote.ToString()} ");
            embed.WithDescription($"Reason: {_reason}\nDuration: {banLength}");
            embed.WithColor(new Color(255, 0, 0));

            return embed.Build();
        }

        private bool TryProcessBanParams(string reasonInput)
        {
            try
            {
                if (reasonInput == "no reason specified")
                {
                    _reason = reasonInput;
                    return true;
                }
                else
                {
                    _reason = "test";//string.Join(' ', reasonArray.Skip(1));
                }

                string[] reasonArray = reasonInput.Split(' ');
                string banLength = reasonArray[0].ToLower();

                //minutes
                if (Utils.Utils.IsDigitsOnly(banLength))
                {
                    int.TryParse(banLength, out int result);
                    _disciplineDuration = new DateTime().AddMinutes(result);
                }
                //hours
                else if (durationHoursMeasurementRegex.Match(banLength).Success)
                {
                    banLength = banLength.Replace("h", string.Empty);
                    int.TryParse(banLength, out int result);
                    _disciplineDuration = DateTime.Now.AddDays(result);
                }
                //days
                else if (durationDaysMeasurementRegex.Match(banLength).Success)
                {
                    banLength = banLength.Replace("d", string.Empty);
                    int.TryParse(banLength, out int result);
                    _disciplineDuration = DateTime.Now.AddDays(result);
                }
                else
                {
                    return false;
                }


                return true;

            }
            catch (Exception ex)
            {

                throw ex; //todo
            }

        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [DevOrAdmin]
        private async Task KickUser(SocketGuildUser user, [Remainder]string reason = "No reason specified")
        {
            try
            {
                await user.KickAsync();
                string kickTargetName = user.Username;

                var seymourEmote = DiscordContext.GetEmote("ayyseymour");//todo

                var embed = new EmbedBuilder();
                embed.WithTitle($"{Context.User.Username} booted {kickTargetName} {seymourEmote.ToString()} ");
                embed.WithDescription($"reason: {reason}");
                embed.WithColor(new Color(255, 0, 0));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            catch (Exception ex)
            {

                throw ex; //todo
            }
        }

    }
}
