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
            var withThisGeneratorFilename = $"generator.{Name.ToLower()}.json";
            (
                validateGimmeSettings: (await fileSystemService.GetCurrentGimmeSettingsAsync()).MustExists(),
                validateGenerator: NewGeneratorMustNotExists(withThisGeneratorFilename)
            )
            .Apply
            (

                //TODO: Handle exceptions here?
                (currentGimmeSettings, newGeneratorModel)
                 => List(
                           CreateGeneratorFile(withThisGeneratorFilename, newGeneratorModel),
                           UpdateGimmeSettingsGeneratorFiles(currentGimmeSettings, withThisGeneratorFilename)
                         )
            )
            .Match(
                Succ: messages =>
                {
                    messages.Map(m => console.WriteLineWithColor(m, TextColor.Success));
                },
                Fail: errors =>
                {
                    errors.Map(m => console.WriteLineWithColor(m.Value, TextColor.Error));
                }
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
                                                    .Where(fileSystemService.FileExists)
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
