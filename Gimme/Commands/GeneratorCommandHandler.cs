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
        public static Unit Execute(IDictionary<string, string> variables, ITemplatingService templatingService, IConsole console)
        {
            string templateContent =
@"
public class {{class}} {

}
";
            var data = new Dictionary<string,string>()
            {
                {"class", "OhYeah"}
            };

            return templatingService
                .TryToMerge(data, templateContent)
                .ToValidation()
                .MapFail(ExceptionToError)
                .ResultTo(console)
                .ToUnit();

        }
    }
}