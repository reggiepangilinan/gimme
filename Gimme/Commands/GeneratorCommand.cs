using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Extensions;
using Gimme.Models;
using Gimme.Services;
using Gimme.Validations;
using McMaster.Extensions.CommandLineUtils;

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
            var newGeneratorFilename = $"generator.{Name.ToLower()}.json";

            // Validate if generator already exists
            if (fileSystemService.FileExists(newGeneratorFilename))
            {
                console
                    .AppendEmptyLine()
                    .WriteLineWithColor($"🥶 Generator already exists - `{newGeneratorFilename}`", TextColor.Error)
                    .AppendEmptyLine();
                return;
            }

            var currentGimmeSettings = await fileSystemService.GetCurrentGimmeSettingsAsync();

            // Creates a new generator
            var generatorText = JsonSerializer.Serialize<GeneratorModel>(
                new GeneratorModel()
                {
                    Name = Name.ToLower(),
                    Description = "Your generator description goes here.",
                    Prompts = new List<PromptModel>()
                    {
                        {
                            new PromptModel()
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
                }, jsonSerializerOptions);

            fileSystemService.WriteAllTextToFile(newGeneratorFilename, generatorText);

            console
                 .AppendEmptyLine()
                 .WriteLineWithColor($"✅ Created generator `{newGeneratorFilename}`", TextColor.Success);

            // Update gimmeSettings.json file
            currentGimmeSettings.GeneratorsFiles = currentGimmeSettings
                                                    .GeneratorsFiles
                                                    .Append(new[] { newGeneratorFilename })
                                                    .ToList()
                                                    .Where(x=> fileSystemService.FileExists(x))
                                                    .Distinct();

            var updatedGimmeSettingsText = JsonSerializer.Serialize<GimmeSettingsModel>(currentGimmeSettings, jsonSerializerOptions);

            fileSystemService.WriteAllTextToFile(Constants.GIMME_SETTINGS_FILENAME, updatedGimmeSettingsText);

            console.WriteLineWithColor($"✅ Updated file `{Constants.GIMME_SETTINGS_FILENAME}`", TextColor.Success)
                 .AppendEmptyLine();
        }
    }
}
