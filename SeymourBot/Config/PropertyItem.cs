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
        CommandPrefix,

        //non-sensible user settings
        Guild_Mordhau,
        Role_Muted,
        Role_LimitedUser,
        Role_ContentCreator,
        Role_Private,
        Role_Moderator,
        Role_Developer,
        Channel_Main,
        Channel_Logging
    }
}
