using Discord;
using Discord.WebSocket;
using OverseerBot.Caching.Images;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;
using Toolbox.Exceptions;
using Toolbox.Resources;

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

                var logChannel = DiscordContextOverseer.GetDeletedMessageLog();
                var message = await msgBefore.GetOrDownloadAsync();

                if (message != null)
                {
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
            }
            catch (System.Exception ex)
            {
                await ExceptionManager.LogExceptionAsync(ErrMessages.EditedMessageException);
            }
        }

        public static async Task DeletedMessageEvent(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            try
            {
                var logChannel = DiscordContextOverseer.GetDeletedMessageLog();
                var message = await msg.GetOrDownloadAsync();

                if (message != null)
                {
                    var embed = new EmbedBuilder();
                    List<CachedFile> filesData = null;
                    bool hasImages = false;

                    //if at least one image (embedded or attached) was in the deleted message, look for it in the cache
                    bool imageExists = CheckForImageWithinEmbeds(message);
                    bool hasURLs = CheckForURLs(message);
                    if ((message.Embeds.Count > 0 && imageExists) || (message.Attachments.Count > 0 && hasURLs))
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
                    else //it's just a message
                    {

                        embed.WithTitle($"🗑 {message.Author.Username}#{message.Author.Discriminator} deleted message in {message.Channel.Name}. UserID = {message.Author.Id}");
                    }

                    //check for other types of embeds
                    string extraMessageContent = SearchForOtherEmbedTypes(message);

                    embed.WithDescription(message.Content + extraMessageContent);
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
            }
            catch (System.Exception ex)
            {
                await ExceptionManager.LogExceptionAsync(ErrMessages.DeletedMessageException, ex);
            }
        }

        private static string SearchForOtherEmbedTypes(IMessage message)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (message.Embeds.Any(x => x.Type == EmbedType.Rich))
                {
                    var msgOtherEmbeds = message.Embeds.Where(x => x.Type == EmbedType.Article ||
                                                                      x.Type == EmbedType.Tweet ||
                                                                       x.Type == EmbedType.Link).ToList();
                    foreach (var item in msgOtherEmbeds)
                    {
                        if (string.IsNullOrEmpty(item.Url))
                        {
                            stringBuilder.AppendLine("URL not found" + "\n");
                        }
                        else
                        {
                            stringBuilder.AppendLine(item.Url + "\n");
                        }
                    }
                }

                if (stringBuilder.Length > 0)
                {
                    return stringBuilder.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(, ex);
            }
        }

        private static bool CheckForURLs(IMessage message)
        {
            try
            {
                return message.Attachments.Where(x => x.Url != null && x.Url.Count() > 0).Count() > 0;
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }
        }

        private static bool CheckForImageWithinEmbeds(IMessage message)
        {
            try
            {
                return message.Embeds.Select(x => x.Type != EmbedType.Rich).Any();
            }
            catch (Exception ex)
            {
                throw ex; //todo
            }

        }

        public static async Task ReceivedMessageEvent(SocketMessage arg)
        {
            if (arg.Channel.Id != DiscordContextOverseer.GetLoggingChannel().Id) //avoid caching images the bot is posting
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
