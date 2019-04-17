using System;

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
