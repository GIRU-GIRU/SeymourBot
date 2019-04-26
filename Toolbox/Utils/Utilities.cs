using Discord;
using Discord.Commands;
using SeymourBot.Storage.User;
using System;
using Toolbox.DiscordUtilities;

namespace Toolbox.Utils
{
    public static class Utilities
    {
        public static Embed BuildDefaultEmbed(DisciplinaryEventEnum eventType,
                                                 TimeSpan timeSpan,
                                                   string reason,
                                                      string targetName,
                                                          bool existing,
                                                             string author = "Seymour")
        {
            try
            {
                switch (eventType)
                {
                    case DisciplinaryEventEnum.KickEvent:
                        return GenerateKickEmbed(eventType, author, reason, targetName);
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

                return GenerateDefaultEmbed(eventType, timeSpan, reason, targetName, existing, author);
            }
            catch (Exception ex)
            {
                throw ex; // todo
            }
        }

        private static Embed GenerateDefaultEmbed(DisciplinaryEventEnum eventType,TimeSpan timeSpan, string reason, string targetName, bool existing, string author)
        {
            try
            {
                string commandName = ReturnEventTypeString(eventType);
                string duration = "Permanent.";

                if (timeSpan.TotalDays >= 1)
                {
                    duration = $"{Math.Round(timeSpan.TotalDays, 2)} day{SAppend(timeSpan.TotalDays)}.";
                }
                else if (timeSpan.TotalHours >= 1)
                {
                    duration = $"{Math.Round(timeSpan.TotalHours, 2)} hour{SAppend(timeSpan.TotalHours)}.";
                }
                else if (timeSpan.TotalMinutes >= 1)
                {
                    duration = $"{Math.Round(timeSpan.TotalMinutes, 2)} min{SAppend(timeSpan.TotalMinutes)}.";
                }
                else if (timeSpan.TotalSeconds >= 1)
                {
                    duration = $"{Math.Round(timeSpan.TotalSeconds, 2)} sec{SAppend(timeSpan.TotalSeconds)}.";
                }

                Emote emote = duration == "Permanent" ? DiscordContextSeymour.GetEmoteReee() : DiscordContextSeymour.GetEmoteAyySeymour();

                string existingDisciplinary = String.Empty;
                if (existing)
                {
                    existingDisciplinary = " updated to";
                }

                var embed = new EmbedBuilder();
                embed.WithTitle($"{author} {commandName} {targetName} {emote.ToString()}");
                embed.WithDescription($"Reason: {reason}\nDuration{existingDisciplinary}: {duration}");
                embed.WithColor(new Color(255, 0, 0));

                return embed.Build();
            }
            catch (Exception ex)
            {
                throw ex; // todo
            }
        }
        public static Embed BuildBlacklistEmbed(TimeSpan timeSpan, string username, bool existing, string author = "SeymourBot")
        {
            try
            {
                string commandName = "blacklisted";
                string duration = "1 month";

                if (timeSpan.TotalDays >= 1)
                {
                    duration = $"{Math.Round(timeSpan.TotalDays, 2)} day{SAppend(timeSpan.TotalDays)}.";
                }
                else if (timeSpan.TotalHours >= 1)
                {
                    duration = $"{Math.Round(timeSpan.TotalHours , 2)} hour{SAppend(timeSpan.TotalHours)}.";
                }
                else if (timeSpan.TotalMinutes >= 1)
                {
                    duration = $"{Math.Round(timeSpan.TotalMinutes, 2)} min{SAppend(timeSpan.TotalMinutes)}.";
                }

                Emote emote = duration == "Permanent" ? DiscordContextSeymour.GetEmoteReee() : DiscordContextSeymour.GetEmoteAyySeymour();

                string existingDisciplinary = String.Empty;
                if (existing)
                {
                    existingDisciplinary = " updated to";
                }

                var embed = new EmbedBuilder();
                embed.WithTitle($"{author} {commandName} {username} {emote.ToString()}");
                embed.WithDescription($"Duration{existingDisciplinary}: {duration}");
                embed.WithColor(new Color(255, 0, 0));

                return embed.Build();
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        private static Embed GenerateKickEmbed(DisciplinaryEventEnum eventType, string author, string reason, string targetName)
        {
            try
            {
                var seymourEmote = DiscordContextSeymour.GetEmoteAyySeymour();

                var embed = new EmbedBuilder();
                embed.WithTitle($"{author} booted {targetName} {seymourEmote.ToString()} ");
                embed.WithDescription($"reason: {reason}");
                embed.WithColor(new Color(255, 0, 0));

                return embed.Build();
            }
            catch (Exception ex)
            {
                throw ex;  //todo
            }
        }

        public static Embed BuildRemoveDisciplinaryEmbed(string action, string targetName)
        {
            try
            {
                var seymourEmote = DiscordContextSeymour.GetEmoteAyySeymour();

                var embed = new EmbedBuilder();
                embed.WithTitle($"{action} {targetName} {seymourEmote.ToString()} ");
                embed.WithColor(new Color(0, 255, 0));

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

        private static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private static string SAppend(int amount)
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

        private static string SAppend(double amount)
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
