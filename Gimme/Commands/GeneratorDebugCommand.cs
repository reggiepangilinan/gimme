using static LanguageExt.Prelude;
using McMaster.Extensions.CommandLineUtils;
using Gimme.Core.Extensions;

namespace Gimme.Commands
{
    [Command
        (
            Name = GeneratorDebugCommand.NAME,
            Description = @"⚡️ Can't see your generator in the list? Execute this command to see if there are any errors when loading your generator"
        ),
    ]
    public class GeneratorDebugCommand
    {
        public const string NAME = "generator-debug";

        public void OnExecute(CommandLineApplication app, IConsole console)
            =>
            StartUpLogs
            .BuildGeneratorResults
            .Match
            (
                None: () => console.WriteLineInfo("ℹ️  You don't have any generators."),
                Some: results =>
                      results.Map(valueOf =>
                        {
                            if (valueOf.errors.IsEmpty && valueOf.cli.IsSome)
                            {
                                console.WriteLineSuccess($"✅ Generator `{valueOf.generatorName}` loaded");
                            }
                            else
                            {
                                console.WriteLineInfo($"❗️ Generator {valueOf.generatorName} has validation errors");
                                valueOf.Item3.Map(y => console.WriteLineWarning("  - " + y.Message));
                            }
                            return unit;
                        })
            );
    }
}
