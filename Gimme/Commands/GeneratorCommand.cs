using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Extensions;
using Gimme.Models;
using Gimme.Services;
using Gimme.Validations;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Linq;

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
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public GeneratorCommand(IFileSystemService fileSystemService, JsonSerializerOptions jsonSerializerOptions)
        {
            this.fileSystemService = fileSystemService;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task OnExecute(CommandLineApplication app, IConsole console)
        {
            var generatorFilename = $"generator.{Name.ToLower()}.json";
            (
                (await fileSystemService.GetCurrentGimmeSettingsAsync()).MustExists(),
                NewGeneratorMustNotExists(generatorFilename)
            )
            .Apply((item1, item2) => (item1, item2))
            .Map(
                values => List(
                                CreateGeneratorFile(generatorFilename, newGenerator: values.Item2),
                                UpdateGimmeSettingsGeneratorFiles(currentGimmeSettings: values.Item1, generatorFilename)
                              )
            )
            .Match(
                Succ: messages =>
                    messages.Map(m => console.WriteLineWithColor(m, TextColor.Success)
                                            ),
                Fail: errors =>
                    errors.Map(m => console.WriteLineWithColor(m.Value.ToString(), TextColor.Error))
            );
        }

        private string CreateGeneratorFile(string newGeneratorFilename, GeneratorModel newGenerator)
        {
            fileSystemService.WriteAllTextToFile(newGeneratorFilename, JsonSerializer.Serialize<GeneratorModel>(
                newGenerator, jsonSerializerOptions));

            return $"✅ Created generator `{newGeneratorFilename}`";
        }

        private string UpdateGimmeSettingsGeneratorFiles(GimmeSettingsModel currentGimmeSettings, string newGeneratorFilename)
        {
            currentGimmeSettings.GeneratorsFiles = currentGimmeSettings
                                                    .GeneratorsFiles
                                                    .Append(new[] { newGeneratorFilename })
                                                    .ToList()
                                                    .Where(x=> fileSystemService.FileExists(x))
                                                    .Distinct();

            var updatedGimmeSettingsText = JsonSerializer.Serialize<GimmeSettingsModel>(currentGimmeSettings, jsonSerializerOptions);

            fileSystemService.WriteAllTextToFile(Constants.GIMME_SETTINGS_FILENAME, updatedGimmeSettingsText);

            return $"✅ Updated file `{Constants.GIMME_SETTINGS_FILENAME}`";
        }


        private Validation<Error, GeneratorModel> NewGeneratorMustNotExists(string newGeneratorFilename)
            => fileSystemService.FileExists(newGeneratorFilename)
                ? Fail<Error, GeneratorModel>($"🥶 Generator already exists - `{newGeneratorFilename}`")
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
