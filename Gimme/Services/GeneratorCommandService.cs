using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Gimme.Commands;
using Gimme.Core.Extensions;
using Gimme.Core.Models;
using LanguageExt;
using LanguageExt.Common;
using McMaster.Extensions.CommandLineUtils;
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
        public Lst<GeneratorModel> GetGenerators(GimmeSettingsModel fromSettings)
         => fromSettings.GeneratorsFiles
                        .Map(fileSystemService.TryToReadAllText)
                        .BindT(fileSystemService.TryToDeserialize<GeneratorModel>)
                        .Succs()
                        .Freeze();

        public (string generatorName, Option<CommandLineApplication> cli, Lst<Error> errors) BuildCommand(GeneratorModel generator)
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

            toList(generator.Options ?? Lst<OptionModel>.Empty)
                .Map(ToCommandOption)
                .Map(command.Options.AddCommandOption);

            // var subjectOption = new CommandOption("-s|--subject", CommandOptionType.SingleValue);
            // subjectOption.IsRequired(allowEmptyStrings: false, $"{subjectOption.LongName} is required.");
            // subjectOption.Description = "Some option";
            // dynamicSubCommand.Options.Add(subjectOption);
            command.OnExecute(() =>
                GeneratorCommandHandler
                    .Execute(
                        new Dictionary<string, string>(),
                        templatingService,
                        PhysicalConsole.Singleton)
            );

            return (generatorName, Option<CommandLineApplication>.Some(command), Lst<Error>.Empty);
        }

        private static CommandOption ToCommandOption(OptionModel optionModel)
        {
            var option = new CommandOption(optionModel.Template, CommandOptionType.SingleValue);
            option.Description = optionModel.Description;

            //TODO: Add valiators here
            return option;
        }
    }

    public interface IGeneratorCommandService
    {
        Lst<GeneratorModel> GetGenerators(GimmeSettingsModel fromSettings);
        (string generatorName, Option<CommandLineApplication> cli, Lst<Error> errors) BuildCommand(GeneratorModel generator);
    }
}