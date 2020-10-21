using Gimme.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme.Extensions
{
    public static class CommanLineUtilsExtensions
    {
        public static CommandOption AddCommandOption(this System.Collections.Generic.List<CommandOption> options, CommandOption option)
        {
            options.Add(option);
            return option;
        }

        public static CommandLineApplication AddGimmeSubcommand(this CommandLineApplication<GimmeCommand> command, CommandLineApplication subCommand)
        {
            command.AddSubcommand(subCommand);
            return subCommand;
        }
    }
}
