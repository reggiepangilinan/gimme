using System.Linq;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme
{
    public static class StartUpLogs
    {
        private static Lst<(string, Option<CommandLineApplication>, Lst<Error>)> _buildGeneratorResults;

        public static Option<Lst<(string generatorName, Option<CommandLineApplication> cli, Lst<Error> errors)>> BuildGeneratorResults 
        { 
            get => 
            _buildGeneratorResults.Length() <= 0
            ? None 
            : Some(_buildGeneratorResults); 
        }

        public static void Set(Lst<(string, Option<CommandLineApplication>, Lst<Error>)> buildGeneratorResults)
        {
            _buildGeneratorResults = buildGeneratorResults;
        }
    }
}