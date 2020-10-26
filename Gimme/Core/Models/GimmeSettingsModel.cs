using System.Collections.Generic;

namespace Gimme.Core.Models
{
    public class GimmeSettingsModel
    {
        public IEnumerable<string> GeneratorsFiles { get; set; }

        public IDictionary<string, string> Variables {get;set;}
    }
}
