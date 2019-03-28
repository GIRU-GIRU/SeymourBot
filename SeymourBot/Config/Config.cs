using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Config
{
    /// <summary>
    /// The class used by the ConfigManager to store the configuration.
    /// </summary>
    public class Config
    {
        private List<Property> properties;

        public Config()
        {
            properties = new List<Property>();
        }

        public List<Property> Properties { get => properties; set => properties = value; }

        public string Get(PropertyItem item)
        {
            return properties.Find(x => x.Item == item).Value;
        }

        public void Set(PropertyItem item, string value)
        {
            properties.Find(x => x.Item == item).Value = value;
        }

        public List<Property> GetAllProperties()
        {
            return properties;
        }
    }

    public static class ConfigInitializer
    {
        private readonly static IDictionary<PropertyItem, string> DefaultConfiguration = new Dictionary<PropertyItem, string>()
        {
            {PropertyItem.BotToken, ""},
            {PropertyItem.MessageCacheSize, ""},
            {PropertyItem.CommandPrefix, ""},
            {PropertyItem.Guild_Mordhau, ""},
            {PropertyItem.Role_Muted, ""},
            {PropertyItem.Role_LimitedUser, ""},
            {PropertyItem.Role_ContentCreator, ""},
            {PropertyItem.Role_Private, ""},
            {PropertyItem.Role_Moderator, ""},
            {PropertyItem.Role_Developer, ""},
            {PropertyItem.Channel_Logging, ""},
            {PropertyItem.Channel_Main, ""}
        };

        public static Config InitConfiguration()
        {
            Config config = new Config();
            Property property;

            foreach (var item in DefaultConfiguration)
            {
                property = new Property
                {
                    Item = item.Key,
                    Value = item.Value
                };
                config.Properties.Add(property);
            }
            return config;
        }
    }
}
