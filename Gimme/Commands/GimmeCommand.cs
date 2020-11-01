using System.Threading.Tasks;
using Gimme.Services;
using LanguageExt;
using static LanguageExt.Prelude;
using McMaster.Extensions.CommandLineUtils;
using Gimme.Core.Extensions;

namespace Gimme.Commands
{

    [Command(
            Name = GimmeCommand.NAME,
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
        Subcommand(typeof(Commands.GeneratorCommand)),
        Subcommand(typeof(GeneratorDebugCommand))
    ]
    public class GimmeCommand
    {
        public const string NAME = "gimme";
        
        private readonly IFileSystemService fileSystemService;

        public GimmeCommand(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public void OnExecute(CommandLineApplication app, IConsole console)
            => fileSystemService.GetCurrentGimmeSettings()
                    .Match(
                                 None: () => AskUserToInitialize(app, console),
                                 Some: _ =>  {
                                                app.ShowHelp();
                                             }
                                );
    
        private Unit Execute(CommandLineApplication app) {
            app.Execute(new string[] { InitializeCommand.NAME });
            return unit;
        }

        public Unit AskUserToInitialize(CommandLineApplication app, IConsole console) => Prompt.GetYesNo(
                @" 🧐 Gimme has not been initialized in this directory. Do you want to run `init` here?",
                defaultAnswer: false,
                GimmeConsoleExtensions.GetTextColor(console, ConsoleTextColor.Info)
            ) ?
            Execute(app) :
            unit;
    }
}