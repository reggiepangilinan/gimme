using System.Linq;
using LanguageExt;
using LanguageExt.Common;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme
{
    public static class StartUpLogs {
        private static Lst<(string, Option<CommandLineApplication>, Lst<Error>)>  _buildGeneratorResults;

        public static bool NoErrors => _buildGeneratorResults.Length() <= 0 || _buildGeneratorResults.Map(x=> x.Item3).SelectMany(y=> y).Count() <= 0;

        public static Lst<(string, Option<CommandLineApplication>, Lst<Error>)> Result { get => _buildGeneratorResults; }

        public static void Set(Lst<(string, Option<CommandLineApplication>, Lst<Error>)> buildGeneratorResults)
        {
            _buildGeneratorResults = buildGeneratorResults;
        }
    }
}