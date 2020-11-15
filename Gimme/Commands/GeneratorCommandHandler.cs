using System.Collections.Generic;
using Gimme.Core.Extensions;
using Gimme.Services;
using static Gimme.Core.Transformers.All;
using HandlebarsDotNet;
using LanguageExt;
using McMaster.Extensions.CommandLineUtils;
using System.Linq;
using System.Threading.Tasks;

namespace Gimme.Commands
{
    public static class GeneratorCommandHandler
    {
        public static Unit Execute(Lst<CommandOption> commandOptions, IDictionary<string, string> variables, ITemplatingService templatingService, IEnumerable<Core.Models.ActionModel> actions, IConsole console)
        {
            //Merge command options and variables from settings
            var data = new Dictionary<string, string>(
                                            variables.Append(
                                                commandOptions
                                                .Map(c =>
                                                        KeyValuePair
                                                        .Create(c.LongName, c.Value()?.ToString())
                                                     )
                                                )
                                            );

            // Validate each action
            actions.Fold(Lst<string>.Empty,(x,s) => {
                if(s.Type == Core.Models.ActionType.Add) {
                    return x.Add(s.Name);
                }
                return x;
            });

            // Execute each action


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