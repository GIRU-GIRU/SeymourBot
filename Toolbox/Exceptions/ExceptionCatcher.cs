using Discord.Commands;

namespace Toolbox.Exceptions
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
