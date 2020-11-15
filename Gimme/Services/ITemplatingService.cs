using System.Collections.Generic;
using LanguageExt;

namespace Gimme.Services
{
    public interface ITemplatingService
    {
        Try<string> TryToMerge(IDictionary<string, string> data, string templateContent);
    }
}
