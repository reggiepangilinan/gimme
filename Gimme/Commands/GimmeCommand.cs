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

        "),
    ]
    public class GimmeCommand
    {
        private readonly IFileSystemService fileSystemService;

        public GimmeCommand(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }

        public void OnExecute(CommandLineApplication app, IConsole console)
        {

            var gimmeSettingsExists = fileSystemService.FileExists(Constants.GIMME_SETTINGS_FILENAME);

            console.WriteLine("Hello from Gimme!");
            console.WriteLine("GimmeSettings found " + gimmeSettingsExists);
        }
    }
}
