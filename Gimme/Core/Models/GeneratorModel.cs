using System.Collections.Generic;
using FluentValidation;

namespace Gimme.Core.Models
{
    public class GeneratorModel
    {
        public string Name { get; set; }
        public string Description { get; set; }    
        public IEnumerable<OptionModel> Options { get; set; }

        public IEnumerable<ActionModel> Actions { get; set; }
    }

    public class GeneratorModelValidator : AbstractValidator<GeneratorModel> {
        public GeneratorModelValidator()
        {
            RuleFor(x=> x.Name).NotEmpty();
            RuleFor(x=> x.Description).NotEmpty();
            RuleForEach(x=> x.Options).SetValidator(new OptionModelValidator());
            RuleFor(x=> x.Actions).NotEmpty();
        }
    }
}
