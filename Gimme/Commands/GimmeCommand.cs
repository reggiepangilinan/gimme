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
        Subcommand(typeof(Commands.InitializeCommand))
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
            if(!fileSystemService.FileExists(Constants.GIMME_SETTINGS_FILENAME)) {
                
                var initializeGimme = Prompt.GetYesNo(
                    @" 🧐 Gimme has not been initialized in this directory. Do you want to run `init` here?", 
                    defaultAnswer: false,
                    GimmeConsoleExtensions.GetTextColor(console, TextColor.Info)
                    );

                    if(initializeGimme) {
                        await app.ExecuteAsync(new string[] {InitializeCommand.NAME });
                    }
            }

            // var generator1 = new GeneratorModel()
            // {
            //     Name = "sample",
            //     Prompts = new List<PromptModel>() {
            //       {
            //           new PromptModel() {
            //             Name = "prompt name 1",
            //           }
            //       }
            //     },
            //     Actions = new List<ActionModel>() {
            //       {
            //           new ActionModel() {
            //             Name = "action name 1",
            //           }
            //       }
            //     }
            // };

            // var settingsModel = new GimmeSettingsModel()
            // {
            //     GeneratorsFiles = new List<string>() {
            //         ""
            //     }
            // };

            // var settingsJSONString = JsonSerializer.Serialize<GimmeSettingsModel>(settingsModel, new JsonSerializerOptions() {
            //     WriteIndented = true
            // });

            // fileSystemService.WriteAllTextToFile(Constants.GIMME_SETTINGS_FILENAME,settingsJSONString);
            
            // console.WriteLine("Hello from Gimme!");
            // console.WriteLine("GimmeSettings found " + gimmeSettingsExists);
        }
    }
}
