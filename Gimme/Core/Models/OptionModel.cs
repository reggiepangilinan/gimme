using System.Collections.Generic;
using FluentValidation;

namespace Gimme.Core.Models
{
    public class OptionModel
    {
        public string Template { get; set; }
        public string Description { get; set; }
        public OptionValidatorsModel Validators { get; set; } = new OptionValidatorsModel();
    }

    public class OptionValidatorsModel
    {
        public bool IsRequired { get; set; } = false;
        public List<string> AllowedValues { get; set; } = new List<string>();
        public bool? IsEmailAddress { get; set; } = false;
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string RegEx { get; set; }
    }

    public class OptionModelValidator : AbstractValidator<OptionModel>
    {
        public OptionModelValidator()
        {
            RuleFor(x => x.Template).NotEmpty().WithName("Option.Template");
            RuleFor(x => x.Description).NotEmpty().WithName("Option.Description");
        }
    }
}
