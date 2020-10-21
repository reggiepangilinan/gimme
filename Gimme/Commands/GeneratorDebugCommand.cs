using Gimme.Extensions;
using LanguageExt;
using static LanguageExt.Prelude;
using McMaster.Extensions.CommandLineUtils;

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
        {
            if (StartUpLogs.NoErrors)
            {
                console.WriteLineSuccess("✅ No errors detected when trying to building your generators.");
            }
            else
            {
                StartUpLogs.Result.Map(x =>
                {
                    if (x.Item3.IsEmpty && x.Item2.IsSome)
                    {
                        console.WriteLineSuccess($"✅ Generator {x.Item1} loaded");
                    }
                    else
                    {
                        console.WriteLineInfo($"❗️ Generator {x.Item1} has validation errors");
                        x.Item3.Map(y => console.WriteLineWarning("  - " + y.Message));
                    }
                    return unit;
                });
            }
        }
    }
}
