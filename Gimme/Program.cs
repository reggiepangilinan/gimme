using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Commands;
using Gimme.Core.Extensions;
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
            //Set console output to UTF8 for emojiness.
            Console.OutputEncoding = Encoding.UTF8;

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
            var buildGeneratorResult = BuildGenerator(app, fileSystemService, generatorCommandService);

            StartUpLogs.Set(buildGeneratorResult);
            await ExecuteCommandAsync(args, app, console);
        }

        private static Lst<(string, Option<CommandLineApplication>, Lst<Error>)> BuildGenerator(CommandLineApplication<GimmeCommand> app, IFileSystemService fileSystemService, IGeneratorCommandService generatorCommandService)
        {
            var buildGeneratorResult = fileSystemService.GetCurrentGimmeSettings()
                            .Some(settings =>
                                    generatorCommandService.GetGenerators(settings)
                                    .Map(generator => Try(() => generatorCommandService.BuildCommand(generator)))
                                    .Succs()
                                    .Freeze()
                            ).None(() => Lst<(string, Option<CommandLineApplication>, Lst<Error>)>.Empty);

            buildGeneratorResult.Map(x =>
                                        x.Map(y =>
                                                y.Item2.Map(app.AddGimmeSubcommand)
                                              )
                                    );
            return buildGeneratorResult;
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
