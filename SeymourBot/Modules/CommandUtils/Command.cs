using System;
using System.Linq;
using Toolbox.Exceptions;
using Toolbox.Resources;

namespace SeymourBot.Modules.CommandUtils
{
    class Command
    {

        public readonly string CommandName;
        public readonly string CommandContent;
        public readonly string[] CommandParameters;
        public readonly bool Error;

        public Command(string userInput)
        {
            Error = false;
            try
            {
                string[] splitInput = userInput.Split(' ');
                CommandName = splitInput[0];
                CommandContent = String.Join(' ', splitInput.Skip(0));
                CommandParameters = CommandContent.Split(' ');
            }
            catch (Exception e)
            {
                ExceptionManager.HandleException(ErrMessages.CommandParsingException, e);
                Error = true;
            }
        }

    }
}
