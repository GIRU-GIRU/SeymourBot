using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Resources
{
    public static class ResourceUtils
    {
        public static string BuildString(string message, params string[] values)
        {
            string[] messageSplit = message.Split('|');
            StringBuilder builder = new StringBuilder();
            int i = 0;
            for (; i < messageSplit.Length && i < values.Length; i++)
            {
                builder.Append(messageSplit[i]).Append(values[i]);
            }
            for (; i < messageSplit.Length; i++)
            {
                builder.Append(messageSplit[i]);
            }
            return builder.ToString();
        }

        public static string GetBotDialogByName(string name)
        {
            return BotDialogs.ResourceManager.GetString(name);
        }
    }
}