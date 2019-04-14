using Discord;
using Discord.Commands;
using SeymourBot.DiscordUtilities;
using SeymourBot.Storage.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.Utils
{
    public static class Utils
    {
        public static Embed BuildDefaultEmbed(DisciplinaryEventEnum eventType, SocketCommandContext context, TimeSpan timeSpan, string reason, string targetName)
        {
            try
            {
                switch (eventType)
                {
                    case DisciplinaryEventEnum.KickEvent:
                        return GenerateKickEmbed(eventType, context, reason, targetName);
                    case DisciplinaryEventEnum.BanEvent:
                        break;
                    case DisciplinaryEventEnum.BanCleanseEvent:
                        break;
                    case DisciplinaryEventEnum.MuteEvent:
                        break;
                    case DisciplinaryEventEnum.LimitedUserEvent:
                        break;
                    case DisciplinaryEventEnum.WarnEvent:
                        break;
                    default:
                        break;
                }

                return GenerateDefaultEmbed(eventType, context, timeSpan, reason, targetName);
            }
            catch (Exception ex)
            {
                throw ex; // todo
            }
        }

        private static Embed GenerateDefaultEmbed(DisciplinaryEventEnum eventType, SocketCommandContext context, TimeSpan timeSpan, string reason, string targetName)
        {
            try
            {
                string commandName = ReturnEventTypeString(eventType);
                string duration = "Permanent.";

                if (timeSpan.TotalDays > 0)
                {
                    duration = $"{Math.Round(timeSpan.TotalDays, 2)} day{SAppend(timeSpan.TotalDays)}.";
                }
                else if (timeSpan.TotalHours > 0)
                {
                    duration = $"{Math.Round(timeSpan.TotalMinutes, 2)} hour{SAppend(timeSpan.TotalHours)}.";
                }
                else if (timeSpan.TotalMinutes > 0)
                {
                    duration = $"{Math.Round(timeSpan.TotalMinutes, 2)} min{SAppend(timeSpan.TotalMinutes)}.";
                }

                Emote emote = duration == "Permanent" ? DiscordContext.GetEmoteReee() : DiscordContext.GetEmoteAyySeymour();

                var embed = new EmbedBuilder();
                embed.WithTitle($"{context.User.Username} {commandName} {targetName} {emote.ToString()}");
                embed.WithDescription($"Reason: {reason}\nDuration: {duration}");
                embed.WithColor(new Color(255, 0, 0));

                return embed.Build();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static Embed GenerateKickEmbed(DisciplinaryEventEnum eventType, SocketCommandContext context, string reason, string targetName)
        {
            try
            {
                var seymourEmote = DiscordContext.GetEmoteAyySeymour();

                var embed = new EmbedBuilder();
                embed.WithTitle($"{context.User.Username} booted {targetName} {seymourEmote.ToString()} ");
                embed.WithDescription($"reason: {reason}");
                embed.WithColor(new Color(255, 0, 0));

                return embed.Build();
            }
            catch (Exception ex)
            {
                throw ex;  //todo
            }
        }


        private static string ReturnEventTypeString(DisciplinaryEventEnum eventType)
        {
            switch (eventType)
            {
                case DisciplinaryEventEnum.BanEvent:
                    return "banned";
                case DisciplinaryEventEnum.BanCleanseEvent:
                    return "bancleansed";
                case DisciplinaryEventEnum.KickEvent:
                    return "kicked";
                case DisciplinaryEventEnum.MuteEvent:
                    return "muted";
                case DisciplinaryEventEnum.LimitedUserEvent:
                    return "limited";
                case DisciplinaryEventEnum.WarnEvent:
                    return "warned";
                default:
                    return "disciplined";
            }
        }


        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static string SAppend(int amount)
        {
            if (amount > 1)
            {
                return "s";
            }
            else
            {
                return string.Empty;
            }
        }

        public static string SAppend(double amount)
        {
            if (amount > 1)
            {
                return "s";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
