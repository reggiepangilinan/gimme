using System;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Commands;
using Gimme.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Gimme
{
    public class Program
    {
        static async Task Main(string[] args)
        {

           // Register services 
           ServiceCollection services = new ServiceCollection();
           services.AddSingleton<IFileSystemService,FileSystemService>();
           services.AddSingleton<JsonSerializerOptions>( _ => new JsonSerializerOptions(){
               PropertyNameCaseInsensitive = false,
               ReadCommentHandling = JsonCommentHandling.Skip,
               WriteIndented = true
           });
           ServiceProvider servideProvider = services.BuildServiceProvider();

           // Initialize the main command line application
           var app = new CommandLineApplication<GimmeCommand>();
           app.Conventions.UseDefaultConventions()
                          .UseConstructorInjection(servideProvider);;

            // Read gimmeSettings
            var fileSystemService = servideProvider.GetService<IFileSystemService>();

            // Dynamically build sub commands
            var dynamicSubCommand = new CommandLineApplication();
            dynamicSubCommand.HelpOption();
            dynamicSubCommand.Name = "dynamic";
            dynamicSubCommand.Description = "⚡️ Some description about the command";

            var subjectOption = new CommandOption("-s|--subject",CommandOptionType.SingleValue);
            subjectOption.IsRequired(allowEmptyStrings: false, $"{subjectOption.LongName} is required.");
            subjectOption.Description = "Some option";

            dynamicSubCommand.Options.Add(subjectOption);
            dynamicSubCommand.OnExecuteAsync(cancellationToken => {
                return Task.Run(()=> Console.WriteLine(@"Hello from dynamic command 👋")); 
            });
            app.AddSubcommand(dynamicSubCommand);

           // Run application                          
           await app.ExecuteAsync(args);
        }
    }


}
