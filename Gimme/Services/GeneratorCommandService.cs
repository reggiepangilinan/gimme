using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gimme.Core.Models;
using Gimme.Extensions;
using LanguageExt;
using McMaster.Extensions.CommandLineUtils;
using static LanguageExt.Prelude;

namespace Gimme.Services
{
    public class GeneratorCommandService : IGeneratorCommandService
    {
        private readonly IFileSystemService fileSystemService;
        private readonly IConsole console;

        public GeneratorCommandService(IFileSystemService fileSystemService, IConsole console)
        {
            this.fileSystemService = fileSystemService;
            this.console = console;
        }
        public Lst<GeneratorModel> GetGenerators(GimmeSettingsModel fromSettings)
         => fromSettings.GeneratorsFiles
                        .Map(fileSystemService.TryToReadAllText)
                        .BindT(fileSystemService.TryToDeserialize<GeneratorModel>)
                        .Succs()
                        .Freeze();

        public CommandLineApplication BuildCommand(GeneratorModel generator)
        {
            // Dynamically build sub commands
            var command = new CommandLineApplication();
            command.HelpOption();
            command.Name = generator.Name;
            command.Description = generator.Description;

            toList(generator.Options)
            .Map(ToCommandOption)
            .Map(command.Options.AddCommandOption);

            // var subjectOption = new CommandOption("-s|--subject", CommandOptionType.SingleValue);
            // subjectOption.IsRequired(allowEmptyStrings: false, $"{subjectOption.LongName} is required.");
            // subjectOption.Description = "Some option";
            // dynamicSubCommand.Options.Add(subjectOption);
            command.OnExecuteAsync(cancellationToken =>
            {
                return Task.Run(() => console.WriteLine(@"Hello from dynamic command 👋"));
            });
            return command;
        }

        private static CommandOption ToCommandOption(OptionModel optionModel)
        {
            var option = new CommandOption(optionModel.Template, CommandOptionType.SingleValue);
            option.Description = optionModel.Description;
            return option;
        }
    }

    public interface IGeneratorCommandService
    {
        Lst<GeneratorModel> GetGenerators(GimmeSettingsModel fromSettings);
        CommandLineApplication BuildCommand(GeneratorModel generator);
    }
}