
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toolbox.DiscordUtilities;

namespace Toolbox.Exceptions
{
    public class ExceptionManager
    {
        public static async Task LogExceptionAsync(string message)
        {
            try
            {
                HandleExceptionHelper(new Exception(), message);
            }
            catch (Exception ex)
            {
                await LogExceptionAsync(message);
            }
        }

        public static void HandleException(string message)
        {
            HandleExceptionHelper(new Exception(), message);
        }

        public static void HandleException(string message, Exception ex)
        {
            HandleExceptionHelper(ex, message);
        }

        public static void HandleException(string message, Exception ex, params string[] extraParameters)
        {
            foreach (string extraParameter in extraParameters)
            {
                message = message + " " + extraParameter;
            }
            HandleExceptionHelper(ex, message);
        }

        public static async Task LogExceptionAsync(Exception ex)
        {
            await DiscordContext.LogError(ex.Message);
        }

        private static void HandleExceptionHelper(Exception ex, string message)
        {
            ex = GetInnermostException(ex);

            if (ex.GetType() == typeof(SeymourException))
            {
                throw ex;
            }
            else
            {
                throw new SeymourException(message + " Caused by " + ex.Message);
            }
        }

        private static Exception GetInnermostException(Exception ex)
        {
            Exception result = ex;

            while (result.InnerException != null)
            {
                result = result.InnerException;
            }

            return result;

        }
    }
}
