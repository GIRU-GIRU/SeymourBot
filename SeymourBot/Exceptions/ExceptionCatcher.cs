using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeymourBot.Exceptions
{
    public class CommandErrorResult : RuntimeResult
    {
        public CommandErrorResult(CommandError? error, string reason) : base(error, reason)
        {
             
        }

        public static CommandErrorResult FromError(string reason) =>
       new CommandErrorResult(CommandError.Unsuccessful, reason);

        public static CommandErrorResult FromSuccess(string reason = null) =>
            new CommandErrorResult(null, reason);
    }
}
