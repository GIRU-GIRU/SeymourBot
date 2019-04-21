using Discord;
using Discord.WebSocket;
using OverseerBot.Caching.Images;
using System;
using System.Threading.Tasks;
using Toolbox.Config;
using Toolbox.DiscordUtilities;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OverseerBot.UserMessageLogging
{
    public static class MessageLogger
    {
        public static async Task EditedMessageEvent(Cacheable<IMessage, ulong> msgBefore, SocketMessage msgAfter, ISocketMessageChannel channel)
        {
            try
            {
                if (string.IsNullOrEmpty(msgAfter.Content)) return;
                if (msgBefore.Value.Content == msgAfter.Content) return;

                var logChannel = DiscordContext.GetDeletedMessageLog();
                var message = await msgBefore.GetOrDownloadAsync();

                List<byte[]> imagesData = null;
                bool hasImages = false;

                string time = msgAfter.Timestamp.DateTime.ToLongDateString() + " " + msgAfter.Timestamp.DateTime.ToLongTimeString();

                var embed = new EmbedBuilder();
                if (hasImages)
                {
                    if (imagesData != null)
                    {
                        embed.WithTitle($"✍️ {msgAfter.Author.Username}#{msgAfter.Author.Discriminator} edited messageID {message.Id} at {time} containing {imagesData.Count} image{((imagesData.Count > 1) ? "" : "s") }");
                    }
                    else
                    {
                        embed.WithTitle($"✍️ {msgAfter.Author.Username}#{msgAfter.Author.Discriminator} edited messageID {message.Id} at {time} containing images that weren't cached");
                    }
                }
                else
                {
                    embed.WithTitle($"🗑 {message.Author.Username}#{message.Author.Discriminator} deleted message in {message.Channel.Name}. UserID = {message.Author.Id}");
                }
                embed.WithTitle($"✍️ {msgAfter.Author.Username}#{msgAfter.Author.Discriminator} edited messageID {message.Id} at {time}");
                embed.WithDescription($"in #{channel.Name}, Original: " + msgBefore.Value.Content);
                embed.WithColor(new Color(250, 255, 0));

                await logChannel.SendMessageAsync("", false, embed.Build());
                if (imagesData != null)
                {
                    foreach (byte[] image in imagesData)
                    {
                        using (var ms = new MemoryStream(image))
                        {
                            await logChannel.SendFileAsync(ms, "cached.png");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;//todo
            }
        }

        public static async Task DeletedMessageEvent(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            try
            {
                var logChannel = DiscordContext.GetDeletedMessageLog();
                var message = await msg.GetOrDownloadAsync();
                var embed = new EmbedBuilder();
                List<CachedFile> filesData = null;
                bool hasImages = false;

                //if at least one image (embedded or attached) was in the deleted message, look for it in the cache
                if ((message.Embeds.Count > 0 && message.Embeds.Select(x => x.Image.GetValueOrDefault()).Count() > 0)
                    || (message.Attachments.Count > 0 && message.Attachments.Where(x => x.Url != null && x.Url.Count() > 0).Count() > 0))
                {
                    hasImages = true;
                    filesData = ImageCacheManager.FindImagesInCache(message.Id);
                }
                if (hasImages)
                {
                    if (filesData != null)
                    {
                        embed.WithTitle($"🗑 {message.Author.Username}#{message.Author.Discriminator} deleted message containing {filesData.Count} image{((filesData.Count > 1) ? "" : "s") } in {message.Channel.Name}. UserID = {message.Author.Id}");
                    }
                    else
                    {
                        embed.WithTitle($"🗑 {message.Author.Username}#{message.Author.Discriminator} deleted message containing images that weren't cached in {message.Channel.Name}. UserID = {message.Author.Id}");
                    }
                }
                else
                {
                    embed.WithTitle($"🗑 {message.Author.Username}#{message.Author.Discriminator} deleted message in {message.Channel.Name}. UserID = {message.Author.Id}");
                }
                embed.WithDescription(message.Content);
                embed.WithColor(new Color(255, 0, 0));

                await logChannel.SendMessageAsync("", false, embed.Build());
                if (filesData != null)
                {
                    foreach (var file in filesData)
                    {
                        using (var ms = new MemoryStream(file.File))
                        {
                            await logChannel.SendFileAsync(ms, file.FileName);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex; //todo
            }
        }

        public static async Task ReceivedMessageEvent(SocketMessage arg)
        {
            if (arg.Channel.Id != DiscordContext.GetLoggingChannel().Id) //avoid caching images the bot is posting
            {
                var names = new List<string>();
                var result = GetImageUrls(arg, out names);
                if (result != null)
                {
                    await ImageCacheManager.CacheImageAsync(result, names, arg.Id);
                }
            }
        }

        //checks if message contains any images
        private static List<string> GetImageUrls(IMessage message, out List<string> imageNames)
        {
            imageNames = new List<string>();
            List<string> imageUrls = null;
            if (message.Embeds.Count > 0)
            {
                imageUrls = new List<string>();
                var images = message.Embeds.Where(x => x.Url != null && x.Url.Count() > 0);
                var name = "";
                foreach (var image in images)
                {
                    name = "embed";
                    switch (image.Type)
                    {
                        case EmbedType.Gifv:
                            name += ".mp4";
                            imageUrls.Add(image.Url);
                            imageNames.Add(name);
                            break;
                        case EmbedType.Video:
                            name += ".mp4";
                            imageUrls.Add(image.Url);
                            imageNames.Add(name);
                            break;
                        case EmbedType.Image:
                            name += ".png";
                            imageUrls.Add(image.Url);
                            imageNames.Add(name);
                            break;
                    }
                }
            }
            if (message.Attachments.Count > 0)
            {
                var images = message.Attachments.Where(x => x.Url != null && x.Url.Count() > 0);
                if (imageUrls == null)
                {
                    imageUrls = new List<string>();
                }
                foreach (var image in images)
                {
                    imageUrls.Add(image.Url);
                    imageNames.Add(image.Filename);
                }
            }
            return imageUrls;
        }
    }
}
