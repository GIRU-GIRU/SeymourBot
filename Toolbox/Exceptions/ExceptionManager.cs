﻿
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            catch (Exception e)
            {
                await DiscordContextOverseer.LogErrorAsync(e.Message);
            }
        }

        public static async Task LogExceptionAsync(string message, Exception ex)
        {
            try
            {
                HandleExceptionHelper(ex, message);
            }
            catch (Exception e)
            {
                await DiscordContextOverseer.LogErrorAsync(e.Message);
            }
        }

        public static void HandleException(string message)
        {
            HandleExceptionHelper(new Exception(), message);
        }


        public static void HandleExceptionOld(string message, Exception ex)
        {
            HandleExceptionHelper(ex, message);
        }
        public static void HandleException(string message, Exception ex)
        {
            try
            {
                var innermostExceptionMessage = GetInnermostException(ex).Message;

                DiscordContextSeymour.GetLoggingChannel().SendMessageAsync($"Error in {message} - {innermostExceptionMessage}");
            }
            catch (Exception e)
            {
                throw e;
            }

           
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
            await DiscordContextSeymour.LogError(ex.Message);
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

        //private static Exception GetInnermostException(Exception ex)
        //{
        //    Exception result = ex;

        //    for (int i = 0; i < 10; i++)
        //    {
        //        if (result.InnerException != null)
        //        {
        //            result = result.InnerException;
        //        }
        //    }

        //    return result;

        //}

        //format (GetType().FullName, ExceptionHandler.GetAsyncMethodName(), ex)

        public static Exception GetInnermostException(Exception ex)
        {

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }
        public static string GetAsyncMethodName([CallerMemberName]string name = "unknown") => name;

    }
}
