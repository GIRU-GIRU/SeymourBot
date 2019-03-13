using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeymourBot.Config
{
    /// <summary>
    /// The enum listing the names of properties stored in the configuration.
    /// </summary>
    /// 
    public enum PropertyItem
    {
        None,
        BotToken,
        MessageCacheSize,
        AuditChannel,
        Mordhau_General,
        CommandPrefix
    }

    public static class ConfigItems
    {
        public readonly static IDictionary<PropertyItem, string> DefaultConfigItems = new Dictionary<PropertyItem, string>()
        {

        };
    }

}
