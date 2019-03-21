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
    public class ConfigManager
    {
        private Config configuration;
        private Config userSettings;
        private const string autoSavePath = @"Config\";

        public ConfigManager()
        {
            Refresh();
        }

        /// <summary>
        /// Re-Load the config from the file system
        /// </summary>
        public void Refresh()
        {
            configuration = new Config();
            configuration.Name = ConfigInitializer.ConfigurationName;
            configuration = LoadConfig();
            if (configuration == null)
            {
                configuration = ConfigInitializer.InitConfiguration();
                SaveConfig();
                ExceptionManager.ThrowException("0201");
            }

            userSettings = new Config();
            userSettings.Name = ConfigInitializer.SettingsName;
            userSettings = LoadSettings();
            if (userSettings == null)
            {
                userSettings = ConfigInitializer.InitSettings();
                SaveSettings();
                ExceptionManager.ThrowException("0202");
            }
        }

        public string GetProperty(PropertyItem item)
        {
            return configuration.Get(item);
        }

        public void SetProperty(PropertyItem item, string value)
        {
            configuration.Set(item, value);
        }

        public void SaveConfig()
        {
            FileIO.SaveXML<Config>(configuration, BuildPath(configuration));
        }

        private Config LoadConfig()
        {
            return FileIO.LoadXML<Config>(BuildPath(configuration));
        }

        public void SaveSettings()
        {
            FileIO.SaveJSON<Config>(userSettings, BuildPath(userSettings));
        }

        private Config LoadSettings()
        {
            return FileIO.LoadJSON<Config>(BuildPath(userSettings));
        }

        public bool GetBooleanProperty(PropertyItem item)
        {
            string temp = configuration.Get(item);
            bool result;
            if (bool.TryParse(temp, out result))
            {
                return result;
            }
            else return false;
        }

        public int GetIntegerProperty(PropertyItem item)
        {
            string temp = configuration.Get(item);
            int result;
            if (int.TryParse(temp, out result))
            {
                return result;
            }
            else return 0;
        }

        private string BuildPath(Config config)
        {
            return autoSavePath + config.Name;
        }

    }
}
