using SeymourBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeymourBot.Modules.CommandUtils
{
    class Command
    {

        public readonly string CommandName;

        public readonly string CommandContent;

        public readonly bool Error;

        public Command(string userInput)
        {
            Error = false;
            try
            {
                string[] splitInput = userInput.Split(' ');
                CommandName = splitInput[0];
                CommandContent = String.Join(' ', splitInput.Skip(0));
            }
            catch (Exception e)
            {
                ExceptionManager.HandleException("0401", e);
                Error = true;
            }
        }
    }
}
