using System.Collections.Generic;
using Gimme.Core.Extensions;
using Gimme.Services;
using static Gimme.Core.Transformers.All;
using HandlebarsDotNet;
using LanguageExt;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme.Commands
{
    public static class GeneratorCommandHandler
    {
        public static Unit Execute(Lst<CommandOption> commandOptions, IDictionary<string, string> variables, ITemplatingService templatingService, IConsole console)
        {
            //Merge command options and variables from settings
            var data = new Dictionary<string, string> (
                                            variables.Append
                                            (
                                                commandOptions
                                                .Map(c => 
                                                        KeyValuePair
                                                        .Create(c.LongName, c.Value()?.ToString())
                                                     )
                                            ));

            
            string templateContent =
@"
public class {{class}} {
{{name}}
}
";

            return templatingService
                .TryToMerge(data, templateContent)
                .ToValidation()
                .MapFail(ExceptionToError)
                .ResultTo(console)
                .ToUnit();

        }
    }
}