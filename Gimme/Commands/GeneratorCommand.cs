using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LanguageExt;
using static LanguageExt.Prelude;
using Gimme.Extensions;
using static Gimme.Extensions.All;
using Gimme.Models;
using Gimme.Services;
using Gimme.Validations;
using McMaster.Extensions.CommandLineUtils;
using System.Linq;
using LanguageExt.Common;

namespace Gimme.Commands
{
    [Command
        (
            Name = GeneratorCommand.NAME,
            Description = @"✅ Gives you a new generator.{name}.json file in the currrent directory"
        ),
    ]
    [GimmeSettingsFileMustExistsValidation()]
    public class GeneratorCommand
    {
        public const string NAME = "generator";

        [Required]
        [Option(Description = "Required. The name of the generator")]
        public string Name { get; }

        private readonly IFileSystemService fileSystemService;

        public GeneratorCommand(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public void OnExecute(CommandLineApplication app, IConsole console)
        {
            var withThisGeneratorFilename = $"generator.{Name.ToLower()}.json";
            (
                from withCurrentGimmeSettings in fileSystemService.GetCurrentGimmeSettings().MustExists()
                from withNewGeneratorModel in NewGeneratorMustNotExists(withThisGeneratorFilename)
                from messageNewGenerator in CreateGeneratorFile(withThisGeneratorFilename, withNewGeneratorModel)
                from messageUpdateSettings in UpdateGimmeSettingsGeneratorFiles(withCurrentGimmeSettings, withThisGeneratorFilename)
                select List(messageNewGenerator, messageUpdateSettings)
            )
            .ResultTo(console);
        }

        private Validation<Error, string> CreateGeneratorFile(string newGeneratorFilename, GeneratorModel newGenerator)
        => fileSystemService.TryToSerialize<GeneratorModel>(fromValue: newGenerator)
                             .Match(
                                 Succ: fileTextContent => fileSystemService
                                                        .WriteAllTextToFile(newGeneratorFilename, fileTextContent)
                                                        .Map(_ => $"✅ Created generator `{newGeneratorFilename}`"),
                                 Fail: e => Left<Error, string>(Error.New(e))
                             ).ToValidation();

        private Validation<Error, string> UpdateGimmeSettingsGeneratorFiles(GimmeSettingsModel currentGimmeSettings, string newGeneratorFilename)
        {
            //TODO: Don't mutate
            currentGimmeSettings.GeneratorsFiles = currentGimmeSettings
                                    .GeneratorsFiles
                                    .Append(new[] { newGeneratorFilename })
                                    .ToList()
                                    .Where(fileSystemService.FileExists)
                                    .Distinct();

            return fileSystemService.TryToSerialize<GimmeSettingsModel>(fromValue: currentGimmeSettings)
                         .Match(
                             Succ: fileTextContent => fileSystemService
                                                    .WriteAllTextToFile(newGeneratorFilename, fileTextContent)
                                                    .Map(_ => $"✅ Updated file `{Constants.GIMME_SETTINGS_FILENAME}`"),
                             Fail: e => Left<Error, string>(Error.New(e))
                         ).ToValidation();
        }

        private Validation<Error, GeneratorModel> NewGeneratorMustNotExists(string newGeneratorFilename)
            => fileSystemService.FileExists(newGeneratorFilename)
                ? Fail<Error, GeneratorModel>(Error.New($"🥶 Generator already exists - `{newGeneratorFilename}`"))
                : Success<Error, GeneratorModel>(
                    new GeneratorModel()
                    {
                        Name = Name.ToLower(),
                        Description = "Your generator description goes here.",
                        Options = new List<OptionModel>()
                    {
                        {
                            new OptionModel()
                            {
                                Name= ""
                            }
                        }
                    },
                        Actions = new List<ActionModel>()
                    {
                        {
                            new ActionModel()
                            {
                                Name= ""
                            }
                        }
                    },
                    });

    }
}
