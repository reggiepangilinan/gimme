using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Commands;
using Gimme.Extensions;
using Gimme.Services;
using LanguageExt;
using LanguageExt.Common;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace Gimme
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            // Register services 
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFileSystemService, FileSystemService>();
            services.AddSingleton<ICommandBuilderService, CommandBuilderService>();
            services.AddSingleton<IGeneratorCommandService, GeneratorCommandService>();
            services.AddSingleton<JsonSerializerOptions>(_ => new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            });
            ServiceProvider servideProvider = services.BuildServiceProvider();

            // Initialize the main command line application
            var app = new CommandLineApplication<GimmeCommand>();
            app.Conventions.UseDefaultConventions()
                           .UseConstructorInjection(servideProvider);

            // Read gimmeSettings
            var console = servideProvider.GetService<IConsole>() ?? PhysicalConsole.Singleton;
            var fileSystemService = servideProvider.GetService<IFileSystemService>();
            var generatorCommandService = servideProvider.GetService<IGeneratorCommandService>();

            var buildGeneratorResult = fileSystemService.GetCurrentGimmeSettings()
                            .Some(settings =>
                                    generatorCommandService.GetGenerators(settings)
                                    .Map(generator => Try(() => generatorCommandService.BuildCommand(generator)))
                                    .Succs()
                                    .Freeze()
                            ).None(() => Lst<(string, Option<CommandLineApplication>, Lst<Error>)>.Empty);

            buildGeneratorResult.Map(x=> 
                                        x.Map(y=> 
                                                y.Item2.Map(app.AddGimmeSubcommand)
                                              )
                                    );           

            StartUpLogs.Set(buildGeneratorResult); 

            await ExecuteCommandAsync(args, app, console);
        }

        private static async Task ExecuteCommandAsync(string[] args, CommandLineApplication<GimmeCommand> app, IConsole console)
        {
            try
            {
                // Run application                          
                await app.ExecuteAsync(args);
            }
            catch (CommandParsingException ex)
            {
                console.WriteLineError(ex.Message);

                if (ex is UnrecognizedCommandParsingException uex && uex.NearestMatches.Any())
                {
                    console.WriteLineInfo("Did you mean this?");
                    console.WriteLineInfo("    " + uex.NearestMatches.First());
                }
            }
        }
    }


}
