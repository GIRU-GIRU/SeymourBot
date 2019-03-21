﻿using System;
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
            {"0501", "Error in Info Command"}
        };
    }
}
