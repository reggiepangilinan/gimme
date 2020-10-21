using System;
using System.Collections.Generic;

namespace Gimme.Core.Models
{
    public class GimmeSettingsModel
    {
        public IEnumerable<string> GeneratorsFiles { get; set; }
    }

    public class GeneratorModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<OptionModel> Options { get; set; }
        public IEnumerable<ActionModel> Actions { get; set; }
    }

    public class OptionModel
    {
        public string Template { get; set; }
        public string Description { get; set; }
    }

    public class ActionModel
    {
        public string Name { get; set; }
    }
}
