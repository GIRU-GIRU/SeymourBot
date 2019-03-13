
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
        public static T LoadNoDialog<T>(string path)
        {
            T result = default(T);
            if (path != null)
            {
                path = path + ".xml";
                if (File.Exists(path)) {
                    using (var stream = File.Open(path, FileMode.OpenOrCreate))
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        try
                        {
                            result = (T)serializer.Deserialize(stream);
                        }
                        catch (InvalidCastException)
                        {
                            Console.WriteLine("LoadNoDialog InvalidCastException");
                        }
                    }
                }
            }
            return result;
        }

        public static void SaveNoDialog<T>(T target, string path)
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
    }
}
