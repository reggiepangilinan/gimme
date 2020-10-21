using System.Collections.Generic;
using System.Text.Json;
using Gimme.Core.Models;
using Gimme.Extensions;
using Gimme.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme.Commands
{
    [Command (
        Name = InitializeCommand.NAME,
        Description = @"✅ Gives you a new gimmeSettings.json in the currrent directory"
    ), ]

    public class InitializeCommand {
        public const string NAME = "init";

        private readonly IFileSystemService fileSystemService;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public InitializeCommand (IFileSystemService fileSystemService, JsonSerializerOptions jsonSerializerOptions) {
            this.fileSystemService = fileSystemService;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public void OnExecute (CommandLineApplication app, IConsole console) {
            // Can't initialize twice so this guard should be here.
            if (fileSystemService.FileExists (Constants.GIMME_SETTINGS_FILENAME)) return;

            var settingsText = JsonSerializer.Serialize<GimmeSettingsModel> (
                new GimmeSettingsModel () {
                    GeneratorsFiles = new List<string> ()
                }, jsonSerializerOptions);

            fileSystemService.WriteAllTextToFile (Constants.GIMME_SETTINGS_FILENAME, settingsText);

            console.WriteLineWithColor ($"✅ Created file {Constants.GIMME_SETTINGS_FILENAME}", TextColor.Success);
        }
    }
}