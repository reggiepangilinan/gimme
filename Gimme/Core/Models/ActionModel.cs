using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FluentValidation;

namespace Gimme.Core.Models
{
    public class ActionModel
    {
        public string Name { get; set; }
        public ActionType Type { get; set; } = ActionType.Add;
        public string Path { get; set; }
        public string TemplateFile { get; set; }
           
    }

    [JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumMemberConverter))]
    public enum ActionType {
        [EnumMember(Value = nameof(ActionType.Add))]
        Add
    }

    public class ActionModelValidator : AbstractValidator<ActionModel>
    {
        public ActionModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithName("Action.Name");
        }
    }
}
