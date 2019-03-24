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
        //sensible properties for the bot Configuration
        BotToken,
        MessageCacheSize,
        AuditChannel,
        Mordhau_General,
        CommandPrefix,


        //non-sensible user settings
        MordhauGuild,

    }
}
