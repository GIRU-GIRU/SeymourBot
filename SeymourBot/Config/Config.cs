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

        /// <summary>
        /// Creates a new config with default values.
        /// </summary>
        public void Init()
        {
            properties = new List<Property>();
            Property property;

            foreach (var item in ConfigItems.DefaultConfigItems)
            {
                property = new Property
                {
                    Item = item.Key,
                    Value = item.Value
                };
                properties.Add(property);
            }
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
}
