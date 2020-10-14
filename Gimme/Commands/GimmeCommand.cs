using System.Threading.Tasks;
using Gimme.Extensions;
using Gimme.Services;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme.Commands
{

    [Command(
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
        Subcommand(typeof(Commands.InitializeCommand)),
        Subcommand(typeof(Commands.GeneratorCommand))
    ]
    public class GimmeCommand
    {
        private readonly IFileSystemService fileSystemService;

        public GimmeCommand(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public async Task OnExecute(CommandLineApplication app, IConsole console)
        {
            if (!fileSystemService.FileExists(Constants.GIMME_SETTINGS_FILENAME))
            {
                await AskUserToInitialize(app, console);
            }
            else
            {
                app.ShowHelp();
            }
        }

        private static async Task AskUserToInitialize(CommandLineApplication app, IConsole console)
        {
            var initializeGimme = Prompt.GetYesNo(
                @" 🧐 Gimme has not been initialized in this directory. Do you want to run `init` here?",
                defaultAnswer: false,
                GimmeConsoleExtensions.GetTextColor(console, TextColor.Info)
                );

            if (initializeGimme)
            {
                await app.ExecuteAsync(new string[] { InitializeCommand.NAME });
            }
        }
    }
}
