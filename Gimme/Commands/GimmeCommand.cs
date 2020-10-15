using System.Threading.Tasks;
using Gimme.Extensions;
using Gimme.Services;
using LanguageExt;
using static LanguageExt.Prelude;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme.Commands {

    [Command (
            Name = "gimme",
            Description =
            @"

         ✨    
 ██████╗ ██╗███╗   ███╗███╗   ███╗███████╗
██╔════╝ ██║████╗ ████║████╗ ████║██╔════╝
██║  ███╗██║██╔████╔██║██╔████╔██║█████╗  
██║   ██║██║██║╚██╔╝██║██║╚██╔╝██║██╔══╝  
╚██████╔╝██║██║ ╚═╝ ██║██║ ╚═╝ ██║███████╗
 ╚═════╝ ╚═╝╚═╝     ╚═╝╚═╝     ╚═╝╚══════╝

The dotnet cli tool that gives you what you want 😎 

        "
        ),
        Subcommand (typeof (Commands.InitializeCommand)),
        Subcommand (typeof (Commands.GeneratorCommand))
    ]
    public class GimmeCommand {
        private readonly IFileSystemService fileSystemService;

        public GimmeCommand (IFileSystemService fileSystemService) {
            this.fileSystemService = fileSystemService;
        }

        public async Task OnExecute (CommandLineApplication app, IConsole console) {
            await fileSystemService.FileExistsValidation (Constants.GIMME_SETTINGS_FILENAME)
                .MatchAsync (
                    FailAsync: async _ => await AskUserToInitialize (app, console),
                    SuccAsync : async _ => await Task.Run (() => app.ShowHelp ()).ToUnit ());

        }

        private static async Task<Unit> AskUserToInitialize (CommandLineApplication app, IConsole console) => Prompt.GetYesNo (
                @" 🧐 Gimme has not been initialized in this directory. Do you want to run `init` here?",
                defaultAnswer : false,
                GimmeConsoleExtensions.GetTextColor (console, TextColor.Info)
            ) ?
            await app.ExecuteAsync (new string[] { InitializeCommand.NAME }).ToUnit () :
            unit;
    }
}