﻿
using Newtonsoft.Json;
using SeymourBot.Exceptions;
using SeymourBot.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace SeymourBot.Config
{
    public class FileIO
    {
        public static T LoadXML<T>(string path)
        {
            T result = default(T);
            try
            {
                if (path != null)
                {
                    path = path + ".xml";
                    Directory.CreateDirectory(path);
                    if (File.Exists(path))
                    {
                        using (var stream = File.Open(path, FileMode.OpenOrCreate))
                        {
                            var serializer = new XmlSerializer(typeof(T));
                            result = (T)serializer.Deserialize(stream);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FileIOException, ex, path);
            }
            return result;
        }

        public static void SaveXML<T>(T target, string path)
        {
            try
            {
                if (path != null)
                {
                    path = path + ".xml";
                    using (var stream = File.Create(path))
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(stream, target);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FileIOException, ex, path);
            }
        }

        public static T LoadJSON<T>(string path)
        {
            T result = default(T);
            try
            {

                if (path != null)
                {
                    path = path + ".json";
                    Directory.CreateDirectory(path.Split('\\')[0]);
                    if (File.Exists(path))
                    {
                        result = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FileIOException, ex, path);
            }
            return result;
        }

        public static void SaveJSON<T>(T target, string path)
        {
            try
            {
                if (path != null)
                {
                    path = path + ".json";
                    using (var stream = File.Create(path))
                    {
                        SerializeToStream(stream, target);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.HandleException(ErrMessages.FileIOException, ex, path);
            }
        }

        private static void SerializeToStream(Stream stream, object target)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(stream))
            using (var jsonTextWriter = new JsonTextWriter(sw))
            {
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(jsonTextWriter, target);
            }
        }
    }
}
