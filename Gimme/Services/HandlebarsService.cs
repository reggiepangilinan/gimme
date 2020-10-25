using System.Collections.Generic;
using HandlebarsDotNet;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Gimme.Services
{
    public class HandlebarsService : ITemplatingService
    {
        public Try<string> TryToMerge(IDictionary<string, string> data, string templateContent)
            => Try(()=> Handlebars.Compile(templateContent)(data));
    }
}
