using FluentValidation;

namespace Gimme.Core.Models
{
    public class OptionModel 
    {
        public string Template { get; set; }
        public string Description { get; set; }
    }

    public class OptionModelValidator : AbstractValidator<OptionModel> {
        public OptionModelValidator()
        {
            RuleFor(x=> x.Template).NotEmpty().WithName("Option.Template");
            RuleFor(x=> x.Description).NotEmpty().WithName("Option.Description");
        }
    }
}
