using System.Collections.Generic;
using Gimme.Core.Extensions;
using Gimme.Core.Models;
using Gimme.Services;
using LanguageExt;
using LanguageExt.Common;
using McMaster.Extensions.CommandLineUtils;
using static LanguageExt.Prelude;

namespace Gimme.Commands
{
    [Command(
        Name = InitializeCommand.NAME,
        Description = @"⚡️ Gives you a new gimmeSettings.json in the currrent directory"
    ),]

    public class InitializeCommand
    {
        public const string NAME = "init";
        private readonly IFileSystemService fileSystemService;

        public InitializeCommand(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        private GimmeSettingsModel DefaultGimmeSettings =>
            new GimmeSettingsModel()
            {
                GeneratorsFiles = new List<string>()
            };

        public void OnExecute(CommandLineApplication app, IConsole console)
            => match(fileSystemService.GetCurrentGimmeSettings(),
                Some: _  => console
                            .WriteLineInfo($"You already have an existing {Constants.GIMME_SETTINGS_FILENAME} file in this directory.")
                            .ToUnit(), 
                None: () =>
                    (
                        from messageInitialized in InitializeGimmeSettings(DefaultGimmeSettings)
                        select messageInitialized
                    )
                    .ResultTo(console)
                    .ToUnit()
                );
        
        private Validation<Error, string> InitializeGimmeSettings(GimmeSettingsModel defaultSettings)
            => fileSystemService.TryToSerialize<GimmeSettingsModel>(fromValue: defaultSettings)
                                .Match(
                                    Succ: fileTextContent => fileSystemService
                                                            .WriteAllTextToFile(Constants.GIMME_SETTINGS_FILENAME, fileTextContent)
                                                            .Map(_ => $"✅ Created file {Constants.GIMME_SETTINGS_FILENAME}"),
                                    Fail: e => Left<Error, string>(Error.New(e))
                                ).ToValidation();

    }
}