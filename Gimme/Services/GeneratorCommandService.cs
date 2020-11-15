using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gimme.Commands;
using Gimme.Core.Extensions;
using Gimme.Core.Models;
using LanguageExt;
using LanguageExt.Common;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using static LanguageExt.Prelude;

namespace Gimme.Services
{
    public class GeneratorCommandService : IGeneratorCommandService
    {
        private readonly ITemplatingService templatingService;
        private readonly IFileSystemService fileSystemService;

        public GeneratorCommandService(ITemplatingService templatingService, IFileSystemService fileSystemService)
        {
            this.templatingService = templatingService;
            this.fileSystemService = fileSystemService;
        }
        public Lst<Try<GeneratorModel>> GetGenerators(GimmeSettingsModel fromSettings)
         => fromSettings.GeneratorsFiles
                        .Map(fileSystemService.TryToReadAllText)
                        .BindT(fileSystemService.TryToDeserialize<GeneratorModel>)
                        .Freeze();

        public (string generatorName, Option<CommandLineApplication> cli, Lst<Error> errors) BuildCommand(IDictionary<string, string> variablesFromSettings, GeneratorModel generator)
        {
            string generatorName = string.IsNullOrWhiteSpace(generator.Name) ? "unknown generator" : generator.Name;

            var validator = new GeneratorModelValidator();
            var result = validator.Validate(generator);

            //TODO: Convert to validation monad
            if (!result.IsValid)
            {
                return (generatorName, Option<CommandLineApplication>.None, result.Errors.Map(x => Error.New(x.ErrorMessage)).Freeze());
            }

            // Dynamically build sub commands
            var command = new CommandLineApplication();
            command.HelpOption();
            command.Name = generator.Name;
            command.Description = generator.Description;


            //TODO: Validate template pattern
            var commandOptions = toList(generator.Options ?? Lst<OptionModel>.Empty)
                 .Map(ToCommandOption)
                 .Map(command.Options.AddCommandOption);

            command.OnExecute(() =>
                GeneratorCommandHandler
                    .Execute(
                        commandOptions,
                        variablesFromSettings,
                        templatingService,
                        generator.Actions,
                        PhysicalConsole.Singleton
                        )
            );

            return (generatorName, Option<CommandLineApplication>.Some(command), Lst<Error>.Empty);
        }

        private static CommandOption ToCommandOption(OptionModel optionModel)
        {
            var option = new CommandOption(optionModel.Template, CommandOptionType.SingleValue);
            option.Description = optionModel.Description;

            if (optionModel.Validators.IsRequired)
                option.IsRequired(false, $"{option.LongName} is required.");

            if (optionModel.Validators.IsEmailAddress.GetValueOrDefault())
                option.Validators.Add(new AttributeValidator(new EmailAddressAttribute()));

            if (optionModel.Validators.MinLength.HasValue)
                option.Validators.Add(new AttributeValidator(new MinLengthAttribute(optionModel.Validators.MinLength.GetValueOrDefault())));

            if (optionModel.Validators.MaxLength.HasValue) 
                option.Validators.Add(new AttributeValidator(new MaxLengthAttribute(optionModel.Validators.MaxLength.GetValueOrDefault())));

            if (!string.IsNullOrWhiteSpace(optionModel.Validators.RegEx)) 
                option.Validators.Add(new AttributeValidator(new RegularExpressionAttribute(optionModel.Validators.RegEx)));

            if (optionModel.Validators.AllowedValues.Length() > 0) option.Validators.Add(new AttributeValidator(new AllowedValuesAttribute(optionModel.Validators.AllowedValues.ToArray())));

            return option;
        }
    }

    public interface IGeneratorCommandService
    {
        Lst<Try<GeneratorModel>> GetGenerators(GimmeSettingsModel fromSettings);
        (string generatorName, Option<CommandLineApplication> cli, Lst<Error> errors) BuildCommand(IDictionary<string, string> variables, GeneratorModel generator);
    }
}