using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Config;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Discord;
using System.IO.Compression;
using Toolbox.DiscordUtilities;

namespace OverseerBot.Caching.Images
{
    public static class ImageCacheManager
    {
        private static ImageCache cache;

        static ImageCacheManager()
        {
            cache = new ImageCache(ConfigManager.GetIntegerProperty(PropertyItem.ImageCacheSize));
        }

        public static async Task CacheImageAsync(List<string> urls, List<string> fileNames, ulong messageId)
        {
            if (fileNames.Count == urls.Count)
            {
                CachedElement result = new CachedElement() { MessageId = messageId };
                for (int i = 0; i < fileNames.Count; i++)
                {
                    result.Files.Add(new CachedFile() { File = ConvertImageTo64(await DownloadImageAsync(urls[i])), FileName = fileNames[i] });
                }
                cache.AddImageInCache(result);
            }
        }

        public static List<CachedFile> FindImagesInCache(ulong messageId)
        {
            List<CachedFile> result = null;
            var retrievedElement = cache.FindImage(messageId);
            if (retrievedElement != null)
            {
                var retrievedFiles = retrievedElement.Files;
                result = new List<CachedFile>();
                foreach (var file in retrievedFiles)
                {
                    if (file != null)
                    {
                        result.Add(new CachedFile() { File = ConvertImageFrom64(file.File), FileName = file.FileName });
                    }
                }
            }
            return result;
        }

        //--------- Conversion methods ---------

        private static byte[] ConvertImageTo64(byte[] image)
        {
            string base64 = Convert.ToBase64String(image);

            return Zip(base64);
        }

        private static byte[] ConvertImageFrom64(byte[] image)
        {
            string unzipped = Unzip(image);

            return Convert.FromBase64String(unzipped);
        }

        private static async Task<byte[]> DownloadImageAsync(string url)
        {
            byte[] result = null;
            using (var ws = new WebClient())
            {
                result = await ws.DownloadDataTaskAsync(url);
            }
            return result;
        }

        //--------- Zipping methods ---------
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        private static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        private static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}

