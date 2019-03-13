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
        private Config config;
        private const string autoSavePath = "Config";

        public ConfigManager()
        {
            Refresh();
        }

        /// <summary>
        /// Re-Load the config from the file system
        /// </summary>
        public void Refresh()
        {
            config = LoadConfig();
            //if the config file cannot be found, load the default values.
            if (config == null)
            {
                config = new Config();
                config.Init();
            }
        }

        public string GetProperty(PropertyItem item)
        {
            return config.Get(item);
        }

        public void SetProperty(PropertyItem item, string value)
        {
            config.Set(item, value);
        }

        public void SaveConfig()
        {
            FileIO.SaveNoDialog<Config>(config, "Settings");
        }

        private Config LoadConfig()
        {
            return FileIO.LoadNoDialog<Config>("Settings");
        }

        public bool GetBooleanProperty(PropertyItem item)
        {
            string temp = config.Get(item);
            bool result;
            if (bool.TryParse(temp, out result))
            {
                return result;
            }
            else return false;
        }

        public int GetIntegerProperty(PropertyItem item)
        {
            string temp = config.Get(item);
            int result;
            if (int.TryParse(temp, out result))
            {
                return result;
            }
            else return 0;
        }

    }
}
