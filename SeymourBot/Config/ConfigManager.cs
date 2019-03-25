using SeymourBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeymourBot.Config
{

    /// <summary>
    /// Save, load and store the configuration.
    /// </summary>
    public static class ConfigManager
    {
        private static Config configuration;
        private static Config userSettings;
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
            configuration.Name = ConfigInitializer.ConfigurationName;
            configuration = LoadConfig();
            if (configuration == null)
            {
                configuration = ConfigInitializer.InitConfiguration();
                SaveConfig();
                ExceptionManager.HandleException("0201", new Exception());
            }

            userSettings = new Config();
            userSettings.Name = ConfigInitializer.SettingsName;
            userSettings = LoadSettings();
            if (userSettings == null)
            {
                userSettings = ConfigInitializer.InitSettings();
                SaveSettings();
                ExceptionManager.HandleException("0202", new Exception());
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
            FileIO.SaveXML<Config>(configuration, BuildPath(configuration));
        }

        private static Config LoadConfig()
        {
            return FileIO.LoadXML<Config>(BuildPath(configuration));
        }

        public static void SaveSettings()
        {
            FileIO.SaveJSON<Config>(userSettings, BuildPath(userSettings));
        }

        private static Config LoadSettings()
        {
            return FileIO.LoadJSON<Config>(BuildPath(userSettings));
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

        public static ulong GetUlongUserSetting(PropertyItem item)
        {
            string temp = userSettings.Get(item);
            ulong result;
            if (ulong.TryParse(temp, out result))
            {
                return result;
            }
            else return 0;
        }

        private static string BuildPath(Config config)
        {
            return autoSavePath + config.Name;
        }

    }
}
