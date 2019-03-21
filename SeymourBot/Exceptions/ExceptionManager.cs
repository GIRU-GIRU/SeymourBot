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
            //send a message in the chat
        }

        public static void HandleException(string code, Exception ex, params string[] extraParameters)
        {
            string message = GetMessage(code);
            foreach (string extraParameter in extraParameters)
            {
                message = message + " " + extraParameter;
            }
            //send a message in the chat
        }

        public static void ThrowException(string code)
        {
            throw new Exception(GetMessage(code));
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
