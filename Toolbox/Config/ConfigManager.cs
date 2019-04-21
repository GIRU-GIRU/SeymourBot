using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace Toolbox.Config
{

    /// <summary>
    /// Save, load and store the configuration.
    /// </summary>
    public static class ConfigManager
    {
        private static Config configuration;
        private const string autoSavePath = @"Config\";

        static ConfigManager()
        {
            Refresh();
        }

        /// <summary>
        /// Re-Load the config from the file system
        /// </summary>
        public static void Refresh()
        {
            configuration = new Config();
            configuration = LoadConfig();
            if (configuration == null)
            {
                configuration = new Config();
                configuration.Verify();
                SaveConfig();
                _ = ExceptionManager.LogExceptionAsync(ErrMessages.NoConfigException);
            }
            else
            {
                configuration.Verify();
                SaveConfig();
            }
        }

        public static string GetProperty(PropertyItem item)
        {
            return configuration.Get(item);
        }

        public static void SetProperty(PropertyItem item, string value)
        {
            configuration.Set(item, value);
        }

        public static void SaveConfig()
        {
            FileIO.SaveJSON<Config>(configuration, BuildPath(configuration));
        }

        private static Config LoadConfig()
        {
            return FileIO.LoadJSON<Config>(BuildPath(configuration));
        }

        public static bool GetBooleanProperty(PropertyItem item)
        {
            string temp = configuration.Get(item);
            bool result;
            if (bool.TryParse(temp, out result))
            {
                return result;
            }
            else return false;
        }

        public static int GetIntegerProperty(PropertyItem item)
        {
            string temp = configuration.Get(item);
            int result;
            if (int.TryParse(temp, out result))
            {
                return result;
            }
            else return 0;
        }

        public static ulong GetUlongProperty(PropertyItem item)
        {
            string temp = configuration.Get(item);
            ulong result;
            if (ulong.TryParse(temp, out result))
            {
                return result;
            }
            else return 0;
        }
        private static string BuildPath(Config config)
        {
            return autoSavePath + "Configuration";
        }
    }
}
