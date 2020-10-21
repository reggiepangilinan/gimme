using FluentValidation;

namespace Gimme.Core.Models
{
    public class ActionModel
    {
        public string Name { get; set; }
    }

    public class ActionModelValidator : AbstractValidator<ActionModel>
    {
        public ActionModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithName("Action.Name");
        }
    }
}
