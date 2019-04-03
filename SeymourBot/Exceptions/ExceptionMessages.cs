using SeymourBot.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.Exceptions
{
    /// <summary>
    /// Used to regroup all error messages
    /// </summary>
    //Error codes are build with two digits for the component and two digits for the error code
    //example : 0201 is affecting 02 (the config system) and represent the 01 exception (configuration not found).
    class ExceptionMessages
    {
        private static readonly string UserDBPath = ConfigManager.GetSeymourUserDBPath();
        private static readonly string InfoDBPath = ConfigManager.GetSeymourInfoDBPath();

        public readonly static IDictionary<string, string> Messages = new Dictionary<string, string>()
        {
            //01 Program exceptions

            //02 ConfigManager exceptions
            {"0201", "Failed to load Configuration, a default configuration file has been generated."},
            {"0202", "Failed to load User Settings, a default configuration file has been generated."},

            //03 FileIO exceptions
            {"0301", "Error Loading XML File"},
            {"0302", "Error Saving XML File"},
            {"0303", "Error Loading JSON File"},
            {"0304", "Error Saving JSON File"},

            //04 CommandParsing exceptions
            {"0401", "Error Parsing Command"},

            //05 Info command exceptions
            {"0501", "Error in Info Command"},

            //06 Storage Exception
            {"0601",  $"Error creating new info command, ensure the DB path is {UserDBPath}" },
            {"0602", $"Error getting info command, ensure the DB path is {InfoDBPath}"},
            {"0603", $"Error getting info command list, ensure the DB path is {InfoDBPath}" },
            {"0604", $"Error getting timed events, ensure the DB path is {UserDBPath}" },
            {"0605", $"Error saving timed event, ensure the DB path is {UserDBPath}" },
            {"0606", $"Error archiving timed event, ensure the DB path is {UserDBPath}" },
            {"0607", $"Could not find the timed event to archive, ensure the DB path is {UserDBPath}" },

            //07 DiscordContext exceptions
            {"0701", "Error getting role from Discord" },

            //08 SeymourInitilization exceptions
            {"0801", "Failed to login to Discord" }
        };
    }
}
