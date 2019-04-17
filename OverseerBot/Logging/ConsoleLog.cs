using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace OverseerBot.Logging
{
    public static class ConsoleLogger
    {
        public static void LogToConsole(string prefix, Exception ex)
        {
            if (!String.IsNullOrWhiteSpace(ex.Message))
            {
                Console.WriteLine(prefix + ex.Message);
            }
        }
    }
}
