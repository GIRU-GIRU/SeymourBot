using SeymourBot.DiscordUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.Exceptions
{
    class ExceptionManager
    {
        public static void HandleException(string code, Exception ex)
        {
            string message = GetMessage(code);
            HandleExceptionHelper(ex, message);
        }

        public static void HandleException(string code, Exception ex, params string[] extraParameters)
        {
            string message = GetMessage(code);
            foreach (string extraParameter in extraParameters)
            {
                message = message + " " + extraParameter;
            }
            HandleExceptionHelper(ex, message);
        }

        public static async void LogExceptionAsync(Exception ex)
        {
            await DiscordContext.LogError(ex.Message);
        }

        private static void HandleExceptionHelper(Exception ex, string message)
        {
            if (ex.GetType() == typeof(SeymourException))
            {
                throw ex;
            }
            else
            {
                throw new SeymourException(message + " Caused by " + ex.Message);
            }
        }

        private static string GetMessage(string code)
        {
            string message;
            if (!ExceptionMessages.Messages.TryGetValue(code, out message))
            {
                message = "Unknown Exception";
            }
            return message;
        }
    }
}
